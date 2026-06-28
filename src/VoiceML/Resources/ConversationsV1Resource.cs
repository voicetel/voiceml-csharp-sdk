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
    public ConversationsV1ConversationsResource Conversations { get; }
    public ConversationsV1RolesResource Roles { get; }
    public ConversationsV1UsersResource Users { get; }
    public ConversationsV1CredentialsResource Credentials { get; }
    public ConversationsV1ConfigurationResource Configuration { get; }
    public ConversationsV1ParticipantConversationsResource ParticipantConversations { get; }
    public ConversationsV1ConversationWithParticipantsResource ConversationWithParticipants { get; }
    public ConversationsV1ServicesResource Services { get; }

    public ConversationsV1Resource(Transport transport)
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
    public ConversationsV1ConversationsResource(Transport transport) { _transport = transport; }

    public async Task<ConversationsV1Conversation> CreateAsync(CreateConversationRequest request, CancellationToken ct = default)
    {
        var r = await _transport.SendAsync<ConversationsV1Conversation>(HttpMethod.Post,
            "/v1/Conversations", formBody: request.ToForm(), ct: ct).ConfigureAwait(false);
        return r ?? throw new ApiException("empty body on POST /v1/Conversations", 201);
    }

    public async Task<ConversationsV1ConversationList> ListAsync(ListV1PageParams? filter = null, CancellationToken ct = default)
    {
        var p = filter ?? new ListV1PageParams();
        return await _transport.SendAsync<ConversationsV1ConversationList>(HttpMethod.Get,
            "/v1/Conversations", queryParams: p.ToQuery(), ct: ct).ConfigureAwait(false)
            ?? new ConversationsV1ConversationList();
    }

    public async Task<ConversationsV1Conversation> GetAsync(string conversationSid, CancellationToken ct = default)
    {
        var r = await _transport.SendAsync<ConversationsV1Conversation>(HttpMethod.Get,
            $"/v1/Conversations/{conversationSid}", ct: ct).ConfigureAwait(false);
        return r ?? throw new ApiException("empty body on GET /v1/Conversations/{sid}", 200);
    }

    public async Task<ConversationsV1Conversation> UpdateAsync(string conversationSid, UpdateConversationRequest request, CancellationToken ct = default)
    {
        var r = await _transport.SendAsync<ConversationsV1Conversation>(HttpMethod.Post,
            $"/v1/Conversations/{conversationSid}", formBody: request.ToForm(), ct: ct).ConfigureAwait(false);
        return r ?? throw new ApiException("empty body on POST /v1/Conversations/{sid}", 200);
    }

    public Task DeleteAsync(string conversationSid, CancellationToken ct = default) =>
        _transport.SendAsync<object>(HttpMethod.Delete, $"/v1/Conversations/{conversationSid}", ct: ct);

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
    private readonly string _conversationSid;

    internal ConversationsV1MessagesScope(Transport transport, string conversationSid)
    {
        _transport = transport;
        _conversationSid = conversationSid;
    }

    private string BasePath => $"/v1/Conversations/{_conversationSid}/Messages";

    public async Task<ConversationsV1ConversationMessage> CreateAsync(CreateConversationMessageRequest request, CancellationToken ct = default)
    {
        var r = await _transport.SendAsync<ConversationsV1ConversationMessage>(HttpMethod.Post,
            BasePath, formBody: request.ToForm(), ct: ct).ConfigureAwait(false);
        return r ?? throw new ApiException("empty body on POST .../Messages", 201);
    }

    public async Task<ConversationsV1ConversationMessageList> ListAsync(ListV1PageParams? filter = null, CancellationToken ct = default)
    {
        var p = filter ?? new ListV1PageParams();
        return await _transport.SendAsync<ConversationsV1ConversationMessageList>(HttpMethod.Get,
            BasePath, queryParams: p.ToQuery(), ct: ct).ConfigureAwait(false)
            ?? new ConversationsV1ConversationMessageList();
    }

    public async Task<ConversationsV1ConversationMessage> GetAsync(string messageSid, CancellationToken ct = default)
    {
        var r = await _transport.SendAsync<ConversationsV1ConversationMessage>(HttpMethod.Get,
            $"{BasePath}/{messageSid}", ct: ct).ConfigureAwait(false);
        return r ?? throw new ApiException("empty body on GET .../Messages/{sid}", 200);
    }

    public async Task<ConversationsV1ConversationMessage> UpdateAsync(string messageSid, UpdateConversationMessageRequest request, CancellationToken ct = default)
    {
        var r = await _transport.SendAsync<ConversationsV1ConversationMessage>(HttpMethod.Post,
            $"{BasePath}/{messageSid}", formBody: request.ToForm(), ct: ct).ConfigureAwait(false);
        return r ?? throw new ApiException("empty body on POST .../Messages/{sid}", 200);
    }

    public Task DeleteAsync(string messageSid, CancellationToken ct = default) =>
        _transport.SendAsync<object>(HttpMethod.Delete, $"{BasePath}/{messageSid}", ct: ct);

    /// <summary>Scope to a specific Message's <c>/Receipts</c> sub-resource.</summary>
    public ConversationsV1ReceiptsScope Receipts(string messageSid) =>
        new(_transport, _conversationSid, messageSid);
}

