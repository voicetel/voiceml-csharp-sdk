using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using VoiceML.Exceptions;
using VoiceML.Models;

namespace VoiceML.Resources;

/// <summary>Top-level <c>/SIP/*</c> surface — SIP Trunking. Three sub-resources:
/// <see cref="Domains"/>, <see cref="CredentialLists"/>, <see cref="IpAccessControlLists"/>.</summary>
public sealed class SipResource
{
    public SipDomainsResource Domains { get; }
    public SipCredentialListsResource CredentialLists { get; }
    public SipIpAccessControlListsResource IpAccessControlLists { get; }

    public SipResource(Transport transport)
    {
        Domains = new SipDomainsResource(transport);
        CredentialLists = new SipCredentialListsResource(transport);
        IpAccessControlLists = new SipIpAccessControlListsResource(transport);
    }
}

/// <summary>Operations on <c>/SIP/Domains</c> plus the four mapping endpoints attached to
/// a SipDomain (historical aliases + Auth/Calls + Auth/Registrations).</summary>
public sealed class SipDomainsResource : ResourceBase
{
    public SipDomainsResource(Transport transport) : base(transport) { }

    // --- /SIP/Domains CRUD --------------------------------------------------

    public async Task<SipDomainList> ListAsync(ListSipPageParams? filter = null, CancellationToken ct = default)
    {
        var p = filter ?? new ListSipPageParams();
        return await Transport.SendAsync<SipDomainList>(HttpMethod.Get,
            Path("SIP", "Domains"), queryParams: p.ToQuery(), ct: ct).ConfigureAwait(false)
            ?? new SipDomainList();
    }

    public async Task<SipDomain> CreateAsync(CreateSipDomainRequest request, CancellationToken ct = default)
    {
        var r = await Transport.SendAsync<SipDomain>(HttpMethod.Post,
            Path("SIP", "Domains"), formBody: request.ToForm(), ct: ct).ConfigureAwait(false);
        return r ?? throw new ApiException("empty body on POST /SIP/Domains", 200);
    }

    public async Task<SipDomain> GetAsync(string domainSid, CancellationToken ct = default)
    {
        var r = await Transport.SendAsync<SipDomain>(HttpMethod.Get,
            Path("SIP", "Domains", domainSid), ct: ct).ConfigureAwait(false);
        return r ?? throw new ApiException("empty body on GET /SIP/Domains/{sid}", 200);
    }

    public async Task<SipDomain> UpdateAsync(string domainSid, UpdateSipDomainRequest request, CancellationToken ct = default)
    {
        var r = await Transport.SendAsync<SipDomain>(HttpMethod.Post,
            Path("SIP", "Domains", domainSid), formBody: request.ToForm(), ct: ct).ConfigureAwait(false);
        return r ?? throw new ApiException("empty body on POST /SIP/Domains/{sid}", 200);
    }

    public Task DeleteAsync(string domainSid, CancellationToken ct = default) =>
        Transport.SendAsync<object>(HttpMethod.Delete, Path("SIP", "Domains", domainSid), ct: ct);

    // --- Historical CredentialList mappings ---------------------------------

    public async Task<SipCredentialListMappingList> ListCredentialListMappingsAsync(string domainSid, ListSipPageParams? filter = null, CancellationToken ct = default)
    {
        var p = filter ?? new ListSipPageParams();
        return await Transport.SendAsync<SipCredentialListMappingList>(HttpMethod.Get,
            Path("SIP", "Domains", domainSid, "CredentialListMappings"), queryParams: p.ToQuery(), ct: ct).ConfigureAwait(false)
            ?? new SipCredentialListMappingList();
    }

    public async Task<SipDomainMapping> CreateCredentialListMappingAsync(string domainSid, CreateSipCredentialListMappingRequest request, CancellationToken ct = default)
    {
        var r = await Transport.SendAsync<SipDomainMapping>(HttpMethod.Post,
            Path("SIP", "Domains", domainSid, "CredentialListMappings"), formBody: request.ToForm(), ct: ct).ConfigureAwait(false);
        return r ?? throw new ApiException("empty mapping POST", 200);
    }

