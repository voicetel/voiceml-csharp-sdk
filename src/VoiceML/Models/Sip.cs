using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace VoiceML.Models;

// ===========================================================================
// Response shapes
// ===========================================================================

/// <summary>A SIP ingress domain — Twilio-compatible <c>SD…</c> resource. Bind a CredentialList
/// and/or IpAccessControlList via the mapping sub-resources to authenticate inbound SIP traffic.</summary>
public sealed record SipDomain
{
    /// <summary>Unique SID identifying this resource.</summary>
    [JsonPropertyName("sid")] public string? Sid { get; init; }
    /// <summary>SID of the account that owns this resource.</summary>
    [JsonPropertyName("account_sid")] public string? AccountSid { get; init; }
    /// <summary>Fully-qualified SIP domain. Required on create.</summary>
    [JsonPropertyName("domain_name")] public string? DomainName { get; init; }
    /// <summary>API version used for this resource (2010-04-01 on the account-scoped API).</summary>
    [JsonPropertyName("api_version")] public string? ApiVersion { get; init; }
    /// <summary>Human-readable name used to identify this resource.</summary>
    [JsonPropertyName("friendly_name")] public string? FriendlyName { get; init; }
    /// <summary>Always empty in this implementation. Reserved for Twilio compatibility; no code path populates it (attaching CredentialList/IpAccessControlList mappings does not set it).</summary>
    [JsonPropertyName("auth_type")] public string? AuthType { get; init; }
    /// <summary>URL from which to fetch TwiML for voice call handling.</summary>
    [JsonPropertyName("voice_url")] public string? VoiceUrl { get; init; }
    /// <summary>HTTP method used to fetch the voice TwiML URL (GET or POST).</summary>
    [JsonPropertyName("voice_method")] public string? VoiceMethod { get; init; }
    /// <summary>Fallback TwiML URL used when the primary voice handler fails.</summary>
    [JsonPropertyName("voice_fallback_url")] public string? VoiceFallbackUrl { get; init; }
    /// <summary>HTTP method used to fetch the fallback voice URL.</summary>
    [JsonPropertyName("voice_fallback_method")] public string? VoiceFallbackMethod { get; init; }
    /// <summary>URL used for voice status callback.</summary>
    [JsonPropertyName("voice_status_callback_url")] public string? VoiceStatusCallbackUrl { get; init; }
    /// <summary>HTTP method for voice status callback requests.</summary>
    [JsonPropertyName("voice_status_callback_method")] public string? VoiceStatusCallbackMethod { get; init; }
    /// <summary>Whether sip registration is enabled for this resource.</summary>
    [JsonPropertyName("sip_registration")] public bool? SipRegistration { get; init; }
    /// <summary>Whether emergency calling enabled is enabled for this resource.</summary>
    [JsonPropertyName("emergency_calling_enabled")] public bool? EmergencyCallingEnabled { get; init; }
    /// <summary>Whether secure is enabled for this resource.</summary>
    [JsonPropertyName("secure")] public bool? Secure { get; init; }
    /// <summary>SID of the related Byoc Trunk.</summary>
    [JsonPropertyName("byoc_trunk_sid")] public string? ByocTrunkSid { get; init; }
    /// <summary>SID of the related Emergency Caller.</summary>
    [JsonPropertyName("emergency_caller_sid")] public string? EmergencyCallerSid { get; init; }
    /// <summary>Timestamp when this resource was created.</summary>
    [JsonPropertyName("date_created")] public string? DateCreated { get; init; }
    /// <summary>Timestamp of the most recent modification to this resource.</summary>
    [JsonPropertyName("date_updated")] public string? DateUpdated { get; init; }
    /// <summary>Resource URI relative to the API host.</summary>
    [JsonPropertyName("uri")] public string? Uri { get; init; }
    /// <summary>Map of related subresource names to their resource URIs.</summary>
    [JsonPropertyName("subresource_uris")] public Dictionary<string, string>? SubresourceUris { get; init; }
}

