using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using VoiceML.Exceptions;
using VoiceML.Models;

namespace VoiceML.Resources;

/// <summary>Operations on <c>/Queues</c> and <c>/Queues/{sid}/Members</c>.</summary>
public sealed class QueuesResource : ResourceBase
{
    /// <summary>Construct with the shared transport.</summary>
    public QueuesResource(Transport transport) : base(transport) { }

    /// <summary>Create a queue.</summary>
    public async Task<Queue> CreateAsync(CreateQueueRequest request, CancellationToken ct = default)
    {
        var result = await Transport.SendAsync<Queue>(
            HttpMethod.Post,
            Path("Queues"),
            formBody: request.ToForm(),
            ct: ct).ConfigureAwait(false);
        return result ?? throw new ApiException("empty body on POST /Queues", 200);
    }

    /// <summary>List queues.</summary>
    public async Task<QueueList> ListAsync(CancellationToken ct = default)
    {
        var result = await Transport.SendAsync<QueueList>(
            HttpMethod.Get, Path("Queues"), ct: ct).ConfigureAwait(false);
        return result ?? new QueueList();
    }

    /// <summary>Fetch a queue by SID.</summary>
    public async Task<Queue> GetAsync(string queueSid, CancellationToken ct = default)
    {
        var result = await Transport.SendAsync<Queue>(
            HttpMethod.Get, Path("Queues", queueSid), ct: ct).ConfigureAwait(false);
        return result ?? throw new ApiException("empty body on GET /Queues/{sid}", 200);
    }

    /// <summary>Update a queue's name or max size.</summary>
    public async Task<Queue> UpdateAsync(string queueSid, UpdateQueueRequest request, CancellationToken ct = default)
    {
        var result = await Transport.SendAsync<Queue>(
            HttpMethod.Post,
            Path("Queues", queueSid),
            formBody: request.ToForm(),
            ct: ct).ConfigureAwait(false);
        return result ?? throw new ApiException("empty body on POST /Queues/{sid}", 200);
    }

    /// <summary>Delete a queue. Returns 409 if there are waiting members.</summary>
    public Task DeleteAsync(string queueSid, CancellationToken ct = default)
        => Transport.SendNoContentAsync(HttpMethod.Delete, Path("Queues", queueSid), ct: ct);

    /// <summary>List the members currently waiting in a queue.</summary>
    public async Task<QueueMemberList> ListMembersAsync(string queueSid, CancellationToken ct = default)
    {
        var result = await Transport.SendAsync<QueueMemberList>(
            HttpMethod.Get, Path("Queues", queueSid, "Members"), ct: ct).ConfigureAwait(false);
        return result ?? new QueueMemberList();
    }

    /// <summary>Peek at the member at the front of the queue (does not dequeue).</summary>
    public async Task<QueueMember> PeekFrontAsync(string queueSid, CancellationToken ct = default)
    {
        var result = await Transport.SendAsync<QueueMember>(
            HttpMethod.Get,
            Path("Queues", queueSid, "Members", "Front"),
            ct: ct).ConfigureAwait(false);
        return result ?? throw new ApiException("empty body on GET /Queues/{sid}/Members/Front", 200);
    }

    /// <summary>Dequeue the front-of-queue member and redirect them to <see cref="DequeueRequest.Url"/>.</summary>
    public async Task<QueueMember> DequeueFrontAsync(string queueSid, DequeueRequest request, CancellationToken ct = default)
    {
        var result = await Transport.SendAsync<QueueMember>(
            HttpMethod.Post,
            Path("Queues", queueSid, "Members", "Front"),
            formBody: request.ToForm(),
            ct: ct).ConfigureAwait(false);
        return result ?? throw new ApiException("empty body on POST /Queues/{sid}/Members/Front", 200);
    }

    /// <summary>Fetch a member by their Call SID.</summary>
    public async Task<QueueMember> GetMemberAsync(string queueSid, string callSid, CancellationToken ct = default)
    {
        var result = await Transport.SendAsync<QueueMember>(
            HttpMethod.Get,
            Path("Queues", queueSid, "Members", callSid),
            ct: ct).ConfigureAwait(false);
        return result ?? throw new ApiException("empty body on GET /Queues/{sid}/Members/{csid}", 200);
    }

    /// <summary>Dequeue a specific member and redirect them to <see cref="DequeueRequest.Url"/>.</summary>
    public async Task<QueueMember> DequeueMemberAsync(
        string queueSid, string callSid, DequeueRequest request, CancellationToken ct = default)
    {
        var result = await Transport.SendAsync<QueueMember>(
            HttpMethod.Post,
            Path("Queues", queueSid, "Members", callSid),
            formBody: request.ToForm(),
            ct: ct).ConfigureAwait(false);
        return result ?? throw new ApiException("empty body on POST /Queues/{sid}/Members/{csid}", 200);
    }
}
