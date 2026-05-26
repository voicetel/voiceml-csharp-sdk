using System.Collections.Generic;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using VoiceML.Exceptions;
using VoiceML.Models;

namespace VoiceML.Resources;

/// <summary>Operations on <c>/Calls</c> and call-scoped sub-resources (Recordings, Streams,
/// Siprec, Transcriptions, Notifications, Events, UserDefinedMessages).</summary>
public sealed class CallsResource : ResourceBase
{
    /// <summary>Construct with the shared transport.</summary>
    public CallsResource(Transport transport) : base(transport) { }

    // -------------------------------------------------------------------
    // /Calls
    // -------------------------------------------------------------------

    /// <summary>List calls. Pass an empty <see cref="ListCallsParams"/> for an unfiltered list.</summary>
    public async Task<CallList> ListAsync(ListCallsParams? filter = null, CancellationToken ct = default)
    {
        var p = filter ?? new ListCallsParams();
        var result = await Transport.SendAsync<CallList>(
            HttpMethod.Get,
            Path("Calls"),
            queryParams: p.ToQuery(),
            ct: ct).ConfigureAwait(false);
        return result ?? new CallList();
    }

    /// <summary>Iterate through all calls across pages, yielding one <see cref="Call"/> at a time.
    /// Pass an empty <see cref="ListCallsParams"/> for an unfiltered iteration.</summary>
    public async IAsyncEnumerable<Call> IterateAsync(
        ListCallsParams? filter = null,
        int page = 0,
        int? pageSize = null,
        [EnumeratorCancellation] CancellationToken ct = default)
    {
        var p = filter ?? new ListCallsParams();
        while (true)
        {
            var chunk = await ListAsync(
                p with { Page = page, PageSize = pageSize ?? p.PageSize },
                ct).ConfigureAwait(false);
            foreach (var item in chunk.Calls) yield return item;
            if (string.IsNullOrEmpty(chunk.NextPageUri) || chunk.Calls.Count == 0) yield break;
            page++;
        }
    }

    /// <summary>Originate a new call.</summary>
    public async Task<Call> CreateAsync(CreateCallRequest request, CancellationToken ct = default)
    {
        var result = await Transport.SendAsync<Call>(
            HttpMethod.Post,
            Path("Calls"),
            formBody: request.ToForm(),
            ct: ct).ConfigureAwait(false);
        return result ?? throw new ApiException("empty body on POST /Calls", 200);
    }

    /// <summary>Fetch a single call by SID.</summary>
    public async Task<Call> GetAsync(string callSid, CancellationToken ct = default)
    {
        var result = await Transport.SendAsync<Call>(
            HttpMethod.Get,
            Path("Calls", callSid),
            ct: ct).ConfigureAwait(false);
        return result ?? throw new ApiException("empty body on GET /Calls/{sid}", 200);
    }

    /// <summary>Update / control a live call (terminate, redirect, run inline TwiML).</summary>
    public async Task<Call> UpdateAsync(string callSid, UpdateCallRequest request, CancellationToken ct = default)
    {
        var result = await Transport.SendAsync<Call>(
            HttpMethod.Post,
            Path("Calls", callSid),
            formBody: request.ToForm(),
            ct: ct).ConfigureAwait(false);
        return result ?? throw new ApiException("empty body on POST /Calls/{sid}", 200);
    }

    /// <summary>Delete a call record.</summary>
    public Task DeleteAsync(string callSid, CancellationToken ct = default)
        => Transport.SendNoContentAsync(HttpMethod.Delete, Path("Calls", callSid), ct: ct);

    // -------------------------------------------------------------------
    // /Calls/{sid}/Recordings
    // -------------------------------------------------------------------

    /// <summary>List recordings for a call.</summary>
    public async Task<RecordingList> ListRecordingsAsync(
        string callSid, ListRecordingsParams? filter = null, CancellationToken ct = default)
    {
        var p = filter ?? new ListRecordingsParams();
        var result = await Transport.SendAsync<RecordingList>(
            HttpMethod.Get,
            Path("Calls", callSid, "Recordings"),
            queryParams: p.ToQuery(),
            ct: ct).ConfigureAwait(false);
        return result ?? new RecordingList();
    }