/// <summary>Page of sip domain resources with continuation metadata.</summary>
public sealed record SipDomainList : Page
{
    /// <summary>The returned domains in this page.</summary>
    [JsonPropertyName("domains")] public List<SipDomain> Domains { get; init; } = new();
}

/// <summary>A named bag of SIP-digest credentials — <c>CL…</c>.</summary>
public sealed record SipCredentialList
{
    /// <summary>Unique SID identifying this resource.</summary>
    [JsonPropertyName("sid")] public string? Sid { get; init; }
    /// <summary>SID of the account that owns this resource.</summary>
    [JsonPropertyName("account_sid")] public string? AccountSid { get; init; }
    /// <summary>Human-readable label (Twilio free-text semantics). VoiceML extension: when this value is a valid phone number, it is also used as the outbound caller ID presented by every device registered under this credential list; a non-numeric label sets no caller ID.</summary>
    [JsonPropertyName("friendly_name")] public string? FriendlyName { get; init; }
    /// <summary>Timestamp when this resource was created.</summary>
    [JsonPropertyName("date_created")] public string? DateCreated { get; init; }
    /// <summary>Timestamp of the most recent modification to this resource.</summary>
    [JsonPropertyName("date_updated")] public string? DateUpdated { get; init; }
    /// <summary>Resource URI relative to the API host.</summary>
    [JsonPropertyName("uri")] public string? Uri { get; init; }
    /// <summary>Map of related subresource names to their resource URIs.</summary>
    [JsonPropertyName("subresource_uris")] public Dictionary<string, string>? SubresourceUris { get; init; }
}

/// <summary>Page of sip credential list resources with continuation metadata.</summary>
public sealed record SipCredentialListList : Page
{
    /// <summary>The returned credential lists in this page.</summary>
    [JsonPropertyName("credential_lists")] public List<SipCredentialList> CredentialLists { get; init; } = new();
}

/// <summary>A single SIP-digest username + (write-only) password — <c>CR…</c>. Password is
/// never round-tripped on response — use Update with a new password to rotate.</summary>
public sealed record SipCredential
{
    /// <summary>Unique SID identifying this resource.</summary>
    [JsonPropertyName("sid")] public string? Sid { get; init; }
    /// <summary>SID of the account that owns this resource.</summary>
    [JsonPropertyName("account_sid")] public string? AccountSid { get; init; }
    /// <summary>SID of the related Credential List.</summary>
    [JsonPropertyName("credential_list_sid")] public string? CredentialListSid { get; init; }
    /// <summary>Username used for SIP digest authentication.</summary>
    [JsonPropertyName("username")] public string? Username { get; init; }
    /// <summary>Timestamp when this resource was created.</summary>
    [JsonPropertyName("date_created")] public string? DateCreated { get; init; }
    /// <summary>Timestamp of the most recent modification to this resource.</summary>
    [JsonPropertyName("date_updated")] public string? DateUpdated { get; init; }
    /// <summary>Resource URI relative to the API host.</summary>
    [JsonPropertyName("uri")] public string? Uri { get; init; }
}

/// <summary>Page of credentials within a CredentialList. Spec name <c>SipCredentialListPage</c>
/// mirrors Twilio — it's a *page of credentials*, not of credential-lists.</summary>
public sealed record SipCredentialListPage : Page
{
    /// <summary>The returned credentials in this page.</summary>
    [JsonPropertyName("credentials")] public List<SipCredential> Credentials { get; init; } = new();
}

/// <summary>A named bag of CIDR-bound IPs — <c>AL…</c>.</summary>
public sealed record SipIpAccessControlList
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
    /// <summary>Resource URI relative to the API host.</summary>
    [JsonPropertyName("uri")] public string? Uri { get; init; }
    /// <summary>Map of related subresource names to their resource URIs.</summary>
    [JsonPropertyName("subresource_uris")] public Dictionary<string, string>? SubresourceUris { get; init; }
}

