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
    /// <summary>SID of the account that owns this resource.</summary>
    [JsonPropertyName("account_sid")] public string? AccountSid { get; init; }
    /// <summary>SID of the parent Conversation Service.</summary>
    [JsonPropertyName("chat_service_sid")] public string? ChatServiceSid { get; init; }
    /// <summary>SID of the Messaging Service associated with this resource.</summary>
    [JsonPropertyName("messaging_service_sid")] public string? MessagingServiceSid { get; init; }
    /// <summary>Unique SID identifying this resource.</summary>
    [JsonPropertyName("sid")] public string? Sid { get; init; }
    /// <summary>Human-readable name used to identify this resource.</summary>
    [JsonPropertyName("friendly_name")] public string? FriendlyName { get; init; }
    /// <summary>Application-defined unique name that can be used instead of the SID.</summary>
    [JsonPropertyName("unique_name")] public string? UniqueName { get; init; }
    /// <summary>Application-defined attributes encoded as a compact JSON string.</summary>
    [JsonPropertyName("attributes")] public string? Attributes { get; init; }
    /// <summary>Lifecycle state of this resource; future values are preserved as strings.</summary>
    [JsonPropertyName("state")] public string? State { get; init; }
    /// <summary>Timestamp when this resource was created.</summary>
    [JsonPropertyName("date_created")] public string? DateCreated { get; init; }
    /// <summary>Timestamp of the most recent modification to this resource.</summary>
    [JsonPropertyName("date_updated")] public string? DateUpdated { get; init; }
    /// <summary>ISO 8601 inactivity and closure durations configured for this Conversation.</summary>
    [JsonPropertyName("timers")] public JsonElement? Timers { get; init; }
    /// <summary>Absolute URL of this resource.</summary>
    [JsonPropertyName("url")] public string? Url { get; init; }
    /// <summary>URLs of related resources and child collections.</summary>
    [JsonPropertyName("links")] public JsonElement? Links { get; init; }
    /// <summary>Channel bindings attached to this resource, preserving their JSON structure.</summary>
    [JsonPropertyName("bindings")] public JsonElement? Bindings { get; init; }
}

/// <summary>Page of service conversation resources with continuation metadata.</summary>
public sealed record ConversationsV1ServiceConversationList : IV1PagedList
{
    /// <summary>The returned conversations in this page.</summary>
    [JsonPropertyName("conversations")] public List<ConversationsV1ServiceConversation> Conversations { get; init; } = new();
    /// <summary>Pagination metadata; follow next_page_url to fetch the next page.</summary>
    [JsonPropertyName("meta")] public VoiceV1Meta? Meta { get; init; }
}

/// <summary>Service-scoped Conversation Message — <c>IM…</c>.</summary>
public sealed record ConversationsV1ServiceConversationMessage
{
    /// <summary>SID of the account that owns this resource.</summary>
    [JsonPropertyName("account_sid")] public string? AccountSid { get; init; }
    /// <summary>SID of the parent Conversation Service.</summary>
    [JsonPropertyName("chat_service_sid")] public string? ChatServiceSid { get; init; }
    /// <summary>SID of the parent Conversation.</summary>
    [JsonPropertyName("conversation_sid")] public string? ConversationSid { get; init; }
    /// <summary>Unique SID identifying this resource.</summary>
    [JsonPropertyName("sid")] public string? Sid { get; init; }
    /// <summary>Monotonically increasing message index within its Conversation.</summary>
    [JsonPropertyName("index")] public int Index { get; init; }
    /// <summary>Identity of the user or external participant who authored the message.</summary>
    [JsonPropertyName("author")] public string? Author { get; init; }
    /// <summary>Text content of the message.</summary>
    [JsonPropertyName("body")] public string? Body { get; init; }
    /// <summary>Media attachments associated with the Conversation message, preserving provider metadata.</summary>
    [JsonPropertyName("media")] public JsonElement? Media { get; init; }
    /// <summary>Application-defined attributes encoded as a compact JSON string.</summary>
    [JsonPropertyName("attributes")] public string? Attributes { get; init; }
    /// <summary>SID of the related Participant.</summary>
    [JsonPropertyName("participant_sid")] public string? ParticipantSid { get; init; }
    /// <summary>Timestamp when this resource was created.</summary>
    [JsonPropertyName("date_created")] public string? DateCreated { get; init; }
    /// <summary>Timestamp of the most recent modification to this resource.</summary>
    [JsonPropertyName("date_updated")] public string? DateUpdated { get; init; }
    /// <summary>Absolute URL of this resource.</summary>
    [JsonPropertyName("url")] public string? Url { get; init; }
    /// <summary>Aggregated delivery receipt information for the message, preserving the JSON detail.</summary>
    [JsonPropertyName("delivery")] public JsonElement? Delivery { get; init; }
    /// <summary>URLs of related resources and child collections.</summary>
    [JsonPropertyName("links")] public JsonElement? Links { get; init; }
    /// <summary>SID of the related Content.</summary>
    [JsonPropertyName("content_sid")] public string? ContentSid { get; init; }
}

/// <summary>Page of service conversation message resources with continuation metadata.</summary>
public sealed record ConversationsV1ServiceConversationMessageList : IV1PagedList
{
    /// <summary>The returned messages in this page.</summary>
    [JsonPropertyName("messages")] public List<ConversationsV1ServiceConversationMessage> Messages { get; init; } = new();
    /// <summary>Pagination metadata; follow next_page_url to fetch the next page.</summary>
    [JsonPropertyName("meta")] public VoiceV1Meta? Meta { get; init; }
}

