namespace VoiceML.Exceptions;

/// <summary>Raised when the client is constructed with missing or conflicting configuration.</summary>
public sealed class ConfigurationException : VoiceMLException
{
    /// <summary>Construct with a message.</summary>
    public ConfigurationException(string message) : base(message) { }
}
