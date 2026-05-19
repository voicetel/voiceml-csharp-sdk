using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace VoiceML.Models;

/// <summary>A SIPREC-session resource (REST equivalent of <c>&lt;Start&gt;&lt;Siprec&gt;</c>).</summary>
public sealed record SiprecSession
{
    /// <summary>SIPREC session SID.</summary>
    [JsonPropertyName("sid")] public string Sid { get; init; } = "";

    /// <summary>Owning Account SID.</summary>
    [JsonPropertyName("account_sid")] public string AccountSid { get; init; } = "";

    /// <summary>Source Call SID.</summary>
    [JsonPropertyName("call_sid")] public string CallSid { get; init; } = "";

    /// <summary>Friendly name.</summary>
    [JsonPropertyName("name")] public string? Name { get; init; }

    /// <summary>SRC connector name.</summary>
    [JsonPropertyName("connector_name")] public string? ConnectorName { get; init; }

    /// <summary>Status: <c>in-progress</c> or <c>stopped</c>.</summary>
    [JsonPropertyName("status")] public string Status { get; init; } = "";

    /// <summary>Twilio API version label.</summary>
    [JsonPropertyName("api_version")] public string ApiVersion { get; init; } = "";

    /// <summary>URI of this SIPREC session.</summary>
    [JsonPropertyName("uri")] public string Uri { get; init; } = "";

    /// <summary>RFC 3339 creation timestamp.</summary>
    [JsonPropertyName("date_created")] public string? DateCreated { get; init; }

    /// <summary>RFC 3339 last-modification timestamp.</summary>
    [JsonPropertyName("date_updated")] public string? DateUpdated { get; init; }
}

/// <summary>List response for <c>GET /Calls/{sid}/Siprec</c>.</summary>
public sealed record SiprecList : Page
{
    /// <summary>The page of SIPREC-session resources.</summary>
    [JsonPropertyName("siprec")] public List<SiprecSession> Siprec { get; init; } = new();
}

/// <summary>Body for <c>POST /Calls/{sid}/Siprec</c>.</summary>
public sealed record StartSiprecRequest : IFormSerializable
{
    /// <summary>Friendly name.</summary>
    public string? Name { get; init; }

    /// <summary>SRC connector name.</summary>
    public string? ConnectorName { get; init; }

    /// <summary>Track selector: <c>inbound_track</c>, <c>outbound_track</c>, or <c>both_tracks</c>.</summary>
    public string? Track { get; init; }

    /// <summary>Status-callback URL.</summary>
    public string? StatusCallback { get; init; }

    /// <summary>HTTP method for the status callback.</summary>
    public string? StatusCallbackMethod { get; init; }

    /// <inheritdoc/>
    public IEnumerable<KeyValuePair<string, string?>> ToForm()
    {
        yield return new("Name", Name);
        yield return new("ConnectorName", ConnectorName);
        yield return new("Track", Track);
        yield return new("StatusCallback", StatusCallback);
        yield return new("StatusCallbackMethod", StatusCallbackMethod);
    }
}

/// <summary>Body for <c>POST /Calls/{sid}/Siprec/{sid}</c>. Clears VoiceML's session tracking
/// only — the SRS recording itself continues until call hangup.</summary>
public sealed record StopSiprecRequest : IFormSerializable
{
    /// <summary>Always <c>stopped</c>.</summary>
    public string Status { get; init; } = "stopped";

    /// <inheritdoc/>
    public IEnumerable<KeyValuePair<string, string?>> ToForm()
    {
        yield return new("Status", Status);
    }
}
