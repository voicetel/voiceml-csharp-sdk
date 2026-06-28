using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace VoiceML.Models;

/// <summary>A Twilio-compatible Account resource (<c>AC</c> + 32 hex). VoiceML exposes
/// the canonical Account fetch / update surface for migration compatibility — the
/// <see cref="Sid"/> echoes the AccountSid of the credentials in use.</summary>
public sealed record Account
{
    /// <summary>Account SID (<c>AC</c> + 32 hex).</summary>
    [JsonPropertyName("sid")] public string Sid { get; init; } = "";

    /// <summary>Parent (owner) Account SID — same as <see cref="Sid"/> for top-level accounts.</summary>
    [JsonPropertyName("owner_account_sid")] public string? OwnerAccountSid { get; init; }

    /// <summary>Friendly name. Mutable via update.</summary>
    [JsonPropertyName("friendly_name")] public string? FriendlyName { get; init; }

    /// <summary>Account status: <c>active</c>, <c>suspended</c>, or <c>closed</c>.</summary>
    [JsonPropertyName("status")] public string? Status { get; init; }

    /// <summary>Account type: <c>Trial</c> or <c>Full</c>.</summary>
    [JsonPropertyName("type")] public string? Type { get; init; }

    /// <summary>The account auth token. Twilio echoes this on fetch; rotate via the API key endpoints.</summary>
    [JsonPropertyName("auth_token")] public string? AuthToken { get; init; }

    /// <summary>RFC 2822 creation timestamp.</summary>
    [JsonPropertyName("date_created")] public string? DateCreated { get; init; }

    /// <summary>RFC 2822 last-modification timestamp.</summary>
    [JsonPropertyName("date_updated")] public string? DateUpdated { get; init; }

    /// <summary>URI of this Account resource.</summary>
    [JsonPropertyName("uri")] public string? Uri { get; init; }

    /// <summary>Map of sub-resource URIs (Calls, Conferences, Messages, etc.).</summary>
    [JsonPropertyName("subresource_uris")] public Dictionary<string, string>? SubresourceUris { get; init; }
}