    public async Task<SipDomainMapping> GetCredentialListMappingAsync(string domainSid, string mappingSid, CancellationToken ct = default)
    {
        var r = await Transport.SendAsync<SipDomainMapping>(HttpMethod.Get,
            Path("SIP", "Domains", domainSid, "CredentialListMappings", mappingSid), ct: ct).ConfigureAwait(false);
        return r ?? throw new ApiException("empty mapping GET", 200);
    }

    public Task DeleteCredentialListMappingAsync(string domainSid, string mappingSid, CancellationToken ct = default) =>
        Transport.SendAsync<object>(HttpMethod.Delete, Path("SIP", "Domains", domainSid, "CredentialListMappings", mappingSid), ct: ct);

    // --- Historical IpAccessControlList mappings ---------------------------

    public async Task<SipIpAccessControlListMappingList> ListIpAccessControlListMappingsAsync(string domainSid, ListSipPageParams? filter = null, CancellationToken ct = default)
    {
        var p = filter ?? new ListSipPageParams();
        return await Transport.SendAsync<SipIpAccessControlListMappingList>(HttpMethod.Get,
            Path("SIP", "Domains", domainSid, "IpAccessControlListMappings"), queryParams: p.ToQuery(), ct: ct).ConfigureAwait(false)
            ?? new SipIpAccessControlListMappingList();
    }

    public async Task<SipDomainMapping> CreateIpAccessControlListMappingAsync(string domainSid, CreateSipIpAccessControlListMappingRequest request, CancellationToken ct = default)
    {
        var r = await Transport.SendAsync<SipDomainMapping>(HttpMethod.Post,
            Path("SIP", "Domains", domainSid, "IpAccessControlListMappings"), formBody: request.ToForm(), ct: ct).ConfigureAwait(false);
        return r ?? throw new ApiException("empty mapping POST", 200);
    }

    public async Task<SipDomainMapping> GetIpAccessControlListMappingAsync(string domainSid, string mappingSid, CancellationToken ct = default)
    {
        var r = await Transport.SendAsync<SipDomainMapping>(HttpMethod.Get,
            Path("SIP", "Domains", domainSid, "IpAccessControlListMappings", mappingSid), ct: ct).ConfigureAwait(false);
        return r ?? throw new ApiException("empty mapping GET", 200);
    }

    public Task DeleteIpAccessControlListMappingAsync(string domainSid, string mappingSid, CancellationToken ct = default) =>
        Transport.SendAsync<object>(HttpMethod.Delete, Path("SIP", "Domains", domainSid, "IpAccessControlListMappings", mappingSid), ct: ct);

    // --- Auth/Calls/CredentialListMappings ----------------------------------

    public async Task<SipCredentialListMappingList> ListAuthCallsCredentialListMappingsAsync(string domainSid, ListSipPageParams? filter = null, CancellationToken ct = default)
    {
        var p = filter ?? new ListSipPageParams();
        return await Transport.SendAsync<SipCredentialListMappingList>(HttpMethod.Get,
            Path("SIP", "Domains", domainSid, "Auth", "Calls", "CredentialListMappings"), queryParams: p.ToQuery(), ct: ct).ConfigureAwait(false)
            ?? new SipCredentialListMappingList();
    }

    public async Task<SipDomainMapping> CreateAuthCallsCredentialListMappingAsync(string domainSid, CreateSipCredentialListMappingRequest request, CancellationToken ct = default)
    {
        var r = await Transport.SendAsync<SipDomainMapping>(HttpMethod.Post,
            Path("SIP", "Domains", domainSid, "Auth", "Calls", "CredentialListMappings"), formBody: request.ToForm(), ct: ct).ConfigureAwait(false);
        return r ?? throw new ApiException("empty mapping POST", 200);
    }

    public async Task<SipDomainMapping> GetAuthCallsCredentialListMappingAsync(string domainSid, string mappingSid, CancellationToken ct = default)
    {
        var r = await Transport.SendAsync<SipDomainMapping>(HttpMethod.Get,
            Path("SIP", "Domains", domainSid, "Auth", "Calls", "CredentialListMappings", mappingSid), ct: ct).ConfigureAwait(false);
        return r ?? throw new ApiException("empty mapping GET", 200);
    }

    public Task DeleteAuthCallsCredentialListMappingAsync(string domainSid, string mappingSid, CancellationToken ct = default) =>
        Transport.SendAsync<object>(HttpMethod.Delete, Path("SIP", "Domains", domainSid, "Auth", "Calls", "CredentialListMappings", mappingSid), ct: ct);

