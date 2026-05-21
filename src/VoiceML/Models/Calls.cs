using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace VoiceML.Models;

/// <summary>A Twilio-shape Call resource.</summary>
public sealed record Call
{
    /// <summary>Twilio-format Call SID (<c>CA</c> + 32 hex).</summary>
    [JsonPropertyName("sid")] public string Sid { get; init; } = "";

    /// <summary>Account SID that owns this call.</summary>
    [JsonPropertyName("account_sid")] public string AccountSid { get; init; } = "";

    /// <summary>Twilio API version label (always <c>2010-04-01</c> for this surface).</summary>
    [JsonPropertyName("api_version")] public string ApiVersion { get; init; } = "";

    /// <summary>Destination phone number, E.164 (or SIP URI).</summary>
    [JsonPropertyName("to")] public string? To { get; init; }

    /// <summary>National-format rendering of <see cref="To"/>.</summary>
    [JsonPropertyName("to_formatted")] public string? ToFormatted { get; init; }

    /// <summary>Originating phone number, E.164 (or SIP URI). Mapped from JSON <c>from</c>;
    /// <c>from</c> is a C# keyword, so the property is named <c>From</c> here.</summary>
    [JsonPropertyName("from")] public string? From { get; init; }

    /// <summary>National-format rendering of <see cref="From"/>.</summary>
    [JsonPropertyName("from_formatted")] public string? FromFormatted { get; init; }

    /// <summary>SID of the parent (A-leg) call when this is a B-leg created by <c>&lt;Dial&gt;</c>.</summary>
    [JsonPropertyName("parent_call_sid")] public string? ParentCallSid { get; init; }

    /// <summary>SIP <c>From:</c> display-name captured on inbound calls (CNAM-style).</summary>
    [JsonPropertyName("caller_name")] public string? CallerName { get; init; }

    /// <summary>Originator of a forwarded inbound call, from SIP <c>Diversion</c> / <c>History-Info</c>.</summary>
    [JsonPropertyName("forwarded_from")] public string? ForwardedFrom { get; init; }

    /// <summary>Call status. One of <c>queued</c>, <c>ringing</c>, <c>in-progress</c>,
    /// <c>completed</c>, <c>busy</c>, <c>no-answer</c>, <c>canceled</c>, <c>failed</c>.</summary>
    [JsonPropertyName("status")] public string Status { get; init; } = "";

    /// <summary>Call direction: <c>inbound</c>, <c>outbound-api</c>, or <c>outbound-dial</c>.</summary>
    [JsonPropertyName("direction")] public string Direction { get; init; } = "";

    /// <summary>AMD verdict (Twilio-strict enum: <c>human</c>, <c>machine_start</c>, <c>machine_end_*</c>,
    /// <c>fax</c>, <c>unknown</c>, or empty string).</summary>
    [JsonPropertyName("answered_by")] public string? AnsweredBy { get; init; }

    /// <summary>RFC 3339 call start time.</summary>
    [JsonPropertyName("start_time")] public string? StartTime { get; init; }

    /// <summary>RFC 3339 call end time.</summary>
    [JsonPropertyName("end_time")] public string? EndTime { get; init; }

    /// <summary>Call duration in seconds, as a decimal string.</summary>
    [JsonPropertyName("duration")] public string? Duration { get; init; }

    /// <summary>Call price as a decimal string.</summary>
    [JsonPropertyName("price")] public string? Price { get; init; }

    /// <summary>Currency of <see cref="Price"/> (ISO 4217).</summary>
    [JsonPropertyName("price_unit")] public string? PriceUnit { get; init; }

    /// <summary>Phone-number SID (Twilio-format).</summary>
    [JsonPropertyName("phone_number_sid")] public string? PhoneNumberSid { get; init; }

    /// <summary>Annotation field.</summary>
    [JsonPropertyName("annotation")] public string? Annotation { get; init; }

