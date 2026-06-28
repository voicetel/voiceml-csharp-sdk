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

public sealed record ConversationsV1ConversationList
{
    [JsonPropertyName("conversations")] public List<ConversationsV1Conversation> Conversations { get; init; } = new();
    [JsonPropertyName("meta")] public VoiceV1Meta? Meta { get; init; }
}

/// <summary>A message inside a Conversation — <c>IM…</c>.</summary>
public sealed record ConversationsV1ConversationMessage
{
    [JsonPropertyName("account_sid")] public string? AccountSid { get; init; }
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

public sealed record ConversationsV1ConversationMessageList
{
    [JsonPropertyName("messages")] public List<ConversationsV1ConversationMessage> Messages { get; init; } = new();
    [JsonPropertyName("meta")] public VoiceV1Meta? Meta { get; init; }
}

/// <summary>A participant in a Conversation — <c>MB…</c>.</summary>
public sealed record ConversationsV1ConversationParticipant
{
    [JsonPropertyName("account_sid")] public string? AccountSid { get; init; }
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

public sealed record ConversationsV1ConversationParticipantList
{
    [JsonPropertyName("participants")] public List<ConversationsV1ConversationParticipant> Participants { get; init; } = new();
    [JsonPropertyName("meta")] public VoiceV1Meta? Meta { get; init; }
}

/// <summary>A per-channel delivery receipt for a Conversation Message — <c>DY…</c>.</summary>
public sealed record ConversationsV1ConversationMessageReceipt
{
    [JsonPropertyName("account_sid")] public string? AccountSid { get; init; }
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

public sealed record ConversationsV1ConversationMessageReceiptList
{
    [JsonPropertyName("delivery_receipts")] public List<ConversationsV1ConversationMessageReceipt> DeliveryReceipts { get; init; } = new();
    [JsonPropertyName("meta")] public VoiceV1Meta? Meta { get; init; }
}

/// <summary>A conversation-scoped webhook — <c>WH…</c>.</summary>
public sealed record ConversationsV1ConversationScopedWebhook
{
    [JsonPropertyName("sid")] public string? Sid { get; init; }
    [JsonPropertyName("account_sid")] public string? AccountSid { get; init; }
    [JsonPropertyName("conversation_sid")] public string? ConversationSid { get; init; }
    [JsonPropertyName("target")] public string? Target { get; init; }
    [JsonPropertyName("url")] public string? Url { get; init; }
    [JsonPropertyName("configuration")] public JsonElement? Configuration { get; init; }
    [JsonPropertyName("date_created")] public string? DateCreated { get; init; }
    [JsonPropertyName("date_updated")] public string? DateUpdated { get; init; }
}

public sealed record ConversationsV1ConversationScopedWebhookList
{
    [JsonPropertyName("webhooks")] public List<ConversationsV1ConversationScopedWebhook> Webhooks { get; init; } = new();
    [JsonPropertyName("meta")] public VoiceV1Meta? Meta { get; init; }
}

/// <summary>A Role in Conversations — <c>RL…</c>.</summary>
public sealed record ConversationsV1Role
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

public sealed record ConversationsV1RoleList
{
    [JsonPropertyName("roles")] public List<ConversationsV1Role> Roles { get; init; } = new();
    [JsonPropertyName("meta")] public VoiceV1Meta? Meta { get; init; }
}

/// <summary>A User in Conversations — <c>US…</c>.</summary>
public sealed record ConversationsV1User
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

public sealed record ConversationsV1UserList
{
    [JsonPropertyName("users")] public List<ConversationsV1User> Users { get; init; } = new();
    [JsonPropertyName("meta")] public VoiceV1Meta? Meta { get; init; }
}

/// <summary>A push notification Credential — <c>CR…</c>.</summary>
public sealed record ConversationsV1Credential
{
    [JsonPropertyName("sid")] public string? Sid { get; init; }
    [JsonPropertyName("account_sid")] public string? AccountSid { get; init; }
    [JsonPropertyName("friendly_name")] public string? FriendlyName { get; init; }
    [JsonPropertyName("type")] public string? Type { get; init; }
    [JsonPropertyName("sandbox")] public string? Sandbox { get; init; }
    [JsonPropertyName("date_created")] public string? DateCreated { get; init; }
    [JsonPropertyName("date_updated")] public string? DateUpdated { get; init; }
    [JsonPropertyName("url")] public string? Url { get; init; }
}

public sealed record ConversationsV1CredentialList
{
    [JsonPropertyName("credentials")] public List<ConversationsV1Credential> Credentials { get; init; } = new();
    [JsonPropertyName("meta")] public VoiceV1Meta? Meta { get; init; }
}

/// <summary>Account-global Conversations Configuration (no sid; singleton per account).</summary>
public sealed record ConversationsV1Configuration
{
    [JsonPropertyName("account_sid")] public string? AccountSid { get; init; }
    [JsonPropertyName("default_chat_service_sid")] public string? DefaultChatServiceSid { get; init; }
    [JsonPropertyName("default_messaging_service_sid")] public string? DefaultMessagingServiceSid { get; init; }
    [JsonPropertyName("default_inactive_timer")] public string? DefaultInactiveTimer { get; init; }
    [JsonPropertyName("default_closed_timer")] public string? DefaultClosedTimer { get; init; }
    [JsonPropertyName("url")] public string? Url { get; init; }
    [JsonPropertyName("links")] public JsonElement? Links { get; init; }
}

/// <summary>Account-global Conversation webhook config (singleton).</summary>
public sealed record ConversationsV1ConfigurationWebhook
{
    [JsonPropertyName("account_sid")] public string? AccountSid { get; init; }
    [JsonPropertyName("method")] public string? Method { get; init; }
    [JsonPropertyName("filters")] public List<string>? Filters { get; init; }
    [JsonPropertyName("pre_webhook_url")] public string? PreWebhookUrl { get; init; }
    [JsonPropertyName("post_webhook_url")] public string? PostWebhookUrl { get; init; }
    [JsonPropertyName("target")] public string? Target { get; init; }
    [JsonPropertyName("url")] public string? Url { get; init; }
}

/// <summary>A Configuration Address binding (e.g. sms phone number to auto-create conversations) — <c>IG…</c>.</summary>
public sealed record ConversationsV1ConfigAddress
{
    [JsonPropertyName("sid")] public string? Sid { get; init; }
    [JsonPropertyName("account_sid")] public string? AccountSid { get; init; }
    [JsonPropertyName("type")] public string? Type { get; init; }
    [JsonPropertyName("address")] public string? Address { get; init; }
    [JsonPropertyName("friendly_name")] public string? FriendlyName { get; init; }
    [JsonPropertyName("auto_creation")] public JsonElement? AutoCreation { get; init; }
    [JsonPropertyName("date_created")] public string? DateCreated { get; init; }
    [JsonPropertyName("date_updated")] public string? DateUpdated { get; init; }
    [JsonPropertyName("url")] public string? Url { get; init; }
    [JsonPropertyName("address_country")] public string? AddressCountry { get; init; }
}

public sealed record ConversationsV1ConfigAddressList
{
    [JsonPropertyName("addresses")] public List<ConversationsV1ConfigAddress> Addresses { get; init; } = new();
    [JsonPropertyName("meta")] public VoiceV1Meta? Meta { get; init; }
}

/// <summary>A "flattened" view of a participant + the conversations they belong to.</summary>
public sealed record ConversationsV1ParticipantConversation
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

public sealed record ConversationsV1ParticipantConversationList
{
    [JsonPropertyName("conversations")] public List<ConversationsV1ParticipantConversation> Conversations { get; init; } = new();
    [JsonPropertyName("meta")] public VoiceV1Meta? Meta { get; init; }
}

/// <summary>A Conversation created via <c>/v1/ConversationWithParticipants</c>. Same shape as
/// <see cref="ConversationsV1Conversation"/> but distinct schema in spec.</summary>
public sealed record ConversationsV1ConversationWithParticipants
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

/// <summary>Per-user view of the conversations they belong to.</summary>
public sealed record ConversationsV1UserConversation
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

public sealed record ConversationsV1UserConversationList
{
    [JsonPropertyName("conversations")] public List<ConversationsV1UserConversation> Conversations { get; init; } = new();
    [JsonPropertyName("meta")] public VoiceV1Meta? Meta { get; init; }
}

/// <summary>A Conversations Service — <c>IS…</c> (isolation boundary for conversations).</summary>
public sealed record ConversationsV1Service
{
    [JsonPropertyName("sid")] public string? Sid { get; init; }
    [JsonPropertyName("account_sid")] public string? AccountSid { get; init; }
    [JsonPropertyName("friendly_name")] public string? FriendlyName { get; init; }
    [JsonPropertyName("date_created")] public string? DateCreated { get; init; }
    [JsonPropertyName("date_updated")] public string? DateUpdated { get; init; }
    [JsonPropertyName("url")] public string? Url { get; init; }
    [JsonPropertyName("links")] public JsonElement? Links { get; init; }
}

public sealed record ConversationsV1ServiceList
{
    [JsonPropertyName("services")] public List<ConversationsV1Service> Services { get; init; } = new();
    [JsonPropertyName("meta")] public VoiceV1Meta? Meta { get; init; }
}

// ---- Request bodies (form-encoded) ----------------------------------------

/// <summary>Common <c>PageSize</c> query for /v1 list endpoints.</summary>
public sealed record ListV1PageParams
{
    public int? PageSize { get; init; }

