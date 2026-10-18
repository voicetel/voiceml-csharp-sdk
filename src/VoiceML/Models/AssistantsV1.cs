using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace VoiceML.Models;

// ===========================================================================
// Assistants v1 (/v1/Assistants, /v1/Tools, /v1/Knowledge, /v1/Sessions,
// /v1/Policies + nested Assistant- and Knowledge-scoped sub-resources).
//
// Twilio-compatible Assistants surface. All endpoints accept and return
// application/json (NOT form-encoded — distinct from /Calls and Conversations
// v1). Identifiers use opaque string ids (e.g. ^aia_asst_.+$) instead of
// 34-char hex sids. Account resolves from HTTP Basic auth; dates are ISO-8601;
// list responses use the VoiceV1Meta envelope.
// ===========================================================================

// ---- Response shapes ------------------------------------------------------

/// <summary>An Assistant — identified by <c>aia_asst_…</c>. Returned from
/// list/create/update; the fetch endpoint returns the richer
/// <see cref="AssistantsV1AssistantWithToolsAndKnowledge"/> variant.</summary>
public sealed record AssistantsV1Assistant
{
    /// <summary>SID of the account that owns this resource.</summary>
    [JsonPropertyName("account_sid")] public string? AccountSid { get; init; }
    /// <summary>Identifier of the associated with this resource.</summary>
    [JsonPropertyName("id")] public string? Id { get; init; }
    /// <summary>Human-readable name used to identify the Assistant, tool, knowledge source or policy.</summary>
    [JsonPropertyName("name")] public string? Name { get; init; }
    /// <summary>Application-defined owner of this Assistant.</summary>
    [JsonPropertyName("owner")] public string? Owner { get; init; }
    /// <summary>VoiceML extension: the BYO-LLM model backing the assistant (Phase 6).</summary>
    [JsonPropertyName("model")] public string? Model { get; init; }
    /// <summary>Instructions describing the Assistant personality and expected behavior.</summary>
    [JsonPropertyName("personality_prompt")] public string? PersonalityPrompt { get; init; }
    /// <summary>Customer-specific inference-provider settings, preserving the declared configuration object.</summary>
    [JsonPropertyName("customer_ai")] public JsonElement? CustomerAi { get; init; }
    /// <summary>Absolute URL of this resource.</summary>
    [JsonPropertyName("url")] public string? Url { get; init; }
    /// <summary>Timestamp when this resource was created.</summary>
    [JsonPropertyName("date_created")] public string? DateCreated { get; init; }
    /// <summary>Timestamp of the most recent modification to this resource.</summary>
    [JsonPropertyName("date_updated")] public string? DateUpdated { get; init; }
}

/// <summary>Page of assistant resources with continuation metadata.</summary>
public sealed record AssistantsV1AssistantList : IV1PagedList
{
    /// <summary>The returned assistants in this page.</summary>
    [JsonPropertyName("assistants")] public List<AssistantsV1Assistant> Assistants { get; init; } = new();
    /// <summary>Pagination metadata; follow next_page_url to fetch the next page.</summary>
    [JsonPropertyName("meta")] public VoiceV1Meta? Meta { get; init; }
}

/// <summary>An Assistant with its attached Tools and Knowledge inlined.
/// Returned by <c>GET /v1/Assistants/{id}</c>.</summary>
public sealed record AssistantsV1AssistantWithToolsAndKnowledge
{
    /// <summary>SID of the account that owns this resource.</summary>
    [JsonPropertyName("account_sid")] public string? AccountSid { get; init; }
    /// <summary>Identifier of the associated with this resource.</summary>
    [JsonPropertyName("id")] public string? Id { get; init; }
    /// <summary>Human-readable name used to identify the Assistant, tool, knowledge source or policy.</summary>
    [JsonPropertyName("name")] public string? Name { get; init; }
    /// <summary>Application-defined owner of this Assistant.</summary>
    [JsonPropertyName("owner")] public string? Owner { get; init; }
    /// <summary>VoiceML extension: the BYO-LLM model backing the assistant (Phase 6).</summary>
    [JsonPropertyName("model")] public string? Model { get; init; }
    /// <summary>Instructions describing the Assistant personality and expected behavior.</summary>
    [JsonPropertyName("personality_prompt")] public string? PersonalityPrompt { get; init; }
    /// <summary>Customer-specific inference-provider settings, preserving the declared configuration object.</summary>
    [JsonPropertyName("customer_ai")] public JsonElement? CustomerAi { get; init; }
    /// <summary>Absolute URL of this resource.</summary>
    [JsonPropertyName("url")] public string? Url { get; init; }
    /// <summary>Timestamp when this resource was created.</summary>
    [JsonPropertyName("date_created")] public string? DateCreated { get; init; }
    /// <summary>Timestamp of the most recent modification to this resource.</summary>
    [JsonPropertyName("date_updated")] public string? DateUpdated { get; init; }
    /// <summary>The returned tools in this page.</summary>
    [JsonPropertyName("tools")] public List<AssistantsV1Tool> Tools { get; init; } = new();
    /// <summary>The returned knowledge in this page.</summary>
    [JsonPropertyName("knowledge")] public List<AssistantsV1Knowledge> Knowledge { get; init; } = new();
}

