using System.Collections.Generic;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using VoiceML.Exceptions;
using VoiceML.Models;

namespace VoiceML.Resources;

/// <summary>Operations on <c>/OutgoingCallerIds</c> — verified caller-ID numbers that can be
/// used as the <c>From</c> on outbound calls.</summary>
public sealed class OutgoingCallerIdsResource : ResourceBase
{
    internal OutgoingCallerIdsResource(Transport transport) : base(transport) { }

    /// <summary>List verified caller IDs. Pass an empty
    /// <see cref="ListOutgoingCallerIdsParams"/> for an unfiltered list.</summary>
    public async Task<OutgoingCallerIdList> ListAsync(
        ListOutgoingCallerIdsParams? filter = null, CancellationToken ct = default)
    {
        var p = filter ?? new ListOutgoingCallerIdsParams();
        var result = await Transport.SendAsync<OutgoingCallerIdList>(
            HttpMethod.Get,
            Path("OutgoingCallerIds"),
            queryParams: p.ToQuery(),
            ct: ct).ConfigureAwait(false);
        return result ?? new OutgoingCallerIdList();
    }

    /// <summary>Fetch the page a list response links to next, following its
    /// <see cref="Page.NextPageUri"/> verbatim. Returns <c>null</c> when the page is the last
    /// one.</summary>
    public Task<OutgoingCallerIdList?> NextPageAsync(OutgoingCallerIdList page, CancellationToken ct = default)
        => string.IsNullOrEmpty(page.NextPageUri)
            ? Task.FromResult<OutgoingCallerIdList?>(null)
            : Transport.SendPageUriAsync<OutgoingCallerIdList>(page.NextPageUri!, ct);

    /// <summary>Iterate through all verified caller IDs across pages, yielding one
    /// <see cref="OutgoingCallerId"/> at a time.</summary>
    public async IAsyncEnumerable<OutgoingCallerId> IterateAsync(
        ListOutgoingCallerIdsParams? filter = null,
        [EnumeratorCancellation] CancellationToken ct = default)
    {
        var page = await ListAsync(filter, ct).ConfigureAwait(false);
        while (true)
        {
            foreach (var item in page.OutgoingCallerIds)
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

    /// <summary>Start caller-ID verification for a number. The server dials the number and
    /// reads out a validation code; the returned <see cref="ValidationRequest"/> identifies
    /// the validation call so the application can poll its status.</summary>
    public async Task<ValidationRequest> CreateAsync(
        CreateOutgoingCallerIdRequest request, CancellationToken ct = default)
    {
        var result = await Transport.SendAsync<ValidationRequest>(
            HttpMethod.Post,
            Path("OutgoingCallerIds"),
            formBody: request.ToForm(),
            ct: ct).ConfigureAwait(false);
        return result ?? throw new ApiException("empty body on POST /OutgoingCallerIds", 200);
    }

    /// <summary>Fetch a single verified caller ID by SID.</summary>
    public async Task<OutgoingCallerId> GetAsync(string sid, CancellationToken ct = default)
    {
        var result = await Transport.SendAsync<OutgoingCallerId>(
            HttpMethod.Get,
            Path("OutgoingCallerIds", sid),
            ct: ct).ConfigureAwait(false);
        return result ?? throw new ApiException("empty body on GET /OutgoingCallerIds/{sid}", 200);
    }

    /// <summary>Rename a verified caller ID. Only-set-fields-touched.</summary>
    public async Task<OutgoingCallerId> UpdateAsync(
        string sid, UpdateOutgoingCallerIdRequest request, CancellationToken ct = default)
    {
        var result = await Transport.SendAsync<OutgoingCallerId>(
            HttpMethod.Post,
            Path("OutgoingCallerIds", sid),
            formBody: request.ToForm(),
            ct: ct).ConfigureAwait(false);
        return result ?? throw new ApiException("empty body on POST /OutgoingCallerIds/{sid}", 200);
    }

    /// <summary>Delete a verified caller ID. Idempotent.</summary>
    public Task DeleteAsync(string sid, CancellationToken ct = default)
        => Transport.SendNoContentAsync(HttpMethod.Delete, Path("OutgoingCallerIds", sid), ct: ct);
}
