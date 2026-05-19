using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace VoiceML.Models;

/// <summary>A real-time call-transcription resource (REST equivalent of <c>&lt;Start&gt;&lt;Transcription&gt;</c>).</summary>
public sealed record CallTranscription
{
    /// <summary>Transcription SID.</summary>
    [JsonPropertyName("sid")] public string Sid { get; init; } = "";

    /// <summary>Owning Account SID.</summary>
    [JsonPropertyName("account_sid")] public string AccountSid { get; init; } = "";

    /// <summary>Source Call SID.</summary>
    [JsonPropertyName("call_sid")] public string CallSid { get; init; } = "";

    /// <summary>Friendly name.</summary>
    [JsonPropertyName("name")] public string? Name { get; init; }

    /// <summary>BCP-47 language code.</summary>
    [JsonPropertyName("language_code")] public string? LanguageCode { get; init; }

    /// <summary>Engine: <c>deepgram</c>, <c>google</c>, <c>aws</c>, or <c>azure</c>.</summary>
    [JsonPropertyName("transcription_engine")] public string? TranscriptionEngine { get; init; }

    /// <summary>Status: <c>in-progress</c> or <c>stopped</c>.</summary>
    [JsonPropertyName("status")] public string Status { get; init; } = "";

    /// <summary>Twilio API version label.</summary>
    [JsonPropertyName("api_version")] public string ApiVersion { get; init; } = "";

    /// <summary>URI of this transcription.</summary>
    [JsonPropertyName("uri")] public string Uri { get; init; } = "";

    /// <summary>RFC 3339 creation timestamp.</summary>
    [JsonPropertyName("date_created")] public string? DateCreated { get; init; }

    /// <summary>RFC 3339 last-modification timestamp.</summary>
    [JsonPropertyName("date_updated")] public string? DateUpdated { get; init; }
}

/// <summary>List response for <c>GET /Calls/{sid}/Transcriptions</c>.</summary>
public sealed record TranscriptionList : Page
{
    /// <summary>The page of CallTranscription resources.</summary>
    [JsonPropertyName("transcriptions")] public List<CallTranscription> Transcriptions { get; init; } = new();
}

/// <summary>Body for <c>POST /Calls/{sid}/Transcriptions</c>.</summary>
public sealed record StartTranscriptionRequest : IFormSerializable
{
    /// <summary>Friendly name.</summary>
    public string? Name { get; init; }

    /// <summary>Track selector: <c>inbound_track</c>, <c>outbound_track</c>, or <c>both_tracks</c>.</summary>
    public string? Track { get; init; }

    /// <summary>BCP-47 language code.</summary>
    public string? LanguageCode { get; init; }

    /// <summary>Engine: <c>deepgram</c>, <c>google</c>, <c>aws</c>, or <c>azure</c>.</summary>
    public string? TranscriptionEngine { get; init; }

    /// <summary>Whether to filter profanity in results.</summary>
    public bool? ProfanityFilter { get; init; }

    /// <summary>Whether to emit partial (interim) results.</summary>
    public bool? PartialResults { get; init; }

    /// <summary>Engine hints (free-form comma-separated phrases).</summary>
    public string? Hints { get; init; }

    /// <summary>Status-callback URL.</summary>
    public string? StatusCallback { get; init; }

    /// <summary>HTTP method for the status callback.</summary>
    public string? StatusCallbackMethod { get; init; }

    /// <summary>Status-callback events to subscribe to.</summary>
    public string? StatusCallbackEvents { get; init; }

    /// <inheritdoc/>
    public IEnumerable<KeyValuePair<string, string?>> ToForm()
    {
        yield return new("Name", Name);
        yield return new("Track", Track);
        yield return new("LanguageCode", LanguageCode);
        yield return new("TranscriptionEngine", TranscriptionEngine);
        yield return new("ProfanityFilter", FormHelpers.BoolStr(ProfanityFilter));
        yield return new("PartialResults", FormHelpers.BoolStr(PartialResults));
        yield return new("Hints", Hints);
        yield return new("StatusCallback", StatusCallback);
        yield return new("StatusCallbackMethod", StatusCallbackMethod);
        yield return new("StatusCallbackEvents", StatusCallbackEvents);
    }
}

/// <summary>Body for <c>POST /Calls/{sid}/Transcriptions/{sid}</c>.</summary>
public sealed record StopTranscriptionRequest : IFormSerializable
{
    /// <summary>Always <c>stopped</c>.</summary>
    public string Status { get; init; } = "stopped";

    /// <inheritdoc/>
    public IEnumerable<KeyValuePair<string, string?>> ToForm()
    {
        yield return new("Status", Status);
    }
}
