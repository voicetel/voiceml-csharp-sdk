using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using VoiceML.Exceptions;
using VoiceML.Models;

namespace VoiceML.Resources;

// ===========================================================================
// Conversations v1 — Phase 4 service-scoped resources
// (conversations.twilio.com/v1/Services/{ChatServiceSid}/…)
//
// 15 resource families, 48 ops, all isolation-boundary scoped under a chat
// service sid (IS…). Surface entry: client.ConversationsV1.Services.Scope(IS…).
// ===========================================================================

/// <summary>Per-service Phase 4 surface — every operation under
/// <c>/v1/Services/{ChatServiceSid}/…</c>. Returned by
/// <c>client.ConversationsV1.Services.Scope(chatServiceSid)</c>.</summary>
public sealed class ConversationsV1ServiceScopeResource
{
    private readonly Transport _transport;
    private readonly string _chatServiceSid;

    public ConversationsV1ServiceConversationsResource Conversations { get; }
    public ConversationsV1ServiceRolesResource Roles { get; }
    public ConversationsV1ServiceUsersResource Users { get; }
    public ConversationsV1ServiceBindingsResource Bindings { get; }
    public ConversationsV1ServiceConfigurationResource Configuration { get; }
    public ConversationsV1ServiceParticipantConversationsResource ParticipantConversations { get; }
    public ConversationsV1ServiceConversationWithParticipantsResource ConversationWithParticipants { get; }

    internal ConversationsV1ServiceScopeResource(Transport transport, string chatServiceSid)
    {
        _transport = transport;
        _chatServiceSid = chatServiceSid;
        Conversations = new ConversationsV1ServiceConversationsResource(transport, chatServiceSid);
        Roles = new ConversationsV1ServiceRolesResource(transport, chatServiceSid);
        Users = new ConversationsV1ServiceUsersResource(transport, chatServiceSid);
        Bindings = new ConversationsV1ServiceBindingsResource(transport, chatServiceSid);
        Configuration = new ConversationsV1ServiceConfigurationResource(transport, chatServiceSid);
        ParticipantConversations = new ConversationsV1ServiceParticipantConversationsResource(transport, chatServiceSid);
        ConversationWithParticipants = new ConversationsV1ServiceConversationWithParticipantsResource(transport, chatServiceSid);
    }

    /// <summary>The Chat Service sid this scope targets.</summary>
    public string ChatServiceSid => _chatServiceSid;
}

// ---- /v1/Services/{IS…}/Conversations (+ scoped sub-resources) -------------

/// <summary>Operations on <c>/v1/Services/{ChatServiceSid}/Conversations</c> plus
/// factory accessors for the per-conversation sub-resources
/// (<see cref="Messages"/>, <see cref="Participants"/>, <see cref="Webhooks"/>).</summary>
public sealed class ConversationsV1ServiceConversationsResource
{
    private readonly Transport _transport;
    private readonly string _chatServiceSid;

    internal ConversationsV1ServiceConversationsResource(Transport transport, string chatServiceSid)
    {
        _transport = transport;
        _chatServiceSid = chatServiceSid;
    }

    private string BasePath => $"/v1/Services/{_chatServiceSid}/Conversations";

    public async Task<ConversationsV1ServiceConversation> CreateAsync(CreateServiceConversationRequest request, CancellationToken ct = default)
    {
        var r = await _transport.SendAsync<ConversationsV1ServiceConversation>(HttpMethod.Post,
            BasePath, formBody: request.ToForm(), ct: ct).ConfigureAwait(false);
        return r ?? throw new ApiException("empty body on POST /v1/Services/{is}/Conversations", 201);
    }

    public async Task<ConversationsV1ServiceConversationList> ListAsync(ListV1PageParams? filter = null, CancellationToken ct = default)
    {
        var p = filter ?? new ListV1PageParams();
        return await _transport.SendAsync<ConversationsV1ServiceConversationList>(HttpMethod.Get,
            BasePath, queryParams: p.ToQuery(), ct: ct).ConfigureAwait(false)
            ?? new ConversationsV1ServiceConversationList();
    }

