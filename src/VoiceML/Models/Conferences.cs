using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace VoiceML.Models;

/// <summary>A Twilio-shape Conference resource.</summary>
public sealed record Conference
{
    /// <summary>Conference SID (<c>CF</c> + 32 hex).</summary>
    [JsonPropertyName("sid")] public string Sid { get; init; } = "";

    /// <summary>Owning Account SID.</summary>
    [JsonPropertyName("account_sid")] public string AccountSid { get; init; } = "";

    /// <summary>Friendly name set by the customer (or auto-generated).</summary>
    [JsonPropertyName("friendly_name")] public string FriendlyName { get; init; } = "";

    /// <summary>Current status: <c>init</c>, <c>in-progress</c>, or <c>completed</c>. Server-side, VoiceML emits only <c>in-progress</c>/<c>completed</c>; <c>init</c> is documented in the spec for Twilio enum-deserializer parity.</summary>
    [JsonPropertyName("status")] public string Status { get; init; } = "";

    /// <summary>Conference region.</summary>
    [JsonPropertyName("region")] public string? Region { get; init; }

    /// <summary>Twilio API version label.</summary>
    [JsonPropertyName("api_version")] public string ApiVersion { get; init; } = "";

    /// <summary>URI of this conference.</summary>
    [JsonPropertyName("uri")] public string Uri { get; init; } = "";

    /// <summary>RFC 3339 creation timestamp.</summary>
    [JsonPropertyName("date_created")] public string? DateCreated { get; init; }

    /// <summary>RFC 3339 last-modification timestamp.</summary>
    [JsonPropertyName("date_updated")] public string? DateUpdated { get; init; }

    /// <summary>Reason the conference ended (free-form).</summary>
    [JsonPropertyName("reason_conference_ended")] public string? ReasonConferenceEnded { get; init; }

    /// <summary>SID of the call that triggered the conference end.</summary>
    [JsonPropertyName("call_sid_ending_conference")] public string? CallSidEndingConference { get; init; }

    /// <summary>Map of subresource name → URI.</summary>
    [JsonPropertyName("subresource_uris")] public Dictionary<string, string>? SubresourceUris { get; init; }

    /// <summary>Live participant count.</summary>
    [JsonPropertyName("member_count")] public int? MemberCount { get; init; }
}

/// <summary>List response for <c>GET /Conferences</c>.</summary>
public sealed record ConferenceList : Page
{
    /// <summary>The page of Conference resources.</summary>
    [JsonPropertyName("conferences")] public List<Conference> Conferences { get; init; } = new();
}

/// <summary>A live participant in a conference.</summary>
public sealed record Participant
{
    /// <summary>The call SID joined into the conference.</summary>
    [JsonPropertyName("call_sid")] public string CallSid { get; init; } = "";

    /// <summary>The conference SID this participant is part of.</summary>
    [JsonPropertyName("conference_sid")] public string ConferenceSid { get; init; } = "";

    /// <summary>Owning Account SID.</summary>
    [JsonPropertyName("account_sid")] public string AccountSid { get; init; } = "";

    /// <summary>Whether the participant's audio is muted.</summary>
    [JsonPropertyName("muted")] public bool Muted { get; init; }

    /// <summary>Whether the participant is on hold.</summary>
    [JsonPropertyName("hold")] public bool Hold { get; init; }

    /// <summary>When <see cref="Coaching"/> is true, the CallSid of the participant this coach
    /// can speak to (Twilio whisper-coach). Empty when not coaching.</summary>
    [JsonPropertyName("call_sid_to_coach")] public string? CallSidToCoach { get; init; }

    /// <summary>True when this participant joined with the coach role (Twilio whisper-coach).</summary>
    [JsonPropertyName("coaching")] public bool Coaching { get; init; }

    /// <summary>Pre-join queue-wait in seconds, string-encoded per Twilio's wire shape.</summary>
    [JsonPropertyName("queue_time")] public string QueueTime { get; init; } = "";

    /// <summary>Whether joining starts the conference.</summary>
    [JsonPropertyName("start_conference_on_enter")] public bool StartConferenceOnEnter { get; init; }

    /// <summary>Whether leaving ends the conference.</summary>
    [JsonPropertyName("end_conference_on_exit")] public bool EndConferenceOnExit { get; init; }

