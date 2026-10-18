using System.Text.Json.Serialization;

namespace VoiceML.Models;

/// <summary>A diagnostic notification for an account or call. Current notification endpoints are compatibility stubs; this model preserves the declared response contract.</summary>
public sealed record Notification
{
    /// <summary>Unique notification SID.</summary>
    [JsonPropertyName("sid")] public string Sid { get; init; } = "";
    /// <summary>Account that owns this notification.</summary>
    [JsonPropertyName("account_sid")] public string AccountSid { get; init; } = "";
    /// <summary>Call associated with the diagnostic event.</summary>
    [JsonPropertyName("call_sid")] public string CallSid { get; init; } = "";
    /// <summary>API version associated with this notification; nullable per the contract.</summary>
    [JsonPropertyName("api_version")] public string? ApiVersion { get; init; }
    /// <summary>Diagnostic error code, string-encoded on the wire.</summary>
    [JsonPropertyName("error_code")] public string ErrorCode { get; init; } = "";
    /// <summary>Log level, represented as a numeric string such as 0 or 1.</summary>
    [JsonPropertyName("log")] public string Log { get; init; } = "";
    /// <summary>Timestamp when the diagnostic event occurred.</summary>
    [JsonPropertyName("message_date")] public string MessageDate { get; init; } = "";
    /// <summary>Human-readable diagnostic detail.</summary>
    [JsonPropertyName("message_text")] public string? MessageText { get; init; }
    /// <summary>Documentation URL describing the diagnostic error.</summary>
    [JsonPropertyName("more_info")] public string MoreInfo { get; init; } = "";
    /// <summary>HTTP method used for the request that produced the notification.</summary>
    [JsonPropertyName("request_method")] public string? RequestMethod { get; init; }
    /// <summary>URL of the request that produced the notification.</summary>
    [JsonPropertyName("request_url")] public string? RequestUrl { get; init; }
    /// <summary>Resource URI relative to the API host.</summary>
    [JsonPropertyName("uri")] public string Uri { get; init; } = "";
    /// <summary>Creation timestamp of this notification.</summary>
    [JsonPropertyName("date_created")] public string DateCreated { get; init; } = "";
    /// <summary>Last modification timestamp of this notification.</summary>
    [JsonPropertyName("date_updated")] public string DateUpdated { get; init; } = "";
}