    public async Task<ConversationsV1ServiceConversation> GetAsync(string conversationSid, CancellationToken ct = default)
    {
        var r = await _transport.SendAsync<ConversationsV1ServiceConversation>(HttpMethod.Get,
            $"{BasePath}/{conversationSid}", ct: ct).ConfigureAwait(false);
        return r ?? throw new ApiException("empty body on GET /v1/Services/{is}/Conversations/{sid}", 200);
    }

    public async Task<ConversationsV1ServiceConversation> UpdateAsync(string conversationSid, UpdateServiceConversationRequest request, CancellationToken ct = default)
    {
        var r = await _transport.SendAsync<ConversationsV1ServiceConversation>(HttpMethod.Post,
            $"{BasePath}/{conversationSid}", formBody: request.ToForm(), ct: ct).ConfigureAwait(false);
        return r ?? throw new ApiException("empty body on POST /v1/Services/{is}/Conversations/{sid}", 200);
    }

    public Task DeleteAsync(string conversationSid, CancellationToken ct = default) =>
        _transport.SendAsync<object>(HttpMethod.Delete, $"{BasePath}/{conversationSid}", ct: ct);

    /// <summary>Scope to a specific Conversation's <c>/Messages</c> sub-resource (service-scoped).</summary>
    public ConversationsV1ServiceMessagesScope Messages(string conversationSid) =>
        new(_transport, _chatServiceSid, conversationSid);

    /// <summary>Scope to a specific Conversation's <c>/Participants</c> sub-resource (service-scoped).</summary>
    public ConversationsV1ServiceParticipantsScope Participants(string conversationSid) =>
        new(_transport, _chatServiceSid, conversationSid);

    /// <summary>Scope to a specific Conversation's <c>/Webhooks</c> sub-resource (service-scoped).</summary>
    public ConversationsV1ServiceWebhooksScope Webhooks(string conversationSid) =>
        new(_transport, _chatServiceSid, conversationSid);
}

/// <summary>Operations on <c>/v1/Services/{IS…}/Conversations/{CH…}/Messages</c>.</summary>
public sealed class ConversationsV1ServiceMessagesScope
{
    private readonly Transport _transport;
    private readonly string _chatServiceSid;
    private readonly string _conversationSid;

    internal ConversationsV1ServiceMessagesScope(Transport transport, string chatServiceSid, string conversationSid)
    {
        _transport = transport;
        _chatServiceSid = chatServiceSid;
        _conversationSid = conversationSid;
    }

    private string BasePath => $"/v1/Services/{_chatServiceSid}/Conversations/{_conversationSid}/Messages";

    public async Task<ConversationsV1ServiceConversationMessage> CreateAsync(CreateServiceConversationMessageRequest request, CancellationToken ct = default)
    {
        var r = await _transport.SendAsync<ConversationsV1ServiceConversationMessage>(HttpMethod.Post,
            BasePath, formBody: request.ToForm(), ct: ct).ConfigureAwait(false);
        return r ?? throw new ApiException("empty body on POST .../Messages", 201);
    }

    public async Task<ConversationsV1ServiceConversationMessageList> ListAsync(ListV1PageParams? filter = null, CancellationToken ct = default)
    {
        var p = filter ?? new ListV1PageParams();
        return await _transport.SendAsync<ConversationsV1ServiceConversationMessageList>(HttpMethod.Get,
            BasePath, queryParams: p.ToQuery(), ct: ct).ConfigureAwait(false)
            ?? new ConversationsV1ServiceConversationMessageList();
    }

    public async Task<ConversationsV1ServiceConversationMessage> GetAsync(string messageSid, CancellationToken ct = default)
    {
        var r = await _transport.SendAsync<ConversationsV1ServiceConversationMessage>(HttpMethod.Get,
            $"{BasePath}/{messageSid}", ct: ct).ConfigureAwait(false);
        return r ?? throw new ApiException("empty body on GET .../Messages/{sid}", 200);
    }

