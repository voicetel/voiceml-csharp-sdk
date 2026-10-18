using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace VoiceML.Models;

// ===========================================================================
// Conversations v1 (conversations.twilio.com/v1)
// Twilio-compatible /v1 surface. Account resolves from HTTP Basic auth,
// dates are ISO-8601, list responses carry the VoiceV1Meta envelope.
// ===========================================================================

// ---- Response shapes -------------------------------------------------------

/// <summary>A Conversation (stateful messaging thread) — <c>CH…</c>.</summary>
public sealed record ConversationsV1Conversation
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

/// <summary>Page of conversation resources with continuation metadata.</summary>
public sealed record ConversationsV1ConversationList : IV1PagedList
{
    /// <summary>The returned conversations in this page.</summary>
    [JsonPropertyName("conversations")] public List<ConversationsV1Conversation> Conversations { get; init; } = new();
    /// <summary>Pagination metadata; follow next_page_url to fetch the next page.</summary>
    [JsonPropertyName("meta")] public VoiceV1Meta? Meta { get; init; }
}

/// <summary>A message inside a Conversation — <c>IM…</c>.</summary>
public sealed record ConversationsV1ConversationMessage
{
    /// <summary>SID of the account that owns this resource.</summary>
    [JsonPropertyName("account_sid")] public string? AccountSid { get; init; }
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

/// <summary>Page of conversation message resources with continuation metadata.</summary>
public sealed record ConversationsV1ConversationMessageList : IV1PagedList
{
    /// <summary>The returned messages in this page.</summary>
    [JsonPropertyName("messages")] public List<ConversationsV1ConversationMessage> Messages { get; init; } = new();
    /// <summary>Pagination metadata; follow next_page_url to fetch the next page.</summary>
    [JsonPropertyName("meta")] public VoiceV1Meta? Meta { get; init; }
}

/// <summary>A participant in a Conversation — <c>MB…</c>.</summary>
public sealed record ConversationsV1ConversationParticipant
{
    /// <summary>SID of the account that owns this resource.</summary>
    [JsonPropertyName("account_sid")] public string? AccountSid { get; init; }
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

/// <summary>Page of conversation participant resources with continuation metadata.</summary>
public sealed record ConversationsV1ConversationParticipantList : IV1PagedList
{
    /// <summary>The returned participants in this page.</summary>
    [JsonPropertyName("participants")] public List<ConversationsV1ConversationParticipant> Participants { get; init; } = new();
    /// <summary>Pagination metadata; follow next_page_url to fetch the next page.</summary>
    [JsonPropertyName("meta")] public VoiceV1Meta? Meta { get; init; }
}

/// <summary>A per-channel delivery receipt for a Conversation Message — <c>DY…</c>.</summary>
public sealed record ConversationsV1ConversationMessageReceipt
{
    /// <summary>SID of the account that owns this resource.</summary>
    [JsonPropertyName("account_sid")] public string? AccountSid { get; init; }
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

/// <summary>Page of conversation message receipt resources with continuation metadata.</summary>
public sealed record ConversationsV1ConversationMessageReceiptList : IV1PagedList
{
    /// <summary>The returned delivery receipts in this page.</summary>
    [JsonPropertyName("delivery_receipts")] public List<ConversationsV1ConversationMessageReceipt> DeliveryReceipts { get; init; } = new();
    /// <summary>Pagination metadata; follow next_page_url to fetch the next page.</summary>
    [JsonPropertyName("meta")] public VoiceV1Meta? Meta { get; init; }
}

/// <summary>A conversation-scoped webhook — <c>WH…</c>.</summary>
public sealed record ConversationsV1ConversationScopedWebhook
{
    /// <summary>Unique SID identifying this resource.</summary>
    [JsonPropertyName("sid")] public string? Sid { get; init; }
    /// <summary>SID of the account that owns this resource.</summary>
    [JsonPropertyName("account_sid")] public string? AccountSid { get; init; }
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

/// <summary>Page of conversation scoped webhook resources with continuation metadata.</summary>
public sealed record ConversationsV1ConversationScopedWebhookList : IV1PagedList
{
    /// <summary>The returned webhooks in this page.</summary>
    [JsonPropertyName("webhooks")] public List<ConversationsV1ConversationScopedWebhook> Webhooks { get; init; } = new();
    /// <summary>Pagination metadata; follow next_page_url to fetch the next page.</summary>
    [JsonPropertyName("meta")] public VoiceV1Meta? Meta { get; init; }
}

/// <summary>A Role in Conversations — <c>RL…</c>.</summary>
public sealed record ConversationsV1Role
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

/// <summary>Page of role resources with continuation metadata.</summary>
public sealed record ConversationsV1RoleList : IV1PagedList
{
    /// <summary>The returned roles in this page.</summary>
    [JsonPropertyName("roles")] public List<ConversationsV1Role> Roles { get; init; } = new();
    /// <summary>Pagination metadata; follow next_page_url to fetch the next page.</summary>
    [JsonPropertyName("meta")] public VoiceV1Meta? Meta { get; init; }
}

/// <summary>A User in Conversations — <c>US…</c>.</summary>
public sealed record ConversationsV1User
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

/// <summary>Page of user resources with continuation metadata.</summary>
public sealed record ConversationsV1UserList : IV1PagedList
{
    /// <summary>The returned users in this page.</summary>
    [JsonPropertyName("users")] public List<ConversationsV1User> Users { get; init; } = new();
    /// <summary>Pagination metadata; follow next_page_url to fetch the next page.</summary>
    [JsonPropertyName("meta")] public VoiceV1Meta? Meta { get; init; }
}

/// <summary>A push notification Credential — <c>CR…</c>.</summary>
public sealed record ConversationsV1Credential
{
    /// <summary>Unique SID identifying this resource.</summary>
    [JsonPropertyName("sid")] public string? Sid { get; init; }
    /// <summary>SID of the account that owns this resource.</summary>
    [JsonPropertyName("account_sid")] public string? AccountSid { get; init; }
    /// <summary>Human-readable name used to identify this resource.</summary>
    [JsonPropertyName("friendly_name")] public string? FriendlyName { get; init; }
    /// <summary>Type: apn, gcm, fcm.</summary>
    [JsonPropertyName("type")] public string? Type { get; init; }
    /// <summary>Whether this channel credential is configured for the provider sandbox environment.</summary>
    [JsonPropertyName("sandbox")] public string? Sandbox { get; init; }
    /// <summary>Timestamp when this resource was created.</summary>
    [JsonPropertyName("date_created")] public string? DateCreated { get; init; }
    /// <summary>Timestamp of the most recent modification to this resource.</summary>
    [JsonPropertyName("date_updated")] public string? DateUpdated { get; init; }
    /// <summary>Absolute URL of this resource.</summary>
    [JsonPropertyName("url")] public string? Url { get; init; }
}

/// <summary>Page of credential resources with continuation metadata.</summary>
public sealed record ConversationsV1CredentialList : IV1PagedList
{
    /// <summary>The returned credentials in this page.</summary>
    [JsonPropertyName("credentials")] public List<ConversationsV1Credential> Credentials { get; init; } = new();
    /// <summary>Pagination metadata; follow next_page_url to fetch the next page.</summary>
    [JsonPropertyName("meta")] public VoiceV1Meta? Meta { get; init; }
}

/// <summary>Account-global Conversations Configuration (no sid; singleton per account).</summary>
public sealed record ConversationsV1Configuration
{
    /// <summary>SID of the account that owns this resource.</summary>
    [JsonPropertyName("account_sid")] public string? AccountSid { get; init; }
    /// <summary>SID of the related Default Chat Service.</summary>
    [JsonPropertyName("default_chat_service_sid")] public string? DefaultChatServiceSid { get; init; }
    /// <summary>SID of the related Default Messaging Service.</summary>
    [JsonPropertyName("default_messaging_service_sid")] public string? DefaultMessagingServiceSid { get; init; }
    /// <summary>Default ISO 8601 duration before an active Conversation becomes inactive.</summary>
    [JsonPropertyName("default_inactive_timer")] public string? DefaultInactiveTimer { get; init; }
    /// <summary>Default ISO 8601 duration before an inactive Conversation is closed.</summary>
    [JsonPropertyName("default_closed_timer")] public string? DefaultClosedTimer { get; init; }
    /// <summary>Absolute URL of this resource.</summary>
    [JsonPropertyName("url")] public string? Url { get; init; }
    /// <summary>URLs of related resources and child collections.</summary>
    [JsonPropertyName("links")] public JsonElement? Links { get; init; }
}

/// <summary>Account-global Conversation webhook config (singleton).</summary>
public sealed record ConversationsV1ConfigurationWebhook
{
    /// <summary>SID of the account that owns this resource.</summary>
    [JsonPropertyName("account_sid")] public string? AccountSid { get; init; }
    /// <summary>HTTP method used to fetch `Url` (Twilio default = POST).</summary>
    [JsonPropertyName("method")] public string? Method { get; init; }
    /// <summary>The returned filters in this page.</summary>
    [JsonPropertyName("filters")] public List<string>? Filters { get; init; }
    /// <summary>URL used for pre webhook.</summary>
    [JsonPropertyName("pre_webhook_url")] public string? PreWebhookUrl { get; init; }
    /// <summary>URL used for post webhook.</summary>
    [JsonPropertyName("post_webhook_url")] public string? PostWebhookUrl { get; init; }
    /// <summary>The SIP URI target.</summary>
    [JsonPropertyName("target")] public string? Target { get; init; }
    /// <summary>Absolute URL of this resource.</summary>
    [JsonPropertyName("url")] public string? Url { get; init; }
}

/// <summary>A Configuration Address binding (e.g. sms phone number to auto-create conversations) — <c>IG…</c>.</summary>
public sealed record ConversationsV1ConfigAddress
{
    /// <summary>Unique SID identifying this resource.</summary>
    [JsonPropertyName("sid")] public string? Sid { get; init; }
    /// <summary>SID of the account that owns this resource.</summary>
    [JsonPropertyName("account_sid")] public string? AccountSid { get; init; }
    /// <summary>Provider-defined category identifying the kind of resource or channel.</summary>
    [JsonPropertyName("type")] public string? Type { get; init; }
    /// <summary>Channel address associated with the binding, such as an SMS number or email address.</summary>
    [JsonPropertyName("address")] public string? Address { get; init; }
    /// <summary>Human-readable name used to identify this resource.</summary>
    [JsonPropertyName("friendly_name")] public string? FriendlyName { get; init; }
    /// <summary>Whether inbound messages to this address automatically create a Conversation.</summary>
    [JsonPropertyName("auto_creation")] public JsonElement? AutoCreation { get; init; }
    /// <summary>Timestamp when this resource was created.</summary>
    [JsonPropertyName("date_created")] public string? DateCreated { get; init; }
    /// <summary>Timestamp of the most recent modification to this resource.</summary>
    [JsonPropertyName("date_updated")] public string? DateUpdated { get; init; }
    /// <summary>Absolute URL of this resource.</summary>
    [JsonPropertyName("url")] public string? Url { get; init; }
    /// <summary>ISO country code used to qualify the channel address.</summary>
    [JsonPropertyName("address_country")] public string? AddressCountry { get; init; }
}

/// <summary>Page of config address resources with continuation metadata.</summary>
public sealed record ConversationsV1ConfigAddressList : IV1PagedList
{
    /// <summary>The returned addresses in this page.</summary>
    [JsonPropertyName("addresses")] public List<ConversationsV1ConfigAddress> Addresses { get; init; } = new();
    /// <summary>Pagination metadata; follow next_page_url to fetch the next page.</summary>
    [JsonPropertyName("meta")] public VoiceV1Meta? Meta { get; init; }
}

/// <summary>A "flattened" view of a participant + the conversations they belong to.</summary>
public sealed record ConversationsV1ParticipantConversation
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

/// <summary>Page of participant conversation resources with continuation metadata.</summary>
public sealed record ConversationsV1ParticipantConversationList : IV1PagedList
{
    /// <summary>The returned conversations in this page.</summary>
    [JsonPropertyName("conversations")] public List<ConversationsV1ParticipantConversation> Conversations { get; init; } = new();
    /// <summary>Pagination metadata; follow next_page_url to fetch the next page.</summary>
    [JsonPropertyName("meta")] public VoiceV1Meta? Meta { get; init; }
}

/// <summary>A Conversation created via <c>/v1/ConversationWithParticipants</c>. Same shape as
/// <see cref="ConversationsV1Conversation"/> but distinct schema in spec.</summary>
public sealed record ConversationsV1ConversationWithParticipants
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

/// <summary>Per-user view of the conversations they belong to.</summary>
public sealed record ConversationsV1UserConversation
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

/// <summary>Page of user conversation resources with continuation metadata.</summary>
public sealed record ConversationsV1UserConversationList : IV1PagedList
{
    /// <summary>The returned conversations in this page.</summary>
    [JsonPropertyName("conversations")] public List<ConversationsV1UserConversation> Conversations { get; init; } = new();
    /// <summary>Pagination metadata; follow next_page_url to fetch the next page.</summary>
    [JsonPropertyName("meta")] public VoiceV1Meta? Meta { get; init; }
}

/// <summary>A Conversations Service — <c>IS…</c> (isolation boundary for conversations).</summary>
public sealed record ConversationsV1Service
{
    /// <summary>Unique SID identifying this resource.</summary>
    [JsonPropertyName("sid")] public string? Sid { get; init; }
    /// <summary>SID of the account that owns this resource.</summary>
    [JsonPropertyName("account_sid")] public string? AccountSid { get; init; }
    /// <summary>Human-readable name used to identify this resource.</summary>
    [JsonPropertyName("friendly_name")] public string? FriendlyName { get; init; }
    /// <summary>Timestamp when this resource was created.</summary>
    [JsonPropertyName("date_created")] public string? DateCreated { get; init; }
    /// <summary>Timestamp of the most recent modification to this resource.</summary>
    [JsonPropertyName("date_updated")] public string? DateUpdated { get; init; }
    /// <summary>Absolute URL of this resource.</summary>
    [JsonPropertyName("url")] public string? Url { get; init; }
    /// <summary>URLs of related resources and child collections.</summary>
    [JsonPropertyName("links")] public JsonElement? Links { get; init; }
}

/// <summary>Page of service resources with continuation metadata.</summary>
public sealed record ConversationsV1ServiceList : IV1PagedList
{
    /// <summary>The returned services in this page.</summary>
    [JsonPropertyName("services")] public List<ConversationsV1Service> Services { get; init; } = new();
    /// <summary>Pagination metadata; follow next_page_url to fetch the next page.</summary>
    [JsonPropertyName("meta")] public VoiceV1Meta? Meta { get; init; }
}

// ---- Request bodies (form-encoded) ----------------------------------------

/// <summary>Common <c>PageSize</c> query for /v1 list endpoints.</summary>
public sealed record ListV1PageParams
{
    /// <summary>Requested number of items per page.</summary>
    public int? PageSize { get; init; }

