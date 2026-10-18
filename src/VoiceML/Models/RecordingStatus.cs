namespace VoiceML;

/// <summary>String constants for the <c>status</c> field on Recording resources.</summary>
public static class RecordingStatus
{
    /// <summary>Wire value in-progress for recording status.</summary>
    public const string InProgress = "in-progress";
    /// <summary>Wire value paused for recording status.</summary>
    public const string Paused = "paused";
    /// <summary>Wire value stopped for recording status.</summary>
    public const string Stopped = "stopped";
    /// <summary>Wire value processing for recording status.</summary>
    public const string Processing = "processing";
    /// <summary>Wire value completed for recording status.</summary>
    public const string Completed = "completed";
    /// <summary>Wire value absent for recording status.</summary>
    public const string Absent = "absent";
    /// <summary>Wire value deleted for recording status.</summary>
    public const string Deleted = "deleted";
}
