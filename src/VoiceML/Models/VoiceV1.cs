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
    [JsonPropertyName("first_page_url")] public string? FirstPageUrl { get; init; }
    [JsonPropertyName("next_page_url")] public string? NextPageUrl { get; init; }
    [JsonPropertyName("previous_page_url")] public string? PreviousPageUrl { get; init; }
    [JsonPropertyName("url")] public string? Url { get; init; }
    [JsonPropertyName("page")] public int? Page { get; init; }
    [JsonPropertyName("page_size")] public int? PageSize { get; init; }
    [JsonPropertyName("key")] public string? Key { get; init; }
}

// ---- Response shapes -------------------------------------------------------

/// <summary>An allowed source IP for Voice v1 — <c>IL…</c>. Maps to a <see cref="VoiceV1SourceIpMapping"/>
/// for inbound call routing.</summary>
public sealed record VoiceV1IpRecord
{
    [JsonPropertyName("account_sid")] public string? AccountSid { get; init; }
    [JsonPropertyName("sid")] public string? Sid { get; init; }
    [JsonPropertyName("friendly_name")] public string? FriendlyName { get; init; }
    [JsonPropertyName("ip_address")] public string? IpAddress { get; init; }
    [JsonPropertyName("cidr_prefix_length")] public int CidrPrefixLength { get; init; }
    [JsonPropertyName("date_created")] public string? DateCreated { get; init; }
    [JsonPropertyName("date_updated")] public string? DateUpdated { get; init; }
    [JsonPropertyName("url")] public string? Url { get; init; }
}

public sealed record VoiceV1IpRecordList
{
    [JsonPropertyName("ip_records")] public List<VoiceV1IpRecord> IpRecords { get; init; } = new();
    [JsonPropertyName("meta")] public VoiceV1Meta? Meta { get; init; }
}

/// <summary>Source-IP-to-SIP-Domain binding — <c>IB…</c>. Routes inbound calls from an
/// <see cref="VoiceV1IpRecord"/> source to a SIP Domain.</summary>
public sealed record VoiceV1SourceIpMapping
{
    [JsonPropertyName("sid")] public string? Sid { get; init; }
    [JsonPropertyName("ip_record_sid")] public string? IpRecordSid { get; init; }
    [JsonPropertyName("sip_domain_sid")] public string? SipDomainSid { get; init; }
    [JsonPropertyName("date_created")] public string? DateCreated { get; init; }
    [JsonPropertyName("date_updated")] public string? DateUpdated { get; init; }
    [JsonPropertyName("url")] public string? Url { get; init; }
}

public sealed record VoiceV1SourceIpMappingList
{
    [JsonPropertyName("source_ip_mappings")] public List<VoiceV1SourceIpMapping> SourceIpMappings { get; init; } = new();
    [JsonPropertyName("meta")] public VoiceV1Meta? Meta { get; init; }
}

/// <summary>A bring-your-own-carrier trunk — <c>BY…</c>.</summary>
public sealed record VoiceV1ByocTrunk
{
    [JsonPropertyName("account_sid")] public string? AccountSid { get; init; }
    [JsonPropertyName("sid")] public string? Sid { get; init; }
    [JsonPropertyName("friendly_name")] public string? FriendlyName { get; init; }
    [JsonPropertyName("voice_url")] public string? VoiceUrl { get; init; }
    [JsonPropertyName("voice_method")] public string? VoiceMethod { get; init; }
    [JsonPropertyName("voice_fallback_url")] public string? VoiceFallbackUrl { get; init; }
    [JsonPropertyName("voice_fallback_method")] public string? VoiceFallbackMethod { get; init; }
    [JsonPropertyName("status_callback_url")] public string? StatusCallbackUrl { get; init; }
    [JsonPropertyName("status_callback_method")] public string? StatusCallbackMethod { get; init; }
    [JsonPropertyName("cnam_lookup_enabled")] public bool? CnamLookupEnabled { get; init; }
    [JsonPropertyName("connection_policy_sid")] public string? ConnectionPolicySid { get; init; }
    [JsonPropertyName("from_domain_sid")] public string? FromDomainSid { get; init; }
    [JsonPropertyName("date_created")] public string? DateCreated { get; init; }
    [JsonPropertyName("date_updated")] public string? DateUpdated { get; init; }
    [JsonPropertyName("url")] public string? Url { get; init; }
}