    // --- Auth/Calls/IpAccessControlListMappings ----------------------------

    public async Task<SipIpAccessControlListMappingList> ListAuthCallsIpAccessControlListMappingsAsync(string domainSid, ListSipPageParams? filter = null, CancellationToken ct = default)
    {
        var p = filter ?? new ListSipPageParams();
        return await Transport.SendAsync<SipIpAccessControlListMappingList>(HttpMethod.Get,
            Path("SIP", "Domains", domainSid, "Auth", "Calls", "IpAccessControlListMappings"), queryParams: p.ToQuery(), ct: ct).ConfigureAwait(false)
            ?? new SipIpAccessControlListMappingList();
    }

    public async Task<SipDomainMapping> CreateAuthCallsIpAccessControlListMappingAsync(string domainSid, CreateSipIpAccessControlListMappingRequest request, CancellationToken ct = default)
    {
        var r = await Transport.SendAsync<SipDomainMapping>(HttpMethod.Post,
            Path("SIP", "Domains", domainSid, "Auth", "Calls", "IpAccessControlListMappings"), formBody: request.ToForm(), ct: ct).ConfigureAwait(false);
        return r ?? throw new ApiException("empty mapping POST", 200);
    }

    public async Task<SipDomainMapping> GetAuthCallsIpAccessControlListMappingAsync(string domainSid, string mappingSid, CancellationToken ct = default)
    {
        var r = await Transport.SendAsync<SipDomainMapping>(HttpMethod.Get,
            Path("SIP", "Domains", domainSid, "Auth", "Calls", "IpAccessControlListMappings", mappingSid), ct: ct).ConfigureAwait(false);
        return r ?? throw new ApiException("empty mapping GET", 200);
    }

    public Task DeleteAuthCallsIpAccessControlListMappingAsync(string domainSid, string mappingSid, CancellationToken ct = default) =>
        Transport.SendAsync<object>(HttpMethod.Delete, Path("SIP", "Domains", domainSid, "Auth", "Calls", "IpAccessControlListMappings", mappingSid), ct: ct);

    // --- Auth/Registrations/CredentialListMappings (no IP-ACL counterpart)

    public async Task<SipCredentialListMappingList> ListAuthRegistrationsCredentialListMappingsAsync(string domainSid, ListSipPageParams? filter = null, CancellationToken ct = default)
    {
        var p = filter ?? new ListSipPageParams();
        return await Transport.SendAsync<SipCredentialListMappingList>(HttpMethod.Get,
            Path("SIP", "Domains", domainSid, "Auth", "Registrations", "CredentialListMappings"), queryParams: p.ToQuery(), ct: ct).ConfigureAwait(false)
            ?? new SipCredentialListMappingList();
    }

    public async Task<SipDomainMapping> CreateAuthRegistrationsCredentialListMappingAsync(string domainSid, CreateSipCredentialListMappingRequest request, CancellationToken ct = default)
    {
        var r = await Transport.SendAsync<SipDomainMapping>(HttpMethod.Post,
            Path("SIP", "Domains", domainSid, "Auth", "Registrations", "CredentialListMappings"), formBody: request.ToForm(), ct: ct).ConfigureAwait(false);
        return r ?? throw new ApiException("empty mapping POST", 200);
    }

    public async Task<SipDomainMapping> GetAuthRegistrationsCredentialListMappingAsync(string domainSid, string mappingSid, CancellationToken ct = default)
    {
        var r = await Transport.SendAsync<SipDomainMapping>(HttpMethod.Get,
            Path("SIP", "Domains", domainSid, "Auth", "Registrations", "CredentialListMappings", mappingSid), ct: ct).ConfigureAwait(false);
        return r ?? throw new ApiException("empty mapping GET", 200);
    }

    public Task DeleteAuthRegistrationsCredentialListMappingAsync(string domainSid, string mappingSid, CancellationToken ct = default) =>
        Transport.SendAsync<object>(HttpMethod.Delete, Path("SIP", "Domains", domainSid, "Auth", "Registrations", "CredentialListMappings", mappingSid), ct: ct);
}

/// <summary>Operations on <c>/SIP/CredentialLists</c> plus per-list /Credentials sub-resource.</summary>
public sealed class SipCredentialListsResource : ResourceBase
{
    public SipCredentialListsResource(Transport transport) : base(transport) { }

