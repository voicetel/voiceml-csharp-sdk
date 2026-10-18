using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace VoiceML;

/// <summary>Validates and computes <c>X-Twilio-Signature</c> webhook signatures.
/// <para>VoiceML signs every outbound webhook (status callbacks, message DLRs, conversation
/// events) the way this SDK verifies: take the full callback URL with any <c>#fragment</c>
/// stripped, append each <c>key</c> + <c>value</c> pair for every form parameter sorted by key
/// in ascending byte order (repeated keys keep their received order; GET requests sign the URL
/// alone), then HMAC-SHA1 the result with the account's API key and Base64-encode
/// (standard alphabet, padded).</para>
/// <para>Comparison is constant-time. As a compatibility affordance, signatures computed over
/// the URL with its default port explicitly present (<c>:443</c> for https, <c>:80</c> for
/// http) — or with it removed — are also accepted.</para>
/// <para>JSON bodies are not signed by the server, so no <c>bodySHA256</c> variant exists.</para>
/// </summary>
public static class RequestValidator
{
    /// <summary>Validate an <c>X-Twilio-Signature</c> header value for an incoming webhook.
    /// </summary>
    /// <param name="apiKey">The account's API key (the HTTP-Basic password used for the API).</param>
    /// <param name="url">The full URL the webhook was delivered to, as the receiving server saw
    /// it (query string included, fragment excluded).</param>
    /// <param name="formParams">The form parameters received on the request, in received order.
    /// May be empty for GET callbacks.</param>
    /// <param name="signature">The Base64 signature from the <c>X-Twilio-Signature</c> header.</param>
    /// <returns><c>true</c> when the signature matches; comparison is constant-time.</returns>
    public static bool ValidateRequest(
        string apiKey,
        string url,
        IEnumerable<KeyValuePair<string, string>> formParams,
        string signature)
    {
        if (string.IsNullOrEmpty(apiKey) || string.IsNullOrEmpty(url) || signature is null)
        {
            return false;
        }

        // Canonical variants: the URL as given, plus the default port added or removed.
        var pairs = formParams.ToArray();
        var candidates = new List<string> { GetSignature(apiKey, url, pairs) };

        var withPort = AddDefaultPort(url);
        if (withPort is not null && withPort != url)
        {
            candidates.Add(GetSignature(apiKey, withPort, pairs));
        }
        var withoutPort = RemoveDefaultPort(url);
        if (withoutPort is not null && withoutPort != url)
        {
            candidates.Add(GetSignature(apiKey, withoutPort, pairs));
        }

        var expected = Encoding.ASCII.GetBytes(signature);
        foreach (var candidate in candidates)
        {
            var computed = Encoding.ASCII.GetBytes(candidate);
            if (FixedTimeEqualsByte(computed, expected))
            {
                return true;
            }
        }
        return false;
    }

    /// <summary>Compute the <c>X-Twilio-Signature</c> value for a URL and form parameters —
    /// the same algorithm the server uses. Useful for signing outbound requests in tests.</summary>
    /// <param name="apiKey">The account's API key.</param>
    /// <param name="url">The full callback URL (fragment stripped if present).</param>
    /// <param name="formParams">Form parameters in any order; they are sorted by key before
    /// signing. Repeated keys keep their given relative order.</param>
    /// <returns>Base64 (standard, padded) HMAC-SHA1 signature.</returns>
    public static string GetSignature(
        string apiKey,
        string url,
        IEnumerable<KeyValuePair<string, string>>? formParams = null)
    {
        var sb = new StringBuilder(StripFragment(url));
        if (formParams is not null)
        {
            var pairs = formParams.OrderBy(p => Encoding.UTF8.GetBytes(p.Key), ByteComparer.Instance);
            foreach (var kv in pairs)
            {
                sb.Append(kv.Key).Append(kv.Value);
            }
        }

        using var hmac = new HMACSHA1(Encoding.UTF8.GetBytes(apiKey));
        var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(sb.ToString()));
        return Convert.ToBase64String(hash);
    }

    private static bool FixedTimeEqualsByte(byte[] left, byte[] right)
    {
        if (left.Length == 0 || left.Length != right.Length)
        {
            return false;
        }
        return CryptographicOperations.FixedTimeEquals(left, right);
    }

    private static string StripFragment(string url)
    {
        var hashIndex = url.IndexOf('#');
        return hashIndex < 0 ? url : url[..hashIndex];
    }

    private static string? AddDefaultPort(string url)
        => WithDefaultPort(url, include: true);

    private static string? RemoveDefaultPort(string url)
        => WithDefaultPort(url, include: false);

    private static string? WithDefaultPort(string url, bool include)
    {
        var raw = StripFragment(url);
        if (!Uri.TryCreate(raw, UriKind.Absolute, out var uri) || !uri.IsDefaultPort)
        {
            return null;
        }
        var defaultPort = uri.Scheme == "https" ? 443 : uri.Scheme == "http" ? 80 : -1;
        if (defaultPort < 0)
        {
            return null;
        }
        // Change only the authority's port bytes. Uri.AbsoluteUri would also
        // canonicalize host case and escape spelling, invalidating valid signatures.
        var authorityStart = raw.IndexOf("://", StringComparison.Ordinal) + 3;
        var authorityEnd = raw.IndexOfAny(new[] { '/', '?' }, authorityStart);
        if (authorityEnd < 0) authorityEnd = raw.Length;
        var authority = raw[authorityStart..authorityEnd];
        var colon = authority.LastIndexOf(':');
        var hasPort = colon > authority.LastIndexOf(']') && colon > authority.LastIndexOf('@') &&
            int.TryParse(authority[(colon + 1)..], out _);
        if (include)
            return hasPort ? raw : raw[..authorityEnd] + ":" + defaultPort + raw[authorityEnd..];
        return hasPort ? raw[..(authorityStart + colon)] + raw[authorityEnd..] : raw;
    }

    private sealed class ByteComparer : IComparer<byte[]>
    {
        internal static readonly ByteComparer Instance = new();
        /// <summary>Compare request validator. Route: Request Validator.</summary>
        public int Compare(byte[]? x, byte[]? y) => (x ?? Array.Empty<byte>()).AsSpan().SequenceCompareTo(y ?? Array.Empty<byte>());
    }
}