/// <summary>Service-scoped Conversation Participant — <c>MB…</c>.</summary>
public sealed record ConversationsV1ServiceConversationParticipant
{
    /// <summary>SID of the account that owns this resource.</summary>
    [JsonPropertyName("account_sid")] public string? AccountSid { get; init; }
    /// <summary>SID of the parent Conversation Service.</summary>
    [JsonPropertyName("chat_service_sid")] public string? ChatServiceSid { get; init; }
    /// <summary>SID of the parent Conversation.</summary>
    [JsonPropertyName("conversation_sid")] public string? ConversationSid { get; init; }
    /// <summary>Unique SID identifying this resource.</summary>
    [JsonPropertyName("sid")] public string? Sid { get; init; }
    /// <summary>Application-defined user identity.</summary>
    [JsonPropertyName("identity")] public string? Identity { get; init; }
    /// <summary>Application-defined attributes encoded as a compact JSON string.</summary>
    [JsonPropertyName("attributes")] public string? Attributes { get; init; }
    /// <summary>Address and proxy-address settings used to connect the participant to a messaging channel.</summary>
    [JsonPropertyName("messaging_binding")] public JsonElement? MessagingBinding { get; init; }
    /// <summary>SID of the role controlling the participant or user permissions.</summary>
    [JsonPropertyName("role_sid")] public string? RoleSid { get; init; }
    /// <summary>Timestamp when this resource was created.</summary>
    [JsonPropertyName("date_created")] public string? DateCreated { get; init; }
    /// <summary>Timestamp of the most recent modification to this resource.</summary>
    [JsonPropertyName("date_updated")] public string? DateUpdated { get; init; }
    /// <summary>Absolute URL of this resource.</summary>
    [JsonPropertyName("url")] public string? Url { get; init; }
    /// <summary>Index of the most recent message marked as read by this participant.</summary>
    [JsonPropertyName("last_read_message_index")] public int? LastReadMessageIndex { get; init; }
    /// <summary>Timestamp when this participant last updated its read position.</summary>
    [JsonPropertyName("last_read_timestamp")] public string? LastReadTimestamp { get; init; }
}

/// <summary>Page of service conversation participant resources with continuation metadata.</summary>
public sealed record ConversationsV1ServiceConversationParticipantList : IV1PagedList
{
    /// <summary>The returned participants in this page.</summary>
    [JsonPropertyName("participants")] public List<ConversationsV1ServiceConversationParticipant> Participants { get; init; } = new();
    /// <summary>Pagination metadata; follow next_page_url to fetch the next page.</summary>
    [JsonPropertyName("meta")] public VoiceV1Meta? Meta { get; init; }
}

/// <summary>Service-scoped Conversation Message Receipt — <c>DY…</c>.</summary>
public sealed record ConversationsV1ServiceConversationMessageReceipt
{
    /// <summary>SID of the account that owns this resource.</summary>
    [JsonPropertyName("account_sid")] public string? AccountSid { get; init; }
    /// <summary>SID of the parent Conversation Service.</summary>
    [JsonPropertyName("chat_service_sid")] public string? ChatServiceSid { get; init; }
    /// <summary>SID of the parent Conversation.</summary>
    [JsonPropertyName("conversation_sid")] public string? ConversationSid { get; init; }
    /// <summary>Unique SID identifying this resource.</summary>
    [JsonPropertyName("sid")] public string? Sid { get; init; }
    /// <summary>SID of the related Message.</summary>
    [JsonPropertyName("message_sid")] public string? MessageSid { get; init; }
    /// <summary>SID of the related Channel Message.</summary>
    [JsonPropertyName("channel_message_sid")] public string? ChannelMessageSid { get; init; }
    /// <summary>SID of the related Participant.</summary>
    [JsonPropertyName("participant_sid")] public string? ParticipantSid { get; init; }
    /// <summary>Current resource state; unrecognized future values are preserved as strings.</summary>
    [JsonPropertyName("status")] public string? Status { get; init; }
    /// <summary>Numeric delivery error code; zero indicates no reported error.</summary>
    [JsonPropertyName("error_code")] public int ErrorCode { get; init; }
    /// <summary>Timestamp when this resource was created.</summary>
    [JsonPropertyName("date_created")] public string? DateCreated { get; init; }
    /// <summary>Timestamp of the most recent modification to this resource.</summary>
    [JsonPropertyName("date_updated")] public string? DateUpdated { get; init; }
    /// <summary>Absolute URL of this resource.</summary>
    [JsonPropertyName("url")] public string? Url { get; init; }
}

/// <summary>Page of service conversation message receipt resources with continuation metadata.</summary>
public sealed record ConversationsV1ServiceConversationMessageReceiptList : IV1PagedList
{
    /// <summary>The returned delivery receipts in this page.</summary>
    [JsonPropertyName("delivery_receipts")] public List<ConversationsV1ServiceConversationMessageReceipt> DeliveryReceipts { get; init; } = new();
    /// <summary>Pagination metadata; follow next_page_url to fetch the next page.</summary>
    [JsonPropertyName("meta")] public VoiceV1Meta? Meta { get; init; }
}

/// <summary>Service-scoped Conversation Scoped Webhook — <c>WH…</c>.</summary>
public sealed record ConversationsV1ServiceConversationScopedWebhook
{
    /// <summary>Unique SID identifying this resource.</summary>
    [JsonPropertyName("sid")] public string? Sid { get; init; }
    /// <summary>SID of the account that owns this resource.</summary>
    [JsonPropertyName("account_sid")] public string? AccountSid { get; init; }
    /// <summary>SID of the parent Conversation Service.</summary>
    [JsonPropertyName("chat_service_sid")] public string? ChatServiceSid { get; init; }
    /// <summary>SID of the parent Conversation.</summary>
    [JsonPropertyName("conversation_sid")] public string? ConversationSid { get; init; }
    /// <summary>The SIP URI target.</summary>
    [JsonPropertyName("target")] public string? Target { get; init; }
    /// <summary>Absolute URL of this resource.</summary>
    [JsonPropertyName("url")] public string? Url { get; init; }
    /// <summary>Channel or webhook configuration, preserving the service-specific JSON structure.</summary>
    [JsonPropertyName("configuration")] public JsonElement? Configuration { get; init; }
    /// <summary>Timestamp when this resource was created.</summary>
    [JsonPropertyName("date_created")] public string? DateCreated { get; init; }
    /// <summary>Timestamp of the most recent modification to this resource.</summary>
    [JsonPropertyName("date_updated")] public string? DateUpdated { get; init; }
}