/// <summary>Operations on <c>/v1/Conversations/{ConversationSid}/Messages/{MessageSid}/Receipts</c> (read-only).</summary>
public sealed class ConversationsV1ReceiptsScope
{
    private readonly Transport _transport;
    private readonly string _conversationSid;
    private readonly string _messageSid;

    internal ConversationsV1ReceiptsScope(Transport transport, string conversationSid, string messageSid)
    {
        _transport = transport;
        _conversationSid = conversationSid;
        _messageSid = messageSid;
    }

    private string BasePath => $"/v1/Conversations/{_conversationSid}/Messages/{_messageSid}/Receipts";

    public async Task<ConversationsV1ConversationMessageReceiptList> ListAsync(ListV1PageParams? filter = null, CancellationToken ct = default)
    {
        var p = filter ?? new ListV1PageParams();
        return await _transport.SendAsync<ConversationsV1ConversationMessageReceiptList>(HttpMethod.Get,
            BasePath, queryParams: p.ToQuery(), ct: ct).ConfigureAwait(false)
            ?? new ConversationsV1ConversationMessageReceiptList();
    }

    public async Task<ConversationsV1ConversationMessageReceipt> GetAsync(string receiptSid, CancellationToken ct = default)
    {
        var r = await _transport.SendAsync<ConversationsV1ConversationMessageReceipt>(HttpMethod.Get,
            $"{BasePath}/{receiptSid}", ct: ct).ConfigureAwait(false);
        return r ?? throw new ApiException("empty body on GET .../Receipts/{sid}", 200);
    }
}

/// <summary>Operations on <c>/v1/Conversations/{ConversationSid}/Participants</c>.</summary>
public sealed class ConversationsV1ParticipantsScope
{
    private readonly Transport _transport;
    private readonly string _conversationSid;

    internal ConversationsV1ParticipantsScope(Transport transport, string conversationSid)
    {
        _transport = transport;
        _conversationSid = conversationSid;
    }

    private string BasePath => $"/v1/Conversations/{_conversationSid}/Participants";

    public async Task<ConversationsV1ConversationParticipant> CreateAsync(CreateConversationParticipantRequest request, CancellationToken ct = default)
    {
        var r = await _transport.SendAsync<ConversationsV1ConversationParticipant>(HttpMethod.Post,
            BasePath, formBody: request.ToForm(), ct: ct).ConfigureAwait(false);
        return r ?? throw new ApiException("empty body on POST .../Participants", 201);
    }

