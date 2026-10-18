using System.Collections.Generic;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using VoiceML.Exceptions;
using VoiceML.Models;

namespace VoiceML.Resources;

/// <summary>Account-scoped <c>/Recordings</c> operations.
/// <para>Per-call recording start/stop/list lives on <see cref="CallsResource"/> — this
/// resource handles the account-wide list, single-recording fetch (metadata + audio),
/// and delete.</para>
/// </summary>
public sealed class RecordingsResource : ResourceBase
{
    internal RecordingsResource(Transport transport) : base(transport) { }

    /// <summary>List recordings across the entire account.</summary>
    public async Task<RecordingList> ListAsync(ListRecordingsParams? filter = null, CancellationToken ct = default)
    {
        var p = filter ?? new ListRecordingsParams();
        var result = await Transport.SendAsync<RecordingList>(
            HttpMethod.Get,
            Path("Recordings"),
            queryParams: p.ToQuery(),
            ct: ct).ConfigureAwait(false);
        return result ?? new RecordingList();
    }

    /// <summary>Iterate through all recordings across pages, yielding one
    /// <see cref="Recording"/> at a time.</summary>
    public async IAsyncEnumerable<Recording> IterateAsync(
        ListRecordingsParams? filter = null,
        [EnumeratorCancellation] CancellationToken ct = default)
    {
        var page = await ListAsync(filter, ct).ConfigureAwait(false);
        while (true)
        {
            foreach (var item in page.Recordings) yield return item;
            if (string.IsNullOrEmpty(page.NextPageUri)) yield break;
            page = await Transport.SendPageUriAsync<RecordingList>(page.NextPageUri!, ct).ConfigureAwait(false)
                ?? throw new VoiceMLException("empty page while following next_page_uri");
        }
    }

    /// <summary>Fetch the metadata JSON for a recording.</summary>
    /// <summary>Fetch the page a list response links to next, following its
    /// <see cref="Page.NextPageUri"/> verbatim. Returns <c>null</c> when the page is the last
    /// one.</summary>
    public Task<RecordingList?> NextPageAsync(RecordingList page, CancellationToken ct = default)
        => string.IsNullOrEmpty(page.NextPageUri)
            ? Task.FromResult<RecordingList?>(null)
            : Transport.SendPageUriAsync<RecordingList>(page.NextPageUri!, ct);

    /// <summary>Fetch recordings. Route: Recordings, recordingSid.</summary>
    public async Task<Recording> GetAsync(
        string recordingSid, GetRecordingParams? filter = null, CancellationToken ct = default)
    {
        IEnumerable<KeyValuePair<string, string?>>? query = filter?.ToQuery();
        var result = await Transport.SendAsync<Recording>(
            HttpMethod.Get, Path("Recordings", recordingSid), queryParams: query, ct: ct).ConfigureAwait(false);
        return result ?? throw new ApiException("empty body on GET /Recordings/{sid}", 200);
    }

    /// <summary>Fetch the WAV audio for a recording. Three server delivery shapes are
    /// flattened by following any 302 redirect to S3:
    /// <list type="bullet">
    ///   <item><description><c>200 OK</c> — local file present.</description></item>
    ///   <item><description><c>302 Found</c> — archived to S3; the SDK follows the presigned URL.</description></item>
    ///   <item><description><c>410 Gone</c> — local file gone AND no S3 key. Throws <see cref="GoneException"/>.</description></item>
    /// </list></summary>
    public async Task<RecordingAudio> GetAudioAsync(string recordingSid, CancellationToken ct = default)
    {
        var (_, bytes, _, contentHeaders, viaRedirect) = await Transport.FetchBytesAsync(
            PathNoSuffix("Recordings", recordingSid) + ".wav", sendAuth: true, ct: ct).ConfigureAwait(false);
        var contentType = "application/octet-stream";
        if (contentHeaders.ContentType is { } ct2)
        {
            contentType = ct2.MediaType ?? contentType;
        }
        return new RecordingAudio
        {
            Sid = recordingSid,
            Content = bytes,
            ContentType = contentType,
            ViaRedirect = viaRedirect,
        };
    }

    /// <summary>Scope to a recording's <c>/Transcriptions</c> sub-resource — the offline
    /// transcripts derived from the completed recording.</summary>
    public RecordingTranscriptionsResource Transcriptions(string recordingSid)
        => new(Transport, recordingSid);

    /// <summary>Delete a recording.</summary>
    public Task DeleteAsync(string recordingSid, CancellationToken ct = default)
        => Transport.SendNoContentAsync(HttpMethod.Delete, Path("Recordings", recordingSid), ct: ct);
}
