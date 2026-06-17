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
    [JsonPropertyName("sid")] public string Sid { get; init; } = "";
    [JsonPropertyName("account_sid")] public string AccountSid { get; init; } = "";
    [JsonPropertyName("domain_name")] public string DomainName { get; init; } = "";
    [JsonPropertyName("api_version")] public string ApiVersion { get; init; } = "";
    [JsonPropertyName("friendly_name")] public string? FriendlyName { get; init; }
    [JsonPropertyName("auth_type")] public string? AuthType { get; init; }
    [JsonPropertyName("voice_url")] public string? VoiceUrl { get; init; }
    [JsonPropertyName("voice_method")] public string? VoiceMethod { get; init; }
    [JsonPropertyName("voice_fallback_url")] public string? VoiceFallbackUrl { get; init; }
    [JsonPropertyName("voice_fallback_method")] public string? VoiceFallbackMethod { get; init; }
    [JsonPropertyName("voice_status_callback_url")] public string? VoiceStatusCallbackUrl { get; init; }
    [JsonPropertyName("voice_status_callback_method")] public string? VoiceStatusCallbackMethod { get; init; }
    [JsonPropertyName("sip_registration")] public bool? SipRegistration { get; init; }
    [JsonPropertyName("emergency_calling_enabled")] public bool? EmergencyCallingEnabled { get; init; }
    [JsonPropertyName("secure")] public bool? Secure { get; init; }
    [JsonPropertyName("byoc_trunk_sid")] public string? ByocTrunkSid { get; init; }
    [JsonPropertyName("emergency_caller_sid")] public string? EmergencyCallerSid { get; init; }
    [JsonPropertyName("date_created")] public string DateCreated { get; init; } = "";
    [JsonPropertyName("date_updated")] public string DateUpdated { get; init; } = "";
    [JsonPropertyName("uri")] public string Uri { get; init; } = "";
    [JsonPropertyName("subresource_uris")] public Dictionary<string, string>? SubresourceUris { get; init; }
}

public sealed record SipDomainList : Page
{
    [JsonPropertyName("domains")] public List<SipDomain> Domains { get; init; } = new();
}

/// <summary>A named bag of SIP-digest credentials — <c>CL…</c>.</summary>
public sealed record SipCredentialList
{
    [JsonPropertyName("sid")] public string Sid { get; init; } = "";
    [JsonPropertyName("account_sid")] public string AccountSid { get; init; } = "";
    [JsonPropertyName("friendly_name")] public string? FriendlyName { get; init; }
    [JsonPropertyName("date_created")] public string DateCreated { get; init; } = "";
    [JsonPropertyName("date_updated")] public string DateUpdated { get; init; } = "";
    [JsonPropertyName("uri")] public string Uri { get; init; } = "";
    [JsonPropertyName("subresource_uris")] public Dictionary<string, string>? SubresourceUris { get; init; }
}

public sealed record SipCredentialListList : Page
{
    [JsonPropertyName("credential_lists")] public List<SipCredentialList> CredentialLists { get; init; } = new();
}

/// <summary>A single SIP-digest username + (write-only) password — <c>CR…</c>. Password is
/// never round-tripped on response — use Update with a new password to rotate.</summary>
public sealed record SipCredential
{
    [JsonPropertyName("sid")] public string Sid { get; init; } = "";
    [JsonPropertyName("account_sid")] public string AccountSid { get; init; } = "";
    [JsonPropertyName("credential_list_sid")] public string CredentialListSid { get; init; } = "";
    [JsonPropertyName("username")] public string Username { get; init; } = "";
    [JsonPropertyName("date_created")] public string DateCreated { get; init; } = "";
    [JsonPropertyName("date_updated")] public string DateUpdated { get; init; } = "";
    [JsonPropertyName("uri")] public string Uri { get; init; } = "";
}

/// <summary>Page of credentials within a CredentialList. Spec name <c>SipCredentialListPage</c>
/// mirrors Twilio — it's a *page of credentials*, not of credential-lists.</summary>
public sealed record SipCredentialListPage : Page
{
    [JsonPropertyName("credentials")] public List<SipCredential> Credentials { get; init; } = new();
}

