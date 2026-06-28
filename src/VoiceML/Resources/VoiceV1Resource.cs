using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using VoiceML.Exceptions;
using VoiceML.Models;

namespace VoiceML.Resources;

/// <summary>Top-level <c>/v1/*</c> Voice surface — Twilio Voice v1 (BYOC, connection
/// policies, source-IP routing, dialing-permissions settings). Six sub-resources:
/// <see cref="IpRecords"/>, <see cref="SourceIpMappings"/>, <see cref="ByocTrunks"/>,
/// <see cref="ConnectionPolicies"/>, <see cref="Settings"/>.
/// <para>The <c>/v1/</c> namespace bypasses the <c>/2010-04-01/Accounts/{Sid}/</c>
/// prefix used by the rest of the SDK — account resolves from HTTP Basic auth.</para>
/// </summary>
public sealed class VoiceV1Resource
{
    public VoiceV1IpRecordsResource IpRecords { get; }
    public VoiceV1SourceIpMappingsResource SourceIpMappings { get; }
    public VoiceV1ByocTrunksResource ByocTrunks { get; }
    public VoiceV1ConnectionPoliciesResource ConnectionPolicies { get; }
    public VoiceV1SettingsResource Settings { get; }

    public VoiceV1Resource(Transport transport)
    {
        IpRecords = new VoiceV1IpRecordsResource(transport);
        SourceIpMappings = new VoiceV1SourceIpMappingsResource(transport);
        ByocTrunks = new VoiceV1ByocTrunksResource(transport);
        ConnectionPolicies = new VoiceV1ConnectionPoliciesResource(transport);
        Settings = new VoiceV1SettingsResource(transport);
    }
}

/// <summary>Operations on <c>/v1/IpRecords</c>.</summary>
public sealed class VoiceV1IpRecordsResource
{
    private readonly Transport _transport;
    public VoiceV1IpRecordsResource(Transport transport) { _transport = transport; }

    public async Task<VoiceV1IpRecord> CreateAsync(CreateVoiceV1IpRecordRequest request, CancellationToken ct = default)
    {
        var r = await _transport.SendAsync<VoiceV1IpRecord>(HttpMethod.Post,
            "/v1/IpRecords", formBody: request.ToForm(), ct: ct).ConfigureAwait(false);
        return r ?? throw new ApiException("empty body on POST /v1/IpRecords", 201);
    }

    public async Task<VoiceV1IpRecordList> ListAsync(ListV1PageParams? filter = null, CancellationToken ct = default)
    {
        var p = filter ?? new ListV1PageParams();
        return await _transport.SendAsync<VoiceV1IpRecordList>(HttpMethod.Get,
            "/v1/IpRecords", queryParams: p.ToQuery(), ct: ct).ConfigureAwait(false)
            ?? new VoiceV1IpRecordList();
    }

    public async Task<VoiceV1IpRecord> GetAsync(string sid, CancellationToken ct = default)
    {
        var r = await _transport.SendAsync<VoiceV1IpRecord>(HttpMethod.Get,
            $"/v1/IpRecords/{sid}", ct: ct).ConfigureAwait(false);
        return r ?? throw new ApiException("empty body on GET /v1/IpRecords/{sid}", 200);
    }

    public async Task<VoiceV1IpRecord> UpdateAsync(string sid, UpdateVoiceV1IpRecordRequest request, CancellationToken ct = default)
    {
        var r = await _transport.SendAsync<VoiceV1IpRecord>(HttpMethod.Post,
            $"/v1/IpRecords/{sid}", formBody: request.ToForm(), ct: ct).ConfigureAwait(false);
        return r ?? throw new ApiException("empty body on POST /v1/IpRecords/{sid}", 200);
    }

    public Task DeleteAsync(string sid, CancellationToken ct = default) =>
        _transport.SendAsync<object>(HttpMethod.Delete, $"/v1/IpRecords/{sid}", ct: ct);
}

/// <summary>Operations on <c>/v1/SourceIpMappings</c>.</summary>
public sealed class VoiceV1SourceIpMappingsResource
{
    private readonly Transport _transport;
    public VoiceV1SourceIpMappingsResource(Transport transport) { _transport = transport; }

    public async Task<VoiceV1SourceIpMapping> CreateAsync(CreateVoiceV1SourceIpMappingRequest request, CancellationToken ct = default)
    {
        var r = await _transport.SendAsync<VoiceV1SourceIpMapping>(HttpMethod.Post,
            "/v1/SourceIpMappings", formBody: request.ToForm(), ct: ct).ConfigureAwait(false);
        return r ?? throw new ApiException("empty body on POST /v1/SourceIpMappings", 201);
    }

    public async Task<VoiceV1SourceIpMappingList> ListAsync(ListV1PageParams? filter = null, CancellationToken ct = default)
    {
        var p = filter ?? new ListV1PageParams();
        return await _transport.SendAsync<VoiceV1SourceIpMappingList>(HttpMethod.Get,
            "/v1/SourceIpMappings", queryParams: p.ToQuery(), ct: ct).ConfigureAwait(false)
            ?? new VoiceV1SourceIpMappingList();
    }