    /// <summary>Iterate through all recordings for a call across pages, yielding one
    /// <see cref="Recording"/> at a time.</summary>
    public async IAsyncEnumerable<Recording> IterateRecordingsAsync(
        string callSid,
        ListRecordingsParams? filter = null,
        int page = 0,
        int? pageSize = null,
        [EnumeratorCancellation] CancellationToken ct = default)
    {
        var p = filter ?? new ListRecordingsParams();
        while (true)
        {
            var chunk = await ListRecordingsAsync(
                callSid,
                p with { Page = page, PageSize = pageSize ?? p.PageSize },
                ct).ConfigureAwait(false);
            foreach (var item in chunk.Recordings) yield return item;
            if (string.IsNullOrEmpty(chunk.NextPageUri) || chunk.Recordings.Count == 0) yield break;
            page++;
        }
    }

    /// <summary>Start a recording on a live call.</summary>
    public async Task<Recording> StartRecordingAsync(string callSid, StartRecordingRequest? request = null, CancellationToken ct = default)
    {
        IEnumerable<KeyValuePair<string, string?>>? form = request?.ToForm();
        var result = await Transport.SendAsync<Recording>(
            HttpMethod.Post,
            Path("Calls", callSid, "Recordings"),
            formBody: form,
            ct: ct).ConfigureAwait(false);
        return result ?? throw new ApiException("empty body on POST /Calls/{sid}/Recordings", 200);
    }

    /// <summary>Fetch a single recording by SID.</summary>
    public async Task<Recording> GetRecordingAsync(string callSid, string recordingSid, CancellationToken ct = default)
    {
        var result = await Transport.SendAsync<Recording>(
            HttpMethod.Get,
            Path("Calls", callSid, "Recordings", recordingSid),
            ct: ct).ConfigureAwait(false);
        return result ?? throw new ApiException("empty body on GET /Calls/{sid}/Recordings/{rsid}", 200);
    }

    /// <summary>Update a recording's state (stop / pause / resume).</summary>
    public async Task<Recording> UpdateRecordingAsync(
        string callSid, string recordingSid, UpdateRecordingRequest request, CancellationToken ct = default)
    {
        var result = await Transport.SendAsync<Recording>(
            HttpMethod.Post,
            Path("Calls", callSid, "Recordings", recordingSid),
            formBody: request.ToForm(),
            ct: ct).ConfigureAwait(false);
        return result ?? throw new ApiException("empty body on POST /Calls/{sid}/Recordings/{rsid}", 200);
    }

    /// <summary>Delete a recording.</summary>
    public Task DeleteRecordingAsync(string callSid, string recordingSid, CancellationToken ct = default)
        => Transport.SendNoContentAsync(HttpMethod.Delete, Path("Calls", callSid, "Recordings", recordingSid), ct: ct);

    // -------------------------------------------------------------------
    // /Calls/{sid}/Streams
    // -------------------------------------------------------------------

    /// <summary>List media streams for a call.</summary>
    public async Task<StreamList> ListStreamsAsync(string callSid, CancellationToken ct = default)
    {
        var result = await Transport.SendAsync<StreamList>(
            HttpMethod.Get,
            Path("Calls", callSid, "Streams"),
            ct: ct).ConfigureAwait(false);
        return result ?? new StreamList();
    }

    /// <summary>Start a media stream on a live call.</summary>
    public async Task<Stream> StartStreamAsync(string callSid, StartStreamRequest request, CancellationToken ct = default)
    {
        var result = await Transport.SendAsync<Stream>(
            HttpMethod.Post,
            Path("Calls", callSid, "Streams"),
            formBody: request.ToForm(),
            ct: ct).ConfigureAwait(false);
        return result ?? throw new ApiException("empty body on POST /Calls/{sid}/Streams", 200);
    }

