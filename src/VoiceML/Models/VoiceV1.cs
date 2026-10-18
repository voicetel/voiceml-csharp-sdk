using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace VoiceML.Models;

// ===========================================================================
// Voice v1 (voice.twilio.com/v1)
// Twilio-compatible /v1 surface. Account resolves from HTTP Basic auth,
// dates are ISO-8601, list responses carry the VoiceV1Meta envelope.
// ===========================================================================

/// <summary>Twilio-style page envelope used by every <c>/v1/</c> list response
/// (Voice v1 + Conversations v1). Distinct from the 2010-04-01 <see cref="Page"/>
/// shape — different field names (<c>first_page_url</c> vs <c>first_page_uri</c>)
/// and a top-level <c>meta</c> wrapper.</summary>
public sealed record VoiceV1Meta
{
    /// <summary>Absolute URL of the first page of this collection.</summary>
    [JsonPropertyName("first_page_url")] public string? FirstPageUrl { get; init; }
    /// <summary>Absolute URL of the next page, or null when this is the final page.</summary>
    [JsonPropertyName("next_page_url")] public string? NextPageUrl { get; init; }
    /// <summary>Absolute URL of the preceding page, or null on the first page.</summary>
    [JsonPropertyName("previous_page_url")] public string? PreviousPageUrl { get; init; }
    /// <summary>Absolute URL of this resource.</summary>
    [JsonPropertyName("url")] public string? Url { get; init; }
    /// <summary>Zero-based index of the returned page.</summary>
    [JsonPropertyName("page")] public int? Page { get; init; }
    /// <summary>Requested number of items per page.</summary>
    [JsonPropertyName("page_size")] public int? PageSize { get; init; }
    /// <summary>JSON property name containing the collection items.</summary>
    [JsonPropertyName("key")] public string? Key { get; init; }
}

// ---- Response shapes -------------------------------------------------------

/// <summary>An allowed source IP for Voice v1 — <c>IL…</c>. Maps to a <see cref="VoiceV1SourceIpMapping"/>
/// for inbound call routing.</summary>
public sealed record VoiceV1IpRecord
{
    /// <summary>SID of the account that owns this resource.</summary>
    [JsonPropertyName("account_sid")] public string? AccountSid { get; init; }
    /// <summary>Unique SID identifying this resource.</summary>
    [JsonPropertyName("sid")] public string? Sid { get; init; }
    /// <summary>Human-readable name used to identify this resource.</summary>
    [JsonPropertyName("friendly_name")] public string? FriendlyName { get; init; }
    /// <summary>IP address authorized by this resource.</summary>
    [JsonPropertyName("ip_address")] public string? IpAddress { get; init; }
    /// <summary>CIDR prefix length applied to the IP address.</summary>
    [JsonPropertyName("cidr_prefix_length")] public int CidrPrefixLength { get; init; }
    /// <summary>Timestamp when this resource was created.</summary>
    [JsonPropertyName("date_created")] public string? DateCreated { get; init; }
    /// <summary>Timestamp of the most recent modification to this resource.</summary>
    [JsonPropertyName("date_updated")] public string? DateUpdated { get; init; }
    /// <summary>Absolute URL of this resource.</summary>
    [JsonPropertyName("url")] public string? Url { get; init; }
}

/// <summary>Page of ip record resources with continuation metadata.</summary>
public sealed record VoiceV1IpRecordList : IV1PagedList
{
    /// <summary>The returned ip records in this page.</summary>
    [JsonPropertyName("ip_records")] public List<VoiceV1IpRecord> IpRecords { get; init; } = new();
    /// <summary>Pagination metadata; follow next_page_url to fetch the next page.</summary>
    [JsonPropertyName("meta")] public VoiceV1Meta? Meta { get; init; }
}

/// <summary>Source-IP-to-SIP-Domain binding — <c>IB…</c>. Routes inbound calls from an
/// <see cref="VoiceV1IpRecord"/> source to a SIP Domain.</summary>
public sealed record VoiceV1SourceIpMapping
{
    /// <summary>Unique SID identifying this resource.</summary>
    [JsonPropertyName("sid")] public string? Sid { get; init; }
    /// <summary>SID of the related Ip Record.</summary>
    [JsonPropertyName("ip_record_sid")] public string? IpRecordSid { get; init; }
    /// <summary>SID of the related Sip Domain.</summary>
    [JsonPropertyName("sip_domain_sid")] public string? SipDomainSid { get; init; }
    /// <summary>Timestamp when this resource was created.</summary>
    [JsonPropertyName("date_created")] public string? DateCreated { get; init; }
    /// <summary>Timestamp of the most recent modification to this resource.</summary>
    [JsonPropertyName("date_updated")] public string? DateUpdated { get; init; }
    /// <summary>Absolute URL of this resource.</summary>
    [JsonPropertyName("url")] public string? Url { get; init; }
}