    /// <summary>Group SID.</summary>
    [JsonPropertyName("group_sid")] public string? GroupSid { get; init; }

    /// <summary>Queue time in seconds, as a decimal string.</summary>
    [JsonPropertyName("queue_time")] public string? QueueTime { get; init; }

    /// <summary>Trunk SID.</summary>
    [JsonPropertyName("trunk_sid")] public string? TrunkSid { get; init; }

    /// <summary>RFC 3339 creation timestamp.</summary>
    [JsonPropertyName("date_created")] public string DateCreated { get; init; } = "";

    /// <summary>RFC 3339 last-modification timestamp.</summary>
    [JsonPropertyName("date_updated")] public string DateUpdated { get; init; } = "";

    /// <summary>URI of this Call resource.</summary>
    [JsonPropertyName("uri")] public string Uri { get; init; } = "";

    /// <summary>Map of subresource name → URI (recordings, streams, transcriptions...).</summary>
    [JsonPropertyName("subresource_uris")] public Dictionary<string, string>? SubresourceUris { get; init; }
}

/// <summary>List response for <c>GET /Calls</c>. Pagination fields are inherited from <see cref="Page"/>.</summary>
public sealed record CallList : Page
{
    /// <summary>The page of Call resources.</summary>
    [JsonPropertyName("calls")] public List<Call> Calls { get; init; } = new();
}

/// <summary>Body for <c>POST /Calls</c>. Set at most one of <see cref="Url"/> /
/// <see cref="Twiml"/> / <see cref="ApplicationSid"/> (Twiml wins if multiple are set —
/// Twilio's documented precedence).</summary>
public sealed record CreateCallRequest : IFormSerializable
{
    /// <summary>Destination phone number (E.164) or SIP URI.</summary>
    public required string To { get; init; }

    /// <summary>Originating phone number (E.164) or SIP URI.</summary>
    public required string From { get; init; }

    /// <summary>URL to fetch TwiML from when the call is answered.</summary>
    public string? Url { get; init; }

    /// <summary>HTTP method for the <see cref="Url"/> fetch (<c>GET</c> or <c>POST</c>).</summary>
    public string? Method { get; init; }

    /// <summary>Inline TwiML to execute. Wins over <see cref="Url"/> when both are set.</summary>
    public string? Twiml { get; init; }

    /// <summary>SID of a persistent Application bundle to dispatch.</summary>
    public string? ApplicationSid { get; init; }

    /// <summary>Fallback URL fetched when the primary TwiML source errors.</summary>
    public string? FallbackUrl { get; init; }

    /// <summary>HTTP method for the fallback URL fetch.</summary>
    public string? FallbackMethod { get; init; }

    /// <summary>URL to POST call-status callbacks to.</summary>
    public string? StatusCallback { get; init; }

    /// <summary>HTTP method for the status-callback POST.</summary>
    public string? StatusCallbackMethod { get; init; }

    /// <summary>Status-callback events to subscribe to (e.g. <c>initiated</c>, <c>ringing</c>,
    /// <c>answered</c>, <c>completed</c>). Repeated form parameter on the wire.</summary>
    public List<string>? StatusCallbackEvent { get; init; }

    /// <summary>AMD mode: <c>Enable</c> or <c>DetectMessageEnd</c>.</summary>
    public string? MachineDetection { get; init; }

    /// <summary>AMD overall timeout in seconds.</summary>
    public int? MachineDetectionTimeout { get; init; }

    /// <summary>AMD speech-threshold in milliseconds.</summary>
    public int? MachineDetectionSpeechThreshold { get; init; }

    /// <summary>AMD speech-end-threshold in milliseconds.</summary>
    public int? MachineDetectionSpeechEndThreshold { get; init; }

    /// <summary>AMD silence timeout in milliseconds.</summary>
    public int? MachineDetectionSilenceTimeout { get; init; }

    /// <summary>URL to POST async AMD results to.</summary>
    public string? AsyncAmdStatusCallback { get; init; }

