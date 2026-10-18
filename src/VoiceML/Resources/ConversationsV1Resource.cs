using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using VoiceML.Exceptions;
using VoiceML.Models;

namespace VoiceML.Resources;

/// <summary>Top-level <c>/v1/*</c> Conversations surface — Twilio Conversations v1.
/// Account resolves from HTTP Basic auth; dates are ISO-8601; list responses use
/// the <see cref="VoiceV1Meta"/> envelope.
/// <para>Sub-resources: <see cref="Conversations"/>, <see cref="Roles"/>,
/// <see cref="Users"/>, <see cref="Credentials"/>, <see cref="Configuration"/>,
/// <see cref="ParticipantConversations"/>, <see cref="ConversationWithParticipants"/>,
/// <see cref="Services"/>.</para>
/// </summary>
public sealed class ConversationsV1Resource
{
    /// <summary>Create and manage messaging threads and their participants, messages and webhooks.</summary>
    public ConversationsV1ConversationsResource Conversations { get; }
    /// <summary>Manage permission sets assigned to Conversation users and participants.</summary>
    public ConversationsV1RolesResource Roles { get; }
    /// <summary>Manage Conversation user identities and their Conversation memberships.</summary>
    public ConversationsV1UsersResource Users { get; }
    /// <summary>Manage provider credentials used by Conversation messaging channels.</summary>
    public ConversationsV1CredentialsResource Credentials { get; }
    /// <summary>Channel or webhook configuration, preserving the service-specific JSON structure.</summary>
    public ConversationsV1ConfigurationResource Configuration { get; }
    /// <summary>Find Conversations associated with a participant identity or messaging address.</summary>
    public ConversationsV1ParticipantConversationsResource ParticipantConversations { get; }
    /// <summary>Create a Conversation together with its initial participants.</summary>
    public ConversationsV1ConversationWithParticipantsResource ConversationWithParticipants { get; }
    /// <summary>Manage Conversation Services and their scoped resources.</summary>
    public ConversationsV1ServicesResource Services { get; }

    internal ConversationsV1Resource(Transport transport)
    {
        Conversations = new ConversationsV1ConversationsResource(transport);
        Roles = new ConversationsV1RolesResource(transport);
        Users = new ConversationsV1UsersResource(transport);
        Credentials = new ConversationsV1CredentialsResource(transport);
        Configuration = new ConversationsV1ConfigurationResource(transport);
        ParticipantConversations = new ConversationsV1ParticipantConversationsResource(transport);
        ConversationWithParticipants = new ConversationsV1ConversationWithParticipantsResource(transport);
        Services = new ConversationsV1ServicesResource(transport);
    }
}

// ---- /v1/Conversations (+ scoped sub-resources) ----------------------------

/// <summary>Operations on <c>/v1/Conversations</c> plus factory accessors for the
/// per-conversation sub-resources (<see cref="Messages"/>, <see cref="Participants"/>,
/// <see cref="Webhooks"/>).</summary>
public sealed class ConversationsV1ConversationsResource
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

    internal ConversationsV1ConversationsResource(Transport transport) { _transport = transport; }

    /// <summary>Create conversations. Route: /v1/Conversations.</summary>
    public async Task<ConversationsV1Conversation> CreateAsync(CreateConversationRequest request, CancellationToken ct = default)
    {
        var r = await _transport.SendAsync<ConversationsV1Conversation>(HttpMethod.Post,
            "/v1/Conversations", formBody: request.ToForm(), ct: ct).ConfigureAwait(false);
        return r ?? throw new ApiException("empty body on POST /v1/Conversations", 201);
    }

    /// <summary>List conversations. Route: /v1/Conversations.</summary>
    public async Task<ConversationsV1ConversationList> ListAsync(ListV1PageParams? filter = null, CancellationToken ct = default)
    {
        var p = filter ?? new ListV1PageParams();
        return await _transport.SendAsync<ConversationsV1ConversationList>(HttpMethod.Get,
            "/v1/Conversations", queryParams: p.ToQuery(), ct: ct).ConfigureAwait(false)
            ?? new ConversationsV1ConversationList();
    }

    /// <summary>Fetch conversations. Route: /v1/Conversations/{PathSegment.Encode(conversationSid)}.</summary>
    public async Task<ConversationsV1Conversation> GetAsync(string conversationSid, CancellationToken ct = default)
    {
        var r = await _transport.SendAsync<ConversationsV1Conversation>(HttpMethod.Get,
            $"/v1/Conversations/{PathSegment.Encode(conversationSid)}", ct: ct).ConfigureAwait(false);
        return r ?? throw new ApiException("empty body on GET /v1/Conversations/{sid}", 200);
    }

    /// <summary>Update conversations. Route: /v1/Conversations/{PathSegment.Encode(conversationSid)}.</summary>
    public async Task<ConversationsV1Conversation> UpdateAsync(string conversationSid, UpdateConversationRequest request, CancellationToken ct = default)
    {
        var r = await _transport.SendAsync<ConversationsV1Conversation>(HttpMethod.Post,
            $"/v1/Conversations/{PathSegment.Encode(conversationSid)}", formBody: request.ToForm(), ct: ct).ConfigureAwait(false);
        return r ?? throw new ApiException("empty body on POST /v1/Conversations/{sid}", 200);
    }

    /// <summary>Delete conversations. Route: /v1/Conversations/{PathSegment.Encode(conversationSid)}.</summary>
    public Task DeleteAsync(string conversationSid, CancellationToken ct = default) =>
        _transport.SendAsync<object>(HttpMethod.Delete, $"/v1/Conversations/{PathSegment.Encode(conversationSid)}", ct: ct);

    /// <summary>Scope to a specific Conversation's <c>/Messages</c> sub-resource.</summary>
    public ConversationsV1MessagesScope Messages(string conversationSid) => new(_transport, conversationSid);

    /// <summary>Scope to a specific Conversation's <c>/Participants</c> sub-resource.</summary>
    public ConversationsV1ParticipantsScope Participants(string conversationSid) => new(_transport, conversationSid);

    /// <summary>Scope to a specific Conversation's <c>/Webhooks</c> sub-resource.</summary>
    public ConversationsV1WebhooksScope Webhooks(string conversationSid) => new(_transport, conversationSid);
}