    /// <summary>Participant status (<c>queued</c>, <c>connecting</c>, <c>ringing</c>, <c>connected</c>,
    /// <c>on-hold</c>, <c>complete</c>, <c>failed</c>, <c>completed</c>).</summary>
    [JsonPropertyName("status")] public string Status { get; init; } = "";

    /// <summary>Friendly label.</summary>
    [JsonPropertyName("label")] public string? Label { get; init; }

    /// <summary>Twilio API version label.</summary>
    [JsonPropertyName("api_version")] public string ApiVersion { get; init; } = "";

    /// <summary>URI of this participant.</summary>
    [JsonPropertyName("uri")] public string Uri { get; init; } = "";

    /// <summary>RFC 3339 creation timestamp.</summary>
    [JsonPropertyName("date_created")] public string? DateCreated { get; init; }

    /// <summary>RFC 3339 last-modification timestamp.</summary>
    [JsonPropertyName("date_updated")] public string? DateUpdated { get; init; }
}

/// <summary>List response for <c>GET /Conferences/{sid}/Participants</c>.</summary>
public sealed record ParticipantList : Page
{
    /// <summary>The page of Participant resources.</summary>
    [JsonPropertyName("participants")] public List<Participant> Participants { get; init; } = new();
}

/// <summary>Query-string params for <c>GET /Conferences</c>.</summary>
public sealed record ListConferencesParams
{
    /// <summary>Filter to conferences with this exact friendly name.</summary>
    public string? FriendlyName { get; init; }

    /// <summary>Filter to conferences in this lifecycle state.</summary>
    public string? Status { get; init; }

    /// <summary>Zero-based page index.</summary>
    public int? Page { get; init; }

    /// <summary>Page size.</summary>
    public int? PageSize { get; init; }

    /// <summary>Render as a query-parameter sequence.</summary>
    public IEnumerable<KeyValuePair<string, string?>> ToQuery()
    {
        yield return new("FriendlyName", FriendlyName);
        yield return new("Status", Status);
        yield return new("Page", Page?.ToString());
        yield return new("PageSize", PageSize?.ToString());
    }
}

/// <summary>Query-string params for <c>GET /Conferences/{sid}/Participants</c>.</summary>
public sealed record ListParticipantsParams
{
    /// <summary>Filter to participants with this muted state.</summary>
    public bool? Muted { get; init; }

    /// <summary>Filter to participants with this hold state.</summary>
    public bool? Hold { get; init; }

    /// <summary>Filter to participants with this coaching state.</summary>
    public bool? Coaching { get; init; }

    /// <summary>Zero-based page index.</summary>
    public int? Page { get; init; }

    /// <summary>Page size.</summary>
    public int? PageSize { get; init; }

    /// <summary>Render as a query-parameter sequence.</summary>
    public IEnumerable<KeyValuePair<string, string?>> ToQuery()
    {
        yield return new("Muted", FormHelpers.BoolStr(Muted));
        yield return new("Hold", FormHelpers.BoolStr(Hold));
        yield return new("Coaching", FormHelpers.BoolStr(Coaching));
        yield return new("Page", Page?.ToString());
        yield return new("PageSize", PageSize?.ToString());
    }
}

/// <summary>Body for <c>POST /Conferences/{sid}</c>. v1 supports only <c>Status=completed</c>.</summary>
public sealed record EndConferenceRequest : IFormSerializable
{
    /// <summary>Always <c>completed</c> in this version.</summary>
    public string Status { get; init; } = "completed";

    /// <inheritdoc/>
    public IEnumerable<KeyValuePair<string, string?>> ToForm()
    {
        yield return new("Status", Status);
    }
}

/// <summary>Body for <c>POST /Conferences/{sid}/Participants/{callSid}</c>. At least one of
/// <see cref="Muted"/> / <see cref="Hold"/> must be set.</summary>
public sealed record UpdateParticipantRequest : IFormSerializable
{
    /// <summary>Mute/unmute the participant.</summary>
    public bool? Muted { get; init; }

    /// <summary>Place the participant on hold, or take them off hold.</summary>
    public bool? Hold { get; init; }

    /// <inheritdoc/>
    public IEnumerable<KeyValuePair<string, string?>> ToForm()
    {
        yield return new("Muted", FormHelpers.BoolStr(Muted));
        yield return new("Hold", FormHelpers.BoolStr(Hold));
    }
}
