using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace VoiceML.Models;

// ===========================================================================
// Conversations v1 — Phase 4 service-scoped surface
// (conversations.twilio.com/v1/Services/{ChatServiceSid}/…)
//
// 15 resource families, 48 ops, all isolation-boundary scoped under a chat
// service sid (IS…). Field shapes mirror the account-level Conversations v1
// records but add `chat_service_sid` and live in their own namespace per spec.
// ===========================================================================

// ---- Response shapes -------------------------------------------------------

/// <summary>Service-scoped Conversation — <c>CH…</c> under <c>/v1/Services/{IS…}/Conversations</c>.</summary>
public sealed record ConversationsV1ServiceConversation
{
    [JsonPropertyName("account_sid")] public string? AccountSid { get; init; }
    [JsonPropertyName("chat_service_sid")] public string? ChatServiceSid { get; init; }
    [JsonPropertyName("messaging_service_sid")] public string? MessagingServiceSid { get; init; }
    [JsonPropertyName("sid")] public string? Sid { get; init; }
    [JsonPropertyName("friendly_name")] public string? FriendlyName { get; init; }
    [JsonPropertyName("unique_name")] public string? UniqueName { get; init; }
    [JsonPropertyName("attributes")] public string? Attributes { get; init; }
    [JsonPropertyName("state")] public string? State { get; init; }
    [JsonPropertyName("date_created")] public string? DateCreated { get; init; }
    [JsonPropertyName("date_updated")] public string? DateUpdated { get; init; }
    [JsonPropertyName("timers")] public JsonElement? Timers { get; init; }
    [JsonPropertyName("url")] public string? Url { get; init; }
    [JsonPropertyName("links")] public JsonElement? Links { get; init; }
    [JsonPropertyName("bindings")] public JsonElement? Bindings { get; init; }
}

public sealed record ConversationsV1ServiceConversationList
{
    [JsonPropertyName("conversations")] public List<ConversationsV1ServiceConversation> Conversations { get; init; } = new();
    [JsonPropertyName("meta")] public VoiceV1Meta? Meta { get; init; }
}

/// <summary>Service-scoped Conversation Message — <c>IM…</c>.</summary>
public sealed record ConversationsV1ServiceConversationMessage
{
    [JsonPropertyName("account_sid")] public string? AccountSid { get; init; }
    [JsonPropertyName("chat_service_sid")] public string? ChatServiceSid { get; init; }
    [JsonPropertyName("conversation_sid")] public string? ConversationSid { get; init; }
    [JsonPropertyName("sid")] public string? Sid { get; init; }
    [JsonPropertyName("index")] public int Index { get; init; }
    [JsonPropertyName("author")] public string? Author { get; init; }
    [JsonPropertyName("body")] public string? Body { get; init; }
    [JsonPropertyName("media")] public JsonElement? Media { get; init; }
    [JsonPropertyName("attributes")] public string? Attributes { get; init; }
    [JsonPropertyName("participant_sid")] public string? ParticipantSid { get; init; }
    [JsonPropertyName("date_created")] public string? DateCreated { get; init; }
    [JsonPropertyName("date_updated")] public string? DateUpdated { get; init; }
    [JsonPropertyName("url")] public string? Url { get; init; }
    [JsonPropertyName("delivery")] public JsonElement? Delivery { get; init; }
    [JsonPropertyName("links")] public JsonElement? Links { get; init; }
    [JsonPropertyName("content_sid")] public string? ContentSid { get; init; }
}

public sealed record ConversationsV1ServiceConversationMessageList
{
    [JsonPropertyName("messages")] public List<ConversationsV1ServiceConversationMessage> Messages { get; init; } = new();
    [JsonPropertyName("meta")] public VoiceV1Meta? Meta { get; init; }
}