    /// <summary>Render the declared filters as query pairs; null values are omitted by the transport.</summary>
    public IEnumerable<KeyValuePair<string, string?>> ToQuery()
    {
        yield return new("PageSize", PageSize?.ToString());
    }
}

/// <summary>Parameters for create conversation request.</summary>
public sealed record CreateConversationRequest : IFormSerializable
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
    /// <summary>Email address used for this Conversation channel binding.</summary>
    public string? BindingsEmailAddress { get; init; }
    /// <summary>Display name used for the Conversation email binding.</summary>
    public string? BindingsEmailName { get; init; }

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
        yield return new("Bindings.Email.Address", BindingsEmailAddress);
        yield return new("Bindings.Email.Name", BindingsEmailName);
    }
}

/// <summary>Parameters for update conversation request.</summary>
public sealed record UpdateConversationRequest : IFormSerializable
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

/// <summary>Parameters for create conversation message request.</summary>
public sealed record CreateConversationMessageRequest : IFormSerializable
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

/// <summary>Parameters for update conversation message request.</summary>
public sealed record UpdateConversationMessageRequest : IFormSerializable
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

/// <summary>Parameters for create conversation participant request.</summary>
public sealed record CreateConversationParticipantRequest : IFormSerializable
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

/// <summary>Parameters for update conversation participant request.</summary>
public sealed record UpdateConversationParticipantRequest : IFormSerializable
{
    /// <summary>Application-defined user identity.</summary>
    public string? Identity { get; init; }
    /// <summary>Application-defined attributes encoded as a compact JSON string.</summary>
    public string? Attributes { get; init; }
    /// <summary>SID of the role controlling the participant or user permissions.</summary>
    public string? RoleSid { get; init; }
    /// <summary>Index of the most recent message marked as read by this participant.</summary>
    public int? LastReadMessageIndex { get; init; }
    /// <summary>Timestamp when this participant last updated its read position.</summary>
    public string? LastReadTimestamp { get; init; }