    public async Task<ConversationsV1ConversationParticipantList> ListAsync(ListV1PageParams? filter = null, CancellationToken ct = default)
    {
        var p = filter ?? new ListV1PageParams();
        return await _transport.SendAsync<ConversationsV1ConversationParticipantList>(HttpMethod.Get,
            BasePath, queryParams: p.ToQuery(), ct: ct).ConfigureAwait(false)
            ?? new ConversationsV1ConversationParticipantList();
    }

    public async Task<ConversationsV1ConversationParticipant> GetAsync(string participantSid, CancellationToken ct = default)
    {
        var r = await _transport.SendAsync<ConversationsV1ConversationParticipant>(HttpMethod.Get,
            $"{BasePath}/{participantSid}", ct: ct).ConfigureAwait(false);
        return r ?? throw new ApiException("empty body on GET .../Participants/{sid}", 200);
    }

    public async Task<ConversationsV1ConversationParticipant> UpdateAsync(string participantSid, UpdateConversationParticipantRequest request, CancellationToken ct = default)
    {
        var r = await _transport.SendAsync<ConversationsV1ConversationParticipant>(HttpMethod.Post,
            $"{BasePath}/{participantSid}", formBody: request.ToForm(), ct: ct).ConfigureAwait(false);
        return r ?? throw new ApiException("empty body on POST .../Participants/{sid}", 200);
    }

    public Task DeleteAsync(string participantSid, CancellationToken ct = default) =>
        _transport.SendAsync<object>(HttpMethod.Delete, $"{BasePath}/{participantSid}", ct: ct);
}

/// <summary>Operations on <c>/v1/Conversations/{ConversationSid}/Webhooks</c> (conversation-scoped webhooks).</summary>
public sealed class ConversationsV1WebhooksScope
{
    private readonly Transport _transport;
    private readonly string _conversationSid;

    internal ConversationsV1WebhooksScope(Transport transport, string conversationSid)
    {
        _transport = transport;
        _conversationSid = conversationSid;
    }

    private string BasePath => $"/v1/Conversations/{_conversationSid}/Webhooks";

    public async Task<ConversationsV1ConversationScopedWebhook> CreateAsync(CreateConversationScopedWebhookRequest request, CancellationToken ct = default)
    {
        var r = await _transport.SendAsync<ConversationsV1ConversationScopedWebhook>(HttpMethod.Post,
            BasePath, formBody: request.ToForm(), ct: ct).ConfigureAwait(false);
        return r ?? throw new ApiException("empty body on POST .../Webhooks", 201);
    }

    public async Task<ConversationsV1ConversationScopedWebhookList> ListAsync(ListV1PageParams? filter = null, CancellationToken ct = default)
    {
        var p = filter ?? new ListV1PageParams();
        return await _transport.SendAsync<ConversationsV1ConversationScopedWebhookList>(HttpMethod.Get,
            BasePath, queryParams: p.ToQuery(), ct: ct).ConfigureAwait(false)
            ?? new ConversationsV1ConversationScopedWebhookList();
    }

    public async Task<ConversationsV1ConversationScopedWebhook> GetAsync(string webhookSid, CancellationToken ct = default)
    {
        var r = await _transport.SendAsync<ConversationsV1ConversationScopedWebhook>(HttpMethod.Get,
            $"{BasePath}/{webhookSid}", ct: ct).ConfigureAwait(false);
        return r ?? throw new ApiException("empty body on GET .../Webhooks/{sid}", 200);
    }

    public async Task<ConversationsV1ConversationScopedWebhook> UpdateAsync(string webhookSid, UpdateConversationScopedWebhookRequest request, CancellationToken ct = default)
    {
        var r = await _transport.SendAsync<ConversationsV1ConversationScopedWebhook>(HttpMethod.Post,
            $"{BasePath}/{webhookSid}", formBody: request.ToForm(), ct: ct).ConfigureAwait(false);
        return r ?? throw new ApiException("empty body on POST .../Webhooks/{sid}", 200);
    }