/// <summary>Service-scoped Conversation Participant — <c>MB…</c>.</summary>
public sealed record ConversationsV1ServiceConversationParticipant
{
    [JsonPropertyName("account_sid")] public string? AccountSid { get; init; }
    [JsonPropertyName("chat_service_sid")] public string? ChatServiceSid { get; init; }
    [JsonPropertyName("conversation_sid")] public string? ConversationSid { get; init; }
    [JsonPropertyName("sid")] public string? Sid { get; init; }
    [JsonPropertyName("identity")] public string? Identity { get; init; }
    [JsonPropertyName("attributes")] public string? Attributes { get; init; }
    [JsonPropertyName("messaging_binding")] public JsonElement? MessagingBinding { get; init; }
    [JsonPropertyName("role_sid")] public string? RoleSid { get; init; }
    [JsonPropertyName("date_created")] public string? DateCreated { get; init; }
    [JsonPropertyName("date_updated")] public string? DateUpdated { get; init; }
    [JsonPropertyName("url")] public string? Url { get; init; }
    [JsonPropertyName("last_read_message_index")] public int? LastReadMessageIndex { get; init; }
    [JsonPropertyName("last_read_timestamp")] public string? LastReadTimestamp { get; init; }
}

public sealed record ConversationsV1ServiceConversationParticipantList
{
    [JsonPropertyName("participants")] public List<ConversationsV1ServiceConversationParticipant> Participants { get; init; } = new();
    [JsonPropertyName("meta")] public VoiceV1Meta? Meta { get; init; }
}

/// <summary>Service-scoped Conversation Message Receipt — <c>DY…</c>.</summary>
public sealed record ConversationsV1ServiceConversationMessageReceipt
{
    [JsonPropertyName("account_sid")] public string? AccountSid { get; init; }
    [JsonPropertyName("chat_service_sid")] public string? ChatServiceSid { get; init; }
    [JsonPropertyName("conversation_sid")] public string? ConversationSid { get; init; }
    [JsonPropertyName("sid")] public string? Sid { get; init; }
    [JsonPropertyName("message_sid")] public string? MessageSid { get; init; }
    [JsonPropertyName("channel_message_sid")] public string? ChannelMessageSid { get; init; }
    [JsonPropertyName("participant_sid")] public string? ParticipantSid { get; init; }
    [JsonPropertyName("status")] public string? Status { get; init; }
    [JsonPropertyName("error_code")] public int ErrorCode { get; init; }
    [JsonPropertyName("date_created")] public string? DateCreated { get; init; }
    [JsonPropertyName("date_updated")] public string? DateUpdated { get; init; }
    [JsonPropertyName("url")] public string? Url { get; init; }
}

public sealed record ConversationsV1ServiceConversationMessageReceiptList
{
    [JsonPropertyName("delivery_receipts")] public List<ConversationsV1ServiceConversationMessageReceipt> DeliveryReceipts { get; init; } = new();
    [JsonPropertyName("meta")] public VoiceV1Meta? Meta { get; init; }
}

/// <summary>Service-scoped Conversation Scoped Webhook — <c>WH…</c>.</summary>
public sealed record ConversationsV1ServiceConversationScopedWebhook
{
    [JsonPropertyName("sid")] public string? Sid { get; init; }
    [JsonPropertyName("account_sid")] public string? AccountSid { get; init; }
    [JsonPropertyName("chat_service_sid")] public string? ChatServiceSid { get; init; }
    [JsonPropertyName("conversation_sid")] public string? ConversationSid { get; init; }
    [JsonPropertyName("target")] public string? Target { get; init; }
    [JsonPropertyName("url")] public string? Url { get; init; }
    [JsonPropertyName("configuration")] public JsonElement? Configuration { get; init; }
    [JsonPropertyName("date_created")] public string? DateCreated { get; init; }
    [JsonPropertyName("date_updated")] public string? DateUpdated { get; init; }
}

public sealed record ConversationsV1ServiceConversationScopedWebhookList
{
    [JsonPropertyName("webhooks")] public List<ConversationsV1ServiceConversationScopedWebhook> Webhooks { get; init; } = new();
    [JsonPropertyName("meta")] public VoiceV1Meta? Meta { get; init; }
}