/// <summary>Page of source ip mapping resources with continuation metadata.</summary>
public sealed record VoiceV1SourceIpMappingList : IV1PagedList
{
    /// <summary>The returned source ip mappings in this page.</summary>
    [JsonPropertyName("source_ip_mappings")] public List<VoiceV1SourceIpMapping> SourceIpMappings { get; init; } = new();
    /// <summary>Pagination metadata; follow next_page_url to fetch the next page.</summary>
    [JsonPropertyName("meta")] public VoiceV1Meta? Meta { get; init; }
}

/// <summary>A bring-your-own-carrier trunk — <c>BY…</c>.</summary>
public sealed record VoiceV1ByocTrunk
{
    /// <summary>SID of the account that owns this resource.</summary>
    [JsonPropertyName("account_sid")] public string? AccountSid { get; init; }
    /// <summary>Unique SID identifying this resource.</summary>
    [JsonPropertyName("sid")] public string? Sid { get; init; }
    /// <summary>Human-readable name used to identify this resource.</summary>
    [JsonPropertyName("friendly_name")] public string? FriendlyName { get; init; }
    /// <summary>URL from which to fetch TwiML for voice call handling.</summary>
    [JsonPropertyName("voice_url")] public string? VoiceUrl { get; init; }
    /// <summary>HTTP method used to fetch the voice TwiML URL (GET or POST).</summary>
    [JsonPropertyName("voice_method")] public string? VoiceMethod { get; init; }
    /// <summary>Fallback TwiML URL used when the primary voice handler fails.</summary>
    [JsonPropertyName("voice_fallback_url")] public string? VoiceFallbackUrl { get; init; }
    /// <summary>HTTP method used to fetch the fallback voice URL.</summary>
    [JsonPropertyName("voice_fallback_method")] public string? VoiceFallbackMethod { get; init; }
    /// <summary>URL used for status callback.</summary>
    [JsonPropertyName("status_callback_url")] public string? StatusCallbackUrl { get; init; }
    /// <summary>HTTP method for status callback requests.</summary>
    [JsonPropertyName("status_callback_method")] public string? StatusCallbackMethod { get; init; }
    /// <summary>Whether cnam lookup enabled is enabled for this resource.</summary>
    [JsonPropertyName("cnam_lookup_enabled")] public bool? CnamLookupEnabled { get; init; }
    /// <summary>SID of the related Connection Policy.</summary>
    [JsonPropertyName("connection_policy_sid")] public string? ConnectionPolicySid { get; init; }
    /// <summary>SID of the related From Domain.</summary>
    [JsonPropertyName("from_domain_sid")] public string? FromDomainSid { get; init; }
    /// <summary>Timestamp when this resource was created.</summary>
    [JsonPropertyName("date_created")] public string? DateCreated { get; init; }
    /// <summary>Timestamp of the most recent modification to this resource.</summary>
    [JsonPropertyName("date_updated")] public string? DateUpdated { get; init; }
    /// <summary>Absolute URL of this resource.</summary>
    [JsonPropertyName("url")] public string? Url { get; init; }
}

/// <summary>Page of byoc trunk resources with continuation metadata.</summary>
public sealed record VoiceV1ByocTrunkList : IV1PagedList
{
    /// <summary>The returned byoc trunks in this page.</summary>
    [JsonPropertyName("byoc_trunks")] public List<VoiceV1ByocTrunk> ByocTrunks { get; init; } = new();
    /// <summary>Pagination metadata; follow next_page_url to fetch the next page.</summary>
    [JsonPropertyName("meta")] public VoiceV1Meta? Meta { get; init; }
}

/// <summary>An origination policy — <c>NY…</c>. Holds an ordered set of
/// <see cref="VoiceV1ConnectionPolicyTarget"/>s with priority/weight failover.</summary>
public sealed record VoiceV1ConnectionPolicy
{
    /// <summary>SID of the account that owns this resource.</summary>
    [JsonPropertyName("account_sid")] public string? AccountSid { get; init; }
    /// <summary>Unique SID identifying this resource.</summary>
    [JsonPropertyName("sid")] public string? Sid { get; init; }
    /// <summary>Human-readable name used to identify this resource.</summary>
    [JsonPropertyName("friendly_name")] public string? FriendlyName { get; init; }
    /// <summary>Timestamp when this resource was created.</summary>
    [JsonPropertyName("date_created")] public string? DateCreated { get; init; }
    /// <summary>Timestamp of the most recent modification to this resource.</summary>
    [JsonPropertyName("date_updated")] public string? DateUpdated { get; init; }
    /// <summary>Absolute URL of this resource.</summary>
    [JsonPropertyName("url")] public string? Url { get; init; }
    /// <summary>URLs of related resources and child collections.</summary>
    [JsonPropertyName("links")] public Dictionary<string, string>? Links { get; init; }
}

