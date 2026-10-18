using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using VoiceML.Exceptions;
using VoiceML.Models;

namespace VoiceML.Resources;

/// <summary>Top-level <c>/SIP/*</c> surface — SIP Trunking. Sub-resources:
/// <see cref="Domains"/>, <see cref="CredentialLists"/>, <see cref="IpAccessControlLists"/>,
/// <see cref="Registrations"/>.</summary>
public sealed class SipResource
{
    /// <summary>Operations on <c>/SIP/Domains</c>.</summary>
    public SipDomainsResource Domains { get; }

    /// <summary>Operations on <c>/SIP/CredentialLists</c>.</summary>
    public SipCredentialListsResource CredentialLists { get; }

    /// <summary>Operations on <c>/SIP/IpAccessControlLists</c>.</summary>
    public SipIpAccessControlListsResource IpAccessControlLists { get; }

    /// <summary>Operations on <c>/SIP/Registrations</c> (VoiceML extension).</summary>
    public SipRegistrationsResource Registrations { get; }

    internal SipResource(Transport transport)
    {
        Domains = new SipDomainsResource(transport);
        CredentialLists = new SipCredentialListsResource(transport);
        IpAccessControlLists = new SipIpAccessControlListsResource(transport);
        Registrations = new SipRegistrationsResource(transport);
    }
}

/// <summary>Operations on <c>/SIP/Domains</c> plus the four mapping endpoints attached to
/// a SipDomain (historical aliases + Auth/Calls + Auth/Registrations).</summary>
public sealed class SipDomainsResource : ResourceBase
{
    internal SipDomainsResource(Transport transport) : base(transport) { }

    // --- /SIP/Domains CRUD --------------------------------------------------

    /// <summary>List sip domains. Route: SIP, Domains.</summary>
    public async Task<SipDomainList> ListAsync(ListSipPageParams? filter = null, CancellationToken ct = default)
    {
        var p = filter ?? new ListSipPageParams();
        return await Transport.SendAsync<SipDomainList>(HttpMethod.Get,
            Path("SIP", "Domains"), queryParams: p.ToQuery(), ct: ct).ConfigureAwait(false)
            ?? new SipDomainList();
    }

    /// <summary>Create sip domains. Route: SIP, Domains.</summary>
    public async Task<SipDomain> CreateAsync(CreateSipDomainRequest request, CancellationToken ct = default)
    {
        var r = await Transport.SendAsync<SipDomain>(HttpMethod.Post,
            Path("SIP", "Domains"), formBody: request.ToForm(), ct: ct).ConfigureAwait(false);
        return r ?? throw new ApiException("empty body on POST /SIP/Domains", 200);
    }

    /// <summary>Fetch sip domains. Route: SIP, Domains, domainSid.</summary>
    public async Task<SipDomain> GetAsync(string domainSid, CancellationToken ct = default)
    {
        var r = await Transport.SendAsync<SipDomain>(HttpMethod.Get,
            Path("SIP", "Domains", domainSid), ct: ct).ConfigureAwait(false);
        return r ?? throw new ApiException("empty body on GET /SIP/Domains/{sid}", 200);
    }

    /// <summary>Update sip domains. Route: SIP, Domains, domainSid.</summary>
    public async Task<SipDomain> UpdateAsync(string domainSid, UpdateSipDomainRequest request, CancellationToken ct = default)
    {
        var r = await Transport.SendAsync<SipDomain>(HttpMethod.Post,
            Path("SIP", "Domains", domainSid), formBody: request.ToForm(), ct: ct).ConfigureAwait(false);
        return r ?? throw new ApiException("empty body on POST /SIP/Domains/{sid}", 200);
    }

    /// <summary>Delete sip domains. Route: SIP, Domains, domainSid.</summary>
    public Task DeleteAsync(string domainSid, CancellationToken ct = default) =>
        Transport.SendAsync<object>(HttpMethod.Delete, Path("SIP", "Domains", domainSid), ct: ct);

    // --- Historical CredentialList mappings ---------------------------------