/// <summary>A Tool — identified by <c>aia_tool_…</c>. Tools can be defined globally
/// and attached to one or more Assistants.</summary>
public sealed record AssistantsV1Tool
{
    /// <summary>SID of the account that owns this resource.</summary>
    [JsonPropertyName("account_sid")] public string? AccountSid { get; init; }
    /// <summary>Identifier of the associated with this resource.</summary>
    [JsonPropertyName("id")] public string? Id { get; init; }
    /// <summary>Human-readable name used to identify the Assistant, tool, knowledge source or policy.</summary>
    [JsonPropertyName("name")] public string? Name { get; init; }
    /// <summary>Provider-defined category identifying the kind of resource or channel.</summary>
    [JsonPropertyName("type")] public string? Type { get; init; }
    /// <summary>Human-readable explanation of the purpose of the tool, knowledge source or policy.</summary>
    [JsonPropertyName("description")] public string? Description { get; init; }
    /// <summary>Whether the target is active. Default true.</summary>
    [JsonPropertyName("enabled")] public bool Enabled { get; init; }
    /// <summary>Whether requires auth is enabled for this resource.</summary>
    [JsonPropertyName("requires_auth")] public bool RequiresAuth { get; init; }
    /// <summary>Pagination metadata; follow next_page_url to fetch the next page.</summary>
    [JsonPropertyName("meta")] public JsonElement? Meta { get; init; }
    /// <summary>Absolute URL of this resource.</summary>
    [JsonPropertyName("url")] public string? Url { get; init; }
    /// <summary>Timestamp when this resource was created.</summary>
    [JsonPropertyName("date_created")] public string? DateCreated { get; init; }
    /// <summary>Timestamp of the most recent modification to this resource.</summary>
    [JsonPropertyName("date_updated")] public string? DateUpdated { get; init; }
}

/// <summary>Page of tool resources with continuation metadata.</summary>
public sealed record AssistantsV1ToolList : IV1PagedList
{
    /// <summary>The returned tools in this page.</summary>
    [JsonPropertyName("tools")] public List<AssistantsV1Tool> Tools { get; init; } = new();
    /// <summary>Pagination metadata; follow next_page_url to fetch the next page.</summary>
    [JsonPropertyName("meta")] public VoiceV1Meta? Meta { get; init; }
}

/// <summary>A Tool with its attached Policies inlined.
/// Returned by <c>GET /v1/Tools/{id}</c>.</summary>
public sealed record AssistantsV1ToolWithPolicies
{
    /// <summary>SID of the account that owns this resource.</summary>
    [JsonPropertyName("account_sid")] public string? AccountSid { get; init; }
    /// <summary>Identifier of the associated with this resource.</summary>
    [JsonPropertyName("id")] public string? Id { get; init; }
    /// <summary>Human-readable name used to identify the Assistant, tool, knowledge source or policy.</summary>
    [JsonPropertyName("name")] public string? Name { get; init; }
    /// <summary>Provider-defined category identifying the kind of resource or channel.</summary>
    [JsonPropertyName("type")] public string? Type { get; init; }
    /// <summary>Human-readable explanation of the purpose of the tool, knowledge source or policy.</summary>
    [JsonPropertyName("description")] public string? Description { get; init; }
    /// <summary>Whether the target is active. Default true.</summary>
    [JsonPropertyName("enabled")] public bool Enabled { get; init; }
    /// <summary>Whether requires auth is enabled for this resource.</summary>
    [JsonPropertyName("requires_auth")] public bool RequiresAuth { get; init; }
    /// <summary>Pagination metadata; follow next_page_url to fetch the next page.</summary>
    [JsonPropertyName("meta")] public JsonElement? Meta { get; init; }
    /// <summary>Absolute URL of this resource.</summary>
    [JsonPropertyName("url")] public string? Url { get; init; }
    /// <summary>Timestamp when this resource was created.</summary>
    [JsonPropertyName("date_created")] public string? DateCreated { get; init; }
    /// <summary>Timestamp of the most recent modification to this resource.</summary>
    [JsonPropertyName("date_updated")] public string? DateUpdated { get; init; }
    /// <summary>The returned policies in this page.</summary>
    [JsonPropertyName("policies")] public List<AssistantsV1Policy> Policies { get; init; } = new();
}

