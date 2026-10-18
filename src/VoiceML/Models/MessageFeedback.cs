using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace VoiceML.Models;

/// <summary>Delivery-quality feedback echoed back on a Message
/// (<c>POST /2010-04-01/Accounts/{AccountSid}/Messages/{Sid}/Feedback.json</c>). The
/// outcome is recorded on the response only; there is no separate downstream analytics
/// surface.</summary>
public sealed record MessageFeedback
{
    /// <summary>Owning account SID.</summary>
    [JsonPropertyName("account_sid")] public string AccountSid { get; init; } = "";

    /// <summary>SID of the Message the feedback was submitted for.</summary>
    [JsonPropertyName("message_sid")] public string MessageSid { get; init; } = "";

    /// <summary>Delivery outcome. See <see cref="VoiceML.MessageFeedbackOutcome"/> for the
    /// allowed string constants; future values are preserved as strings.</summary>
    [JsonPropertyName("outcome")] public string Outcome { get; init; } = "";

    /// <summary>Creation timestamp (RFC 2822).</summary>
    [JsonPropertyName("date_created")] public string DateCreated { get; init; } = "";

    /// <summary>Last-modification timestamp (RFC 2822).</summary>
    [JsonPropertyName("date_updated")] public string DateUpdated { get; init; } = "";

    /// <summary>URI of this Feedback resource.</summary>
    [JsonPropertyName("uri")] public string Uri { get; init; } = "";
}

/// <summary>Body for <c>POST /Messages/{Sid}/Feedback.json</c>. Any value other than
/// <see cref="VoiceML.MessageFeedbackOutcome.Confirmed"/> — including an omitted value —
/// is echoed back as <c>unconfirmed</c>; feedback is not persisted.</summary>
public sealed record CreateMessageFeedbackRequest : IFormSerializable
{
    /// <summary>Delivery outcome — <see cref="VoiceML.MessageFeedbackOutcome.Confirmed"/> or
    /// <see cref="VoiceML.MessageFeedbackOutcome.Unconfirmed"/>. Optional: omitted values are
    /// treated as <c>unconfirmed</c>.</summary>
    public string? Outcome { get; init; }

    /// <inheritdoc/>
    public IEnumerable<KeyValuePair<string, string?>> ToForm()
    {
        yield return new("Outcome", Outcome);
    }
}