    /// <summary>Fetch a single stream by SID.</summary>
    public async Task<Stream> GetStreamAsync(string callSid, string streamSid, CancellationToken ct = default)
    {
        var result = await Transport.SendAsync<Stream>(
            HttpMethod.Get,
            Path("Calls", callSid, "Streams", streamSid),
            ct: ct).ConfigureAwait(false);
        return result ?? throw new ApiException("empty body on GET /Calls/{sid}/Streams/{ssid}", 200);
    }

    /// <summary>Stop a media stream.</summary>
    public async Task<Stream> StopStreamAsync(
        string callSid, string streamSid, StopStreamRequest? request = null, CancellationToken ct = default)
    {
        var body = request ?? new StopStreamRequest();
        var result = await Transport.SendAsync<Stream>(
            HttpMethod.Post,
            Path("Calls", callSid, "Streams", streamSid),
            formBody: body.ToForm(),
            ct: ct).ConfigureAwait(false);
        return result ?? throw new ApiException("empty body on POST /Calls/{sid}/Streams/{ssid}", 200);
    }

    // -------------------------------------------------------------------
    // /Calls/{sid}/Siprec
    // -------------------------------------------------------------------

    /// <summary>List SIPREC sessions for a call.</summary>
    public async Task<SiprecList> ListSiprecAsync(string callSid, CancellationToken ct = default)
    {
        var result = await Transport.SendAsync<SiprecList>(
            HttpMethod.Get,
            Path("Calls", callSid, "Siprec"),
            ct: ct).ConfigureAwait(false);
        return result ?? new SiprecList();
    }

    /// <summary>Start a SIPREC session on a live call.</summary>
    public async Task<SiprecSession> StartSiprecAsync(
        string callSid, StartSiprecRequest? request = null, CancellationToken ct = default)
    {
        IEnumerable<KeyValuePair<string, string?>>? form = request?.ToForm();
        var result = await Transport.SendAsync<SiprecSession>(
            HttpMethod.Post,
            Path("Calls", callSid, "Siprec"),
            formBody: form,
            ct: ct).ConfigureAwait(false);
        return result ?? throw new ApiException("empty body on POST /Calls/{sid}/Siprec", 200);
    }

    /// <summary>Fetch a single SIPREC session by SID.</summary>
    public async Task<SiprecSession> GetSiprecAsync(string callSid, string siprecSid, CancellationToken ct = default)
    {
        var result = await Transport.SendAsync<SiprecSession>(
            HttpMethod.Get,
            Path("Calls", callSid, "Siprec", siprecSid),
            ct: ct).ConfigureAwait(false);
        return result ?? throw new ApiException("empty body on GET /Calls/{sid}/Siprec/{ssid}", 200);
    }

    /// <summary>Stop a SIPREC session.</summary>
    public async Task<SiprecSession> StopSiprecAsync(
        string callSid, string siprecSid, StopSiprecRequest? request = null, CancellationToken ct = default)
    {
        var body = request ?? new StopSiprecRequest();
        var result = await Transport.SendAsync<SiprecSession>(
            HttpMethod.Post,
            Path("Calls", callSid, "Siprec", siprecSid),
            formBody: body.ToForm(),
            ct: ct).ConfigureAwait(false);
        return result ?? throw new ApiException("empty body on POST /Calls/{sid}/Siprec/{ssid}", 200);
    }

    // -------------------------------------------------------------------
    // /Calls/{sid}/Transcriptions
    // -------------------------------------------------------------------

    /// <summary>List transcriptions for a call.</summary>
    public async Task<TranscriptionList> ListTranscriptionsAsync(string callSid, CancellationToken ct = default)
    {
        var result = await Transport.SendAsync<TranscriptionList>(
            HttpMethod.Get,
            Path("Calls", callSid, "Transcriptions"),
            ct: ct).ConfigureAwait(false);
        return result ?? new TranscriptionList();
    }

    /// <summary>Start a transcription on a live call.</summary>
    public async Task<CallTranscription> StartTranscriptionAsync(
        string callSid, StartTranscriptionRequest? request = null, CancellationToken ct = default)
    {
        IEnumerable<KeyValuePair<string, string?>>? form = request?.ToForm();
        var result = await Transport.SendAsync<CallTranscription>(
            HttpMethod.Post,
            Path("Calls", callSid, "Transcriptions"),
            formBody: form,
            ct: ct).ConfigureAwait(false);
        return result ?? throw new ApiException("empty body on POST /Calls/{sid}/Transcriptions", 200);
    }