/// <summary>A Knowledge source — identified by <c>aia_know_…</c>. Knowledge can be
/// defined globally and attached to one or more Assistants.</summary>
public sealed record AssistantsV1Knowledge
{
    /// <summary>SID of the account that owns this resource.</summary>
    [JsonPropertyName("account_sid")] public string? AccountSid { get; init; }
    /// <summary>Identifier of the associated with this resource.</summary>
    [JsonPropertyName("id")] public string? Id { get; init; }
    /// <summary>Human-readable name used to identify the Assistant, tool, knowledge source or policy.</summary>
    [JsonPropertyName("name")] public string? Name { get; init; }
    /// <summary>Provider-defined category identifying the kind of resource or channel.</summary>
    [JsonPropertyName("type")] public string? Type { get; init; }
    /// <summary>Human-readable explanation of the purpose of the tool, knowledge source or policy.</summary>
    [JsonPropertyName("description")] public string? Description { get; init; }
    /// <summary>Current resource state; unrecognized future values are preserved as strings.</summary>
    [JsonPropertyName("status")] public string? Status { get; init; }
    /// <summary>Model identifier used to generate embeddings for knowledge indexing.</summary>
    [JsonPropertyName("embedding_model")] public string? EmbeddingModel { get; init; }
    /// <summary>Source-specific ingestion settings, such as the document URL or uploaded content.</summary>
    [JsonPropertyName("knowledge_source_details")] public JsonElement? KnowledgeSourceDetails { get; init; }
    /// <summary>Absolute URL of this resource.</summary>
    [JsonPropertyName("url")] public string? Url { get; init; }
    /// <summary>Timestamp when this resource was created.</summary>
    [JsonPropertyName("date_created")] public string? DateCreated { get; init; }
    /// <summary>Timestamp of the most recent modification to this resource.</summary>
    [JsonPropertyName("date_updated")] public string? DateUpdated { get; init; }
}

/// <summary>Page of knowledge resources with continuation metadata.</summary>
public sealed record AssistantsV1KnowledgeList : IV1PagedList
{
    /// <summary>The returned knowledge in this page.</summary>
    [JsonPropertyName("knowledge")] public List<AssistantsV1Knowledge> Knowledge { get; init; } = new();
    /// <summary>Pagination metadata; follow next_page_url to fetch the next page.</summary>
    [JsonPropertyName("meta")] public VoiceV1Meta? Meta { get; init; }
}

/// <summary>Ingestion status snapshot for a Knowledge source. Returned by
/// <c>GET /v1/Knowledge/{id}/Status</c>.</summary>
public sealed record AssistantsV1KnowledgeStatus
{
    /// <summary>SID of the account that owns this resource.</summary>
    [JsonPropertyName("account_sid")] public string? AccountSid { get; init; }
    /// <summary>Current resource state; unrecognized future values are preserved as strings.</summary>
    [JsonPropertyName("status")] public string? Status { get; init; }
    /// <summary>Most recent status reported by the knowledge ingestion or indexing process.</summary>
    [JsonPropertyName("last_status")] public string? LastStatus { get; init; }
    /// <summary>Timestamp of the most recent modification to this resource.</summary>
    [JsonPropertyName("date_updated")] public string? DateUpdated { get; init; }
}

/// <summary>A single chunk produced by ingesting a Knowledge source.</summary>
public sealed record AssistantsV1KnowledgeChunk
{
    /// <summary>SID of the account that owns this resource.</summary>
    [JsonPropertyName("account_sid")] public string? AccountSid { get; init; }
    /// <summary>Message text or the indexed text content of a knowledge chunk.</summary>
    [JsonPropertyName("content")] public string? Content { get; init; }
    /// <summary>Application-supplied metadata attached to the indexed knowledge chunk.</summary>
    [JsonPropertyName("metadata")] public JsonElement? Metadata { get; init; }
    /// <summary>Timestamp when this resource was created.</summary>
    [JsonPropertyName("date_created")] public string? DateCreated { get; init; }
    /// <summary>Timestamp of the most recent modification to this resource.</summary>
    [JsonPropertyName("date_updated")] public string? DateUpdated { get; init; }
}

