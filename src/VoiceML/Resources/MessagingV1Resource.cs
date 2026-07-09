using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using VoiceML.Exceptions;
using VoiceML.Models;

namespace VoiceML.Resources;

/// <summary>Top-level <c>/v1/*</c> Messaging surface — Twilio <c>messaging.twilio.com/v1</c>.
/// The whole group is routed at the messaging host (<c>messaging.voicetel.com</c>) by the client,
/// which is what disambiguates a Messaging Service (<c>MG…</c>) from a Conversation Service
/// (<c>IS…</c>): they share the <c>/v1/Services</c> path shape. See <see cref="ProductHosts"/>.</summary>
public sealed class MessagingV1Resource
{
    /// <summary>Operations on <c>/v1/Services</c> (Messaging Services).</summary>
    public MessagingV1ServicesResource Services { get; }

    public MessagingV1Resource(Transport transport)
    {
        Services = new MessagingV1ServicesResource(transport);
    }
}

/// <summary>Operations on <c>/v1/Services</c> at the messaging host.
/// <para><see cref="CreateAsync"/> / <see cref="ListAsync"/> / <see cref="GetAsync"/> /
/// <see cref="DeleteAsync"/> reuse the shared path; <see cref="UpdateAsync"/>
/// (<c>POST /v1/Services/{sid}</c>) is unique to Messaging Service.</para></summary>
public sealed class MessagingV1ServicesResource
{
    private readonly Transport _transport;
    public MessagingV1ServicesResource(Transport transport) { _transport = transport; }

    public async Task<MessagingService> CreateAsync(CreateMessagingServiceRequest request, CancellationToken ct = default)
    {
        var r = await _transport.SendAsync<MessagingService>(HttpMethod.Post,
            "/v1/Services", formBody: request.ToForm(), ct: ct).ConfigureAwait(false);
        return r ?? throw new ApiException("empty body on POST /v1/Services", 201);
    }

    public async Task<MessagingServiceList> ListAsync(int? pageSize = null, CancellationToken ct = default)
    {
        var query = new[] { new System.Collections.Generic.KeyValuePair<string, string?>("PageSize", pageSize?.ToString()) };
        return await _transport.SendAsync<MessagingServiceList>(HttpMethod.Get,
            "/v1/Services", queryParams: query, ct: ct).ConfigureAwait(false)
            ?? new MessagingServiceList();
    }

    public async Task<MessagingService> GetAsync(string sid, CancellationToken ct = default)
    {
        var r = await _transport.SendAsync<MessagingService>(HttpMethod.Get,
            $"/v1/Services/{sid}", ct: ct).ConfigureAwait(false);
        return r ?? throw new ApiException("empty body on GET /v1/Services/{sid}", 200);
    }

    public async Task<MessagingService> UpdateAsync(string sid, UpdateMessagingServiceRequest request, CancellationToken ct = default)
    {
        var r = await _transport.SendAsync<MessagingService>(HttpMethod.Post,
            $"/v1/Services/{sid}", formBody: request.ToForm(), ct: ct).ConfigureAwait(false);
        return r ?? throw new ApiException("empty body on POST /v1/Services/{sid}", 200);
    }

    public Task DeleteAsync(string sid, CancellationToken ct = default) =>
        _transport.SendAsync<object>(HttpMethod.Delete, $"/v1/Services/{sid}", ct: ct);
}
