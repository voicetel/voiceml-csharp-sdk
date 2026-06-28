using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace VoiceML.Models;

/// <summary>A Twilio-compatible recording-derived transcription resource
/// (<c>TR</c> + 32 hex). Distinct from <see cref="CallTranscription"/>:
/// <see cref="CallTranscription"/> models the realtime <c>&lt;Start&gt;&lt;Transcription&gt;</c>
/// stream attached to a live call; this resource models the offline transcript
/// derived from a completed <c>Recording</c>. Surfaced at
/// <c>/2010-04-01/Accounts/{AccountSid}/Transcriptions</c> and
/// <c>/2010-04-01/Accounts/{AccountSid}/Recordings/{RecordingSid}/Transcriptions</c>
/// — same shape on both paths.</summary>
public sealed record RecordingTranscription
{
    /// <summary>Transcription SID (<c>TR</c> + 32 hex).</summary>
    [JsonPropertyName("sid")] public string Sid { get; init; } = "";

    /// <summary>Owning Account SID.</summary>
    [JsonPropertyName("account_sid")] public string AccountSid { get; init; } = "";

    /// <summary>Twilio API version label.</summary>
    [JsonPropertyName("api_version")] public string? ApiVersion { get; init; }

    /// <summary>Recording SID this transcription was derived from.</summary>
    [JsonPropertyName("recording_sid")] public string? RecordingSid { get; init; }

    /// <summary>Transcription duration in whole seconds, as a decimal string.</summary>
    [JsonPropertyName("duration")] public string? Duration { get; init; }

    /// <summary>Per-transcription price as a decimal string. <c>null</c> until rated.</summary>
    [JsonPropertyName("price")] public string? Price { get; init; }

    /// <summary>Currency code for <see cref="Price"/> (e.g. <c>USD</c>).</summary>
    [JsonPropertyName("price_unit")] public string? PriceUnit { get; init; }

    /// <summary>Status: <c>in-progress</c>, <c>completed</c>, or <c>failed</c>.</summary>
    [JsonPropertyName("status")] public string? Status { get; init; }

    /// <summary>Transcribed text. <c>null</c> while pending or on failure.</summary>
    [JsonPropertyName("transcription_text")] public string? TranscriptionText { get; init; }

    /// <summary>Transcription tier: <c>fast</c> or <c>full</c>.</summary>
    [JsonPropertyName("type")] public string? Type { get; init; }

    /// <summary>URI of this transcription resource.</summary>
    [JsonPropertyName("uri")] public string? Uri { get; init; }

    /// <summary>RFC 2822 creation timestamp.</summary>
    [JsonPropertyName("date_created")] public string? DateCreated { get; init; }

    /// <summary>RFC 2822 last-modification timestamp.</summary>
    [JsonPropertyName("date_updated")] public string? DateUpdated { get; init; }
}

/// <summary>List response for the recording-derived transcriptions surfaces
/// (account-scoped <c>/Transcriptions</c> and recording-scoped
/// <c>/Recordings/{Sid}/Transcriptions</c>). Distinct from
/// <see cref="TranscriptionList"/>, which lists realtime <see cref="CallTranscription"/>
/// resources tied to a live call.</summary>
public sealed record RecordingTranscriptionList : Page
{
    /// <summary>The page of recording-derived transcription resources.</summary>
    [JsonPropertyName("transcriptions")] public List<RecordingTranscription> Transcriptions { get; init; } = new();
}