/// <summary>Page of knowledge chunk resources with continuation metadata.</summary>
public sealed record AssistantsV1KnowledgeChunkList : IV1PagedList
{
    /// <summary>The returned chunks in this page.</summary>
    [JsonPropertyName("chunks")] public List<AssistantsV1KnowledgeChunk> Chunks { get; init; } = new();
    /// <summary>Pagination metadata; follow next_page_url to fetch the next page.</summary>
    [JsonPropertyName("meta")] public VoiceV1Meta? Meta { get; init; }
}

/// <summary>An Assistant Session — a conversation turn-state per identity.</summary>
public sealed record AssistantsV1Session
{
    /// <summary>Identifier of the associated with this resource.</summary>
    [JsonPropertyName("id")] public string? Id { get; init; }
    /// <summary>SID of the account that owns this resource.</summary>
    [JsonPropertyName("account_sid")] public string? AccountSid { get; init; }
    /// <summary>Identifier of the assistant associated with this resource.</summary>
    [JsonPropertyName("assistant_id")] public string? AssistantId { get; init; }
    /// <summary>Whether verified is enabled for this resource.</summary>
    [JsonPropertyName("verified")] public bool? Verified { get; init; }
    /// <summary>Application-defined user identity.</summary>
    [JsonPropertyName("identity")] public string? Identity { get; init; }
    /// <summary>Timestamp when this resource was created.</summary>
    [JsonPropertyName("date_created")] public string? DateCreated { get; init; }
    /// <summary>Timestamp of the most recent modification to this resource.</summary>
    [JsonPropertyName("date_updated")] public string? DateUpdated { get; init; }
}

/// <summary>Page of session resources with continuation metadata.</summary>
public sealed record AssistantsV1SessionList : IV1PagedList
{
    /// <summary>The returned sessions in this page.</summary>
    [JsonPropertyName("sessions")] public List<AssistantsV1Session> Sessions { get; init; } = new();
    /// <summary>Pagination metadata; follow next_page_url to fetch the next page.</summary>
    [JsonPropertyName("meta")] public VoiceV1Meta? Meta { get; init; }
}

/// <summary>A Message inside a Session — identified by <c>aia_msg_…</c>.</summary>
public sealed record AssistantsV1Message
{
    /// <summary>Identifier of the associated with this resource.</summary>
    [JsonPropertyName("id")] public string? Id { get; init; }
    /// <summary>SID of the account that owns this resource.</summary>
    [JsonPropertyName("account_sid")] public string? AccountSid { get; init; }
    /// <summary>Identifier of the assistant associated with this resource.</summary>
    [JsonPropertyName("assistant_id")] public string? AssistantId { get; init; }
    /// <summary>Identifier of the session associated with this resource.</summary>
    [JsonPropertyName("session_id")] public string? SessionId { get; init; }
    /// <summary>Application-defined user identity.</summary>
    [JsonPropertyName("identity")] public string? Identity { get; init; }
    /// <summary>Role of the message author within the Assistant conversation.</summary>
    [JsonPropertyName("role")] public string? Role { get; init; }
    /// <summary>Message text or the indexed text content of a knowledge chunk.</summary>
    [JsonPropertyName("content")] public JsonElement? Content { get; init; }
    /// <summary>Pagination metadata; follow next_page_url to fetch the next page.</summary>
    [JsonPropertyName("meta")] public JsonElement? Meta { get; init; }
    /// <summary>Timestamp when this resource was created.</summary>
    [JsonPropertyName("date_created")] public string? DateCreated { get; init; }
    /// <summary>Timestamp of the most recent modification to this resource.</summary>
    [JsonPropertyName("date_updated")] public string? DateUpdated { get; init; }
}

/// <summary>Page of message resources with continuation metadata.</summary>
public sealed record AssistantsV1MessageList : IV1PagedList
{
    /// <summary>The returned messages in this page.</summary>
    [JsonPropertyName("messages")] public List<AssistantsV1Message> Messages { get; init; } = new();
    /// <summary>Pagination metadata; follow next_page_url to fetch the next page.</summary>
    [JsonPropertyName("meta")] public VoiceV1Meta? Meta { get; init; }
}

