using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace VoiceML.Models;

/// <summary>A Twilio-compatible Message resource — VoiceML's outbound SMS surface, backed by the
/// SDK 2.2 gateway. Outbound-only today (no MMS, no inbound webhook delivery). On the wire,
/// <c>num_segments</c> and <c>num_media</c> are string-typed (Twilio's documented shape).
/// <see cref="Status"/> pins to <c>sent</c> on a successful dispatch and <c>failed</c> otherwise —
/// the gateway is fire-and-forget, so there is no in-flight queued/sending/delivered lifecycle.</summary>
public sealed record Message
{
    /// <summary>Message SID (<c>SM</c> + 32 hex).</summary>
    [JsonPropertyName("sid")] public string Sid { get; init; } = "";

    /// <summary>Owning Account SID.</summary>
    [JsonPropertyName("account_sid")] public string AccountSid { get; init; } = "";

    /// <summary>Twilio API version label (always <c>2010-04-01</c> on this surface).</summary>
    [JsonPropertyName("api_version")] public string ApiVersion { get; init; } = "";

    /// <summary>Destination phone number, E.164.</summary>
    [JsonPropertyName("to")] public string To { get; init; } = "";

    /// <summary>Source phone number. Mapped from JSON <c>from</c>; <c>from</c> is a C# keyword,
    /// so the property is named <c>From</c> here.</summary>
    [JsonPropertyName("from")] public string From { get; init; } = "";

    /// <summary>Message text. Empty string after a redaction update.</summary>
    [JsonPropertyName("body")] public string Body { get; init; } = "";

    /// <summary>Message status. One of <c>queued</c>, <c>sending</c>, <c>sent</c>, <c>failed</c>,
    /// <c>delivered</c>, <c>undelivered</c>, <c>receiving</c>, <c>received</c>, <c>accepted</c>,
    /// <c>scheduled</c>, <c>read</c>, <c>canceled</c>. See <see cref="MessageStatus"/> for the
    /// allowed string constants.</summary>
    [JsonPropertyName("status")] public string Status { get; init; } = "";

    /// <summary>Per-message segment count. Twilio surfaces this as a string on the wire
    /// (not an integer) — the SDK preserves that shape for strict-binding compatibility.</summary>
    [JsonPropertyName("num_segments")] public string NumSegments { get; init; } = "";

    /// <summary>Media-attachment count. Always <c>"0"</c> today (no MMS support).
    /// String-typed on the wire, like <see cref="NumSegments"/>.</summary>
    [JsonPropertyName("num_media")] public string NumMedia { get; init; } = "";

    /// <summary>Message direction: <c>outbound-api</c>, <c>outbound-call</c>,
    /// <c>outbound-reply</c>, or <c>inbound</c>.</summary>
    [JsonPropertyName("direction")] public string Direction { get; init; } = "";

    /// <summary>Message price as a decimal string. <c>null</c> when not yet rated.</summary>
    [JsonPropertyName("price")] public string? Price { get; init; }

    /// <summary>Currency of <see cref="Price"/> (ISO 4217). <c>null</c> when not yet rated.</summary>
    [JsonPropertyName("price_unit")] public string? PriceUnit { get; init; }

    /// <summary>Twilio-compat error code on failure (e.g. <c>21609</c> = gateway not configured,
    /// <c>30001</c> = upstream gateway failure). <c>null</c> on success.</summary>
    [JsonPropertyName("error_code")] public int? ErrorCode { get; init; }

    /// <summary>Sanitised error message — never contains upstream URLs or credentials.
    /// <c>null</c> on success.</summary>
    [JsonPropertyName("error_message")] public string? ErrorMessage { get; init; }

    /// <summary>Messaging Service SID, when the message was sent through one. <c>null</c> otherwise.</summary>
    [JsonPropertyName("messaging_service_sid")] public string? MessagingServiceSid { get; init; }

    /// <summary>RFC 2822 creation timestamp.</summary>
    [JsonPropertyName("date_created")] public string DateCreated { get; init; } = "";

    /// <summary>RFC 2822 last-modification timestamp.</summary>
    [JsonPropertyName("date_updated")] public string DateUpdated { get; init; } = "";

    /// <summary>RFC 2822 send timestamp. <c>null</c> until the message has been dispatched.</summary>
    [JsonPropertyName("date_sent")] public string? DateSent { get; init; }

    /// <summary>URI of this Message resource.</summary>
    [JsonPropertyName("uri")] public string Uri { get; init; } = "";

