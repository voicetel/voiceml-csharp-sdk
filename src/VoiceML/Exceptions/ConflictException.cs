namespace VoiceML.Exceptions;

/// <summary>HTTP 409 — request conflicts with current resource state.
/// Typical cases: deleting a queue that still has waiting members, the 20409
/// optimistic-locking conflict on account updates (the account was modified by another
/// request — refresh and retry), or re-adding a phone number already in a Messaging
/// Service sender pool.</summary>
public sealed class ConflictException : ApiException
{
    /// <summary>Construct with structured fields.</summary>
    public ConflictException(string message, int statusCode, int? code = null, object? body = null, string? moreInfo = null, string? codeRaw = null, int? status = null)
        : base(message, statusCode, code, body, moreInfo, codeRaw, status) { }
}