    public async Task<VoiceV1SourceIpMapping> GetAsync(string sid, CancellationToken ct = default)
    {
        var r = await _transport.SendAsync<VoiceV1SourceIpMapping>(HttpMethod.Get,
            $"/v1/SourceIpMappings/{sid}", ct: ct).ConfigureAwait(false);
        return r ?? throw new ApiException("empty body on GET /v1/SourceIpMappings/{sid}", 200);
    }

    public async Task<VoiceV1SourceIpMapping> UpdateAsync(string sid, UpdateVoiceV1SourceIpMappingRequest request, CancellationToken ct = default)
    {
        var r = await _transport.SendAsync<VoiceV1SourceIpMapping>(HttpMethod.Post,
            $"/v1/SourceIpMappings/{sid}", formBody: request.ToForm(), ct: ct).ConfigureAwait(false);
        return r ?? throw new ApiException("empty body on POST /v1/SourceIpMappings/{sid}", 200);
    }

    public Task DeleteAsync(string sid, CancellationToken ct = default) =>
        _transport.SendAsync<object>(HttpMethod.Delete, $"/v1/SourceIpMappings/{sid}", ct: ct);
}

/// <summary>Operations on <c>/v1/ByocTrunks</c>.</summary>
public sealed class VoiceV1ByocTrunksResource
{
    private readonly Transport _transport;
    public VoiceV1ByocTrunksResource(Transport transport) { _transport = transport; }

    public async Task<VoiceV1ByocTrunk> CreateAsync(CreateVoiceV1ByocTrunkRequest request, CancellationToken ct = default)
    {
        var r = await _transport.SendAsync<VoiceV1ByocTrunk>(HttpMethod.Post,
            "/v1/ByocTrunks", formBody: request.ToForm(), ct: ct).ConfigureAwait(false);
        return r ?? throw new ApiException("empty body on POST /v1/ByocTrunks", 201);
    }

    public async Task<VoiceV1ByocTrunkList> ListAsync(ListV1PageParams? filter = null, CancellationToken ct = default)
    {
        var p = filter ?? new ListV1PageParams();
        return await _transport.SendAsync<VoiceV1ByocTrunkList>(HttpMethod.Get,
            "/v1/ByocTrunks", queryParams: p.ToQuery(), ct: ct).ConfigureAwait(false)
            ?? new VoiceV1ByocTrunkList();
    }

    public async Task<VoiceV1ByocTrunk> GetAsync(string sid, CancellationToken ct = default)
    {
        var r = await _transport.SendAsync<VoiceV1ByocTrunk>(HttpMethod.Get,
            $"/v1/ByocTrunks/{sid}", ct: ct).ConfigureAwait(false);
        return r ?? throw new ApiException("empty body on GET /v1/ByocTrunks/{sid}", 200);
    }

    public async Task<VoiceV1ByocTrunk> UpdateAsync(string sid, UpdateVoiceV1ByocTrunkRequest request, CancellationToken ct = default)
    {
        var r = await _transport.SendAsync<VoiceV1ByocTrunk>(HttpMethod.Post,
            $"/v1/ByocTrunks/{sid}", formBody: request.ToForm(), ct: ct).ConfigureAwait(false);
        return r ?? throw new ApiException("empty body on POST /v1/ByocTrunks/{sid}", 200);
    }

    public Task DeleteAsync(string sid, CancellationToken ct = default) =>
        _transport.SendAsync<object>(HttpMethod.Delete, $"/v1/ByocTrunks/{sid}", ct: ct);
}

/// <summary>Operations on <c>/v1/ConnectionPolicies</c> plus the per-policy
/// <c>/Targets</c> sub-resource (via the <see cref="Targets"/> factory).</summary>
public sealed class VoiceV1ConnectionPoliciesResource
{
    private readonly Transport _transport;
    public VoiceV1ConnectionPoliciesResource(Transport transport) { _transport = transport; }

    public async Task<VoiceV1ConnectionPolicy> CreateAsync(CreateVoiceV1ConnectionPolicyRequest request, CancellationToken ct = default)
    {
        var r = await _transport.SendAsync<VoiceV1ConnectionPolicy>(HttpMethod.Post,
            "/v1/ConnectionPolicies", formBody: request.ToForm(), ct: ct).ConfigureAwait(false);
        return r ?? throw new ApiException("empty body on POST /v1/ConnectionPolicies", 201);
    }

    public async Task<VoiceV1ConnectionPolicyList> ListAsync(ListV1PageParams? filter = null, CancellationToken ct = default)
    {
        var p = filter ?? new ListV1PageParams();
        return await _transport.SendAsync<VoiceV1ConnectionPolicyList>(HttpMethod.Get,
            "/v1/ConnectionPolicies", queryParams: p.ToQuery(), ct: ct).ConfigureAwait(false)
            ?? new VoiceV1ConnectionPolicyList();
    }

    public async Task<VoiceV1ConnectionPolicy> GetAsync(string sid, CancellationToken ct = default)
    {
        var r = await _transport.SendAsync<VoiceV1ConnectionPolicy>(HttpMethod.Get,
            $"/v1/ConnectionPolicies/{sid}", ct: ct).ConfigureAwait(false);
        return r ?? throw new ApiException("empty body on GET /v1/ConnectionPolicies/{sid}", 200);
    }