    /// <summary>List Credential List Mappings sip domains. Route: SIP, Domains, domainSid, CredentialListMappings.</summary>
    public async Task<SipCredentialListMappingList> ListCredentialListMappingsAsync(string domainSid, ListSipMappingPageParams? filter = null, CancellationToken ct = default)
    {
        var p = filter ?? new ListSipMappingPageParams();
        return await Transport.SendAsync<SipCredentialListMappingList>(HttpMethod.Get,
            Path("SIP", "Domains", domainSid, "CredentialListMappings"), queryParams: p.ToQuery(), ct: ct).ConfigureAwait(false)
            ?? new SipCredentialListMappingList();
    }

    /// <summary>Create Credential List Mapping sip domains. Route: SIP, Domains, domainSid, CredentialListMappings.</summary>
    public async Task<SipDomainMapping> CreateCredentialListMappingAsync(string domainSid, CreateSipCredentialListMappingRequest request, CancellationToken ct = default)
    {
        var r = await Transport.SendAsync<SipDomainMapping>(HttpMethod.Post,
            Path("SIP", "Domains", domainSid, "CredentialListMappings"), formBody: request.ToForm(), ct: ct).ConfigureAwait(false);
        return r ?? throw new ApiException("empty mapping POST", 200);
    }

    /// <summary>Get Credential List Mapping sip domains. Route: SIP, Domains, domainSid, CredentialListMappings, mappingSid.</summary>
    public async Task<SipDomainMapping> GetCredentialListMappingAsync(string domainSid, string mappingSid, CancellationToken ct = default)
    {
        var r = await Transport.SendAsync<SipDomainMapping>(HttpMethod.Get,
            Path("SIP", "Domains", domainSid, "CredentialListMappings", mappingSid), ct: ct).ConfigureAwait(false);
        return r ?? throw new ApiException("empty mapping GET", 200);
    }

    /// <summary>Delete Credential List Mapping sip domains. Route: SIP, Domains, domainSid, CredentialListMappings, mappingSid.</summary>
    public Task DeleteCredentialListMappingAsync(string domainSid, string mappingSid, CancellationToken ct = default) =>
        Transport.SendAsync<object>(HttpMethod.Delete, Path("SIP", "Domains", domainSid, "CredentialListMappings", mappingSid), ct: ct);

    // --- Historical IpAccessControlList mappings ---------------------------

    /// <summary>List Ip Access Control List Mappings sip domains. Route: SIP, Domains, domainSid, IpAccessControlListMappings.</summary>
    public async Task<SipIpAccessControlListMappingList> ListIpAccessControlListMappingsAsync(string domainSid, ListSipMappingPageParams? filter = null, CancellationToken ct = default)
    {
        var p = filter ?? new ListSipMappingPageParams();
        return await Transport.SendAsync<SipIpAccessControlListMappingList>(HttpMethod.Get,
            Path("SIP", "Domains", domainSid, "IpAccessControlListMappings"), queryParams: p.ToQuery(), ct: ct).ConfigureAwait(false)
            ?? new SipIpAccessControlListMappingList();
    }

    /// <summary>Create Ip Access Control List Mapping sip domains. Route: SIP, Domains, domainSid, IpAccessControlListMappings.</summary>
    public async Task<SipDomainMapping> CreateIpAccessControlListMappingAsync(string domainSid, CreateSipIpAccessControlListMappingRequest request, CancellationToken ct = default)
    {
        var r = await Transport.SendAsync<SipDomainMapping>(HttpMethod.Post,
            Path("SIP", "Domains", domainSid, "IpAccessControlListMappings"), formBody: request.ToForm(), ct: ct).ConfigureAwait(false);
        return r ?? throw new ApiException("empty mapping POST", 200);
    }

    /// <summary>Get Ip Access Control List Mapping sip domains. Route: SIP, Domains, domainSid, IpAccessControlListMappings, mappingSid.</summary>
    public async Task<SipDomainMapping> GetIpAccessControlListMappingAsync(string domainSid, string mappingSid, CancellationToken ct = default)
    {
        var r = await Transport.SendAsync<SipDomainMapping>(HttpMethod.Get,
            Path("SIP", "Domains", domainSid, "IpAccessControlListMappings", mappingSid), ct: ct).ConfigureAwait(false);
        return r ?? throw new ApiException("empty mapping GET", 200);
    }

