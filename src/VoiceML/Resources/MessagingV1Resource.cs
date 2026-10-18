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

    internal MessagingV1Resource(Transport transport)
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

    /// <summary>Fetch the /v1 page a list response links to next, following its
    /// <see cref="VoiceV1Meta.NextPageUrl"/> verbatim — but only when its scheme and host
    /// equal the configured product host, so credentials never go to a foreign host. Returns
    /// <c>null</c> when <paramref name="page"/> is the last one.</summary>
    public Task<T?> NextPageAsync<T>(T page, CancellationToken ct = default) where T : class, IV1PagedList
        => string.IsNullOrEmpty(page.Meta?.NextPageUrl)
            ? Task.FromResult<T?>(null)
            : _transport.SendV1NextPageAsync<T>(page.Meta!.NextPageUrl!, ct);

    internal MessagingV1ServicesResource(Transport transport) { _transport = transport; }

    /// <summary>Create services. Route: /v1/Services.</summary>
    public async Task<MessagingService> CreateAsync(CreateMessagingServiceRequest request, CancellationToken ct = default)
    {
        var r = await _transport.SendAsync<MessagingService>(HttpMethod.Post,
            "/v1/Services", formBody: request.ToForm(), ct: ct).ConfigureAwait(false);
        return r ?? throw new ApiException("empty body on POST /v1/Services", 201);
    }

    /// <summary>List services. Route: /v1/Services.</summary>
    public async Task<MessagingServiceList> ListAsync(int? pageSize = null, CancellationToken ct = default)
    {
        var query = new[] { new System.Collections.Generic.KeyValuePair<string, string?>("PageSize", pageSize?.ToString()) };
        return await _transport.SendAsync<MessagingServiceList>(HttpMethod.Get,
            "/v1/Services", queryParams: query, ct: ct).ConfigureAwait(false)
            ?? new MessagingServiceList();
    }

    /// <summary>Fetch services. Route: /v1/Services/{PathSegment.Encode(sid)}.</summary>
    public async Task<MessagingService> GetAsync(string sid, CancellationToken ct = default)
    {
        var r = await _transport.SendAsync<MessagingService>(HttpMethod.Get,
            $"/v1/Services/{PathSegment.Encode(sid)}", ct: ct).ConfigureAwait(false);
        return r ?? throw new ApiException("empty body on GET /v1/Services/{sid}", 200);
    }

    /// <summary>Scope to a service's <c>/PhoneNumbers</c> sender-pool sub-resource.</summary>
    public MessagingServicePhoneNumbersResource PhoneNumbers(string chatServiceSid)
        => new(_transport, chatServiceSid);

    /// <summary>Update services. Route: /v1/Services/{PathSegment.Encode(sid)}.</summary>
    public async Task<MessagingService> UpdateAsync(string sid, UpdateMessagingServiceRequest request, CancellationToken ct = default)
    {
        var r = await _transport.SendAsync<MessagingService>(HttpMethod.Post,
            $"/v1/Services/{PathSegment.Encode(sid)}", formBody: request.ToForm(), ct: ct).ConfigureAwait(false);
        return r ?? throw new ApiException("empty body on POST /v1/Services/{sid}", 200);
    }

    /// <summary>Delete services. Route: /v1/Services/{PathSegment.Encode(sid)}.</summary>
    public Task DeleteAsync(string sid, CancellationToken ct = default) =>
        _transport.SendAsync<object>(HttpMethod.Delete, $"/v1/Services/{PathSegment.Encode(sid)}", ct: ct);
}

