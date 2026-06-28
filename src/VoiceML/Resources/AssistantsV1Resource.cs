using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using VoiceML.Exceptions;
using VoiceML.Models;

namespace VoiceML.Resources;

/// <summary>Top-level <c>/v1/*</c> Assistants surface — Twilio-compatible Assistants v1.
/// Account resolves from HTTP Basic auth; identifiers are opaque strings (e.g.
/// <c>aia_asst_…</c>) not 34-char sids; list responses use the
/// <see cref="VoiceV1Meta"/> envelope; request bodies are
/// <c>application/json</c> (NOT form-encoded — distinct from the 2010-04-01
/// surface).
/// <para>Sub-resources: <see cref="Assistants"/>, <see cref="Tools"/>,
/// <see cref="Knowledge"/>, <see cref="Sessions"/>, <see cref="Policies"/>.</para>
/// <para>Nested per-Assistant sub-scopes (<c>Tools</c>, <c>Knowledge</c>,
/// <c>Feedbacks</c>, <c>Messages</c>) hang off the <see cref="Assistants"/>
/// resource via factory methods that take the parent assistant id, following
/// the C# precedent set by <c>Conversations.Messages(convSid)</c>.</para>
/// </summary>
public sealed class AssistantsV1Resource
{
    /// <summary>Operations on <c>/v1/Assistants</c> plus factory accessors for the
    /// per-assistant sub-resources (Tools, Knowledge, Feedbacks, Messages).</summary>
    public AssistantsV1AssistantsResource Assistants { get; }

    /// <summary>Operations on <c>/v1/Tools</c> (the global Tools catalogue).</summary>
    public AssistantsV1ToolsResource Tools { get; }

    /// <summary>Operations on <c>/v1/Knowledge</c> (the global Knowledge catalogue) plus
    /// per-knowledge factory accessors (Status, Chunks).</summary>
    public AssistantsV1KnowledgeResource Knowledge { get; }

    /// <summary>Operations on <c>/v1/Sessions</c> plus per-session factory accessor (Messages).</summary>
    public AssistantsV1SessionsResource Sessions { get; }

    /// <summary>Operations on <c>/v1/Policies</c> (read-only — list only).</summary>
    public AssistantsV1PoliciesResource Policies { get; }

    public AssistantsV1Resource(Transport transport)
    {
        Assistants = new AssistantsV1AssistantsResource(transport);
        Tools = new AssistantsV1ToolsResource(transport);
        Knowledge = new AssistantsV1KnowledgeResource(transport);
        Sessions = new AssistantsV1SessionsResource(transport);
        Policies = new AssistantsV1PoliciesResource(transport);
    }
}

// ---- /v1/Assistants (+ per-assistant scopes) -------------------------------

/// <summary>Operations on <c>/v1/Assistants</c> plus factory accessors for the
/// per-assistant sub-resources (<see cref="Tools"/>, <see cref="Knowledge"/>,
/// <see cref="Feedbacks"/>, <see cref="Messages"/>).</summary>
public sealed class AssistantsV1AssistantsResource
{
    private readonly Transport _transport;
    public AssistantsV1AssistantsResource(Transport transport) { _transport = transport; }

    public async Task<AssistantsV1AssistantList> ListAsync(ListAssistantsV1PageParams? filter = null, CancellationToken ct = default)
    {
        var p = filter ?? new ListAssistantsV1PageParams();
        return await _transport.SendAsync<AssistantsV1AssistantList>(HttpMethod.Get,
            "/v1/Assistants", queryParams: p.ToQuery(), ct: ct).ConfigureAwait(false)
            ?? new AssistantsV1AssistantList();
    }

    public async Task<AssistantsV1Assistant> CreateAsync(CreateAssistantRequest request, CancellationToken ct = default)
    {
        var r = await _transport.SendAsync<AssistantsV1Assistant>(HttpMethod.Post,
            "/v1/Assistants", jsonBody: request, ct: ct).ConfigureAwait(false);
        return r ?? throw new ApiException("empty body on POST /v1/Assistants", 201);
    }

    /// <summary>Fetch an Assistant with its attached Tools and Knowledge inlined.</summary>
    public async Task<AssistantsV1AssistantWithToolsAndKnowledge> GetAsync(string assistantId, CancellationToken ct = default)
    {
        var r = await _transport.SendAsync<AssistantsV1AssistantWithToolsAndKnowledge>(HttpMethod.Get,
            $"/v1/Assistants/{assistantId}", ct: ct).ConfigureAwait(false);
        return r ?? throw new ApiException("empty body on GET /v1/Assistants/{id}", 200);
    }