    /// <summary>Delete Ip Access Control List Mapping sip domains. Route: SIP, Domains, domainSid, IpAccessControlListMappings, mappingSid.</summary>
    public Task DeleteIpAccessControlListMappingAsync(string domainSid, string mappingSid, CancellationToken ct = default) =>
        Transport.SendAsync<object>(HttpMethod.Delete, Path("SIP", "Domains", domainSid, "IpAccessControlListMappings", mappingSid), ct: ct);

    // --- Auth/Calls/CredentialListMappings ----------------------------------

    /// <summary>List Auth Calls Credential List Mappings sip domains. Route: SIP, Domains, domainSid, Auth, Calls, CredentialListMappings.</summary>
    public async Task<SipCredentialListMappingList> ListAuthCallsCredentialListMappingsAsync(string domainSid, ListSipMappingPageParams? filter = null, CancellationToken ct = default)
    {
        var p = filter ?? new ListSipMappingPageParams();
        return await Transport.SendAsync<SipCredentialListMappingList>(HttpMethod.Get,
            Path("SIP", "Domains", domainSid, "Auth", "Calls", "CredentialListMappings"), queryParams: p.ToQuery(), ct: ct).ConfigureAwait(false)
            ?? new SipCredentialListMappingList();
    }

    /// <summary>Create Auth Calls Credential List Mapping sip domains. Route: SIP, Domains, domainSid, Auth, Calls, CredentialListMappings.</summary>
    public async Task<SipDomainMapping> CreateAuthCallsCredentialListMappingAsync(string domainSid, CreateSipCredentialListMappingRequest request, CancellationToken ct = default)
    {
        var r = await Transport.SendAsync<SipDomainMapping>(HttpMethod.Post,
            Path("SIP", "Domains", domainSid, "Auth", "Calls", "CredentialListMappings"), formBody: request.ToForm(), ct: ct).ConfigureAwait(false);
        return r ?? throw new ApiException("empty mapping POST", 200);
    }

    /// <summary>Get Auth Calls Credential List Mapping sip domains. Route: SIP, Domains, domainSid, Auth, Calls, CredentialListMappings, mappingSid.</summary>
    public async Task<SipDomainMapping> GetAuthCallsCredentialListMappingAsync(string domainSid, string mappingSid, CancellationToken ct = default)
    {
        var r = await Transport.SendAsync<SipDomainMapping>(HttpMethod.Get,
            Path("SIP", "Domains", domainSid, "Auth", "Calls", "CredentialListMappings", mappingSid), ct: ct).ConfigureAwait(false);
        return r ?? throw new ApiException("empty mapping GET", 200);
    }

    /// <summary>Delete Auth Calls Credential List Mapping sip domains. Route: SIP, Domains, domainSid, Auth, Calls, CredentialListMappings, mappingSid.</summary>
    public Task DeleteAuthCallsCredentialListMappingAsync(string domainSid, string mappingSid, CancellationToken ct = default) =>
        Transport.SendAsync<object>(HttpMethod.Delete, Path("SIP", "Domains", domainSid, "Auth", "Calls", "CredentialListMappings", mappingSid), ct: ct);

    // --- Auth/Calls/IpAccessControlListMappings ----------------------------

    /// <summary>List Auth Calls Ip Access Control List Mappings sip domains. Route: SIP, Domains, domainSid, Auth, Calls, IpAccessControlListMappings.</summary>
    public async Task<SipIpAccessControlListMappingList> ListAuthCallsIpAccessControlListMappingsAsync(string domainSid, ListSipMappingPageParams? filter = null, CancellationToken ct = default)
    {
        var p = filter ?? new ListSipMappingPageParams();
        return await Transport.SendAsync<SipIpAccessControlListMappingList>(HttpMethod.Get,
            Path("SIP", "Domains", domainSid, "Auth", "Calls", "IpAccessControlListMappings"), queryParams: p.ToQuery(), ct: ct).ConfigureAwait(false)
            ?? new SipIpAccessControlListMappingList();
    }

