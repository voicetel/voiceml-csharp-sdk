using System.Text.Json.Serialization;

namespace VoiceML.Models;

/// <summary>Twilio-compatible Account balance — response shape for
/// <c>GET /2010-04-01/Accounts/{AccountSid}/Balance.json</c>. <see cref="BalanceAmount"/>
/// is decimal-as-string on the wire (Twilio's documented shape), preserved here
/// without numeric coercion so callers can decide rounding policy.</summary>
public sealed record Balance
{
    /// <summary>Owning Account SID.</summary>
    [JsonPropertyName("account_sid")] public string AccountSid { get; init; } = "";

    /// <summary>Balance as a decimal string (e.g. <c>"0.05"</c>). Property is
    /// <c>BalanceAmount</c> in C# to avoid colliding with the enclosing type name.</summary>
    [JsonPropertyName("balance")] public string BalanceAmount { get; init; } = "";

    /// <summary>Currency code (e.g. <c>USD</c>).</summary>
    [JsonPropertyName("currency")] public string? Currency { get; init; }
}
