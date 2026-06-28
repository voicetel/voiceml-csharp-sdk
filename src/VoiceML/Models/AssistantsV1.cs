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
    [JsonPropertyName("account_sid")] public string? AccountSid { get; init; }
    [JsonPropertyName("id")] public string? Id { get; init; }
    [JsonPropertyName("name")] public string? Name { get; init; }
    [JsonPropertyName("owner")] public string? Owner { get; init; }
    [JsonPropertyName("model")] public string? Model { get; init; }
    [JsonPropertyName("personality_prompt")] public string? PersonalityPrompt { get; init; }
    [JsonPropertyName("customer_ai")] public JsonElement? CustomerAi { get; init; }
    [JsonPropertyName("url")] public string? Url { get; init; }
    [JsonPropertyName("date_created")] public string? DateCreated { get; init; }
    [JsonPropertyName("date_updated")] public string? DateUpdated { get; init; }
}

public sealed record AssistantsV1AssistantList
{
    [JsonPropertyName("assistants")] public List<AssistantsV1Assistant> Assistants { get; init; } = new();
    [JsonPropertyName("meta")] public VoiceV1Meta? Meta { get; init; }
}

/// <summary>An Assistant with its attached Tools and Knowledge inlined.
/// Returned by <c>GET /v1/Assistants/{id}</c>.</summary>
public sealed record AssistantsV1AssistantWithToolsAndKnowledge
{
    [JsonPropertyName("account_sid")] public string? AccountSid { get; init; }
    [JsonPropertyName("id")] public string? Id { get; init; }
    [JsonPropertyName("name")] public string? Name { get; init; }
    [JsonPropertyName("owner")] public string? Owner { get; init; }
    [JsonPropertyName("model")] public string? Model { get; init; }
    [JsonPropertyName("personality_prompt")] public string? PersonalityPrompt { get; init; }
    [JsonPropertyName("customer_ai")] public JsonElement? CustomerAi { get; init; }
    [JsonPropertyName("url")] public string? Url { get; init; }
    [JsonPropertyName("date_created")] public string? DateCreated { get; init; }
    [JsonPropertyName("date_updated")] public string? DateUpdated { get; init; }
    [JsonPropertyName("tools")] public List<AssistantsV1Tool> Tools { get; init; } = new();
    [JsonPropertyName("knowledge")] public List<AssistantsV1Knowledge> Knowledge { get; init; } = new();
}

/// <summary>A Tool — identified by <c>aia_tool_…</c>. Tools can be defined globally
/// and attached to one or more Assistants.</summary>
public sealed record AssistantsV1Tool
{
    [JsonPropertyName("account_sid")] public string? AccountSid { get; init; }
    [JsonPropertyName("id")] public string? Id { get; init; }
    [JsonPropertyName("name")] public string? Name { get; init; }
    [JsonPropertyName("type")] public string? Type { get; init; }
    [JsonPropertyName("description")] public string? Description { get; init; }
    [JsonPropertyName("enabled")] public bool Enabled { get; init; }
    [JsonPropertyName("requires_auth")] public bool RequiresAuth { get; init; }
    [JsonPropertyName("meta")] public JsonElement? Meta { get; init; }
    [JsonPropertyName("url")] public string? Url { get; init; }
    [JsonPropertyName("date_created")] public string? DateCreated { get; init; }
    [JsonPropertyName("date_updated")] public string? DateUpdated { get; init; }
}

public sealed record AssistantsV1ToolList
{
    [JsonPropertyName("tools")] public List<AssistantsV1Tool> Tools { get; init; } = new();
    [JsonPropertyName("meta")] public VoiceV1Meta? Meta { get; init; }
}

/// <summary>A Tool with its attached Policies inlined.
/// Returned by <c>GET /v1/Tools/{id}</c>.</summary>
public sealed record AssistantsV1ToolWithPolicies
{
    [JsonPropertyName("account_sid")] public string? AccountSid { get; init; }
    [JsonPropertyName("id")] public string? Id { get; init; }
    [JsonPropertyName("name")] public string? Name { get; init; }
    [JsonPropertyName("type")] public string? Type { get; init; }
    [JsonPropertyName("description")] public string? Description { get; init; }
    [JsonPropertyName("enabled")] public bool Enabled { get; init; }
    [JsonPropertyName("requires_auth")] public bool RequiresAuth { get; init; }
    [JsonPropertyName("meta")] public JsonElement? Meta { get; init; }
    [JsonPropertyName("url")] public string? Url { get; init; }
    [JsonPropertyName("date_created")] public string? DateCreated { get; init; }
    [JsonPropertyName("date_updated")] public string? DateUpdated { get; init; }
    [JsonPropertyName("policies")] public List<AssistantsV1Policy> Policies { get; init; } = new();
}

