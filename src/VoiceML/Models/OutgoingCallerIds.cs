using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace VoiceML.Models;

/// <summary>A Twilio-compatible OutgoingCallerId resource (<c>PN</c> + 32 hex) — a verified
/// caller-id phone number that can be set as the <c>From</c> on outbound calls. SID is the
/// same shape as an IncomingPhoneNumber SID since both surfaces share the <c>PN</c> namespace.</summary>
public sealed record OutgoingCallerId
{
    /// <summary>OutgoingCallerId SID (<c>PN</c> + 32 hex).</summary>
    [JsonPropertyName("sid")] public string? Sid { get; init; }

    /// <summary>Owning Account SID.</summary>
    [JsonPropertyName("account_sid")] public string? AccountSid { get; init; }

    /// <summary>Friendly name (free-form).</summary>
    [JsonPropertyName("friendly_name")] public string? FriendlyName { get; init; }

    /// <summary>E.164-formatted verified caller-id number.</summary>
    [JsonPropertyName("phone_number")] public string? PhoneNumber { get; init; }

    /// <summary>RFC 2822 creation timestamp.</summary>
    [JsonPropertyName("date_created")] public string? DateCreated { get; init; }

    /// <summary>RFC 2822 last-modification timestamp.</summary>
    [JsonPropertyName("date_updated")] public string? DateUpdated { get; init; }

    /// <summary>URI of this OutgoingCallerId resource.</summary>
    [JsonPropertyName("uri")] public string? Uri { get; init; }
}

/// <summary>List response for <c>GET /OutgoingCallerIds</c>.</summary>
public sealed record OutgoingCallerIdList : Page
{
    /// <summary>The page of OutgoingCallerId resources.</summary>
    [JsonPropertyName("outgoing_caller_ids")] public List<OutgoingCallerId> OutgoingCallerIds { get; init; } = new();
}

/// <summary>Response shape for <c>POST /OutgoingCallerIds</c>. Twilio dispatches a
/// validation call to the supplied number and returns the dialled verification code so
/// the application can read it back to the user. The <see cref="CallSid"/> identifies
/// the outbound validation call so applications can poll its status.</summary>
public sealed record ValidationRequest
{
    /// <summary>Owning Account SID.</summary>
    [JsonPropertyName("account_sid")] public string? AccountSid { get; init; }

    /// <summary>SID of the validation call that was dispatched.</summary>
    [JsonPropertyName("call_sid")] public string? CallSid { get; init; }

    /// <summary>Friendly name supplied on the request.</summary>
    [JsonPropertyName("friendly_name")] public string? FriendlyName { get; init; }

    /// <summary>E.164 number being validated.</summary>
    [JsonPropertyName("phone_number")] public string? PhoneNumber { get; init; }

    /// <summary>Verification code to read back to the caller.</summary>
    [JsonPropertyName("validation_code")] public string? ValidationCode { get; init; }
}

/// <summary>Query-string params for <c>GET /OutgoingCallerIds</c>.</summary>
public sealed record ListOutgoingCallerIdsParams
{
    /// <summary>Filter on the whole number, in E.164 without the <c>+</c>, or as the national
    /// ten digits of a +1 number.</summary>
    public string? PhoneNumber { get; init; }

    /// <summary>Filter on the whole friendly name, matched case-insensitively.</summary>
    public string? FriendlyName { get; init; }

    /// <summary>Zero-based page index.</summary>
    public int? Page { get; init; }

    /// <summary>Page size. Max 1000; default 50.</summary>
    public int? PageSize { get; init; }

    /// <summary>Opaque cursor for the next page, provided by the API.</summary>
    public string? PageToken { get; init; }

    /// <summary>Render as a query-parameter sequence.</summary>
    public IEnumerable<KeyValuePair<string, string?>> ToQuery()
    {
        yield return new("PhoneNumber", PhoneNumber);
        yield return new("FriendlyName", FriendlyName);
        yield return new("Page", Page?.ToString());
        yield return new("PageSize", PageSize?.ToString());
        yield return new("PageToken", PageToken);
    }
}

/// <summary>Body for <c>POST /OutgoingCallerIds</c> — start caller-ID verification. The
/// server dials the number and reads out a validation code; the response identifies the
/// validation call.</summary>
public sealed record CreateOutgoingCallerIdRequest : IFormSerializable
{
    /// <summary>The phone number to verify, in E.164 (with the leading <c>+</c>).</summary>
    public required string PhoneNumber { get; init; }

    /// <summary>Friendly name to store for the verified number.</summary>
    public string? FriendlyName { get; init; }

    /// <summary>Seconds to wait before placing the verification call (0-60).</summary>
    public int? CallDelay { get; init; }

    /// <summary>DTMF extension dialed after the verification call connects.</summary>
    public string? Extension { get; init; }

    /// <summary>Webhook fired at the end of the verification call with the verification
    /// outcome.</summary>
    public string? StatusCallback { get; init; }

    /// <summary>HTTP method for <see cref="StatusCallback"/> (<c>GET</c> or <c>POST</c>).</summary>
    public string? StatusCallbackMethod { get; init; }

    /// <inheritdoc/>
    public IEnumerable<KeyValuePair<string, string?>> ToForm()
    {
        yield return new("PhoneNumber", PhoneNumber);
        yield return new("FriendlyName", FriendlyName);
        yield return new("CallDelay", CallDelay?.ToString());
        yield return new("Extension", Extension);
        yield return new("StatusCallback", StatusCallback);
        yield return new("StatusCallbackMethod", StatusCallbackMethod);
    }
}

/// <summary>Body for <c>POST /OutgoingCallerIds/{Sid}</c> — rename a verified caller ID.</summary>
public sealed record UpdateOutgoingCallerIdRequest : IFormSerializable
{
    /// <summary>New friendly name.</summary>
    public string? FriendlyName { get; init; }

    /// <inheritdoc/>
    public IEnumerable<KeyValuePair<string, string?>> ToForm()
    {
        yield return new("FriendlyName", FriendlyName);
    }
}