    public async Task<ConversationsV1ServiceConversationMessage> UpdateAsync(string messageSid, UpdateServiceConversationMessageRequest request, CancellationToken ct = default)
    {
        var r = await _transport.SendAsync<ConversationsV1ServiceConversationMessage>(HttpMethod.Post,
            $"{BasePath}/{messageSid}", formBody: request.ToForm(), ct: ct).ConfigureAwait(false);
        return r ?? throw new ApiException("empty body on POST .../Messages/{sid}", 200);
    }

    public Task DeleteAsync(string messageSid, CancellationToken ct = default) =>
        _transport.SendAsync<object>(HttpMethod.Delete, $"{BasePath}/{messageSid}", ct: ct);

    /// <summary>Scope to a specific Message's <c>/Receipts</c> sub-resource (service-scoped).</summary>
    public ConversationsV1ServiceReceiptsScope Receipts(string messageSid) =>
        new(_transport, _chatServiceSid, _conversationSid, messageSid);
}

/// <summary>Operations on <c>/v1/Services/{IS…}/Conversations/{CH…}/Messages/{IM…}/Receipts</c> (read-only).</summary>
public sealed class ConversationsV1ServiceReceiptsScope
{
    private readonly Transport _transport;
    private readonly string _chatServiceSid;
    private readonly string _conversationSid;
    private readonly string _messageSid;

    internal ConversationsV1ServiceReceiptsScope(Transport transport, string chatServiceSid, string conversationSid, string messageSid)
    {
        _transport = transport;
        _chatServiceSid = chatServiceSid;
        _conversationSid = conversationSid;
        _messageSid = messageSid;
    }

    private string BasePath =>
        $"/v1/Services/{_chatServiceSid}/Conversations/{_conversationSid}/Messages/{_messageSid}/Receipts";

    public async Task<ConversationsV1ServiceConversationMessageReceiptList> ListAsync(ListV1PageParams? filter = null, CancellationToken ct = default)
    {
        var p = filter ?? new ListV1PageParams();
        return await _transport.SendAsync<ConversationsV1ServiceConversationMessageReceiptList>(HttpMethod.Get,
            BasePath, queryParams: p.ToQuery(), ct: ct).ConfigureAwait(false)
            ?? new ConversationsV1ServiceConversationMessageReceiptList();
    }

    public async Task<ConversationsV1ServiceConversationMessageReceipt> GetAsync(string receiptSid, CancellationToken ct = default)
    {
        var r = await _transport.SendAsync<ConversationsV1ServiceConversationMessageReceipt>(HttpMethod.Get,
            $"{BasePath}/{receiptSid}", ct: ct).ConfigureAwait(false);
        return r ?? throw new ApiException("empty body on GET .../Receipts/{sid}", 200);
    }
}

/// <summary>Operations on <c>/v1/Services/{IS…}/Conversations/{CH…}/Participants</c>.</summary>
public sealed class ConversationsV1ServiceParticipantsScope
{
    private readonly Transport _transport;
    private readonly string _chatServiceSid;
    private readonly string _conversationSid;

    internal ConversationsV1ServiceParticipantsScope(Transport transport, string chatServiceSid, string conversationSid)
    {
        _transport = transport;
        _chatServiceSid = chatServiceSid;
        _conversationSid = conversationSid;
    }

    private string BasePath => $"/v1/Services/{_chatServiceSid}/Conversations/{_conversationSid}/Participants";

    public async Task<ConversationsV1ServiceConversationParticipant> CreateAsync(CreateServiceConversationParticipantRequest request, CancellationToken ct = default)
    {
        var r = await _transport.SendAsync<ConversationsV1ServiceConversationParticipant>(HttpMethod.Post,
            BasePath, formBody: request.ToForm(), ct: ct).ConfigureAwait(false);
        return r ?? throw new ApiException("empty body on POST .../Participants", 201);
    }