public sealed record VoiceV1ByocTrunkList
{
    [JsonPropertyName("byoc_trunks")] public List<VoiceV1ByocTrunk> ByocTrunks { get; init; } = new();
    [JsonPropertyName("meta")] public VoiceV1Meta? Meta { get; init; }
}

/// <summary>An origination policy — <c>NY…</c>. Holds an ordered set of
/// <see cref="VoiceV1ConnectionPolicyTarget"/>s with priority/weight failover.</summary>
public sealed record VoiceV1ConnectionPolicy
{
    [JsonPropertyName("account_sid")] public string? AccountSid { get; init; }
    [JsonPropertyName("sid")] public string? Sid { get; init; }
    [JsonPropertyName("friendly_name")] public string? FriendlyName { get; init; }
    [JsonPropertyName("date_created")] public string? DateCreated { get; init; }
    [JsonPropertyName("date_updated")] public string? DateUpdated { get; init; }
    [JsonPropertyName("url")] public string? Url { get; init; }
    [JsonPropertyName("links")] public Dictionary<string, string>? Links { get; init; }
}

public sealed record VoiceV1ConnectionPolicyList
{
    [JsonPropertyName("connection_policies")] public List<VoiceV1ConnectionPolicy> ConnectionPolicies { get; init; } = new();
    [JsonPropertyName("meta")] public VoiceV1Meta? Meta { get; init; }
}

/// <summary>A single SIP target inside a <see cref="VoiceV1ConnectionPolicy"/> — <c>NE…</c>.</summary>
public sealed record VoiceV1ConnectionPolicyTarget
{
    [JsonPropertyName("account_sid")] public string? AccountSid { get; init; }
    [JsonPropertyName("connection_policy_sid")] public string? ConnectionPolicySid { get; init; }
    [JsonPropertyName("sid")] public string? Sid { get; init; }
    [JsonPropertyName("friendly_name")] public string? FriendlyName { get; init; }
    [JsonPropertyName("target")] public string? Target { get; init; }
    [JsonPropertyName("priority")] public int Priority { get; init; }
    [JsonPropertyName("weight")] public int Weight { get; init; }
    [JsonPropertyName("enabled")] public bool? Enabled { get; init; }
    [JsonPropertyName("date_created")] public string? DateCreated { get; init; }
    [JsonPropertyName("date_updated")] public string? DateUpdated { get; init; }
    [JsonPropertyName("url")] public string? Url { get; init; }
}

public sealed record VoiceV1ConnectionPolicyTargetList
{
    [JsonPropertyName("targets")] public List<VoiceV1ConnectionPolicyTarget> Targets { get; init; } = new();
    [JsonPropertyName("meta")] public VoiceV1Meta? Meta { get; init; }
}

/// <summary>Account-wide DialingPermissions settings — singleton.</summary>
public sealed record VoiceV1DialingPermissionsSettings
{
    [JsonPropertyName("dialing_permissions_inheritance")] public bool? DialingPermissionsInheritance { get; init; }
    [JsonPropertyName("url")] public string? Url { get; init; }
}

// ---- Request bodies (form-encoded) ----------------------------------------

public sealed record CreateVoiceV1IpRecordRequest : IFormSerializable
{
    public required string IpAddress { get; init; }
    public string? FriendlyName { get; init; }
    /// <summary>Defaults server-side to 32 when omitted.</summary>
    public int? CidrPrefixLength { get; init; }

    public IEnumerable<KeyValuePair<string, string?>> ToForm()
    {
        yield return new("IpAddress", IpAddress);
        yield return new("FriendlyName", FriendlyName);
        yield return new("CidrPrefixLength", CidrPrefixLength?.ToString());
    }
}

public sealed record UpdateVoiceV1IpRecordRequest : IFormSerializable
{
    public string? FriendlyName { get; init; }

    public IEnumerable<KeyValuePair<string, string?>> ToForm()
    {
        yield return new("FriendlyName", FriendlyName);
    }
}