/// <summary>Page of sip ip access control list resources with continuation metadata.</summary>
public sealed record SipIpAccessControlListList : Page
{
    /// <summary>The returned ip access control lists in this page.</summary>
    [JsonPropertyName("ip_access_control_lists")] public List<SipIpAccessControlList> IpAccessControlLists { get; init; } = new();
}

/// <summary>A single CIDR-bound entry in an IpAccessControlList — <c>IP…</c>.</summary>
public sealed record SipIpAddress
{
    /// <summary>Unique SID identifying this resource.</summary>
    [JsonPropertyName("sid")] public string? Sid { get; init; }
    /// <summary>SID of the account that owns this resource.</summary>
    [JsonPropertyName("account_sid")] public string? AccountSid { get; init; }
    /// <summary>SID of the related Ip Access Control List.</summary>
    [JsonPropertyName("ip_access_control_list_sid")] public string? IpAccessControlListSid { get; init; }
    /// <summary>Human-readable name used to identify this resource.</summary>
    [JsonPropertyName("friendly_name")] public string? FriendlyName { get; init; }
    /// <summary>IP address authorized by this resource.</summary>
    [JsonPropertyName("ip_address")] public string? IpAddress { get; init; }
    /// <summary>CIDR prefix length. Defaults to 32 (single host) when omitted on create.</summary>
    [JsonPropertyName("cidr_prefix_length")] public int CidrPrefixLength { get; init; }
    /// <summary>Timestamp when this resource was created.</summary>
    [JsonPropertyName("date_created")] public string? DateCreated { get; init; }
    /// <summary>Timestamp of the most recent modification to this resource.</summary>
    [JsonPropertyName("date_updated")] public string? DateUpdated { get; init; }
    /// <summary>Resource URI relative to the API host.</summary>
    [JsonPropertyName("uri")] public string? Uri { get; init; }
}

/// <summary>Page of sip ip address resources with continuation metadata.</summary>
public sealed record SipIpAddressList : Page
{
    /// <summary>The returned ip addresses in this page.</summary>
    [JsonPropertyName("ip_addresses")] public List<SipIpAddress> IpAddresses { get; init; } = new();
}

/// <summary>Round-trip shape for every domain mapping sub-resource (Calls / Registrations ×
/// CredentialList / IpAccessControlList). Sid echoes the bound resource sid (<c>CL…</c> for
/// credential mappings, <c>AL…</c> for IP-ACL mappings); DomainSid records the domain.</summary>
public sealed record SipDomainMapping
{
    /// <summary>Unique SID identifying this resource.</summary>
    [JsonPropertyName("sid")] public string? Sid { get; init; }
    /// <summary>SID of the account that owns this resource.</summary>
    [JsonPropertyName("account_sid")] public string? AccountSid { get; init; }
    /// <summary>Human-readable name used to identify this resource.</summary>
    [JsonPropertyName("friendly_name")] public string? FriendlyName { get; init; }
    /// <summary>The SipDomain resource this mapping is attached to.</summary>
    [JsonPropertyName("domain_sid")] public string? DomainSid { get; init; }
    /// <summary>Timestamp when this resource was created.</summary>
    [JsonPropertyName("date_created")] public string? DateCreated { get; init; }
    /// <summary>Timestamp of the most recent modification to this resource.</summary>
    [JsonPropertyName("date_updated")] public string? DateUpdated { get; init; }
    /// <summary>Resource URI relative to the API host.</summary>
    [JsonPropertyName("uri")] public string? Uri { get; init; }
}

/// <summary>Page of sip credential list mapping resources with continuation metadata.</summary>
public sealed record SipCredentialListMappingList : Page
{
    /// <summary>The returned credential list mappings in this page.</summary>
    [JsonPropertyName("credential_list_mappings")] public List<SipDomainMapping> CredentialListMappings { get; init; } = new();
}