/// <summary>Service-scoped Role — <c>RL…</c>.</summary>
public sealed record ConversationsV1ServiceRole
{
    [JsonPropertyName("sid")] public string? Sid { get; init; }
    [JsonPropertyName("account_sid")] public string? AccountSid { get; init; }
    [JsonPropertyName("chat_service_sid")] public string? ChatServiceSid { get; init; }
    [JsonPropertyName("friendly_name")] public string? FriendlyName { get; init; }
    [JsonPropertyName("type")] public string? Type { get; init; }
    [JsonPropertyName("permissions")] public List<string>? Permissions { get; init; }
    [JsonPropertyName("date_created")] public string? DateCreated { get; init; }
    [JsonPropertyName("date_updated")] public string? DateUpdated { get; init; }
    [JsonPropertyName("url")] public string? Url { get; init; }
}

public sealed record ConversationsV1ServiceRoleList
{
    [JsonPropertyName("roles")] public List<ConversationsV1ServiceRole> Roles { get; init; } = new();
    [JsonPropertyName("meta")] public VoiceV1Meta? Meta { get; init; }
}

/// <summary>Service-scoped User — <c>US…</c>.</summary>
public sealed record ConversationsV1ServiceUser
{
    [JsonPropertyName("sid")] public string? Sid { get; init; }
    [JsonPropertyName("account_sid")] public string? AccountSid { get; init; }
    [JsonPropertyName("chat_service_sid")] public string? ChatServiceSid { get; init; }
    [JsonPropertyName("role_sid")] public string? RoleSid { get; init; }
    [JsonPropertyName("identity")] public string? Identity { get; init; }
    [JsonPropertyName("friendly_name")] public string? FriendlyName { get; init; }
    [JsonPropertyName("attributes")] public string? Attributes { get; init; }
    [JsonPropertyName("is_online")] public bool? IsOnline { get; init; }
    [JsonPropertyName("is_notifiable")] public bool? IsNotifiable { get; init; }
    [JsonPropertyName("date_created")] public string? DateCreated { get; init; }
    [JsonPropertyName("date_updated")] public string? DateUpdated { get; init; }
    [JsonPropertyName("url")] public string? Url { get; init; }
    [JsonPropertyName("links")] public JsonElement? Links { get; init; }
}

public sealed record ConversationsV1ServiceUserList
{
    [JsonPropertyName("users")] public List<ConversationsV1ServiceUser> Users { get; init; } = new();
    [JsonPropertyName("meta")] public VoiceV1Meta? Meta { get; init; }
}

/// <summary>Service-scoped Conversation created via <c>/v1/Services/{IS…}/ConversationWithParticipants</c>.</summary>
public sealed record ConversationsV1ServiceConversationWithParticipants
{
    [JsonPropertyName("account_sid")] public string? AccountSid { get; init; }
    [JsonPropertyName("chat_service_sid")] public string? ChatServiceSid { get; init; }
    [JsonPropertyName("messaging_service_sid")] public string? MessagingServiceSid { get; init; }
    [JsonPropertyName("sid")] public string? Sid { get; init; }
    [JsonPropertyName("friendly_name")] public string? FriendlyName { get; init; }
    [JsonPropertyName("unique_name")] public string? UniqueName { get; init; }
    [JsonPropertyName("attributes")] public string? Attributes { get; init; }
    [JsonPropertyName("state")] public string? State { get; init; }
    [JsonPropertyName("date_created")] public string? DateCreated { get; init; }
    [JsonPropertyName("date_updated")] public string? DateUpdated { get; init; }
    [JsonPropertyName("timers")] public JsonElement? Timers { get; init; }
    [JsonPropertyName("links")] public JsonElement? Links { get; init; }
    [JsonPropertyName("bindings")] public JsonElement? Bindings { get; init; }
    [JsonPropertyName("url")] public string? Url { get; init; }
}

