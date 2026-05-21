using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using VoiceML.Exceptions;
using VoiceML.Models;

namespace VoiceML.Resources;

/// <summary>Operations on live conferences and their participants/recordings.</summary>
public sealed class ConferencesResource : ResourceBase
{
    /// <summary>Construct with the shared transport.</summary>
    public ConferencesResource(Transport transport) : base(transport) { }

    /// <summary>List conferences.</summary>
    public async Task<ConferenceList> ListAsync(ListConferencesParams? filter = null, CancellationToken ct = default)
    {
        var p = filter ?? new ListConferencesParams();
        var result = await Transport.SendAsync<ConferenceList>(
            HttpMethod.Get, Path("Conferences"), queryParams: p.ToQuery(), ct: ct).ConfigureAwait(false);
        return result ?? new ConferenceList();
    }

    /// <summary>Fetch a single conference by SID.</summary>
    public async Task<Conference> GetAsync(string conferenceSid, CancellationToken ct = default)
    {
        var result = await Transport.SendAsync<Conference>(
            HttpMethod.Get, Path("Conferences", conferenceSid), ct: ct).ConfigureAwait(false);
        return result ?? throw new ApiException("empty body on GET /Conferences/{sid}", 200);
    }

    /// <summary>End a live conference.</summary>
    public async Task<Conference> EndAsync(
        string conferenceSid, EndConferenceRequest? request = null, CancellationToken ct = default)
    {
        var body = request ?? new EndConferenceRequest();
        var result = await Transport.SendAsync<Conference>(
            HttpMethod.Post,
            Path("Conferences", conferenceSid),
            formBody: body.ToForm(),
            ct: ct).ConfigureAwait(false);
        return result ?? throw new ApiException("empty body on POST /Conferences/{sid}", 200);
    }

    /// <summary>List participants in a conference.</summary>
    public async Task<ParticipantList> ListParticipantsAsync(
        string conferenceSid, ListParticipantsParams? filter = null, CancellationToken ct = default)
    {
        var p = filter ?? new ListParticipantsParams();
        var result = await Transport.SendAsync<ParticipantList>(
            HttpMethod.Get,
            Path("Conferences", conferenceSid, "Participants"),
            queryParams: p.ToQuery(),
            ct: ct).ConfigureAwait(false);
        return result ?? new ParticipantList();
    }

    /// <summary>Fetch a single participant.</summary>
    public async Task<Participant> GetParticipantAsync(
        string conferenceSid, string callSid, CancellationToken ct = default)
    {
        var result = await Transport.SendAsync<Participant>(
            HttpMethod.Get,
            Path("Conferences", conferenceSid, "Participants", callSid),
            ct: ct).ConfigureAwait(false);
        return result ?? throw new ApiException("empty body on GET /Conferences/{sid}/Participants/{csid}", 200);
    }

    /// <summary>Mute / hold a participant.</summary>
    public async Task<Participant> UpdateParticipantAsync(
        string conferenceSid, string callSid, UpdateParticipantRequest request, CancellationToken ct = default)
    {
        var result = await Transport.SendAsync<Participant>(
            HttpMethod.Post,
            Path("Conferences", conferenceSid, "Participants", callSid),
            formBody: request.ToForm(),
            ct: ct).ConfigureAwait(false);
        return result ?? throw new ApiException("empty body on POST /Conferences/{sid}/Participants/{csid}", 200);
    }

    /// <summary>Kick a participant out of a conference.</summary>
    public Task KickParticipantAsync(string conferenceSid, string callSid, CancellationToken ct = default)
        => Transport.SendNoContentAsync(
            HttpMethod.Delete,
            Path("Conferences", conferenceSid, "Participants", callSid),
            ct: ct);

    /// <summary>List recordings made on a conference.</summary>
    public async Task<RecordingList> ListRecordingsAsync(string conferenceSid, CancellationToken ct = default)
    {
        var result = await Transport.SendAsync<RecordingList>(
            HttpMethod.Get,
            Path("Conferences", conferenceSid, "Recordings"),
            ct: ct).ConfigureAwait(false);
        return result ?? new RecordingList();
    }
}
