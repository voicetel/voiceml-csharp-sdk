namespace VoiceML;

/// <summary>String constants for the <c>status</c> field on Participant resources.</summary>
public static class ParticipantStatus
{
    /// <summary>Wire value queued for participant status.</summary>
    public const string Queued = "queued";
    /// <summary>Wire value connecting for participant status.</summary>
    public const string Connecting = "connecting";
    /// <summary>Wire value ringing for participant status.</summary>
    public const string Ringing = "ringing";
    /// <summary>Wire value connected for participant status.</summary>
    public const string Connected = "connected";
    /// <summary>Wire value on-hold for participant status.</summary>
    public const string OnHold = "on-hold";
    /// <summary>Wire value complete for participant status.</summary>
    public const string Complete = "complete";
    /// <summary>Wire value failed for participant status.</summary>
    public const string Failed = "failed";
    /// <summary>Wire value completed for participant status.</summary>
    public const string Completed = "completed";
}