/// <summary>Service-scoped flattened participant + conversation view (read-only — list only).</summary>
public sealed record ConversationsV1ServiceParticipantConversation
{
    [JsonPropertyName("account_sid")] public string? AccountSid { get; init; }
    [JsonPropertyName("chat_service_sid")] public string? ChatServiceSid { get; init; }
    [JsonPropertyName("participant_sid")] public string? ParticipantSid { get; init; }
    [JsonPropertyName("participant_user_sid")] public string? ParticipantUserSid { get; init; }
    [JsonPropertyName("participant_identity")] public string? ParticipantIdentity { get; init; }
    [JsonPropertyName("participant_messaging_binding")] public JsonElement? ParticipantMessagingBinding { get; init; }
    [JsonPropertyName("conversation_sid")] public string? ConversationSid { get; init; }
    [JsonPropertyName("conversation_unique_name")] public string? ConversationUniqueName { get; init; }
    [JsonPropertyName("conversation_friendly_name")] public string? ConversationFriendlyName { get; init; }
    [JsonPropertyName("conversation_attributes")] public string? ConversationAttributes { get; init; }
    [JsonPropertyName("conversation_date_created")] public string? ConversationDateCreated { get; init; }
    [JsonPropertyName("conversation_date_updated")] public string? ConversationDateUpdated { get; init; }
    [JsonPropertyName("conversation_created_by")] public string? ConversationCreatedBy { get; init; }
    [JsonPropertyName("conversation_state")] public string? ConversationState { get; init; }
    [JsonPropertyName("conversation_timers")] public JsonElement? ConversationTimers { get; init; }
    [JsonPropertyName("links")] public JsonElement? Links { get; init; }
}

public sealed record ConversationsV1ServiceParticipantConversationList
{
    [JsonPropertyName("conversations")] public List<ConversationsV1ServiceParticipantConversation> Conversations { get; init; } = new();
    [JsonPropertyName("meta")] public VoiceV1Meta? Meta { get; init; }
}

/// <summary>Service-scoped per-user view of conversations they belong to (list only).</summary>
public sealed record ConversationsV1ServiceUserConversation
{
    [JsonPropertyName("account_sid")] public string? AccountSid { get; init; }
    [JsonPropertyName("chat_service_sid")] public string? ChatServiceSid { get; init; }
    [JsonPropertyName("conversation_sid")] public string? ConversationSid { get; init; }
    [JsonPropertyName("unread_messages_count")] public int? UnreadMessagesCount { get; init; }
    [JsonPropertyName("last_read_message_index")] public int? LastReadMessageIndex { get; init; }
    [JsonPropertyName("participant_sid")] public string? ParticipantSid { get; init; }
    [JsonPropertyName("user_sid")] public string? UserSid { get; init; }
    [JsonPropertyName("friendly_name")] public string? FriendlyName { get; init; }
    [JsonPropertyName("conversation_state")] public string? ConversationState { get; init; }
    [JsonPropertyName("timers")] public JsonElement? Timers { get; init; }
    [JsonPropertyName("attributes")] public string? Attributes { get; init; }
    [JsonPropertyName("date_created")] public string? DateCreated { get; init; }
    [JsonPropertyName("date_updated")] public string? DateUpdated { get; init; }
    [JsonPropertyName("created_by")] public string? CreatedBy { get; init; }
    [JsonPropertyName("notification_level")] public string? NotificationLevel { get; init; }
    [JsonPropertyName("unique_name")] public string? UniqueName { get; init; }
    [JsonPropertyName("url")] public string? Url { get; init; }
    [JsonPropertyName("links")] public JsonElement? Links { get; init; }
}

public sealed record ConversationsV1ServiceUserConversationList
{
    [JsonPropertyName("conversations")] public List<ConversationsV1ServiceUserConversation> Conversations { get; init; } = new();
    [JsonPropertyName("meta")] public VoiceV1Meta? Meta { get; init; }
}

