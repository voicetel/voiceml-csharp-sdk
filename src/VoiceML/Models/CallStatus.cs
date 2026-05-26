namespace VoiceML;

/// <summary>String constants for the <c>status</c> field on Call resources.
/// Use these instead of raw string literals for comparison safety.</summary>
public static class CallStatus
{
    public const string Queued = "queued";
    public const string Ringing = "ringing";
    public const string InProgress = "in-progress";
    public const string Completed = "completed";
    public const string Busy = "busy";
    public const string NoAnswer = "no-answer";
    public const string Canceled = "canceled";
    public const string Failed = "failed";
}