/// <summary>Page of sip ip access control list mapping resources with continuation metadata.</summary>
public sealed record SipIpAccessControlListMappingList : Page
{
    /// <summary>The returned ip access control list mappings in this page.</summary>
    [JsonPropertyName("ip_access_control_list_mappings")] public List<SipDomainMapping> IpAccessControlListMappings { get; init; } = new();
}

/// <summary>List envelope used by every <c>/SIP/Domains/{Sid}/Auth/{Calls|Registrations}/{Credential|IpAccessControl}ListMappings</c>
/// endpoint. The Auth sub-namespaces share a single Twilio-side shape: the items array
/// key is <c>contents</c>, regardless of which mapping kind the URL selects. The
/// per-item shape is the same <see cref="SipDomainMapping"/> as the legacy non-Auth
/// mapping endpoints.</summary>
public sealed record SipAuthMappingList : Page
{
    /// <summary>The page of mapping resources. Wire name: <c>contents</c>.</summary>
    [JsonPropertyName("contents")] public List<SipDomainMapping> Contents { get; init; } = new();
}

// ===========================================================================
// Request bodies (form-encoded)
// ===========================================================================

/// <summary>Parameters for create sip domain request.</summary>
public sealed record CreateSipDomainRequest : IFormSerializable
{
    /// <summary>DNS name of the SIP Domain used to route incoming SIP requests.</summary>
    public required string DomainName { get; init; }
    /// <summary>Human-readable name used to identify this resource.</summary>
    public string? FriendlyName { get; init; }
    /// <summary>URL from which to fetch TwiML for voice call handling.</summary>
    public string? VoiceUrl { get; init; }
    /// <summary>HTTP method used to fetch the voice TwiML URL (GET or POST).</summary>
    public string? VoiceMethod { get; init; }
    /// <summary>Fallback TwiML URL used when the primary voice handler fails.</summary>
    public string? VoiceFallbackUrl { get; init; }
    /// <summary>HTTP method used to fetch the fallback voice URL.</summary>
    public string? VoiceFallbackMethod { get; init; }
    /// <summary>URL used for voice status callback.</summary>
    public string? VoiceStatusCallbackUrl { get; init; }
    /// <summary>HTTP method for voice status callback requests.</summary>
    public string? VoiceStatusCallbackMethod { get; init; }
    /// <summary>Whether sip registration is enabled for this operation.</summary>
    public bool? SipRegistration { get; init; }
    /// <summary>Whether secure is enabled for this operation.</summary>
    public bool? Secure { get; init; }
    /// <summary>Whether emergency calling enabled is enabled for this operation.</summary>
    public bool? EmergencyCallingEnabled { get; init; }
    /// <summary>SID of the related Byoc Trunk.</summary>
    public string? ByocTrunkSid { get; init; }
    /// <summary>SID of the related Emergency Caller.</summary>
    public string? EmergencyCallerSid { get; init; }

    /// <summary>Render the declared request fields as form pairs; lists use repeated keys and null values are omitted by the transport.</summary>
    public IEnumerable<KeyValuePair<string, string?>> ToForm()
    {
        yield return new("DomainName", DomainName);
        yield return new("FriendlyName", FriendlyName);
        yield return new("VoiceUrl", VoiceUrl);
        yield return new("VoiceMethod", VoiceMethod);
        yield return new("VoiceFallbackUrl", VoiceFallbackUrl);
        yield return new("VoiceFallbackMethod", VoiceFallbackMethod);
        yield return new("VoiceStatusCallbackUrl", VoiceStatusCallbackUrl);
        yield return new("VoiceStatusCallbackMethod", VoiceStatusCallbackMethod);
        yield return new("SipRegistration", SipRegistration?.ToString().ToLowerInvariant());
        yield return new("Secure", Secure?.ToString().ToLowerInvariant());
        yield return new("EmergencyCallingEnabled", EmergencyCallingEnabled?.ToString().ToLowerInvariant());
        yield return new("ByocTrunkSid", ByocTrunkSid);
        yield return new("EmergencyCallerSid", EmergencyCallerSid);
    }
}