/// <summary>Page of connection policy resources with continuation metadata.</summary>
public sealed record VoiceV1ConnectionPolicyList : IV1PagedList
{
    /// <summary>The returned connection policies in this page.</summary>
    [JsonPropertyName("connection_policies")] public List<VoiceV1ConnectionPolicy> ConnectionPolicies { get; init; } = new();
    /// <summary>Pagination metadata; follow next_page_url to fetch the next page.</summary>
    [JsonPropertyName("meta")] public VoiceV1Meta? Meta { get; init; }
}

/// <summary>A single SIP target inside a <see cref="VoiceV1ConnectionPolicy"/> — <c>NE…</c>.</summary>
public sealed record VoiceV1ConnectionPolicyTarget
{
    /// <summary>SID of the account that owns this resource.</summary>
    [JsonPropertyName("account_sid")] public string? AccountSid { get; init; }
    /// <summary>SID of the related Connection Policy.</summary>
    [JsonPropertyName("connection_policy_sid")] public string? ConnectionPolicySid { get; init; }
    /// <summary>Unique SID identifying this resource.</summary>
    [JsonPropertyName("sid")] public string? Sid { get; init; }
    /// <summary>Human-readable name used to identify this resource.</summary>
    [JsonPropertyName("friendly_name")] public string? FriendlyName { get; init; }
    /// <summary>The SIP URI target.</summary>
    [JsonPropertyName("target")] public string? Target { get; init; }
    /// <summary>Lower = higher priority. Default 10.</summary>
    [JsonPropertyName("priority")] public int Priority { get; init; }
    /// <summary>Load-balancing weight among equal priorities. Default 10.</summary>
    [JsonPropertyName("weight")] public int Weight { get; init; }
    /// <summary>Whether the target is active. Default true.</summary>
    [JsonPropertyName("enabled")] public bool? Enabled { get; init; }
    /// <summary>Timestamp when this resource was created.</summary>
    [JsonPropertyName("date_created")] public string? DateCreated { get; init; }
    /// <summary>Timestamp of the most recent modification to this resource.</summary>
    [JsonPropertyName("date_updated")] public string? DateUpdated { get; init; }
    /// <summary>Absolute URL of this resource.</summary>
    [JsonPropertyName("url")] public string? Url { get; init; }
}

/// <summary>Page of connection policy target resources with continuation metadata.</summary>
public sealed record VoiceV1ConnectionPolicyTargetList : IV1PagedList
{
    /// <summary>The returned targets in this page.</summary>
    [JsonPropertyName("targets")] public List<VoiceV1ConnectionPolicyTarget> Targets { get; init; } = new();
    /// <summary>Pagination metadata; follow next_page_url to fetch the next page.</summary>
    [JsonPropertyName("meta")] public VoiceV1Meta? Meta { get; init; }
}

/// <summary>Account-wide DialingPermissions settings — singleton.</summary>
public sealed record VoiceV1DialingPermissionsSettings
{
    /// <summary>Whether subaccounts inherit the parent's dialing permissions.</summary>
    [JsonPropertyName("dialing_permissions_inheritance")] public bool? DialingPermissionsInheritance { get; init; }
    /// <summary>Absolute URL of this resource.</summary>
    [JsonPropertyName("url")] public string? Url { get; init; }
}

// ---- Request bodies (form-encoded) ----------------------------------------

/// <summary>Parameters for create voice v1 ip record request.</summary>
public sealed record CreateVoiceV1IpRecordRequest : IFormSerializable
{
    /// <summary>IP address authorized by this resource.</summary>
    public required string IpAddress { get; init; }
    /// <summary>Human-readable name used to identify this resource.</summary>
    public string? FriendlyName { get; init; }
    /// <summary>Defaults server-side to 32 when omitted.</summary>
    public int? CidrPrefixLength { get; init; }

    /// <summary>Render the declared request fields as form pairs; lists use repeated keys and null values are omitted by the transport.</summary>
    public IEnumerable<KeyValuePair<string, string?>> ToForm()
    {
        yield return new("IpAddress", IpAddress);
        yield return new("FriendlyName", FriendlyName);
        yield return new("CidrPrefixLength", CidrPrefixLength?.ToString());
    }
}