    public Task DeleteAsync(string webhookSid, CancellationToken ct = default) =>
        _transport.SendAsync<object>(HttpMethod.Delete, $"{BasePath}/{webhookSid}", ct: ct);
}

// ---- /v1/Roles -------------------------------------------------------------

/// <summary>Operations on <c>/v1/Roles</c>.</summary>
public sealed class ConversationsV1RolesResource
{
    private readonly Transport _transport;
    public ConversationsV1RolesResource(Transport transport) { _transport = transport; }

    public async Task<ConversationsV1Role> CreateAsync(CreateConversationsRoleRequest request, CancellationToken ct = default)
    {
        var r = await _transport.SendAsync<ConversationsV1Role>(HttpMethod.Post,
            "/v1/Roles", formBody: request.ToForm(), ct: ct).ConfigureAwait(false);
        return r ?? throw new ApiException("empty body on POST /v1/Roles", 201);
    }

    public async Task<ConversationsV1RoleList> ListAsync(ListV1PageParams? filter = null, CancellationToken ct = default)
    {
        var p = filter ?? new ListV1PageParams();
        return await _transport.SendAsync<ConversationsV1RoleList>(HttpMethod.Get,
            "/v1/Roles", queryParams: p.ToQuery(), ct: ct).ConfigureAwait(false)
            ?? new ConversationsV1RoleList();
    }

    public async Task<ConversationsV1Role> GetAsync(string sid, CancellationToken ct = default)
    {
        var r = await _transport.SendAsync<ConversationsV1Role>(HttpMethod.Get,
            $"/v1/Roles/{sid}", ct: ct).ConfigureAwait(false);
        return r ?? throw new ApiException("empty body on GET /v1/Roles/{sid}", 200);
    }

    public async Task<ConversationsV1Role> UpdateAsync(string sid, UpdateConversationsRoleRequest request, CancellationToken ct = default)
    {
        var r = await _transport.SendAsync<ConversationsV1Role>(HttpMethod.Post,
            $"/v1/Roles/{sid}", formBody: request.ToForm(), ct: ct).ConfigureAwait(false);
        return r ?? throw new ApiException("empty body on POST /v1/Roles/{sid}", 200);
    }

    public Task DeleteAsync(string sid, CancellationToken ct = default) =>
        _transport.SendAsync<object>(HttpMethod.Delete, $"/v1/Roles/{sid}", ct: ct);
}

// ---- /v1/Users (+ /Conversations scope) -----------------------------------

/// <summary>Operations on <c>/v1/Users</c>.</summary>
public sealed class ConversationsV1UsersResource
{
    private readonly Transport _transport;
    public ConversationsV1UsersResource(Transport transport) { _transport = transport; }

    public async Task<ConversationsV1User> CreateAsync(CreateConversationsUserRequest request, CancellationToken ct = default)
    {
        var r = await _transport.SendAsync<ConversationsV1User>(HttpMethod.Post,
            "/v1/Users", formBody: request.ToForm(), ct: ct).ConfigureAwait(false);
        return r ?? throw new ApiException("empty body on POST /v1/Users", 201);
    }

    public async Task<ConversationsV1UserList> ListAsync(ListV1PageParams? filter = null, CancellationToken ct = default)
    {
        var p = filter ?? new ListV1PageParams();
        return await _transport.SendAsync<ConversationsV1UserList>(HttpMethod.Get,
            "/v1/Users", queryParams: p.ToQuery(), ct: ct).ConfigureAwait(false)
            ?? new ConversationsV1UserList();
    }

    public async Task<ConversationsV1User> GetAsync(string sid, CancellationToken ct = default)
    {
        var r = await _transport.SendAsync<ConversationsV1User>(HttpMethod.Get,
            $"/v1/Users/{sid}", ct: ct).ConfigureAwait(false);
        return r ?? throw new ApiException("empty body on GET /v1/Users/{sid}", 200);
    }

