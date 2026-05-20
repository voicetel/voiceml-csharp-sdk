namespace VoiceML.Exceptions;

/// <summary>HTTP 501 — endpoint is mounted as a stub (e.g. UserDefinedMessages).</summary>
public sealed class NotImplementedAPIException : ApiException
{
    /// <summary>Construct with structured fields.</summary>
    public NotImplementedAPIException(string message, int statusCode, object? code = null, object? body = null, string? moreInfo = null)
        : base(message, statusCode, code, body, moreInfo) { }
}
