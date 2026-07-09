using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace VoiceML.Models;

// ===========================================================================
// Pricing v1/v2 (pricing.twilio.com)
// VoiceML has no dedicated pricing subdomain, so these live on the default host
// (voiceml.voicetel.com) under /v1 and /v2. All operations are read-only GETs.
// VoiceML is NANP-only: every Countries list carries exactly one entry (the
// tenant's own country), and a Numbers fetch 404s for a non-NANP destination.
// All fields are nullable/permissive.
// ===========================================================================

// ---- Price leaves ---------------------------------------------------------

/// <summary>An inbound price leaf keyed by number type (<c>local</c> / <c>toll free</c>).</summary>
public sealed record PricingInboundCallPrice
{
    [JsonPropertyName("base_price")] public string? BasePrice { get; init; }
    [JsonPropertyName("current_price")] public string? CurrentPrice { get; init; }
    [JsonPropertyName("number_type")] public string? NumberType { get; init; }
}

public sealed record PricingOutboundCallPrice
{
    [JsonPropertyName("base_price")] public string? BasePrice { get; init; }
    [JsonPropertyName("current_price")] public string? CurrentPrice { get; init; }
}

public sealed record PricingOutboundCallPriceWithOrigin
{
    [JsonPropertyName("origination_prefixes")] public List<string> OriginationPrefixes { get; init; } = new();
    [JsonPropertyName("base_price")] public string? BasePrice { get; init; }
    [JsonPropertyName("current_price")] public string? CurrentPrice { get; init; }
}

public sealed record PricingOutboundPrefixPrice
{
    [JsonPropertyName("prefixes")] public List<string> Prefixes { get; init; } = new();
    [JsonPropertyName("base_price")] public string? BasePrice { get; init; }
    [JsonPropertyName("current_price")] public string? CurrentPrice { get; init; }
    [JsonPropertyName("friendly_name")] public string? FriendlyName { get; init; }
}

public sealed record PricingOutboundPrefixPriceWithOrigin
{
    [JsonPropertyName("origination_prefixes")] public List<string> OriginationPrefixes { get; init; } = new();
    [JsonPropertyName("destination_prefixes")] public List<string> DestinationPrefixes { get; init; } = new();
    [JsonPropertyName("base_price")] public string? BasePrice { get; init; }
    [JsonPropertyName("current_price")] public string? CurrentPrice { get; init; }
    [JsonPropertyName("friendly_name")] public string? FriendlyName { get; init; }
}

public sealed record PricingOutboundSMSPrice
{
    [JsonPropertyName("carrier")] public string? Carrier { get; init; }
    [JsonPropertyName("mcc")] public string? Mcc { get; init; }
    [JsonPropertyName("mnc")] public string? Mnc { get; init; }
    [JsonPropertyName("prices")] public List<PricingInboundCallPrice> Prices { get; init; } = new();
}

public sealed record PricingPhoneNumberPrice
{
    [JsonPropertyName("number_type")] public string? NumberType { get; init; }
    [JsonPropertyName("base_price")] public string? BasePrice { get; init; }
    [JsonPropertyName("current_price")] public string? CurrentPrice { get; init; }
}

// ---- Countries list envelope ----------------------------------------------

public sealed record PricingCountryRef
{
    [JsonPropertyName("country")] public string? Country { get; init; }
    [JsonPropertyName("iso_country")] public string? IsoCountry { get; init; }
    [JsonPropertyName("url")] public string? Url { get; init; }
}

/// <summary>Shared envelope returned by every <c>Countries.ListAsync</c>.</summary>
public sealed record PricingCountriesList
{
    [JsonPropertyName("countries")] public List<PricingCountryRef> Countries { get; init; } = new();
    [JsonPropertyName("meta")] public VoiceV1Meta? Meta { get; init; }
}

// ---- Pricing v1 country / number bodies -----------------------------------

public sealed record PricingVoiceCountry
{
    [JsonPropertyName("country")] public string? Country { get; init; }
    [JsonPropertyName("iso_country")] public string? IsoCountry { get; init; }
    [JsonPropertyName("outbound_prefix_prices")] public List<PricingOutboundPrefixPrice> OutboundPrefixPrices { get; init; } = new();
    [JsonPropertyName("inbound_call_prices")] public List<PricingInboundCallPrice> InboundCallPrices { get; init; } = new();
    [JsonPropertyName("price_unit")] public string? PriceUnit { get; init; }
    [JsonPropertyName("url")] public string? Url { get; init; }
}