    /// <summary>Render the declared request fields as form pairs; lists use repeated keys and null values are omitted by the transport.</summary>
    public IEnumerable<KeyValuePair<string, string?>> ToForm()
    {
        yield return new("Identity", Identity);
        yield return new("Attributes", Attributes);
        yield return new("RoleSid", RoleSid);
        yield return new("LastReadMessageIndex", LastReadMessageIndex?.ToString());
        yield return new("LastReadTimestamp", LastReadTimestamp);
    }
}

/// <summary>Parameters for create conversation scoped webhook request.</summary>
public sealed record CreateConversationScopedWebhookRequest : IFormSerializable
{
    /// <summary>The SIP URI target.</summary>
    public required string Target { get; init; }
    /// <summary>URL used for configuration.</summary>
    public string? ConfigurationUrl { get; init; }
    /// <summary>HTTP method for configuration requests.</summary>
    public string? ConfigurationMethod { get; init; }
    /// <summary>SID of the related Configuration Flow.</summary>
    public string? ConfigurationFlowSid { get; init; }
    /// <summary>Message index after which a replaying webhook begins receiving Conversation events.</summary>
    public int? ConfigurationReplayAfter { get; init; }

    /// <summary>Render the declared request fields as form pairs; lists use repeated keys and null values are omitted by the transport.</summary>
    public IEnumerable<KeyValuePair<string, string?>> ToForm()
    {
        yield return new("Target", Target);
        yield return new("Configuration.Url", ConfigurationUrl);
        yield return new("Configuration.Method", ConfigurationMethod);
        yield return new("Configuration.FlowSid", ConfigurationFlowSid);
        yield return new("Configuration.ReplayAfter", ConfigurationReplayAfter?.ToString());
    }
}

/// <summary>Parameters for update conversation scoped webhook request.</summary>
public sealed record UpdateConversationScopedWebhookRequest : IFormSerializable
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

/// <summary>Parameters for create conversations role request.</summary>
public sealed record CreateConversationsRoleRequest : IFormSerializable
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

/// <summary>Parameters for update conversations role request.</summary>
public sealed record UpdateConversationsRoleRequest : IFormSerializable
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

/// <summary>Parameters for create conversations user request.</summary>
public sealed record CreateConversationsUserRequest : IFormSerializable
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

/// <summary>Parameters for update conversations user request.</summary>
public sealed record UpdateConversationsUserRequest : IFormSerializable
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

/// <summary>Parameters for update user conversation request.</summary>
public sealed record UpdateUserConversationRequest : IFormSerializable
{
    /// <summary>Per-user Conversation notification preference, such as default or muted.</summary>
    public string? NotificationLevel { get; init; }
    /// <summary>Index of the most recent message marked as read by this participant.</summary>
    public int? LastReadMessageIndex { get; init; }
    /// <summary>Timestamp when this participant last updated its read position.</summary>
    public string? LastReadTimestamp { get; init; }