/// <summary>Operations on <c>/v1/Conversations/{ConversationSid}/Messages</c>.</summary>
public sealed class ConversationsV1MessagesScope
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

    private readonly string _conversationSid;

    internal ConversationsV1MessagesScope(Transport transport, string conversationSid)
    {
        _transport = transport;
        _conversationSid = conversationSid;
    }

    private string BasePath => $"/v1/Conversations/{PathSegment.Encode(_conversationSid)}/Messages";

    /// <summary>Create messages. Route: Messages Scope.</summary>
    public async Task<ConversationsV1ConversationMessage> CreateAsync(CreateConversationMessageRequest request, CancellationToken ct = default)
    {
        var r = await _transport.SendAsync<ConversationsV1ConversationMessage>(HttpMethod.Post,
            BasePath, formBody: request.ToForm(), ct: ct).ConfigureAwait(false);
        return r ?? throw new ApiException("empty body on POST .../Messages", 201);
    }

    /// <summary>List messages. Route: Messages Scope.</summary>
    public async Task<ConversationsV1ConversationMessageList> ListAsync(ListV1PageParams? filter = null, CancellationToken ct = default)
    {
        var p = filter ?? new ListV1PageParams();
        return await _transport.SendAsync<ConversationsV1ConversationMessageList>(HttpMethod.Get,
            BasePath, queryParams: p.ToQuery(), ct: ct).ConfigureAwait(false)
            ?? new ConversationsV1ConversationMessageList();
    }

    /// <summary>Fetch messages. Route: Messages Scope.</summary>
    public async Task<ConversationsV1ConversationMessage> GetAsync(string messageSid, CancellationToken ct = default)
    {
        var r = await _transport.SendAsync<ConversationsV1ConversationMessage>(HttpMethod.Get,
            $"{BasePath}/{PathSegment.Encode(messageSid)}", ct: ct).ConfigureAwait(false);
        return r ?? throw new ApiException("empty body on GET .../Messages/{sid}", 200);
    }

    /// <summary>Update messages. Route: Messages Scope.</summary>
    public async Task<ConversationsV1ConversationMessage> UpdateAsync(string messageSid, UpdateConversationMessageRequest request, CancellationToken ct = default)
    {
        var r = await _transport.SendAsync<ConversationsV1ConversationMessage>(HttpMethod.Post,
            $"{BasePath}/{PathSegment.Encode(messageSid)}", formBody: request.ToForm(), ct: ct).ConfigureAwait(false);
        return r ?? throw new ApiException("empty body on POST .../Messages/{sid}", 200);
    }

    /// <summary>Delete messages. Route: Messages Scope.</summary>
    public Task DeleteAsync(string messageSid, CancellationToken ct = default) =>
        _transport.SendAsync<object>(HttpMethod.Delete, $"{BasePath}/{PathSegment.Encode(messageSid)}", ct: ct);

    /// <summary>Scope to a specific Message's <c>/Receipts</c> sub-resource.</summary>
    public ConversationsV1ReceiptsScope Receipts(string messageSid) =>
        new(_transport, _conversationSid, messageSid);
}

