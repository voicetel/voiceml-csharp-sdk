using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace VoiceML.Models;

/// <summary>A Twilio-compatible Account resource (<c>AC</c> + 32 hex) — the authenticated
/// account itself. The server additionally embeds VoiceML extension keys
/// (<c>name</c>, <c>voice_url</c>, <c>tts_*</c>, <c>stt_*</c>, <c>aws_*</c>, <c>allow_cidr</c>,
/// <c>created_at</c>/<c>updated_at</c>) after the canonical fields; the account secret is
/// never emitted on this surface.</summary>
public sealed record Account
{
    /// <summary>Account SID (<c>AC</c> + 32 hex). Echoes the AccountSid of the credentials in use.</summary>
    [JsonPropertyName("sid")] public string? Sid { get; init; }

    /// <summary>Parent (owner) Account SID — same as <see cref="Sid"/> for top-level accounts.</summary>
    [JsonPropertyName("owner_account_sid")] public string? OwnerAccountSid { get; init; }

    /// <summary>Friendly name. Mutable via update.</summary>
    [JsonPropertyName("friendly_name")] public string? FriendlyName { get; init; }

    /// <summary>Account status: <c>active</c>, <c>suspended</c>, or <c>closed</c>.</summary>
    [JsonPropertyName("status")] public string Status { get; init; } = "";

    /// <summary>Account type: <c>Trial</c> or <c>Full</c>.</summary>
    [JsonPropertyName("type")] public string Type { get; init; } = "";

    /// <summary>Display name (VoiceML extension).</summary>
    [JsonPropertyName("name")] public string? Name { get; init; }

    /// <summary>Default voice webhook URL for the account (VoiceML extension).</summary>
    [JsonPropertyName("voice_url")] public string? VoiceUrl { get; init; }

    /// <summary>BYO speech-to-text vendor name (VoiceML extension).</summary>
    [JsonPropertyName("stt_fs_vendor")] public string? SttFsVendor { get; init; }

    /// <summary>BYO speech-to-text HTTP endpoint (VoiceML extension).</summary>
    [JsonPropertyName("stt_http_url")] public string? SttHttpUrl { get; init; }

    /// <summary>BYO text-to-speech HTTP endpoint (VoiceML extension).</summary>
    [JsonPropertyName("tts_http_url")] public string? TtsHttpUrl { get; init; }

    /// <summary>BYO text-to-speech engine name (VoiceML extension).</summary>
    [JsonPropertyName("tts_engine_name")] public string? TtsEngineName { get; init; }

    /// <summary>Default TTS voice (VoiceML extension).</summary>
    [JsonPropertyName("tts_default_voice")] public string? TtsDefaultVoice { get; init; }

    /// <summary>Default TTS language tag, e.g. <c>en-US</c> (VoiceML extension).</summary>
    [JsonPropertyName("tts_default_language")] public string? TtsDefaultLanguage { get; init; }

    /// <summary>S3 bucket for recording storage (VoiceML extension).</summary>
    [JsonPropertyName("aws_bucket")] public string? AwsBucket { get; init; }

    /// <summary>AWS region for the recording bucket (VoiceML extension).</summary>
    [JsonPropertyName("aws_region")] public string? AwsRegion { get; init; }

    /// <summary>Key prefix within the recording bucket (VoiceML extension).</summary>
    [JsonPropertyName("aws_prefix")] public string? AwsPrefix { get; init; }

    /// <summary>IP allow-list, one CIDR prefix per entry (VoiceML extension). An empty list
    /// allows all source addresses.</summary>
    [JsonPropertyName("allow_cidr")] public List<string>? AllowCidr { get; init; }

    /// <summary>RFC 3339 creation timestamp (VoiceML extension).</summary>
    [JsonPropertyName("created_at")] public string? CreatedAt { get; init; }

    /// <summary>RFC 3339 last-modification timestamp (VoiceML extension).</summary>
    [JsonPropertyName("updated_at")] public string? UpdatedAt { get; init; }

    /// <summary>RFC 2822 creation timestamp (canonical field).</summary>
    [JsonPropertyName("date_created")] public string? DateCreated { get; init; }

    /// <summary>RFC 2822 last-modification timestamp (canonical field).</summary>
    [JsonPropertyName("date_updated")] public string? DateUpdated { get; init; }

    /// <summary>URI of this Account resource.</summary>
    [JsonPropertyName("uri")] public string? Uri { get; init; }

    /// <summary>Map of sub-resource URIs (Calls, Conferences, Messages, etc.).</summary>
    [JsonPropertyName("subresource_uris")] public Dictionary<string, string>? SubresourceUris { get; init; }
}