    public async Task<ConversationsV1ServiceConversationParticipantList> ListAsync(ListV1PageParams? filter = null, CancellationToken ct = default)
    {
        var p = filter ?? new ListV1PageParams();
        return await _transport.SendAsync<ConversationsV1ServiceConversationParticipantList>(HttpMethod.Get,
            BasePath, queryParams: p.ToQuery(), ct: ct).ConfigureAwait(false)
            ?? new ConversationsV1ServiceConversationParticipantList();
    }

    public async Task<ConversationsV1ServiceConversationParticipant> GetAsync(string participantSid, CancellationToken ct = default)
    {
        var r = await _transport.SendAsync<ConversationsV1ServiceConversationParticipant>(HttpMethod.Get,
            $"{BasePath}/{participantSid}", ct: ct).ConfigureAwait(false);
        return r ?? throw new ApiException("empty body on GET .../Participants/{sid}", 200);
    }

    public async Task<ConversationsV1ServiceConversationParticipant> UpdateAsync(string participantSid, UpdateServiceConversationParticipantRequest request, CancellationToken ct = default)
    {
        var r = await _transport.SendAsync<ConversationsV1ServiceConversationParticipant>(HttpMethod.Post,
            $"{BasePath}/{participantSid}", formBody: request.ToForm(), ct: ct).ConfigureAwait(false);
        return r ?? throw new ApiException("empty body on POST .../Participants/{sid}", 200);
    }

    public Task DeleteAsync(string participantSid, CancellationToken ct = default) =>
        _transport.SendAsync<object>(HttpMethod.Delete, $"{BasePath}/{participantSid}", ct: ct);
}

/// <summary>Operations on <c>/v1/Services/{IS…}/Conversations/{CH…}/Webhooks</c> (conversation-scoped webhooks).</summary>
public sealed class ConversationsV1ServiceWebhooksScope
{
    private readonly Transport _transport;
    private readonly string _chatServiceSid;
    private readonly string _conversationSid;

    internal ConversationsV1ServiceWebhooksScope(Transport transport, string chatServiceSid, string conversationSid)
    {
        _transport = transport;
        _chatServiceSid = chatServiceSid;
        _conversationSid = conversationSid;
    }

    private string BasePath => $"/v1/Services/{_chatServiceSid}/Conversations/{_conversationSid}/Webhooks";

    public async Task<ConversationsV1ServiceConversationScopedWebhook> CreateAsync(CreateServiceConversationScopedWebhookRequest request, CancellationToken ct = default)
    {
        var r = await _transport.SendAsync<ConversationsV1ServiceConversationScopedWebhook>(HttpMethod.Post,
            BasePath, formBody: request.ToForm(), ct: ct).ConfigureAwait(false);
        return r ?? throw new ApiException("empty body on POST .../Webhooks", 201);
    }

    public async Task<ConversationsV1ServiceConversationScopedWebhookList> ListAsync(ListV1PageParams? filter = null, CancellationToken ct = default)
    {
        var p = filter ?? new ListV1PageParams();
        return await _transport.SendAsync<ConversationsV1ServiceConversationScopedWebhookList>(HttpMethod.Get,
            BasePath, queryParams: p.ToQuery(), ct: ct).ConfigureAwait(false)
            ?? new ConversationsV1ServiceConversationScopedWebhookList();
    }

    public async Task<ConversationsV1ServiceConversationScopedWebhook> GetAsync(string webhookSid, CancellationToken ct = default)
    {
        var r = await _transport.SendAsync<ConversationsV1ServiceConversationScopedWebhook>(HttpMethod.Get,
            $"{BasePath}/{webhookSid}", ct: ct).ConfigureAwait(false);
        return r ?? throw new ApiException("empty body on GET .../Webhooks/{sid}", 200);
    }

