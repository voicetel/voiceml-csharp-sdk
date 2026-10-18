using System;
using System.Net.Http;
using System.Text;
using System.Text.Json.Serialization;
using VoiceML.Exceptions;

namespace VoiceML;

/// <summary>Immutable options bag for <see cref="VoiceMLClient"/>. Use object-initializer
/// syntax to build:
/// <code>
/// var opts = new ClientOptions {
///     AccountSid = "AC…",
///     ApiKey     = "…",
/// };
/// </code>
/// <para>Validation runs in <see cref="Validate"/>; the client calls it in its constructor.
/// The credential properties are excluded from JSON serialisation and from the record's
/// <see cref="object.ToString"/> rendering, so logging the options never leaks the API key.</para>
/// </summary>
public sealed record ClientOptions
{
    /// <summary>The default VoiceML base URL: <c>https://voiceml.voicetel.com</c>.</summary>
    public const string DefaultBaseUrl = "https://voiceml.voicetel.com";

    /// <summary>Default HTTP timeout (30 seconds).</summary>
    public static readonly TimeSpan DefaultTimeout = TimeSpan.FromSeconds(30);

    /// <summary>Default maximum number of retries on retryable failures. <c>2</c> means at most
    /// 3 attempts per request. See the SDK README for which failures are retried for which
    /// HTTP methods.</summary>
    public const int DefaultMaxRetries = 2;

    /// <summary>Twilio-format AccountSid: literal <c>AC</c> + 32 hex chars. Sent as the
    /// HTTP-Basic username on authenticated API requests.</summary>
    public required string AccountSid { get; init; }

    /// <summary>Per-tenant API key. Sent as the HTTP-Basic password on authenticated requests.
    /// <para>Aliased by <see cref="AuthToken"/> for Twilio-compatible ergonomics — set either
    /// (but not both).</para></summary>
    [JsonIgnore]
    public string? ApiKey { get; init; }

    /// <summary>Twilio-compatible alias for <see cref="ApiKey"/>. Set one or the other (not
    /// both — <see cref="Validate"/> throws on conflict). When set, the value is used as
    /// the HTTP-Basic password exactly like <see cref="ApiKey"/>.</summary>
    [JsonIgnore]
    public string? AuthToken { get; init; }

    /// <summary>The credential actually used on the wire — whichever of
    /// <see cref="ApiKey"/> / <see cref="AuthToken"/> was set. Internal so it can never
    /// leak through reflection-free public surface or accidental serialisation.</summary>
    [JsonIgnore]
    internal string ResolvedCredential => ApiKey ?? AuthToken ?? "";

    /// <summary>Server base URL. Defaults to <see cref="DefaultBaseUrl"/>. Override only when
    /// pointing at a staging server.</summary>
    public string BaseUrl { get; init; } = DefaultBaseUrl;

    /// <summary>Optional explicit base URL for the Messaging Service product
    /// (<c>client.MessagingV1.*</c>). When <c>null</c>, it is derived from <see cref="BaseUrl"/>
    /// by swapping the <c>voiceml</c> label for <c>messaging</c> on recognised
    /// <c>*.voicetel.com</c> hosts (see <see cref="ProductHosts"/>); other base URLs fall back to
    /// <see cref="BaseUrl"/> unchanged. Set this to reach Messaging Service on a custom host.</summary>
    public string? MessagingBaseUrl { get; init; }

    /// <summary>Optional explicit base URL for the Conversations product
    /// (<c>client.ConversationsV1.*</c>). When <c>null</c>, it is derived from <see cref="BaseUrl"/>
    /// by swapping the <c>voiceml</c> label for <c>conversations</c> on recognised
    /// <c>*.voicetel.com</c> hosts (see <see cref="ProductHosts"/>); other base URLs fall back to
    /// <see cref="BaseUrl"/> unchanged. Set this to reach Conversations on a custom host.</summary>
    public string? ConversationsBaseUrl { get; init; }

    /// <summary>Per-request timeout. Applied when this SDK owns the <see cref="HttpClient"/>;
    /// when a caller provides their own client, the caller's timeout is used. When the timeout
    /// elapses the SDK raises <see cref="VoiceMLTimeoutException"/> (a
    /// <see cref="VoiceMLException"/>) — never a bare <c>TaskCanceledException</c>.</summary>
    public TimeSpan Timeout { get; init; } = DefaultTimeout;