    public async Task<AssistantsV1Assistant> UpdateAsync(string assistantId, UpdateAssistantRequest request, CancellationToken ct = default)
    {
        var r = await _transport.SendAsync<AssistantsV1Assistant>(HttpMethod.Put,
            $"/v1/Assistants/{assistantId}", jsonBody: request, ct: ct).ConfigureAwait(false);
        return r ?? throw new ApiException("empty body on PUT /v1/Assistants/{id}", 200);
    }

    public Task DeleteAsync(string assistantId, CancellationToken ct = default) =>
        _transport.SendNoContentAsync(HttpMethod.Delete, $"/v1/Assistants/{assistantId}", ct: ct);

    /// <summary>Scope to a specific Assistant's <c>/Tools</c> attach/detach sub-resource.</summary>
    public AssistantsV1AssistantToolsScope Tools(string assistantId) => new(_transport, assistantId);

    /// <summary>Scope to a specific Assistant's <c>/Knowledge</c> attach/detach sub-resource.</summary>
    public AssistantsV1AssistantKnowledgeScope Knowledge(string assistantId) => new(_transport, assistantId);

    /// <summary>Scope to a specific Assistant's <c>/Feedbacks</c> sub-resource.</summary>
    public AssistantsV1AssistantFeedbacksScope Feedbacks(string assistantId) => new(_transport, assistantId);

    /// <summary>Scope to a specific Assistant's <c>/Messages</c> sub-resource
    /// (send-message endpoint).</summary>
    public AssistantsV1AssistantMessagesScope Messages(string assistantId) => new(_transport, assistantId);
}

/// <summary>Operations on <c>/v1/Assistants/{id}/Tools</c> (list attached + attach/detach by tool id).</summary>
public sealed class AssistantsV1AssistantToolsScope
{
    private readonly Transport _transport;

    /// <summary>The parent Assistant id this scope is bound to.</summary>
    public string AssistantId { get; }

    internal AssistantsV1AssistantToolsScope(Transport transport, string assistantId)
    {
        _transport = transport;
        AssistantId = assistantId;
    }

    private string BasePath => $"/v1/Assistants/{AssistantId}/Tools";

    public async Task<AssistantsV1ToolList> ListAsync(ListAssistantsV1PageParams? filter = null, CancellationToken ct = default)
    {
        var p = filter ?? new ListAssistantsV1PageParams();
        return await _transport.SendAsync<AssistantsV1ToolList>(HttpMethod.Get,
            BasePath, queryParams: p.ToQuery(), ct: ct).ConfigureAwait(false)
            ?? new AssistantsV1ToolList();
    }

    /// <summary>Attach an existing Tool to this Assistant (no request body).</summary>
    public Task AttachAsync(string toolId, CancellationToken ct = default) =>
        _transport.SendNoContentAsync(HttpMethod.Post, $"{BasePath}/{toolId}", ct: ct);

    /// <summary>Detach a Tool from this Assistant.</summary>
    public Task DetachAsync(string toolId, CancellationToken ct = default) =>
        _transport.SendNoContentAsync(HttpMethod.Delete, $"{BasePath}/{toolId}", ct: ct);
}

/// <summary>Operations on <c>/v1/Assistants/{id}/Knowledge</c> (list attached + attach/detach by knowledge id).</summary>
public sealed class AssistantsV1AssistantKnowledgeScope
{
    private readonly Transport _transport;

    /// <summary>The parent Assistant id this scope is bound to.</summary>
    public string AssistantId { get; }

    internal AssistantsV1AssistantKnowledgeScope(Transport transport, string assistantId)
    {
        _transport = transport;
        AssistantId = assistantId;
    }

    private string BasePath => $"/v1/Assistants/{AssistantId}/Knowledge";

    public async Task<AssistantsV1KnowledgeList> ListAsync(ListAssistantsV1PageParams? filter = null, CancellationToken ct = default)
    {
        var p = filter ?? new ListAssistantsV1PageParams();
        return await _transport.SendAsync<AssistantsV1KnowledgeList>(HttpMethod.Get,
            BasePath, queryParams: p.ToQuery(), ct: ct).ConfigureAwait(false)
            ?? new AssistantsV1KnowledgeList();
    }

    /// <summary>Attach an existing Knowledge source to this Assistant (no request body).</summary>
    public Task AttachAsync(string knowledgeId, CancellationToken ct = default) =>
        _transport.SendNoContentAsync(HttpMethod.Post, $"{BasePath}/{knowledgeId}", ct: ct);