    public async Task<ConversationsV1ServiceConversationScopedWebhook> UpdateAsync(string webhookSid, UpdateServiceConversationScopedWebhookRequest request, CancellationToken ct = default)
    {
        var r = await _transport.SendAsync<ConversationsV1ServiceConversationScopedWebhook>(HttpMethod.Post,
            $"{BasePath}/{webhookSid}", formBody: request.ToForm(), ct: ct).ConfigureAwait(false);
        return r ?? throw new ApiException("empty body on POST .../Webhooks/{sid}", 200);
    }

    public Task DeleteAsync(string webhookSid, CancellationToken ct = default) =>
        _transport.SendAsync<object>(HttpMethod.Delete, $"{BasePath}/{webhookSid}", ct: ct);
}

// ---- /v1/Services/{IS…}/Roles ---------------------------------------------

/// <summary>Operations on <c>/v1/Services/{ChatServiceSid}/Roles</c>.</summary>
public sealed class ConversationsV1ServiceRolesResource
{
    private readonly Transport _transport;
    private readonly string _chatServiceSid;

    internal ConversationsV1ServiceRolesResource(Transport transport, string chatServiceSid)
    {
        _transport = transport;
        _chatServiceSid = chatServiceSid;
    }

    private string BasePath => $"/v1/Services/{_chatServiceSid}/Roles";

    public async Task<ConversationsV1ServiceRole> CreateAsync(CreateServiceRoleRequest request, CancellationToken ct = default)
    {
        var r = await _transport.SendAsync<ConversationsV1ServiceRole>(HttpMethod.Post,
            BasePath, formBody: request.ToForm(), ct: ct).ConfigureAwait(false);
        return r ?? throw new ApiException("empty body on POST .../Roles", 201);
    }

    public async Task<ConversationsV1ServiceRoleList> ListAsync(ListV1PageParams? filter = null, CancellationToken ct = default)
    {
        var p = filter ?? new ListV1PageParams();
        return await _transport.SendAsync<ConversationsV1ServiceRoleList>(HttpMethod.Get,
            BasePath, queryParams: p.ToQuery(), ct: ct).ConfigureAwait(false)
            ?? new ConversationsV1ServiceRoleList();
    }

    public async Task<ConversationsV1ServiceRole> GetAsync(string sid, CancellationToken ct = default)
    {
        var r = await _transport.SendAsync<ConversationsV1ServiceRole>(HttpMethod.Get,
            $"{BasePath}/{sid}", ct: ct).ConfigureAwait(false);
        return r ?? throw new ApiException("empty body on GET .../Roles/{sid}", 200);
    }

    public async Task<ConversationsV1ServiceRole> UpdateAsync(string sid, UpdateServiceRoleRequest request, CancellationToken ct = default)
    {
        var r = await _transport.SendAsync<ConversationsV1ServiceRole>(HttpMethod.Post,
            $"{BasePath}/{sid}", formBody: request.ToForm(), ct: ct).ConfigureAwait(false);
        return r ?? throw new ApiException("empty body on POST .../Roles/{sid}", 200);
    }

    public Task DeleteAsync(string sid, CancellationToken ct = default) =>
        _transport.SendAsync<object>(HttpMethod.Delete, $"{BasePath}/{sid}", ct: ct);
}

// ---- /v1/Services/{IS…}/Users (+ /Conversations scope) --------------------

/// <summary>Operations on <c>/v1/Services/{ChatServiceSid}/Users</c>. Use
/// <see cref="Conversations(string)"/> to access the per-user UserConversations list.</summary>
public sealed class ConversationsV1ServiceUsersResource
{
    private readonly Transport _transport;
    private readonly string _chatServiceSid;

    internal ConversationsV1ServiceUsersResource(Transport transport, string chatServiceSid)
    {
        _transport = transport;
        _chatServiceSid = chatServiceSid;
    }

    private string BasePath => $"/v1/Services/{_chatServiceSid}/Users";