    public IEnumerable<KeyValuePair<string, string?>> ToQuery()
    {
        yield return new("PageSize", PageSize?.ToString());
    }
}

public sealed record CreateConversationRequest : IFormSerializable
{
    public string? FriendlyName { get; init; }
    public string? UniqueName { get; init; }
    public string? MessagingServiceSid { get; init; }
    public string? Attributes { get; init; }
    public string? State { get; init; }
    public string? TimersInactive { get; init; }
    public string? TimersClosed { get; init; }
    public string? BindingsEmailAddress { get; init; }
    public string? BindingsEmailName { get; init; }

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

public sealed record UpdateConversationRequest : IFormSerializable
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

public sealed record CreateConversationMessageRequest : IFormSerializable
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

public sealed record UpdateConversationMessageRequest : IFormSerializable
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

public sealed record CreateConversationParticipantRequest : IFormSerializable
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

public sealed record UpdateConversationParticipantRequest : IFormSerializable
{
    public string? Identity { get; init; }
    public string? Attributes { get; init; }
    public string? RoleSid { get; init; }
    public int? LastReadMessageIndex { get; init; }
    public string? LastReadTimestamp { get; init; }

    public IEnumerable<KeyValuePair<string, string?>> ToForm()
    {
        yield return new("Identity", Identity);
        yield return new("Attributes", Attributes);
        yield return new("RoleSid", RoleSid);
        yield return new("LastReadMessageIndex", LastReadMessageIndex?.ToString());
        yield return new("LastReadTimestamp", LastReadTimestamp);
    }
}

public sealed record CreateConversationScopedWebhookRequest : IFormSerializable
{
    public required string Target { get; init; }
    public string? ConfigurationUrl { get; init; }
    public string? ConfigurationMethod { get; init; }
    public string? ConfigurationFlowSid { get; init; }
    public int? ConfigurationReplayAfter { get; init; }

