using System.Collections.Generic;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using VoiceML.Exceptions;
using VoiceML.Models;

namespace VoiceML.Resources;

/// <summary>Operations on <c>/Calls</c> and call-scoped sub-resources (Recordings, Streams,
/// Siprec, Transcriptions, Notifications, Events, Payments, UserDefinedMessages).</summary>
public sealed class CallsResource : ResourceBase
{
    internal CallsResource(Transport transport) : base(transport) { }

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

    /// <summary>Fetch the page a list response links to next, following its
    /// <see cref="Page.NextPageUri"/> verbatim. Returns <c>null</c> when the page is the last
    /// one.</summary>
    public Task<CallList?> NextPageAsync(CallList page, CancellationToken ct = default)
        => string.IsNullOrEmpty(page.NextPageUri)
            ? Task.FromResult<CallList?>(null)
            : Transport.SendPageUriAsync<CallList>(page.NextPageUri!, ct);

    /// <summary>Iterate through all calls across pages, yielding one <see cref="Call"/> at a
    /// time. Pages are fetched by following <see cref="Page.NextPageUri"/> verbatim; iteration
    /// stops at the last page, and no further requests are made once the consumer stops
    /// enumerating. Pass an empty <see cref="ListCallsParams"/> for an unfiltered iteration.</summary>
    public async IAsyncEnumerable<Call> IterateAsync(
        ListCallsParams? filter = null,
        [EnumeratorCancellation] CancellationToken ct = default)
    {
        var page = await ListAsync(filter, ct).ConfigureAwait(false);
        while (true)
        {
            foreach (var item in page.Calls)
            {
                yield return item;
            }
            if (string.IsNullOrEmpty(page.NextPageUri))
            {
                yield break;
            }
            page = await NextPageAsync(page, ct).ConfigureAwait(false)
                ?? throw new VoiceMLException("empty page while following next_page_uri");
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
        string callSid, ListCallRecordingsParams? filter = null, CancellationToken ct = default)
    {
        var p = filter ?? new ListCallRecordingsParams();
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
        ListCallRecordingsParams? filter = null,
        [EnumeratorCancellation] CancellationToken ct = default)
    {
        var page = await ListRecordingsAsync(callSid, filter, ct).ConfigureAwait(false);
        while (true)
        {
            foreach (var item in page.Recordings)
            {
                yield return item;
            }
            if (string.IsNullOrEmpty(page.NextPageUri))
            {
                yield break;
            }
            page = await Transport.SendPageUriAsync<RecordingList>(page.NextPageUri!, ct).ConfigureAwait(false)
                ?? throw new VoiceMLException("empty page while following next_page_uri");
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
    // /Calls/{sid}/Streams (POST start/stop only — the GET list/fetch
    // endpoints were removed from the API and answer 405/20004)
    // -------------------------------------------------------------------

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
    // /Calls/{sid}/Siprec (POST start/stop only — the GET list/fetch
    // endpoints were removed from the API and answer 405/20004)
    // -------------------------------------------------------------------

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
    // /Calls/{sid}/Transcriptions (POST start/stop only — the GET list/fetch
    // endpoints were removed from the API and answer 405/20004)
    // -------------------------------------------------------------------

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
    public async Task<Notification> GetNotificationAsync(
        string callSid, string notificationSid, CancellationToken ct = default)
    {
        var result = await Transport.SendAsync<Notification>(
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
    // /Calls/{sid}/Payments — REST companion to the <Pay> TwiML verb
    // -------------------------------------------------------------------

    /// <summary>Start a <c>&lt;Pay&gt;</c> session on the live call. Returns 201 with the
    /// freshly-minted <see cref="CallPayment"/>. Returns 403 when the tenant is not
    /// <c>pay_enabled</c> or has no <c>stripe_secret_key</c> configured. The response carries no
    /// <c>status</c> field — track the session through your StatusCallback.</summary>
    public async Task<CallPayment> StartPaymentAsync(
        string callSid, StartPaymentRequest request, CancellationToken ct = default)
    {
        var result = await Transport.SendAsync<CallPayment>(
            HttpMethod.Post,
            Path("Calls", callSid, "Payments"),
            formBody: request.ToForm(),
            ct: ct).ConfigureAwait(false);
        return result ?? throw new ApiException("empty body on POST /Calls/{sid}/Payments", 200);
    }

    /// <summary>Advance or terminate an existing Pay session. <c>Status=complete</c> captures
    /// the collected fields; <c>Status=cancel</c> aborts the session. <c>Capture=…</c> tells
    /// the runtime which input the caller is about to type next.</summary>
    public async Task<CallPayment> UpdatePaymentAsync(
        string callSid, string paymentSid, UpdatePaymentRequest request, CancellationToken ct = default)
    {
        var result = await Transport.SendAsync<CallPayment>(
            HttpMethod.Post,
            Path("Calls", callSid, "Payments", paymentSid),
            formBody: request.ToForm(),
            ct: ct).ConfigureAwait(false);
        return result ?? throw new ApiException("empty body on POST /Calls/{sid}/Payments/{psid}", 200);
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
