using System.Collections.Generic;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using VoiceML.Exceptions;
using VoiceML.Models;

namespace VoiceML.Resources;

/// <summary>Operations on the account's recording-derived transcriptions
/// (<c>/2010-04-01/Accounts/{AccountSid}/Transcriptions.json</c>).</summary>
public sealed class TranscriptionsResource : ResourceBase
{
    internal TranscriptionsResource(Transport transport) : base(transport) { }

    /// <summary>List the account's transcriptions. Pass an empty
    /// <see cref="ListTranscriptionsParams"/> for an unfiltered list.</summary>
    public async Task<RecordingTranscriptionList> ListAsync(
        ListTranscriptionsParams? filter = null, CancellationToken ct = default)
    {
        var p = filter ?? new ListTranscriptionsParams();
        var result = await Transport.SendAsync<RecordingTranscriptionList>(
            HttpMethod.Get,
            Path("Transcriptions"),
            queryParams: p.ToQuery(),
            ct: ct).ConfigureAwait(false);
        return result ?? new RecordingTranscriptionList();
    }

    /// <summary>Fetch the page a list response links to next, following its
    /// <see cref="Page.NextPageUri"/> verbatim. Returns <c>null</c> when the page is the last
    /// one.</summary>
    public Task<RecordingTranscriptionList?> NextPageAsync(RecordingTranscriptionList page, CancellationToken ct = default)
        => string.IsNullOrEmpty(page.NextPageUri)
            ? Task.FromResult<RecordingTranscriptionList?>(null)
            : Transport.SendPageUriAsync<RecordingTranscriptionList>(page.NextPageUri!, ct);

    /// <summary>Iterate through all transcriptions across pages, yielding one
    /// <see cref="RecordingTranscription"/> at a time.</summary>
    public async IAsyncEnumerable<RecordingTranscription> IterateAsync(
        ListTranscriptionsParams? filter = null,
        [EnumeratorCancellation] CancellationToken ct = default)
    {
        var page = await ListAsync(filter, ct).ConfigureAwait(false);
        while (true)
        {
            foreach (var item in page.Transcriptions)
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

    /// <summary>Fetch a single transcription by SID.</summary>
    public async Task<RecordingTranscription> GetAsync(string sid, CancellationToken ct = default)
    {
        var result = await Transport.SendAsync<RecordingTranscription>(
            HttpMethod.Get,
            Path("Transcriptions", sid),
            ct: ct).ConfigureAwait(false);
        return result ?? throw new ApiException("empty body on GET /Transcriptions/{sid}", 200);
    }

    /// <summary>Delete a transcription.</summary>
    public Task DeleteAsync(string sid, CancellationToken ct = default)
        => Transport.SendNoContentAsync(HttpMethod.Delete, Path("Transcriptions", sid), ct: ct);
}

/// <summary>Recording-scoped transcription operations
/// (<c>/Recordings/{RecordingSid}/Transcriptions</c>). Obtain via
/// <see cref="RecordingsResource.Transcriptions"/>.</summary>
public sealed class RecordingTranscriptionsResource : ResourceBase
{
    private readonly string _recordingSid;

    internal RecordingTranscriptionsResource(Transport transport, string recordingSid) : base(transport)
    {
        _recordingSid = recordingSid;
    }

    /// <summary>List the transcriptions derived from this recording.</summary>
    public async Task<RecordingTranscriptionList> ListAsync(
        ListTranscriptionsParams? filter = null, CancellationToken ct = default)
    {
        var p = filter ?? new ListTranscriptionsParams();
        var result = await Transport.SendAsync<RecordingTranscriptionList>(
            HttpMethod.Get,
            Path("Recordings", _recordingSid, "Transcriptions"),
            queryParams: p.ToQuery(),
            ct: ct).ConfigureAwait(false);
        return result ?? new RecordingTranscriptionList();
    }

    /// <summary>Fetch the page a list response links to next, following its
    /// <see cref="Page.NextPageUri"/> verbatim. Returns <c>null</c> when the page is the last
    /// one.</summary>
    public Task<RecordingTranscriptionList?> NextPageAsync(RecordingTranscriptionList page, CancellationToken ct = default)
        => string.IsNullOrEmpty(page.NextPageUri)
            ? Task.FromResult<RecordingTranscriptionList?>(null)
            : Transport.SendPageUriAsync<RecordingTranscriptionList>(page.NextPageUri!, ct);

    /// <summary>Iterate through all transcriptions of this recording across pages, yielding
    /// one <see cref="RecordingTranscription"/> at a time.</summary>
    public async IAsyncEnumerable<RecordingTranscription> IterateAsync(
        ListTranscriptionsParams? filter = null,
        [EnumeratorCancellation] CancellationToken ct = default)
    {
        var page = await ListAsync(filter, ct).ConfigureAwait(false);
        while (true)
        {
            foreach (var item in page.Transcriptions)
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

    /// <summary>Fetch a single transcription of this recording by SID.</summary>
    public async Task<RecordingTranscription> GetAsync(string sid, CancellationToken ct = default)
    {
        var result = await Transport.SendAsync<RecordingTranscription>(
            HttpMethod.Get,
            Path("Recordings", _recordingSid, "Transcriptions", sid),
            ct: ct).ConfigureAwait(false);
        return result ?? throw new ApiException("empty body on GET /Recordings/{rsid}/Transcriptions/{sid}", 200);
    }

    /// <summary>Delete a transcription of this recording.</summary>
    public Task DeleteAsync(string sid, CancellationToken ct = default)
        => Transport.SendNoContentAsync(
            HttpMethod.Delete, Path("Recordings", _recordingSid, "Transcriptions", sid), ct: ct);
}
