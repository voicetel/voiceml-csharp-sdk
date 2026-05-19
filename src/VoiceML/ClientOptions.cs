using System;
using System.Net.Http;
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
/// <para>Validation runs in <see cref="Validate"/>; the client calls it in its constructor.</para>
/// </summary>
public sealed record ClientOptions
{
    /// <summary>The default VoiceML base URL: <c>https://voiceml.voicetel.com</c>.</summary>
    public const string DefaultBaseUrl = "https://voiceml.voicetel.com";

    /// <summary>Default HTTP timeout (30 seconds).</summary>
    public static readonly TimeSpan DefaultTimeout = TimeSpan.FromSeconds(30);

    /// <summary>Default maximum number of retries on retryable failures (429, 5xx, transient transport errors).</summary>
    public const int DefaultMaxRetries = 2;

    /// <summary>Twilio-format AccountSid: literal <c>AC</c> + 32 hex chars. Sent as the
    /// HTTP-Basic username on every request.</summary>
    public required string AccountSid { get; init; }

    /// <summary>Per-tenant API key. Sent as the HTTP-Basic password on every request.</summary>
    public required string ApiKey { get; init; }

    /// <summary>Server base URL. Defaults to <see cref="DefaultBaseUrl"/>. Override only when
    /// pointing at a staging server.</summary>
    public string BaseUrl { get; init; } = DefaultBaseUrl;

    /// <summary>Per-request timeout. Applied when this SDK owns the <see cref="HttpClient"/>;
    /// when a caller provides their own client, the caller's timeout is used.</summary>
    public TimeSpan Timeout { get; init; } = DefaultTimeout;

    /// <summary>Maximum number of retries on retryable failures. <c>0</c> disables retries.</summary>
    public int MaxRetries { get; init; } = DefaultMaxRetries;

    /// <summary>User-Agent string sent on every request. Defaults to <see cref="VoiceMLVersion.DefaultUserAgent"/>.</summary>
    public string UserAgent { get; init; } = VoiceMLVersion.DefaultUserAgent;

    /// <summary>Optional pre-built <see cref="HttpClient"/>. If supplied, the SDK reuses it and
    /// does NOT dispose it. Useful for injecting <c>IHttpClientFactory</c> output.</summary>
    public HttpClient? HttpClient { get; init; }

    /// <summary>Optional log sink. Receives one line per request/response/retry event.
    /// <c>null</c> disables all logging. Use this to bridge to <c>ILogger</c> if desired:
    /// <code>Logger = msg =&gt; logger.LogDebug("{Msg}", msg)</code>. Keeping it a delegate
    /// avoids taking a hard dependency on <c>Microsoft.Extensions.Logging.Abstractions</c>.</summary>
    public Action<string>? Logger { get; init; }

    /// <summary>Validate required fields. Throws <see cref="ConfigurationException"/> on failure.</summary>
    public void Validate()
    {
        if (string.IsNullOrWhiteSpace(AccountSid))
        {
            throw new ConfigurationException("AccountSid is required");
        }
        if (string.IsNullOrWhiteSpace(ApiKey))
        {
            throw new ConfigurationException("ApiKey is required");
        }
        if (string.IsNullOrWhiteSpace(BaseUrl))
        {
            throw new ConfigurationException("BaseUrl must be non-empty");
        }
        if (MaxRetries < 0)
        {
            throw new ConfigurationException("MaxRetries must be >= 0");
        }
        if (Timeout <= TimeSpan.Zero)
        {
            throw new ConfigurationException("Timeout must be positive");
        }
    }
}
