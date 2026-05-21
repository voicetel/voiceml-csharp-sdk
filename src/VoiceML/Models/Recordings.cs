using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace VoiceML.Models;

/// <summary>A Twilio-shape Recording resource.</summary>
public sealed record Recording
{
    /// <summary>Recording SID (<c>RE</c> + 32 hex).</summary>
    [JsonPropertyName("sid")] public string Sid { get; init; } = "";

    /// <summary>Owning Account SID.</summary>
    [JsonPropertyName("account_sid")] public string AccountSid { get; init; } = "";

    /// <summary>Source Call SID.</summary>
    [JsonPropertyName("call_sid")] public string CallSid { get; init; } = "";

    /// <summary>Source Conference SID, if conference-scoped.</summary>
    [JsonPropertyName("conference_sid")] public string? ConferenceSid { get; init; }

    /// <summary>Recording status (<c>in-progress</c>, <c>completed</c>, <c>failed</c>, <c>absent</c>,
    /// <c>paused</c>, <c>stopped</c>, <c>processing</c>).</summary>
    [JsonPropertyName("status")] public string Status { get; init; } = "";

    /// <summary>Recording source enum (<c>OutboundAPI</c>, <c>RecordVerb</c>, <c>DialVerb</c>,
    /// <c>Conference</c>, <c>Trunking</c>, <c>StartCallRecordingAPI</c>,
    /// <c>StartConferenceRecordingAPI</c>).</summary>
    [JsonPropertyName("source")] public string? Source { get; init; }

    /// <summary>Number of audio channels.</summary>
    [JsonPropertyName("channels")] public int? Channels { get; init; }

    /// <summary>Duration in seconds, as a decimal string.</summary>
    [JsonPropertyName("duration")] public string? Duration { get; init; }

    /// <summary>Twilio API version label.</summary>
    [JsonPropertyName("api_version")] public string? ApiVersion { get; init; }

    /// <summary>URI of this recording.</summary>
    [JsonPropertyName("uri")] public string? Uri { get; init; }

    /// <summary>Direct download URL for the recording media (S3 presigned URL or equivalent).
    /// Added in spec v0.6.2 (D5). When present, callers can fetch the audio bytes without going
    /// through the SDK's <c>.wav</c> redirect path.</summary>
    [JsonPropertyName("media_url")] public string? MediaUrl { get; init; }

    /// <summary>RFC 3339 creation timestamp.</summary>
    [JsonPropertyName("date_created")] public string? DateCreated { get; init; }

    /// <summary>RFC 3339 last-modification timestamp.</summary>
    [JsonPropertyName("date_updated")] public string? DateUpdated { get; init; }

    /// <summary>RFC 3339 recording start timestamp.</summary>
    [JsonPropertyName("start_time")] public string? StartTime { get; init; }

    /// <summary>Recording price as a decimal string.</summary>
    [JsonPropertyName("price")] public string? Price { get; init; }

    /// <summary>Currency of <see cref="Price"/>.</summary>
    [JsonPropertyName("price_unit")] public string? PriceUnit { get; init; }

    /// <summary>Encryption-details object (server-side encryption metadata).</summary>
    [JsonPropertyName("encryption_details")] public Dictionary<string, object>? EncryptionDetails { get; init; }

    /// <summary>Map of subresource name → URI.</summary>
    [JsonPropertyName("subresource_uris")] public Dictionary<string, object>? SubresourceUris { get; init; }

    /// <summary>Twilio-canonical per-call error taxonomy for failed recordings. <c>null</c> when
    /// no error (never <c>0</c> on the wire).</summary>
    [JsonPropertyName("error_code")] public int? ErrorCode { get; init; }
}

/// <summary>Recordings list response. The account-scoped endpoint returns the canonical Twilio
/// pagination fields; per-call and per-conference endpoints currently return only the
/// <c>recordings</c> array.</summary>
public sealed record RecordingList
{
    /// <summary>The page of Recording resources.</summary>
    [JsonPropertyName("recordings")] public List<Recording> Recordings { get; init; } = new();

    /// <summary>Zero-based page index.</summary>
    [JsonPropertyName("page")] public int? Page { get; init; }

    /// <summary>Page size.</summary>
    [JsonPropertyName("page_size")] public int? PageSize { get; init; }

    /// <summary>Total items.</summary>
    [JsonPropertyName("total")] public int? Total { get; init; }

    /// <summary>Total pages.</summary>
    [JsonPropertyName("num_pages")] public int? NumPages { get; init; }

    /// <summary>URI of the first page.</summary>
    [JsonPropertyName("first_page_uri")] public string? FirstPageUri { get; init; }

