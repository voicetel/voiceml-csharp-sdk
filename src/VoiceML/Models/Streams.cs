using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace VoiceML.Models;

/// <summary>A media-stream resource (REST equivalent of <c>&lt;Connect&gt;&lt;Stream&gt;</c> /
/// <c>&lt;Start&gt;&lt;Stream&gt;</c>).</summary>
public sealed record Stream
{
    /// <summary>Stream SID.</summary>
    [JsonPropertyName("sid")] public string Sid { get; init; } = "";

    /// <summary>Owning Account SID.</summary>
    [JsonPropertyName("account_sid")] public string AccountSid { get; init; } = "";

    /// <summary>Source Call SID.</summary>
    [JsonPropertyName("call_sid")] public string CallSid { get; init; } = "";

    /// <summary>Friendly name.</summary>
    [JsonPropertyName("name")] public string? Name { get; init; }

    /// <summary>Status: <c>in-progress</c> or <c>stopped</c>.</summary>
    [JsonPropertyName("status")] public string Status { get; init; } = "";

    /// <summary>Twilio API version label.</summary>
    [JsonPropertyName("api_version")] public string ApiVersion { get; init; } = "";

    /// <summary>URI of this stream.</summary>
    [JsonPropertyName("uri")] public string Uri { get; init; } = "";

    /// <summary>RFC 3339 creation timestamp.</summary>
    [JsonPropertyName("date_created")] public string? DateCreated { get; init; }

    /// <summary>RFC 3339 last-modification timestamp.</summary>
    [JsonPropertyName("date_updated")] public string? DateUpdated { get; init; }
}

/// <summary>List response for <c>GET /Calls/{sid}/Streams</c>.</summary>
public sealed record StreamList : Page
{
    /// <summary>The page of Stream resources.</summary>
    [JsonPropertyName("streams")] public List<Stream> Streams { get; init; } = new();
}

/// <summary>Body for <c>POST /Calls/{sid}/Streams</c>. <see cref="Url"/> is the wss:// endpoint.</summary>
public sealed record StartStreamRequest : IFormSerializable
{
    /// <summary>WebSocket endpoint URL.</summary>
    public required string Url { get; init; }

    /// <summary>Track selector: <c>inbound_track</c>, <c>outbound_track</c>, or <c>both_tracks</c>.</summary>
    public string? Track { get; init; }

    /// <summary>Friendly name.</summary>
    public string? Name { get; init; }

    /// <summary>Status-callback URL.</summary>
    public string? StatusCallback { get; init; }

    /// <summary>HTTP method for the status callback.</summary>
    public string? StatusCallbackMethod { get; init; }

    /// <inheritdoc/>
    public IEnumerable<KeyValuePair<string, string?>> ToForm()
    {
        yield return new("Url", Url);
        yield return new("Track", Track);
        yield return new("Name", Name);
        yield return new("StatusCallback", StatusCallback);
        yield return new("StatusCallbackMethod", StatusCallbackMethod);
    }
}

/// <summary>Body for <c>POST /Calls/{sid}/Streams/{sid}</c>.</summary>
public sealed record StopStreamRequest : IFormSerializable
{
    /// <summary>Always <c>stopped</c>.</summary>
    public string Status { get; init; } = "stopped";

    /// <inheritdoc/>
    public IEnumerable<KeyValuePair<string, string?>> ToForm()
    {
        yield return new("Status", Status);
    }
}