/// <summary>The account configuration view plus the freshly generated <c>api_key</c>, returned
/// by <c>POST /2010-04-01/Accounts/{AccountSid}/RotateKey.json</c>. No read endpoint exposes
/// the key, so store it as soon as rotation returns — requests using the previous key fail
/// with 401 from that point on. The <see cref="ApiKey"/> is redacted from the record's
/// default string rendering so logging the response never leaks it.</summary>
public sealed record AccountWithKey
{
    /// <summary>The account's <c>AC</c> SID.</summary>
    [JsonPropertyName("sid")] public string Sid { get; init; } = "";

    /// <summary>The new API key returned by rotation. Secret — store it now; it is redacted
    /// from default string output and must not be logged or committed.</summary>
    [JsonPropertyName("api_key")] public string ApiKey { get; init; } = "";

    /// <summary>SIP username for the account's SIP access.</summary>
    [JsonPropertyName("sip_username")] public string SipUsername { get; init; } = "";

    /// <summary>Display name, when set.</summary>
    [JsonPropertyName("name")] public string? Name { get; init; }

    /// <summary>Allowed calling regions.</summary>
    [JsonPropertyName("phone_regions")] public List<string>? PhoneRegions { get; init; }

    /// <summary>IP allow-list, one CIDR prefix per entry. An empty list allows all source
    /// addresses.</summary>
    [JsonPropertyName("allow_cidr")] public List<string>? AllowCidr { get; init; }

    /// <summary>Whether the account is disabled.</summary>
    [JsonPropertyName("disabled")] public bool Disabled { get; init; }

    /// <summary>BYO speech-to-text FreeSWITCH vendor name, when set.</summary>
    [JsonPropertyName("stt_fs_vendor")] public string? SttFsVendor { get; init; }

    /// <summary>BYO speech-to-text HTTP endpoint, when set.</summary>
    [JsonPropertyName("stt_http_url")] public string? SttHttpUrl { get; init; }

    /// <summary>Whether a BYO speech-to-text bearer token is configured. The token itself is
    /// never returned.</summary>
    [JsonPropertyName("stt_http_token_set")] public bool SttHttpTokenSet { get; init; }

    /// <summary>BYO text-to-speech HTTP endpoint, when set.</summary>
    [JsonPropertyName("tts_http_url")] public string? TtsHttpUrl { get; init; }

    /// <summary>Whether a BYO text-to-speech bearer token is configured. The token itself is
    /// never returned.</summary>
    [JsonPropertyName("tts_http_token_set")] public bool TtsHttpTokenSet { get; init; }

    /// <summary>BYO text-to-speech engine name, when set.</summary>
    [JsonPropertyName("tts_engine_name")] public string? TtsEngineName { get; init; }

    /// <summary>Default TTS voice, when set.</summary>
    [JsonPropertyName("tts_default_voice")] public string? TtsDefaultVoice { get; init; }

    /// <summary>Default TTS language tag, e.g. <c>en-US</c>, when set.</summary>
    [JsonPropertyName("tts_default_language")] public string? TtsDefaultLanguage { get; init; }

    /// <summary>Whether a Stripe secret key is configured. The key itself is never
    /// returned.</summary>
    [JsonPropertyName("stripe_secret_key_set")] public bool StripeSecretKeySet { get; init; }

    /// <summary>Whether payments are enabled for the account.</summary>
    [JsonPropertyName("pay_enabled")] public bool PayEnabled { get; init; }

    /// <summary>Configured SMS provider, when set.</summary>
    [JsonPropertyName("sms_provider")] public string? SmsProvider { get; init; }

    /// <summary>Allowed SMS providers.</summary>
    [JsonPropertyName("sms_allowed_providers")] public List<string>? SmsAllowedProviders { get; init; }

    /// <summary>Allowed speech-to-text providers.</summary>
    [JsonPropertyName("stt_allowed_providers")] public List<string>? SttAllowedProviders { get; init; }

    /// <summary>Configured speech-to-text provider, when set.</summary>
    [JsonPropertyName("stt_provider")] public string? SttProvider { get; init; }

    /// <summary>Default voice webhook URL for the account, when set.</summary>
    [JsonPropertyName("voice_url")] public string? VoiceUrl { get; init; }

    /// <summary>HTTP method for <see cref="VoiceUrl"/>, when set.</summary>
    [JsonPropertyName("voice_method")] public string? VoiceMethod { get; init; }

    /// <summary>Fallback voice webhook URL, when set.</summary>
    [JsonPropertyName("voice_fallback_url")] public string? VoiceFallbackUrl { get; init; }

    /// <summary>HTTP method for <see cref="VoiceFallbackUrl"/>, when set.</summary>
    [JsonPropertyName("voice_fallback_method")] public string? VoiceFallbackMethod { get; init; }

