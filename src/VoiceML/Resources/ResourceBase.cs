namespace VoiceML.Resources;

/// <summary>Shared base for resource classes — wraps a <see cref="Transport"/> and renders
/// URL paths under the Twilio-compatible prefix <c>/2010-04-01/Accounts/{AccountSid}/…</c>.</summary>
public abstract class ResourceBase
{
    /// <summary>The shared transport for this client.</summary>
    protected Transport Transport { get; }

    /// <summary>Construct with a transport reference.</summary>
    protected ResourceBase(Transport transport)
    {
        Transport = transport;
    }

    /// <summary>Build a URL path under <c>/2010-04-01/Accounts/{AccountSid}/…</c> with a
    /// <c>.json</c> suffix on the final segment (Twilio convention). Empty segments are
    /// skipped; nothing is URL-encoded — callers pass sids and slugs that don't need
    /// escaping (Twilio sids never do). For non-JSON content (e.g. <c>.wav</c> audio),
    /// use <see cref="PathNoSuffix"/>.</summary>
    protected string Path(params string[] segments)
    {
        var sb = PathBuilder(segments);
        sb.Append(".json");
        return sb.ToString();
    }

    /// <summary>Build a URL path under <c>/2010-04-01/Accounts/{AccountSid}/…</c> WITHOUT
    /// the <c>.json</c> suffix. Used by callers that append their own extension (e.g.
    /// <c>.wav</c> for recording audio).</summary>
    protected string PathNoSuffix(params string[] segments)
    {
        return PathBuilder(segments).ToString();
    }

    private System.Text.StringBuilder PathBuilder(string[] segments)
    {
        var sb = new System.Text.StringBuilder("/2010-04-01/Accounts/");
        sb.Append(Transport.AccountSid);
        foreach (var seg in segments)
        {
            if (string.IsNullOrEmpty(seg))
            {
                continue;
            }
            sb.Append('/');
            sb.Append(seg);
        }
        return sb;
    }
}