/// <summary>Parameters for update sip domain request.</summary>
public sealed record UpdateSipDomainRequest : IFormSerializable
{
    /// <summary>Human-readable name used to identify this resource.</summary>
    public string? FriendlyName { get; init; }
    /// <summary>URL from which to fetch TwiML for voice call handling.</summary>
    public string? VoiceUrl { get; init; }
    /// <summary>HTTP method used to fetch the voice TwiML URL (GET or POST).</summary>
    public string? VoiceMethod { get; init; }
    /// <summary>Fallback TwiML URL used when the primary voice handler fails.</summary>
    public string? VoiceFallbackUrl { get; init; }
    /// <summary>HTTP method used to fetch the fallback voice URL.</summary>
    public string? VoiceFallbackMethod { get; init; }
    /// <summary>URL used for voice status callback.</summary>
    public string? VoiceStatusCallbackUrl { get; init; }
    /// <summary>HTTP method for voice status callback requests.</summary>
    public string? VoiceStatusCallbackMethod { get; init; }
    /// <summary>Whether sip registration is enabled for this operation.</summary>
    public bool? SipRegistration { get; init; }
    /// <summary>Whether secure is enabled for this operation.</summary>
    public bool? Secure { get; init; }
    /// <summary>Whether emergency calling enabled is enabled for this operation.</summary>
    public bool? EmergencyCallingEnabled { get; init; }
    /// <summary>SID of the related Byoc Trunk.</summary>
    public string? ByocTrunkSid { get; init; }
    /// <summary>SID of the related Emergency Caller.</summary>
    public string? EmergencyCallerSid { get; init; }

    /// <summary>Render the declared request fields as form pairs; lists use repeated keys and null values are omitted by the transport.</summary>
    public IEnumerable<KeyValuePair<string, string?>> ToForm()
    {
        yield return new("FriendlyName", FriendlyName);
        yield return new("VoiceUrl", VoiceUrl);
        yield return new("VoiceMethod", VoiceMethod);
        yield return new("VoiceFallbackUrl", VoiceFallbackUrl);
        yield return new("VoiceFallbackMethod", VoiceFallbackMethod);
        yield return new("VoiceStatusCallbackUrl", VoiceStatusCallbackUrl);
        yield return new("VoiceStatusCallbackMethod", VoiceStatusCallbackMethod);
        yield return new("SipRegistration", SipRegistration?.ToString().ToLowerInvariant());
        yield return new("Secure", Secure?.ToString().ToLowerInvariant());
        yield return new("EmergencyCallingEnabled", EmergencyCallingEnabled?.ToString().ToLowerInvariant());
        yield return new("ByocTrunkSid", ByocTrunkSid);
        yield return new("EmergencyCallerSid", EmergencyCallerSid);
    }
}

/// <summary>Parameters for create sip credential list request.</summary>
public sealed record CreateSipCredentialListRequest : IFormSerializable
{
    /// <summary>Human-readable name used to identify this resource.</summary>
    public required string FriendlyName { get; init; }
    /// <summary>Render the declared request fields as form pairs; lists use repeated keys and null values are omitted by the transport.</summary>
    public IEnumerable<KeyValuePair<string, string?>> ToForm()
    {
        yield return new("FriendlyName", FriendlyName);
    }
}

/// <summary>Parameters for update sip credential list request.</summary>
public sealed record UpdateSipCredentialListRequest : IFormSerializable
{
    /// <summary>Human-readable name used to identify this resource.</summary>
    public string? FriendlyName { get; init; }
    /// <summary>Render the declared request fields as form pairs; lists use repeated keys and null values are omitted by the transport.</summary>
    public IEnumerable<KeyValuePair<string, string?>> ToForm()
    {
        yield return new("FriendlyName", FriendlyName);
    }
}

