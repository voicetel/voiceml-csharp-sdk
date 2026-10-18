using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using VoiceML.Exceptions;
using VoiceML.Models;

namespace VoiceML.Resources;

/// <summary>Daily event-log archive operations
/// (<c>/2010-04-01/Accounts/{AccountSid}/Exports</c>). VoiceML extension — nightly archives
/// of calls, messages, recordings metadata and payments, retained for one year.</summary>
public sealed class ExportsResource : ResourceBase
{
    internal ExportsResource(Transport transport) : base(transport) { }

    /// <summary>List the account's available archives, newest first.</summary>
    public async Task<ExportList> ListAsync(CancellationToken ct = default)
    {
        var result = await Transport.SendAsync<ExportList>(
            HttpMethod.Get,
            Path("Exports"),
            ct: ct).ConfigureAwait(false);
        return result ?? new ExportList();
    }

    /// <summary>Download one day's archive. The endpoint answers with a 302 redirect to a
    /// time-limited presigned URL; the SDK follows it (dropping credentials on the cross-host
    /// hop and refusing an https → http downgrade) and returns the bytes.</summary>
    /// <param name="date">Archive day, <c>YYYY-MM-DD</c>.</param>
    /// <param name="ct">Cancellation token.</param>
    public async Task<ExportArchive> DownloadAsync(string date, CancellationToken ct = default)
    {
        var (_, bytes, _, contentHeaders, viaRedirect) = await Transport.FetchBytesAsync(
            PathNoSuffix("Exports", date), ct: ct).ConfigureAwait(false);
        var contentType = "application/octet-stream";
        if (contentHeaders.ContentType is { } ct2)
        {
            contentType = ct2.MediaType ?? contentType;
        }
        return new ExportArchive
        {
            Date = date,
            Content = bytes,
            ContentType = contentType,
            ViaRedirect = viaRedirect,
        };
    }
}