    public async Task<SipCredentialListList> ListAsync(ListSipPageParams? filter = null, CancellationToken ct = default)
    {
        var p = filter ?? new ListSipPageParams();
        return await Transport.SendAsync<SipCredentialListList>(HttpMethod.Get,
            Path("SIP", "CredentialLists"), queryParams: p.ToQuery(), ct: ct).ConfigureAwait(false)
            ?? new SipCredentialListList();
    }

    public async Task<SipCredentialList> CreateAsync(CreateSipCredentialListRequest request, CancellationToken ct = default)
    {
        var r = await Transport.SendAsync<SipCredentialList>(HttpMethod.Post,
            Path("SIP", "CredentialLists"), formBody: request.ToForm(), ct: ct).ConfigureAwait(false);
        return r ?? throw new ApiException("empty CL POST", 200);
    }

    public async Task<SipCredentialList> GetAsync(string credentialListSid, CancellationToken ct = default)
    {
        var r = await Transport.SendAsync<SipCredentialList>(HttpMethod.Get,
            Path("SIP", "CredentialLists", credentialListSid), ct: ct).ConfigureAwait(false);
        return r ?? throw new ApiException("empty CL GET", 200);
    }

    public async Task<SipCredentialList> UpdateAsync(string credentialListSid, UpdateSipCredentialListRequest request, CancellationToken ct = default)
    {
        var r = await Transport.SendAsync<SipCredentialList>(HttpMethod.Post,
            Path("SIP", "CredentialLists", credentialListSid), formBody: request.ToForm(), ct: ct).ConfigureAwait(false);
        return r ?? throw new ApiException("empty CL POST", 200);
    }

    public Task DeleteAsync(string credentialListSid, CancellationToken ct = default) =>
        Transport.SendAsync<object>(HttpMethod.Delete, Path("SIP", "CredentialLists", credentialListSid), ct: ct);

    // --- /Credentials sub-resource -----------------------------------------

    public async Task<SipCredentialListPage> ListCredentialsAsync(string credentialListSid, ListSipPageParams? filter = null, CancellationToken ct = default)
    {
        var p = filter ?? new ListSipPageParams();
        return await Transport.SendAsync<SipCredentialListPage>(HttpMethod.Get,
            Path("SIP", "CredentialLists", credentialListSid, "Credentials"), queryParams: p.ToQuery(), ct: ct).ConfigureAwait(false)
            ?? new SipCredentialListPage();
    }

    public async Task<SipCredential> CreateCredentialAsync(string credentialListSid, CreateSipCredentialRequest request, CancellationToken ct = default)
    {
        var r = await Transport.SendAsync<SipCredential>(HttpMethod.Post,
            Path("SIP", "CredentialLists", credentialListSid, "Credentials"), formBody: request.ToForm(), ct: ct).ConfigureAwait(false);
        return r ?? throw new ApiException("empty CR POST", 200);
    }

    public async Task<SipCredential> GetCredentialAsync(string credentialListSid, string credentialSid, CancellationToken ct = default)
    {
        var r = await Transport.SendAsync<SipCredential>(HttpMethod.Get,
            Path("SIP", "CredentialLists", credentialListSid, "Credentials", credentialSid), ct: ct).ConfigureAwait(false);
        return r ?? throw new ApiException("empty CR GET", 200);
    }

    public async Task<SipCredential> UpdateCredentialAsync(string credentialListSid, string credentialSid, UpdateSipCredentialRequest request, CancellationToken ct = default)
    {
        var r = await Transport.SendAsync<SipCredential>(HttpMethod.Post,
            Path("SIP", "CredentialLists", credentialListSid, "Credentials", credentialSid), formBody: request.ToForm(), ct: ct).ConfigureAwait(false);
        return r ?? throw new ApiException("empty CR POST", 200);
    }

    public Task DeleteCredentialAsync(string credentialListSid, string credentialSid, CancellationToken ct = default) =>
        Transport.SendAsync<object>(HttpMethod.Delete, Path("SIP", "CredentialLists", credentialListSid, "Credentials", credentialSid), ct: ct);
}

/// <summary>Operations on <c>/SIP/IpAccessControlLists</c> plus per-list /IpAddresses sub-resource.</summary>
public sealed class SipIpAccessControlListsResource : ResourceBase
{
    public SipIpAccessControlListsResource(Transport transport) : base(transport) { }

