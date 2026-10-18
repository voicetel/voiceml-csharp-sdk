using System;

namespace VoiceML.Exceptions;

/// <summary>The request exceeded the configured <see cref="ClientOptions.Timeout"/> and the SDK
/// stopped waiting. The underlying <see cref="OperationCanceledException"/> (raised by
/// <see cref="System.Net.Http.HttpClient.Timeout"/>) is preserved as the inner exception.
/// <para>Timeouts are retried only for idempotent requests (GET/HEAD/PUT/DELETE).</para></summary>
public sealed class VoiceMLTimeoutException : ApiException
{
    /// <summary>Construct with the timed-out request description and cause.</summary>
    public VoiceMLTimeoutException(string message, Exception inner) : base(message, 0, inner) { }
}