public sealed record PricingVoiceNumber
{
    [JsonPropertyName("number")] public string? Number { get; init; }
    [JsonPropertyName("country")] public string? Country { get; init; }
    [JsonPropertyName("iso_country")] public string? IsoCountry { get; init; }
    [JsonPropertyName("outbound_call_price")] public PricingOutboundCallPrice? OutboundCallPrice { get; init; }
    [JsonPropertyName("inbound_call_price")] public PricingInboundCallPrice? InboundCallPrice { get; init; }
    [JsonPropertyName("price_unit")] public string? PriceUnit { get; init; }
    [JsonPropertyName("url")] public string? Url { get; init; }
}

public sealed record PricingMessagingCountry
{
    [JsonPropertyName("country")] public string? Country { get; init; }
    [JsonPropertyName("iso_country")] public string? IsoCountry { get; init; }
    [JsonPropertyName("outbound_sms_prices")] public List<PricingOutboundSMSPrice> OutboundSmsPrices { get; init; } = new();
    [JsonPropertyName("inbound_sms_prices")] public List<PricingInboundCallPrice> InboundSmsPrices { get; init; } = new();
    [JsonPropertyName("price_unit")] public string? PriceUnit { get; init; }
    [JsonPropertyName("url")] public string? Url { get; init; }
}

public sealed record PricingPhoneNumberCountry
{
    [JsonPropertyName("country")] public string? Country { get; init; }
    [JsonPropertyName("iso_country")] public string? IsoCountry { get; init; }
    [JsonPropertyName("phone_number_prices")] public List<PricingPhoneNumberPrice> PhoneNumberPrices { get; init; } = new();
    [JsonPropertyName("price_unit")] public string? PriceUnit { get; init; }
    [JsonPropertyName("url")] public string? Url { get; init; }
}

// ---- Pricing v2 country / number bodies -----------------------------------

public sealed record PricingVoiceCountryV2
{
    [JsonPropertyName("country")] public string? Country { get; init; }
    [JsonPropertyName("iso_country")] public string? IsoCountry { get; init; }
    [JsonPropertyName("outbound_prefix_prices")] public List<PricingOutboundPrefixPriceWithOrigin> OutboundPrefixPrices { get; init; } = new();
    [JsonPropertyName("inbound_call_prices")] public List<PricingInboundCallPrice> InboundCallPrices { get; init; } = new();
    [JsonPropertyName("price_unit")] public string? PriceUnit { get; init; }
    [JsonPropertyName("url")] public string? Url { get; init; }
}

public sealed record PricingVoiceNumberV2
{
    [JsonPropertyName("destination_number")] public string? DestinationNumber { get; init; }
    [JsonPropertyName("origination_number")] public string? OriginationNumber { get; init; }
    [JsonPropertyName("country")] public string? Country { get; init; }
    [JsonPropertyName("iso_country")] public string? IsoCountry { get; init; }
    [JsonPropertyName("outbound_call_prices")] public List<PricingOutboundCallPriceWithOrigin> OutboundCallPrices { get; init; } = new();
    [JsonPropertyName("inbound_call_price")] public PricingInboundCallPrice? InboundCallPrice { get; init; }
    [JsonPropertyName("price_unit")] public string? PriceUnit { get; init; }
    [JsonPropertyName("url")] public string? Url { get; init; }
}

public sealed record PricingTrunkingCountry
{
    [JsonPropertyName("country")] public string? Country { get; init; }
    [JsonPropertyName("iso_country")] public string? IsoCountry { get; init; }
    [JsonPropertyName("terminating_prefix_prices")] public List<PricingOutboundPrefixPriceWithOrigin> TerminatingPrefixPrices { get; init; } = new();
    [JsonPropertyName("originating_call_prices")] public List<PricingInboundCallPrice> OriginatingCallPrices { get; init; } = new();
    [JsonPropertyName("price_unit")] public string? PriceUnit { get; init; }
    [JsonPropertyName("url")] public string? Url { get; init; }
}

public sealed record PricingTrunkingNumber
{
    [JsonPropertyName("destination_number")] public string? DestinationNumber { get; init; }
    [JsonPropertyName("origination_number")] public string? OriginationNumber { get; init; }
    [JsonPropertyName("country")] public string? Country { get; init; }
    [JsonPropertyName("iso_country")] public string? IsoCountry { get; init; }
    [JsonPropertyName("terminating_prefix_prices")] public List<PricingOutboundPrefixPriceWithOrigin> TerminatingPrefixPrices { get; init; } = new();
    [JsonPropertyName("originating_call_price")] public PricingInboundCallPrice? OriginatingCallPrice { get; init; }
    [JsonPropertyName("price_unit")] public string? PriceUnit { get; init; }
    [JsonPropertyName("url")] public string? Url { get; init; }
}