    /// <summary>Map of subresource name → URI (currently <c>media</c> and <c>feedback</c>).</summary>
    [JsonPropertyName("subresource_uris")] public Dictionary<string, string>? SubresourceUris { get; init; }
}

/// <summary>List response for <c>GET /Messages</c>. Pagination fields are inherited from <see cref="Page"/>.</summary>
public sealed record MessageList : Page
{
    /// <summary>The page of Message resources.</summary>
    [JsonPropertyName("messages")] public List<Message> Messages { get; init; } = new();
}

/// <summary>Body for <c>POST /Messages</c>. <see cref="To"/> and <see cref="Body"/> are required;
/// <see cref="From"/> falls back to the tenant's configured default sender when omitted.</summary>
public sealed record CreateMessageRequest : IFormSerializable
{
    /// <summary>Destination phone number (E.164).</summary>
    public required string To { get; init; }

    /// <summary>Message text.</summary>
    public required string Body { get; init; }

    /// <summary>Source phone number; falls back to the tenant's configured default sender.</summary>
    public string? From { get; init; }

    /// <summary>Messaging Service SID. Accepted for compatibility; not yet routed against.</summary>
    public string? MessagingServiceSid { get; init; }

    /// <summary>Status-callback URL. Reserved — outbound SMS is fire-and-forget today.</summary>
    public string? StatusCallback { get; init; }

    /// <inheritdoc/>
    public IEnumerable<KeyValuePair<string, string?>> ToForm()
    {
        yield return new("To", To);
        yield return new("Body", Body);
        yield return new("From", From);
        yield return new("MessagingServiceSid", MessagingServiceSid);
        yield return new("StatusCallback", StatusCallback);
    }
}

/// <summary>Query-string params for <c>GET /Messages</c>. Twilio uses the literal query-names
/// <c>DateSent&lt;</c> / <c>DateSent&gt;</c>; the SDK keeps them verbatim on the wire,
/// modelled here as <see cref="DateSentLt"/> / <see cref="DateSentGt"/>.</summary>
public sealed record ListMessagesParams
{
    /// <summary>Filter by destination number.</summary>
    public string? To { get; init; }

    /// <summary>Filter by source number.</summary>
    public string? From { get; init; }

    /// <summary>Messages sent on this UTC date (<c>YYYY-MM-DD</c>). Wire name: <c>DateSent</c>.</summary>
    public string? DateSent { get; init; }

    /// <summary>Messages sent strictly before this UTC date/time. Wire name: <c>DateSent&lt;</c>.</summary>
    public string? DateSentLt { get; init; }

    /// <summary>Messages sent strictly after this UTC date/time. Wire name: <c>DateSent&gt;</c>.</summary>
    public string? DateSentGt { get; init; }

    /// <summary>Zero-based page index.</summary>
    public int? Page { get; init; }

    /// <summary>Page size.</summary>
    public int? PageSize { get; init; }

    /// <summary>Opaque cursor for the next page (spec v0.6.4).</summary>
    public string? PageToken { get; init; }

    /// <summary>Render as the Twilio-compatible query-parameter sequence.</summary>
    public IEnumerable<KeyValuePair<string, string?>> ToQuery()
    {
        yield return new("To", To);
        yield return new("From", From);
        yield return new("DateSent", DateSent);
        yield return new("DateSent<", DateSentLt);
        yield return new("DateSent>", DateSentGt);
        yield return new("Page", Page?.ToString());
        yield return new("PageSize", PageSize?.ToString());
        yield return new("PageToken", PageToken);
    }
}

/// <summary>Body for <c>POST /Messages/{Sid}</c>. Only <c>Body=""</c> (redaction) is honoured today;
/// <c>Status=canceled</c> returns 21610 because the SDK 2.2 gateway is fire-and-forget.</summary>
public sealed record UpdateMessageRequest : IFormSerializable
{
    /// <summary>Pass empty string to redact. Non-empty Body is ignored — Twilio's documented
    /// redaction semantics.</summary>
    public string? Body { get; init; }

    /// <summary>Cancellation request. Only <c>canceled</c> is meaningful on the wire; returns
    /// 21610 because outbound SMS is fire-and-forget.</summary>
    public string? Status { get; init; }

    /// <inheritdoc/>
    public IEnumerable<KeyValuePair<string, string?>> ToForm()
    {
        yield return new("Body", Body);
        yield return new("Status", Status);
    }
}
