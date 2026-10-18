using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace VoiceML.Models;

/// <summary>A Monitor v1 Alert (<c>GET /v1/Alerts</c>) — a failure event rendered in the
/// Alert shape: call-scoped errors and messaging delivery failures alike. The list view
/// omits the request/response detail; fetch includes it when the row carries any.</summary>
public sealed record MonitorAlert
{
    /// <summary>Alert SID.</summary>
    [JsonPropertyName("sid")] public string Sid { get; init; } = "";

    /// <summary>Owning account SID.</summary>
    [JsonPropertyName("account_sid")] public string AccountSid { get; init; } = "";

    /// <summary>Human-readable description of the failure.</summary>
    [JsonPropertyName("alert_text")] public string AlertText { get; init; } = "";

    /// <summary>Always <c>null</c> on this surface.</summary>
    [JsonPropertyName("api_version")] public string? ApiVersion { get; init; }

    /// <summary>Creation timestamp (ISO-8601).</summary>
    [JsonPropertyName("date_created")] public string DateCreated { get; init; } = "";

    /// <summary>Timestamp the underlying failure was generated at (ISO-8601).</summary>
    [JsonPropertyName("date_generated")] public string DateGenerated { get; init; } = "";

    /// <summary>Last-modification timestamp (ISO-8601).</summary>
    [JsonPropertyName("date_updated")] public string DateUpdated { get; init; } = "";

    /// <summary>Numeric error code, as a string (e.g. <c>11200</c>).</summary>
    [JsonPropertyName("error_code")] public string ErrorCode { get; init; } = "";

    /// <summary>Severity. See <see cref="VoiceML.MonitorLogLevel"/> for the allowed string
    /// constants; future values are preserved as strings.</summary>
    [JsonPropertyName("log_level")] public string LogLevel { get; init; } = "";

    /// <summary>Error-dictionary URL for <see cref="ErrorCode"/>.</summary>
    [JsonPropertyName("more_info")] public string MoreInfo { get; init; } = "";

    /// <summary>HTTP method of the request that tripped the alert.</summary>
    [JsonPropertyName("request_method")] public string RequestMethod { get; init; } = "";

    /// <summary>URL of the request that tripped the alert.</summary>
    [JsonPropertyName("request_url")] public string RequestUrl { get; init; } = "";

    /// <summary>The affected resource — the message SID for messaging failures, the call SID
    /// for call-scoped ones; <c>null</c> when neither exists.</summary>
    [JsonPropertyName("resource_sid")] public string? ResourceSid { get; init; }

    /// <summary>Messaging Service SID for message-linked rows; <c>null</c> otherwise.</summary>
    [JsonPropertyName("service_sid")] public string? ServiceSid { get; init; }

    /// <summary>Absolute URL of this Alert resource.</summary>
    [JsonPropertyName("url")] public string Url { get; init; } = "";

    /// <summary>Fetch only: raw request headers when the event stored them.</summary>
    [JsonPropertyName("request_headers")] public string? RequestHeaders { get; init; }

    /// <summary>Fetch only: request variables when the event stored them.</summary>
    [JsonPropertyName("request_variables")] public string? RequestVariables { get; init; }

    /// <summary>Fetch only: response body when the event stored it.</summary>
    [JsonPropertyName("response_body")] public string? ResponseBody { get; init; }

    /// <summary>Fetch only: response headers when the event stored them.</summary>
    [JsonPropertyName("response_headers")] public string? ResponseHeaders { get; init; }
}

/// <summary>Page of Alert resources with continuation metadata. Alerts are served as a
/// single page: <see cref="VoiceV1Meta.NextPageUrl"/> is always <c>null</c>, and
/// <c>PageSize</c> limits the returned rows (default 50, capped at 1000).</summary>
public sealed record MonitorAlertList : IV1PagedList
{
    /// <summary>The returned alerts in this page.</summary>
    [JsonPropertyName("alerts")] public List<MonitorAlert> Alerts { get; init; } = new();

    /// <summary>Pagination metadata; <c>next_page_url</c> is always <c>null</c> on this
    /// collection.</summary>
    [JsonPropertyName("meta")] public VoiceV1Meta? Meta { get; init; }
}

/// <summary>Query-string params for <c>GET /v1/Alerts</c>.</summary>
public sealed record ListMonitorAlertsParams
{
    /// <summary>Filter by severity — <see cref="VoiceML.MonitorLogLevel.Error"/> or
    /// <see cref="VoiceML.MonitorLogLevel.Warning"/>. Only these two are distinguished.</summary>
    public string? LogLevel { get; init; }

    /// <summary>Only alerts generated at or after this boundary — <c>YYYY-MM-DD</c> or a full
    /// ISO-8601 timestamp. A malformed value is ignored by the server.</summary>
    public string? StartDate { get; init; }

    /// <summary>Only alerts generated on or before this day, inclusive — <c>YYYY-MM-DD</c> or a
    /// full ISO-8601 timestamp. A malformed value is ignored by the server.</summary>
    public string? EndDate { get; init; }

    /// <summary>Maximum number of alerts returned by the single page (default 50, capped at
    /// 1000). Alerts are an exception among <c>/v1</c> collections: PageSize limits the
    /// page and there is no continuation link.</summary>
    public int? PageSize { get; init; }

    /// <summary>Render the declared filters as query pairs; null values are omitted by the
    /// transport.</summary>
    public IEnumerable<KeyValuePair<string, string?>> ToQuery()
    {
        yield return new("LogLevel", LogLevel);
        yield return new("StartDate", StartDate);
        yield return new("EndDate", EndDate);
        yield return new("PageSize", PageSize?.ToString());
    }
}