    public async Task<ConversationsV1ServiceUser> CreateAsync(CreateServiceUserRequest request, CancellationToken ct = default)
    {
        var r = await _transport.SendAsync<ConversationsV1ServiceUser>(HttpMethod.Post,
            BasePath, formBody: request.ToForm(), ct: ct).ConfigureAwait(false);
        return r ?? throw new ApiException("empty body on POST .../Users", 201);
    }

    public async Task<ConversationsV1ServiceUserList> ListAsync(ListV1PageParams? filter = null, CancellationToken ct = default)
    {
        var p = filter ?? new ListV1PageParams();
        return await _transport.SendAsync<ConversationsV1ServiceUserList>(HttpMethod.Get,
            BasePath, queryParams: p.ToQuery(), ct: ct).ConfigureAwait(false)
            ?? new ConversationsV1ServiceUserList();
    }

    public async Task<ConversationsV1ServiceUser> GetAsync(string sid, CancellationToken ct = default)
    {
        var r = await _transport.SendAsync<ConversationsV1ServiceUser>(HttpMethod.Get,
            $"{BasePath}/{sid}", ct: ct).ConfigureAwait(false);
        return r ?? throw new ApiException("empty body on GET .../Users/{sid}", 200);
    }

    public async Task<ConversationsV1ServiceUser> UpdateAsync(string sid, UpdateServiceUserRequest request, CancellationToken ct = default)
    {
        var r = await _transport.SendAsync<ConversationsV1ServiceUser>(HttpMethod.Post,
            $"{BasePath}/{sid}", formBody: request.ToForm(), ct: ct).ConfigureAwait(false);
        return r ?? throw new ApiException("empty body on POST .../Users/{sid}", 200);
    }

    public Task DeleteAsync(string sid, CancellationToken ct = default) =>
        _transport.SendAsync<object>(HttpMethod.Delete, $"{BasePath}/{sid}", ct: ct);

    /// <summary>Scope to a specific User's <c>/Conversations</c> (UserConversation) sub-resource (service-scoped, list-only).</summary>
    public ConversationsV1ServiceUserConversationsScope Conversations(string userSid) =>
        new(_transport, _chatServiceSid, userSid);
}

/// <summary>Operations on <c>/v1/Services/{IS…}/Users/{US…}/Conversations</c> — list-only.</summary>
public sealed class ConversationsV1ServiceUserConversationsScope
{
    private readonly Transport _transport;
    private readonly string _chatServiceSid;
    private readonly string _userSid;

    internal ConversationsV1ServiceUserConversationsScope(Transport transport, string chatServiceSid, string userSid)
    {
        _transport = transport;
        _chatServiceSid = chatServiceSid;
        _userSid = userSid;
    }

    private string BasePath => $"/v1/Services/{_chatServiceSid}/Users/{_userSid}/Conversations";

    public async Task<ConversationsV1ServiceUserConversationList> ListAsync(ListV1PageParams? filter = null, CancellationToken ct = default)
    {
        var p = filter ?? new ListV1PageParams();
        return await _transport.SendAsync<ConversationsV1ServiceUserConversationList>(HttpMethod.Get,
            BasePath, queryParams: p.ToQuery(), ct: ct).ConfigureAwait(false)
            ?? new ConversationsV1ServiceUserConversationList();
    }
}

// ---- /v1/Services/{IS…}/Bindings (read+delete) ----------------------------

/// <summary>Operations on <c>/v1/Services/{ChatServiceSid}/Bindings</c> — push Bindings (read+delete only).</summary>
public sealed class ConversationsV1ServiceBindingsResource
{
    private readonly Transport _transport;
    private readonly string _chatServiceSid;

    internal ConversationsV1ServiceBindingsResource(Transport transport, string chatServiceSid)
    {
        _transport = transport;
        _chatServiceSid = chatServiceSid;
    }

    private string BasePath => $"/v1/Services/{_chatServiceSid}/Bindings";

