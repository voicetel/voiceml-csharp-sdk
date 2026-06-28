using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using VoiceML.Exceptions;
using VoiceML.Models;

namespace VoiceML.Resources;

/// <summary>Top-level <c>/v2/*</c> surface — Twilio routes/v2 Inbound
/// Processing Region API. Sub-resources: <see cref="SipDomains"/>, <see cref="PhoneNumbers"/>.</summary>
public sealed class RoutesV2Resource
{
    public RoutesV2SipDomainsResource SipDomains { get; }
    public RoutesV2PhoneNumbersResource PhoneNumbers { get; }

    public RoutesV2Resource(Transport transport)
    {
        SipDomains = new RoutesV2SipDomainsResource(transport);
        PhoneNumbers = new RoutesV2PhoneNumbersResource(transport);
    }
}

/// <summary>Operations on <c>/v2/PhoneNumbers/{PhoneNumber}</c>. Keyed by
/// E.164 phone number (or PN sid); account resolved from HTTP Basic auth.
/// Bypasses the <c>/2010-04-01/Accounts/{Sid}/</c> prefix like its sibling
/// <see cref="RoutesV2SipDomainsResource"/>.</summary>
public sealed class RoutesV2PhoneNumbersResource
{
    private readonly Transport _transport;

    public RoutesV2PhoneNumbersResource(Transport transport)
    {
        _transport = transport;
    }

    /// <summary>Fetch a phone number's Inbound Processing Region binding.</summary>
    public async Task<RoutesV2PhoneNumber> GetAsync(string phoneNumber, CancellationToken ct = default)
    {
        var r = await _transport.SendAsync<RoutesV2PhoneNumber>(HttpMethod.Get,
            $"/v2/PhoneNumbers/{phoneNumber}", ct: ct).ConfigureAwait(false);
        return r ?? throw new ApiException("empty body on GET /v2/PhoneNumbers/{number}", 200);
    }

    /// <summary>Update a phone number's voice region and/or friendly name.</summary>
    public async Task<RoutesV2PhoneNumber> UpdateAsync(string phoneNumber, UpdateRoutesV2PhoneNumberRequest request, CancellationToken ct = default)
    {
        var r = await _transport.SendAsync<RoutesV2PhoneNumber>(HttpMethod.Post,
            $"/v2/PhoneNumbers/{phoneNumber}", formBody: request.ToForm(), ct: ct).ConfigureAwait(false);
        return r ?? throw new ApiException("empty body on POST /v2/PhoneNumbers/{number}", 200);
    }
}

/// <summary>Operations on <c>/v2/SipDomains/{SipDomain}</c>. Keyed by domain
/// name; the account is resolved from HTTP Basic auth. The <c>/v2/</c>
/// namespace bypasses the <c>/2010-04-01/Accounts/{Sid}/</c> prefix used by
/// the rest of the SDK.</summary>
public sealed class RoutesV2SipDomainsResource
{
    private readonly Transport _transport;

    public RoutesV2SipDomainsResource(Transport transport)
    {
        _transport = transport;
    }

    /// <summary>Fetch a domain's Inbound Processing Region binding.</summary>
    public async Task<RoutesV2SipDomain> GetAsync(string domainName, CancellationToken ct = default)
    {
        var r = await _transport.SendAsync<RoutesV2SipDomain>(HttpMethod.Get,
            $"/v2/SipDomains/{domainName}", ct: ct).ConfigureAwait(false);
        return r ?? throw new ApiException("empty body on GET /v2/SipDomains/{name}", 200);
    }

    /// <summary>Update a domain's voice region and/or friendly name.</summary>
    public async Task<RoutesV2SipDomain> UpdateAsync(string domainName, UpdateRoutesV2SipDomainRequest request, CancellationToken ct = default)
    {
        var r = await _transport.SendAsync<RoutesV2SipDomain>(HttpMethod.Post,
            $"/v2/SipDomains/{domainName}", formBody: request.ToForm(), ct: ct).ConfigureAwait(false);
        return r ?? throw new ApiException("empty body on POST /v2/SipDomains/{name}", 200);
    }
}