    public IEnumerable<KeyValuePair<string, string?>> ToForm()
    {
        yield return new("Target", Target);
        yield return new("Configuration.Url", ConfigurationUrl);
        yield return new("Configuration.Method", ConfigurationMethod);
        yield return new("Configuration.FlowSid", ConfigurationFlowSid);
        yield return new("Configuration.ReplayAfter", ConfigurationReplayAfter?.ToString());
    }
}

public sealed record UpdateConversationScopedWebhookRequest : IFormSerializable
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

public sealed record CreateConversationsRoleRequest : IFormSerializable
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

public sealed record UpdateConversationsRoleRequest : IFormSerializable
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

public sealed record CreateConversationsUserRequest : IFormSerializable
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

public sealed record UpdateConversationsUserRequest : IFormSerializable
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

public sealed record UpdateUserConversationRequest : IFormSerializable
{
    public string? NotificationLevel { get; init; }
    public int? LastReadMessageIndex { get; init; }
    public string? LastReadTimestamp { get; init; }

    public IEnumerable<KeyValuePair<string, string?>> ToForm()
    {
        yield return new("NotificationLevel", NotificationLevel);
        yield return new("LastReadMessageIndex", LastReadMessageIndex?.ToString());
        yield return new("LastReadTimestamp", LastReadTimestamp);
    }
}

public sealed record CreateConversationsCredentialRequest : IFormSerializable
{
    public required string Type { get; init; }
    public string? FriendlyName { get; init; }
    public string? Certificate { get; init; }
    public string? PrivateKey { get; init; }
    public bool? Sandbox { get; init; }
    public string? ApiKey { get; init; }
    public string? Secret { get; init; }

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

public sealed record UpdateConversationsCredentialRequest : IFormSerializable
{
    public string? Type { get; init; }
    public string? FriendlyName { get; init; }
    public string? Certificate { get; init; }
    public string? PrivateKey { get; init; }
    public bool? Sandbox { get; init; }
    public string? ApiKey { get; init; }
    public string? Secret { get; init; }

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

public sealed record UpdateConversationsConfigurationRequest : IFormSerializable
{
    public string? DefaultChatServiceSid { get; init; }
    public string? DefaultMessagingServiceSid { get; init; }
    public string? DefaultInactiveTimer { get; init; }
    public string? DefaultClosedTimer { get; init; }