/// <summary>A Knowledge source — identified by <c>aia_know_…</c>. Knowledge can be
/// defined globally and attached to one or more Assistants.</summary>
public sealed record AssistantsV1Knowledge
{
    [JsonPropertyName("account_sid")] public string? AccountSid { get; init; }
    [JsonPropertyName("id")] public string? Id { get; init; }
    [JsonPropertyName("name")] public string? Name { get; init; }
    [JsonPropertyName("type")] public string? Type { get; init; }
    [JsonPropertyName("description")] public string? Description { get; init; }
    [JsonPropertyName("status")] public string? Status { get; init; }
    [JsonPropertyName("embedding_model")] public string? EmbeddingModel { get; init; }
    [JsonPropertyName("knowledge_source_details")] public JsonElement? KnowledgeSourceDetails { get; init; }
    [JsonPropertyName("url")] public string? Url { get; init; }
    [JsonPropertyName("date_created")] public string? DateCreated { get; init; }
    [JsonPropertyName("date_updated")] public string? DateUpdated { get; init; }
}

public sealed record AssistantsV1KnowledgeList
{
    [JsonPropertyName("knowledge")] public List<AssistantsV1Knowledge> Knowledge { get; init; } = new();
    [JsonPropertyName("meta")] public VoiceV1Meta? Meta { get; init; }
}

/// <summary>Ingestion status snapshot for a Knowledge source. Returned by
/// <c>GET /v1/Knowledge/{id}/Status</c>.</summary>
public sealed record AssistantsV1KnowledgeStatus
{
    [JsonPropertyName("account_sid")] public string? AccountSid { get; init; }
    [JsonPropertyName("status")] public string? Status { get; init; }
    [JsonPropertyName("last_status")] public string? LastStatus { get; init; }
    [JsonPropertyName("date_updated")] public string? DateUpdated { get; init; }
}

/// <summary>A single chunk produced by ingesting a Knowledge source.</summary>
public sealed record AssistantsV1KnowledgeChunk
{
    [JsonPropertyName("account_sid")] public string? AccountSid { get; init; }
    [JsonPropertyName("content")] public string? Content { get; init; }
    [JsonPropertyName("metadata")] public JsonElement? Metadata { get; init; }
    [JsonPropertyName("date_created")] public string? DateCreated { get; init; }
    [JsonPropertyName("date_updated")] public string? DateUpdated { get; init; }
}

public sealed record AssistantsV1KnowledgeChunkList
{
    [JsonPropertyName("chunks")] public List<AssistantsV1KnowledgeChunk> Chunks { get; init; } = new();
    [JsonPropertyName("meta")] public VoiceV1Meta? Meta { get; init; }
}

/// <summary>An Assistant Session — a conversation turn-state per identity.</summary>
public sealed record AssistantsV1Session
{
    [JsonPropertyName("id")] public string? Id { get; init; }
    [JsonPropertyName("account_sid")] public string? AccountSid { get; init; }
    [JsonPropertyName("assistant_id")] public string? AssistantId { get; init; }
    [JsonPropertyName("verified")] public bool? Verified { get; init; }
    [JsonPropertyName("identity")] public string? Identity { get; init; }
    [JsonPropertyName("date_created")] public string? DateCreated { get; init; }
    [JsonPropertyName("date_updated")] public string? DateUpdated { get; init; }
}

public sealed record AssistantsV1SessionList
{
    [JsonPropertyName("sessions")] public List<AssistantsV1Session> Sessions { get; init; } = new();
    [JsonPropertyName("meta")] public VoiceV1Meta? Meta { get; init; }
}

/// <summary>A Message inside a Session — identified by <c>aia_msg_…</c>.</summary>
public sealed record AssistantsV1Message
{
    [JsonPropertyName("id")] public string? Id { get; init; }
    [JsonPropertyName("account_sid")] public string? AccountSid { get; init; }
    [JsonPropertyName("assistant_id")] public string? AssistantId { get; init; }
    [JsonPropertyName("session_id")] public string? SessionId { get; init; }
    [JsonPropertyName("identity")] public string? Identity { get; init; }
    [JsonPropertyName("role")] public string? Role { get; init; }
    [JsonPropertyName("content")] public JsonElement? Content { get; init; }
    [JsonPropertyName("meta")] public JsonElement? Meta { get; init; }
    [JsonPropertyName("date_created")] public string? DateCreated { get; init; }
    [JsonPropertyName("date_updated")] public string? DateUpdated { get; init; }
}