    /// <summary>Fetch a single transcription by SID.</summary>
    public async Task<CallTranscription> GetTranscriptionAsync(
        string callSid, string transcriptionSid, CancellationToken ct = default)
    {
        var result = await Transport.SendAsync<CallTranscription>(
            HttpMethod.Get,
            Path("Calls", callSid, "Transcriptions", transcriptionSid),
            ct: ct).ConfigureAwait(false);
        return result ?? throw new ApiException("empty body on GET /Calls/{sid}/Transcriptions/{tsid}", 200);
    }

    /// <summary>Stop a transcription.</summary>
    public async Task<CallTranscription> StopTranscriptionAsync(
        string callSid, string transcriptionSid, StopTranscriptionRequest? request = null,
        CancellationToken ct = default)
    {
        var body = request ?? new StopTranscriptionRequest();
        var result = await Transport.SendAsync<CallTranscription>(
            HttpMethod.Post,
            Path("Calls", callSid, "Transcriptions", transcriptionSid),
            formBody: body.ToForm(),
            ct: ct).ConfigureAwait(false);
        return result ?? throw new ApiException("empty body on POST /Calls/{sid}/Transcriptions/{tsid}", 200);
    }

    // -------------------------------------------------------------------
    // /Calls/{sid}/Notifications, /Events — compat stubs (always empty)
    // -------------------------------------------------------------------

    /// <summary>List notifications for a call. Server always returns an empty page (compat stub).</summary>
    public async Task<NotificationsList> ListNotificationsAsync(
        string callSid, ListNotificationsParams? filter = null, CancellationToken ct = default)
    {
        var p = filter ?? new ListNotificationsParams();
        var result = await Transport.SendAsync<NotificationsList>(
            HttpMethod.Get,
            Path("Calls", callSid, "Notifications"),
            queryParams: p.ToQuery(),
            ct: ct).ConfigureAwait(false);
        return result ?? new NotificationsList();
    }

    /// <summary>Fetch a per-call notification. Always 404 today (compat stub).</summary>
    public async Task<Dictionary<string, object?>> GetNotificationAsync(
        string callSid, string notificationSid, CancellationToken ct = default)
    {
        var result = await Transport.SendAsync<Dictionary<string, object?>>(
            HttpMethod.Get,
            Path("Calls", callSid, "Notifications", notificationSid),
            ct: ct).ConfigureAwait(false);
        return result ?? throw new ApiException("empty body on GET /Calls/{sid}/Notifications/{nid}", 200);
    }

    /// <summary>List events for a call. Server always returns an empty page (compat stub).
    /// The canonical event source is the customer's StatusCallback URL.</summary>
    public async Task<EventsList> ListEventsAsync(
        string callSid, ListPageParams? filter = null, CancellationToken ct = default)
    {
        var p = filter ?? new ListPageParams();
        var result = await Transport.SendAsync<EventsList>(
            HttpMethod.Get,
            Path("Calls", callSid, "Events"),
            queryParams: p.ToQuery(),
            ct: ct).ConfigureAwait(false);
        return result ?? new EventsList();
    }

    // -------------------------------------------------------------------
    // /Calls/{sid}/UserDefinedMessages — server returns 501 (compat stub)
    // -------------------------------------------------------------------

    /// <summary><c>POST /Calls/{sid}/UserDefinedMessages</c> — always throws
    /// <see cref="NotImplementedAPIException"/>. The endpoint is mounted server-side as a 501
    /// stub; the SDK forwards the call so callers get a clean exception rather than
    /// discovering at runtime that the endpoint doesn't exist.</summary>
    public Task SendUserDefinedMessageAsync(
        string callSid, IEnumerable<KeyValuePair<string, string?>>? payload = null, CancellationToken ct = default)
    {
        return Transport.SendNoContentAsync(
            HttpMethod.Post,
            Path("Calls", callSid, "UserDefinedMessages"),
            formBody: payload,
            ct: ct);
    }
}
