namespace VoiceML.Exceptions;

/// <summary>HTTP 403 — authenticated, but not allowed to perform this action.</summary>
public sealed class PermissionDeniedException : ApiException
{
    /// <summary>Construct with structured fields.</summary>
    public PermissionDeniedException(string message, int statusCode, object? code = null, object? body = null)
        : base(message, statusCode, code, body) { }
}