/// <summary>Operations on <c>/v1/Conversations/{ConversationSid}/Messages/{MessageSid}/Receipts</c> (read-only).</summary>
public sealed class ConversationsV1ReceiptsScope
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

    private readonly string _conversationSid;
    private readonly string _messageSid;

    internal ConversationsV1ReceiptsScope(Transport transport, string conversationSid, string messageSid)
    {
        _transport = transport;
        _conversationSid = conversationSid;
        _messageSid = messageSid;
    }

    private string BasePath => $"/v1/Conversations/{PathSegment.Encode(_conversationSid)}/Messages/{PathSegment.Encode(_messageSid)}/Receipts";

    /// <summary>List receipts. Route: Receipts Scope.</summary>
    public async Task<ConversationsV1ConversationMessageReceiptList> ListAsync(ListV1PageParams? filter = null, CancellationToken ct = default)
    {
        var p = filter ?? new ListV1PageParams();
        return await _transport.SendAsync<ConversationsV1ConversationMessageReceiptList>(HttpMethod.Get,
            BasePath, queryParams: p.ToQuery(), ct: ct).ConfigureAwait(false)
            ?? new ConversationsV1ConversationMessageReceiptList();
    }

    /// <summary>Fetch receipts. Route: Receipts Scope.</summary>
    public async Task<ConversationsV1ConversationMessageReceipt> GetAsync(string receiptSid, CancellationToken ct = default)
    {
        var r = await _transport.SendAsync<ConversationsV1ConversationMessageReceipt>(HttpMethod.Get,
            $"{BasePath}/{PathSegment.Encode(receiptSid)}", ct: ct).ConfigureAwait(false);
        return r ?? throw new ApiException("empty body on GET .../Receipts/{sid}", 200);
    }
}

/// <summary>Operations on <c>/v1/Conversations/{ConversationSid}/Participants</c>.</summary>
public sealed class ConversationsV1ParticipantsScope
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

    private readonly string _conversationSid;

    internal ConversationsV1ParticipantsScope(Transport transport, string conversationSid)
    {
        _transport = transport;
        _conversationSid = conversationSid;
    }

    private string BasePath => $"/v1/Conversations/{PathSegment.Encode(_conversationSid)}/Participants";

    /// <summary>Create participants. Route: Participants Scope.</summary>
    public async Task<ConversationsV1ConversationParticipant> CreateAsync(CreateConversationParticipantRequest request, CancellationToken ct = default)
    {
        var r = await _transport.SendAsync<ConversationsV1ConversationParticipant>(HttpMethod.Post,
            BasePath, formBody: request.ToForm(), ct: ct).ConfigureAwait(false);
        return r ?? throw new ApiException("empty body on POST .../Participants", 201);
    }

    /// <summary>List participants. Route: Participants Scope.</summary>
    public async Task<ConversationsV1ConversationParticipantList> ListAsync(ListV1PageParams? filter = null, CancellationToken ct = default)
    {
        var p = filter ?? new ListV1PageParams();
        return await _transport.SendAsync<ConversationsV1ConversationParticipantList>(HttpMethod.Get,
            BasePath, queryParams: p.ToQuery(), ct: ct).ConfigureAwait(false)
            ?? new ConversationsV1ConversationParticipantList();
    }

    /// <summary>Fetch participants. Route: Participants Scope.</summary>
    public async Task<ConversationsV1ConversationParticipant> GetAsync(string participantSid, CancellationToken ct = default)
    {
        var r = await _transport.SendAsync<ConversationsV1ConversationParticipant>(HttpMethod.Get,
            $"{BasePath}/{PathSegment.Encode(participantSid)}", ct: ct).ConfigureAwait(false);
        return r ?? throw new ApiException("empty body on GET .../Participants/{sid}", 200);
    }

    /// <summary>Update participants. Route: Participants Scope.</summary>
    public async Task<ConversationsV1ConversationParticipant> UpdateAsync(string participantSid, UpdateConversationParticipantRequest request, CancellationToken ct = default)
    {
        var r = await _transport.SendAsync<ConversationsV1ConversationParticipant>(HttpMethod.Post,
            $"{BasePath}/{PathSegment.Encode(participantSid)}", formBody: request.ToForm(), ct: ct).ConfigureAwait(false);
        return r ?? throw new ApiException("empty body on POST .../Participants/{sid}", 200);
    }

    /// <summary>Delete participants. Route: Participants Scope.</summary>
    public Task DeleteAsync(string participantSid, CancellationToken ct = default) =>
        _transport.SendAsync<object>(HttpMethod.Delete, $"{BasePath}/{PathSegment.Encode(participantSid)}", ct: ct);
}