    /// <summary>HTTP method for the async-AMD callback.</summary>
    public string? AsyncAmdStatusCallbackMethod { get; init; }

    /// <summary>Whether to record the call (true/false).</summary>
    public bool? Record { get; init; }

    /// <summary>URL to POST recording-status callbacks to.</summary>
    public string? RecordingStatusCallback { get; init; }

    /// <summary>HTTP method for the recording-status callback.</summary>
    public string? RecordingStatusCallbackMethod { get; init; }

    /// <summary>Recording-status events to subscribe to.</summary>
    public string? RecordingStatusCallbackEvent { get; init; }

    /// <summary>Recording channels: <c>mono</c> or <c>dual</c>.</summary>
    public string? RecordingChannels { get; init; }

    /// <summary>Recording track: <c>inbound</c>, <c>outbound</c>, or <c>both</c>.</summary>
    public string? RecordingTrack { get; init; }

    /// <summary>Recording trim mode: <c>trim-silence</c> or <c>do-not-trim</c>.</summary>
    public string? Trim { get; init; }

    /// <summary>Ring timeout in seconds.</summary>
    public int? Timeout { get; init; }

    /// <summary>DTMF digits to send after the call is answered.</summary>
    public string? SendDigits { get; init; }

    /// <summary>Caller ID override (E.164 or short name).</summary>
    public string? CallerId { get; init; }

    /// <summary>Call reason (free-form).</summary>
    public string? CallReason { get; init; }

    /// <summary>SIP auth username for the originate.</summary>
    public string? SipAuthUsername { get; init; }

    /// <summary>SIP auth password for the originate.</summary>
    public string? SipAuthPassword { get; init; }

    /// <summary>BYOC (Bring-Your-Own-Carrier) trunk SID.</summary>
    public string? Byoc { get; init; }

    /// <summary>Run AMD asynchronously.</summary>
    public bool? AsyncAmd { get; init; }

    /// <summary>Opaque call token.</summary>
    public string? CallToken { get; init; }

    /// <inheritdoc/>
    public IEnumerable<KeyValuePair<string, string?>> ToForm()
    {
        yield return new("To", To);
        yield return new("From", From);
        yield return new("Url", Url);
        yield return new("Method", Method);
        yield return new("Twiml", Twiml);
        yield return new("ApplicationSid", ApplicationSid);
        yield return new("FallbackUrl", FallbackUrl);
        yield return new("FallbackMethod", FallbackMethod);
        yield return new("StatusCallback", StatusCallback);
        yield return new("StatusCallbackMethod", StatusCallbackMethod);
        if (StatusCallbackEvent is not null)
        {
            foreach (var ev in StatusCallbackEvent)
            {
                yield return new("StatusCallbackEvent", ev);
            }
        }
        yield return new("MachineDetection", MachineDetection);
        yield return new("MachineDetectionTimeout", MachineDetectionTimeout?.ToString());
        yield return new("MachineDetectionSpeechThreshold", MachineDetectionSpeechThreshold?.ToString());
        yield return new("MachineDetectionSpeechEndThreshold", MachineDetectionSpeechEndThreshold?.ToString());
        yield return new("MachineDetectionSilenceTimeout", MachineDetectionSilenceTimeout?.ToString());
        yield return new("AsyncAmdStatusCallback", AsyncAmdStatusCallback);
        yield return new("AsyncAmdStatusCallbackMethod", AsyncAmdStatusCallbackMethod);
        yield return new("Record", FormHelpers.BoolStr(Record));
        yield return new("RecordingStatusCallback", RecordingStatusCallback);
        yield return new("RecordingStatusCallbackMethod", RecordingStatusCallbackMethod);
        yield return new("RecordingStatusCallbackEvent", RecordingStatusCallbackEvent);
        yield return new("RecordingChannels", RecordingChannels);
        yield return new("RecordingTrack", RecordingTrack);
        yield return new("Trim", Trim);
        yield return new("Timeout", Timeout?.ToString());
        yield return new("SendDigits", SendDigits);
        yield return new("CallerId", CallerId);
        yield return new("CallReason", CallReason);
        yield return new("SipAuthUsername", SipAuthUsername);
        yield return new("SipAuthPassword", SipAuthPassword);
        yield return new("Byoc", Byoc);
        yield return new("AsyncAmd", FormHelpers.BoolStr(AsyncAmd));
        yield return new("CallToken", CallToken);
    }
}