    public async Task<ConversationsV1ServiceBindingList> ListAsync(ListServiceBindingsParams? filter = null, CancellationToken ct = default)
    {
        var p = filter ?? new ListServiceBindingsParams();
        return await _transport.SendAsync<ConversationsV1ServiceBindingList>(HttpMethod.Get,
            BasePath, queryParams: p.ToQuery(), ct: ct).ConfigureAwait(false)
            ?? new ConversationsV1ServiceBindingList();
    }

    public async Task<ConversationsV1ServiceBinding> GetAsync(string sid, CancellationToken ct = default)
    {
        var r = await _transport.SendAsync<ConversationsV1ServiceBinding>(HttpMethod.Get,
            $"{BasePath}/{sid}", ct: ct).ConfigureAwait(false);
        return r ?? throw new ApiException("empty body on GET .../Bindings/{sid}", 200);
    }

    public Task DeleteAsync(string sid, CancellationToken ct = default) =>
        _transport.SendAsync<object>(HttpMethod.Delete, $"{BasePath}/{sid}", ct: ct);
}

// ---- /v1/Services/{IS…}/Configuration (+ /Notifications + /Webhooks) ------

/// <summary>Operations on the per-service <c>/v1/Services/{ChatServiceSid}/Configuration</c>
/// (singleton), plus its <see cref="Notifications"/> and <see cref="Webhooks"/> sub-singletons.</summary>
public sealed class ConversationsV1ServiceConfigurationResource
{
    private readonly Transport _transport;
    private readonly string _chatServiceSid;

    public ConversationsV1ServiceNotificationResource Notifications { get; }
    public ConversationsV1ServiceWebhookConfigurationResource Webhooks { get; }

    internal ConversationsV1ServiceConfigurationResource(Transport transport, string chatServiceSid)
    {
        _transport = transport;
        _chatServiceSid = chatServiceSid;
        Notifications = new ConversationsV1ServiceNotificationResource(transport, chatServiceSid);
        Webhooks = new ConversationsV1ServiceWebhookConfigurationResource(transport, chatServiceSid);
    }

    private string BasePath => $"/v1/Services/{_chatServiceSid}/Configuration";

    public async Task<ConversationsV1ServiceConfiguration> FetchAsync(CancellationToken ct = default)
    {
        var r = await _transport.SendAsync<ConversationsV1ServiceConfiguration>(HttpMethod.Get,
            BasePath, ct: ct).ConfigureAwait(false);
        return r ?? throw new ApiException("empty body on GET .../Configuration", 200);
    }

    public async Task<ConversationsV1ServiceConfiguration> UpdateAsync(UpdateServiceConfigurationRequest request, CancellationToken ct = default)
    {
        var r = await _transport.SendAsync<ConversationsV1ServiceConfiguration>(HttpMethod.Post,
            BasePath, formBody: request.ToForm(), ct: ct).ConfigureAwait(false);
        return r ?? throw new ApiException("empty body on POST .../Configuration", 200);
    }
}

/// <summary>Operations on <c>/v1/Services/{ChatServiceSid}/Configuration/Notifications</c> (singleton, no sid).</summary>
public sealed class ConversationsV1ServiceNotificationResource
{
    private readonly Transport _transport;
    private readonly string _chatServiceSid;

    internal ConversationsV1ServiceNotificationResource(Transport transport, string chatServiceSid)
    {
        _transport = transport;
        _chatServiceSid = chatServiceSid;
    }

    private string BasePath => $"/v1/Services/{_chatServiceSid}/Configuration/Notifications";

    public async Task<ConversationsV1ServiceNotification> FetchAsync(CancellationToken ct = default)
    {
        var r = await _transport.SendAsync<ConversationsV1ServiceNotification>(HttpMethod.Get,
            BasePath, ct: ct).ConfigureAwait(false);
        return r ?? throw new ApiException("empty body on GET .../Configuration/Notifications", 200);
    }

    public async Task<ConversationsV1ServiceNotification> UpdateAsync(UpdateServiceNotificationRequest request, CancellationToken ct = default)
    {
        var r = await _transport.SendAsync<ConversationsV1ServiceNotification>(HttpMethod.Post,
            BasePath, formBody: request.ToForm(), ct: ct).ConfigureAwait(false);
        return r ?? throw new ApiException("empty body on POST .../Configuration/Notifications", 200);
    }
}

