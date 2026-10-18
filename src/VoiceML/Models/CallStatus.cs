namespace VoiceML;

/// <summary>String constants for the <c>status</c> field on Call resources.
/// Use these instead of raw string literals for comparison safety.</summary>
public static class CallStatus
{
    /// <summary>Wire value queued for call status.</summary>
    public const string Queued = "queued";
    /// <summary>Wire value ringing for call status.</summary>
    public const string Ringing = "ringing";
    /// <summary>Wire value in-progress for call status.</summary>
    public const string InProgress = "in-progress";
    /// <summary>Wire value completed for call status.</summary>
    public const string Completed = "completed";
    /// <summary>Wire value busy for call status.</summary>
    public const string Busy = "busy";
    /// <summary>Wire value no-answer for call status.</summary>
    public const string NoAnswer = "no-answer";
    /// <summary>Wire value canceled for call status.</summary>
    public const string Canceled = "canceled";
    /// <summary>Wire value failed for call status.</summary>
    public const string Failed = "failed";
}
