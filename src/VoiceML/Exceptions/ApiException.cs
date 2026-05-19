using System;

namespace VoiceML.Exceptions;

/// <summary>Catch-all for non-2xx API responses. Specific status families have subclasses;
/// catch <see cref="ApiException"/> to handle them all uniformly. The Twilio-shape error
/// body (<c>{code, message, more_info, status}</c>) is parsed into <see cref="Code"/>
/// / <see cref="Exception.Message"/> when present; the raw payload is on <see cref="Body"/>.</summary>
public class ApiException : VoiceMLException
{
    /// <summary>HTTP status code returned by the server. 0 for transport-level failures.</summary>
    public int StatusCode { get; }

    /// <summary>Twilio-format error code from the response body, when present. May be int or string.</summary>
    public object? Code { get; }

    /// <summary>Raw response body — parsed JSON object/array when JSON, plain string otherwise.</summary>
    public object? Body { get; }

    /// <summary>Construct with structured fields.</summary>
    public ApiException(string message, int statusCode, object? code = null, object? body = null) : base(message)
    {
        StatusCode = statusCode;
        Code = code;
        Body = body;
    }

    /// <summary>Construct wrapping an inner exception (e.g. transport failure).</summary>
    public ApiException(string message, int statusCode, Exception inner) : base(message, inner)
    {
        StatusCode = statusCode;
    }
}