    /// <summary>Create Auth Calls Ip Access Control List Mapping sip domains. Route: SIP, Domains, domainSid, Auth, Calls, IpAccessControlListMappings.</summary>
    public async Task<SipDomainMapping> CreateAuthCallsIpAccessControlListMappingAsync(string domainSid, CreateSipIpAccessControlListMappingRequest request, CancellationToken ct = default)
    {
        var r = await Transport.SendAsync<SipDomainMapping>(HttpMethod.Post,
            Path("SIP", "Domains", domainSid, "Auth", "Calls", "IpAccessControlListMappings"), formBody: request.ToForm(), ct: ct).ConfigureAwait(false);
        return r ?? throw new ApiException("empty mapping POST", 200);
    }

    /// <summary>Get Auth Calls Ip Access Control List Mapping sip domains. Route: SIP, Domains, domainSid, Auth, Calls, IpAccessControlListMappings, mappingSid.</summary>
    public async Task<SipDomainMapping> GetAuthCallsIpAccessControlListMappingAsync(string domainSid, string mappingSid, CancellationToken ct = default)
    {
        var r = await Transport.SendAsync<SipDomainMapping>(HttpMethod.Get,
            Path("SIP", "Domains", domainSid, "Auth", "Calls", "IpAccessControlListMappings", mappingSid), ct: ct).ConfigureAwait(false);
        return r ?? throw new ApiException("empty mapping GET", 200);
    }

    /// <summary>Delete Auth Calls Ip Access Control List Mapping sip domains. Route: SIP, Domains, domainSid, Auth, Calls, IpAccessControlListMappings, mappingSid.</summary>
    public Task DeleteAuthCallsIpAccessControlListMappingAsync(string domainSid, string mappingSid, CancellationToken ct = default) =>
        Transport.SendAsync<object>(HttpMethod.Delete, Path("SIP", "Domains", domainSid, "Auth", "Calls", "IpAccessControlListMappings", mappingSid), ct: ct);

    // --- Auth/Registrations/CredentialListMappings (no IP-ACL counterpart)

    /// <summary>List Auth Registrations Credential List Mappings sip domains. Route: SIP, Domains, domainSid, Auth, Registrations, CredentialListMappings.</summary>
    public async Task<SipCredentialListMappingList> ListAuthRegistrationsCredentialListMappingsAsync(string domainSid, ListSipMappingPageParams? filter = null, CancellationToken ct = default)
    {
        var p = filter ?? new ListSipMappingPageParams();
        return await Transport.SendAsync<SipCredentialListMappingList>(HttpMethod.Get,
            Path("SIP", "Domains", domainSid, "Auth", "Registrations", "CredentialListMappings"), queryParams: p.ToQuery(), ct: ct).ConfigureAwait(false)
            ?? new SipCredentialListMappingList();
    }

    /// <summary>Create Auth Registrations Credential List Mapping sip domains. Route: SIP, Domains, domainSid, Auth, Registrations, CredentialListMappings.</summary>
    public async Task<SipDomainMapping> CreateAuthRegistrationsCredentialListMappingAsync(string domainSid, CreateSipCredentialListMappingRequest request, CancellationToken ct = default)
    {
        var r = await Transport.SendAsync<SipDomainMapping>(HttpMethod.Post,
            Path("SIP", "Domains", domainSid, "Auth", "Registrations", "CredentialListMappings"), formBody: request.ToForm(), ct: ct).ConfigureAwait(false);
        return r ?? throw new ApiException("empty mapping POST", 200);
    }

    /// <summary>Get Auth Registrations Credential List Mapping sip domains. Route: SIP, Domains, domainSid, Auth, Registrations, CredentialListMappings, mappingSid.</summary>
    public async Task<SipDomainMapping> GetAuthRegistrationsCredentialListMappingAsync(string domainSid, string mappingSid, CancellationToken ct = default)
    {
        var r = await Transport.SendAsync<SipDomainMapping>(HttpMethod.Get,
            Path("SIP", "Domains", domainSid, "Auth", "Registrations", "CredentialListMappings", mappingSid), ct: ct).ConfigureAwait(false);
        return r ?? throw new ApiException("empty mapping GET", 200);
    }

