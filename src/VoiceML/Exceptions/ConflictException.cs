namespace VoiceML.Exceptions;

/// <summary>HTTP 409 — request conflicts with current resource state.
/// Typical case: deleting a queue that still has waiting members.</summary>
public sealed class ConflictException : ApiException
{
    /// <summary>Construct with structured fields.</summary>
    public ConflictException(string message, int statusCode, object? code = null, object? body = null)
        : base(message, statusCode, code, body) { }
}