/// <summary>Operations on <c>/v1/Services/{ChatServiceSid}/PhoneNumbers</c> — the Messaging
/// Service's sender pool. Obtain via
/// <see cref="MessagingV1ServicesResource.PhoneNumbers"/>. A number belongs to at most one
/// service: re-adding it to the same service is 409/21710, adding it under another service
/// is 409/21712.</summary>
public sealed class MessagingServicePhoneNumbersResource
{
    private readonly Transport _transport;
    /// <summary>Fetch the /v1 page a list response links to next, following its
    /// <see cref="VoiceV1Meta.NextPageUrl"/> verbatim — but only when its scheme and host
    /// equal the configured product host, so credentials never go to a foreign host. Returns
    /// <c>null</c> when <paramref name="page"/> is the last one.</summary>
    public Task<T?> NextPageAsync<T>(T page, CancellationToken ct = default) where T : class, IV1PagedList
        => string.IsNullOrEmpty(page.Meta?.NextPageUrl)
            ? Task.FromResult<T?>(null)
            : _transport.SendV1NextPageAsync<T>(page.Meta!.NextPageUrl!, ct);

    private readonly string _chatServiceSid;

    internal MessagingServicePhoneNumbersResource(Transport transport, string chatServiceSid)
    {
        _transport = transport;
        _chatServiceSid = chatServiceSid;
    }

    private string BasePath => $"/v1/Services/{PathSegment.Encode(_chatServiceSid)}/PhoneNumbers";

    /// <summary>List the numbers in the service's sender pool.</summary>
    public async Task<MessagingServicePhoneNumberList> ListAsync(
        int? pageSize = null, int? page = null, CancellationToken ct = default)
    {
        var query = new[]
        {
            new System.Collections.Generic.KeyValuePair<string, string?>("PageSize", pageSize?.ToString()),
            new System.Collections.Generic.KeyValuePair<string, string?>("Page", page?.ToString()),
        };
        return await _transport.SendAsync<MessagingServicePhoneNumberList>(HttpMethod.Get,
            BasePath, queryParams: query, ct: ct).ConfigureAwait(false)
            ?? new MessagingServicePhoneNumberList();
    }

    /// <summary>Fetch the page a list response links to next, following its
    /// <see cref="VoiceV1Meta.NextPageUrl"/> verbatim after checking it stays on the
    /// configured messaging host. Returns <c>null</c> when the page is the last one.</summary>
    public Task<MessagingServicePhoneNumberList?> NextPageAsync(MessagingServicePhoneNumberList page, CancellationToken ct = default)
        => string.IsNullOrEmpty(page.Meta?.NextPageUrl)
            ? Task.FromResult<MessagingServicePhoneNumberList?>(null)
            : _transport.SendV1NextPageAsync<MessagingServicePhoneNumberList>(page.Meta!.NextPageUrl!, ct);

    /// <summary>Add an account-owned IncomingPhoneNumber (by PN sid) to the pool. Twilio
    /// answers 200 on the add.</summary>
    public async Task<MessagingServicePhoneNumber> CreateAsync(
        AddMessagingServicePhoneNumberRequest request, CancellationToken ct = default)
    {
        var r = await _transport.SendAsync<MessagingServicePhoneNumber>(HttpMethod.Post,
            BasePath, formBody: request.ToForm(), ct: ct).ConfigureAwait(false);
        return r ?? throw new ApiException("empty body on POST /v1/Services/{sid}/PhoneNumbers", 200);
    }

    /// <summary>Fetch one pool entry by sid.</summary>
    public async Task<MessagingServicePhoneNumber> GetAsync(string sid, CancellationToken ct = default)
    {
        var r = await _transport.SendAsync<MessagingServicePhoneNumber>(HttpMethod.Get,
            BasePath + "/" + PathSegment.Encode(sid), ct: ct).ConfigureAwait(false);
        return r ?? throw new ApiException("empty body on GET /v1/Services/{sid}/PhoneNumbers/{pn}", 200);
    }

    /// <summary>Remove a number from the pool. The IncomingPhoneNumber itself stays on the
    /// account.</summary>
    public Task DeleteAsync(string sid, CancellationToken ct = default) =>
        _transport.SendNoContentAsync(HttpMethod.Delete, BasePath + "/" + PathSegment.Encode(sid), ct: ct);
}