    public async Task<ConversationsV1User> UpdateAsync(string sid, UpdateConversationsUserRequest request, CancellationToken ct = default)
    {
        var r = await _transport.SendAsync<ConversationsV1User>(HttpMethod.Post,
            $"/v1/Users/{sid}", formBody: request.ToForm(), ct: ct).ConfigureAwait(false);
        return r ?? throw new ApiException("empty body on POST /v1/Users/{sid}", 200);
    }

    public Task DeleteAsync(string sid, CancellationToken ct = default) =>
        _transport.SendAsync<object>(HttpMethod.Delete, $"/v1/Users/{sid}", ct: ct);

    /// <summary>Scope to a specific User's <c>/Conversations</c> (UserConversation) sub-resource.</summary>
    public ConversationsV1UserConversationsScope Conversations(string userSid) => new(_transport, userSid);
}

/// <summary>Operations on <c>/v1/Users/{UserSid}/Conversations</c> — read+update+delete only.</summary>
public sealed class ConversationsV1UserConversationsScope
{
    private readonly Transport _transport;
    private readonly string _userSid;

    internal ConversationsV1UserConversationsScope(Transport transport, string userSid)
    {
        _transport = transport;
        _userSid = userSid;
    }

    private string BasePath => $"/v1/Users/{_userSid}/Conversations";

    public async Task<ConversationsV1UserConversationList> ListAsync(ListV1PageParams? filter = null, CancellationToken ct = default)
    {
        var p = filter ?? new ListV1PageParams();
        return await _transport.SendAsync<ConversationsV1UserConversationList>(HttpMethod.Get,
            BasePath, queryParams: p.ToQuery(), ct: ct).ConfigureAwait(false)
            ?? new ConversationsV1UserConversationList();
    }

    public async Task<ConversationsV1UserConversation> GetAsync(string conversationSid, CancellationToken ct = default)
    {
        var r = await _transport.SendAsync<ConversationsV1UserConversation>(HttpMethod.Get,
            $"{BasePath}/{conversationSid}", ct: ct).ConfigureAwait(false);
        return r ?? throw new ApiException("empty body on GET /v1/Users/.../Conversations/{sid}", 200);
    }

    public async Task<ConversationsV1UserConversation> UpdateAsync(string conversationSid, UpdateUserConversationRequest request, CancellationToken ct = default)
    {
        var r = await _transport.SendAsync<ConversationsV1UserConversation>(HttpMethod.Post,
            $"{BasePath}/{conversationSid}", formBody: request.ToForm(), ct: ct).ConfigureAwait(false);
        return r ?? throw new ApiException("empty body on POST /v1/Users/.../Conversations/{sid}", 200);
    }

    public Task DeleteAsync(string conversationSid, CancellationToken ct = default) =>
        _transport.SendAsync<object>(HttpMethod.Delete, $"{BasePath}/{conversationSid}", ct: ct);
}

// ---- /v1/Credentials -------------------------------------------------------

/// <summary>Operations on <c>/v1/Credentials</c> (push-notification credentials).</summary>
public sealed class ConversationsV1CredentialsResource
{
    private readonly Transport _transport;
    public ConversationsV1CredentialsResource(Transport transport) { _transport = transport; }

    public async Task<ConversationsV1Credential> CreateAsync(CreateConversationsCredentialRequest request, CancellationToken ct = default)
    {
        var r = await _transport.SendAsync<ConversationsV1Credential>(HttpMethod.Post,
            "/v1/Credentials", formBody: request.ToForm(), ct: ct).ConfigureAwait(false);
        return r ?? throw new ApiException("empty body on POST /v1/Credentials", 201);
    }

