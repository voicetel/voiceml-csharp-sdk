using System.Collections.Generic;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using VoiceML.Exceptions;
using VoiceML.Models;

namespace VoiceML.Resources;

/// <summary>Operations on live conferences and their participants/recordings.</summary>
public sealed class ConferencesResource : ResourceBase
{
    internal ConferencesResource(Transport transport) : base(transport) { }

    /// <summary>List conferences.</summary>
    public async Task<ConferenceList> ListAsync(ListConferencesParams? filter = null, CancellationToken ct = default)
    {
        var p = filter ?? new ListConferencesParams();
        var result = await Transport.SendAsync<ConferenceList>(
            HttpMethod.Get, Path("Conferences"), queryParams: p.ToQuery(), ct: ct).ConfigureAwait(false);
        return result ?? new ConferenceList();
    }

    /// <summary>Iterate through all conferences across pages, yielding one
    /// <see cref="Conference"/> at a time.</summary>
    public async IAsyncEnumerable<Conference> IterateAsync(
        ListConferencesParams? filter = null,
        [EnumeratorCancellation] CancellationToken ct = default)
    {
        var page = await ListAsync(filter, ct).ConfigureAwait(false);
        while (true)
        {
            foreach (var item in page.Conferences) yield return item;
            if (string.IsNullOrEmpty(page.NextPageUri)) yield break;
            page = await Transport.SendPageUriAsync<ConferenceList>(page.NextPageUri!, ct).ConfigureAwait(false)
                ?? throw new VoiceMLException("empty page while following next_page_uri");
        }
    }

    /// <summary>Fetch the page a conferences list response links to next, following its
    /// <see cref="Page.NextPageUri"/> verbatim. Returns <c>null</c> when the page is the last
    /// one.</summary>
    public Task<ConferenceList?> NextPageAsync(ConferenceList page, CancellationToken ct = default)
        => string.IsNullOrEmpty(page.NextPageUri)
            ? Task.FromResult<ConferenceList?>(null)
            : Transport.SendPageUriAsync<ConferenceList>(page.NextPageUri!, ct);

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

    /// <summary>Iterate through all participants in a conference across pages, yielding one
    /// <see cref="Participant"/> at a time.</summary>
    public async IAsyncEnumerable<Participant> IterateParticipantsAsync(
        string conferenceSid,
        ListParticipantsParams? filter = null,
        [EnumeratorCancellation] CancellationToken ct = default)
    {
        var page = await ListParticipantsAsync(conferenceSid, filter, ct).ConfigureAwait(false);
        while (true)
        {
            foreach (var item in page.Participants) yield return item;
            if (string.IsNullOrEmpty(page.NextPageUri)) yield break;
            page = await Transport.SendPageUriAsync<ParticipantList>(page.NextPageUri!, ct).ConfigureAwait(false)
                ?? throw new VoiceMLException("empty page while following next_page_uri");
        }
    }

    /// <summary>Fetch the page a participants list response links to next, following its
    /// <see cref="Page.NextPageUri"/> verbatim. Returns <c>null</c> when the page is the last
    /// one.</summary>
    public Task<ParticipantList?> NextParticipantsPageAsync(ParticipantList page, CancellationToken ct = default)
        => string.IsNullOrEmpty(page.NextPageUri)
            ? Task.FromResult<ParticipantList?>(null)
            : Transport.SendPageUriAsync<ParticipantList>(page.NextPageUri!, ct);

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

    /// <summary>Dial a leg into a conference.</summary>
    public async Task<Participant> CreateParticipantAsync(
        string conferenceSid, CreateParticipantRequest request, CancellationToken ct = default)
    {
        var result = await Transport.SendAsync<Participant>(
            HttpMethod.Post,
            Path("Conferences", conferenceSid, "Participants"),
            formBody: request.ToForm(),
            ct: ct).ConfigureAwait(false);
        return result ?? throw new ApiException("empty body on POST /Conferences/{sid}/Participants", 201);
    }