    /// <summary>S3 bucket for recording storage, when set.</summary>
    [JsonPropertyName("aws_bucket")] public string? AwsBucket { get; init; }

    /// <summary>AWS region for the recording bucket, when set.</summary>
    [JsonPropertyName("aws_region")] public string? AwsRegion { get; init; }

    /// <summary>Key prefix within the recording bucket, when set.</summary>
    [JsonPropertyName("aws_prefix")] public string? AwsPrefix { get; init; }

    /// <summary>AWS access key ID for recording storage, when set.</summary>
    [JsonPropertyName("aws_access_key_id")] public string? AwsAccessKeyId { get; init; }

    /// <summary>Whether an AWS secret access key is configured. The key itself is never
    /// returned.</summary>
    [JsonPropertyName("aws_secret_access_key_set")] public bool AwsSecretAccessKeySet { get; init; }

    /// <summary>AWS KMS key ID for server-side encryption, when set.</summary>
    [JsonPropertyName("aws_kms_key_id")] public string? AwsKmsKeyId { get; init; }

    /// <summary>RFC 3339 creation timestamp.</summary>
    [JsonPropertyName("created_at")] public string CreatedAt { get; init; } = "";

    /// <summary>RFC 3339 last-modification timestamp.</summary>
    [JsonPropertyName("updated_at")] public string UpdatedAt { get; init; } = "";

    /// <summary>Render the account with the API key redacted. Replaces the
    /// compiler-generated record member, which would otherwise print the secret.</summary>
    private bool PrintMembers(System.Text.StringBuilder builder)
    {
        builder.Append("Sid = ").Append(Sid);
        builder.Append(", ApiKey = ").Append(ApiKey.Length == 0 ? "" : "[REDACTED]");
        builder.Append(", SipUsername = ").Append(SipUsername);
        builder.Append(", Name = ").Append(Name ?? "null");
        builder.Append(", PhoneRegions = ").Append(PhoneRegions is null ? "null" : string.Join(", ", PhoneRegions));
        builder.Append(", AllowCidr = ").Append(AllowCidr is null ? "null" : string.Join(", ", AllowCidr));
        builder.Append(", Disabled = ").Append(Disabled);
        builder.Append(", SttFsVendor = ").Append(SttFsVendor ?? "null");
        builder.Append(", SttHttpUrl = ").Append(SttHttpUrl ?? "null");
        builder.Append(", SttHttpTokenSet = ").Append(SttHttpTokenSet);
        builder.Append(", TtsHttpUrl = ").Append(TtsHttpUrl ?? "null");
        builder.Append(", TtsHttpTokenSet = ").Append(TtsHttpTokenSet);
        builder.Append(", TtsEngineName = ").Append(TtsEngineName ?? "null");
        builder.Append(", TtsDefaultVoice = ").Append(TtsDefaultVoice ?? "null");
        builder.Append(", TtsDefaultLanguage = ").Append(TtsDefaultLanguage ?? "null");
        builder.Append(", StripeSecretKeySet = ").Append(StripeSecretKeySet);
        builder.Append(", PayEnabled = ").Append(PayEnabled);
        builder.Append(", SmsProvider = ").Append(SmsProvider ?? "null");
        builder.Append(", SmsAllowedProviders = ").Append(SmsAllowedProviders is null ? "null" : string.Join(", ", SmsAllowedProviders));
        builder.Append(", SttAllowedProviders = ").Append(SttAllowedProviders is null ? "null" : string.Join(", ", SttAllowedProviders));
        builder.Append(", SttProvider = ").Append(SttProvider ?? "null");
        builder.Append(", VoiceUrl = ").Append(VoiceUrl ?? "null");
        builder.Append(", VoiceMethod = ").Append(VoiceMethod ?? "null");
        builder.Append(", VoiceFallbackUrl = ").Append(VoiceFallbackUrl ?? "null");
        builder.Append(", VoiceFallbackMethod = ").Append(VoiceFallbackMethod ?? "null");
        builder.Append(", AwsBucket = ").Append(AwsBucket ?? "null");
        builder.Append(", AwsRegion = ").Append(AwsRegion ?? "null");
        builder.Append(", AwsPrefix = ").Append(AwsPrefix ?? "null");
        builder.Append(", AwsAccessKeyId = ").Append(AwsAccessKeyId ?? "null");
        builder.Append(", AwsSecretAccessKeySet = ").Append(AwsSecretAccessKeySet);
        builder.Append(", AwsKmsKeyId = ").Append(AwsKmsKeyId ?? "null");
        builder.Append(", CreatedAt = ").Append(CreatedAt);
        builder.Append(", UpdatedAt = ").Append(UpdatedAt);
        return true;
    }
}

