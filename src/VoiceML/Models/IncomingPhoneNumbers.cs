using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace VoiceML.Models;

/// <summary>Per-channel capability flags on an <see cref="IncomingPhoneNumber"/>. VoiceML
/// is voice-only: <c>Voice = true</c>, the rest <c>false</c>.</summary>
public sealed record IncomingPhoneNumberCapabilities
{
    /// <summary>Whether voice is supported on this number.</summary>
    [JsonPropertyName("voice")] public bool Voice { get; init; }

    /// <summary>Whether SMS is supported. VoiceML always emits <c>false</c>.</summary>
    [JsonPropertyName("sms")] public bool Sms { get; init; }

    /// <summary>Whether MMS is supported. VoiceML always emits <c>false</c>.</summary>
    [JsonPropertyName("mms")] public bool Mms { get; init; }

    /// <summary>Whether fax is supported. VoiceML always emits <c>false</c>.</summary>
    [JsonPropertyName("fax")] public bool Fax { get; init; }
}

/// <summary>A Twilio-shape IncomingPhoneNumber resource — a DID assigned to the tenant.
/// <para>The canonical identifier is <see cref="Sid"/> (<c>PN</c> + 32 hex). <see cref="PhoneNumber"/>
/// carries the E.164 form. Twilio-compat fields VoiceML doesn't track (SMS, regulatory,
/// emergency, trunking) emit safe defaults so strict-binding deserializers don't throw.</para></summary>
public sealed record IncomingPhoneNumber
{
    /// <summary>Twilio-format Phone Number SID (<c>PN</c> + 32 hex).</summary>
    [JsonPropertyName("sid")] public string Sid { get; init; } = "";

    /// <summary>Owning Account SID.</summary>
    [JsonPropertyName("account_sid")] public string AccountSid { get; init; } = "";

    /// <summary>E.164 phone number (leading <c>+</c>, 7-15 digits).</summary>
    [JsonPropertyName("phone_number")] public string PhoneNumber { get; init; } = "";

    /// <summary>Display name. Empty by default in v0.5.x.</summary>
    [JsonPropertyName("friendly_name")] public string? FriendlyName { get; init; }

    /// <summary>Twilio API version label (always <c>2010-04-01</c> for this surface).</summary>
    [JsonPropertyName("api_version")] public string ApiVersion { get; init; } = "";

    /// <summary>Resource URI under <c>/2010-04-01/Accounts/{AccountSid}/IncomingPhoneNumbers/{Sid}.json</c>.</summary>
    [JsonPropertyName("uri")] public string Uri { get; init; } = "";

    /// <summary>Twilio-compat origin: <c>twilio</c>, <c>hosted</c>, or empty.
    /// VoiceML emits empty (operator-managed provisioning model).</summary>
    [JsonPropertyName("origin")] public string? Origin { get; init; }

    /// <summary>Twilio-compat beta-inventory flag. VoiceML always emits <c>false</c>.</summary>
    [JsonPropertyName("beta")] public bool? Beta { get; init; }

    /// <summary>Per-channel capability flags.</summary>
    [JsonPropertyName("capabilities")] public IncomingPhoneNumberCapabilities? Capabilities { get; init; }

    /// <summary>Primary voice-URL — fetched on inbound calls.</summary>
    [JsonPropertyName("voice_url")] public string? VoiceUrl { get; init; }

    /// <summary>HTTP method for the voice URL (<c>GET</c> or <c>POST</c>).</summary>
    [JsonPropertyName("voice_method")] public string? VoiceMethod { get; init; }

    /// <summary>Voice fallback URL — fetched when the primary URL errors.</summary>
    [JsonPropertyName("voice_fallback_url")] public string? VoiceFallbackUrl { get; init; }

    /// <summary>HTTP method for the voice fallback URL.</summary>
    [JsonPropertyName("voice_fallback_method")] public string? VoiceFallbackMethod { get; init; }

    /// <summary>Twilio-compat linked Application SID. VoiceML emits empty.</summary>
    [JsonPropertyName("voice_application_sid")] public string? VoiceApplicationSid { get; init; }

    /// <summary>Twilio-compat CNAM lookup flag. VoiceML always emits <c>false</c>.</summary>
    [JsonPropertyName("voice_caller_id_lookup")] public bool? VoiceCallerIdLookup { get; init; }

    /// <summary>Twilio-compat: VoiceML routes inbound as <c>voice</c> only.</summary>
    [JsonPropertyName("voice_receive_mode")] public string? VoiceReceiveMode { get; init; }

    /// <summary>Twilio-compat: SMS handler URL. Empty (VoiceML is voice-only).</summary>
    [JsonPropertyName("sms_url")] public string? SmsUrl { get; init; }

    /// <summary>Twilio-compat: empty (SMS not handled).</summary>
    [JsonPropertyName("sms_method")] public string? SmsMethod { get; init; }

    /// <summary>Twilio-compat: empty (SMS not handled).</summary>
    [JsonPropertyName("sms_fallback_url")] public string? SmsFallbackUrl { get; init; }

    /// <summary>Twilio-compat: empty (SMS not handled).</summary>
    [JsonPropertyName("sms_fallback_method")] public string? SmsFallbackMethod { get; init; }

    /// <summary>Twilio-compat: empty (SMS not handled).</summary>
    [JsonPropertyName("sms_application_sid")] public string? SmsApplicationSid { get; init; }

    /// <summary>Twilio-compat lifecycle webhook. VoiceML emits empty.</summary>
    [JsonPropertyName("status_callback")] public string? StatusCallback { get; init; }