/// <summary>A named bag of CIDR-bound IPs — <c>AL…</c>.</summary>
public sealed record SipIpAccessControlList
{
    [JsonPropertyName("sid")] public string Sid { get; init; } = "";
    [JsonPropertyName("account_sid")] public string AccountSid { get; init; } = "";
    [JsonPropertyName("friendly_name")] public string? FriendlyName { get; init; }
    [JsonPropertyName("date_created")] public string DateCreated { get; init; } = "";
    [JsonPropertyName("date_updated")] public string DateUpdated { get; init; } = "";
    [JsonPropertyName("uri")] public string Uri { get; init; } = "";
    [JsonPropertyName("subresource_uris")] public Dictionary<string, string>? SubresourceUris { get; init; }
}

public sealed record SipIpAccessControlListList : Page
{
    [JsonPropertyName("ip_access_control_lists")] public List<SipIpAccessControlList> IpAccessControlLists { get; init; } = new();
}

/// <summary>A single CIDR-bound entry in an IpAccessControlList — <c>IP…</c>.</summary>
public sealed record SipIpAddress
{
    [JsonPropertyName("sid")] public string Sid { get; init; } = "";
    [JsonPropertyName("account_sid")] public string AccountSid { get; init; } = "";
    [JsonPropertyName("ip_access_control_list_sid")] public string IpAccessControlListSid { get; init; } = "";
    [JsonPropertyName("friendly_name")] public string FriendlyName { get; init; } = "";
    [JsonPropertyName("ip_address")] public string IpAddress { get; init; } = "";
    [JsonPropertyName("cidr_prefix_length")] public int CidrPrefixLength { get; init; }
    [JsonPropertyName("date_created")] public string DateCreated { get; init; } = "";
    [JsonPropertyName("date_updated")] public string DateUpdated { get; init; } = "";
    [JsonPropertyName("uri")] public string Uri { get; init; } = "";
}

public sealed record SipIpAddressList : Page
{
    [JsonPropertyName("ip_addresses")] public List<SipIpAddress> IpAddresses { get; init; } = new();
}

/// <summary>Round-trip shape for every domain mapping sub-resource (Calls / Registrations ×
/// CredentialList / IpAccessControlList). Sid echoes the bound resource sid (<c>CL…</c> for
/// credential mappings, <c>AL…</c> for IP-ACL mappings); DomainSid records the domain.</summary>
public sealed record SipDomainMapping
{
    [JsonPropertyName("sid")] public string Sid { get; init; } = "";
    [JsonPropertyName("account_sid")] public string AccountSid { get; init; } = "";
    [JsonPropertyName("friendly_name")] public string? FriendlyName { get; init; }
    [JsonPropertyName("domain_sid")] public string? DomainSid { get; init; }
    [JsonPropertyName("date_created")] public string DateCreated { get; init; } = "";
    [JsonPropertyName("date_updated")] public string DateUpdated { get; init; } = "";
    [JsonPropertyName("uri")] public string Uri { get; init; } = "";
}

public sealed record SipCredentialListMappingList : Page
{
    [JsonPropertyName("credential_list_mappings")] public List<SipDomainMapping> CredentialListMappings { get; init; } = new();
}

public sealed record SipIpAccessControlListMappingList : Page
{
    [JsonPropertyName("ip_access_control_list_mappings")] public List<SipDomainMapping> IpAccessControlListMappings { get; init; } = new();
}

// ===========================================================================
// Request bodies (form-encoded)
// ===========================================================================

