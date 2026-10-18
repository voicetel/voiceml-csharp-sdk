using System;

namespace VoiceML.Exceptions;

/// <summary>Catch-all for non-2xx API responses. Specific status families have subclasses;
/// catch <see cref="ApiException"/> to handle them all uniformly. The error body
/// (<c>{code, message, more_info, status}</c>) is parsed into <see cref="Code"/> /
/// <see cref="Exception.Message"/> / <see cref="MoreInfo"/> / <see cref="Status"/> when present;
/// the raw payload is on <see cref="Body"/>.</summary>
public class ApiException : VoiceMLException
{
    /// <summary>HTTP status code returned by the server. 0 for transport-level failures.</summary>
    public int StatusCode { get; }

    /// <summary>Numeric error code from the response body, when it parsed as an integer
    /// (e.g. <c>21211</c>). <c>null</c> when absent or non-numeric — see <see cref="CodeRaw"/>.</summary>
    public int? Code { get; }

    /// <summary>Raw error-code value from the response body as a string, whatever its JSON
    /// type was. Use this when the server sends a non-numeric code.</summary>
    public string? CodeRaw { get; }

    /// <summary>HTTP status code echoed inside the JSON body, when present. Usually equals
    /// <see cref="StatusCode"/>.</summary>
    public int? Status { get; }

    /// <summary><c>more_info</c> URL from the error body, when present.
    /// Documentation URL that explains the error class.</summary>
    public string? MoreInfo { get; }

    /// <summary>Raw response body — parsed JSON object/array when JSON, plain string otherwise.</summary>
    public object? Body { get; }

    /// <summary>Construct with structured fields.</summary>
    public ApiException(
        string message,
        int statusCode,
        int? code = null,
        object? body = null,
        string? moreInfo = null,
        string? codeRaw = null,
        int? status = null,
        Exception? inner = null) : base(message, inner)
    {
        StatusCode = statusCode;
        Code = code;
        Body = body;
        MoreInfo = moreInfo;
        CodeRaw = codeRaw;
        Status = status;
    }

    /// <summary>Construct wrapping an inner exception (e.g. transport failure).</summary>
    public ApiException(string message, int statusCode, Exception? inner) : base(message, inner)
    {
        StatusCode = statusCode;
    }
}
