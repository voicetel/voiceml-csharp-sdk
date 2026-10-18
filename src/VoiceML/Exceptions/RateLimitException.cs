using System;

namespace VoiceML.Exceptions;

/// <summary>HTTP 429 — per-account rate limit exceeded. The server's <c>Retry-After</c>
/// header may direct when to retry; when present it is exposed on <see cref="RetryAfter"/>
/// (and as seconds on <see cref="RetryAfterSeconds"/>). A <c>Retry-After</c> longer than the
/// SDK's 60-second cap is never slept through — the error is raised immediately with the
/// parsed value attached.</summary>
public sealed class RateLimitException : ApiException
{
    /// <summary>Value of the response's <c>Retry-After</c> header, if parseable.</summary>
    public TimeSpan? RetryAfter { get; }

    /// <summary>Value of the response's <c>Retry-After</c> header in seconds, if parseable.</summary>
    public double? RetryAfterSeconds { get; }

    /// <summary>Construct with structured fields.</summary>
    public RateLimitException(
        string message,
        int statusCode,
        int? code = null,
        object? body = null,
        double? retryAfterSeconds = null,
        string? moreInfo = null,
        string? codeRaw = null,
        int? status = null,
        TimeSpan? retryAfter = null)
        : base(message, statusCode, code, body, moreInfo, codeRaw, status)
    {
        RetryAfterSeconds = retryAfterSeconds;
        RetryAfter = retryAfter;
    }
}