/// <summary>Body for <c>POST /Calls/{sid}</c>. Three flows on the same endpoint:
/// <list type="bullet">
///   <item><description><c>Status="completed"|"canceled"</c> — terminate the call.</description></item>
///   <item><description><c>Twiml=&lt;inline&gt;</c> — execute inline TwiML on the live call.</description></item>
///   <item><description><c>Url=…</c> — fetch new TwiML and execute on the live call.</description></item>
/// </list></summary>
public sealed record UpdateCallRequest : IFormSerializable
{
    /// <summary>One of <c>completed</c> or <c>canceled</c> — terminates the call.</summary>
    public string? Status { get; init; }

    /// <summary>Inline TwiML to execute on the live call.</summary>
    public string? Twiml { get; init; }

    /// <summary>URL to fetch new TwiML from for the live call.</summary>
    public string? Url { get; init; }

    /// <summary>HTTP method for the URL fetch.</summary>
    public string? Method { get; init; }

    /// <summary>Fallback URL.</summary>
    public string? FallbackUrl { get; init; }

    /// <summary>HTTP method for the fallback URL.</summary>
    public string? FallbackMethod { get; init; }

    /// <summary>Status-callback URL.</summary>
    public string? StatusCallback { get; init; }

    /// <summary>HTTP method for the status-callback POST.</summary>
    public string? StatusCallbackMethod { get; init; }

    /// <summary>Status-callback events to subscribe to.</summary>
    public List<string>? StatusCallbackEvent { get; init; }

    /// <inheritdoc/>
    public IEnumerable<KeyValuePair<string, string?>> ToForm()
    {
        yield return new("Status", Status);
        yield return new("Twiml", Twiml);
        yield return new("Url", Url);
        yield return new("Method", Method);
        yield return new("FallbackUrl", FallbackUrl);
        yield return new("FallbackMethod", FallbackMethod);
        yield return new("StatusCallback", StatusCallback);
        yield return new("StatusCallbackMethod", StatusCallbackMethod);
        if (StatusCallbackEvent is not null)
        {
            foreach (var ev in StatusCallbackEvent)
            {
                yield return new("StatusCallbackEvent", ev);
            }
        }
    }
}

/// <summary>Query-string params for <c>GET /Calls</c>. Twilio uses the literal query-name
/// <c>StartTime&gt;=</c> / <c>StartTime&lt;=</c>; the SDK keeps them verbatim on the wire,
/// modelled here as <see cref="StartTimeGte"/> / <see cref="StartTimeLte"/>.</summary>
public sealed record ListCallsParams
{
    /// <summary>Filter by destination number.</summary>
    public string? To { get; init; }

    /// <summary>Filter by originating number.</summary>
    public string? From { get; init; }

    /// <summary>Filter by call status.</summary>
    public string? Status { get; init; }

    /// <summary>Filter to B-legs of the given parent Call SID.</summary>
    public string? ParentCallSid { get; init; }

    /// <summary>Calls started on this UTC date (<c>YYYY-MM-DD</c>). Wire name: <c>StartTime</c>.</summary>
    public string? StartTime { get; init; }

    /// <summary>Calls started strictly before this UTC date/time. Wire name: <c>StartTime&lt;</c>.</summary>
    public string? StartTimeLt { get; init; }

    /// <summary>Calls started strictly after this UTC date/time. Wire name: <c>StartTime&gt;</c>.</summary>
    public string? StartTimeGt { get; init; }