    /// <summary>Render the declared request fields as form pairs; lists use repeated keys and null values are omitted by the transport.</summary>
    public IEnumerable<KeyValuePair<string, string?>> ToForm()
    {
        yield return new("NotificationLevel", NotificationLevel);
        yield return new("LastReadMessageIndex", LastReadMessageIndex?.ToString());
        yield return new("LastReadTimestamp", LastReadTimestamp);
    }
}

/// <summary>Parameters for create conversations credential request.</summary>
public sealed record CreateConversationsCredentialRequest : IFormSerializable
{
    /// <summary>Provider-defined category identifying the kind of resource or channel.</summary>
    public required string Type { get; init; }
    /// <summary>Human-readable name used to identify this resource.</summary>
    public string? FriendlyName { get; init; }
    /// <summary>Public certificate used to authenticate the channel integration.</summary>
    public string? Certificate { get; init; }
    /// <summary>Private key used to authenticate the channel provider; keep it confidential.</summary>
    public string? PrivateKey { get; init; }
    /// <summary>Whether sandbox is enabled for this operation.</summary>
    public bool? Sandbox { get; init; }
    /// <summary>API key used to authenticate the channel provider; treat it as a secret.</summary>
    public string? ApiKey { get; init; }
    /// <summary>Provider authentication secret used by this channel credential.</summary>
    public string? Secret { get; init; }