/// <summary>Operations on <c>/v1/Conversations/{ConversationSid}/Webhooks</c> (conversation-scoped webhooks).</summary>
public sealed class ConversationsV1WebhooksScope
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

    private readonly string _conversationSid;

    internal ConversationsV1WebhooksScope(Transport transport, string conversationSid)
    {
        _transport = transport;
        _conversationSid = conversationSid;
    }

    private string BasePath => $"/v1/Conversations/{PathSegment.Encode(_conversationSid)}/Webhooks";

    /// <summary>Create webhooks. Route: Webhooks Scope.</summary>
    public async Task<ConversationsV1ConversationScopedWebhook> CreateAsync(CreateConversationScopedWebhookRequest request, CancellationToken ct = default)
    {
        var r = await _transport.SendAsync<ConversationsV1ConversationScopedWebhook>(HttpMethod.Post,
            BasePath, formBody: request.ToForm(), ct: ct).ConfigureAwait(false);
        return r ?? throw new ApiException("empty body on POST .../Webhooks", 201);
    }

    /// <summary>List webhooks. Route: Webhooks Scope.</summary>
    public async Task<ConversationsV1ConversationScopedWebhookList> ListAsync(ListV1PageParams? filter = null, CancellationToken ct = default)
    {
        var p = filter ?? new ListV1PageParams();
        return await _transport.SendAsync<ConversationsV1ConversationScopedWebhookList>(HttpMethod.Get,
            BasePath, queryParams: p.ToQuery(), ct: ct).ConfigureAwait(false)
            ?? new ConversationsV1ConversationScopedWebhookList();
    }

    /// <summary>Fetch webhooks. Route: Webhooks Scope.</summary>
    public async Task<ConversationsV1ConversationScopedWebhook> GetAsync(string webhookSid, CancellationToken ct = default)
    {
        var r = await _transport.SendAsync<ConversationsV1ConversationScopedWebhook>(HttpMethod.Get,
            $"{BasePath}/{PathSegment.Encode(webhookSid)}", ct: ct).ConfigureAwait(false);
        return r ?? throw new ApiException("empty body on GET .../Webhooks/{sid}", 200);
    }

    /// <summary>Update webhooks. Route: Webhooks Scope.</summary>
    public async Task<ConversationsV1ConversationScopedWebhook> UpdateAsync(string webhookSid, UpdateConversationScopedWebhookRequest request, CancellationToken ct = default)
    {
        var r = await _transport.SendAsync<ConversationsV1ConversationScopedWebhook>(HttpMethod.Post,
            $"{BasePath}/{PathSegment.Encode(webhookSid)}", formBody: request.ToForm(), ct: ct).ConfigureAwait(false);
        return r ?? throw new ApiException("empty body on POST .../Webhooks/{sid}", 200);
    }

    /// <summary>Delete webhooks. Route: Webhooks Scope.</summary>
    public Task DeleteAsync(string webhookSid, CancellationToken ct = default) =>
        _transport.SendAsync<object>(HttpMethod.Delete, $"{BasePath}/{PathSegment.Encode(webhookSid)}", ct: ct);
}

// ---- /v1/Roles -------------------------------------------------------------

/// <summary>Operations on <c>/v1/Roles</c>.</summary>
public sealed class ConversationsV1RolesResource
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

    internal ConversationsV1RolesResource(Transport transport) { _transport = transport; }

    /// <summary>Create roles. Route: /v1/Roles.</summary>
    public async Task<ConversationsV1Role> CreateAsync(CreateConversationsRoleRequest request, CancellationToken ct = default)
    {
        var r = await _transport.SendAsync<ConversationsV1Role>(HttpMethod.Post,
            "/v1/Roles", formBody: request.ToForm(), ct: ct).ConfigureAwait(false);
        return r ?? throw new ApiException("empty body on POST /v1/Roles", 201);
    }

    /// <summary>List roles. Route: /v1/Roles.</summary>
    public async Task<ConversationsV1RoleList> ListAsync(ListV1PageParams? filter = null, CancellationToken ct = default)
    {
        var p = filter ?? new ListV1PageParams();
        return await _transport.SendAsync<ConversationsV1RoleList>(HttpMethod.Get,
            "/v1/Roles", queryParams: p.ToQuery(), ct: ct).ConfigureAwait(false)
            ?? new ConversationsV1RoleList();
    }

    /// <summary>Fetch roles. Route: /v1/Roles/{PathSegment.Encode(sid)}.</summary>
    public async Task<ConversationsV1Role> GetAsync(string sid, CancellationToken ct = default)
    {
        var r = await _transport.SendAsync<ConversationsV1Role>(HttpMethod.Get,
            $"/v1/Roles/{PathSegment.Encode(sid)}", ct: ct).ConfigureAwait(false);
        return r ?? throw new ApiException("empty body on GET /v1/Roles/{sid}", 200);
    }

    /// <summary>Update roles. Route: /v1/Roles/{PathSegment.Encode(sid)}.</summary>
    public async Task<ConversationsV1Role> UpdateAsync(string sid, UpdateConversationsRoleRequest request, CancellationToken ct = default)
    {
        var r = await _transport.SendAsync<ConversationsV1Role>(HttpMethod.Post,
            $"/v1/Roles/{PathSegment.Encode(sid)}", formBody: request.ToForm(), ct: ct).ConfigureAwait(false);
        return r ?? throw new ApiException("empty body on POST /v1/Roles/{sid}", 200);
    }

    /// <summary>Delete roles. Route: /v1/Roles/{PathSegment.Encode(sid)}.</summary>
    public Task DeleteAsync(string sid, CancellationToken ct = default) =>
        _transport.SendAsync<object>(HttpMethod.Delete, $"/v1/Roles/{PathSegment.Encode(sid)}", ct: ct);
}

