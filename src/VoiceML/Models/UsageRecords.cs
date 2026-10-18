using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace VoiceML.Models;

/// <summary>A Usage Record — usage accrued in one category over a window
/// (<c>GET /2010-04-01/Accounts/{AccountSid}/Usage/Records*.json</c>). Numeric values are
/// decimal-as-string on the wire (Twilio's documented shape) and preserved without coercion.</summary>
public sealed record UsageRecord
{
    /// <summary>Owning Account SID.</summary>
    [JsonPropertyName("account_sid")] public string? AccountSid { get; init; }

    /// <summary>Twilio API version label.</summary>
    [JsonPropertyName("api_version")] public string? ApiVersion { get; init; }

    /// <summary>The records are current as of this timestamp (GMT).</summary>
    [JsonPropertyName("as_of")] public string? AsOf { get; init; }

    /// <summary>Usage category — exactly the set of categories the VoiceML product surface
    /// can accrue.</summary>
    [JsonPropertyName("category")] public string? Category { get; init; }

    /// <summary>Number of usage events (calls/messages). <c>null</c> on the totalprice record.</summary>
    [JsonPropertyName("count")] public string? Count { get; init; }

    /// <summary><c>"calls"</c> or <c>"messages"</c>; empty on the totalprice record.</summary>
    [JsonPropertyName("count_unit")] public string? CountUnit { get; init; }

    /// <summary>Human-readable description of the category.</summary>
    [JsonPropertyName("description")] public string? Description { get; init; }

    /// <summary>Last day of the record's window (inclusive), GMT.</summary>
    [JsonPropertyName("end_date")] public string? EndDate { get; init; }

    /// <summary>Total price over the window as a minimal-decimal string (e.g. <c>"0.0525"</c>).</summary>
    [JsonPropertyName("price")] public string? Price { get; init; }

    /// <summary>ISO 4217 currency, lowercase (e.g. <c>"usd"</c>).</summary>
    [JsonPropertyName("price_unit")] public string? PriceUnit { get; init; }

    /// <summary>First day of the record's window, GMT.</summary>
    [JsonPropertyName("start_date")] public string? StartDate { get; init; }

    /// <summary>Map of subresource URIs for this record.</summary>
    [JsonPropertyName("subresource_uris")] public Dictionary<string, object>? SubresourceUris { get; init; }

    /// <summary>This record's list URI, carrying the resolved window.</summary>
    [JsonPropertyName("uri")] public string? Uri { get; init; }

    /// <summary>Usage amount in <see cref="UsageUnit"/> (minutes for calls, segments for the
    /// direction splits, messages for the SMS/MMS aggregates).</summary>
    [JsonPropertyName("usage")] public string? Usage { get; init; }

    /// <summary><c>"minutes"</c>, <c>"segments"</c>, <c>"messages"</c>, <c>"usd"</c>
    /// (totalprice), or empty.</summary>
    [JsonPropertyName("usage_unit")] public string? UsageUnit { get; init; }
}

/// <summary>List response for the Usage Records endpoints.</summary>
public sealed record UsageRecordList : Page
{
    /// <summary>The page of UsageRecord resources.</summary>
    [JsonPropertyName("usage_records")] public List<UsageRecord> UsageRecords { get; init; } = new();
}

/// <summary>Query-string params shared by every Usage Records window
/// (<c>/Usage/Records</c> and <c>/Usage/Records/{AllTime,Daily,…}</c>).</summary>
public sealed record ListUsageRecordsParams
{
    /// <summary>Only this category's records are returned. An unknown category is a 400
    /// (error 20001).</summary>
    public string? Category { get; init; }

    /// <summary>Window start (inclusive), <c>YYYY-MM-DD</c> in GMT, or an offset such as
    /// <c>-30days</c>.</summary>
    public string? StartDate { get; init; }

    /// <summary>Window end (inclusive), <c>YYYY-MM-DD</c> in GMT, or an offset such as
    /// <c>+30days</c>.</summary>
    public string? EndDate { get; init; }

    /// <summary>Accepted for compatibility; VoiceML accounts have no subaccounts, so the
    /// flag does not change the result.</summary>
    public bool? IncludeSubaccounts { get; init; }

    /// <summary>Zero-based page index.</summary>
    public int? Page { get; init; }

    /// <summary>Page size.</summary>
    public int? PageSize { get; init; }

    /// <summary>Opaque cursor for the next page, provided by the API.</summary>
    public string? PageToken { get; init; }

    /// <summary>Render as a query-parameter sequence.</summary>
    public IEnumerable<KeyValuePair<string, string?>> ToQuery()
    {
        yield return new("Category", Category);
        yield return new("StartDate", StartDate);
        yield return new("EndDate", EndDate);
        yield return new("IncludeSubaccounts", FormHelpers.BoolStr(IncludeSubaccounts));
        yield return new("Page", Page?.ToString());
        yield return new("PageSize", PageSize?.ToString());
        yield return new("PageToken", PageToken);
    }
}