public sealed record AssistantsV1MessageList
{
    [JsonPropertyName("messages")] public List<AssistantsV1Message> Messages { get; init; } = new();
    [JsonPropertyName("meta")] public VoiceV1Meta? Meta { get; init; }
}

/// <summary>Result of sending a message to an Assistant via
/// <c>POST /v1/Assistants/{id}/Messages</c>. Returns the (possibly
/// newly-created) session id plus the synchronous reply.</summary>
public sealed record AssistantsV1SendMessageResponse
{
    [JsonPropertyName("status")] public string? Status { get; init; }
    [JsonPropertyName("flagged")] public bool? Flagged { get; init; }
    [JsonPropertyName("aborted")] public bool? Aborted { get; init; }
    [JsonPropertyName("session_id")] public string? SessionId { get; init; }
    [JsonPropertyName("account_sid")] public string? AccountSid { get; init; }
    [JsonPropertyName("body")] public string? Body { get; init; }
    [JsonPropertyName("error")] public string? Error { get; init; }
}

/// <summary>Feedback on a specific Assistant message — identified by <c>aia_fdbk_…</c>.</summary>
public sealed record AssistantsV1Feedback
{
    [JsonPropertyName("id")] public string? Id { get; init; }
    [JsonPropertyName("account_sid")] public string? AccountSid { get; init; }
    [JsonPropertyName("user_sid")] public string? UserSid { get; init; }
    [JsonPropertyName("assistant_id")] public string? AssistantId { get; init; }
    [JsonPropertyName("session_id")] public string? SessionId { get; init; }
    [JsonPropertyName("message_id")] public string? MessageId { get; init; }
    [JsonPropertyName("score")] public float? Score { get; init; }
    [JsonPropertyName("text")] public string? Text { get; init; }
    [JsonPropertyName("date_created")] public string? DateCreated { get; init; }
    [JsonPropertyName("date_updated")] public string? DateUpdated { get; init; }
}

public sealed record AssistantsV1FeedbackList
{
    [JsonPropertyName("feedbacks")] public List<AssistantsV1Feedback> Feedbacks { get; init; } = new();
    [JsonPropertyName("meta")] public VoiceV1Meta? Meta { get; init; }
}

/// <summary>A Policy — identified by <c>aia_plcy_…</c>. Policies attach to Tools or Knowledge.</summary>
public sealed record AssistantsV1Policy
{
    [JsonPropertyName("id")] public string? Id { get; init; }
    [JsonPropertyName("account_sid")] public string? AccountSid { get; init; }
    [JsonPropertyName("user_sid")] public string? UserSid { get; init; }
    [JsonPropertyName("name")] public string? Name { get; init; }
    [JsonPropertyName("description")] public string? Description { get; init; }
    [JsonPropertyName("type")] public string? Type { get; init; }
    [JsonPropertyName("policy_details")] public JsonElement? PolicyDetails { get; init; }
    [JsonPropertyName("date_created")] public string? DateCreated { get; init; }
    [JsonPropertyName("date_updated")] public string? DateUpdated { get; init; }
}

public sealed record AssistantsV1PolicyList
{
    [JsonPropertyName("policies")] public List<AssistantsV1Policy> Policies { get; init; } = new();
    [JsonPropertyName("meta")] public VoiceV1Meta? Meta { get; init; }
}

// ---- Request bodies (application/json) -------------------------------------

/// <summary>Optional Customer-AI engine toggles on Assistant create/update. Spec lists
/// these specific keys; the server accepts any free-form JSON object so an arbitrary
/// <see cref="JsonElement"/> can be sent via the <see cref="CreateAssistantRequest.CustomerAi"/>
/// extension point if you need extra fields.</summary>
public sealed record AssistantsV1CustomerAi
{
    [JsonPropertyName("perception_engine_enabled")] public bool? PerceptionEngineEnabled { get; init; }
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

    public IEnumerable<KeyValuePair<string, string?>> ToQuery()
    {
        yield return new("PageSize", PageSize?.ToString());
        yield return new("Page", Page?.ToString());
        yield return new("PageToken", PageToken);
    }
}