// ---- /v1/Users (+ /Conversations scope) -----------------------------------

/// <summary>Operations on <c>/v1/Users</c>.</summary>
public sealed class ConversationsV1UsersResource
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

    internal ConversationsV1UsersResource(Transport transport) { _transport = transport; }

    /// <summary>Create users. Route: /v1/Users.</summary>
    public async Task<ConversationsV1User> CreateAsync(CreateConversationsUserRequest request, CancellationToken ct = default)
    {
        var r = await _transport.SendAsync<ConversationsV1User>(HttpMethod.Post,
            "/v1/Users", formBody: request.ToForm(), ct: ct).ConfigureAwait(false);
        return r ?? throw new ApiException("empty body on POST /v1/Users", 201);
    }

    /// <summary>List users. Route: /v1/Users.</summary>
    public async Task<ConversationsV1UserList> ListAsync(ListV1PageParams? filter = null, CancellationToken ct = default)
    {
        var p = filter ?? new ListV1PageParams();
        return await _transport.SendAsync<ConversationsV1UserList>(HttpMethod.Get,
            "/v1/Users", queryParams: p.ToQuery(), ct: ct).ConfigureAwait(false)
            ?? new ConversationsV1UserList();
    }

    /// <summary>Fetch users. Route: /v1/Users/{PathSegment.Encode(sid)}.</summary>
    public async Task<ConversationsV1User> GetAsync(string sid, CancellationToken ct = default)
    {
        var r = await _transport.SendAsync<ConversationsV1User>(HttpMethod.Get,
            $"/v1/Users/{PathSegment.Encode(sid)}", ct: ct).ConfigureAwait(false);
        return r ?? throw new ApiException("empty body on GET /v1/Users/{sid}", 200);
    }

    /// <summary>Update users. Route: /v1/Users/{PathSegment.Encode(sid)}.</summary>
    public async Task<ConversationsV1User> UpdateAsync(string sid, UpdateConversationsUserRequest request, CancellationToken ct = default)
    {
        var r = await _transport.SendAsync<ConversationsV1User>(HttpMethod.Post,
            $"/v1/Users/{PathSegment.Encode(sid)}", formBody: request.ToForm(), ct: ct).ConfigureAwait(false);
        return r ?? throw new ApiException("empty body on POST /v1/Users/{sid}", 200);
    }

    /// <summary>Delete users. Route: /v1/Users/{PathSegment.Encode(sid)}.</summary>
    public Task DeleteAsync(string sid, CancellationToken ct = default) =>
        _transport.SendAsync<object>(HttpMethod.Delete, $"/v1/Users/{PathSegment.Encode(sid)}", ct: ct);

    /// <summary>Scope to a specific User's <c>/Conversations</c> (UserConversation) sub-resource.</summary>
    public ConversationsV1UserConversationsScope Conversations(string userSid) => new(_transport, userSid);
}

/// <summary>Operations on <c>/v1/Users/{UserSid}/Conversations</c> — read+update+delete only.</summary>
public sealed class ConversationsV1UserConversationsScope
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

    private readonly string _userSid;

    internal ConversationsV1UserConversationsScope(Transport transport, string userSid)
    {
        _transport = transport;
        _userSid = userSid;
    }

    private string BasePath => $"/v1/Users/{PathSegment.Encode(_userSid)}/Conversations";

    /// <summary>List user conversations. Route: User Conversations Scope.</summary>
    public async Task<ConversationsV1UserConversationList> ListAsync(ListV1PageParams? filter = null, CancellationToken ct = default)
    {
        var p = filter ?? new ListV1PageParams();
        return await _transport.SendAsync<ConversationsV1UserConversationList>(HttpMethod.Get,
            BasePath, queryParams: p.ToQuery(), ct: ct).ConfigureAwait(false)
            ?? new ConversationsV1UserConversationList();
    }

    /// <summary>Fetch user conversations. Route: User Conversations Scope.</summary>
    public async Task<ConversationsV1UserConversation> GetAsync(string conversationSid, CancellationToken ct = default)
    {
        var r = await _transport.SendAsync<ConversationsV1UserConversation>(HttpMethod.Get,
            $"{BasePath}/{PathSegment.Encode(conversationSid)}", ct: ct).ConfigureAwait(false);
        return r ?? throw new ApiException("empty body on GET /v1/Users/.../Conversations/{sid}", 200);
    }

    /// <summary>Update user conversations. Route: User Conversations Scope.</summary>
    public async Task<ConversationsV1UserConversation> UpdateAsync(string conversationSid, UpdateUserConversationRequest request, CancellationToken ct = default)
    {
        var r = await _transport.SendAsync<ConversationsV1UserConversation>(HttpMethod.Post,
            $"{BasePath}/{PathSegment.Encode(conversationSid)}", formBody: request.ToForm(), ct: ct).ConfigureAwait(false);
        return r ?? throw new ApiException("empty body on POST /v1/Users/.../Conversations/{sid}", 200);
    }

    /// <summary>Delete user conversations. Route: User Conversations Scope.</summary>
    public Task DeleteAsync(string conversationSid, CancellationToken ct = default) =>
        _transport.SendAsync<object>(HttpMethod.Delete, $"{BasePath}/{PathSegment.Encode(conversationSid)}", ct: ct);
}

