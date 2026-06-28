using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace VoiceML.Models;

/// <summary>A Twilio-compatible OutgoingCallerId resource (<c>PN</c> + 32 hex) — a verified
/// caller-id phone number that can be set as the <c>From</c> on outbound calls. SID is the
/// same shape as an IncomingPhoneNumber SID since both surfaces share the <c>PN</c> namespace.</summary>
public sealed record OutgoingCallerId
{
    /// <summary>OutgoingCallerId SID (<c>PN</c> + 32 hex).</summary>
    [JsonPropertyName("sid")] public string Sid { get; init; } = "";

    /// <summary>Owning Account SID.</summary>
    [JsonPropertyName("account_sid")] public string AccountSid { get; init; } = "";

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
    [JsonPropertyName("account_sid")] public string AccountSid { get; init; } = "";

    /// <summary>SID of the validation call that was dispatched.</summary>
    [JsonPropertyName("call_sid")] public string? CallSid { get; init; }

    /// <summary>Friendly name supplied on the request.</summary>
    [JsonPropertyName("friendly_name")] public string? FriendlyName { get; init; }

    /// <summary>E.164 number being validated.</summary>
    [JsonPropertyName("phone_number")] public string? PhoneNumber { get; init; }

    /// <summary>Verification code to read back to the caller.</summary>
    [JsonPropertyName("validation_code")] public string? ValidationCode { get; init; }
}