    /// <summary>Detach a Knowledge source from this Assistant.</summary>
    public Task DetachAsync(string knowledgeId, CancellationToken ct = default) =>
        _transport.SendNoContentAsync(HttpMethod.Delete, $"{BasePath}/{knowledgeId}", ct: ct);
}

/// <summary>Operations on <c>/v1/Assistants/{id}/Feedbacks</c> (list and create).</summary>
public sealed class AssistantsV1AssistantFeedbacksScope
{
    private readonly Transport _transport;

    /// <summary>The parent Assistant id this scope is bound to.</summary>
    public string AssistantId { get; }

    internal AssistantsV1AssistantFeedbacksScope(Transport transport, string assistantId)
    {
        _transport = transport;
        AssistantId = assistantId;
    }

    private string BasePath => $"/v1/Assistants/{AssistantId}/Feedbacks";

    public async Task<AssistantsV1FeedbackList> ListAsync(ListAssistantsV1PageParams? filter = null, CancellationToken ct = default)
    {
        var p = filter ?? new ListAssistantsV1PageParams();
        return await _transport.SendAsync<AssistantsV1FeedbackList>(HttpMethod.Get,
            BasePath, queryParams: p.ToQuery(), ct: ct).ConfigureAwait(false)
            ?? new AssistantsV1FeedbackList();
    }

    public async Task<AssistantsV1Feedback> CreateAsync(CreateAssistantFeedbackRequest request, CancellationToken ct = default)
    {
        var r = await _transport.SendAsync<AssistantsV1Feedback>(HttpMethod.Post,
            BasePath, jsonBody: request, ct: ct).ConfigureAwait(false);
        return r ?? throw new ApiException("empty body on POST /v1/Assistants/{id}/Feedbacks", 201);
    }
}

/// <summary>Operations on <c>/v1/Assistants/{id}/Messages</c> — the
/// send-message endpoint. Returns <see cref="AssistantsV1SendMessageResponse"/>,
/// not an <see cref="AssistantsV1Message"/> (the session may be created
/// transparently as part of the send).</summary>
public sealed class AssistantsV1AssistantMessagesScope
{
    private readonly Transport _transport;

    /// <summary>The parent Assistant id this scope is bound to.</summary>
    public string AssistantId { get; }

    internal AssistantsV1AssistantMessagesScope(Transport transport, string assistantId)
    {
        _transport = transport;
        AssistantId = assistantId;
    }

    private string BasePath => $"/v1/Assistants/{AssistantId}/Messages";

    /// <summary>Send a message to the Assistant. If <see cref="SendAssistantMessageRequest.SessionId"/>
    /// is omitted the server creates a fresh session keyed by the identity.</summary>
    public async Task<AssistantsV1SendMessageResponse> SendAsync(SendAssistantMessageRequest request, CancellationToken ct = default)
    {
        var r = await _transport.SendAsync<AssistantsV1SendMessageResponse>(HttpMethod.Post,
            BasePath, jsonBody: request, ct: ct).ConfigureAwait(false);
        return r ?? throw new ApiException("empty body on POST /v1/Assistants/{id}/Messages", 200);
    }
}

// ---- /v1/Tools -------------------------------------------------------------

/// <summary>Operations on <c>/v1/Tools</c> (the global Tools catalogue). Fetch
/// returns the richer <see cref="AssistantsV1ToolWithPolicies"/> variant.</summary>
public sealed class AssistantsV1ToolsResource
{
    private readonly Transport _transport;
    public AssistantsV1ToolsResource(Transport transport) { _transport = transport; }

    public async Task<AssistantsV1ToolList> ListAsync(ListAssistantsV1ToolsParams? filter = null, CancellationToken ct = default)
    {
        var p = filter ?? new ListAssistantsV1ToolsParams();
        return await _transport.SendAsync<AssistantsV1ToolList>(HttpMethod.Get,
            "/v1/Tools", queryParams: p.ToQuery(), ct: ct).ConfigureAwait(false)
            ?? new AssistantsV1ToolList();
    }

    public async Task<AssistantsV1Tool> CreateAsync(CreateAssistantToolRequest request, CancellationToken ct = default)
    {
        var r = await _transport.SendAsync<AssistantsV1Tool>(HttpMethod.Post,
            "/v1/Tools", jsonBody: request, ct: ct).ConfigureAwait(false);
        return r ?? throw new ApiException("empty body on POST /v1/Tools", 201);
    }

    /// <summary>Fetch a Tool with its attached Policies inlined.</summary>
    public async Task<AssistantsV1ToolWithPolicies> GetAsync(string toolId, CancellationToken ct = default)
    {
        var r = await _transport.SendAsync<AssistantsV1ToolWithPolicies>(HttpMethod.Get,
            $"/v1/Tools/{toolId}", ct: ct).ConfigureAwait(false);
        return r ?? throw new ApiException("empty body on GET /v1/Tools/{id}", 200);
    }