// ---- /v1/Credentials -------------------------------------------------------

/// <summary>Operations on <c>/v1/Credentials</c> (push-notification credentials).</summary>
public sealed class ConversationsV1CredentialsResource
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

    internal ConversationsV1CredentialsResource(Transport transport) { _transport = transport; }

    /// <summary>Create credentials. Route: /v1/Credentials.</summary>
    public async Task<ConversationsV1Credential> CreateAsync(CreateConversationsCredentialRequest request, CancellationToken ct = default)
    {
        var r = await _transport.SendAsync<ConversationsV1Credential>(HttpMethod.Post,
            "/v1/Credentials", formBody: request.ToForm(), ct: ct).ConfigureAwait(false);
        return r ?? throw new ApiException("empty body on POST /v1/Credentials", 201);
    }

    /// <summary>List credentials. Route: /v1/Credentials.</summary>
    public async Task<ConversationsV1CredentialList> ListAsync(ListV1PageParams? filter = null, CancellationToken ct = default)
    {
        var p = filter ?? new ListV1PageParams();
        return await _transport.SendAsync<ConversationsV1CredentialList>(HttpMethod.Get,
            "/v1/Credentials", queryParams: p.ToQuery(), ct: ct).ConfigureAwait(false)
            ?? new ConversationsV1CredentialList();
    }

    /// <summary>Fetch credentials. Route: /v1/Credentials/{PathSegment.Encode(sid)}.</summary>
    public async Task<ConversationsV1Credential> GetAsync(string sid, CancellationToken ct = default)
    {
        var r = await _transport.SendAsync<ConversationsV1Credential>(HttpMethod.Get,
            $"/v1/Credentials/{PathSegment.Encode(sid)}", ct: ct).ConfigureAwait(false);
        return r ?? throw new ApiException("empty body on GET /v1/Credentials/{sid}", 200);
    }

    /// <summary>Update credentials. Route: /v1/Credentials/{PathSegment.Encode(sid)}.</summary>
    public async Task<ConversationsV1Credential> UpdateAsync(string sid, UpdateConversationsCredentialRequest request, CancellationToken ct = default)
    {
        var r = await _transport.SendAsync<ConversationsV1Credential>(HttpMethod.Post,
            $"/v1/Credentials/{PathSegment.Encode(sid)}", formBody: request.ToForm(), ct: ct).ConfigureAwait(false);
        return r ?? throw new ApiException("empty body on POST /v1/Credentials/{sid}", 200);
    }

    /// <summary>Delete credentials. Route: /v1/Credentials/{PathSegment.Encode(sid)}.</summary>
    public Task DeleteAsync(string sid, CancellationToken ct = default) =>
        _transport.SendAsync<object>(HttpMethod.Delete, $"/v1/Credentials/{PathSegment.Encode(sid)}", ct: ct);
}

// ---- /v1/Configuration (+ /Webhooks + /Addresses) -------------------------

/// <summary>Operations on the account-global <c>/v1/Configuration</c> (singleton),
/// plus the <see cref="Webhooks"/> sub-singleton and <see cref="Addresses"/> sub-collection.</summary>
public sealed class ConversationsV1ConfigurationResource
{
    private readonly Transport _transport;
    /// <summary>Configure callback URLs, HTTP methods and event filters for Conversation webhooks.</summary>
    public ConversationsV1ConfigurationWebhooksResource Webhooks { get; }
    /// <summary>Manage channel addresses and their automatic Conversation creation settings.</summary>
    public ConversationsV1ConfigAddressesResource Addresses { get; }

    internal ConversationsV1ConfigurationResource(Transport transport)
    {
        _transport = transport;
        Webhooks = new ConversationsV1ConfigurationWebhooksResource(transport);
        Addresses = new ConversationsV1ConfigAddressesResource(transport);
    }

    /// <summary>Fetch configuration. Route: /v1/Configuration.</summary>
    public async Task<ConversationsV1Configuration> FetchAsync(CancellationToken ct = default)
    {
        var r = await _transport.SendAsync<ConversationsV1Configuration>(HttpMethod.Get,
            "/v1/Configuration", ct: ct).ConfigureAwait(false);
        return r ?? throw new ApiException("empty body on GET /v1/Configuration", 200);
    }