/// <summary>Body for <c>POST /Accounts/{Sid}</c> — update the authenticated account. Only the
/// fields present in the request are changed. The server rejects a stale update with 409 /
/// error 20409 (optimistic-locking conflict) — refresh the resource and retry; the SDK
/// surfaces that as <see cref="VoiceML.Exceptions.ConflictException"/>.</summary>
public sealed record UpdateAccountRequest : IFormSerializable
{
    /// <summary>Human-readable account name.</summary>
    public string? FriendlyName { get; init; }

    /// <summary>Default voice webhook URL for the account. No web server needed — host your
    /// VoiceML webhook handling wherever this points.</summary>
    public string? VoiceUrl { get; init; }

    /// <summary>HTTP method for <see cref="VoiceUrl"/> (<c>GET</c> or <c>POST</c>).</summary>
    public string? VoiceMethod { get; init; }

    /// <summary>Fallback voice webhook URL.</summary>
    public string? VoiceFallbackUrl { get; init; }

    /// <summary>HTTP method for <see cref="VoiceFallbackUrl"/>.</summary>
    public string? VoiceFallbackMethod { get; init; }

    /// <summary>BYO speech-to-text FreeSWITCH vendor name.</summary>
    public string? SttFsVendor { get; init; }

    /// <summary>BYO speech-to-text HTTP endpoint.</summary>
    public string? SttHttpUrl { get; init; }

    /// <summary>BYO speech-to-text bearer token. Secret — never echoed back on fetch.</summary>
    public string? SttHttpToken { get; init; }

    /// <summary>BYO text-to-speech HTTP endpoint.</summary>
    public string? TtsHttpUrl { get; init; }

    /// <summary>BYO text-to-speech bearer token. Secret — never echoed back on fetch.</summary>
    public string? TtsHttpToken { get; init; }

    /// <summary>BYO text-to-speech engine name.</summary>
    public string? TtsEngineName { get; init; }

    /// <summary>Default TTS voice.</summary>
    public string? TtsDefaultVoice { get; init; }

    /// <summary>Default TTS language tag (e.g. <c>en-US</c>).</summary>
    public string? TtsDefaultLanguage { get; init; }

    /// <summary>S3 bucket for recording storage.</summary>
    public string? AwsBucket { get; init; }

    /// <summary>AWS region for the recording bucket.</summary>
    public string? AwsRegion { get; init; }

    /// <summary>Key prefix within the recording bucket.</summary>
    public string? AwsPrefix { get; init; }

    /// <summary>AWS access key ID for recording storage. Secret — never echoed back on fetch.</summary>
    public string? AwsAccessKeyId { get; init; }

    /// <summary>AWS secret access key for recording storage. Secret — never echoed back on fetch.</summary>
    public string? AwsSecretAccessKey { get; init; }

    /// <summary>AWS KMS key ID for server-side encryption.</summary>
    public string? AwsKmsKeyId { get; init; }

    /// <summary>IP allow-list, one CIDR prefix per entry (repeated form parameter). An empty
    /// list allows all source addresses.</summary>
    public List<string>? AllowCidr { get; init; }

    /// <inheritdoc/>
    public IEnumerable<KeyValuePair<string, string?>> ToForm()
    {
        yield return new("FriendlyName", FriendlyName);
        yield return new("VoiceUrl", VoiceUrl);
        yield return new("VoiceMethod", VoiceMethod);
        yield return new("VoiceFallbackUrl", VoiceFallbackUrl);
        yield return new("VoiceFallbackMethod", VoiceFallbackMethod);
        yield return new("SttFsVendor", SttFsVendor);
        yield return new("SttHttpUrl", SttHttpUrl);
        yield return new("SttHttpToken", SttHttpToken);
        yield return new("TtsHttpUrl", TtsHttpUrl);
        yield return new("TtsHttpToken", TtsHttpToken);
        yield return new("TtsEngineName", TtsEngineName);
        yield return new("TtsDefaultVoice", TtsDefaultVoice);
        yield return new("TtsDefaultLanguage", TtsDefaultLanguage);
        yield return new("AwsBucket", AwsBucket);
        yield return new("AwsRegion", AwsRegion);
        yield return new("AwsPrefix", AwsPrefix);
        yield return new("AwsAccessKeyId", AwsAccessKeyId);
        yield return new("AwsSecretAccessKey", AwsSecretAccessKey);
        yield return new("AwsKmsKeyId", AwsKmsKeyId);
        if (AllowCidr is not null)
        {
            foreach (var cidr in AllowCidr)
            {
                yield return new("AllowCidr", cidr);
            }
        }
    }
}