/// <summary>Service-scoped push Binding — <c>BS…</c> (read+delete only).</summary>
public sealed record ConversationsV1ServiceBinding
{
    [JsonPropertyName("sid")] public string? Sid { get; init; }
    [JsonPropertyName("account_sid")] public string? AccountSid { get; init; }
    [JsonPropertyName("chat_service_sid")] public string? ChatServiceSid { get; init; }
    [JsonPropertyName("credential_sid")] public string? CredentialSid { get; init; }
    [JsonPropertyName("date_created")] public string? DateCreated { get; init; }
    [JsonPropertyName("date_updated")] public string? DateUpdated { get; init; }
    [JsonPropertyName("endpoint")] public string? Endpoint { get; init; }
    [JsonPropertyName("identity")] public string? Identity { get; init; }
    [JsonPropertyName("binding_type")] public string? BindingType { get; init; }
    [JsonPropertyName("message_types")] public List<string>? MessageTypes { get; init; }
    [JsonPropertyName("url")] public string? Url { get; init; }
}

public sealed record ConversationsV1ServiceBindingList
{
    [JsonPropertyName("bindings")] public List<ConversationsV1ServiceBinding> Bindings { get; init; } = new();
    [JsonPropertyName("meta")] public VoiceV1Meta? Meta { get; init; }
}

/// <summary>Per-service Configuration singleton (no sid — keyed by chat service).</summary>
public sealed record ConversationsV1ServiceConfiguration
{
    [JsonPropertyName("chat_service_sid")] public string? ChatServiceSid { get; init; }
    [JsonPropertyName("default_conversation_creator_role_sid")] public string? DefaultConversationCreatorRoleSid { get; init; }
    [JsonPropertyName("default_conversation_role_sid")] public string? DefaultConversationRoleSid { get; init; }
    [JsonPropertyName("default_chat_service_role_sid")] public string? DefaultChatServiceRoleSid { get; init; }
    [JsonPropertyName("url")] public string? Url { get; init; }
    [JsonPropertyName("links")] public JsonElement? Links { get; init; }
    [JsonPropertyName("reachability_enabled")] public bool? ReachabilityEnabled { get; init; }
}

/// <summary>Per-service push Notification configuration singleton.</summary>
public sealed record ConversationsV1ServiceNotification
{
    [JsonPropertyName("account_sid")] public string? AccountSid { get; init; }
    [JsonPropertyName("chat_service_sid")] public string? ChatServiceSid { get; init; }
    [JsonPropertyName("new_message")] public JsonElement? NewMessage { get; init; }
    [JsonPropertyName("added_to_conversation")] public JsonElement? AddedToConversation { get; init; }
    [JsonPropertyName("removed_from_conversation")] public JsonElement? RemovedFromConversation { get; init; }
    [JsonPropertyName("log_enabled")] public bool? LogEnabled { get; init; }
    [JsonPropertyName("url")] public string? Url { get; init; }
}

/// <summary>Per-service Webhook configuration singleton.</summary>
public sealed record ConversationsV1ServiceWebhookConfiguration
{
    [JsonPropertyName("account_sid")] public string? AccountSid { get; init; }
    [JsonPropertyName("chat_service_sid")] public string? ChatServiceSid { get; init; }
    [JsonPropertyName("pre_webhook_url")] public string? PreWebhookUrl { get; init; }
    [JsonPropertyName("post_webhook_url")] public string? PostWebhookUrl { get; init; }
    [JsonPropertyName("filters")] public List<string>? Filters { get; init; }
    [JsonPropertyName("method")] public string? Method { get; init; }
    [JsonPropertyName("url")] public string? Url { get; init; }
}

// ---- Request bodies (form-encoded) ----------------------------------------

public sealed record CreateServiceConversationRequest : IFormSerializable
{
    public string? FriendlyName { get; init; }
    public string? UniqueName { get; init; }
    public string? MessagingServiceSid { get; init; }
    public string? Attributes { get; init; }
    public string? State { get; init; }
    public string? TimersInactive { get; init; }
    public string? TimersClosed { get; init; }

