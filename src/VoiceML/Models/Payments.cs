using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace VoiceML.Models;

/// <summary>REST companion to the <c>&lt;Pay&gt;</c> TwiML verb. The response shape mirrors
/// Twilio's deliberately-minimal payload — runtime config (<c>ChargeAmount</c>,
/// <c>PaymentConnector</c>, <c>ValidCardTypes</c>, etc.) is captured server-side and not echoed
/// back. Tenant-side BYO is binding: the account must have <c>pay_enabled = true</c> AND a
/// <c>stripe_secret_key</c> configured, or the call fails 403.</summary>
public sealed record CallPayment
{
    /// <summary>Payment SID (<c>PY</c> + 32 hex).</summary>
    [JsonPropertyName("sid")] public string Sid { get; init; } = "";

    /// <summary>Owning Account SID.</summary>
    [JsonPropertyName("account_sid")] public string AccountSid { get; init; } = "";

    /// <summary>The Call SID this payment session is bound to.</summary>
    [JsonPropertyName("call_sid")] public string CallSid { get; init; } = "";

    /// <summary>Twilio API version label.</summary>
    [JsonPropertyName("api_version")] public string ApiVersion { get; init; } = "";

    /// <summary>RFC 2822 creation timestamp.</summary>
    [JsonPropertyName("date_created")] public string DateCreated { get; init; } = "";

    /// <summary>RFC 2822 last-modification timestamp.</summary>
    [JsonPropertyName("date_updated")] public string DateUpdated { get; init; } = "";

    /// <summary>URI of this CallPayment resource.</summary>
    [JsonPropertyName("uri")] public string Uri { get; init; } = "";
}

/// <summary>String constants for the <c>BankAccountType</c> field on a <c>&lt;Pay&gt;</c> session.</summary>
public static class PaymentBankAccountType
{
    /// <summary>Personal checking account.</summary>
    public const string ConsumerChecking = "consumer-checking";

    /// <summary>Personal savings account.</summary>
    public const string ConsumerSavings = "consumer-savings";

    /// <summary>Business checking account.</summary>
    public const string CommercialChecking = "commercial-checking";
}

/// <summary>String constants for the <c>Input</c> field on a <c>&lt;Pay&gt;</c> session.
/// DTMF is the only supported input mode today.</summary>
public static class PaymentInput
{
    /// <summary>Dual-tone multi-frequency keypad input.</summary>
    public const string Dtmf = "dtmf";
}

/// <summary>String constants for the <c>PaymentMethod</c> field on a <c>&lt;Pay&gt;</c> session.</summary>
public static class PaymentMethod
{
    /// <summary>Credit card collection flow.</summary>
    public const string CreditCard = "credit-card";

    /// <summary>ACH debit collection flow.</summary>
    public const string AchDebit = "ach-debit";
}

/// <summary>String constants for the <c>TokenType</c> field on a <c>&lt;Pay&gt;</c> session.</summary>
public static class PaymentTokenType
{
    /// <summary>One-time-use token.</summary>
    public const string OneTime = "one-time";

    /// <summary>Reusable token for repeat charges.</summary>
    public const string Reusable = "reusable";

    /// <summary>Stripe-style PaymentMethod token.</summary>
    public const string PaymentMethod = "payment-method";
}

/// <summary>String constants for the <c>Capture</c> field on Pay-session updates — tells the
/// runtime which input the caller is about to type next.</summary>
public static class PaymentCapture
{
    /// <summary>Capture the payment card number.</summary>
    public const string PaymentCardNumber = "payment-card-number";

    /// <summary>Capture the card expiration date.</summary>
    public const string ExpirationDate = "expiration-date";

    /// <summary>Capture the card security code (CVV/CVC).</summary>
    public const string SecurityCode = "security-code";

    /// <summary>Capture the cardholder's postal code.</summary>
    public const string PostalCode = "postal-code";

    /// <summary>Capture the bank routing number.</summary>
    public const string BankRoutingNumber = "bank-routing-number";

    /// <summary>Capture the bank account number.</summary>
    public const string BankAccountNumber = "bank-account-number";

    /// <summary>Re-capture the card number for matcher verification.</summary>
    public const string PaymentCardNumberMatcher = "payment-card-number-matcher";

    /// <summary>Re-capture the expiration date for matcher verification.</summary>
    public const string ExpirationDateMatcher = "expiration-date-matcher";

    /// <summary>Re-capture the security code for matcher verification.</summary>
    public const string SecurityCodeMatcher = "security-code-matcher";

    /// <summary>Re-capture the postal code for matcher verification.</summary>
    public const string PostalCodeMatcher = "postal-code-matcher";
}

/// <summary>String constants for the <c>Status</c> field on Pay-session updates —
/// terminates the session.</summary>
public static class PaymentSessionStatus
{
    /// <summary>Capture the collected fields and complete the session.</summary>
    public const string Complete = "complete";

    /// <summary>Abort the session without capturing.</summary>
    public const string Cancel = "cancel";
}