    /// <summary>Maximum number of retries on retryable failures. <c>0</c> disables retries.</summary>
    public int MaxRetries { get; init; } = DefaultMaxRetries;

    /// <summary>User-Agent string sent on every request. Defaults to <see cref="VoiceMLVersion.DefaultUserAgent"/>.</summary>
    public string UserAgent { get; init; } = VoiceMLVersion.DefaultUserAgent;

    /// <summary>Optional pre-built <see cref="HttpClient"/>. If supplied, the SDK reuses it and
    /// does NOT dispose it. Useful for injecting <c>IHttpClientFactory</c> output. Prefer one
    /// long-lived client per process (see the SDK README) — creating a client per request
    /// exhausts sockets.</summary>
    [JsonIgnore]
    public HttpClient? HttpClient { get; init; }

    /// <summary>Optional log sink. Receives one line per request/response/retry event.
    /// <c>null</c> disables all logging. Use this to bridge to <c>ILogger</c> if desired:
    /// <code>Logger = msg =&gt; logger.LogDebug("{Msg}", msg)</code>. Keeping it a delegate
    /// avoids taking a hard dependency on <c>Microsoft.Extensions.Logging.Abstractions</c>.</summary>
    [JsonIgnore]
    public Action<string>? Logger { get; init; }

    /// <summary>Validate required fields. Throws <see cref="ConfigurationException"/> on failure.</summary>
    public void Validate()
    {
        if (string.IsNullOrWhiteSpace(AccountSid))
        {
            throw new ConfigurationException("AccountSid is required");
        }
        var hasApiKey = !string.IsNullOrWhiteSpace(ApiKey);
        var hasAuthToken = !string.IsNullOrWhiteSpace(AuthToken);
        if (hasApiKey && hasAuthToken)
        {
            throw new ConfigurationException("Set either ApiKey or AuthToken — not both. AuthToken is a Twilio-compatible alias for ApiKey.");
        }
        if (!hasApiKey && !hasAuthToken)
        {
            throw new ConfigurationException("ApiKey (or its alias AuthToken) is required");
        }
        if (string.IsNullOrWhiteSpace(BaseUrl))
        {
            throw new ConfigurationException("BaseUrl must be non-empty");
        }
        foreach (var url in new[] { BaseUrl, MessagingBaseUrl, ConversationsBaseUrl })
        {
            if (url is null) continue;
            if (!Uri.TryCreate(url, UriKind.Absolute, out var uri) || uri.Scheme is not ("http" or "https") ||
                uri.UserInfo.Length > 0 || uri.Fragment.Length > 0 || uri.Query.Length > 0)
                throw new ConfigurationException("Base URLs must be absolute HTTP(S) URLs without credentials, query or fragment.");
        }
        if (MaxRetries < 0)
        {
            throw new ConfigurationException("MaxRetries must be >= 0");
        }
        if (Timeout <= TimeSpan.Zero || Timeout.TotalMilliseconds > int.MaxValue)
        {
            throw new ConfigurationException("Timeout must be positive and no greater than Int32.MaxValue milliseconds");
        }
    }

    /// <summary>Render the options with the credential fields redacted. Replaces the
    /// compiler-generated record member, which would otherwise print the API key.</summary>
    private bool PrintMembers(StringBuilder builder)
    {
        builder.Append("AccountSid = ").Append(AccountSid);
        builder.Append(", ApiKey = ").Append(ApiKey is null ? "null" : "[REDACTED]");
        builder.Append(", AuthToken = ").Append(AuthToken is null ? "null" : "[REDACTED]");
        builder.Append(", BaseUrl = ").Append(BaseUrl);
        builder.Append(", MessagingBaseUrl = ").Append(MessagingBaseUrl ?? "null");
        builder.Append(", ConversationsBaseUrl = ").Append(ConversationsBaseUrl ?? "null");
        builder.Append(", Timeout = ").Append(Timeout);
        builder.Append(", MaxRetries = ").Append(MaxRetries);
        builder.Append(", UserAgent = ").Append(UserAgent);
        builder.Append(", HttpClient = ").Append(HttpClient is null ? "null" : HttpClient.GetType().Name);
        builder.Append(", Logger = ").Append(Logger is null ? "null" : "set");
        return true;
    }
}
