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
    internal MessagesResource(Transport transport) : base(transport) { }

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
        [EnumeratorCancellation] CancellationToken ct = default)
    {
        var page = await ListAsync(filter, ct).ConfigureAwait(false);
        while (true)
        {
            foreach (var item in page.Messages) yield return item;
            if (string.IsNullOrEmpty(page.NextPageUri)) yield break;
            page = await Transport.SendPageUriAsync<MessageList>(page.NextPageUri!, ct).ConfigureAwait(false)
                ?? throw new VoiceMLException("empty page while following next_page_uri");
        }
    }

    /// <summary>Dispatch an outbound SMS.</summary>
    /// <summary>Fetch the page a list response links to next, following its
    /// <see cref="Page.NextPageUri"/> verbatim. Returns <c>null</c> when the page is the last
    /// one.</summary>
    public Task<MessageList?> NextPageAsync(MessageList page, CancellationToken ct = default)
        => string.IsNullOrEmpty(page.NextPageUri)
            ? Task.FromResult<MessageList?>(null)
            : Transport.SendPageUriAsync<MessageList>(page.NextPageUri!, ct);

    /// <summary>Create messages. Route: Messages.</summary>
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

    /// <summary>Scope to a message's <c>/Media</c> sub-resource — the MMS attachments on
    /// the message.</summary>
    public MessageMediaResource Media(string messageSid) => new(Transport, messageSid);

    /// <summary>Submit delivery feedback for a message
    /// (<c>POST /2010-04-01/Accounts/{AccountSid}/Messages/{Sid}/Feedback.json</c>). Any
    /// outcome other than <see cref="VoiceML.MessageFeedbackOutcome.Confirmed"/> — including
    /// an omitted one — is echoed back as <c>unconfirmed</c>; feedback is not persisted.</summary>
    public async Task<MessageFeedback> CreateFeedbackAsync(string messageSid, CreateMessageFeedbackRequest request, CancellationToken ct = default)
    {
        var result = await Transport.SendAsync<MessageFeedback>(
            HttpMethod.Post,
            Path("Messages", messageSid, "Feedback"),
            formBody: request.ToForm(),
            ct: ct).ConfigureAwait(false);
        return result ?? throw new ApiException("empty body on POST /Messages/{sid}/Feedback", 201);
    }

    /// <summary>Delete a message resource from the account's store.</summary>
    public Task DeleteAsync(string messageSid, CancellationToken ct = default)
        => Transport.SendNoContentAsync(HttpMethod.Delete, Path("Messages", messageSid), ct: ct);
}

/// <summary>Operations on <c>/Messages/{MessageSid}/Media</c> — the media attachments on a
/// message. Obtain via <see cref="MessagesResource.Media"/>.</summary>
public sealed class MessageMediaResource : ResourceBase
{
    private readonly string _messageSid;

    internal MessageMediaResource(Transport transport, string messageSid) : base(transport)
    {
        _messageSid = messageSid;
    }

    /// <summary>List the media attachments on this message.</summary>
    public async Task<MediaList> ListAsync(ListMediaParams? filter = null, CancellationToken ct = default)
    {
        var p = filter ?? new ListMediaParams();
        var result = await Transport.SendAsync<MediaList>(
            HttpMethod.Get,
            Path("Messages", _messageSid, "Media"),
            queryParams: p.ToQuery(),
            ct: ct).ConfigureAwait(false);
        return result ?? new MediaList();
    }

    /// <summary>Fetch the page a list response links to next, following its
    /// <see cref="Page.NextPageUri"/> verbatim. Returns <c>null</c> when the page is the last
    /// one.</summary>
    public Task<MediaList?> NextPageAsync(MediaList page, CancellationToken ct = default)
        => string.IsNullOrEmpty(page.NextPageUri)
            ? Task.FromResult<MediaList?>(null)
            : Transport.SendPageUriAsync<MediaList>(page.NextPageUri!, ct);

    /// <summary>Fetch one media attachment by SID.</summary>
    public async Task<Media> GetAsync(string sid, CancellationToken ct = default)
    {
        var result = await Transport.SendAsync<Media>(
            HttpMethod.Get,
            Path("Messages", _messageSid, "Media", sid),
            ct: ct).ConfigureAwait(false);
        return result ?? throw new ApiException("empty body on GET /Messages/{msid}/Media/{sid}", 200);
    }

    /// <summary>Delete one media attachment.</summary>
    public Task DeleteAsync(string sid, CancellationToken ct = default)
        => Transport.SendNoContentAsync(
            HttpMethod.Delete, Path("Messages", _messageSid, "Media", sid), ct: ct);
}