/// <summary>Parameters for create sip credential request.</summary>
public sealed record CreateSipCredentialRequest : IFormSerializable
{
    /// <summary>Username used for SIP digest authentication.</summary>
    public required string Username { get; init; }
    /// <summary>Write-only password used for SIP digest authentication.</summary>
    public required string Password { get; init; }
    /// <summary>Render the declared request fields as form pairs; lists use repeated keys and null values are omitted by the transport.</summary>
    public IEnumerable<KeyValuePair<string, string?>> ToForm()
    {
        yield return new("Username", Username);
        yield return new("Password", Password);
    }
}

/// <summary>Only the password is mutable; username is pinned at creation time.</summary>
public sealed record UpdateSipCredentialRequest : IFormSerializable
{
    /// <summary>Write-only password used for SIP digest authentication.</summary>
    public required string Password { get; init; }
    /// <summary>Render the declared request fields as form pairs; lists use repeated keys and null values are omitted by the transport.</summary>
    public IEnumerable<KeyValuePair<string, string?>> ToForm()
    {
        yield return new("Password", Password);
    }
}

/// <summary>Parameters for create sip ip access control list request.</summary>
public sealed record CreateSipIpAccessControlListRequest : IFormSerializable
{
    /// <summary>Human-readable name used to identify this resource.</summary>
    public required string FriendlyName { get; init; }
    /// <summary>Render the declared request fields as form pairs; lists use repeated keys and null values are omitted by the transport.</summary>
    public IEnumerable<KeyValuePair<string, string?>> ToForm()
    {
        yield return new("FriendlyName", FriendlyName);
    }
}

/// <summary>Parameters for update sip ip access control list request.</summary>
public sealed record UpdateSipIpAccessControlListRequest : IFormSerializable
{
    /// <summary>Human-readable name used to identify this resource.</summary>
    public string? FriendlyName { get; init; }
    /// <summary>Render the declared request fields as form pairs; lists use repeated keys and null values are omitted by the transport.</summary>
    public IEnumerable<KeyValuePair<string, string?>> ToForm()
    {
        yield return new("FriendlyName", FriendlyName);
    }
}

/// <summary>Parameters for create sip ip address request.</summary>
public sealed record CreateSipIpAddressRequest : IFormSerializable
{
    /// <summary>Human-readable name used to identify this resource.</summary>
    public required string FriendlyName { get; init; }
    /// <summary>IP address authorized by this resource.</summary>
    public required string IpAddress { get; init; }
    /// <summary>Defaults server-side to 32 (single host) when omitted.</summary>
    public int? CidrPrefixLength { get; init; }
    /// <summary>Render the declared request fields as form pairs; lists use repeated keys and null values are omitted by the transport.</summary>
    public IEnumerable<KeyValuePair<string, string?>> ToForm()
    {
        yield return new("FriendlyName", FriendlyName);
        yield return new("IpAddress", IpAddress);
        yield return new("CidrPrefixLength", CidrPrefixLength?.ToString());
    }
}

/// <summary>Parameters for update sip ip address request.</summary>
public sealed record UpdateSipIpAddressRequest : IFormSerializable
{
    /// <summary>Human-readable name used to identify this resource.</summary>
    public string? FriendlyName { get; init; }
    /// <summary>IP address authorized by this resource.</summary>
    public string? IpAddress { get; init; }
    /// <summary>CIDR prefix length applied to the IP address.</summary>
    public int? CidrPrefixLength { get; init; }
    /// <summary>Render the declared request fields as form pairs; lists use repeated keys and null values are omitted by the transport.</summary>
    public IEnumerable<KeyValuePair<string, string?>> ToForm()
    {
        yield return new("FriendlyName", FriendlyName);
        yield return new("IpAddress", IpAddress);
        yield return new("CidrPrefixLength", CidrPrefixLength?.ToString());
    }
}

