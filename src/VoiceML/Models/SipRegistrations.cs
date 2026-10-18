using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace VoiceML.Models;

/// <summary>A registered SIP endpoint under the account's SIP Domains
/// (<c>GET /2010-04-01/Accounts/{AccountSid}/SIP/Registrations.json</c>). VoiceML extension —
/// every AOR registered under the account, with its contacts and, when the endpoint is on a
/// call, the active call.</summary>
public sealed record SipRegistration
{
    /// <summary>Address-of-record for the registration.</summary>
    [JsonPropertyName("aor")] public string? Aor { get; init; }

    /// <summary>Registered username.</summary>
    [JsonPropertyName("username")] public string? Username { get; init; }

    /// <summary>SIP Domain the registration belongs to.</summary>
    [JsonPropertyName("domain")] public string? Domain { get; init; }

    /// <summary>SID of the SIP Domain.</summary>
    [JsonPropertyName("domain_sid")] public string? DomainSid { get; init; }

    /// <summary>Registration status (<c>registered</c> or <c>unregistered</c>).</summary>
    [JsonPropertyName("status")] public string? Status { get; init; }

    /// <summary>Configured caller ID for the endpoint.</summary>
    [JsonPropertyName("caller_id")] public string? CallerId { get; init; }

    /// <summary>User-Agent of the registering endpoint.</summary>
    [JsonPropertyName("user_agent")] public string? UserAgent { get; init; }

    /// <summary>Timestamp of the last successful registration.</summary>
    [JsonPropertyName("last_registered")] public string? LastRegistered { get; init; }

    /// <summary>Active contact URIs for the AOR.</summary>
    [JsonPropertyName("contacts")] public List<JsonElement?> Contacts { get; init; } = new();

    /// <summary>When the endpoint is on a call: direction, remote party, and since-when.
    /// <c>null</c> when idle.</summary>
    [JsonPropertyName("active_call")] public JsonElement? ActiveCall { get; init; }
}

/// <summary>List response for <c>GET /SIP/Registrations</c>.</summary>
public sealed record SipRegistrationList : Page
{
    /// <summary>The page of SipRegistration resources.</summary>
    [JsonPropertyName("registrations")] public List<SipRegistration> Registrations { get; init; } = new();
}