    /// <summary>Earliest start_time (inclusive). Serialized as <c>StartTime&gt;=</c>.</summary>
    public string? StartTimeGte { get; init; }

    /// <summary>Latest start_time (inclusive). Serialized as <c>StartTime&lt;=</c>.</summary>
    public string? StartTimeLte { get; init; }

    /// <summary>Calls ended on this UTC date (<c>YYYY-MM-DD</c>). Wire name: <c>EndTime</c>.</summary>
    public string? EndTime { get; init; }

    /// <summary>Calls ended strictly before this UTC date/time. Wire name: <c>EndTime&lt;</c>.</summary>
    public string? EndTimeLt { get; init; }

    /// <summary>Calls ended strictly after this UTC date/time. Wire name: <c>EndTime&gt;</c>.</summary>
    public string? EndTimeGt { get; init; }

    /// <summary>Zero-based page index.</summary>
    public int? Page { get; init; }

    /// <summary>Page size.</summary>
    public int? PageSize { get; init; }

    /// <summary>Render as the Twilio-shape query-parameter sequence.</summary>
    public IEnumerable<KeyValuePair<string, string?>> ToQuery()
    {
        yield return new("To", To);
        yield return new("From", From);
        yield return new("Status", Status);
        yield return new("ParentCallSid", ParentCallSid);
        yield return new("StartTime", StartTime);
        yield return new("StartTime<", StartTimeLt);
        yield return new("StartTime>", StartTimeGt);
        // Note: literal Twilio query-name with `>=` / `<=`.
        yield return new("StartTime>=", StartTimeGte);
        yield return new("StartTime<=", StartTimeLte);
        yield return new("EndTime", EndTime);
        yield return new("EndTime<", EndTimeLt);
        yield return new("EndTime>", EndTimeGt);
        yield return new("Page", Page?.ToString());
        yield return new("PageSize", PageSize?.ToString());
    }
}

/// <summary>Pagination params for compat-stub list endpoints (Notifications, Events).</summary>
public sealed record ListPageParams
{
    /// <summary>Zero-based page index.</summary>
    public int? Page { get; init; }

    /// <summary>Page size.</summary>
    public int? PageSize { get; init; }

    /// <summary>Render as a query-parameter sequence.</summary>
    public IEnumerable<KeyValuePair<string, string?>> ToQuery()
    {
        yield return new("Page", Page?.ToString());
        yield return new("PageSize", PageSize?.ToString());
    }
}

/// <summary>List response for <c>GET /Calls/{sid}/Notifications</c> — always empty (compat stub).</summary>
public sealed record NotificationsList
{
    /// <summary>The notifications (always empty).</summary>
    [JsonPropertyName("notifications")] public List<object> Notifications { get; init; } = new();

    /// <summary>Zero-based page index.</summary>
    [JsonPropertyName("page")] public int Page { get; init; }

    /// <summary>Page size.</summary>
    [JsonPropertyName("page_size")] public int PageSize { get; init; }

    /// <summary>Total items.</summary>
    [JsonPropertyName("total")] public int Total { get; init; }

    /// <summary>URI of this collection.</summary>
    [JsonPropertyName("uri")] public string? Uri { get; init; }
}

/// <summary>List response for <c>GET /Calls/{sid}/Events</c> — always empty (compat stub).
/// The canonical event source is the customer's StatusCallback URL.</summary>
public sealed record EventsList
{
    /// <summary>The events (always empty).</summary>
    [JsonPropertyName("events")] public List<object> Events { get; init; } = new();

    /// <summary>Zero-based page index.</summary>
    [JsonPropertyName("page")] public int Page { get; init; }

    /// <summary>Page size.</summary>
    [JsonPropertyName("page_size")] public int PageSize { get; init; }

    /// <summary>Total items.</summary>
    [JsonPropertyName("total")] public int Total { get; init; }

    /// <summary>URI of this collection.</summary>
    [JsonPropertyName("uri")] public string? Uri { get; init; }
}
