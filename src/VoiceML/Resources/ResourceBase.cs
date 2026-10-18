namespace VoiceML.Resources;

/// <summary>Percent-encodes a single URL path segment. Every caller-supplied path parameter
/// (SIDs, phone numbers, domain names, dates) goes through here before interpolation, keeping
/// the RFC 3986 unreserved set (<c>A-Z a-z 0-9 - . _ ~</c>) and percent-encoding everything
/// else — so <c>../</c> traversal, <c>?</c> query injection and <c>#</c> fragment injection in
/// caller input are impossible.</summary>
internal static class PathSegment
{
    /// <summary>Encode one path segment.</summary>
    public static string Encode(string segment)
    {
        if (segment is "." or "..")
            throw new VoiceML.Exceptions.VoiceMLException("A path parameter cannot be a dot segment.");
        return System.Uri.EscapeDataString(segment);
    }
}

/// <summary>Shared base for resource classes — wraps a <see cref="Transport"/> and renders
/// URL paths under the Twilio-compatible prefix <c>/2010-04-01/Accounts/{AccountSid}/…</c>.
/// Each caller-supplied segment is percent-encoded exactly once.</summary>
public abstract class ResourceBase
{
    /// <summary>Fetch the next page of an account-scoped list using its server-provided continuation URI. Returns null when no next page exists.</summary>
    public System.Threading.Tasks.Task<T?> FetchNextPageAsync<T>(T page, System.Threading.CancellationToken ct = default) where T : Models.Page
        => string.IsNullOrEmpty(page.NextPageUri)
            ? System.Threading.Tasks.Task.FromResult<T?>(null)
            : Transport.SendPageUriAsync<T>(page.NextPageUri, ct);

    /// <summary>The shared transport for this client.</summary>
    private protected Transport Transport { get; }

    /// <summary>Construct with a transport reference.</summary>
    private protected ResourceBase(Transport transport)
    {
        Transport = transport;
    }

    /// <summary>Build a URL path under <c>/2010-04-01/Accounts/{AccountSid}/…</c> with a
    /// <c>.json</c> suffix on the final segment (Twilio convention for this API's JSON
    /// endpoints). Empty segments are skipped; every caller-supplied segment is
    /// percent-encoded. For non-JSON content (e.g. <c>.wav</c> audio), use
    /// <see cref="PathNoSuffix"/>.</summary>
    private protected string Path(params string[] segments)
    {
        var sb = PathBuilder(segments);
        sb.Append(".json");
        return sb.ToString();
    }

    /// <summary>Build a URL path under <c>/2010-04-01/Accounts/{AccountSid}/…</c> WITHOUT
    /// the <c>.json</c> suffix. Used by callers that append their own extension (e.g.
    /// <c>.wav</c> for recording audio) and by endpoints the spec defines without a suffix
    /// (e.g. <c>/Exports/{Date}</c>).</summary>
    private protected string PathNoSuffix(params string[] segments)
    {
        return PathBuilder(segments).ToString();
    }

    private System.Text.StringBuilder PathBuilder(string[] segments)
    {
        var sb = new System.Text.StringBuilder("/2010-04-01/Accounts/");
        sb.Append(PathSegment.Encode(Transport.AccountSid));
        foreach (var seg in segments)
        {
            if (string.IsNullOrEmpty(seg))
            {
                continue;
            }
            sb.Append('/');
            sb.Append(PathSegment.Encode(seg));
        }
        return sb;
    }
}
