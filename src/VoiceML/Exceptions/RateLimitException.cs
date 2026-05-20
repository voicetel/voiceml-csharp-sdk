namespace VoiceML.Exceptions;

/// <summary>HTTP 429 — per-account rate limit exceeded. The server's <c>Retry-After</c>
/// header may hint when to retry; if present it is on <see cref="RetryAfterSeconds"/>.</summary>
public sealed class RateLimitException : ApiException
{
    /// <summary>Value of the response's <c>Retry-After</c> header, if it parsed as a number.</summary>
    public double? RetryAfterSeconds { get; }

    /// <summary>Construct with structured fields.</summary>
    public RateLimitException(
        string message,
        int statusCode,
        object? code = null,
        object? body = null,
        double? retryAfterSeconds = null,
        string? moreInfo = null)
        : base(message, statusCode, code, body, moreInfo)
    {
        RetryAfterSeconds = retryAfterSeconds;
    }
}