    /// <summary>List recordings made on a conference.</summary>
    public async Task<RecordingList> ListRecordingsAsync(
        string conferenceSid, ListCallRecordingsParams? filter = null, CancellationToken ct = default)
    {
        var p = filter ?? new ListCallRecordingsParams();
        var result = await Transport.SendAsync<RecordingList>(
            HttpMethod.Get,
            Path("Conferences", conferenceSid, "Recordings"),
            queryParams: p.ToQuery(),
            ct: ct).ConfigureAwait(false);
        return result ?? new RecordingList();
    }

    /// <summary>Iterate through all recordings on a conference across pages, yielding one
    /// <see cref="Recording"/> at a time.</summary>
    public async IAsyncEnumerable<Recording> IterateRecordingsAsync(
        string conferenceSid,
        ListCallRecordingsParams? filter = null,
        [EnumeratorCancellation] CancellationToken ct = default)
    {
        var page = await ListRecordingsAsync(conferenceSid, filter, ct).ConfigureAwait(false);
        while (true)
        {
            foreach (var item in page.Recordings) yield return item;
            if (string.IsNullOrEmpty(page.NextPageUri)) yield break;
            page = await Transport.SendPageUriAsync<RecordingList>(page.NextPageUri!, ct).ConfigureAwait(false)
                ?? throw new VoiceMLException("empty page while following next_page_uri");
        }
    }

    /// <summary>Fetch the page a conference-recordings list response links to next, following
    /// its <see cref="Page.NextPageUri"/> verbatim. Returns <c>null</c> when the page is the
    /// last one.</summary>
    public Task<RecordingList?> NextRecordingsPageAsync(RecordingList page, CancellationToken ct = default)
        => string.IsNullOrEmpty(page.NextPageUri)
            ? Task.FromResult<RecordingList?>(null)
            : Transport.SendPageUriAsync<RecordingList>(page.NextPageUri!, ct);

    /// <summary>Start a recording on the named conference's mixed audio. The new Recording
    /// is returned with <c>source</c> = <c>StartConferenceRecordingAPI</c>,
    /// <c>status</c> = <c>in-progress</c>, and <c>channels</c> = 1 (conference recordings are
    /// always mono).</summary>
    public async Task<Recording> StartRecordingAsync(
        string conferenceSid, StartConferenceRecordingRequest? request = null, CancellationToken ct = default)
    {
        var form = request?.ToForm();
        var result = await Transport.SendAsync<Recording>(
            HttpMethod.Post,
            Path("Conferences", conferenceSid, "Recordings"),
            formBody: form,
            ct: ct).ConfigureAwait(false);
        return result ?? throw new ApiException("empty body on POST /Conferences/{sid}/Recordings", 200);
    }

    /// <summary>Fetch metadata for a conference-scoped recording.</summary>
    public async Task<Recording> GetRecordingAsync(
        string conferenceSid, string recordingSid, CancellationToken ct = default)
    {
        var result = await Transport.SendAsync<Recording>(
            HttpMethod.Get,
            Path("Conferences", conferenceSid, "Recordings", recordingSid),
            ct: ct).ConfigureAwait(false);
        return result ?? throw new ApiException("empty body on GET /Conferences/{sid}/Recordings/{rsid}", 200);
    }

    /// <summary>Update a conference-scoped recording.</summary>
    public async Task<Recording> UpdateRecordingAsync(
        string conferenceSid, string recordingSid, UpdateConferenceRecordingRequest request,
        CancellationToken ct = default)
    {
        var result = await Transport.SendAsync<Recording>(
            HttpMethod.Post,
            Path("Conferences", conferenceSid, "Recordings", recordingSid),
            formBody: request.ToForm(),
            ct: ct).ConfigureAwait(false);
        return result ?? throw new ApiException("empty body on POST /Conferences/{sid}/Recordings/{rsid}", 200);
    }

    /// <summary>Delete a conference-scoped recording.</summary>
    public Task DeleteRecordingAsync(
        string conferenceSid, string recordingSid, CancellationToken ct = default)
        => Transport.SendNoContentAsync(
            HttpMethod.Delete,
            Path("Conferences", conferenceSid, "Recordings", recordingSid),
            ct: ct);
}
