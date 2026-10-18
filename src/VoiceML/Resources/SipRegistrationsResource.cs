using System.Collections.Generic;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using VoiceML.Exceptions;
using VoiceML.Models;

namespace VoiceML.Resources;

/// <summary>SIP registration listing
/// (<c>GET /2010-04-01/Accounts/{AccountSid}/SIP/Registrations.json</c>) — a VoiceML
/// extension listing every AOR registered under the account's SIP Domains, with contacts
/// and, when the endpoint is on a call, the active call. Obtain via
/// <see cref="SipResource.Registrations"/>.</summary>
public sealed class SipRegistrationsResource : ResourceBase
{
    internal SipRegistrationsResource(Transport transport) : base(transport) { }

    /// <summary>List registered SIP endpoints for the account (paged). Pass an empty
    /// <see cref="ListPageParams"/> for an unfiltered list.</summary>
    public async Task<SipRegistrationList> ListAsync(ListPageParams? filter = null, CancellationToken ct = default)
    {
        var p = filter ?? new ListPageParams();
        var result = await Transport.SendAsync<SipRegistrationList>(
            HttpMethod.Get,
            Path("SIP", "Registrations"),
            queryParams: p.ToQuery(),
            ct: ct).ConfigureAwait(false);
        return result ?? new SipRegistrationList();
    }

    /// <summary>Fetch the page a list response links to next, following its
    /// <see cref="Page.NextPageUri"/> verbatim. Returns <c>null</c> when the page is the last
    /// one.</summary>
    public Task<SipRegistrationList?> NextPageAsync(SipRegistrationList page, CancellationToken ct = default)
        => string.IsNullOrEmpty(page.NextPageUri)
            ? Task.FromResult<SipRegistrationList?>(null)
            : Transport.SendPageUriAsync<SipRegistrationList>(page.NextPageUri!, ct);

    /// <summary>Iterate through all SIP registrations across pages, yielding one
    /// <see cref="SipRegistration"/> at a time.</summary>
    public async IAsyncEnumerable<SipRegistration> IterateAsync(
        ListPageParams? filter = null,
        [EnumeratorCancellation] CancellationToken ct = default)
    {
        var page = await ListAsync(filter, ct).ConfigureAwait(false);
        while (true)
        {
            foreach (var item in page.Registrations)
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
}
