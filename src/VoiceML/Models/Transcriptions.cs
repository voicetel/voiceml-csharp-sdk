using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace VoiceML.Models;

/// <summary>A real-time call-transcription resource (REST equivalent of <c>&lt;Start&gt;&lt;Transcription&gt;</c>).</summary>
public sealed record CallTranscription
{
    /// <summary>Transcription SID.</summary>
    [JsonPropertyName("sid")] public string? Sid { get; init; }

    /// <summary>Owning Account SID.</summary>
    [JsonPropertyName("account_sid")] public string? AccountSid { get; init; }

    /// <summary>Source Call SID.</summary>
    [JsonPropertyName("call_sid")] public string? CallSid { get; init; }

    /// <summary>Friendly name.</summary>
    [JsonPropertyName("name")] public string? Name { get; init; }

    /// <summary>BCP-47 language code.</summary>
    [JsonPropertyName("language_code")] public string? LanguageCode { get; init; }

    /// <summary>Engine: <c>deepgram</c>, <c>google</c>, <c>aws</c>, or <c>azure</c>.</summary>
    [JsonPropertyName("transcription_engine")] public string? TranscriptionEngine { get; init; }

    /// <summary>Status: <c>in-progress</c> or <c>stopped</c>.</summary>
    [JsonPropertyName("status")] public string Status { get; init; } = "";

    /// <summary>Legacy field, no longer returned by the server; retained as nullable for older response payloads.</summary>
    [JsonPropertyName("api_version")] public string? ApiVersion { get; init; }

    /// <summary>URI of this transcription.</summary>
    [JsonPropertyName("uri")] public string? Uri { get; init; }

    /// <summary>RFC 3339 creation timestamp.</summary>
    [JsonPropertyName("date_created")] public string? DateCreated { get; init; }

    /// <summary>RFC 3339 last-modification timestamp.</summary>
    [JsonPropertyName("date_updated")] public string? DateUpdated { get; init; }
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

    /// <summary>Engine-specific model hint (e.g. Deepgram's <c>nova-3</c>).</summary>
    public string? SpeechModel { get; init; }

    /// <summary>Whether the engine should punctuate transcripts automatically.</summary>
    public bool? EnableAutomaticPunctuation { get; init; }

    /// <summary>Accepted for Twilio compatibility; VoiceML has no Conversation Intelligence,
    /// so a set value logs a 32655 Notification and transcription proceeds.</summary>
    public string? IntelligenceService { get; init; }

    /// <summary>Transcription lifecycle webhook (Twilio's REST name; no <c>StatusCallback</c>
    /// alias). Renamed from the pre-0.9.5 <c>StatusCallback</c> property, whose wire name the
    /// server never read.</summary>
    public string? StatusCallbackUrl { get; init; }

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
        yield return new("SpeechModel", SpeechModel);
        yield return new("EnableAutomaticPunctuation", FormHelpers.BoolStr(EnableAutomaticPunctuation));
        yield return new("IntelligenceService", IntelligenceService);
        yield return new("StatusCallbackUrl", StatusCallbackUrl);
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

/// <summary>Query-string params for the transcription list endpoints
/// (<c>GET /Transcriptions</c> and <c>GET /Recordings/{sid}/Transcriptions</c>).</summary>
public sealed record ListTranscriptionsParams
{
    /// <summary>Filter to transcriptions created on this UTC date (<c>YYYY-MM-DD</c>).</summary>
    public string? DateCreated { get; init; }

    /// <summary>Transcriptions created strictly before this UTC date/time. Wire name: <c>DateCreated&lt;</c>.</summary>
    public string? DateCreatedLt { get; init; }

    /// <summary>Transcriptions created strictly after this UTC date/time. Wire name: <c>DateCreated&gt;</c>.</summary>
    public string? DateCreatedGt { get; init; }

    /// <summary>Zero-based page index.</summary>
    public int? Page { get; init; }

    /// <summary>Page size.</summary>
    public int? PageSize { get; init; }

    /// <summary>Opaque cursor for the next page, provided by the API.</summary>
    public string? PageToken { get; init; }

    /// <summary>Render as a query-parameter sequence.</summary>
    public IEnumerable<KeyValuePair<string, string?>> ToQuery()
    {
        yield return new("DateCreated", DateCreated);
        yield return new("DateCreated<", DateCreatedLt);
        yield return new("DateCreated>", DateCreatedGt);
        yield return new("Page", Page?.ToString());
        yield return new("PageSize", PageSize?.ToString());
        yield return new("PageToken", PageToken);
    }
}
