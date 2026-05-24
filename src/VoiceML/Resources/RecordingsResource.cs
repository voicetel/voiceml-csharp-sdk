using System.Collections.Generic;
using System.Net.Http;
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
    /// <summary>Construct with the shared transport.</summary>
    public RecordingsResource(Transport transport) : base(transport) { }

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

    /// <summary>Fetch the metadata JSON for a recording.</summary>
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
        var (status, bytes, responseHeaders, contentHeaders) = await Transport.FetchBytesAsync(
            PathNoSuffix("Recordings", recordingSid) + ".wav", ct).ConfigureAwait(false);
        var contentType = "application/octet-stream";
        if (contentHeaders.ContentType is { } ct2)
        {
            contentType = ct2.MediaType ?? contentType;
        }
        // S3-served content reliably has the x-amz-id-2 header; use that as the "we followed a
        // redirect" tell. (HttpClient swallows the original 302, so we can't observe it directly.)
        bool viaRedirect = status == 200 && responseHeaders.Contains("x-amz-id-2");
        return new RecordingAudio
        {
            Sid = recordingSid,
            Content = bytes,
            ContentType = contentType,
            ViaRedirect = viaRedirect,
        };
    }

    /// <summary>Delete a recording.</summary>
    public Task DeleteAsync(string recordingSid, CancellationToken ct = default)
        => Transport.SendNoContentAsync(HttpMethod.Delete, Path("Recordings", recordingSid), ct: ct);
}
