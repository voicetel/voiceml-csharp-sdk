namespace VoiceML.Exceptions;

/// <summary>HTTP 404 — the resource does not exist (or belongs to a different tenant).</summary>
public sealed class NotFoundException : ApiException
{
    /// <summary>Construct with structured fields.</summary>
    public NotFoundException(string message, int statusCode, object? code = null, object? body = null, string? moreInfo = null)
        : base(message, statusCode, code, body, moreInfo) { }
}