/// <summary>Page of service conversation scoped webhook resources with continuation metadata.</summary>
public sealed record ConversationsV1ServiceConversationScopedWebhookList : IV1PagedList
{
    /// <summary>The returned webhooks in this page.</summary>
    [JsonPropertyName("webhooks")] public List<ConversationsV1ServiceConversationScopedWebhook> Webhooks { get; init; } = new();
    /// <summary>Pagination metadata; follow next_page_url to fetch the next page.</summary>
    [JsonPropertyName("meta")] public VoiceV1Meta? Meta { get; init; }
}

/// <summary>Service-scoped Role — <c>RL…</c>.</summary>
public sealed record ConversationsV1ServiceRole
{
    /// <summary>Unique SID identifying this resource.</summary>
    [JsonPropertyName("sid")] public string? Sid { get; init; }
    /// <summary>SID of the account that owns this resource.</summary>
    [JsonPropertyName("account_sid")] public string? AccountSid { get; init; }
    /// <summary>SID of the parent Conversation Service.</summary>
    [JsonPropertyName("chat_service_sid")] public string? ChatServiceSid { get; init; }
    /// <summary>Human-readable name used to identify this resource.</summary>
    [JsonPropertyName("friendly_name")] public string? FriendlyName { get; init; }
    /// <summary>Type: conversation, service.</summary>
    [JsonPropertyName("type")] public string? Type { get; init; }
    /// <summary>Permission names granted by this role.</summary>
    [JsonPropertyName("permissions")] public List<string>? Permissions { get; init; }
    /// <summary>Timestamp when this resource was created.</summary>
    [JsonPropertyName("date_created")] public string? DateCreated { get; init; }
    /// <summary>Timestamp of the most recent modification to this resource.</summary>
    [JsonPropertyName("date_updated")] public string? DateUpdated { get; init; }
    /// <summary>Absolute URL of this resource.</summary>
    [JsonPropertyName("url")] public string? Url { get; init; }
}

/// <summary>Page of service role resources with continuation metadata.</summary>
public sealed record ConversationsV1ServiceRoleList : IV1PagedList
{
    /// <summary>The returned roles in this page.</summary>
    [JsonPropertyName("roles")] public List<ConversationsV1ServiceRole> Roles { get; init; } = new();
    /// <summary>Pagination metadata; follow next_page_url to fetch the next page.</summary>
    [JsonPropertyName("meta")] public VoiceV1Meta? Meta { get; init; }
}

/// <summary>Service-scoped User — <c>US…</c>.</summary>
public sealed record ConversationsV1ServiceUser
{
    /// <summary>Unique SID identifying this resource.</summary>
    [JsonPropertyName("sid")] public string? Sid { get; init; }
    /// <summary>SID of the account that owns this resource.</summary>
    [JsonPropertyName("account_sid")] public string? AccountSid { get; init; }
    /// <summary>SID of the parent Conversation Service.</summary>
    [JsonPropertyName("chat_service_sid")] public string? ChatServiceSid { get; init; }
    /// <summary>SID of the role controlling the participant or user permissions.</summary>
    [JsonPropertyName("role_sid")] public string? RoleSid { get; init; }
    /// <summary>Application-defined user identity.</summary>
    [JsonPropertyName("identity")] public string? Identity { get; init; }
    /// <summary>Human-readable name used to identify this resource.</summary>
    [JsonPropertyName("friendly_name")] public string? FriendlyName { get; init; }
    /// <summary>Application-defined attributes encoded as a compact JSON string.</summary>
    [JsonPropertyName("attributes")] public string? Attributes { get; init; }
    /// <summary>Whether is online is enabled for this resource.</summary>
    [JsonPropertyName("is_online")] public bool? IsOnline { get; init; }
    /// <summary>Whether is notifiable is enabled for this resource.</summary>
    [JsonPropertyName("is_notifiable")] public bool? IsNotifiable { get; init; }
    /// <summary>Timestamp when this resource was created.</summary>
    [JsonPropertyName("date_created")] public string? DateCreated { get; init; }
    /// <summary>Timestamp of the most recent modification to this resource.</summary>
    [JsonPropertyName("date_updated")] public string? DateUpdated { get; init; }
    /// <summary>Absolute URL of this resource.</summary>
    [JsonPropertyName("url")] public string? Url { get; init; }
    /// <summary>URLs of related resources and child collections.</summary>
    [JsonPropertyName("links")] public JsonElement? Links { get; init; }
}

/// <summary>Page of service user resources with continuation metadata.</summary>
public sealed record ConversationsV1ServiceUserList : IV1PagedList
{
    /// <summary>The returned users in this page.</summary>
    [JsonPropertyName("users")] public List<ConversationsV1ServiceUser> Users { get; init; } = new();
    /// <summary>Pagination metadata; follow next_page_url to fetch the next page.</summary>
    [JsonPropertyName("meta")] public VoiceV1Meta? Meta { get; init; }
}