/// <summary>Result of sending a message to an Assistant via
/// <c>POST /v1/Assistants/{id}/Messages</c>. Returns the (possibly
/// newly-created) session id plus the synchronous reply.</summary>
public sealed record AssistantsV1SendMessageResponse
{
    /// <summary>Current resource state; unrecognized future values are preserved as strings.</summary>
    [JsonPropertyName("status")] public string? Status { get; init; }
    /// <summary>Whether flagged is enabled for this resource.</summary>
    [JsonPropertyName("flagged")] public bool? Flagged { get; init; }
    /// <summary>Whether aborted is enabled for this resource.</summary>
    [JsonPropertyName("aborted")] public bool? Aborted { get; init; }
    /// <summary>Identifier of the session associated with this resource.</summary>
    [JsonPropertyName("session_id")] public string? SessionId { get; init; }
    /// <summary>SID of the account that owns this resource.</summary>
    [JsonPropertyName("account_sid")] public string? AccountSid { get; init; }
    /// <summary>Text content of the message.</summary>
    [JsonPropertyName("body")] public string? Body { get; init; }
    /// <summary>Error detail returned when processing the message failed.</summary>
    [JsonPropertyName("error")] public string? Error { get; init; }
}

/// <summary>Feedback on a specific Assistant message — identified by <c>aia_fdbk_…</c>.</summary>
public sealed record AssistantsV1Feedback
{
    /// <summary>Identifier of the associated with this resource.</summary>
    [JsonPropertyName("id")] public string? Id { get; init; }
    /// <summary>SID of the account that owns this resource.</summary>
    [JsonPropertyName("account_sid")] public string? AccountSid { get; init; }
    /// <summary>SID of the related User.</summary>
    [JsonPropertyName("user_sid")] public string? UserSid { get; init; }
    /// <summary>Identifier of the assistant associated with this resource.</summary>
    [JsonPropertyName("assistant_id")] public string? AssistantId { get; init; }
    /// <summary>Identifier of the session associated with this resource.</summary>
    [JsonPropertyName("session_id")] public string? SessionId { get; init; }
    /// <summary>Identifier of the message associated with this resource.</summary>
    [JsonPropertyName("message_id")] public string? MessageId { get; init; }
    /// <summary>Feedback score assigned to the Assistant response.</summary>
    [JsonPropertyName("score")] public float? Score { get; init; }
    /// <summary>Free-form feedback explaining the assigned Assistant response score.</summary>
    [JsonPropertyName("text")] public string? Text { get; init; }
    /// <summary>Timestamp when this resource was created.</summary>
    [JsonPropertyName("date_created")] public string? DateCreated { get; init; }
    /// <summary>Timestamp of the most recent modification to this resource.</summary>
    [JsonPropertyName("date_updated")] public string? DateUpdated { get; init; }
}

/// <summary>Page of feedback resources with continuation metadata.</summary>
public sealed record AssistantsV1FeedbackList : IV1PagedList
{
    /// <summary>The returned feedbacks in this page.</summary>
    [JsonPropertyName("feedbacks")] public List<AssistantsV1Feedback> Feedbacks { get; init; } = new();
    /// <summary>Pagination metadata; follow next_page_url to fetch the next page.</summary>
    [JsonPropertyName("meta")] public VoiceV1Meta? Meta { get; init; }
}

/// <summary>A Policy — identified by <c>aia_plcy_…</c>. Policies attach to Tools or Knowledge.</summary>
public sealed record AssistantsV1Policy
{
    /// <summary>Identifier of the associated with this resource.</summary>
    [JsonPropertyName("id")] public string? Id { get; init; }
    /// <summary>SID of the account that owns this resource.</summary>
    [JsonPropertyName("account_sid")] public string? AccountSid { get; init; }
    /// <summary>SID of the related User.</summary>
    [JsonPropertyName("user_sid")] public string? UserSid { get; init; }
    /// <summary>Human-readable name used to identify the Assistant, tool, knowledge source or policy.</summary>
    [JsonPropertyName("name")] public string? Name { get; init; }
    /// <summary>Human-readable explanation of the purpose of the tool, knowledge source or policy.</summary>
    [JsonPropertyName("description")] public string? Description { get; init; }
    /// <summary>Provider-defined category identifying the kind of resource or channel.</summary>
    [JsonPropertyName("type")] public string? Type { get; init; }
    /// <summary>Policy-specific rules and configuration, preserving the declared JSON object.</summary>
    [JsonPropertyName("policy_details")] public JsonElement? PolicyDetails { get; init; }
    /// <summary>Timestamp when this resource was created.</summary>
    [JsonPropertyName("date_created")] public string? DateCreated { get; init; }
    /// <summary>Timestamp of the most recent modification to this resource.</summary>
    [JsonPropertyName("date_updated")] public string? DateUpdated { get; init; }
}