    public async Task<SipIpAccessControlListList> ListAsync(ListSipPageParams? filter = null, CancellationToken ct = default)
    {
        var p = filter ?? new ListSipPageParams();
        return await Transport.SendAsync<SipIpAccessControlListList>(HttpMethod.Get,
            Path("SIP", "IpAccessControlLists"), queryParams: p.ToQuery(), ct: ct).ConfigureAwait(false)
            ?? new SipIpAccessControlListList();
    }

    public async Task<SipIpAccessControlList> CreateAsync(CreateSipIpAccessControlListRequest request, CancellationToken ct = default)
    {
        var r = await Transport.SendAsync<SipIpAccessControlList>(HttpMethod.Post,
            Path("SIP", "IpAccessControlLists"), formBody: request.ToForm(), ct: ct).ConfigureAwait(false);
        return r ?? throw new ApiException("empty ACL POST", 200);
    }

    public async Task<SipIpAccessControlList> GetAsync(string aclSid, CancellationToken ct = default)
    {
        var r = await Transport.SendAsync<SipIpAccessControlList>(HttpMethod.Get,
            Path("SIP", "IpAccessControlLists", aclSid), ct: ct).ConfigureAwait(false);
        return r ?? throw new ApiException("empty ACL GET", 200);
    }

    public async Task<SipIpAccessControlList> UpdateAsync(string aclSid, UpdateSipIpAccessControlListRequest request, CancellationToken ct = default)
    {
        var r = await Transport.SendAsync<SipIpAccessControlList>(HttpMethod.Post,
            Path("SIP", "IpAccessControlLists", aclSid), formBody: request.ToForm(), ct: ct).ConfigureAwait(false);
        return r ?? throw new ApiException("empty ACL POST", 200);
    }

    public Task DeleteAsync(string aclSid, CancellationToken ct = default) =>
        Transport.SendAsync<object>(HttpMethod.Delete, Path("SIP", "IpAccessControlLists", aclSid), ct: ct);

    // --- /IpAddresses sub-resource ------------------------------------------

    public async Task<SipIpAddressList> ListIpAddressesAsync(string aclSid, ListSipPageParams? filter = null, CancellationToken ct = default)
    {
        var p = filter ?? new ListSipPageParams();
        return await Transport.SendAsync<SipIpAddressList>(HttpMethod.Get,
            Path("SIP", "IpAccessControlLists", aclSid, "IpAddresses"), queryParams: p.ToQuery(), ct: ct).ConfigureAwait(false)
            ?? new SipIpAddressList();
    }

    public async Task<SipIpAddress> CreateIpAddressAsync(string aclSid, CreateSipIpAddressRequest request, CancellationToken ct = default)
    {
        var r = await Transport.SendAsync<SipIpAddress>(HttpMethod.Post,
            Path("SIP", "IpAccessControlLists", aclSid, "IpAddresses"), formBody: request.ToForm(), ct: ct).ConfigureAwait(false);
        return r ?? throw new ApiException("empty IP POST", 200);
    }

    public async Task<SipIpAddress> GetIpAddressAsync(string aclSid, string ipAddressSid, CancellationToken ct = default)
    {
        var r = await Transport.SendAsync<SipIpAddress>(HttpMethod.Get,
            Path("SIP", "IpAccessControlLists", aclSid, "IpAddresses", ipAddressSid), ct: ct).ConfigureAwait(false);
        return r ?? throw new ApiException("empty IP GET", 200);
    }

    public async Task<SipIpAddress> UpdateIpAddressAsync(string aclSid, string ipAddressSid, UpdateSipIpAddressRequest request, CancellationToken ct = default)
    {
        var r = await Transport.SendAsync<SipIpAddress>(HttpMethod.Post,
            Path("SIP", "IpAccessControlLists", aclSid, "IpAddresses", ipAddressSid), formBody: request.ToForm(), ct: ct).ConfigureAwait(false);
        return r ?? throw new ApiException("empty IP POST", 200);
    }

    public Task DeleteIpAddressAsync(string aclSid, string ipAddressSid, CancellationToken ct = default) =>
        Transport.SendAsync<object>(HttpMethod.Delete, Path("SIP", "IpAccessControlLists", aclSid, "IpAddresses", ipAddressSid), ct: ct);
}