    /// <summary>Delete Auth Registrations Credential List Mapping sip domains. Route: SIP, Domains, domainSid, Auth, Registrations, CredentialListMappings, mappingSid.</summary>
    public Task DeleteAuthRegistrationsCredentialListMappingAsync(string domainSid, string mappingSid, CancellationToken ct = default) =>
        Transport.SendAsync<object>(HttpMethod.Delete, Path("SIP", "Domains", domainSid, "Auth", "Registrations", "CredentialListMappings", mappingSid), ct: ct);
}

/// <summary>Operations on <c>/SIP/CredentialLists</c> plus per-list /Credentials sub-resource.</summary>
public sealed class SipCredentialListsResource : ResourceBase
{
    internal SipCredentialListsResource(Transport transport) : base(transport) { }

    /// <summary>List sip credential lists. Route: SIP, CredentialLists.</summary>
    public async Task<SipCredentialListList> ListAsync(ListSipPageParams? filter = null, CancellationToken ct = default)
    {
        var p = filter ?? new ListSipPageParams();
        return await Transport.SendAsync<SipCredentialListList>(HttpMethod.Get,
            Path("SIP", "CredentialLists"), queryParams: p.ToQuery(), ct: ct).ConfigureAwait(false)
            ?? new SipCredentialListList();
    }

    /// <summary>Create sip credential lists. Route: SIP, CredentialLists.</summary>
    public async Task<SipCredentialList> CreateAsync(CreateSipCredentialListRequest request, CancellationToken ct = default)
    {
        var r = await Transport.SendAsync<SipCredentialList>(HttpMethod.Post,
            Path("SIP", "CredentialLists"), formBody: request.ToForm(), ct: ct).ConfigureAwait(false);
        return r ?? throw new ApiException("empty CL POST", 200);
    }

    /// <summary>Fetch sip credential lists. Route: SIP, CredentialLists, credentialListSid.</summary>
    public async Task<SipCredentialList> GetAsync(string credentialListSid, CancellationToken ct = default)
    {
        var r = await Transport.SendAsync<SipCredentialList>(HttpMethod.Get,
            Path("SIP", "CredentialLists", credentialListSid), ct: ct).ConfigureAwait(false);
        return r ?? throw new ApiException("empty CL GET", 200);
    }

    /// <summary>Update sip credential lists. Route: SIP, CredentialLists, credentialListSid.</summary>
    public async Task<SipCredentialList> UpdateAsync(string credentialListSid, UpdateSipCredentialListRequest request, CancellationToken ct = default)
    {
        var r = await Transport.SendAsync<SipCredentialList>(HttpMethod.Post,
            Path("SIP", "CredentialLists", credentialListSid), formBody: request.ToForm(), ct: ct).ConfigureAwait(false);
        return r ?? throw new ApiException("empty CL POST", 200);
    }

    /// <summary>Delete sip credential lists. Route: SIP, CredentialLists, credentialListSid.</summary>
    public Task DeleteAsync(string credentialListSid, CancellationToken ct = default) =>
        Transport.SendAsync<object>(HttpMethod.Delete, Path("SIP", "CredentialLists", credentialListSid), ct: ct);

    // --- /Credentials sub-resource -----------------------------------------

    /// <summary>List Credentials sip credential lists. Route: SIP, CredentialLists, credentialListSid, Credentials.</summary>
    public async Task<SipCredentialListPage> ListCredentialsAsync(string credentialListSid, ListSipMappingPageParams? filter = null, CancellationToken ct = default)
    {
        var p = filter ?? new ListSipMappingPageParams();
        return await Transport.SendAsync<SipCredentialListPage>(HttpMethod.Get,
            Path("SIP", "CredentialLists", credentialListSid, "Credentials"), queryParams: p.ToQuery(), ct: ct).ConfigureAwait(false)
            ?? new SipCredentialListPage();
    }