/// <summary>Service-scoped Conversation created via <c>/v1/Services/{IS…}/ConversationWithParticipants</c>.</summary>
public sealed record ConversationsV1ServiceConversationWithParticipants
{
    /// <summary>SID of the account that owns this resource.</summary>
    [JsonPropertyName("account_sid")] public string? AccountSid { get; init; }
    /// <summary>SID of the parent Conversation Service.</summary>
    [JsonPropertyName("chat_service_sid")] public string? ChatServiceSid { get; init; }
    /// <summary>SID of the Messaging Service associated with this resource.</summary>
    [JsonPropertyName("messaging_service_sid")] public string? MessagingServiceSid { get; init; }
    /// <summary>Unique SID identifying this resource.</summary>
    [JsonPropertyName("sid")] public string? Sid { get; init; }
    /// <summary>Human-readable name used to identify this resource.</summary>
    [JsonPropertyName("friendly_name")] public string? FriendlyName { get; init; }
    /// <summary>Application-defined unique name that can be used instead of the SID.</summary>
    [JsonPropertyName("unique_name")] public string? UniqueName { get; init; }
    /// <summary>Application-defined attributes encoded as a compact JSON string.</summary>
    [JsonPropertyName("attributes")] public string? Attributes { get; init; }
    /// <summary>Lifecycle state of this resource; future values are preserved as strings.</summary>
    [JsonPropertyName("state")] public string? State { get; init; }
    /// <summary>Timestamp when this resource was created.</summary>
    [JsonPropertyName("date_created")] public string? DateCreated { get; init; }
    /// <summary>Timestamp of the most recent modification to this resource.</summary>
    [JsonPropertyName("date_updated")] public string? DateUpdated { get; init; }
    /// <summary>ISO 8601 inactivity and closure durations configured for this Conversation.</summary>
    [JsonPropertyName("timers")] public JsonElement? Timers { get; init; }
    /// <summary>URLs of related resources and child collections.</summary>
    [JsonPropertyName("links")] public JsonElement? Links { get; init; }
    /// <summary>Channel bindings attached to this resource, preserving their JSON structure.</summary>
    [JsonPropertyName("bindings")] public JsonElement? Bindings { get; init; }
    /// <summary>Absolute URL of this resource.</summary>
    [JsonPropertyName("url")] public string? Url { get; init; }
}

/// <summary>Service-scoped flattened participant + conversation view (read-only — list only).</summary>
public sealed record ConversationsV1ServiceParticipantConversation
{
    /// <summary>SID of the account that owns this resource.</summary>
    [JsonPropertyName("account_sid")] public string? AccountSid { get; init; }
    /// <summary>SID of the parent Conversation Service.</summary>
    [JsonPropertyName("chat_service_sid")] public string? ChatServiceSid { get; init; }
    /// <summary>SID of the related Participant.</summary>
    [JsonPropertyName("participant_sid")] public string? ParticipantSid { get; init; }
    /// <summary>SID of the related Participant User.</summary>
    [JsonPropertyName("participant_user_sid")] public string? ParticipantUserSid { get; init; }
    /// <summary>Application-defined identity used to locate the Conversation participant.</summary>
    [JsonPropertyName("participant_identity")] public string? ParticipantIdentity { get; init; }
    /// <summary>External messaging address used to locate the participant across Conversations.</summary>
    [JsonPropertyName("participant_messaging_binding")] public JsonElement? ParticipantMessagingBinding { get; init; }
    /// <summary>SID of the parent Conversation.</summary>
    [JsonPropertyName("conversation_sid")] public string? ConversationSid { get; init; }
    /// <summary>Application-defined unique name of the parent Conversation.</summary>
    [JsonPropertyName("conversation_unique_name")] public string? ConversationUniqueName { get; init; }
    /// <summary>Human-readable name of the parent Conversation.</summary>
    [JsonPropertyName("conversation_friendly_name")] public string? ConversationFriendlyName { get; init; }
    /// <summary>Application attributes on the parent Conversation, encoded as a JSON string.</summary>
    [JsonPropertyName("conversation_attributes")] public string? ConversationAttributes { get; init; }
    /// <summary>Creation timestamp of the parent Conversation.</summary>
    [JsonPropertyName("conversation_date_created")] public string? ConversationDateCreated { get; init; }
    /// <summary>Most recent modification timestamp of the parent Conversation.</summary>
    [JsonPropertyName("conversation_date_updated")] public string? ConversationDateUpdated { get; init; }
    /// <summary>Identity that created the parent Conversation.</summary>
    [JsonPropertyName("conversation_created_by")] public string? ConversationCreatedBy { get; init; }
    /// <summary>Conversation State: inactive, active, closed.</summary>
    [JsonPropertyName("conversation_state")] public string? ConversationState { get; init; }
    /// <summary>Inactivity and closure timers configured on the parent Conversation.</summary>
    [JsonPropertyName("conversation_timers")] public JsonElement? ConversationTimers { get; init; }
    /// <summary>URLs of related resources and child collections.</summary>
    [JsonPropertyName("links")] public JsonElement? Links { get; init; }
}

/// <summary>Page of service participant conversation resources with continuation metadata.</summary>
public sealed record ConversationsV1ServiceParticipantConversationList : IV1PagedList
{
    /// <summary>The returned conversations in this page.</summary>
    [JsonPropertyName("conversations")] public List<ConversationsV1ServiceParticipantConversation> Conversations { get; init; } = new();
    /// <summary>Pagination metadata; follow next_page_url to fetch the next page.</summary>
    [JsonPropertyName("meta")] public VoiceV1Meta? Meta { get; init; }
}

/// <summary>Service-scoped per-user view of conversations they belong to (list only).</summary>
public sealed record ConversationsV1ServiceUserConversation
{
    /// <summary>SID of the account that owns this resource.</summary>
    [JsonPropertyName("account_sid")] public string? AccountSid { get; init; }
    /// <summary>SID of the parent Conversation Service.</summary>
    [JsonPropertyName("chat_service_sid")] public string? ChatServiceSid { get; init; }
    /// <summary>SID of the parent Conversation.</summary>
    [JsonPropertyName("conversation_sid")] public string? ConversationSid { get; init; }
    /// <summary>Number of unread messages associated with this resource.</summary>
    [JsonPropertyName("unread_messages_count")] public int? UnreadMessagesCount { get; init; }
    /// <summary>Index of the most recent message marked as read by this participant.</summary>
    [JsonPropertyName("last_read_message_index")] public int? LastReadMessageIndex { get; init; }
    /// <summary>SID of the related Participant.</summary>
    [JsonPropertyName("participant_sid")] public string? ParticipantSid { get; init; }
    /// <summary>SID of the related User.</summary>
    [JsonPropertyName("user_sid")] public string? UserSid { get; init; }
    /// <summary>Human-readable name used to identify this resource.</summary>
    [JsonPropertyName("friendly_name")] public string? FriendlyName { get; init; }
    /// <summary>Conversation State: inactive, active, closed.</summary>
    [JsonPropertyName("conversation_state")] public string? ConversationState { get; init; }
    /// <summary>ISO 8601 inactivity and closure durations configured for this Conversation.</summary>
    [JsonPropertyName("timers")] public JsonElement? Timers { get; init; }
    /// <summary>Application-defined attributes encoded as a compact JSON string.</summary>
    [JsonPropertyName("attributes")] public string? Attributes { get; init; }
    /// <summary>Timestamp when this resource was created.</summary>
    [JsonPropertyName("date_created")] public string? DateCreated { get; init; }
    /// <summary>Timestamp of the most recent modification to this resource.</summary>
    [JsonPropertyName("date_updated")] public string? DateUpdated { get; init; }
    /// <summary>Identity of the user or application that created the resource.</summary>
    [JsonPropertyName("created_by")] public string? CreatedBy { get; init; }
    /// <summary>Notification Level: default, muted.</summary>
    [JsonPropertyName("notification_level")] public string? NotificationLevel { get; init; }
    /// <summary>Application-defined unique name that can be used instead of the SID.</summary>
    [JsonPropertyName("unique_name")] public string? UniqueName { get; init; }
    /// <summary>Absolute URL of this resource.</summary>
    [JsonPropertyName("url")] public string? Url { get; init; }
    /// <summary>URLs of related resources and child collections.</summary>
    [JsonPropertyName("links")] public JsonElement? Links { get; init; }
}

