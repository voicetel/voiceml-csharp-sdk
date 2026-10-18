using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace VoiceML.Models;

// ===========================================================================
// Messaging v1 (messaging.twilio.com/v1)
// A Messaging Service (MG…) shares the /v1/Services path shape with the
// Conversations Service (IS…); the two are disambiguated on the wire by host
// (messaging.voicetel.com vs conversations.voicetel.com). This SDK routes
// client.MessagingV1.* at the messaging host automatically (see ProductHosts).
// Only the Messaging Service has an update verb, so POST /v1/Services/{sid}
// has no path collision.
// ===========================================================================

/// <summary>A Messaging Service — Twilio <c>MG…</c> resource.
/// <para>The feature-toggle fields (<c>sticky_sender</c>, <c>mms_converter</c>, …) are
/// accept-and-echo on VoiceML; the service's operative role is gating scheduled sends.</para>
/// </summary>
public sealed record MessagingService
{
    /// <summary>Unique SID identifying this resource.</summary>
    [JsonPropertyName("sid")] public string? Sid { get; init; }
    /// <summary>SID of the account that owns this resource.</summary>
    [JsonPropertyName("account_sid")] public string? AccountSid { get; init; }
    /// <summary>Human-readable name used to identify this resource.</summary>
    [JsonPropertyName("friendly_name")] public string? FriendlyName { get; init; }
    /// <summary>Timestamp when this resource was created.</summary>
    [JsonPropertyName("date_created")] public string? DateCreated { get; init; }
    /// <summary>Timestamp of the most recent modification to this resource.</summary>
    [JsonPropertyName("date_updated")] public string? DateUpdated { get; init; }
    /// <summary>URL used for inbound request.</summary>
    [JsonPropertyName("inbound_request_url")] public string? InboundRequestUrl { get; init; }
    /// <summary>HTTP method for inbound requests.</summary>
    [JsonPropertyName("inbound_method")] public string? InboundMethod { get; init; }
    /// <summary>URL used for fallback.</summary>
    [JsonPropertyName("fallback_url")] public string? FallbackUrl { get; init; }
    /// <summary>HTTP method for fallback requests.</summary>
    [JsonPropertyName("fallback_method")] public string? FallbackMethod { get; init; }
    /// <summary>Callback URL for message delivery status notifications.</summary>
    [JsonPropertyName("status_callback")] public string? StatusCallback { get; init; }
    /// <summary>Whether the sender is consistently selected for a recipient across messages.</summary>
    [JsonPropertyName("sticky_sender")] public bool? StickySender { get; init; }
    /// <summary>Whether media messages may be converted for recipients without MMS capability.</summary>
    [JsonPropertyName("mms_converter")] public bool? MmsConverter { get; init; }
    /// <summary>Whether text encoding substitutions are enabled for message delivery.</summary>
    [JsonPropertyName("smart_encoding")] public bool? SmartEncoding { get; init; }
    /// <summary>Configured message-content scanning policy.</summary>
    [JsonPropertyName("scan_message_content")] public string? ScanMessageContent { get; init; }
    /// <summary>Whether a long-code sender may be selected when the preferred sender is unavailable.</summary>
    [JsonPropertyName("fallback_to_long_code")] public bool? FallbackToLongCode { get; init; }
    /// <summary>Whether sender selection prefers a number matching the recipient area code.</summary>
    [JsonPropertyName("area_code_geomatch")] public bool? AreaCodeGeomatch { get; init; }
    /// <summary>Whether message validation is performed synchronously before accepting the send.</summary>
    [JsonPropertyName("synchronous_validation")] public bool? SynchronousValidation { get; init; }
    /// <summary>Seconds the message may stay queued before failing with 30001 (Twilio's `Queue overflow`). Per-message value wins over the Messaging Service's `validity_period`; default 36000. Counts from SendAt for scheduled sends.</summary>
    [JsonPropertyName("validity_period")] public int? ValidityPeriod { get; init; }
    /// <summary>Absolute URL of this resource.</summary>
    [JsonPropertyName("url")] public string? Url { get; init; }
    /// <summary>Declared messaging use case for this Messaging Service.</summary>
    [JsonPropertyName("usecase")] public string? Usecase { get; init; }
    /// <summary>Whether inbound routing uses the phone number webhook rather than the service webhook.</summary>
    [JsonPropertyName("use_inbound_webhook_on_number")] public bool? UseInboundWebhookOnNumber { get; init; }

    /// <summary>Whether inbound webhooks carry OptOutType for START, STOP and HELP keywords.</summary>
    [JsonPropertyName("advanced_opt_out")] public bool AdvancedOptOut { get; init; }
}

/// <summary>List envelope for <c>GET /v1/Services</c> on the messaging host.</summary>
public sealed record MessagingServiceList : IV1PagedList
{
    /// <summary>The returned services in this page.</summary>
    [JsonPropertyName("services")] public List<MessagingService> Services { get; init; } = new();
    /// <summary>Pagination metadata; follow next_page_url to fetch the next page.</summary>
    [JsonPropertyName("meta")] public VoiceV1Meta? Meta { get; init; }
}