    /// <summary>Update configuration. Route: /v1/Configuration.</summary>
    public async Task<ConversationsV1Configuration> UpdateAsync(UpdateConversationsConfigurationRequest request, CancellationToken ct = default)
    {
        var r = await _transport.SendAsync<ConversationsV1Configuration>(HttpMethod.Post,
            "/v1/Configuration", formBody: request.ToForm(), ct: ct).ConfigureAwait(false);
        return r ?? throw new ApiException("empty body on POST /v1/Configuration", 200);
    }
}

/// <summary>Operations on <c>/v1/Configuration/Webhooks</c> (singleton, no sid).</summary>
public sealed class ConversationsV1ConfigurationWebhooksResource
{
    private readonly Transport _transport;
    internal ConversationsV1ConfigurationWebhooksResource(Transport transport) { _transport = transport; }

    /// <summary>Fetch configuration webhooks. Route: /v1/Configuration/Webhooks.</summary>
    public async Task<ConversationsV1ConfigurationWebhook> FetchAsync(CancellationToken ct = default)
    {
        var r = await _transport.SendAsync<ConversationsV1ConfigurationWebhook>(HttpMethod.Get,
            "/v1/Configuration/Webhooks", ct: ct).ConfigureAwait(false);
        return r ?? throw new ApiException("empty body on GET /v1/Configuration/Webhooks", 200);
    }

    /// <summary>Update configuration webhooks. Route: /v1/Configuration/Webhooks.</summary>
    public async Task<ConversationsV1ConfigurationWebhook> UpdateAsync(UpdateConversationsConfigurationWebhookRequest request, CancellationToken ct = default)
    {
        var r = await _transport.SendAsync<ConversationsV1ConfigurationWebhook>(HttpMethod.Post,
            "/v1/Configuration/Webhooks", formBody: request.ToForm(), ct: ct).ConfigureAwait(false);
        return r ?? throw new ApiException("empty body on POST /v1/Configuration/Webhooks", 200);
    }
}

/// <summary>Operations on <c>/v1/Configuration/Addresses</c> (Configuration Address bindings).</summary>
public sealed class ConversationsV1ConfigAddressesResource
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

    internal ConversationsV1ConfigAddressesResource(Transport transport) { _transport = transport; }

    /// <summary>Create config addresses. Route: /v1/Configuration/Addresses.</summary>
    public async Task<ConversationsV1ConfigAddress> CreateAsync(CreateConfigAddressRequest request, CancellationToken ct = default)
    {
        var r = await _transport.SendAsync<ConversationsV1ConfigAddress>(HttpMethod.Post,
            "/v1/Configuration/Addresses", formBody: request.ToForm(), ct: ct).ConfigureAwait(false);
        return r ?? throw new ApiException("empty body on POST /v1/Configuration/Addresses", 201);
    }

    /// <summary>List config addresses. Route: /v1/Configuration/Addresses.</summary>
    public async Task<ConversationsV1ConfigAddressList> ListAsync(ListV1PageParams? filter = null, CancellationToken ct = default)
    {
        var p = filter ?? new ListV1PageParams();
        return await _transport.SendAsync<ConversationsV1ConfigAddressList>(HttpMethod.Get,
            "/v1/Configuration/Addresses", queryParams: p.ToQuery(), ct: ct).ConfigureAwait(false)
            ?? new ConversationsV1ConfigAddressList();
    }

    /// <summary>Fetch config addresses. Route: /v1/Configuration/Addresses/{PathSegment.Encode(sid)}.</summary>
    public async Task<ConversationsV1ConfigAddress> GetAsync(string sid, CancellationToken ct = default)
    {
        var r = await _transport.SendAsync<ConversationsV1ConfigAddress>(HttpMethod.Get,
            $"/v1/Configuration/Addresses/{PathSegment.Encode(sid)}", ct: ct).ConfigureAwait(false);
        return r ?? throw new ApiException("empty body on GET /v1/Configuration/Addresses/{sid}", 200);
    }

    /// <summary>Update config addresses. Route: /v1/Configuration/Addresses/{PathSegment.Encode(sid)}.</summary>
    public async Task<ConversationsV1ConfigAddress> UpdateAsync(string sid, UpdateConfigAddressRequest request, CancellationToken ct = default)
    {
        var r = await _transport.SendAsync<ConversationsV1ConfigAddress>(HttpMethod.Post,
            $"/v1/Configuration/Addresses/{PathSegment.Encode(sid)}", formBody: request.ToForm(), ct: ct).ConfigureAwait(false);
        return r ?? throw new ApiException("empty body on POST /v1/Configuration/Addresses/{sid}", 200);
    }

    /// <summary>Delete config addresses. Route: /v1/Configuration/Addresses/{PathSegment.Encode(sid)}.</summary>
    public Task DeleteAsync(string sid, CancellationToken ct = default) =>
        _transport.SendAsync<object>(HttpMethod.Delete, $"/v1/Configuration/Addresses/{PathSegment.Encode(sid)}", ct: ct);
}