/// <summary>Parameters for update voice v1 ip record request.</summary>
public sealed record UpdateVoiceV1IpRecordRequest : IFormSerializable
{
    /// <summary>Human-readable name used to identify this resource.</summary>
    public string? FriendlyName { get; init; }

    /// <summary>Render the declared request fields as form pairs; lists use repeated keys and null values are omitted by the transport.</summary>
    public IEnumerable<KeyValuePair<string, string?>> ToForm()
    {
        yield return new("FriendlyName", FriendlyName);
    }
}

/// <summary>Parameters for create voice v1 source ip mapping request.</summary>
public sealed record CreateVoiceV1SourceIpMappingRequest : IFormSerializable
{
    /// <summary>SID of the related Ip Record.</summary>
    public required string IpRecordSid { get; init; }
    /// <summary>SID of the related Sip Domain.</summary>
    public required string SipDomainSid { get; init; }

    /// <summary>Render the declared request fields as form pairs; lists use repeated keys and null values are omitted by the transport.</summary>
    public IEnumerable<KeyValuePair<string, string?>> ToForm()
    {
        yield return new("IpRecordSid", IpRecordSid);
        yield return new("SipDomainSid", SipDomainSid);
    }
}

/// <summary>Parameters for update voice v1 source ip mapping request.</summary>
public sealed record UpdateVoiceV1SourceIpMappingRequest : IFormSerializable
{
    /// <summary>SID of the related Sip Domain.</summary>
    public required string SipDomainSid { get; init; }

    /// <summary>Render the declared request fields as form pairs; lists use repeated keys and null values are omitted by the transport.</summary>
    public IEnumerable<KeyValuePair<string, string?>> ToForm()
    {
        yield return new("SipDomainSid", SipDomainSid);
    }
}

/// <summary>Parameters for create voice v1 byoc trunk request.</summary>
public sealed record CreateVoiceV1ByocTrunkRequest : IFormSerializable
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
    /// <summary>URL used for status callback.</summary>
    public string? StatusCallbackUrl { get; init; }
    /// <summary>HTTP method for status callback requests.</summary>
    public string? StatusCallbackMethod { get; init; }
    /// <summary>Whether cnam lookup enabled is enabled for this operation.</summary>
    public bool? CnamLookupEnabled { get; init; }
    /// <summary>SID of the related Connection Policy.</summary>
    public string? ConnectionPolicySid { get; init; }
    /// <summary>SID of the related From Domain.</summary>
    public string? FromDomainSid { get; init; }

    /// <summary>Render the declared request fields as form pairs; lists use repeated keys and null values are omitted by the transport.</summary>
    public IEnumerable<KeyValuePair<string, string?>> ToForm()
    {
        yield return new("FriendlyName", FriendlyName);
        yield return new("VoiceUrl", VoiceUrl);
        yield return new("VoiceMethod", VoiceMethod);
        yield return new("VoiceFallbackUrl", VoiceFallbackUrl);
        yield return new("VoiceFallbackMethod", VoiceFallbackMethod);
        yield return new("StatusCallbackUrl", StatusCallbackUrl);
        yield return new("StatusCallbackMethod", StatusCallbackMethod);
        yield return new("CnamLookupEnabled", FormHelpers.BoolStr(CnamLookupEnabled));
        yield return new("ConnectionPolicySid", ConnectionPolicySid);
        yield return new("FromDomainSid", FromDomainSid);
    }
}

/// <summary>Parameters for update voice v1 byoc trunk request.</summary>
public sealed record UpdateVoiceV1ByocTrunkRequest : IFormSerializable
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
    /// <summary>URL used for status callback.</summary>
    public string? StatusCallbackUrl { get; init; }
    /// <summary>HTTP method for status callback requests.</summary>
    public string? StatusCallbackMethod { get; init; }
    /// <summary>Whether cnam lookup enabled is enabled for this operation.</summary>
    public bool? CnamLookupEnabled { get; init; }
    /// <summary>SID of the related Connection Policy.</summary>
    public string? ConnectionPolicySid { get; init; }
    /// <summary>SID of the related From Domain.</summary>
    public string? FromDomainSid { get; init; }

    /// <summary>Render the declared request fields as form pairs; lists use repeated keys and null values are omitted by the transport.</summary>
    public IEnumerable<KeyValuePair<string, string?>> ToForm()
    {
        yield return new("FriendlyName", FriendlyName);
        yield return new("VoiceUrl", VoiceUrl);
        yield return new("VoiceMethod", VoiceMethod);
        yield return new("VoiceFallbackUrl", VoiceFallbackUrl);
        yield return new("VoiceFallbackMethod", VoiceFallbackMethod);
        yield return new("StatusCallbackUrl", StatusCallbackUrl);
        yield return new("StatusCallbackMethod", StatusCallbackMethod);
        yield return new("CnamLookupEnabled", FormHelpers.BoolStr(CnamLookupEnabled));
        yield return new("ConnectionPolicySid", ConnectionPolicySid);
        yield return new("FromDomainSid", FromDomainSid);
    }
}