/// <summary>Body for <c>POST /v1/Services</c> (messaging host). <see cref="FriendlyName"/> required.</summary>
public sealed record CreateMessagingServiceRequest : IFormSerializable
{
    /// <summary>Human-readable name used to identify this resource.</summary>
    public required string FriendlyName { get; init; }
    /// <summary>URL used for inbound request.</summary>
    public string? InboundRequestUrl { get; init; }
    /// <summary>HTTP method for inbound requests.</summary>
    public string? InboundMethod { get; init; }
    /// <summary>URL used for fallback.</summary>
    public string? FallbackUrl { get; init; }
    /// <summary>HTTP method for fallback requests.</summary>
    public string? FallbackMethod { get; init; }
    /// <summary>Callback URL for message delivery status notifications.</summary>
    public string? StatusCallback { get; init; }
    /// <summary>Whether the sender is consistently selected for a recipient across messages.</summary>
    public bool? StickySender { get; init; }
    /// <summary>Whether media messages may be converted for recipients without MMS capability.</summary>
    public bool? MmsConverter { get; init; }
    /// <summary>Whether text encoding substitutions are enabled for message delivery.</summary>
    public bool? SmartEncoding { get; init; }
    /// <summary>Configured message-content scanning policy.</summary>
    public string? ScanMessageContent { get; init; }
    /// <summary>Whether a long-code sender may be selected when the preferred sender is unavailable.</summary>
    public bool? FallbackToLongCode { get; init; }
    /// <summary>Whether sender selection prefers a number matching the recipient area code.</summary>
    public bool? AreaCodeGeomatch { get; init; }
    /// <summary>Whether message validation is performed synchronously before accepting the send.</summary>
    public bool? SynchronousValidation { get; init; }
    /// <summary>Seconds the message may stay queued before failing with 30001 (Twilio's `Queue overflow`). Per-message value wins over the Messaging Service's `validity_period`; default 36000. Counts from SendAt for scheduled sends.</summary>
    public int? ValidityPeriod { get; init; }
    /// <summary>Declared messaging use case for this Messaging Service.</summary>
    public string? Usecase { get; init; }
    /// <summary>Whether inbound routing uses the phone number webhook rather than the service webhook.</summary>
    public bool? UseInboundWebhookOnNumber { get; init; }

    /// <summary>Render the declared request fields as form pairs; lists use repeated keys and null values are omitted by the transport.</summary>
    public IEnumerable<KeyValuePair<string, string?>> ToForm()
    {
        yield return new("FriendlyName", FriendlyName);
        yield return new("InboundRequestUrl", InboundRequestUrl);
        yield return new("InboundMethod", InboundMethod);
        yield return new("FallbackUrl", FallbackUrl);
        yield return new("FallbackMethod", FallbackMethod);
        yield return new("StatusCallback", StatusCallback);
        yield return new("StickySender", FormHelpers.BoolStr(StickySender));
        yield return new("MmsConverter", FormHelpers.BoolStr(MmsConverter));
        yield return new("SmartEncoding", FormHelpers.BoolStr(SmartEncoding));
        yield return new("ScanMessageContent", ScanMessageContent);
        yield return new("FallbackToLongCode", FormHelpers.BoolStr(FallbackToLongCode));
        yield return new("AreaCodeGeomatch", FormHelpers.BoolStr(AreaCodeGeomatch));
        yield return new("SynchronousValidation", FormHelpers.BoolStr(SynchronousValidation));
        yield return new("ValidityPeriod", ValidityPeriod?.ToString());
        yield return new("Usecase", Usecase);
        yield return new("UseInboundWebhookOnNumber", FormHelpers.BoolStr(UseInboundWebhookOnNumber));
    }
}

/// <summary>Body for <c>POST /v1/Services/{sid}</c> (messaging host). All fields optional.</summary>
public sealed record UpdateMessagingServiceRequest : IFormSerializable
{
    /// <summary>Human-readable name used to identify this resource.</summary>
    public string? FriendlyName { get; init; }
    /// <summary>URL used for inbound request.</summary>
    public string? InboundRequestUrl { get; init; }
    /// <summary>HTTP method for inbound requests.</summary>
    public string? InboundMethod { get; init; }
    /// <summary>URL used for fallback.</summary>
    public string? FallbackUrl { get; init; }
    /// <summary>HTTP method for fallback requests.</summary>
    public string? FallbackMethod { get; init; }
    /// <summary>Callback URL for message delivery status notifications.</summary>
    public string? StatusCallback { get; init; }
    /// <summary>Whether the sender is consistently selected for a recipient across messages.</summary>
    public bool? StickySender { get; init; }
    /// <summary>Whether media messages may be converted for recipients without MMS capability.</summary>
    public bool? MmsConverter { get; init; }
    /// <summary>Whether text encoding substitutions are enabled for message delivery.</summary>
    public bool? SmartEncoding { get; init; }
    /// <summary>Configured message-content scanning policy.</summary>
    public string? ScanMessageContent { get; init; }
    /// <summary>Whether a long-code sender may be selected when the preferred sender is unavailable.</summary>
    public bool? FallbackToLongCode { get; init; }
    /// <summary>Whether sender selection prefers a number matching the recipient area code.</summary>
    public bool? AreaCodeGeomatch { get; init; }
    /// <summary>Whether message validation is performed synchronously before accepting the send.</summary>
    public bool? SynchronousValidation { get; init; }
    /// <summary>Seconds the message may stay queued before failing with 30001 (Twilio's `Queue overflow`). Per-message value wins over the Messaging Service's `validity_period`; default 36000. Counts from SendAt for scheduled sends.</summary>
    public int? ValidityPeriod { get; init; }
    /// <summary>Declared messaging use case for this Messaging Service.</summary>
    public string? Usecase { get; init; }
    /// <summary>Whether inbound routing uses the phone number webhook rather than the service webhook.</summary>
    public bool? UseInboundWebhookOnNumber { get; init; }

