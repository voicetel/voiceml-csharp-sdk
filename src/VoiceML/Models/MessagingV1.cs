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
    [JsonPropertyName("sid")] public string? Sid { get; init; }
    [JsonPropertyName("account_sid")] public string? AccountSid { get; init; }
    [JsonPropertyName("friendly_name")] public string? FriendlyName { get; init; }
    [JsonPropertyName("date_created")] public string? DateCreated { get; init; }
    [JsonPropertyName("date_updated")] public string? DateUpdated { get; init; }
    [JsonPropertyName("inbound_request_url")] public string? InboundRequestUrl { get; init; }
    [JsonPropertyName("inbound_method")] public string? InboundMethod { get; init; }
    [JsonPropertyName("fallback_url")] public string? FallbackUrl { get; init; }
    [JsonPropertyName("fallback_method")] public string? FallbackMethod { get; init; }
    [JsonPropertyName("status_callback")] public string? StatusCallback { get; init; }
    [JsonPropertyName("sticky_sender")] public bool? StickySender { get; init; }
    [JsonPropertyName("mms_converter")] public bool? MmsConverter { get; init; }
    [JsonPropertyName("smart_encoding")] public bool? SmartEncoding { get; init; }
    [JsonPropertyName("scan_message_content")] public string? ScanMessageContent { get; init; }
    [JsonPropertyName("fallback_to_long_code")] public bool? FallbackToLongCode { get; init; }
    [JsonPropertyName("area_code_geomatch")] public bool? AreaCodeGeomatch { get; init; }
    [JsonPropertyName("synchronous_validation")] public bool? SynchronousValidation { get; init; }
    [JsonPropertyName("validity_period")] public int? ValidityPeriod { get; init; }
    [JsonPropertyName("url")] public string? Url { get; init; }
    [JsonPropertyName("usecase")] public string? Usecase { get; init; }
    [JsonPropertyName("use_inbound_webhook_on_number")] public bool? UseInboundWebhookOnNumber { get; init; }
}

/// <summary>List envelope for <c>GET /v1/Services</c> on the messaging host.</summary>
public sealed record MessagingServiceList
{
    [JsonPropertyName("services")] public List<MessagingService> Services { get; init; } = new();
    [JsonPropertyName("meta")] public VoiceV1Meta? Meta { get; init; }
}

/// <summary>Body for <c>POST /v1/Services</c> (messaging host). <see cref="FriendlyName"/> required.</summary>
public sealed record CreateMessagingServiceRequest : IFormSerializable
{
    public required string FriendlyName { get; init; }
    public string? InboundRequestUrl { get; init; }
    public string? InboundMethod { get; init; }
    public string? FallbackUrl { get; init; }
    public string? FallbackMethod { get; init; }
    public string? StatusCallback { get; init; }
    public bool? StickySender { get; init; }
    public bool? MmsConverter { get; init; }
    public bool? SmartEncoding { get; init; }
    public string? ScanMessageContent { get; init; }
    public bool? FallbackToLongCode { get; init; }
    public bool? AreaCodeGeomatch { get; init; }
    public bool? SynchronousValidation { get; init; }
    public int? ValidityPeriod { get; init; }
    public string? Usecase { get; init; }
    public bool? UseInboundWebhookOnNumber { get; init; }

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
    public string? FriendlyName { get; init; }
    public string? InboundRequestUrl { get; init; }
    public string? InboundMethod { get; init; }
    public string? FallbackUrl { get; init; }
    public string? FallbackMethod { get; init; }
    public string? StatusCallback { get; init; }
    public bool? StickySender { get; init; }
    public bool? MmsConverter { get; init; }
    public bool? SmartEncoding { get; init; }
    public string? ScanMessageContent { get; init; }
    public bool? FallbackToLongCode { get; init; }
    public bool? AreaCodeGeomatch { get; init; }
    public bool? SynchronousValidation { get; init; }
    public int? ValidityPeriod { get; init; }
    public string? Usecase { get; init; }
    public bool? UseInboundWebhookOnNumber { get; init; }

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