    public IEnumerable<KeyValuePair<string, string?>> ToForm()
    {
        yield return new("FriendlyName", FriendlyName);
        yield return new("UniqueName", UniqueName);
        yield return new("MessagingServiceSid", MessagingServiceSid);
        yield return new("Attributes", Attributes);
        yield return new("State", State);
        yield return new("Timers.Inactive", TimersInactive);
        yield return new("Timers.Closed", TimersClosed);
    }
}

public sealed record UpdateServiceConversationRequest : IFormSerializable
{
    public string? FriendlyName { get; init; }
    public string? UniqueName { get; init; }
    public string? Attributes { get; init; }
    public string? State { get; init; }
    public string? TimersInactive { get; init; }
    public string? TimersClosed { get; init; }

    public IEnumerable<KeyValuePair<string, string?>> ToForm()
    {
        yield return new("FriendlyName", FriendlyName);
        yield return new("UniqueName", UniqueName);
        yield return new("Attributes", Attributes);
        yield return new("State", State);
        yield return new("Timers.Inactive", TimersInactive);
        yield return new("Timers.Closed", TimersClosed);
    }
}

public sealed record CreateServiceConversationMessageRequest : IFormSerializable
{
    public string? Author { get; init; }
    public string? Body { get; init; }
    public string? Attributes { get; init; }
    public string? ContentSid { get; init; }

    public IEnumerable<KeyValuePair<string, string?>> ToForm()
    {
        yield return new("Author", Author);
        yield return new("Body", Body);
        yield return new("Attributes", Attributes);
        yield return new("ContentSid", ContentSid);
    }
}

public sealed record UpdateServiceConversationMessageRequest : IFormSerializable
{
    public string? Author { get; init; }
    public string? Body { get; init; }
    public string? Attributes { get; init; }

    public IEnumerable<KeyValuePair<string, string?>> ToForm()
    {
        yield return new("Author", Author);
        yield return new("Body", Body);
        yield return new("Attributes", Attributes);
    }
}

public sealed record CreateServiceConversationParticipantRequest : IFormSerializable
{
    public string? Identity { get; init; }
    public string? Attributes { get; init; }
    public string? RoleSid { get; init; }
    public string? MessagingBindingAddress { get; init; }
    public string? MessagingBindingProxyAddress { get; init; }
    public string? MessagingBindingProjectedAddress { get; init; }

    public IEnumerable<KeyValuePair<string, string?>> ToForm()
    {
        yield return new("Identity", Identity);
        yield return new("Attributes", Attributes);
        yield return new("RoleSid", RoleSid);
        yield return new("MessagingBinding.Address", MessagingBindingAddress);
        yield return new("MessagingBinding.ProxyAddress", MessagingBindingProxyAddress);
        yield return new("MessagingBinding.ProjectedAddress", MessagingBindingProjectedAddress);
    }
}

public sealed record UpdateServiceConversationParticipantRequest : IFormSerializable
{
    public string? Attributes { get; init; }
    public string? RoleSid { get; init; }

    public IEnumerable<KeyValuePair<string, string?>> ToForm()
    {
        yield return new("Attributes", Attributes);
        yield return new("RoleSid", RoleSid);
    }
}

public sealed record CreateServiceConversationScopedWebhookRequest : IFormSerializable
{
    public required string Target { get; init; }
    public string? ConfigurationUrl { get; init; }
    public string? ConfigurationMethod { get; init; }
    public string? ConfigurationFlowSid { get; init; }

    public IEnumerable<KeyValuePair<string, string?>> ToForm()
    {
        yield return new("Target", Target);
        yield return new("Configuration.Url", ConfigurationUrl);
        yield return new("Configuration.Method", ConfigurationMethod);
        yield return new("Configuration.FlowSid", ConfigurationFlowSid);
    }
}