/// <summary>Page of policy resources with continuation metadata.</summary>
public sealed record AssistantsV1PolicyList : IV1PagedList
{
    /// <summary>The returned policies in this page.</summary>
    [JsonPropertyName("policies")] public List<AssistantsV1Policy> Policies { get; init; } = new();
    /// <summary>Pagination metadata; follow next_page_url to fetch the next page.</summary>
    [JsonPropertyName("meta")] public VoiceV1Meta? Meta { get; init; }
}

// ---- Request bodies (application/json) -------------------------------------

/// <summary>Optional Customer-AI engine toggles on Assistant create/update. Spec lists
/// these specific keys; the server accepts any free-form JSON object so an arbitrary
/// <see cref="JsonElement"/> can be sent via the <see cref="CreateAssistantRequest.CustomerAi"/>
/// extension point if you need extra fields.</summary>
public sealed record AssistantsV1CustomerAi
{
    /// <summary>Whether perception engine enabled is enabled for this operation.</summary>
    [JsonPropertyName("perception_engine_enabled")] public bool? PerceptionEngineEnabled { get; init; }
    /// <summary>Whether personalization engine enabled is enabled for this operation.</summary>
    [JsonPropertyName("personalization_engine_enabled")] public bool? PersonalizationEngineEnabled { get; init; }
}

/// <summary>Page-size query for Assistants v1 list endpoints (some support page tokens too).</summary>
public sealed record ListAssistantsV1PageParams
{
    /// <summary>Number of items per page (server cap applies).</summary>
    public int? PageSize { get; init; }

    /// <summary>Page index (zero-based) — only honoured on a subset of list endpoints per spec.</summary>
    public int? Page { get; init; }

    /// <summary>Opaque continuation token from a previous list response.</summary>
    public string? PageToken { get; init; }

    /// <summary>Render the declared filters as query pairs; null values are omitted by the transport.</summary>
    public IEnumerable<KeyValuePair<string, string?>> ToQuery()
    {
        yield return new("PageSize", PageSize?.ToString());
        yield return new("Page", Page?.ToString());
        yield return new("PageToken", PageToken);
    }
}

/// <summary>Query for the Assistant-scoped and Session-scoped list endpoints that declare
/// only a <c>PageSize</c> parameter (assistant tools/knowledge/feedbacks, knowledge chunks,
/// sessions, session messages).</summary>
public sealed record AssistantsV1PageSizeParams
{
    /// <summary>Number of items per page (server cap applies).</summary>
    public int? PageSize { get; init; }

    /// <summary>Render as a query-parameter sequence.</summary>
    public IEnumerable<KeyValuePair<string, string?>> ToQuery()
    {
        yield return new("PageSize", PageSize?.ToString());
    }
}

/// <summary>Query for <c>GET /v1/Tools</c>.</summary>
public sealed record ListAssistantsV1ToolsParams
{
    /// <summary>If set, filter to tools attached to this Assistant id.</summary>
    public string? AssistantId { get; init; }
    /// <summary>Requested number of items per page.</summary>
    public int? PageSize { get; init; }

    /// <summary>Render the declared filters as query pairs; null values are omitted by the transport.</summary>
    public IEnumerable<KeyValuePair<string, string?>> ToQuery()
    {
        yield return new("AssistantId", AssistantId);
        yield return new("PageSize", PageSize?.ToString());
    }
}

/// <summary>Query for <c>GET /v1/Knowledge</c>.</summary>
public sealed record ListAssistantsV1KnowledgeParams
{
    /// <summary>If set, filter to knowledge attached to this Assistant id.</summary>
    public string? AssistantId { get; init; }
    /// <summary>Requested number of items per page.</summary>
    public int? PageSize { get; init; }

    /// <summary>Render the declared filters as query pairs; null values are omitted by the transport.</summary>
    public IEnumerable<KeyValuePair<string, string?>> ToQuery()
    {
        yield return new("AssistantId", AssistantId);
        yield return new("PageSize", PageSize?.ToString());
    }
}

/// <summary>Query for <c>GET /v1/Policies</c>.</summary>
public sealed record ListAssistantsV1PoliciesParams
{
    /// <summary>If set, filter to policies attached to this Tool id.</summary>
    public string? ToolId { get; init; }