    public IEnumerable<KeyValuePair<string, string?>> ToForm()
    {
        yield return new("DefaultChatServiceSid", DefaultChatServiceSid);
        yield return new("DefaultMessagingServiceSid", DefaultMessagingServiceSid);
        yield return new("DefaultInactiveTimer", DefaultInactiveTimer);
        yield return new("DefaultClosedTimer", DefaultClosedTimer);
    }
}

public sealed record UpdateConversationsConfigurationWebhookRequest : IFormSerializable
{
    public string? Method { get; init; }
    public IReadOnlyList<string>? Filters { get; init; }
    public string? PreWebhookUrl { get; init; }
    public string? PostWebhookUrl { get; init; }
    public string? Target { get; init; }

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

public sealed record CreateConfigAddressRequest : IFormSerializable
{
    public required string Type { get; init; }
    public required string Address { get; init; }
    public string? FriendlyName { get; init; }
    public bool? AutoCreationEnabled { get; init; }
    public string? AutoCreationType { get; init; }
    public string? AutoCreationWebhookUrl { get; init; }
    public string? AddressCountry { get; init; }

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

public sealed record UpdateConfigAddressRequest : IFormSerializable
{
    public string? FriendlyName { get; init; }
    public bool? AutoCreationEnabled { get; init; }
    public string? AutoCreationType { get; init; }
    public string? AutoCreationWebhookUrl { get; init; }

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

public sealed record CreateConversationWithParticipantsRequest : IFormSerializable
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

public sealed record CreateConversationsServiceRequest : IFormSerializable
{
    public required string FriendlyName { get; init; }

    public IEnumerable<KeyValuePair<string, string?>> ToForm()
    {
        yield return new("FriendlyName", FriendlyName);
    }
}
