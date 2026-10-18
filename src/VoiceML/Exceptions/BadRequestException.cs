namespace VoiceML.Exceptions;

/// <summary>HTTP 400 — the request was malformed or failed server-side validation.</summary>
public sealed class BadRequestException : ApiException
{
    /// <summary>Construct with structured fields.</summary>
    public BadRequestException(string message, int statusCode, int? code = null, object? body = null, string? moreInfo = null, string? codeRaw = null, int? status = null)
        : base(message, statusCode, code, body, moreInfo, codeRaw, status) { }
}
