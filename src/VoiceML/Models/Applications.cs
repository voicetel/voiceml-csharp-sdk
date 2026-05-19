using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace VoiceML.Models;

/// <summary>A Twilio-shape Application resource — a persistent TwiML+callback bundle
/// dispatched by <c>&lt;Dial&gt;&lt;Application&gt;</c>.</summary>
public sealed record Application
{
    /// <summary>Application SID (<c>AP</c> + 32 hex).</summary>
    [JsonPropertyName("sid")] public string Sid { get; init; } = "";

    /// <summary>Owning Account SID.</summary>
    [JsonPropertyName("account_sid")] public string AccountSid { get; init; } = "";

    /// <summary>Friendly name.</summary>
    [JsonPropertyName("friendly_name")] public string FriendlyName { get; init; } = "";

    /// <summary>Twilio API version label.</summary>
    [JsonPropertyName("api_version")] public string ApiVersion { get; init; } = "";

    /// <summary>Primary voice-URL.</summary>
    [JsonPropertyName("voice_url")] public string VoiceUrl { get; init; } = "";

    /// <summary>HTTP method for the voice URL.</summary>
    [JsonPropertyName("voice_method")] public string? VoiceMethod { get; init; }

    /// <summary>Voice fallback URL.</summary>
    [JsonPropertyName("voice_fallback_url")] public string? VoiceFallbackUrl { get; init; }

    /// <summary>HTTP method for the voice fallback URL.</summary>
    [JsonPropertyName("voice_fallback_method")] public string? VoiceFallbackMethod { get; init; }

    /// <summary>Whether caller-ID lookup is requested.</summary>
    [JsonPropertyName("voice_caller_id_lookup")] public bool VoiceCallerIdLookup { get; init; }

    /// <summary>Status-callback URL.</summary>
    [JsonPropertyName("status_callback")] public string? StatusCallback { get; init; }

    /// <summary>HTTP method for the status callback.</summary>
    [JsonPropertyName("status_callback_method")] public string? StatusCallbackMethod { get; init; }

    /// <summary>Comma- or space-delimited list of status-callback events.</summary>
    [JsonPropertyName("status_callback_event")] public string? StatusCallbackEvent { get; init; }

    /// <summary>RFC 3339 creation timestamp.</summary>
    [JsonPropertyName("date_created")] public string DateCreated { get; init; } = "";

    /// <summary>RFC 3339 last-modification timestamp.</summary>
    [JsonPropertyName("date_updated")] public string DateUpdated { get; init; } = "";

    /// <summary>URI of this application.</summary>
    [JsonPropertyName("uri")] public string Uri { get; init; } = "";
}

/// <summary>List response for <c>GET /Applications</c>.</summary>
public sealed record ApplicationList : Page
{
    /// <summary>The page of Application resources.</summary>
    [JsonPropertyName("applications")] public List<Application> Applications { get; init; } = new();
}

/// <summary>Body for <c>POST /Applications</c>. All fields optional per spec.</summary>
public sealed record CreateApplicationRequest : IFormSerializable
{
    /// <summary>Friendly name.</summary>
    public string? FriendlyName { get; init; }

    /// <summary>Voice URL.</summary>
    public string? VoiceUrl { get; init; }

    /// <summary>HTTP method for the voice URL.</summary>
    public string? VoiceMethod { get; init; }

    /// <summary>Voice fallback URL.</summary>
    public string? VoiceFallbackUrl { get; init; }

    /// <summary>HTTP method for the voice fallback URL.</summary>
    public string? VoiceFallbackMethod { get; init; }

    /// <summary>Caller-ID lookup flag.</summary>
    public bool? VoiceCallerIdLookup { get; init; }

    /// <summary>Status-callback URL.</summary>
    public string? StatusCallback { get; init; }

    /// <summary>HTTP method for the status callback.</summary>
    public string? StatusCallbackMethod { get; init; }

    /// <summary>Comma- or space-delimited list of status-callback events.</summary>
    public string? StatusCallbackEvent { get; init; }

    /// <inheritdoc/>
    public IEnumerable<KeyValuePair<string, string?>> ToForm()
    {
        yield return new("FriendlyName", FriendlyName);
        yield return new("VoiceUrl", VoiceUrl);
        yield return new("VoiceMethod", VoiceMethod);
        yield return new("VoiceFallbackUrl", VoiceFallbackUrl);
        yield return new("VoiceFallbackMethod", VoiceFallbackMethod);
        yield return new("VoiceCallerIdLookup", FormHelpers.BoolStr(VoiceCallerIdLookup));
        yield return new("StatusCallback", StatusCallback);
        yield return new("StatusCallbackMethod", StatusCallbackMethod);
        yield return new("StatusCallbackEvent", StatusCallbackEvent);
    }
}

/// <summary>Body for <c>POST /Applications/{sid}</c>. Partial — only set fields are touched.</summary>
public sealed record UpdateApplicationRequest : IFormSerializable
{
    /// <summary>Friendly name.</summary>
    public string? FriendlyName { get; init; }

    /// <summary>Voice URL.</summary>
    public string? VoiceUrl { get; init; }

    /// <summary>HTTP method for the voice URL.</summary>
    public string? VoiceMethod { get; init; }

    /// <summary>Voice fallback URL.</summary>
    public string? VoiceFallbackUrl { get; init; }

    /// <summary>HTTP method for the voice fallback URL.</summary>
    public string? VoiceFallbackMethod { get; init; }

    /// <summary>Caller-ID lookup flag.</summary>
    public bool? VoiceCallerIdLookup { get; init; }

    /// <summary>Status-callback URL.</summary>
    public string? StatusCallback { get; init; }

    /// <summary>HTTP method for the status callback.</summary>
    public string? StatusCallbackMethod { get; init; }

    /// <summary>Comma- or space-delimited list of status-callback events.</summary>
    public string? StatusCallbackEvent { get; init; }

    /// <inheritdoc/>
    public IEnumerable<KeyValuePair<string, string?>> ToForm()
    {
        yield return new("FriendlyName", FriendlyName);
        yield return new("VoiceUrl", VoiceUrl);
        yield return new("VoiceMethod", VoiceMethod);
        yield return new("VoiceFallbackUrl", VoiceFallbackUrl);
        yield return new("VoiceFallbackMethod", VoiceFallbackMethod);
        yield return new("VoiceCallerIdLookup", FormHelpers.BoolStr(VoiceCallerIdLookup));
        yield return new("StatusCallback", StatusCallback);
        yield return new("StatusCallbackMethod", StatusCallbackMethod);
        yield return new("StatusCallbackEvent", StatusCallbackEvent);
    }
}
