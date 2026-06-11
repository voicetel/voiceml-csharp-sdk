using System.Collections.Generic;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using VoiceML.Exceptions;
using VoiceML.Models;

namespace VoiceML.Resources;

/// <summary>Operations on <c>/Messages</c> — VoiceML's Twilio-compatible outbound SMS surface.
/// The gateway is fire-and-forget today: <see cref="UpdateAsync"/> only honours <c>Body=""</c>
/// (redaction), and <c>Status=canceled</c> returns 21610.</summary>
public sealed class MessagesResource : ResourceBase
{
    /// <summary>Construct with the shared transport.</summary>
    public MessagesResource(Transport transport) : base(transport) { }

    /// <summary>List messages. Pass an empty <see cref="ListMessagesParams"/> for an unfiltered list.</summary>
    public async Task<MessageList> ListAsync(ListMessagesParams? filter = null, CancellationToken ct = default)
    {
        var p = filter ?? new ListMessagesParams();
        var result = await Transport.SendAsync<MessageList>(
            HttpMethod.Get,
            Path("Messages"),
            queryParams: p.ToQuery(),
            ct: ct).ConfigureAwait(false);
        return result ?? new MessageList();
    }

    /// <summary>Iterate through all messages across pages, yielding one <see cref="Message"/> at a time.
    /// Pass an empty <see cref="ListMessagesParams"/> for an unfiltered iteration.</summary>
    public async IAsyncEnumerable<Message> IterateAsync(
        ListMessagesParams? filter = null,
        int page = 0,
        int? pageSize = null,
        [EnumeratorCancellation] CancellationToken ct = default)
    {
        var p = filter ?? new ListMessagesParams();
        while (true)
        {
            var chunk = await ListAsync(
                p with { Page = page, PageSize = pageSize ?? p.PageSize },
                ct).ConfigureAwait(false);
            foreach (var item in chunk.Messages) yield return item;
            if (string.IsNullOrEmpty(chunk.NextPageUri) || chunk.Messages.Count == 0) yield break;
            page++;
        }
    }

    /// <summary>Dispatch an outbound SMS.</summary>
    public async Task<Message> CreateAsync(CreateMessageRequest request, CancellationToken ct = default)
    {
        var result = await Transport.SendAsync<Message>(
            HttpMethod.Post,
            Path("Messages"),
            formBody: request.ToForm(),
            ct: ct).ConfigureAwait(false);
        return result ?? throw new ApiException("empty body on POST /Messages", 200);
    }

    /// <summary>Fetch a single message by SID.</summary>
    public async Task<Message> FetchAsync(string messageSid, CancellationToken ct = default)
    {
        var result = await Transport.SendAsync<Message>(
            HttpMethod.Get,
            Path("Messages", messageSid),
            ct: ct).ConfigureAwait(false);
        return result ?? throw new ApiException("empty body on GET /Messages/{sid}", 200);
    }

    /// <summary>Mutate an existing message — redact <c>Body</c> (pass empty string) or attempt cancel.</summary>
    public async Task<Message> UpdateAsync(string messageSid, UpdateMessageRequest request, CancellationToken ct = default)
    {
        var result = await Transport.SendAsync<Message>(
            HttpMethod.Post,
            Path("Messages", messageSid),
            formBody: request.ToForm(),
            ct: ct).ConfigureAwait(false);
        return result ?? throw new ApiException("empty body on POST /Messages/{sid}", 200);
    }

    /// <summary>Delete a message resource from the account's store.</summary>
    public Task DeleteAsync(string messageSid, CancellationToken ct = default)
        => Transport.SendNoContentAsync(HttpMethod.Delete, Path("Messages", messageSid), ct: ct);
}