    /// <summary>Create Credential sip credential lists. Route: SIP, CredentialLists, credentialListSid, Credentials.</summary>
    public async Task<SipCredential> CreateCredentialAsync(string credentialListSid, CreateSipCredentialRequest request, CancellationToken ct = default)
    {
        var r = await Transport.SendAsync<SipCredential>(HttpMethod.Post,
            Path("SIP", "CredentialLists", credentialListSid, "Credentials"), formBody: request.ToForm(), ct: ct).ConfigureAwait(false);
        return r ?? throw new ApiException("empty CR POST", 200);
    }

    /// <summary>Get Credential sip credential lists. Route: SIP, CredentialLists, credentialListSid, Credentials, credentialSid.</summary>
    public async Task<SipCredential> GetCredentialAsync(string credentialListSid, string credentialSid, CancellationToken ct = default)
    {
        var r = await Transport.SendAsync<SipCredential>(HttpMethod.Get,
            Path("SIP", "CredentialLists", credentialListSid, "Credentials", credentialSid), ct: ct).ConfigureAwait(false);
        return r ?? throw new ApiException("empty CR GET", 200);
    }

    /// <summary>Update Credential sip credential lists. Route: SIP, CredentialLists, credentialListSid, Credentials, credentialSid.</summary>
    public async Task<SipCredential> UpdateCredentialAsync(string credentialListSid, string credentialSid, UpdateSipCredentialRequest request, CancellationToken ct = default)
    {
        var r = await Transport.SendAsync<SipCredential>(HttpMethod.Post,
            Path("SIP", "CredentialLists", credentialListSid, "Credentials", credentialSid), formBody: request.ToForm(), ct: ct).ConfigureAwait(false);
        return r ?? throw new ApiException("empty CR POST", 200);
    }

    /// <summary>Delete Credential sip credential lists. Route: SIP, CredentialLists, credentialListSid, Credentials, credentialSid.</summary>
    public Task DeleteCredentialAsync(string credentialListSid, string credentialSid, CancellationToken ct = default) =>
        Transport.SendAsync<object>(HttpMethod.Delete, Path("SIP", "CredentialLists", credentialListSid, "Credentials", credentialSid), ct: ct);
}

/// <summary>Operations on <c>/SIP/IpAccessControlLists</c> plus per-list /IpAddresses sub-resource.</summary>
public sealed class SipIpAccessControlListsResource : ResourceBase
{
    internal SipIpAccessControlListsResource(Transport transport) : base(transport) { }

    /// <summary>List sip ip access control lists. Route: SIP, IpAccessControlLists.</summary>
    public async Task<SipIpAccessControlListList> ListAsync(ListSipPageParams? filter = null, CancellationToken ct = default)
    {
        var p = filter ?? new ListSipPageParams();
        return await Transport.SendAsync<SipIpAccessControlListList>(HttpMethod.Get,
            Path("SIP", "IpAccessControlLists"), queryParams: p.ToQuery(), ct: ct).ConfigureAwait(false)
            ?? new SipIpAccessControlListList();
    }

    /// <summary>Create sip ip access control lists. Route: SIP, IpAccessControlLists.</summary>
    public async Task<SipIpAccessControlList> CreateAsync(CreateSipIpAccessControlListRequest request, CancellationToken ct = default)
    {
        var r = await Transport.SendAsync<SipIpAccessControlList>(HttpMethod.Post,
            Path("SIP", "IpAccessControlLists"), formBody: request.ToForm(), ct: ct).ConfigureAwait(false);
        return r ?? throw new ApiException("empty ACL POST", 200);
    }

    /// <summary>Fetch sip ip access control lists. Route: SIP, IpAccessControlLists, aclSid.</summary>
    public async Task<SipIpAccessControlList> GetAsync(string aclSid, CancellationToken ct = default)
    {
        var r = await Transport.SendAsync<SipIpAccessControlList>(HttpMethod.Get,
            Path("SIP", "IpAccessControlLists", aclSid), ct: ct).ConfigureAwait(false);
        return r ?? throw new ApiException("empty ACL GET", 200);
    }

