namespace VoiceML.Exceptions;

/// <summary>HTTP 5xx — the server hit an error processing the request.</summary>
public sealed class ServerException : ApiException
{
    /// <summary>Construct with structured fields.</summary>
    public ServerException(string message, int statusCode, object? code = null, object? body = null)
        : base(message, statusCode, code, body) { }
}