/// <summary>Body for any <c>…/CredentialListMappings</c> POST (historical / Auth/Calls / Auth/Registrations).</summary>
public sealed record CreateSipCredentialListMappingRequest : IFormSerializable
{
    /// <summary>SID of the related Credential List.</summary>
    public required string CredentialListSid { get; init; }
    /// <summary>Render the declared request fields as form pairs; lists use repeated keys and null values are omitted by the transport.</summary>
    public IEnumerable<KeyValuePair<string, string?>> ToForm()
    {
        yield return new("CredentialListSid", CredentialListSid);
    }
}

/// <summary>Body for any <c>…/IpAccessControlListMappings</c> POST (historical + Auth/Calls).</summary>
public sealed record CreateSipIpAccessControlListMappingRequest : IFormSerializable
{
    /// <summary>SID of the related Ip Access Control List.</summary>
    public required string IpAccessControlListSid { get; init; }
    /// <summary>Render the declared request fields as form pairs; lists use repeated keys and null values are omitted by the transport.</summary>
    public IEnumerable<KeyValuePair<string, string?>> ToForm()
    {
        yield return new("IpAccessControlListSid", IpAccessControlListSid);
    }
}

// ===========================================================================
// Routes V2 — /v2/SipDomains/{SipDomain} (Inbound Processing Region)
// ===========================================================================

/// <summary>SIP-domain Inbound Processing Region binding. Twilio's routes/v2
/// surface. SID is <c>QQ…</c>. Keyed by the registrable SIP domain name (not
/// the <c>SD…</c> SipDomain SID).</summary>
public sealed record RoutesV2SipDomain
{
    /// <summary>Unique SID identifying this resource.</summary>
    [JsonPropertyName("sid")] public string? Sid { get; init; }
    /// <summary>SIP domain name to which the inbound processing region applies.</summary>
    [JsonPropertyName("sip_domain")] public string? SipDomain { get; init; }
    /// <summary>SID of the account that owns this resource.</summary>
    [JsonPropertyName("account_sid")] public string? AccountSid { get; init; }
    /// <summary>Human-readable name used to identify this resource.</summary>
    [JsonPropertyName("friendly_name")] public string? FriendlyName { get; init; }
    /// <summary>Inbound Processing Region that handles inbound calls for this SIP domain.</summary>
    [JsonPropertyName("voice_region")] public string? VoiceRegion { get; init; }
    /// <summary>Absolute URL of this resource.</summary>
    [JsonPropertyName("url")] public string? Url { get; init; }
    /// <summary>Timestamp when this resource was created.</summary>
    [JsonPropertyName("date_created")] public string? DateCreated { get; init; }
    /// <summary>Timestamp of the most recent modification to this resource.</summary>
    [JsonPropertyName("date_updated")] public string? DateUpdated { get; init; }
}

/// <summary>Body for <c>POST /v2/SipDomains/{SipDomain}</c>. All fields optional.</summary>
public sealed record UpdateRoutesV2SipDomainRequest : IFormSerializable
{
    /// <summary>Inbound Processing Region for voice.</summary>
    public string? VoiceRegion { get; init; }
    /// <summary>Human-readable name used to identify this resource.</summary>
    public string? FriendlyName { get; init; }

    /// <summary>Render the declared request fields as form pairs; lists use repeated keys and null values are omitted by the transport.</summary>
    public IEnumerable<KeyValuePair<string, string?>> ToForm()
    {
        yield return new("VoiceRegion", VoiceRegion);
        yield return new("FriendlyName", FriendlyName);
    }
}

// ===========================================================================
// Routes V2 — /v2/PhoneNumbers/{PhoneNumber} (Inbound Processing Region)
// ===========================================================================

