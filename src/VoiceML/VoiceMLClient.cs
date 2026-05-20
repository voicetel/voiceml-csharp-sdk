using System;
using VoiceML.Resources;

namespace VoiceML;

/// <summary>The main VoiceML SDK client. Holds the <see cref="Transport"/> and exposes one
/// resource property per top-level Twilio surface.
/// <para>Usage:</para>
/// <code>
/// using var client = new VoiceMLClient(new ClientOptions {
///     AccountSid = "AC…",
///     ApiKey     = "…",
/// });
/// var call = await client.Calls.CreateAsync(new CreateCallRequest {
///     To = "+1…", From = "+1…", Url = "https://example.com/twiml.xml",
/// });
/// </code>
/// </summary>
public sealed class VoiceMLClient : IDisposable
{
    private readonly Transport _transport;

    /// <summary>Operations on calls and call-scoped sub-resources.</summary>
    public CallsResource Calls { get; }

    /// <summary>Operations on conferences and participants.</summary>
    public ConferencesResource Conferences { get; }

    /// <summary>Operations on queues and queue members.</summary>
    public QueuesResource Queues { get; }

    /// <summary>Operations on application bundles.</summary>
    public ApplicationsResource Applications { get; }

    /// <summary>Account-scoped recording operations.</summary>
    public RecordingsResource Recordings { get; }

    /// <summary>Tenant-self-serve DID management.</summary>
    public IncomingPhoneNumbersResource IncomingPhoneNumbers { get; }

    /// <summary>Diagnostic endpoints: <c>/health</c> and <c>/openapi.json</c>.</summary>
    public DiagnosticsResource Diagnostics { get; }

    /// <summary>The configured AccountSid (from <see cref="ClientOptions.AccountSid"/>).</summary>
    public string AccountSid => _transport.AccountSid;

    /// <summary>The configured server base URL (no trailing slash).</summary>
    public string BaseUrl => _transport.BaseUrl;

    /// <summary>Construct the client from <see cref="ClientOptions"/>. <see cref="ClientOptions.Validate"/>
    /// is called inside the <see cref="Transport"/> constructor and will throw
    /// <see cref="VoiceML.Exceptions.ConfigurationException"/> on missing required fields.</summary>
    public VoiceMLClient(ClientOptions options)
    {
        _transport = new Transport(options);
        Calls = new CallsResource(_transport);
        Conferences = new ConferencesResource(_transport);
        Queues = new QueuesResource(_transport);
        Applications = new ApplicationsResource(_transport);
        Recordings = new RecordingsResource(_transport);
        IncomingPhoneNumbers = new IncomingPhoneNumbersResource(_transport);
        Diagnostics = new DiagnosticsResource(_transport);
    }

    /// <summary>Dispose the underlying transport. If the caller supplied their own
    /// <see cref="System.Net.Http.HttpClient"/> via <see cref="ClientOptions.HttpClient"/>,
    /// it is NOT disposed here — the caller retains lifetime ownership.</summary>
    public void Dispose()
    {
        _transport.Dispose();
    }
}