/// <summary>Page of service user conversation resources with continuation metadata.</summary>
public sealed record ConversationsV1ServiceUserConversationList : IV1PagedList
{
    /// <summary>The returned conversations in this page.</summary>
    [JsonPropertyName("conversations")] public List<ConversationsV1ServiceUserConversation> Conversations { get; init; } = new();
    /// <summary>Pagination metadata; follow next_page_url to fetch the next page.</summary>
    [JsonPropertyName("meta")] public VoiceV1Meta? Meta { get; init; }
}

/// <summary>Service-scoped push Binding — <c>BS…</c> (read+delete only).</summary>
public sealed record ConversationsV1ServiceBinding
{
    /// <summary>Unique SID identifying this resource.</summary>
    [JsonPropertyName("sid")] public string? Sid { get; init; }
    /// <summary>SID of the account that owns this resource.</summary>
    [JsonPropertyName("account_sid")] public string? AccountSid { get; init; }
    /// <summary>SID of the parent Conversation Service.</summary>
    [JsonPropertyName("chat_service_sid")] public string? ChatServiceSid { get; init; }
    /// <summary>SID of the related Credential.</summary>
    [JsonPropertyName("credential_sid")] public string? CredentialSid { get; init; }
    /// <summary>Timestamp when this resource was created.</summary>
    [JsonPropertyName("date_created")] public string? DateCreated { get; init; }
    /// <summary>Timestamp of the most recent modification to this resource.</summary>
    [JsonPropertyName("date_updated")] public string? DateUpdated { get; init; }
    /// <summary>Destination endpoint registered for this channel binding.</summary>
    [JsonPropertyName("endpoint")] public string? Endpoint { get; init; }
    /// <summary>Application-defined user identity.</summary>
    [JsonPropertyName("identity")] public string? Identity { get; init; }
    /// <summary>Binding Type: apn, gcm, fcm, twilsock.</summary>
    [JsonPropertyName("binding_type")] public string? BindingType { get; init; }
    /// <summary>The returned message types in this page.</summary>
    [JsonPropertyName("message_types")] public List<string>? MessageTypes { get; init; }
    /// <summary>Absolute URL of this resource.</summary>
    [JsonPropertyName("url")] public string? Url { get; init; }
}

/// <summary>Page of service binding resources with continuation metadata.</summary>
public sealed record ConversationsV1ServiceBindingList : IV1PagedList
{
    /// <summary>The returned bindings in this page.</summary>
    [JsonPropertyName("bindings")] public List<ConversationsV1ServiceBinding> Bindings { get; init; } = new();
    /// <summary>Pagination metadata; follow next_page_url to fetch the next page.</summary>
    [JsonPropertyName("meta")] public VoiceV1Meta? Meta { get; init; }
}

/// <summary>Per-service Configuration singleton (no sid — keyed by chat service).</summary>
public sealed record ConversationsV1ServiceConfiguration
{
    /// <summary>SID of the parent Conversation Service.</summary>
    [JsonPropertyName("chat_service_sid")] public string? ChatServiceSid { get; init; }
    /// <summary>SID of the related Default Conversation Creator Role.</summary>
    [JsonPropertyName("default_conversation_creator_role_sid")] public string? DefaultConversationCreatorRoleSid { get; init; }
    /// <summary>SID of the related Default Conversation Role.</summary>
    [JsonPropertyName("default_conversation_role_sid")] public string? DefaultConversationRoleSid { get; init; }
    /// <summary>SID of the related Default Chat Service Role.</summary>
    [JsonPropertyName("default_chat_service_role_sid")] public string? DefaultChatServiceRoleSid { get; init; }
    /// <summary>Absolute URL of this resource.</summary>
    [JsonPropertyName("url")] public string? Url { get; init; }
    /// <summary>URLs of related resources and child collections.</summary>
    [JsonPropertyName("links")] public JsonElement? Links { get; init; }
    /// <summary>Whether reachability enabled is enabled for this resource.</summary>
    [JsonPropertyName("reachability_enabled")] public bool? ReachabilityEnabled { get; init; }
}

/// <summary>Per-service push Notification configuration singleton.</summary>
public sealed record ConversationsV1ServiceNotification
{
    /// <summary>SID of the account that owns this resource.</summary>
    [JsonPropertyName("account_sid")] public string? AccountSid { get; init; }
    /// <summary>SID of the parent Conversation Service.</summary>
    [JsonPropertyName("chat_service_sid")] public string? ChatServiceSid { get; init; }
    /// <summary>Whether to send push notifications for new Conversation messages.</summary>
    [JsonPropertyName("new_message")] public JsonElement? NewMessage { get; init; }
    /// <summary>Whether to send a push notification when a participant is added to a Conversation.</summary>
    [JsonPropertyName("added_to_conversation")] public JsonElement? AddedToConversation { get; init; }
    /// <summary>Whether to notify users when they are removed from a Conversation.</summary>
    [JsonPropertyName("removed_from_conversation")] public JsonElement? RemovedFromConversation { get; init; }
    /// <summary>Whether log enabled is enabled for this resource.</summary>
    [JsonPropertyName("log_enabled")] public bool? LogEnabled { get; init; }
    /// <summary>Absolute URL of this resource.</summary>
    [JsonPropertyName("url")] public string? Url { get; init; }
}

