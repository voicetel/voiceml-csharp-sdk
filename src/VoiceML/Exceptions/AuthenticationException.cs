namespace VoiceML.Exceptions;

/// <summary>HTTP 401 — Basic auth missing, account unknown, key wrong, or source IP not allowed.
/// The server intentionally returns an identical 401 for all four failure modes — see the
/// Twilio-compat spec's <c>Unauthorized</c> response description.</summary>
public sealed class AuthenticationException : ApiException
{
    /// <summary>Construct with structured fields.</summary>
    public AuthenticationException(string message, int statusCode, object? code = null, object? body = null, string? moreInfo = null)
        : base(message, statusCode, code, body, moreInfo) { }
}
