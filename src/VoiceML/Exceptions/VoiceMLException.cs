using System;

namespace VoiceML.Exceptions;

/// <summary>Base type for every exception thrown by this SDK. Catch this to handle them all.</summary>
public class VoiceMLException : Exception
{
    /// <summary>Construct with a message only.</summary>
    public VoiceMLException(string message) : base(message) { }

    /// <summary>Construct with a message and inner exception.</summary>
    public VoiceMLException(string message, Exception inner) : base(message, inner) { }
}
