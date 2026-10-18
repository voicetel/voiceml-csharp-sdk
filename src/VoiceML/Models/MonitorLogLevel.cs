namespace VoiceML;

/// <summary>String constants for the <c>log_level</c> filter and field on Monitor v1 Alert
/// resources. Only these two severities are distinguished; use the constants instead of raw
/// string literals for comparison safety.</summary>
public static class MonitorLogLevel
{
    /// <summary>Wire value error for the alert severity.</summary>
    public const string Error = "error";

    /// <summary>Wire value warning for the alert severity.</summary>
    public const string Warning = "warning";
}