public sealed record UpdateServiceConversationScopedWebhookRequest : IFormSerializable
{
    public string? ConfigurationUrl { get; init; }
    public string? ConfigurationMethod { get; init; }
    public string? ConfigurationFlowSid { get; init; }

    public IEnumerable<KeyValuePair<string, string?>> ToForm()
    {
        yield return new("Configuration.Url", ConfigurationUrl);
        yield return new("Configuration.Method", ConfigurationMethod);
        yield return new("Configuration.FlowSid", ConfigurationFlowSid);
    }
}

public sealed record CreateServiceRoleRequest : IFormSerializable
{
    public required string FriendlyName { get; init; }
    public required string Type { get; init; }
    /// <summary>Repeated form param — each value emits a separate <c>Permission=</c> entry.</summary>
    public required IReadOnlyList<string> Permission { get; init; }

    public IEnumerable<KeyValuePair<string, string?>> ToForm()
    {
        yield return new("FriendlyName", FriendlyName);
        yield return new("Type", Type);
        foreach (var p in Permission)
        {
            yield return new("Permission", p);
        }
    }
}

public sealed record UpdateServiceRoleRequest : IFormSerializable
{
    public required IReadOnlyList<string> Permission { get; init; }

    public IEnumerable<KeyValuePair<string, string?>> ToForm()
    {
        foreach (var p in Permission)
        {
            yield return new("Permission", p);
        }
    }
}

public sealed record CreateServiceUserRequest : IFormSerializable
{
    public required string Identity { get; init; }
    public string? FriendlyName { get; init; }
    public string? Attributes { get; init; }
    public string? RoleSid { get; init; }

    public IEnumerable<KeyValuePair<string, string?>> ToForm()
    {
        yield return new("Identity", Identity);
        yield return new("FriendlyName", FriendlyName);
        yield return new("Attributes", Attributes);
        yield return new("RoleSid", RoleSid);
    }
}

public sealed record UpdateServiceUserRequest : IFormSerializable
{
    public string? FriendlyName { get; init; }
    public string? Attributes { get; init; }
    public string? RoleSid { get; init; }

    public IEnumerable<KeyValuePair<string, string?>> ToForm()
    {
        yield return new("FriendlyName", FriendlyName);
        yield return new("Attributes", Attributes);
        yield return new("RoleSid", RoleSid);
    }
}

public sealed record CreateServiceConversationWithParticipantsRequest : IFormSerializable
{
    public string? FriendlyName { get; init; }
    public string? UniqueName { get; init; }
    public string? MessagingServiceSid { get; init; }
    public string? Attributes { get; init; }
    public string? State { get; init; }
    public string? TimersInactive { get; init; }
    public string? TimersClosed { get; init; }
    /// <summary>Repeated. Each entry is a JSON object describing one participant.</summary>
    public IReadOnlyList<string>? Participant { get; init; }

    public IEnumerable<KeyValuePair<string, string?>> ToForm()
    {
        yield return new("FriendlyName", FriendlyName);
        yield return new("UniqueName", UniqueName);
        yield return new("MessagingServiceSid", MessagingServiceSid);
        yield return new("Attributes", Attributes);
        yield return new("State", State);
        yield return new("Timers.Inactive", TimersInactive);
        yield return new("Timers.Closed", TimersClosed);
        if (Participant is not null)
        {
            foreach (var p in Participant)
            {
                yield return new("Participant", p);
            }
        }
    }
}

/// <summary>Query params for <c>GET /v1/Services/{IS…}/ParticipantConversations</c>.</summary>
public sealed record ListServiceParticipantConversationsParams
{
    public string? Identity { get; init; }
    public string? Address { get; init; }
    public int? PageSize { get; init; }

    public IEnumerable<KeyValuePair<string, string?>> ToQuery()
    {
        yield return new("Identity", Identity);
        yield return new("Address", Address);
        yield return new("PageSize", PageSize?.ToString());
    }
}

/// <summary>Query params for <c>GET /v1/Services/{IS…}/Bindings</c>.</summary>
public sealed record ListServiceBindingsParams
{
    public string? BindingType { get; init; }
    public string? Identity { get; init; }
    public int? PageSize { get; init; }

