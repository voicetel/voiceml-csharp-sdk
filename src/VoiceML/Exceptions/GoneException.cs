namespace VoiceML.Exceptions;

/// <summary>HTTP 410 — recording audio is no longer available (no local file, no S3 key).</summary>
public sealed class GoneException : ApiException
{
    /// <summary>Construct with structured fields.</summary>
    public GoneException(string message, int statusCode, object? code = null, object? body = null, string? moreInfo = null)
        : base(message, statusCode, code, body, moreInfo) { }
}