/// <summary>Per-service Webhook configuration singleton.</summary>
public sealed record ConversationsV1ServiceWebhookConfiguration
{
    /// <summary>SID of the account that owns this resource.</summary>
    [JsonPropertyName("account_sid")] public string? AccountSid { get; init; }
    /// <summary>SID of the parent Conversation Service.</summary>
    [JsonPropertyName("chat_service_sid")] public string? ChatServiceSid { get; init; }
    /// <summary>URL used for pre webhook.</summary>
    [JsonPropertyName("pre_webhook_url")] public string? PreWebhookUrl { get; init; }
    /// <summary>URL used for post webhook.</summary>
    [JsonPropertyName("post_webhook_url")] public string? PostWebhookUrl { get; init; }
    /// <summary>The returned filters in this page.</summary>
    [JsonPropertyName("filters")] public List<string>? Filters { get; init; }
    /// <summary>HTTP method used to fetch `Url` (Twilio default = POST).</summary>
    [JsonPropertyName("method")] public string? Method { get; init; }
    /// <summary>Absolute URL of this resource.</summary>
    [JsonPropertyName("url")] public string? Url { get; init; }
}

// ---- Request bodies (form-encoded) ----------------------------------------

/// <summary>Parameters for create service conversation request.</summary>
public sealed record CreateServiceConversationRequest : IFormSerializable
{
    /// <summary>Human-readable name used to identify this resource.</summary>
    public string? FriendlyName { get; init; }
    /// <summary>Application-defined unique name that can be used instead of the SID.</summary>
    public string? UniqueName { get; init; }
    /// <summary>SID of the Messaging Service associated with this resource.</summary>
    public string? MessagingServiceSid { get; init; }
    /// <summary>Application-defined attributes encoded as a compact JSON string.</summary>
    public string? Attributes { get; init; }
    /// <summary>Lifecycle state of this resource; future values are preserved as strings.</summary>
    public string? State { get; init; }
    /// <summary>ISO 8601 duration before the active Conversation becomes inactive.</summary>
    public string? TimersInactive { get; init; }
    /// <summary>ISO 8601 duration before the inactive Conversation is automatically closed.</summary>
    public string? TimersClosed { get; init; }

    /// <summary>Render the declared request fields as form pairs; lists use repeated keys and null values are omitted by the transport.</summary>
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

/// <summary>Parameters for update service conversation request.</summary>
public sealed record UpdateServiceConversationRequest : IFormSerializable
{
    /// <summary>Human-readable name used to identify this resource.</summary>
    public string? FriendlyName { get; init; }
    /// <summary>Application-defined unique name that can be used instead of the SID.</summary>
    public string? UniqueName { get; init; }
    /// <summary>Application-defined attributes encoded as a compact JSON string.</summary>
    public string? Attributes { get; init; }
    /// <summary>Lifecycle state of this resource; future values are preserved as strings.</summary>
    public string? State { get; init; }
    /// <summary>ISO 8601 duration before the active Conversation becomes inactive.</summary>
    public string? TimersInactive { get; init; }
    /// <summary>ISO 8601 duration before the inactive Conversation is automatically closed.</summary>
    public string? TimersClosed { get; init; }

    /// <summary>Render the declared request fields as form pairs; lists use repeated keys and null values are omitted by the transport.</summary>
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

/// <summary>Parameters for create service conversation message request.</summary>
public sealed record CreateServiceConversationMessageRequest : IFormSerializable
{
    /// <summary>Identity of the user or external participant who authored the message.</summary>
    public string? Author { get; init; }
    /// <summary>Text content of the message.</summary>
    public string? Body { get; init; }
    /// <summary>Application-defined attributes encoded as a compact JSON string.</summary>
    public string? Attributes { get; init; }
    /// <summary>SID of the related Content.</summary>
    public string? ContentSid { get; init; }

    /// <summary>Render the declared request fields as form pairs; lists use repeated keys and null values are omitted by the transport.</summary>
    public IEnumerable<KeyValuePair<string, string?>> ToForm()
    {
        yield return new("Author", Author);
        yield return new("Body", Body);
        yield return new("Attributes", Attributes);
        yield return new("ContentSid", ContentSid);
    }
}

/// <summary>Parameters for update service conversation message request.</summary>
public sealed record UpdateServiceConversationMessageRequest : IFormSerializable
{
    /// <summary>Identity of the user or external participant who authored the message.</summary>
    public string? Author { get; init; }
    /// <summary>Text content of the message.</summary>
    public string? Body { get; init; }
    /// <summary>Application-defined attributes encoded as a compact JSON string.</summary>
    public string? Attributes { get; init; }

    /// <summary>Render the declared request fields as form pairs; lists use repeated keys and null values are omitted by the transport.</summary>
    public IEnumerable<KeyValuePair<string, string?>> ToForm()
    {
        yield return new("Author", Author);
        yield return new("Body", Body);
        yield return new("Attributes", Attributes);
    }
}

/// <summary>Parameters for create service conversation participant request.</summary>
public sealed record CreateServiceConversationParticipantRequest : IFormSerializable
{
    /// <summary>Application-defined user identity.</summary>
    public string? Identity { get; init; }
    /// <summary>Application-defined attributes encoded as a compact JSON string.</summary>
    public string? Attributes { get; init; }
    /// <summary>SID of the role controlling the participant or user permissions.</summary>
    public string? RoleSid { get; init; }
    /// <summary>External participant address, such as an E.164 SMS number.</summary>
    public string? MessagingBindingAddress { get; init; }
    /// <summary>Service-owned proxy address used to exchange messages with this participant.</summary>
    public string? MessagingBindingProxyAddress { get; init; }
    /// <summary>Projected address used to represent this participant to the other participants.</summary>
    public string? MessagingBindingProjectedAddress { get; init; }