    public async Task<VoiceV1ConnectionPolicy> UpdateAsync(string sid, UpdateVoiceV1ConnectionPolicyRequest request, CancellationToken ct = default)
    {
        var r = await _transport.SendAsync<VoiceV1ConnectionPolicy>(HttpMethod.Post,
            $"/v1/ConnectionPolicies/{sid}", formBody: request.ToForm(), ct: ct).ConfigureAwait(false);
        return r ?? throw new ApiException("empty body on POST /v1/ConnectionPolicies/{sid}", 200);
    }

    public Task DeleteAsync(string sid, CancellationToken ct = default) =>
        _transport.SendAsync<object>(HttpMethod.Delete, $"/v1/ConnectionPolicies/{sid}", ct: ct);

    /// <summary>Scope to a specific ConnectionPolicy's <c>/Targets</c> sub-resource.</summary>
    public VoiceV1ConnectionPolicyTargetsScope Targets(string connectionPolicySid) =>
        new(_transport, connectionPolicySid);
}

/// <summary>Operations on <c>/v1/ConnectionPolicies/{ConnectionPolicySid}/Targets</c>.
/// Obtained via <see cref="VoiceV1ConnectionPoliciesResource.Targets"/>.</summary>
public sealed class VoiceV1ConnectionPolicyTargetsScope
{
    private readonly Transport _transport;
    private readonly string _connectionPolicySid;

    internal VoiceV1ConnectionPolicyTargetsScope(Transport transport, string connectionPolicySid)
    {
        _transport = transport;
        _connectionPolicySid = connectionPolicySid;
    }

    private string BasePath => $"/v1/ConnectionPolicies/{_connectionPolicySid}/Targets";

    public async Task<VoiceV1ConnectionPolicyTarget> CreateAsync(CreateVoiceV1ConnectionPolicyTargetRequest request, CancellationToken ct = default)
    {
        var r = await _transport.SendAsync<VoiceV1ConnectionPolicyTarget>(HttpMethod.Post,
            BasePath, formBody: request.ToForm(), ct: ct).ConfigureAwait(false);
        return r ?? throw new ApiException("empty body on POST .../Targets", 201);
    }

    public async Task<VoiceV1ConnectionPolicyTargetList> ListAsync(ListV1PageParams? filter = null, CancellationToken ct = default)
    {
        var p = filter ?? new ListV1PageParams();
        return await _transport.SendAsync<VoiceV1ConnectionPolicyTargetList>(HttpMethod.Get,
            BasePath, queryParams: p.ToQuery(), ct: ct).ConfigureAwait(false)
            ?? new VoiceV1ConnectionPolicyTargetList();
    }

    public async Task<VoiceV1ConnectionPolicyTarget> GetAsync(string sid, CancellationToken ct = default)
    {
        var r = await _transport.SendAsync<VoiceV1ConnectionPolicyTarget>(HttpMethod.Get,
            $"{BasePath}/{sid}", ct: ct).ConfigureAwait(false);
        return r ?? throw new ApiException("empty body on GET .../Targets/{sid}", 200);
    }

    public async Task<VoiceV1ConnectionPolicyTarget> UpdateAsync(string sid, UpdateVoiceV1ConnectionPolicyTargetRequest request, CancellationToken ct = default)
    {
        var r = await _transport.SendAsync<VoiceV1ConnectionPolicyTarget>(HttpMethod.Post,
            $"{BasePath}/{sid}", formBody: request.ToForm(), ct: ct).ConfigureAwait(false);
        return r ?? throw new ApiException("empty body on POST .../Targets/{sid}", 200);
    }

    public Task DeleteAsync(string sid, CancellationToken ct = default) =>
        _transport.SendAsync<object>(HttpMethod.Delete, $"{BasePath}/{sid}", ct: ct);
}

/// <summary>Operations on the account-wide <c>/v1/Settings</c> (DialingPermissions). Singleton — no sid.</summary>
public sealed class VoiceV1SettingsResource
{
    private readonly Transport _transport;
    public VoiceV1SettingsResource(Transport transport) { _transport = transport; }

    public async Task<VoiceV1DialingPermissionsSettings> FetchAsync(CancellationToken ct = default)
    {
        var r = await _transport.SendAsync<VoiceV1DialingPermissionsSettings>(HttpMethod.Get,
            "/v1/Settings", ct: ct).ConfigureAwait(false);
        return r ?? throw new ApiException("empty body on GET /v1/Settings", 200);
    }

    public async Task<VoiceV1DialingPermissionsSettings> UpdateAsync(UpdateVoiceV1SettingsRequest request, CancellationToken ct = default)
    {
        var r = await _transport.SendAsync<VoiceV1DialingPermissionsSettings>(HttpMethod.Post,
            "/v1/Settings", formBody: request.ToForm(), ct: ct).ConfigureAwait(false);
        return r ?? throw new ApiException("empty body on POST /v1/Settings", 202);
    }
}