    public async Task<ConversationsV1CredentialList> ListAsync(ListV1PageParams? filter = null, CancellationToken ct = default)
    {
        var p = filter ?? new ListV1PageParams();
        return await _transport.SendAsync<ConversationsV1CredentialList>(HttpMethod.Get,
            "/v1/Credentials", queryParams: p.ToQuery(), ct: ct).ConfigureAwait(false)
            ?? new ConversationsV1CredentialList();
    }

    public async Task<ConversationsV1Credential> GetAsync(string sid, CancellationToken ct = default)
    {
        var r = await _transport.SendAsync<ConversationsV1Credential>(HttpMethod.Get,
            $"/v1/Credentials/{sid}", ct: ct).ConfigureAwait(false);
        return r ?? throw new ApiException("empty body on GET /v1/Credentials/{sid}", 200);
    }

    public async Task<ConversationsV1Credential> UpdateAsync(string sid, UpdateConversationsCredentialRequest request, CancellationToken ct = default)
    {
        var r = await _transport.SendAsync<ConversationsV1Credential>(HttpMethod.Post,
            $"/v1/Credentials/{sid}", formBody: request.ToForm(), ct: ct).ConfigureAwait(false);
        return r ?? throw new ApiException("empty body on POST /v1/Credentials/{sid}", 200);
    }

    public Task DeleteAsync(string sid, CancellationToken ct = default) =>
        _transport.SendAsync<object>(HttpMethod.Delete, $"/v1/Credentials/{sid}", ct: ct);
}

// ---- /v1/Configuration (+ /Webhooks + /Addresses) -------------------------

/// <summary>Operations on the account-global <c>/v1/Configuration</c> (singleton),
/// plus the <see cref="Webhooks"/> sub-singleton and <see cref="Addresses"/> sub-collection.</summary>
public sealed class ConversationsV1ConfigurationResource
{
    private readonly Transport _transport;
    public ConversationsV1ConfigurationWebhooksResource Webhooks { get; }
    public ConversationsV1ConfigAddressesResource Addresses { get; }

    public ConversationsV1ConfigurationResource(Transport transport)
    {
        _transport = transport;
        Webhooks = new ConversationsV1ConfigurationWebhooksResource(transport);
        Addresses = new ConversationsV1ConfigAddressesResource(transport);
    }

    public async Task<ConversationsV1Configuration> FetchAsync(CancellationToken ct = default)
    {
        var r = await _transport.SendAsync<ConversationsV1Configuration>(HttpMethod.Get,
            "/v1/Configuration", ct: ct).ConfigureAwait(false);
        return r ?? throw new ApiException("empty body on GET /v1/Configuration", 200);
    }

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
    public ConversationsV1ConfigurationWebhooksResource(Transport transport) { _transport = transport; }

    public async Task<ConversationsV1ConfigurationWebhook> FetchAsync(CancellationToken ct = default)
    {
        var r = await _transport.SendAsync<ConversationsV1ConfigurationWebhook>(HttpMethod.Get,
            "/v1/Configuration/Webhooks", ct: ct).ConfigureAwait(false);
        return r ?? throw new ApiException("empty body on GET /v1/Configuration/Webhooks", 200);
    }

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
    public ConversationsV1ConfigAddressesResource(Transport transport) { _transport = transport; }

    public async Task<ConversationsV1ConfigAddress> CreateAsync(CreateConfigAddressRequest request, CancellationToken ct = default)
    {
        var r = await _transport.SendAsync<ConversationsV1ConfigAddress>(HttpMethod.Post,
            "/v1/Configuration/Addresses", formBody: request.ToForm(), ct: ct).ConfigureAwait(false);
        return r ?? throw new ApiException("empty body on POST /v1/Configuration/Addresses", 201);
    }

    public async Task<ConversationsV1ConfigAddressList> ListAsync(ListV1PageParams? filter = null, CancellationToken ct = default)
    {
        var p = filter ?? new ListV1PageParams();
        return await _transport.SendAsync<ConversationsV1ConfigAddressList>(HttpMethod.Get,
            "/v1/Configuration/Addresses", queryParams: p.ToQuery(), ct: ct).ConfigureAwait(false)
            ?? new ConversationsV1ConfigAddressList();
    }