    /// <summary>Update sip ip access control lists. Route: SIP, IpAccessControlLists, aclSid.</summary>
    public async Task<SipIpAccessControlList> UpdateAsync(string aclSid, UpdateSipIpAccessControlListRequest request, CancellationToken ct = default)
    {
        var r = await Transport.SendAsync<SipIpAccessControlList>(HttpMethod.Post,
            Path("SIP", "IpAccessControlLists", aclSid), formBody: request.ToForm(), ct: ct).ConfigureAwait(false);
        return r ?? throw new ApiException("empty ACL POST", 200);
    }

    /// <summary>Delete sip ip access control lists. Route: SIP, IpAccessControlLists, aclSid.</summary>
    public Task DeleteAsync(string aclSid, CancellationToken ct = default) =>
        Transport.SendAsync<object>(HttpMethod.Delete, Path("SIP", "IpAccessControlLists", aclSid), ct: ct);

    // --- /IpAddresses sub-resource ------------------------------------------

    /// <summary>List Ip Addresses sip ip access control lists. Route: SIP, IpAccessControlLists, aclSid, IpAddresses.</summary>
    public async Task<SipIpAddressList> ListIpAddressesAsync(string aclSid, ListSipPageParams? filter = null, CancellationToken ct = default)
    {
        var p = filter ?? new ListSipPageParams();
        return await Transport.SendAsync<SipIpAddressList>(HttpMethod.Get,
            Path("SIP", "IpAccessControlLists", aclSid, "IpAddresses"), queryParams: p.ToQuery(), ct: ct).ConfigureAwait(false)
            ?? new SipIpAddressList();
    }

    /// <summary>Create Ip Address sip ip access control lists. Route: SIP, IpAccessControlLists, aclSid, IpAddresses.</summary>
    public async Task<SipIpAddress> CreateIpAddressAsync(string aclSid, CreateSipIpAddressRequest request, CancellationToken ct = default)
    {
        var r = await Transport.SendAsync<SipIpAddress>(HttpMethod.Post,
            Path("SIP", "IpAccessControlLists", aclSid, "IpAddresses"), formBody: request.ToForm(), ct: ct).ConfigureAwait(false);
        return r ?? throw new ApiException("empty IP POST", 200);
    }

    /// <summary>Get Ip Address sip ip access control lists. Route: SIP, IpAccessControlLists, aclSid, IpAddresses, ipAddressSid.</summary>
    public async Task<SipIpAddress> GetIpAddressAsync(string aclSid, string ipAddressSid, CancellationToken ct = default)
    {
        var r = await Transport.SendAsync<SipIpAddress>(HttpMethod.Get,
            Path("SIP", "IpAccessControlLists", aclSid, "IpAddresses", ipAddressSid), ct: ct).ConfigureAwait(false);
        return r ?? throw new ApiException("empty IP GET", 200);
    }

    /// <summary>Update Ip Address sip ip access control lists. Route: SIP, IpAccessControlLists, aclSid, IpAddresses, ipAddressSid.</summary>
    public async Task<SipIpAddress> UpdateIpAddressAsync(string aclSid, string ipAddressSid, UpdateSipIpAddressRequest request, CancellationToken ct = default)
    {
        var r = await Transport.SendAsync<SipIpAddress>(HttpMethod.Post,
            Path("SIP", "IpAccessControlLists", aclSid, "IpAddresses", ipAddressSid), formBody: request.ToForm(), ct: ct).ConfigureAwait(false);
        return r ?? throw new ApiException("empty IP POST", 200);
    }

    /// <summary>Delete Ip Address sip ip access control lists. Route: SIP, IpAccessControlLists, aclSid, IpAddresses, ipAddressSid.</summary>
    public Task DeleteIpAddressAsync(string aclSid, string ipAddressSid, CancellationToken ct = default) =>
        Transport.SendAsync<object>(HttpMethod.Delete, Path("SIP", "IpAccessControlLists", aclSid, "IpAddresses", ipAddressSid), ct: ct);
}
