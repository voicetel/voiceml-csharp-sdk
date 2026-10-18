namespace VoiceML;

/// <summary>String constants for the <c>outcome</c> field on Message feedback requests and
/// resources. Use these instead of raw string literals for comparison safety.</summary>
public static class MessageFeedbackOutcome
{
    /// <summary>Wire value confirmed — the message was delivered.</summary>
    public const string Confirmed = "confirmed";

    /// <summary>Wire value unconfirmed — delivery could not be confirmed. Any request value
    /// other than <c>confirmed</c>, including an omitted one, is echoed as this.</summary>
    public const string Unconfirmed = "unconfirmed";
}