    /// <summary>Twilio-compat: empty.</summary>
    [JsonPropertyName("status_callback_method")] public string? StatusCallbackMethod { get; init; }

    /// <summary>Twilio-compat SIP trunk SID. VoiceML emits empty.</summary>
    [JsonPropertyName("trunk_sid")] public string? TrunkSid { get; init; }

    /// <summary>Twilio-compat regulatory address SID. Not tracked.</summary>
    [JsonPropertyName("address_sid")] public string? AddressSid { get; init; }

    /// <summary>Twilio-compat address requirements: VoiceML emits <c>none</c>.</summary>
    [JsonPropertyName("address_requirements")] public string? AddressRequirements { get; init; }

    /// <summary>Twilio-compat business-identity SID. Not tracked.</summary>
    [JsonPropertyName("identity_sid")] public string? IdentitySid { get; init; }

    /// <summary>Twilio-compat regulatory bundle SID. Not tracked.</summary>
    [JsonPropertyName("bundle_sid")] public string? BundleSid { get; init; }

    /// <summary>Twilio-compat E911 registration. Not handled.</summary>
    [JsonPropertyName("emergency_status")] public string? EmergencyStatus { get; init; }

    /// <summary>Twilio-compat E911 address SID. Not tracked.</summary>
    [JsonPropertyName("emergency_address_sid")] public string? EmergencyAddressSid { get; init; }

    /// <summary>Twilio-compat E911 registration state. Not tracked.</summary>
    [JsonPropertyName("emergency_address_status")] public string? EmergencyAddressStatus { get; init; }

    /// <summary>Twilio-compat provisioning status. Empty (not modelled).</summary>
    [JsonPropertyName("status")] public string? Status { get; init; }

    /// <summary>RFC 3339 creation timestamp.</summary>
    [JsonPropertyName("date_created")] public string? DateCreated { get; init; }

    /// <summary>RFC 3339 last-modification timestamp.</summary>
    [JsonPropertyName("date_updated")] public string? DateUpdated { get; init; }
}

/// <summary>List response for <c>GET /IncomingPhoneNumbers.json</c>.</summary>
public sealed record IncomingPhoneNumberList : Page
{
    /// <summary>The page of <see cref="IncomingPhoneNumber"/> resources.</summary>
    [JsonPropertyName("incoming_phone_numbers")]
    public List<IncomingPhoneNumber> IncomingPhoneNumbers { get; init; } = new();
}

/// <summary>Query-string params for <c>GET /IncomingPhoneNumbers.json</c>.</summary>
public sealed record ListIncomingPhoneNumbersOptions
{
    /// <summary>Exact-match filter on the E.164 phone number. Returns a 0-or-1-row envelope.</summary>
    public string? PhoneNumber { get; init; }

    /// <summary>Zero-based page index.</summary>
    public int? Page { get; init; }

    /// <summary>Page size.</summary>
    public int? PageSize { get; init; }

    /// <summary>Render as a query-parameter sequence.</summary>
    public IEnumerable<KeyValuePair<string, string?>> ToQuery()
    {
        yield return new("PhoneNumber", PhoneNumber);
        yield return new("Page", Page?.ToString());
        yield return new("PageSize", PageSize?.ToString());
    }
}

/// <summary>Body for <c>POST /IncomingPhoneNumbers.json</c>. <see cref="PhoneNumber"/> is
/// required; voice routing fields are optional. Re-POSTing the same number from the same
/// tenant rebinds voice routing (idempotent).</summary>
public sealed record CreateIncomingPhoneNumberOptions : IFormSerializable
{
    /// <summary>E.164 phone number to claim/rebind.</summary>
    public required string PhoneNumber { get; init; }

    /// <summary>Primary voice URL.</summary>
    public string? VoiceUrl { get; init; }

    /// <summary>HTTP method for the voice URL.</summary>
    public string? VoiceMethod { get; init; }

    /// <summary>Voice fallback URL.</summary>
    public string? VoiceFallbackUrl { get; init; }

    /// <summary>HTTP method for the voice fallback URL.</summary>
    public string? VoiceFallbackMethod { get; init; }

    /// <inheritdoc/>
    public IEnumerable<KeyValuePair<string, string?>> ToForm()
    {
        yield return new("PhoneNumber", PhoneNumber);
        yield return new("VoiceUrl", VoiceUrl);
        yield return new("VoiceMethod", VoiceMethod);
        yield return new("VoiceFallbackUrl", VoiceFallbackUrl);
        yield return new("VoiceFallbackMethod", VoiceFallbackMethod);
    }
}

/// <summary>Body for <c>POST /IncomingPhoneNumbers/{Sid}.json</c>. Only-set-fields-touched.</summary>
public sealed record UpdateIncomingPhoneNumberOptions : IFormSerializable
{
    /// <summary>Primary voice URL.</summary>
    public string? VoiceUrl { get; init; }

    /// <summary>HTTP method for the voice URL.</summary>
    public string? VoiceMethod { get; init; }

    /// <summary>Voice fallback URL.</summary>
    public string? VoiceFallbackUrl { get; init; }

    /// <summary>HTTP method for the voice fallback URL.</summary>
    public string? VoiceFallbackMethod { get; init; }

    /// <inheritdoc/>
    public IEnumerable<KeyValuePair<string, string?>> ToForm()
    {
        yield return new("VoiceUrl", VoiceUrl);
        yield return new("VoiceMethod", VoiceMethod);
        yield return new("VoiceFallbackUrl", VoiceFallbackUrl);
        yield return new("VoiceFallbackMethod", VoiceFallbackMethod);
    }
}