    /// <summary>If set, filter to policies attached to this Knowledge id.</summary>
    public string? KnowledgeId { get; init; }
    /// <summary>Requested number of items per page.</summary>
    public int? PageSize { get; init; }

    /// <summary>Render the declared filters as query pairs; null values are omitted by the transport.</summary>
    public IEnumerable<KeyValuePair<string, string?>> ToQuery()
    {
        yield return new("ToolId", ToolId);
        yield return new("KnowledgeId", KnowledgeId);
        yield return new("PageSize", PageSize?.ToString());
    }
}

/// <summary>Body of <c>POST /v1/Assistants</c>. <c>Name</c> is required.</summary>
public sealed record CreateAssistantRequest
{
    /// <summary>Human-readable name used to identify the Assistant, tool, knowledge source or policy.</summary>
    [JsonPropertyName("name")] public required string Name { get; init; }
    /// <summary>Application-defined owner of this Assistant.</summary>
    [JsonPropertyName("owner")] public string? Owner { get; init; }
    /// <summary>Instructions describing the Assistant personality and expected behavior.</summary>
    [JsonPropertyName("personality_prompt")] public string? PersonalityPrompt { get; init; }

    /// <summary>VoiceML extension: the BYO-LLM model backing the assistant.</summary>
    [JsonPropertyName("model")] public string? Model { get; init; }

    /// <summary>Typed Customer-AI engine toggles.</summary>
    [JsonPropertyName("customer_ai")] public AssistantsV1CustomerAi? CustomerAi { get; init; }

    /// <summary>Free-form Segment credential payload (opaque per spec).</summary>
    [JsonPropertyName("segment_credential")] public JsonElement? SegmentCredential { get; init; }
}

/// <summary>Body of <c>PUT /v1/Assistants/{id}</c>. All fields optional (partial update).</summary>
public sealed record UpdateAssistantRequest
{
    /// <summary>Human-readable name used to identify the Assistant, tool, knowledge source or policy.</summary>
    [JsonPropertyName("name")] public string? Name { get; init; }
    /// <summary>Application-defined owner of this Assistant.</summary>
    [JsonPropertyName("owner")] public string? Owner { get; init; }
    /// <summary>Instructions describing the Assistant personality and expected behavior.</summary>
    [JsonPropertyName("personality_prompt")] public string? PersonalityPrompt { get; init; }
    /// <summary>VoiceML extension: the BYO-LLM model backing the assistant (Phase 6).</summary>
    [JsonPropertyName("model")] public string? Model { get; init; }
    /// <summary>Customer-specific inference-provider settings, preserving the declared configuration object.</summary>
    [JsonPropertyName("customer_ai")] public AssistantsV1CustomerAi? CustomerAi { get; init; }
    /// <summary>Credential used for the Assistant customer-data integration.</summary>
    [JsonPropertyName("segment_credential")] public JsonElement? SegmentCredential { get; init; }
}

/// <summary>Body of <c>POST /v1/Tools</c>. <c>Name</c>, <c>Type</c>, <c>Enabled</c> are required.</summary>
public sealed record CreateAssistantToolRequest
{
    /// <summary>Human-readable name used to identify the Assistant, tool, knowledge source or policy.</summary>
    [JsonPropertyName("name")] public required string Name { get; init; }
    /// <summary>Provider-defined category identifying the kind of resource or channel.</summary>
    [JsonPropertyName("type")] public required string Type { get; init; }
    /// <summary>Whether the target is active. Default true.</summary>
    [JsonPropertyName("enabled")] public required bool Enabled { get; init; }
    /// <summary>Identifier of the assistant associated with this resource.</summary>
    [JsonPropertyName("assistant_id")] public string? AssistantId { get; init; }
    /// <summary>Human-readable explanation of the purpose of the tool, knowledge source or policy.</summary>
    [JsonPropertyName("description")] public string? Description { get; init; }
    /// <summary>Pagination metadata; follow next_page_url to fetch the next page.</summary>
    [JsonPropertyName("meta")] public JsonElement? Meta { get; init; }
}