    public async Task<AssistantsV1Tool> UpdateAsync(string toolId, UpdateAssistantToolRequest request, CancellationToken ct = default)
    {
        var r = await _transport.SendAsync<AssistantsV1Tool>(HttpMethod.Put,
            $"/v1/Tools/{toolId}", jsonBody: request, ct: ct).ConfigureAwait(false);
        return r ?? throw new ApiException("empty body on PUT /v1/Tools/{id}", 200);
    }

    public Task DeleteAsync(string toolId, CancellationToken ct = default) =>
        _transport.SendNoContentAsync(HttpMethod.Delete, $"/v1/Tools/{toolId}", ct: ct);
}

// ---- /v1/Knowledge (+ per-knowledge Status / Chunks scopes) ----------------

/// <summary>Operations on <c>/v1/Knowledge</c> (the global Knowledge catalogue) plus
/// per-knowledge factory accessors for <see cref="Status"/> and <see cref="Chunks"/>.</summary>
public sealed class AssistantsV1KnowledgeResource
{
    private readonly Transport _transport;
    public AssistantsV1KnowledgeResource(Transport transport) { _transport = transport; }

    public async Task<AssistantsV1KnowledgeList> ListAsync(ListAssistantsV1KnowledgeParams? filter = null, CancellationToken ct = default)
    {
        var p = filter ?? new ListAssistantsV1KnowledgeParams();
        return await _transport.SendAsync<AssistantsV1KnowledgeList>(HttpMethod.Get,
            "/v1/Knowledge", queryParams: p.ToQuery(), ct: ct).ConfigureAwait(false)
            ?? new AssistantsV1KnowledgeList();
    }

    public async Task<AssistantsV1Knowledge> CreateAsync(CreateAssistantKnowledgeRequest request, CancellationToken ct = default)
    {
        var r = await _transport.SendAsync<AssistantsV1Knowledge>(HttpMethod.Post,
            "/v1/Knowledge", jsonBody: request, ct: ct).ConfigureAwait(false);
        return r ?? throw new ApiException("empty body on POST /v1/Knowledge", 201);
    }

    public async Task<AssistantsV1Knowledge> GetAsync(string knowledgeId, CancellationToken ct = default)
    {
        var r = await _transport.SendAsync<AssistantsV1Knowledge>(HttpMethod.Get,
            $"/v1/Knowledge/{knowledgeId}", ct: ct).ConfigureAwait(false);
        return r ?? throw new ApiException("empty body on GET /v1/Knowledge/{id}", 200);
    }

    public async Task<AssistantsV1Knowledge> UpdateAsync(string knowledgeId, UpdateAssistantKnowledgeRequest request, CancellationToken ct = default)
    {
        var r = await _transport.SendAsync<AssistantsV1Knowledge>(HttpMethod.Put,
            $"/v1/Knowledge/{knowledgeId}", jsonBody: request, ct: ct).ConfigureAwait(false);
        return r ?? throw new ApiException("empty body on PUT /v1/Knowledge/{id}", 200);
    }

    public Task DeleteAsync(string knowledgeId, CancellationToken ct = default) =>
        _transport.SendNoContentAsync(HttpMethod.Delete, $"/v1/Knowledge/{knowledgeId}", ct: ct);

    /// <summary>Scope to a specific Knowledge source's <c>/Status</c> sub-resource (singleton).</summary>
    public AssistantsV1KnowledgeStatusScope Status(string knowledgeId) => new(_transport, knowledgeId);

    /// <summary>Scope to a specific Knowledge source's <c>/Chunks</c> sub-resource (list-only).</summary>
    public AssistantsV1KnowledgeChunksScope Chunks(string knowledgeId) => new(_transport, knowledgeId);
}

/// <summary>Operations on <c>/v1/Knowledge/{id}/Status</c> — singleton, fetch-only.</summary>
public sealed class AssistantsV1KnowledgeStatusScope
{
    private readonly Transport _transport;

    /// <summary>The parent Knowledge id this scope is bound to.</summary>
    public string KnowledgeId { get; }

    internal AssistantsV1KnowledgeStatusScope(Transport transport, string knowledgeId)
    {
        _transport = transport;
        KnowledgeId = knowledgeId;
    }