/// <summary>Parameters for create voice v1 connection policy request.</summary>
public sealed record CreateVoiceV1ConnectionPolicyRequest : IFormSerializable
{
    /// <summary>Human-readable name used to identify this resource.</summary>
    public string? FriendlyName { get; init; }

    /// <summary>Render the declared request fields as form pairs; lists use repeated keys and null values are omitted by the transport.</summary>
    public IEnumerable<KeyValuePair<string, string?>> ToForm()
    {
        yield return new("FriendlyName", FriendlyName);
    }
}

/// <summary>Parameters for update voice v1 connection policy request.</summary>
public sealed record UpdateVoiceV1ConnectionPolicyRequest : IFormSerializable
{
    /// <summary>Human-readable name used to identify this resource.</summary>
    public string? FriendlyName { get; init; }

    /// <summary>Render the declared request fields as form pairs; lists use repeated keys and null values are omitted by the transport.</summary>
    public IEnumerable<KeyValuePair<string, string?>> ToForm()
    {
        yield return new("FriendlyName", FriendlyName);
    }
}

/// <summary>Parameters for create voice v1 connection policy target request.</summary>
public sealed record CreateVoiceV1ConnectionPolicyTargetRequest : IFormSerializable
{
    /// <summary>The SIP URI target.</summary>
    public required string Target { get; init; }
    /// <summary>Human-readable name used to identify this resource.</summary>
    public string? FriendlyName { get; init; }
    /// <summary>Lower = higher priority. Default 10.</summary>
    public int? Priority { get; init; }
    /// <summary>Load-balancing weight among equal priorities. Default 10.</summary>
    public int? Weight { get; init; }
    /// <summary>Whether the target is active. Default true.</summary>
    public bool? Enabled { get; init; }

    /// <summary>Render the declared request fields as form pairs; lists use repeated keys and null values are omitted by the transport.</summary>
    public IEnumerable<KeyValuePair<string, string?>> ToForm()
    {
        yield return new("Target", Target);
        yield return new("FriendlyName", FriendlyName);
        yield return new("Priority", Priority?.ToString());
        yield return new("Weight", Weight?.ToString());
        yield return new("Enabled", FormHelpers.BoolStr(Enabled));
    }
}

/// <summary>Parameters for update voice v1 connection policy target request.</summary>
public sealed record UpdateVoiceV1ConnectionPolicyTargetRequest : IFormSerializable
{
    /// <summary>Human-readable name used to identify this resource.</summary>
    public string? FriendlyName { get; init; }
    /// <summary>The SIP URI target.</summary>
    public string? Target { get; init; }
    /// <summary>Lower = higher priority. Default 10.</summary>
    public int? Priority { get; init; }
    /// <summary>Load-balancing weight among equal priorities. Default 10.</summary>
    public int? Weight { get; init; }
    /// <summary>Whether the target is active. Default true.</summary>
    public bool? Enabled { get; init; }

    /// <summary>Render the declared request fields as form pairs; lists use repeated keys and null values are omitted by the transport.</summary>
    public IEnumerable<KeyValuePair<string, string?>> ToForm()
    {
        yield return new("FriendlyName", FriendlyName);
        yield return new("Target", Target);
        yield return new("Priority", Priority?.ToString());
        yield return new("Weight", Weight?.ToString());
        yield return new("Enabled", FormHelpers.BoolStr(Enabled));
    }
}

/// <summary>Parameters for update voice v1 settings request.</summary>
public sealed record UpdateVoiceV1SettingsRequest : IFormSerializable
{
    /// <summary>Whether subaccounts inherit the parent's dialing permissions.</summary>
    public bool? DialingPermissionsInheritance { get; init; }

    /// <summary>Render the declared request fields as form pairs; lists use repeated keys and null values are omitted by the transport.</summary>
    public IEnumerable<KeyValuePair<string, string?>> ToForm()
    {
        yield return new("DialingPermissionsInheritance", FormHelpers.BoolStr(DialingPermissionsInheritance));
    }
}