/// <summary>Body of <c>PUT /v1/Tools/{id}</c>. All fields optional.</summary>
public sealed record UpdateAssistantToolRequest
{
    /// <summary>Human-readable name used to identify the Assistant, tool, knowledge source or policy.</summary>
    [JsonPropertyName("name")] public string? Name { get; init; }
    /// <summary>Provider-defined category identifying the kind of resource or channel.</summary>
    [JsonPropertyName("type")] public string? Type { get; init; }
    /// <summary>Whether the target is active. Default true.</summary>
    [JsonPropertyName("enabled")] public bool? Enabled { get; init; }
    /// <summary>Human-readable explanation of the purpose of the tool, knowledge source or policy.</summary>
    [JsonPropertyName("description")] public string? Description { get; init; }
    /// <summary>Pagination metadata; follow next_page_url to fetch the next page.</summary>
    [JsonPropertyName("meta")] public JsonElement? Meta { get; init; }
}

/// <summary>Body of <c>POST /v1/Knowledge</c>. <c>Name</c> and <c>Type</c> are required.</summary>
public sealed record CreateAssistantKnowledgeRequest
{
    /// <summary>Human-readable name used to identify the Assistant, tool, knowledge source or policy.</summary>
    [JsonPropertyName("name")] public required string Name { get; init; }
    /// <summary>Provider-defined category identifying the kind of resource or channel.</summary>
    [JsonPropertyName("type")] public required string Type { get; init; }
    /// <summary>Identifier of the assistant associated with this resource.</summary>
    [JsonPropertyName("assistant_id")] public string? AssistantId { get; init; }
    /// <summary>Human-readable explanation of the purpose of the tool, knowledge source or policy.</summary>
    [JsonPropertyName("description")] public string? Description { get; init; }
    /// <summary>Model identifier used to generate embeddings for knowledge indexing.</summary>
    [JsonPropertyName("embedding_model")] public string? EmbeddingModel { get; init; }
    /// <summary>Source-specific ingestion settings, such as the document URL or uploaded content.</summary>
    [JsonPropertyName("knowledge_source_details")] public JsonElement? KnowledgeSourceDetails { get; init; }
}

/// <summary>Body of <c>PUT /v1/Knowledge/{id}</c>. All fields optional.</summary>
public sealed record UpdateAssistantKnowledgeRequest
{
    /// <summary>Human-readable name used to identify the Assistant, tool, knowledge source or policy.</summary>
    [JsonPropertyName("name")] public string? Name { get; init; }
    /// <summary>Provider-defined category identifying the kind of resource or channel.</summary>
    [JsonPropertyName("type")] public string? Type { get; init; }
    /// <summary>Human-readable explanation of the purpose of the tool, knowledge source or policy.</summary>
    [JsonPropertyName("description")] public string? Description { get; init; }
    /// <summary>Model identifier used to generate embeddings for knowledge indexing.</summary>
    [JsonPropertyName("embedding_model")] public string? EmbeddingModel { get; init; }
    /// <summary>Source-specific ingestion settings, such as the document URL or uploaded content.</summary>
    [JsonPropertyName("knowledge_source_details")] public JsonElement? KnowledgeSourceDetails { get; init; }
}

/// <summary>Body of <c>POST /v1/Assistants/{id}/Messages</c>. <c>Identity</c> and <c>Body</c>
/// are required. If <c>SessionId</c> is omitted the server creates one.</summary>
public sealed record SendAssistantMessageRequest
{
    /// <summary>Application-defined user identity.</summary>
    [JsonPropertyName("identity")] public required string Identity { get; init; }
    /// <summary>Text content of the message.</summary>
    [JsonPropertyName("body")] public required string Body { get; init; }
    /// <summary>Identifier of the session associated with this resource.</summary>
    [JsonPropertyName("session_id")] public string? SessionId { get; init; }
    /// <summary>URL of the endpoint used to execute the Assistant tool.</summary>
    [JsonPropertyName("webhook")] public string? Webhook { get; init; }
    /// <summary>Policy enforcement mode applied when evaluating this Assistant policy.</summary>
    [JsonPropertyName("mode")] public string? Mode { get; init; }
}

/// <summary>Body of <c>POST /v1/Assistants/{id}/Feedbacks</c>. <c>SessionId</c> is required.</summary>
public sealed record CreateAssistantFeedbackRequest
{
    /// <summary>Identifier of the session associated with this resource.</summary>
    [JsonPropertyName("session_id")] public required string SessionId { get; init; }
    /// <summary>Identifier of the message associated with this resource.</summary>
    [JsonPropertyName("message_id")] public string? MessageId { get; init; }
    /// <summary>Feedback score assigned to the Assistant response.</summary>
    [JsonPropertyName("score")] public float? Score { get; init; }
    /// <summary>Free-form feedback explaining the assigned Assistant response score.</summary>
    [JsonPropertyName("text")] public string? Text { get; init; }
}