    public async Task<ConversationsV1ConfigAddress> GetAsync(string sid, CancellationToken ct = default)
    {
        var r = await _transport.SendAsync<ConversationsV1ConfigAddress>(HttpMethod.Get,
            $"/v1/Configuration/Addresses/{sid}", ct: ct).ConfigureAwait(false);
        return r ?? throw new ApiException("empty body on GET /v1/Configuration/Addresses/{sid}", 200);
    }

    public async Task<ConversationsV1ConfigAddress> UpdateAsync(string sid, UpdateConfigAddressRequest request, CancellationToken ct = default)
    {
        var r = await _transport.SendAsync<ConversationsV1ConfigAddress>(HttpMethod.Post,
            $"/v1/Configuration/Addresses/{sid}", formBody: request.ToForm(), ct: ct).ConfigureAwait(false);
        return r ?? throw new ApiException("empty body on POST /v1/Configuration/Addresses/{sid}", 200);
    }

    public Task DeleteAsync(string sid, CancellationToken ct = default) =>
        _transport.SendAsync<object>(HttpMethod.Delete, $"/v1/Configuration/Addresses/{sid}", ct: ct);
}

// ---- /v1/ParticipantConversations + /v1/ConversationWithParticipants ------

/// <summary>Operations on <c>/v1/ParticipantConversations</c> (read-only — list only).</summary>
public sealed class ConversationsV1ParticipantConversationsResource
{
    private readonly Transport _transport;
    public ConversationsV1ParticipantConversationsResource(Transport transport) { _transport = transport; }

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
    public ConversationsV1ConversationWithParticipantsResource(Transport transport) { _transport = transport; }

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
    public ConversationsV1ServicesResource(Transport transport) { _transport = transport; }

    public async Task<ConversationsV1Service> CreateAsync(CreateConversationsServiceRequest request, CancellationToken ct = default)
    {
        var r = await _transport.SendAsync<ConversationsV1Service>(HttpMethod.Post,
            "/v1/Services", formBody: request.ToForm(), ct: ct).ConfigureAwait(false);
        return r ?? throw new ApiException("empty body on POST /v1/Services", 201);
    }

    public async Task<ConversationsV1ServiceList> ListAsync(ListV1PageParams? filter = null, CancellationToken ct = default)
    {
        var p = filter ?? new ListV1PageParams();
        return await _transport.SendAsync<ConversationsV1ServiceList>(HttpMethod.Get,
            "/v1/Services", queryParams: p.ToQuery(), ct: ct).ConfigureAwait(false)
            ?? new ConversationsV1ServiceList();
    }

    public async Task<ConversationsV1Service> GetAsync(string chatServiceSid, CancellationToken ct = default)
    {
        var r = await _transport.SendAsync<ConversationsV1Service>(HttpMethod.Get,
            $"/v1/Services/{chatServiceSid}", ct: ct).ConfigureAwait(false);
        return r ?? throw new ApiException("empty body on GET /v1/Services/{sid}", 200);
    }

    public Task DeleteAsync(string chatServiceSid, CancellationToken ct = default) =>
        _transport.SendAsync<object>(HttpMethod.Delete, $"/v1/Services/{chatServiceSid}", ct: ct);

    /// <summary>Scope to a specific Conversations Service (<c>IS…</c>) for the Phase 4
    /// service-scoped sub-resources: Conversations, Roles, Users, Bindings, Configuration,
    /// ParticipantConversations, ConversationWithParticipants. See
    /// <see cref="ConversationsV1ServiceScopeResource"/>.</summary>
    public ConversationsV1ServiceScopeResource Scope(string chatServiceSid) =>
        new(_transport, chatServiceSid);
}