/// <summary>Query for <c>GET /v1/Tools</c>.</summary>
public sealed record ListAssistantsV1ToolsParams
{
    /// <summary>If set, filter to tools attached to this Assistant id.</summary>
    public string? AssistantId { get; init; }
    public int? PageSize { get; init; }

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
    public int? PageSize { get; init; }

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
    public int? PageSize { get; init; }

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
    [JsonPropertyName("name")] public required string Name { get; init; }
    [JsonPropertyName("owner")] public string? Owner { get; init; }
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
    [JsonPropertyName("name")] public string? Name { get; init; }
    [JsonPropertyName("owner")] public string? Owner { get; init; }
    [JsonPropertyName("personality_prompt")] public string? PersonalityPrompt { get; init; }
    [JsonPropertyName("model")] public string? Model { get; init; }
    [JsonPropertyName("customer_ai")] public AssistantsV1CustomerAi? CustomerAi { get; init; }
    [JsonPropertyName("segment_credential")] public JsonElement? SegmentCredential { get; init; }
}

/// <summary>Body of <c>POST /v1/Tools</c>. <c>Name</c>, <c>Type</c>, <c>Enabled</c> are required.</summary>
public sealed record CreateAssistantToolRequest
{
    [JsonPropertyName("name")] public required string Name { get; init; }
    [JsonPropertyName("type")] public required string Type { get; init; }
    [JsonPropertyName("enabled")] public required bool Enabled { get; init; }
    [JsonPropertyName("assistant_id")] public string? AssistantId { get; init; }
    [JsonPropertyName("description")] public string? Description { get; init; }
    [JsonPropertyName("meta")] public JsonElement? Meta { get; init; }
}

/// <summary>Body of <c>PUT /v1/Tools/{id}</c>. All fields optional.</summary>
public sealed record UpdateAssistantToolRequest
{
    [JsonPropertyName("name")] public string? Name { get; init; }
    [JsonPropertyName("type")] public string? Type { get; init; }
    [JsonPropertyName("enabled")] public bool? Enabled { get; init; }
    [JsonPropertyName("description")] public string? Description { get; init; }
    [JsonPropertyName("meta")] public JsonElement? Meta { get; init; }
}

/// <summary>Body of <c>POST /v1/Knowledge</c>. <c>Name</c> and <c>Type</c> are required.</summary>
public sealed record CreateAssistantKnowledgeRequest
{
    [JsonPropertyName("name")] public required string Name { get; init; }
    [JsonPropertyName("type")] public required string Type { get; init; }
    [JsonPropertyName("assistant_id")] public string? AssistantId { get; init; }
    [JsonPropertyName("description")] public string? Description { get; init; }
    [JsonPropertyName("embedding_model")] public string? EmbeddingModel { get; init; }
    [JsonPropertyName("knowledge_source_details")] public JsonElement? KnowledgeSourceDetails { get; init; }
}

/// <summary>Body of <c>PUT /v1/Knowledge/{id}</c>. All fields optional.</summary>
public sealed record UpdateAssistantKnowledgeRequest
{
    [JsonPropertyName("name")] public string? Name { get; init; }
    [JsonPropertyName("type")] public string? Type { get; init; }
    [JsonPropertyName("description")] public string? Description { get; init; }
    [JsonPropertyName("embedding_model")] public string? EmbeddingModel { get; init; }
    [JsonPropertyName("knowledge_source_details")] public JsonElement? KnowledgeSourceDetails { get; init; }
}

/// <summary>Body of <c>POST /v1/Assistants/{id}/Messages</c>. <c>Identity</c> and <c>Body</c>
/// are required. If <c>SessionId</c> is omitted the server creates one.</summary>
public sealed record SendAssistantMessageRequest
{
    [JsonPropertyName("identity")] public required string Identity { get; init; }
    [JsonPropertyName("body")] public required string Body { get; init; }
    [JsonPropertyName("session_id")] public string? SessionId { get; init; }
    [JsonPropertyName("webhook")] public string? Webhook { get; init; }
    [JsonPropertyName("mode")] public string? Mode { get; init; }
}

/// <summary>Body of <c>POST /v1/Assistants/{id}/Feedbacks</c>. <c>SessionId</c> is required.</summary>
public sealed record CreateAssistantFeedbackRequest
{
    [JsonPropertyName("session_id")] public required string SessionId { get; init; }
    [JsonPropertyName("message_id")] public string? MessageId { get; init; }
    [JsonPropertyName("score")] public float? Score { get; init; }
    [JsonPropertyName("text")] public string? Text { get; init; }
}