    public IEnumerable<KeyValuePair<string, string?>> ToQuery()
    {
        yield return new("BindingType", BindingType);
        yield return new("Identity", Identity);
        yield return new("PageSize", PageSize?.ToString());
    }
}

public sealed record UpdateServiceConfigurationRequest : IFormSerializable
{
    public string? DefaultChatServiceRoleSid { get; init; }
    public string? DefaultConversationCreatorRoleSid { get; init; }
    public string? DefaultConversationRoleSid { get; init; }
    public bool? ReachabilityEnabled { get; init; }

    public IEnumerable<KeyValuePair<string, string?>> ToForm()
    {
        yield return new("DefaultChatServiceRoleSid", DefaultChatServiceRoleSid);
        yield return new("DefaultConversationCreatorRoleSid", DefaultConversationCreatorRoleSid);
        yield return new("DefaultConversationRoleSid", DefaultConversationRoleSid);
        yield return new("ReachabilityEnabled", FormHelpers.BoolStr(ReachabilityEnabled));
    }
}

public sealed record UpdateServiceNotificationRequest : IFormSerializable
{
    public bool? LogEnabled { get; init; }
    public bool? NewMessageEnabled { get; init; }
    public string? NewMessageTemplate { get; init; }
    public string? NewMessageSound { get; init; }
    public bool? NewMessageBadgeCountEnabled { get; init; }
    public bool? NewMessageWithMediaEnabled { get; init; }
    public string? NewMessageWithMediaTemplate { get; init; }
    public bool? AddedToConversationEnabled { get; init; }
    public string? AddedToConversationTemplate { get; init; }
    public string? AddedToConversationSound { get; init; }
    public bool? RemovedFromConversationEnabled { get; init; }
    public string? RemovedFromConversationTemplate { get; init; }
    public string? RemovedFromConversationSound { get; init; }

    public IEnumerable<KeyValuePair<string, string?>> ToForm()
    {
        yield return new("LogEnabled", FormHelpers.BoolStr(LogEnabled));
        yield return new("NewMessage.Enabled", FormHelpers.BoolStr(NewMessageEnabled));
        yield return new("NewMessage.Template", NewMessageTemplate);
        yield return new("NewMessage.Sound", NewMessageSound);
        yield return new("NewMessage.BadgeCountEnabled", FormHelpers.BoolStr(NewMessageBadgeCountEnabled));
        yield return new("NewMessage.WithMedia.Enabled", FormHelpers.BoolStr(NewMessageWithMediaEnabled));
        yield return new("NewMessage.WithMedia.Template", NewMessageWithMediaTemplate);
        yield return new("AddedToConversation.Enabled", FormHelpers.BoolStr(AddedToConversationEnabled));
        yield return new("AddedToConversation.Template", AddedToConversationTemplate);
        yield return new("AddedToConversation.Sound", AddedToConversationSound);
        yield return new("RemovedFromConversation.Enabled", FormHelpers.BoolStr(RemovedFromConversationEnabled));
        yield return new("RemovedFromConversation.Template", RemovedFromConversationTemplate);
        yield return new("RemovedFromConversation.Sound", RemovedFromConversationSound);
    }
}

public sealed record UpdateServiceWebhookConfigurationRequest : IFormSerializable
{
    public string? PreWebhookUrl { get; init; }
    public string? PostWebhookUrl { get; init; }
    public string? Method { get; init; }
    public IReadOnlyList<string>? Filters { get; init; }

    public IEnumerable<KeyValuePair<string, string?>> ToForm()
    {
        yield return new("PreWebhookUrl", PreWebhookUrl);
        yield return new("PostWebhookUrl", PostWebhookUrl);
        yield return new("Method", Method);
        if (Filters is not null)
        {
            foreach (var f in Filters)
            {
                yield return new("Filters", f);
            }
        }
    }
}
