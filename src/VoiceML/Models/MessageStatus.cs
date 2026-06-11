namespace VoiceML;

/// <summary>String constants for the <c>status</c> field on Message resources.
/// Use these instead of raw string literals for comparison safety.
/// <para>Today the SDK 2.2 gateway only emits <c>sent</c> or <c>failed</c>; the wider
/// Twilio-compatible enum is documented here so SDK code can switch over the full set.</para></summary>
public static class MessageStatus
{
    /// <summary>Message accepted by the API and queued for dispatch.</summary>
    public const string Queued = "queued";

    /// <summary>Message handed to the carrier and in flight.</summary>
    public const string Sending = "sending";

    /// <summary>Carrier accepted the message.</summary>
    public const string Sent = "sent";

    /// <summary>Dispatch failed (see <c>error_code</c> for the reason).</summary>
    public const string Failed = "failed";

    /// <summary>Handset delivery receipt observed.</summary>
    public const string Delivered = "delivered";

    /// <summary>Carrier reported non-delivery.</summary>
    public const string Undelivered = "undelivered";

    /// <summary>Inbound message being received.</summary>
    public const string Receiving = "receiving";

    /// <summary>Inbound message received.</summary>
    public const string Received = "received";

    /// <summary>Carrier accepted the message for delivery.</summary>
    public const string Accepted = "accepted";

    /// <summary>Message scheduled for a later send time.</summary>
    public const string Scheduled = "scheduled";

    /// <summary>Handset read-receipt observed (where supported).</summary>
    public const string Read = "read";

    /// <summary>Send was canceled before dispatch.</summary>
    public const string Canceled = "canceled";
}