    /// <summary>Render the declared request fields as form pairs; lists use repeated keys and null values are omitted by the transport.</summary>
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

/// <summary>Parameters for update service conversation participant request.</summary>
public sealed record UpdateServiceConversationParticipantRequest : IFormSerializable
{
    /// <summary>Application-defined attributes encoded as a compact JSON string.</summary>
    public string? Attributes { get; init; }
    /// <summary>SID of the role controlling the participant or user permissions.</summary>
    public string? RoleSid { get; init; }

    /// <summary>Render the declared request fields as form pairs; lists use repeated keys and null values are omitted by the transport.</summary>
    public IEnumerable<KeyValuePair<string, string?>> ToForm()
    {
        yield return new("Attributes", Attributes);
        yield return new("RoleSid", RoleSid);
    }
}

/// <summary>Parameters for create service conversation scoped webhook request.</summary>
public sealed record CreateServiceConversationScopedWebhookRequest : IFormSerializable
{
    /// <summary>The SIP URI target.</summary>
    public required string Target { get; init; }
    /// <summary>URL used for configuration.</summary>
    public string? ConfigurationUrl { get; init; }
    /// <summary>HTTP method for configuration requests.</summary>
    public string? ConfigurationMethod { get; init; }
    /// <summary>SID of the related Configuration Flow.</summary>
    public string? ConfigurationFlowSid { get; init; }

    /// <summary>Render the declared request fields as form pairs; lists use repeated keys and null values are omitted by the transport.</summary>
    public IEnumerable<KeyValuePair<string, string?>> ToForm()
    {
        yield return new("Target", Target);
        yield return new("Configuration.Url", ConfigurationUrl);
        yield return new("Configuration.Method", ConfigurationMethod);
        yield return new("Configuration.FlowSid", ConfigurationFlowSid);
    }
}

/// <summary>Parameters for update service conversation scoped webhook request.</summary>
public sealed record UpdateServiceConversationScopedWebhookRequest : IFormSerializable
{
    /// <summary>URL used for configuration.</summary>
    public string? ConfigurationUrl { get; init; }
    /// <summary>HTTP method for configuration requests.</summary>
    public string? ConfigurationMethod { get; init; }
    /// <summary>SID of the related Configuration Flow.</summary>
    public string? ConfigurationFlowSid { get; init; }

    /// <summary>Render the declared request fields as form pairs; lists use repeated keys and null values are omitted by the transport.</summary>
    public IEnumerable<KeyValuePair<string, string?>> ToForm()
    {
        yield return new("Configuration.Url", ConfigurationUrl);
        yield return new("Configuration.Method", ConfigurationMethod);
        yield return new("Configuration.FlowSid", ConfigurationFlowSid);
    }
}

/// <summary>Parameters for create service role request.</summary>
public sealed record CreateServiceRoleRequest : IFormSerializable
{
    /// <summary>Human-readable name used to identify this resource.</summary>
    public required string FriendlyName { get; init; }
    /// <summary>Provider-defined category identifying the kind of resource or channel.</summary>
    public required string Type { get; init; }
    /// <summary>Repeated form param — each value emits a separate <c>Permission=</c> entry.</summary>
    public required IReadOnlyList<string> Permission { get; init; }

    /// <summary>Render the declared request fields as form pairs; lists use repeated keys and null values are omitted by the transport.</summary>
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

/// <summary>Parameters for update service role request.</summary>
public sealed record UpdateServiceRoleRequest : IFormSerializable
{
    /// <summary>The returned permission in this page.</summary>
    public required IReadOnlyList<string> Permission { get; init; }

    /// <summary>Render the declared request fields as form pairs; lists use repeated keys and null values are omitted by the transport.</summary>
    public IEnumerable<KeyValuePair<string, string?>> ToForm()
    {
        foreach (var p in Permission)
        {
            yield return new("Permission", p);
        }
    }
}

/// <summary>Parameters for create service user request.</summary>
public sealed record CreateServiceUserRequest : IFormSerializable
{
    /// <summary>Application-defined user identity.</summary>
    public required string Identity { get; init; }
    /// <summary>Human-readable name used to identify this resource.</summary>
    public string? FriendlyName { get; init; }
    /// <summary>Application-defined attributes encoded as a compact JSON string.</summary>
    public string? Attributes { get; init; }
    /// <summary>SID of the role controlling the participant or user permissions.</summary>
    public string? RoleSid { get; init; }

    /// <summary>Render the declared request fields as form pairs; lists use repeated keys and null values are omitted by the transport.</summary>
    public IEnumerable<KeyValuePair<string, string?>> ToForm()
    {
        yield return new("Identity", Identity);
        yield return new("FriendlyName", FriendlyName);
        yield return new("Attributes", Attributes);
        yield return new("RoleSid", RoleSid);
    }
}

/// <summary>Parameters for update service user request.</summary>
public sealed record UpdateServiceUserRequest : IFormSerializable
{
    /// <summary>Human-readable name used to identify this resource.</summary>
    public string? FriendlyName { get; init; }
    /// <summary>Application-defined attributes encoded as a compact JSON string.</summary>
    public string? Attributes { get; init; }
    /// <summary>SID of the role controlling the participant or user permissions.</summary>
    public string? RoleSid { get; init; }