    /// <summary>Render the declared request fields as form pairs; lists use repeated keys and null values are omitted by the transport.</summary>
    public IEnumerable<KeyValuePair<string, string?>> ToForm()
    {
        yield return new("FriendlyName", FriendlyName);
        yield return new("InboundRequestUrl", InboundRequestUrl);
        yield return new("InboundMethod", InboundMethod);
        yield return new("FallbackUrl", FallbackUrl);
        yield return new("FallbackMethod", FallbackMethod);
        yield return new("StatusCallback", StatusCallback);
        yield return new("StickySender", FormHelpers.BoolStr(StickySender));
        yield return new("MmsConverter", FormHelpers.BoolStr(MmsConverter));
        yield return new("SmartEncoding", FormHelpers.BoolStr(SmartEncoding));
        yield return new("ScanMessageContent", ScanMessageContent);
        yield return new("FallbackToLongCode", FormHelpers.BoolStr(FallbackToLongCode));
        yield return new("AreaCodeGeomatch", FormHelpers.BoolStr(AreaCodeGeomatch));
        yield return new("SynchronousValidation", FormHelpers.BoolStr(SynchronousValidation));
        yield return new("ValidityPeriod", ValidityPeriod?.ToString());
        yield return new("Usecase", Usecase);
        yield return new("UseInboundWebhookOnNumber", FormHelpers.BoolStr(UseInboundWebhookOnNumber));
    }
}

/// <summary>A phone number in a Messaging Service's sender pool — the referenced
/// IncomingPhoneNumber (<c>PN</c> sid) as pooled (<c>/v1/Services/{sid}/PhoneNumbers</c>).</summary>
public sealed record MessagingServicePhoneNumber
{
    /// <summary>The referenced IncomingPhoneNumber's PN sid.</summary>
    [JsonPropertyName("sid")] public string Sid { get; init; } = "";

    /// <summary>Owning Account SID.</summary>
    [JsonPropertyName("account_sid")] public string? AccountSid { get; init; }

    /// <summary>The Messaging Service (<c>MG…</c>) this number is pooled under.</summary>
    [JsonPropertyName("service_sid")] public string? ServiceSid { get; init; }

    /// <summary>The E.164 phone number.</summary>
    [JsonPropertyName("phone_number")] public string? PhoneNumber { get; init; }

    /// <summary>ISO 3166-1 alpha-2 country code of the number.</summary>
    [JsonPropertyName("country_code")] public string? CountryCode { get; init; }

    /// <summary>Per-channel capabilities of the referenced number (raw).</summary>
    [JsonPropertyName("capabilities")] public System.Text.Json.JsonElement? Capabilities { get; init; }

    /// <summary>RFC 3339 creation timestamp.</summary>
    [JsonPropertyName("date_created")] public string? DateCreated { get; init; }

    /// <summary>RFC 3339 last-modification timestamp.</summary>
    [JsonPropertyName("date_updated")] public string? DateUpdated { get; init; }

    /// <summary>URI of this pool entry.</summary>
    [JsonPropertyName("url")] public string? Url { get; init; }
}

/// <summary>List response for <c>GET /v1/Services/{sid}/PhoneNumbers</c>.</summary>
public sealed record MessagingServicePhoneNumberList : IV1PagedList
{
    /// <summary>The page of pool entries.</summary>
    [JsonPropertyName("phone_numbers")] public List<MessagingServicePhoneNumber> PhoneNumbers { get; init; } = new();

    /// <summary>The <c>/v1</c> pagination envelope.</summary>
    [JsonPropertyName("meta")] public VoiceV1Meta? Meta { get; init; }
}

/// <summary>Body for <c>POST /v1/Services/{sid}/PhoneNumbers</c> — add an account-owned
/// IncomingPhoneNumber (by PN sid) to the service's sender pool. A number belongs to at most
/// one service: re-adding to the same service is 409/21710; adding under another service is
/// 409/21712.</summary>
public sealed record AddMessagingServicePhoneNumberRequest : IFormSerializable
{
    /// <summary>The IncomingPhoneNumber's PN sid.</summary>
    public required string PhoneNumberSid { get; init; }

    /// <inheritdoc/>
    public IEnumerable<KeyValuePair<string, string?>> ToForm()
    {
        yield return new("PhoneNumberSid", PhoneNumberSid);
    }
}