public sealed record CreateSipDomainRequest : IFormSerializable
{
    public required string DomainName { get; init; }
    public string? FriendlyName { get; init; }
    public string? VoiceUrl { get; init; }
    public string? VoiceMethod { get; init; }
    public string? VoiceFallbackUrl { get; init; }
    public string? VoiceFallbackMethod { get; init; }
    public string? VoiceStatusCallbackUrl { get; init; }
    public string? VoiceStatusCallbackMethod { get; init; }
    public bool? SipRegistration { get; init; }
    public bool? Secure { get; init; }
    public bool? EmergencyCallingEnabled { get; init; }
    public string? ByocTrunkSid { get; init; }
    public string? EmergencyCallerSid { get; init; }

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

public sealed record UpdateSipDomainRequest : IFormSerializable
{
    public string? FriendlyName { get; init; }
    public string? VoiceUrl { get; init; }
    public string? VoiceMethod { get; init; }
    public string? VoiceFallbackUrl { get; init; }
    public string? VoiceFallbackMethod { get; init; }
    public string? VoiceStatusCallbackUrl { get; init; }
    public string? VoiceStatusCallbackMethod { get; init; }
    public bool? SipRegistration { get; init; }
    public bool? Secure { get; init; }
    public bool? EmergencyCallingEnabled { get; init; }
    public string? ByocTrunkSid { get; init; }
    public string? EmergencyCallerSid { get; init; }

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

public sealed record CreateSipCredentialListRequest : IFormSerializable
{
    public required string FriendlyName { get; init; }
    public IEnumerable<KeyValuePair<string, string?>> ToForm()
    {
        yield return new("FriendlyName", FriendlyName);
    }
}

public sealed record UpdateSipCredentialListRequest : IFormSerializable
{
    public string? FriendlyName { get; init; }
    public IEnumerable<KeyValuePair<string, string?>> ToForm()
    {
        yield return new("FriendlyName", FriendlyName);
    }
}

public sealed record CreateSipCredentialRequest : IFormSerializable
{
    public required string Username { get; init; }
    public required string Password { get; init; }
    public IEnumerable<KeyValuePair<string, string?>> ToForm()
    {
        yield return new("Username", Username);
        yield return new("Password", Password);
    }
}

/// <summary>Only the password is mutable; username is pinned at creation time.</summary>
public sealed record UpdateSipCredentialRequest : IFormSerializable
{
    public required string Password { get; init; }
    public IEnumerable<KeyValuePair<string, string?>> ToForm()
    {
        yield return new("Password", Password);
    }
}

public sealed record CreateSipIpAccessControlListRequest : IFormSerializable
{
    public required string FriendlyName { get; init; }
    public IEnumerable<KeyValuePair<string, string?>> ToForm()
    {
        yield return new("FriendlyName", FriendlyName);
    }
}

public sealed record UpdateSipIpAccessControlListRequest : IFormSerializable
{
    public string? FriendlyName { get; init; }
    public IEnumerable<KeyValuePair<string, string?>> ToForm()
    {
        yield return new("FriendlyName", FriendlyName);
    }
}

public sealed record CreateSipIpAddressRequest : IFormSerializable
{
    public required string FriendlyName { get; init; }
    public required string IpAddress { get; init; }
    /// <summary>Defaults server-side to 32 (single host) when omitted.</summary>
    public int? CidrPrefixLength { get; init; }
    public IEnumerable<KeyValuePair<string, string?>> ToForm()
    {
        yield return new("FriendlyName", FriendlyName);
        yield return new("IpAddress", IpAddress);
        yield return new("CidrPrefixLength", CidrPrefixLength?.ToString());
    }
}

public sealed record UpdateSipIpAddressRequest : IFormSerializable
{
    public string? FriendlyName { get; init; }
    public string? IpAddress { get; init; }
    public int? CidrPrefixLength { get; init; }
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
    public required string CredentialListSid { get; init; }
    public IEnumerable<KeyValuePair<string, string?>> ToForm()
    {
        yield return new("CredentialListSid", CredentialListSid);
    }
}

/// <summary>Body for any <c>…/IpAccessControlListMappings</c> POST (historical + Auth/Calls).</summary>
public sealed record CreateSipIpAccessControlListMappingRequest : IFormSerializable
{
    public required string IpAccessControlListSid { get; init; }
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
    [JsonPropertyName("sid")] public string Sid { get; init; } = "";
    [JsonPropertyName("sip_domain")] public string SipDomain { get; init; } = "";
    [JsonPropertyName("account_sid")] public string AccountSid { get; init; } = "";
    [JsonPropertyName("friendly_name")] public string? FriendlyName { get; init; }
    [JsonPropertyName("voice_region")] public string? VoiceRegion { get; init; }
    [JsonPropertyName("url")] public string? Url { get; init; }
    [JsonPropertyName("date_created")] public string DateCreated { get; init; } = "";
    [JsonPropertyName("date_updated")] public string DateUpdated { get; init; } = "";
}

/// <summary>Body for <c>POST /v2/SipDomains/{SipDomain}</c>. All fields optional.</summary>
public sealed record UpdateRoutesV2SipDomainRequest : IFormSerializable
{
    public string? VoiceRegion { get; init; }
    public string? FriendlyName { get; init; }

    public IEnumerable<KeyValuePair<string, string?>> ToForm()
    {
        yield return new("VoiceRegion", VoiceRegion);
        yield return new("FriendlyName", FriendlyName);
    }
}

/// <summary>Query params for any <c>/SIP</c> list endpoint.</summary>
public sealed record ListSipPageParams
{
    public int? Page { get; init; }
    public int? PageSize { get; init; }
    public string? PageToken { get; init; }

    public IEnumerable<KeyValuePair<string, string?>> ToQuery()
    {
        yield return new("Page", Page?.ToString());
        yield return new("PageSize", PageSize?.ToString());
        yield return new("PageToken", PageToken);
    }
}