// ---- /v1/ParticipantConversations + /v1/ConversationWithParticipants ------

/// <summary>Operations on <c>/v1/ParticipantConversations</c> (read-only — list only).</summary>
public sealed class ConversationsV1ParticipantConversationsResource
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

    internal ConversationsV1ParticipantConversationsResource(Transport transport) { _transport = transport; }

    /// <summary>List participant conversations. Route: /v1/ParticipantConversations.</summary>
    public async Task<ConversationsV1ParticipantConversationList> ListAsync(ListParticipantConversationsParams? filter = null, CancellationToken ct = default)
    {
        var p = filter ?? new ListParticipantConversationsParams();
        return await _transport.SendAsync<ConversationsV1ParticipantConversationList>(HttpMethod.Get,
            "/v1/ParticipantConversations", queryParams: p.ToQuery(), ct: ct).ConfigureAwait(false)
            ?? new ConversationsV1ParticipantConversationList();
    }
}

/// <summary>Operations on <c>/v1/ConversationWithParticipants</c> (create-only — one-call composite).</summary>
public sealed class ConversationsV1ConversationWithParticipantsResource
{
    private readonly Transport _transport;
    internal ConversationsV1ConversationWithParticipantsResource(Transport transport) { _transport = transport; }

    /// <summary>Create conversation with participants. Route: /v1/ConversationWithParticipants.</summary>
    public async Task<ConversationsV1ConversationWithParticipants> CreateAsync(CreateConversationWithParticipantsRequest request, CancellationToken ct = default)
    {
        var r = await _transport.SendAsync<ConversationsV1ConversationWithParticipants>(HttpMethod.Post,
            "/v1/ConversationWithParticipants", formBody: request.ToForm(), ct: ct).ConfigureAwait(false);
        return r ?? throw new ApiException("empty body on POST /v1/ConversationWithParticipants", 201);
    }
}

// ---- /v1/Services ---------------------------------------------------------

/// <summary>Operations on <c>/v1/Services</c> (Conversations Services — chat-service isolation boundaries).</summary>
public sealed class ConversationsV1ServicesResource
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

    internal ConversationsV1ServicesResource(Transport transport) { _transport = transport; }

    /// <summary>Create services. Route: /v1/Services.</summary>
    public async Task<ConversationsV1Service> CreateAsync(CreateConversationsServiceRequest request, CancellationToken ct = default)
    {
        var r = await _transport.SendAsync<ConversationsV1Service>(HttpMethod.Post,
            "/v1/Services", formBody: request.ToForm(), ct: ct).ConfigureAwait(false);
        return r ?? throw new ApiException("empty body on POST /v1/Services", 201);
    }

    /// <summary>List services. Route: /v1/Services.</summary>
    public async Task<ConversationsV1ServiceList> ListAsync(ListV1PageParams? filter = null, CancellationToken ct = default)
    {
        var p = filter ?? new ListV1PageParams();
        return await _transport.SendAsync<ConversationsV1ServiceList>(HttpMethod.Get,
            "/v1/Services", queryParams: p.ToQuery(), ct: ct).ConfigureAwait(false)
            ?? new ConversationsV1ServiceList();
    }

    /// <summary>Fetch services. Route: /v1/Services/{PathSegment.Encode(chatServiceSid)}.</summary>
    public async Task<ConversationsV1Service> GetAsync(string chatServiceSid, CancellationToken ct = default)
    {
        var r = await _transport.SendAsync<ConversationsV1Service>(HttpMethod.Get,
            $"/v1/Services/{PathSegment.Encode(chatServiceSid)}", ct: ct).ConfigureAwait(false);
        return r ?? throw new ApiException("empty body on GET /v1/Services/{sid}", 200);
    }

    /// <summary>Delete services. Route: /v1/Services/{PathSegment.Encode(chatServiceSid)}.</summary>
    public Task DeleteAsync(string chatServiceSid, CancellationToken ct = default) =>
        _transport.SendAsync<object>(HttpMethod.Delete, $"/v1/Services/{PathSegment.Encode(chatServiceSid)}", ct: ct);

    /// <summary>Scope to a specific Conversations Service (<c>IS…</c>) for the Phase 4
    /// service-scoped sub-resources: Conversations, Roles, Users, Bindings, Configuration,
    /// ParticipantConversations, ConversationWithParticipants. See
    /// <see cref="ConversationsV1ServiceScopeResource"/>.</summary>
    public ConversationsV1ServiceScopeResource Scope(string chatServiceSid) =>
        new(_transport, chatServiceSid);
}