    /// <summary>Render the declared request fields as form pairs; lists use repeated keys and null values are omitted by the transport.</summary>
    public IEnumerable<KeyValuePair<string, string?>> ToForm()
    {
        yield return new("Type", Type);
        yield return new("FriendlyName", FriendlyName);
        yield return new("Certificate", Certificate);
        yield return new("PrivateKey", PrivateKey);
        yield return new("Sandbox", FormHelpers.BoolStr(Sandbox));
        yield return new("ApiKey", ApiKey);
        yield return new("Secret", Secret);
    }
}

/// <summary>Parameters for update conversations credential request.</summary>
public sealed record UpdateConversationsCredentialRequest : IFormSerializable
{
    /// <summary>Provider-defined category identifying the kind of resource or channel.</summary>
    public string? Type { get; init; }
    /// <summary>Human-readable name used to identify this resource.</summary>
    public string? FriendlyName { get; init; }
    /// <summary>Public certificate used to authenticate the channel integration.</summary>
    public string? Certificate { get; init; }
    /// <summary>Private key used to authenticate the channel provider; keep it confidential.</summary>
    public string? PrivateKey { get; init; }
    /// <summary>Whether sandbox is enabled for this operation.</summary>
    public bool? Sandbox { get; init; }
    /// <summary>API key used to authenticate the channel provider; treat it as a secret.</summary>
    public string? ApiKey { get; init; }
    /// <summary>Provider authentication secret used by this channel credential.</summary>
    public string? Secret { get; init; }