public sealed record CreateVoiceV1SourceIpMappingRequest : IFormSerializable
{
    public required string IpRecordSid { get; init; }
    public required string SipDomainSid { get; init; }

    public IEnumerable<KeyValuePair<string, string?>> ToForm()
    {
        yield return new("IpRecordSid", IpRecordSid);
        yield return new("SipDomainSid", SipDomainSid);
    }
}

public sealed record UpdateVoiceV1SourceIpMappingRequest : IFormSerializable
{
    public required string SipDomainSid { get; init; }

    public IEnumerable<KeyValuePair<string, string?>> ToForm()
    {
        yield return new("SipDomainSid", SipDomainSid);
    }
}

public sealed record CreateVoiceV1ByocTrunkRequest : IFormSerializable
{
    public string? FriendlyName { get; init; }
    public string? VoiceUrl { get; init; }
    public string? VoiceMethod { get; init; }
    public string? VoiceFallbackUrl { get; init; }
    public string? VoiceFallbackMethod { get; init; }
    public string? StatusCallbackUrl { get; init; }
    public string? StatusCallbackMethod { get; init; }
    public bool? CnamLookupEnabled { get; init; }
    public string? ConnectionPolicySid { get; init; }
    public string? FromDomainSid { get; init; }

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

public sealed record UpdateVoiceV1ByocTrunkRequest : IFormSerializable
{
    public string? FriendlyName { get; init; }
    public string? VoiceUrl { get; init; }
    public string? VoiceMethod { get; init; }
    public string? VoiceFallbackUrl { get; init; }
    public string? VoiceFallbackMethod { get; init; }
    public string? StatusCallbackUrl { get; init; }
    public string? StatusCallbackMethod { get; init; }
    public bool? CnamLookupEnabled { get; init; }
    public string? ConnectionPolicySid { get; init; }
    public string? FromDomainSid { get; init; }

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

public sealed record CreateVoiceV1ConnectionPolicyRequest : IFormSerializable
{
    public string? FriendlyName { get; init; }

    public IEnumerable<KeyValuePair<string, string?>> ToForm()
    {
        yield return new("FriendlyName", FriendlyName);
    }
}

public sealed record UpdateVoiceV1ConnectionPolicyRequest : IFormSerializable
{
    public string? FriendlyName { get; init; }

    public IEnumerable<KeyValuePair<string, string?>> ToForm()
    {
        yield return new("FriendlyName", FriendlyName);
    }
}

public sealed record CreateVoiceV1ConnectionPolicyTargetRequest : IFormSerializable
{
    public required string Target { get; init; }
    public string? FriendlyName { get; init; }
    public int? Priority { get; init; }
    public int? Weight { get; init; }
    public bool? Enabled { get; init; }

    public IEnumerable<KeyValuePair<string, string?>> ToForm()
    {
        yield return new("Target", Target);
        yield return new("FriendlyName", FriendlyName);
        yield return new("Priority", Priority?.ToString());
        yield return new("Weight", Weight?.ToString());
        yield return new("Enabled", FormHelpers.BoolStr(Enabled));
    }
}

public sealed record UpdateVoiceV1ConnectionPolicyTargetRequest : IFormSerializable
{
    public string? FriendlyName { get; init; }
    public string? Target { get; init; }
    public int? Priority { get; init; }
    public int? Weight { get; init; }
    public bool? Enabled { get; init; }

    public IEnumerable<KeyValuePair<string, string?>> ToForm()
    {
        yield return new("FriendlyName", FriendlyName);
        yield return new("Target", Target);
        yield return new("Priority", Priority?.ToString());
        yield return new("Weight", Weight?.ToString());
        yield return new("Enabled", FormHelpers.BoolStr(Enabled));
    }
}

public sealed record UpdateVoiceV1SettingsRequest : IFormSerializable
{
    public bool? DialingPermissionsInheritance { get; init; }

    public IEnumerable<KeyValuePair<string, string?>> ToForm()
    {
        yield return new("DialingPermissionsInheritance", FormHelpers.BoolStr(DialingPermissionsInheritance));
    }
}