    public async Task<AssistantsV1KnowledgeStatus> FetchAsync(CancellationToken ct = default)
    {
        var r = await _transport.SendAsync<AssistantsV1KnowledgeStatus>(HttpMethod.Get,
            $"/v1/Knowledge/{KnowledgeId}/Status", ct: ct).ConfigureAwait(false);
        return r ?? throw new ApiException("empty body on GET /v1/Knowledge/{id}/Status", 200);
    }
}

/// <summary>Operations on <c>/v1/Knowledge/{id}/Chunks</c> — list-only.</summary>
public sealed class AssistantsV1KnowledgeChunksScope
{
    private readonly Transport _transport;

    /// <summary>The parent Knowledge id this scope is bound to.</summary>
    public string KnowledgeId { get; }

    internal AssistantsV1KnowledgeChunksScope(Transport transport, string knowledgeId)
    {
        _transport = transport;
        KnowledgeId = knowledgeId;
    }

    public async Task<AssistantsV1KnowledgeChunkList> ListAsync(ListAssistantsV1PageParams? filter = null, CancellationToken ct = default)
    {
        var p = filter ?? new ListAssistantsV1PageParams();
        return await _transport.SendAsync<AssistantsV1KnowledgeChunkList>(HttpMethod.Get,
            $"/v1/Knowledge/{KnowledgeId}/Chunks", queryParams: p.ToQuery(), ct: ct).ConfigureAwait(false)
            ?? new AssistantsV1KnowledgeChunkList();
    }
}

// ---- /v1/Sessions (+ per-session Messages scope) ---------------------------

/// <summary>Operations on <c>/v1/Sessions</c> (list + fetch only) plus per-session
/// factory accessor for <see cref="Messages"/>.</summary>
public sealed class AssistantsV1SessionsResource
{
    private readonly Transport _transport;
    public AssistantsV1SessionsResource(Transport transport) { _transport = transport; }

    public async Task<AssistantsV1SessionList> ListAsync(ListAssistantsV1PageParams? filter = null, CancellationToken ct = default)
    {
        var p = filter ?? new ListAssistantsV1PageParams();
        return await _transport.SendAsync<AssistantsV1SessionList>(HttpMethod.Get,
            "/v1/Sessions", queryParams: p.ToQuery(), ct: ct).ConfigureAwait(false)
            ?? new AssistantsV1SessionList();
    }

    public async Task<AssistantsV1Session> GetAsync(string sessionId, CancellationToken ct = default)
    {
        var r = await _transport.SendAsync<AssistantsV1Session>(HttpMethod.Get,
            $"/v1/Sessions/{sessionId}", ct: ct).ConfigureAwait(false);
        return r ?? throw new ApiException("empty body on GET /v1/Sessions/{id}", 200);
    }

    /// <summary>Scope to a specific Session's <c>/Messages</c> sub-resource (list-only).</summary>
    public AssistantsV1SessionMessagesScope Messages(string sessionId) => new(_transport, sessionId);
}

/// <summary>Operations on <c>/v1/Sessions/{id}/Messages</c> — list-only.</summary>
public sealed class AssistantsV1SessionMessagesScope
{
    private readonly Transport _transport;

    /// <summary>The parent Session id this scope is bound to.</summary>
    public string SessionId { get; }

    internal AssistantsV1SessionMessagesScope(Transport transport, string sessionId)
    {
        _transport = transport;
        SessionId = sessionId;
    }

    public async Task<AssistantsV1MessageList> ListAsync(ListAssistantsV1PageParams? filter = null, CancellationToken ct = default)
    {
        var p = filter ?? new ListAssistantsV1PageParams();
        return await _transport.SendAsync<AssistantsV1MessageList>(HttpMethod.Get,
            $"/v1/Sessions/{SessionId}/Messages", queryParams: p.ToQuery(), ct: ct).ConfigureAwait(false)
            ?? new AssistantsV1MessageList();
    }
}

// ---- /v1/Policies (read-only — list only) ----------------------------------

/// <summary>Operations on <c>/v1/Policies</c> — read-only. Filterable by
/// <c>ToolId</c> or <c>KnowledgeId</c>.</summary>
public sealed class AssistantsV1PoliciesResource
{
    private readonly Transport _transport;
    public AssistantsV1PoliciesResource(Transport transport) { _transport = transport; }

    public async Task<AssistantsV1PolicyList> ListAsync(ListAssistantsV1PoliciesParams? filter = null, CancellationToken ct = default)
    {
        var p = filter ?? new ListAssistantsV1PoliciesParams();
        return await _transport.SendAsync<AssistantsV1PolicyList>(HttpMethod.Get,
            "/v1/Policies", queryParams: p.ToQuery(), ct: ct).ConfigureAwait(false)
            ?? new AssistantsV1PolicyList();
    }
}