    /// <summary>Render the declared request fields as form pairs; lists use repeated keys and null values are omitted by the transport.</summary>
    public IEnumerable<KeyValuePair<string, string?>> ToForm()
    {
        yield return new("Type", Type);
        yield return new("FriendlyName", FriendlyName);
        yield return new("Certificate", Certificate);
        yield return new("PrivateKey", PrivateKey);
        yield return new("Sandbox", FormHelpers.BoolStr(Sandbox));
        yield return new("ApiKey", ApiKey);
        yield return new("Secret", Secret);
    }
}

/// <summary>Parameters for update conversations configuration request.</summary>
public sealed record UpdateConversationsConfigurationRequest : IFormSerializable
{
    /// <summary>SID of the related Default Chat Service.</summary>
    public string? DefaultChatServiceSid { get; init; }
    /// <summary>SID of the related Default Messaging Service.</summary>
    public string? DefaultMessagingServiceSid { get; init; }
    /// <summary>Default ISO 8601 duration before an active Conversation becomes inactive.</summary>
    public string? DefaultInactiveTimer { get; init; }
    /// <summary>Default ISO 8601 duration before an inactive Conversation is closed.</summary>
    public string? DefaultClosedTimer { get; init; }

    /// <summary>Render the declared request fields as form pairs; lists use repeated keys and null values are omitted by the transport.</summary>
    public IEnumerable<KeyValuePair<string, string?>> ToForm()
    {
        yield return new("DefaultChatServiceSid", DefaultChatServiceSid);
        yield return new("DefaultMessagingServiceSid", DefaultMessagingServiceSid);
        yield return new("DefaultInactiveTimer", DefaultInactiveTimer);
        yield return new("DefaultClosedTimer", DefaultClosedTimer);
    }
}

/// <summary>Parameters for update conversations configuration webhook request.</summary>
public sealed record UpdateConversationsConfigurationWebhookRequest : IFormSerializable
{
    /// <summary>HTTP method used to fetch `Url` (Twilio default = POST).</summary>
    public string? Method { get; init; }
    /// <summary>The returned filters in this page.</summary>
    public IReadOnlyList<string>? Filters { get; init; }
    /// <summary>URL used for pre webhook.</summary>
    public string? PreWebhookUrl { get; init; }
    /// <summary>URL used for post webhook.</summary>
    public string? PostWebhookUrl { get; init; }
    /// <summary>The SIP URI target.</summary>
    public string? Target { get; init; }