/// <summary>Phone-number Inbound Processing Region binding. Twilio's routes/v2
/// surface. SID is <c>QQ…</c>. Keyed by E.164 phone number (or its <c>PN</c> sid).</summary>
public sealed record RoutesV2PhoneNumber
{
    /// <summary>Phone number in E.164 format, including the leading plus sign.</summary>
    [JsonPropertyName("phone_number")] public string? PhoneNumber { get; init; }
    /// <summary>Absolute URL of this resource.</summary>
    [JsonPropertyName("url")] public string? Url { get; init; }
    /// <summary>Unique SID identifying this resource.</summary>
    [JsonPropertyName("sid")] public string? Sid { get; init; }
    /// <summary>SID of the account that owns this resource.</summary>
    [JsonPropertyName("account_sid")] public string? AccountSid { get; init; }
    /// <summary>Human-readable name used to identify this resource.</summary>
    [JsonPropertyName("friendly_name")] public string? FriendlyName { get; init; }
    /// <summary>Inbound Processing Region that handles inbound calls for this phone number.</summary>
    [JsonPropertyName("voice_region")] public string? VoiceRegion { get; init; }
    /// <summary>Timestamp when this resource was created.</summary>
    [JsonPropertyName("date_created")] public string? DateCreated { get; init; }
    /// <summary>Timestamp of the most recent modification to this resource.</summary>
    [JsonPropertyName("date_updated")] public string? DateUpdated { get; init; }
}

/// <summary>Body for <c>POST /v2/PhoneNumbers/{PhoneNumber}</c>. All fields optional.</summary>
public sealed record UpdateRoutesV2PhoneNumberRequest : IFormSerializable
{
    /// <summary>Inbound Processing Region for voice.</summary>
    public string? VoiceRegion { get; init; }
    /// <summary>Human-readable name used to identify this resource.</summary>
    public string? FriendlyName { get; init; }

    /// <summary>Render the declared request fields as form pairs; lists use repeated keys and null values are omitted by the transport.</summary>
    public IEnumerable<KeyValuePair<string, string?>> ToForm()
    {
        yield return new("VoiceRegion", VoiceRegion);
        yield return new("FriendlyName", FriendlyName);
    }
}

/// <summary>Query params for any <c>/SIP</c> list endpoint.</summary>
/// <summary>Query-string params for SIP Domains, CredentialLists, IpAccessControlLists
/// and nested IpAddresses lists, including their FriendlyName filter.</summary>
public sealed record ListSipPageParams
{
    /// <summary>Filter on the friendly name.</summary>
    public string? FriendlyName { get; init; }

    /// <summary>Zero-based page index.</summary>
    public int? Page { get; init; }

    /// <summary>Page size.</summary>
    public int? PageSize { get; init; }

    /// <summary>Opaque cursor for the next page, provided by the API.</summary>
    public string? PageToken { get; init; }

    /// <summary>Render the declared filters as query pairs; null values are omitted by the transport.</summary>
    public IEnumerable<KeyValuePair<string, string?>> ToQuery()
    {
        yield return new("FriendlyName", FriendlyName);
        yield return new("Page", Page?.ToString());
        yield return new("PageSize", PageSize?.ToString());
        yield return new("PageToken", PageToken);
    }
}

/// <summary>Query-string params for domain CredentialList/IpAccessControlList mapping
/// lists and nested credential lists — paging only, no FriendlyName filter.</summary>
public sealed record ListSipMappingPageParams
{
    /// <summary>Zero-based page index.</summary>
    public int? Page { get; init; }

    /// <summary>Page size.</summary>
    public int? PageSize { get; init; }

    /// <summary>Opaque cursor for the next page, provided by the API.</summary>
    public string? PageToken { get; init; }

    /// <summary>Render the declared filters as query pairs; null values are omitted by the transport.</summary>
    public IEnumerable<KeyValuePair<string, string?>> ToQuery()
    {
        yield return new("Page", Page?.ToString());
        yield return new("PageSize", PageSize?.ToString());
        yield return new("PageToken", PageToken);
    }
}
