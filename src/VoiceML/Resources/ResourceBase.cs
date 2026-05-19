namespace VoiceML.Resources;

/// <summary>Shared base for resource classes — wraps a <see cref="Transport"/> and renders
/// URL paths under the canonical Twilio prefix <c>/2010-04-01/Accounts/{AccountSid}/…</c>.</summary>
public abstract class ResourceBase
{
    /// <summary>The shared transport for this client.</summary>
    protected Transport Transport { get; }

    /// <summary>Construct with a transport reference.</summary>
    protected ResourceBase(Transport transport)
    {
        Transport = transport;
    }

    /// <summary>Build a URL path under <c>/2010-04-01/Accounts/{AccountSid}/…</c>. Empty
    /// segments are skipped; nothing is URL-encoded — callers pass sids and slugs that
    /// don't need escaping (Twilio sids never do).</summary>
    protected string Path(params string[] segments)
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
        return sb.ToString();
    }
}