/// <summary>Body for <c>POST /Calls/{CallSid}/Payments</c>. Every attribute the <c>&lt;Pay&gt;</c>
/// TwiML verb accepts has a counterpart here. <see cref="IdempotencyKey"/> is accepted and
/// persisted for diagnostic visibility but replay-dedup is NOT enforced today.</summary>
public sealed record StartPaymentRequest : IFormSerializable
{
    /// <summary>Body-level dedup token. Persisted; replay-dedup not enforced.</summary>
    public string? IdempotencyKey { get; init; }

    /// <summary>Status-callback URL.</summary>
    public string? StatusCallback { get; init; }

    /// <summary>Bank account type. See <see cref="VoiceML.Models.PaymentBankAccountType"/>.</summary>
    public string? BankAccountType { get; init; }

    /// <summary>Charge amount as a decimal string (under 1,000,000).</summary>
    public string? ChargeAmount { get; init; }

    /// <summary>Currency (default <c>USD</c>).</summary>
    public string? Currency { get; init; }

    /// <summary>Free-form description (shown to the caller / persisted on the charge).</summary>
    public string? Description { get; init; }

    /// <summary>Input mode. See <see cref="VoiceML.Models.PaymentInput"/>.</summary>
    public string? Input { get; init; }

    /// <summary>Minimum postal code length (default <c>0</c>).</summary>
    public int? MinPostalCodeLength { get; init; }

    /// <summary>Single-level JSON object passed verbatim to the payment connector.</summary>
    public string? Parameter { get; init; }

    /// <summary>Payment connector name (default <c>Default</c>).</summary>
    public string? PaymentConnector { get; init; }

    /// <summary>Payment method. See <see cref="VoiceML.Models.PaymentMethod"/>.</summary>
    public string? PaymentMethod { get; init; }

    /// <summary>Whether to collect a postal code (default <c>true</c>).</summary>
    public bool? PostalCode { get; init; }

    /// <summary>Whether to collect a security code / CVV (default <c>true</c>).</summary>
    public bool? SecurityCode { get; init; }

    /// <summary>Per-field timeout in seconds (default <c>5</c>).</summary>
    public int? Timeout { get; init; }

    /// <summary>Token type. See <see cref="VoiceML.Models.PaymentTokenType"/>.</summary>
    public string? TokenType { get; init; }

    /// <summary>Space-separated list of accepted card brands.</summary>
    public string? ValidCardTypes { get; init; }

    /// <summary>Comma-separated list of fields that require matcher inputs.</summary>
    public string? RequireMatchingInputs { get; init; }

    /// <summary>Require an explicit caller confirmation before capture.</summary>
    public bool? Confirmation { get; init; }

    /// <inheritdoc/>
    public IEnumerable<KeyValuePair<string, string?>> ToForm()
    {
        yield return new("IdempotencyKey", IdempotencyKey);
        yield return new("StatusCallback", StatusCallback);
        yield return new("BankAccountType", BankAccountType);
        yield return new("ChargeAmount", ChargeAmount);
        yield return new("Currency", Currency);
        yield return new("Description", Description);
        yield return new("Input", Input);
        yield return new("MinPostalCodeLength", MinPostalCodeLength?.ToString());
        yield return new("Parameter", Parameter);
        yield return new("PaymentConnector", PaymentConnector);
        yield return new("PaymentMethod", PaymentMethod);
        yield return new("PostalCode", FormHelpers.BoolStr(PostalCode));
        yield return new("SecurityCode", FormHelpers.BoolStr(SecurityCode));
        yield return new("Timeout", Timeout?.ToString());
        yield return new("TokenType", TokenType);
        yield return new("ValidCardTypes", ValidCardTypes);
        yield return new("RequireMatchingInputs", RequireMatchingInputs);
        yield return new("Confirmation", FormHelpers.BoolStr(Confirmation));
    }
}

/// <summary>Body for <c>POST /Calls/{CallSid}/Payments/{Sid}</c>. Either advance the session
/// (<see cref="Capture"/>) or terminate it (<see cref="Status"/> = <c>complete</c> or <c>cancel</c>).</summary>
public sealed record UpdatePaymentRequest : IFormSerializable
{
    /// <summary>Body-level dedup token. Persisted; replay-dedup not enforced.</summary>
    public string? IdempotencyKey { get; init; }

    /// <summary>Status-callback URL.</summary>
    public string? StatusCallback { get; init; }

    /// <summary>Capture stage. See <see cref="VoiceML.Models.PaymentCapture"/>.</summary>
    public string? Capture { get; init; }

    /// <summary>Session termination. See <see cref="VoiceML.Models.PaymentSessionStatus"/>.</summary>
    public string? Status { get; init; }

    /// <inheritdoc/>
    public IEnumerable<KeyValuePair<string, string?>> ToForm()
    {
        yield return new("IdempotencyKey", IdempotencyKey);
        yield return new("StatusCallback", StatusCallback);
        yield return new("Capture", Capture);
        yield return new("Status", Status);
    }
}