    /// <summary>Render the declared request fields as form pairs; lists use repeated keys and null values are omitted by the transport.</summary>
    public IEnumerable<KeyValuePair<string, string?>> ToForm()
    {
        yield return new("FriendlyName", FriendlyName);
        yield return new("Attributes", Attributes);
        yield return new("RoleSid", RoleSid);
    }
}

/// <summary>Parameters for create service conversation with participants request.</summary>
public sealed record CreateServiceConversationWithParticipantsRequest : IFormSerializable
{
    /// <summary>Human-readable name used to identify this resource.</summary>
    public string? FriendlyName { get; init; }
    /// <summary>Application-defined unique name that can be used instead of the SID.</summary>
    public string? UniqueName { get; init; }
    /// <summary>SID of the Messaging Service associated with this resource.</summary>
    public string? MessagingServiceSid { get; init; }
    /// <summary>Application-defined attributes encoded as a compact JSON string.</summary>
    public string? Attributes { get; init; }
    /// <summary>Lifecycle state of this resource; future values are preserved as strings.</summary>
    public string? State { get; init; }
    /// <summary>ISO 8601 duration before the active Conversation becomes inactive.</summary>
    public string? TimersInactive { get; init; }
    /// <summary>ISO 8601 duration before the inactive Conversation is automatically closed.</summary>
    public string? TimersClosed { get; init; }
    /// <summary>Repeated. Each entry is a JSON object describing one participant.</summary>
    public IReadOnlyList<string>? Participant { get; init; }

    /// <summary>Render the declared request fields as form pairs; lists use repeated keys and null values are omitted by the transport.</summary>
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
    /// <summary>Application-defined user identity.</summary>
    public string? Identity { get; init; }
    /// <summary>Channel address associated with the binding, such as an SMS number or email address.</summary>
    public string? Address { get; init; }
    /// <summary>Requested number of items per page.</summary>
    public int? PageSize { get; init; }

    /// <summary>Render the declared filters as query pairs; null values are omitted by the transport.</summary>
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
    /// <summary>Channel type associated with this binding, such as SMS, WhatsApp or chat.</summary>
    public string? BindingType { get; init; }
    /// <summary>Application-defined user identity.</summary>
    public string? Identity { get; init; }
    /// <summary>Requested number of items per page.</summary>
    public int? PageSize { get; init; }

    /// <summary>Render the declared filters as query pairs; null values are omitted by the transport.</summary>
    public IEnumerable<KeyValuePair<string, string?>> ToQuery()
    {
        yield return new("BindingType", BindingType);
        yield return new("Identity", Identity);
        yield return new("PageSize", PageSize?.ToString());
    }
}

/// <summary>Parameters for update service configuration request.</summary>
public sealed record UpdateServiceConfigurationRequest : IFormSerializable
{
    /// <summary>SID of the related Default Chat Service Role.</summary>
    public string? DefaultChatServiceRoleSid { get; init; }
    /// <summary>SID of the related Default Conversation Creator Role.</summary>
    public string? DefaultConversationCreatorRoleSid { get; init; }
    /// <summary>SID of the related Default Conversation Role.</summary>
    public string? DefaultConversationRoleSid { get; init; }
    /// <summary>Whether reachability enabled is enabled for this operation.</summary>
    public bool? ReachabilityEnabled { get; init; }

    /// <summary>Render the declared request fields as form pairs; lists use repeated keys and null values are omitted by the transport.</summary>
    public IEnumerable<KeyValuePair<string, string?>> ToForm()
    {
        yield return new("DefaultChatServiceRoleSid", DefaultChatServiceRoleSid);
        yield return new("DefaultConversationCreatorRoleSid", DefaultConversationCreatorRoleSid);
        yield return new("DefaultConversationRoleSid", DefaultConversationRoleSid);
        yield return new("ReachabilityEnabled", FormHelpers.BoolStr(ReachabilityEnabled));
    }
}

/// <summary>Parameters for update service notification request.</summary>
public sealed record UpdateServiceNotificationRequest : IFormSerializable
{
    /// <summary>Whether log enabled is enabled for this operation.</summary>
    public bool? LogEnabled { get; init; }
    /// <summary>Whether new message enabled is enabled for this operation.</summary>
    public bool? NewMessageEnabled { get; init; }
    /// <summary>Text template used for a new-message push notification.</summary>
    public string? NewMessageTemplate { get; init; }
    /// <summary>Sound identifier played for a new-message push notification.</summary>
    public string? NewMessageSound { get; init; }
    /// <summary>Whether new message badge count enabled is enabled for this operation.</summary>
    public bool? NewMessageBadgeCountEnabled { get; init; }
    /// <summary>Whether new message with media enabled is enabled for this operation.</summary>
    public bool? NewMessageWithMediaEnabled { get; init; }
    /// <summary>Text template used when a push notification includes media attachments.</summary>
    public string? NewMessageWithMediaTemplate { get; init; }
    /// <summary>Whether added to conversation enabled is enabled for this operation.</summary>
    public bool? AddedToConversationEnabled { get; init; }
    /// <summary>Text template used for an added-to-Conversation push notification.</summary>
    public string? AddedToConversationTemplate { get; init; }
    /// <summary>Sound identifier played for an added-to-Conversation push notification.</summary>
    public string? AddedToConversationSound { get; init; }
    /// <summary>Whether removed from conversation enabled is enabled for this operation.</summary>
    public bool? RemovedFromConversationEnabled { get; init; }
    /// <summary>Text template used for a removed-from-Conversation push notification.</summary>
    public string? RemovedFromConversationTemplate { get; init; }
    /// <summary>Sound identifier played for a removed-from-Conversation push notification.</summary>
    public string? RemovedFromConversationSound { get; init; }

    /// <summary>Render the declared request fields as form pairs; lists use repeated keys and null values are omitted by the transport.</summary>
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

/// <summary>Parameters for update service webhook configuration request.</summary>
public sealed record UpdateServiceWebhookConfigurationRequest : IFormSerializable
{
    /// <summary>URL used for pre webhook.</summary>
    public string? PreWebhookUrl { get; init; }
    /// <summary>URL used for post webhook.</summary>
    public string? PostWebhookUrl { get; init; }
    /// <summary>HTTP method used to fetch `Url` (Twilio default = POST).</summary>
    public string? Method { get; init; }
    /// <summary>The returned filters in this page.</summary>
    public IReadOnlyList<string>? Filters { get; init; }

    /// <summary>Render the declared request fields as form pairs; lists use repeated keys and null values are omitted by the transport.</summary>
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