    /// <summary>Render the declared request fields as form pairs; lists use repeated keys and null values are omitted by the transport.</summary>
    public IEnumerable<KeyValuePair<string, string?>> ToForm()
    {
        yield return new("Method", Method);
        if (Filters is not null)
        {
            foreach (var f in Filters)
            {
                yield return new("Filters", f);
            }
        }
        yield return new("PreWebhookUrl", PreWebhookUrl);
        yield return new("PostWebhookUrl", PostWebhookUrl);
        yield return new("Target", Target);
    }
}

/// <summary>Parameters for create config address request.</summary>
public sealed record CreateConfigAddressRequest : IFormSerializable
{
    /// <summary>Provider-defined category identifying the kind of resource or channel.</summary>
    public required string Type { get; init; }
    /// <summary>Channel address associated with the binding, such as an SMS number or email address.</summary>
    public required string Address { get; init; }
    /// <summary>Human-readable name used to identify this resource.</summary>
    public string? FriendlyName { get; init; }
    /// <summary>Whether auto creation enabled is enabled for this operation.</summary>
    public bool? AutoCreationEnabled { get; init; }
    /// <summary>Conversation creation policy applied to inbound messages for this address.</summary>
    public string? AutoCreationType { get; init; }
    /// <summary>URL used for auto creation webhook.</summary>
    public string? AutoCreationWebhookUrl { get; init; }
    /// <summary>ISO country code used to qualify the channel address.</summary>
    public string? AddressCountry { get; init; }

    /// <summary>Render the declared request fields as form pairs; lists use repeated keys and null values are omitted by the transport.</summary>
    public IEnumerable<KeyValuePair<string, string?>> ToForm()
    {
        yield return new("Type", Type);
        yield return new("Address", Address);
        yield return new("FriendlyName", FriendlyName);
        yield return new("AutoCreation.Enabled", FormHelpers.BoolStr(AutoCreationEnabled));
        yield return new("AutoCreation.Type", AutoCreationType);
        yield return new("AutoCreation.WebhookUrl", AutoCreationWebhookUrl);
        yield return new("AddressCountry", AddressCountry);
    }
}

/// <summary>Parameters for update config address request.</summary>
public sealed record UpdateConfigAddressRequest : IFormSerializable
{
    /// <summary>Human-readable name used to identify this resource.</summary>
    public string? FriendlyName { get; init; }
    /// <summary>Whether auto creation enabled is enabled for this operation.</summary>
    public bool? AutoCreationEnabled { get; init; }
    /// <summary>Conversation creation policy applied to inbound messages for this address.</summary>
    public string? AutoCreationType { get; init; }
    /// <summary>URL used for auto creation webhook.</summary>
    public string? AutoCreationWebhookUrl { get; init; }

    /// <summary>Render the declared request fields as form pairs; lists use repeated keys and null values are omitted by the transport.</summary>
    public IEnumerable<KeyValuePair<string, string?>> ToForm()
    {
        yield return new("FriendlyName", FriendlyName);
        yield return new("AutoCreation.Enabled", FormHelpers.BoolStr(AutoCreationEnabled));
        yield return new("AutoCreation.Type", AutoCreationType);
        yield return new("AutoCreation.WebhookUrl", AutoCreationWebhookUrl);
    }
}

/// <summary>Query params for <c>GET /v1/ParticipantConversations</c>.</summary>
public sealed record ListParticipantConversationsParams
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

/// <summary>Parameters for create conversation with participants request.</summary>
public sealed record CreateConversationWithParticipantsRequest : IFormSerializable
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

/// <summary>Parameters for create conversations service request.</summary>
public sealed record CreateConversationsServiceRequest : IFormSerializable
{
    /// <summary>Human-readable name used to identify this resource.</summary>
    public required string FriendlyName { get; init; }

    /// <summary>Render the declared request fields as form pairs; lists use repeated keys and null values are omitted by the transport.</summary>
    public IEnumerable<KeyValuePair<string, string?>> ToForm()
    {
        yield return new("FriendlyName", FriendlyName);
    }
}