    /// <summary>URI of the next page.</summary>
    [JsonPropertyName("next_page_uri")] public string? NextPageUri { get; init; }

    /// <summary>URI of the previous page.</summary>
    [JsonPropertyName("previous_page_uri")] public string? PreviousPageUri { get; init; }

    /// <summary>URI of this collection.</summary>
    [JsonPropertyName("uri")] public string? Uri { get; init; }
}

/// <summary>Query-string params for recording list endpoints (<c>GET /Recordings</c> and
/// <c>GET /Calls/{sid}/Recordings</c>).</summary>
public sealed record ListRecordingsParams
{
    /// <summary>Filter to recordings created on this UTC date (<c>YYYY-MM-DD</c>).</summary>
    public string? DateCreated { get; init; }

    /// <summary>Recordings created strictly before this UTC date/time. Wire name: <c>DateCreated&lt;</c>.</summary>
    public string? DateCreatedLt { get; init; }

    /// <summary>Recordings created strictly after this UTC date/time. Wire name: <c>DateCreated&gt;</c>.</summary>
    public string? DateCreatedGt { get; init; }

    /// <summary>Filter to recordings whose CallSid equals this value (account-scoped list only).</summary>
    public string? CallSid { get; init; }

    /// <summary>Filter to recordings whose ConferenceSid equals this value (account-scoped list only).</summary>
    public string? ConferenceSid { get; init; }

    /// <summary>Zero-based page index.</summary>
    public int? Page { get; init; }

    /// <summary>Page size.</summary>
    public int? PageSize { get; init; }

    /// <summary>Render as a query-parameter sequence.</summary>
    public IEnumerable<KeyValuePair<string, string?>> ToQuery()
    {
        yield return new("DateCreated", DateCreated);
        yield return new("DateCreated<", DateCreatedLt);
        yield return new("DateCreated>", DateCreatedGt);
        yield return new("CallSid", CallSid);
        yield return new("ConferenceSid", ConferenceSid);
        yield return new("Page", Page?.ToString());
        yield return new("PageSize", PageSize?.ToString());
    }
}

/// <summary>Body for <c>POST /Calls/{sid}/Recordings</c>.</summary>
public sealed record StartRecordingRequest : IFormSerializable
{
    /// <summary>Max duration in seconds.</summary>
    public int? RecordingMaxDuration { get; init; }

    /// <summary>Channels: <c>mono</c> or <c>dual</c>.</summary>
    public string? RecordingChannels { get; init; }

    /// <summary>Play a beep when recording starts.</summary>
    public bool? PlayBeep { get; init; }

    /// <summary>Status-callback URL.</summary>
    public string? RecordingStatusCallback { get; init; }

    /// <summary>HTTP method for the status callback.</summary>
    public string? RecordingStatusCallbackMethod { get; init; }

    /// <summary>Status-callback events to subscribe to.</summary>
    public string? RecordingStatusCallbackEvent { get; init; }

    /// <inheritdoc/>
    public IEnumerable<KeyValuePair<string, string?>> ToForm()
    {
        yield return new("RecordingMaxDuration", RecordingMaxDuration?.ToString());
        yield return new("RecordingChannels", RecordingChannels);
        yield return new("PlayBeep", FormHelpers.BoolStr(PlayBeep));
        yield return new("RecordingStatusCallback", RecordingStatusCallback);
        yield return new("RecordingStatusCallbackMethod", RecordingStatusCallbackMethod);
        yield return new("RecordingStatusCallbackEvent", RecordingStatusCallbackEvent);
    }
}

/// <summary>Body for <c>POST /Calls/{sid}/Recordings/{rsid}</c> — stop / pause / resume.</summary>
public sealed record UpdateRecordingRequest : IFormSerializable
{
    /// <summary>One of <c>stopped</c>, <c>paused</c>, <c>in-progress</c>.</summary>
    public required string Status { get; init; }

    /// <inheritdoc/>
    public IEnumerable<KeyValuePair<string, string?>> ToForm()
    {
        yield return new("Status", Status);
    }
}

/// <summary>Result of fetching <c>GET /Recordings/{sid}.wav</c>.</summary>
public sealed record RecordingAudio
{
    /// <summary>The Recording SID.</summary>
    public string Sid { get; init; } = "";

    /// <summary>The WAV bytes (after following any S3 redirect).</summary>
    public byte[] Content { get; init; } = System.Array.Empty<byte>();

    /// <summary>Content-Type header (typically <c>audio/wav</c>).</summary>
    public string ContentType { get; init; } = "application/octet-stream";

    /// <summary>True when the audio came via an S3 presigned-URL redirect.</summary>
    public bool ViaRedirect { get; init; }
}