/// <summary>Operations on <c>/v1/Services/{ChatServiceSid}/Configuration/Webhooks</c> (singleton, no sid).</summary>
public sealed class ConversationsV1ServiceWebhookConfigurationResource
{
    private readonly Transport _transport;
    private readonly string _chatServiceSid;

    internal ConversationsV1ServiceWebhookConfigurationResource(Transport transport, string chatServiceSid)
    {
        _transport = transport;
        _chatServiceSid = chatServiceSid;
    }

    private string BasePath => $"/v1/Services/{_chatServiceSid}/Configuration/Webhooks";

    public async Task<ConversationsV1ServiceWebhookConfiguration> FetchAsync(CancellationToken ct = default)
    {
        var r = await _transport.SendAsync<ConversationsV1ServiceWebhookConfiguration>(HttpMethod.Get,
            BasePath, ct: ct).ConfigureAwait(false);
        return r ?? throw new ApiException("empty body on GET .../Configuration/Webhooks", 200);
    }

    public async Task<ConversationsV1ServiceWebhookConfiguration> UpdateAsync(UpdateServiceWebhookConfigurationRequest request, CancellationToken ct = default)
    {
        var r = await _transport.SendAsync<ConversationsV1ServiceWebhookConfiguration>(HttpMethod.Post,
            BasePath, formBody: request.ToForm(), ct: ct).ConfigureAwait(false);
        return r ?? throw new ApiException("empty body on POST .../Configuration/Webhooks", 200);
    }
}

// ---- /v1/Services/{IS…}/ParticipantConversations + /ConversationWithParticipants

/// <summary>Operations on <c>/v1/Services/{ChatServiceSid}/ParticipantConversations</c> (read-only — list only).</summary>
public sealed class ConversationsV1ServiceParticipantConversationsResource
{
    private readonly Transport _transport;
    private readonly string _chatServiceSid;

    internal ConversationsV1ServiceParticipantConversationsResource(Transport transport, string chatServiceSid)
    {
        _transport = transport;
        _chatServiceSid = chatServiceSid;
    }

    private string BasePath => $"/v1/Services/{_chatServiceSid}/ParticipantConversations";

    public async Task<ConversationsV1ServiceParticipantConversationList> ListAsync(ListServiceParticipantConversationsParams? filter = null, CancellationToken ct = default)
    {
        var p = filter ?? new ListServiceParticipantConversationsParams();
        return await _transport.SendAsync<ConversationsV1ServiceParticipantConversationList>(HttpMethod.Get,
            BasePath, queryParams: p.ToQuery(), ct: ct).ConfigureAwait(false)
            ?? new ConversationsV1ServiceParticipantConversationList();
    }
}

/// <summary>Operations on <c>/v1/Services/{ChatServiceSid}/ConversationWithParticipants</c> (create-only — one-call composite).</summary>
public sealed class ConversationsV1ServiceConversationWithParticipantsResource
{
    private readonly Transport _transport;
    private readonly string _chatServiceSid;

    internal ConversationsV1ServiceConversationWithParticipantsResource(Transport transport, string chatServiceSid)
    {
        _transport = transport;
        _chatServiceSid = chatServiceSid;
    }

    private string BasePath => $"/v1/Services/{_chatServiceSid}/ConversationWithParticipants";

    public async Task<ConversationsV1ServiceConversationWithParticipants> CreateAsync(CreateServiceConversationWithParticipantsRequest request, CancellationToken ct = default)
    {
        var r = await _transport.SendAsync<ConversationsV1ServiceConversationWithParticipants>(HttpMethod.Post,
            BasePath, formBody: request.ToForm(), ct: ct).ConfigureAwait(false);
        return r ?? throw new ApiException("empty body on POST .../ConversationWithParticipants", 201);
    }
}
