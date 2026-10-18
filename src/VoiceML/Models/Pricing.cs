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
    /// <summary>Standard rate before account-specific pricing, represented as a decimal string.</summary>
    [JsonPropertyName("base_price")] public string? BasePrice { get; init; }
    /// <summary>Effective account-specific rate, represented as a decimal string.</summary>
    [JsonPropertyName("current_price")] public string? CurrentPrice { get; init; }
    /// <summary>Number Type: local, toll free.</summary>
    [JsonPropertyName("number_type")] public string? NumberType { get; init; }
}

/// <summary>Outbound voice rate for one destination, with base and account-specific decimal prices.</summary>
public sealed record PricingOutboundCallPrice
{
    /// <summary>Standard rate before account-specific pricing, represented as a decimal string.</summary>
    [JsonPropertyName("base_price")] public string? BasePrice { get; init; }
    /// <summary>Effective account-specific rate, represented as a decimal string.</summary>
    [JsonPropertyName("current_price")] public string? CurrentPrice { get; init; }
}

/// <summary>Outbound voice rate qualified by both destination and origin.</summary>
public sealed record PricingOutboundCallPriceWithOrigin
{
    /// <summary>The returned origination prefixes in this page.</summary>
    [JsonPropertyName("origination_prefixes")] public List<string> OriginationPrefixes { get; init; } = new();
    /// <summary>Standard rate before account-specific pricing, represented as a decimal string.</summary>
    [JsonPropertyName("base_price")] public string? BasePrice { get; init; }
    /// <summary>Effective account-specific rate, represented as a decimal string.</summary>
    [JsonPropertyName("current_price")] public string? CurrentPrice { get; init; }
}

/// <summary>Outbound voice rate for a destination number prefix.</summary>
public sealed record PricingOutboundPrefixPrice
{
    /// <summary>The returned prefixes in this page.</summary>
    [JsonPropertyName("prefixes")] public List<string> Prefixes { get; init; } = new();
    /// <summary>Standard rate before account-specific pricing, represented as a decimal string.</summary>
    [JsonPropertyName("base_price")] public string? BasePrice { get; init; }
    /// <summary>Effective account-specific rate, represented as a decimal string.</summary>
    [JsonPropertyName("current_price")] public string? CurrentPrice { get; init; }
    /// <summary>Human-readable name used to identify this resource.</summary>
    [JsonPropertyName("friendly_name")] public string? FriendlyName { get; init; }
}

/// <summary>Outbound prefix rate qualified by the originating number prefix.</summary>
public sealed record PricingOutboundPrefixPriceWithOrigin
{
    /// <summary>The returned origination prefixes in this page.</summary>
    [JsonPropertyName("origination_prefixes")] public List<string> OriginationPrefixes { get; init; } = new();
    /// <summary>The returned destination prefixes in this page.</summary>
    [JsonPropertyName("destination_prefixes")] public List<string> DestinationPrefixes { get; init; } = new();
    /// <summary>Standard rate before account-specific pricing, represented as a decimal string.</summary>
    [JsonPropertyName("base_price")] public string? BasePrice { get; init; }
    /// <summary>Effective account-specific rate, represented as a decimal string.</summary>
    [JsonPropertyName("current_price")] public string? CurrentPrice { get; init; }
    /// <summary>Human-readable name used to identify this resource.</summary>
    [JsonPropertyName("friendly_name")] public string? FriendlyName { get; init; }
}

/// <summary>Outbound messaging rate qualified by destination carrier and mobile network codes.</summary>
public sealed record PricingOutboundSMSPrice
{
    /// <summary>Carrier name associated with the quoted messaging rate.</summary>
    [JsonPropertyName("carrier")] public string? Carrier { get; init; }
    /// <summary>Mobile country code used to identify the destination carrier network.</summary>
    [JsonPropertyName("mcc")] public string? Mcc { get; init; }
    /// <summary>Mobile network code identifying the carrier within its mobile country code.</summary>
    [JsonPropertyName("mnc")] public string? Mnc { get; init; }
    /// <summary>The returned prices in this page.</summary>
    [JsonPropertyName("prices")] public List<PricingInboundCallPrice> Prices { get; init; } = new();
}

/// <summary>Rate for a telephone-number class in a country, with base and current prices.</summary>
public sealed record PricingPhoneNumberPrice
{
    /// <summary>Number Type: local, toll free.</summary>
    [JsonPropertyName("number_type")] public string? NumberType { get; init; }
    /// <summary>Standard rate before account-specific pricing, represented as a decimal string.</summary>
    [JsonPropertyName("base_price")] public string? BasePrice { get; init; }
    /// <summary>Effective account-specific rate, represented as a decimal string.</summary>
    [JsonPropertyName("current_price")] public string? CurrentPrice { get; init; }
}

// ---- Countries list envelope ----------------------------------------------

/// <summary>Country name, ISO code and resource URL returned in a pricing-country list.</summary>
public sealed record PricingCountryRef
{
    /// <summary>Human-readable name of the country to which these rates apply.</summary>
    [JsonPropertyName("country")] public string? Country { get; init; }
    /// <summary>ISO 3166-1 alpha-2 country code to which these rates apply.</summary>
    [JsonPropertyName("iso_country")] public string? IsoCountry { get; init; }
    /// <summary>Absolute URL of this resource.</summary>
    [JsonPropertyName("url")] public string? Url { get; init; }
}

/// <summary>Shared envelope returned by every <c>Countries.ListAsync</c>.</summary>
public sealed record PricingCountriesList : IV1PagedList
{
    /// <summary>The returned countries in this page.</summary>
    [JsonPropertyName("countries")] public List<PricingCountryRef> Countries { get; init; } = new();
    /// <summary>Pagination metadata; follow next_page_url to fetch the next page.</summary>
    [JsonPropertyName("meta")] public VoiceV1Meta? Meta { get; init; }
}

// ---- Pricing v1 country / number bodies -----------------------------------

/// <summary>Voice v1 inbound and outbound rates for a country.</summary>
public sealed record PricingVoiceCountry
{
    /// <summary>Human-readable name of the country to which these rates apply.</summary>
    [JsonPropertyName("country")] public string? Country { get; init; }
    /// <summary>ISO 3166-1 alpha-2 country code to which these rates apply.</summary>
    [JsonPropertyName("iso_country")] public string? IsoCountry { get; init; }
    /// <summary>The returned outbound prefix prices in this page.</summary>
    [JsonPropertyName("outbound_prefix_prices")] public List<PricingOutboundPrefixPrice> OutboundPrefixPrices { get; init; } = new();
    /// <summary>The returned inbound call prices in this page.</summary>
    [JsonPropertyName("inbound_call_prices")] public List<PricingInboundCallPrice> InboundCallPrices { get; init; } = new();
    /// <summary>ISO 4217 currency in which the quoted prices are expressed.</summary>
    [JsonPropertyName("price_unit")] public string? PriceUnit { get; init; }
    /// <summary>Absolute URL of this resource.</summary>
    [JsonPropertyName("url")] public string? Url { get; init; }
}

/// <summary>Voice v1 rate quote for a particular telephone number.</summary>
public sealed record PricingVoiceNumber
{
    /// <summary>Telephone number in E.164 format used for this pricing lookup.</summary>
    [JsonPropertyName("number")] public string? Number { get; init; }
    /// <summary>Human-readable name of the country to which these rates apply.</summary>
    [JsonPropertyName("country")] public string? Country { get; init; }
    /// <summary>ISO 3166-1 alpha-2 country code to which these rates apply.</summary>
    [JsonPropertyName("iso_country")] public string? IsoCountry { get; init; }
    /// <summary>Rate for placing calls, represented as a decimal string.</summary>
    [JsonPropertyName("outbound_call_price")] public PricingOutboundCallPrice? OutboundCallPrice { get; init; }
    /// <summary>Rate for receiving calls, represented as a decimal string.</summary>
    [JsonPropertyName("inbound_call_price")] public PricingInboundCallPrice? InboundCallPrice { get; init; }
    /// <summary>ISO 4217 currency in which the quoted prices are expressed.</summary>
    [JsonPropertyName("price_unit")] public string? PriceUnit { get; init; }
    /// <summary>Absolute URL of this resource.</summary>
    [JsonPropertyName("url")] public string? Url { get; init; }
}

/// <summary>Messaging rates for a country, grouped by destination carrier.</summary>
public sealed record PricingMessagingCountry
{
    /// <summary>Human-readable name of the country to which these rates apply.</summary>
    [JsonPropertyName("country")] public string? Country { get; init; }
    /// <summary>ISO 3166-1 alpha-2 country code to which these rates apply.</summary>
    [JsonPropertyName("iso_country")] public string? IsoCountry { get; init; }
    /// <summary>The returned outbound sms prices in this page.</summary>
    [JsonPropertyName("outbound_sms_prices")] public List<PricingOutboundSMSPrice> OutboundSmsPrices { get; init; } = new();
    /// <summary>The returned inbound sms prices in this page.</summary>
    [JsonPropertyName("inbound_sms_prices")] public List<PricingInboundCallPrice> InboundSmsPrices { get; init; } = new();
    /// <summary>ISO 4217 currency in which the quoted prices are expressed.</summary>
    [JsonPropertyName("price_unit")] public string? PriceUnit { get; init; }
    /// <summary>Absolute URL of this resource.</summary>
    [JsonPropertyName("url")] public string? Url { get; init; }
}

/// <summary>Telephone-number prices for the number classes offered in a country.</summary>
public sealed record PricingPhoneNumberCountry
{
    /// <summary>Human-readable name of the country to which these rates apply.</summary>
    [JsonPropertyName("country")] public string? Country { get; init; }
    /// <summary>ISO 3166-1 alpha-2 country code to which these rates apply.</summary>
    [JsonPropertyName("iso_country")] public string? IsoCountry { get; init; }
    /// <summary>The returned phone number prices in this page.</summary>
    [JsonPropertyName("phone_number_prices")] public List<PricingPhoneNumberPrice> PhoneNumberPrices { get; init; } = new();
    /// <summary>ISO 4217 currency in which the quoted prices are expressed.</summary>
    [JsonPropertyName("price_unit")] public string? PriceUnit { get; init; }
    /// <summary>Absolute URL of this resource.</summary>
    [JsonPropertyName("url")] public string? Url { get; init; }
}

// ---- Pricing v2 country / number bodies -----------------------------------

/// <summary>Voice v2 country rates, including destination-prefix and origin-specific rates.</summary>
public sealed record PricingVoiceCountryV2
{
    /// <summary>Human-readable name of the country to which these rates apply.</summary>
    [JsonPropertyName("country")] public string? Country { get; init; }
    /// <summary>ISO 3166-1 alpha-2 country code to which these rates apply.</summary>
    [JsonPropertyName("iso_country")] public string? IsoCountry { get; init; }
    /// <summary>The returned outbound prefix prices in this page.</summary>
    [JsonPropertyName("outbound_prefix_prices")] public List<PricingOutboundPrefixPriceWithOrigin> OutboundPrefixPrices { get; init; } = new();
    /// <summary>The returned inbound call prices in this page.</summary>
    [JsonPropertyName("inbound_call_prices")] public List<PricingInboundCallPrice> InboundCallPrices { get; init; } = new();
    /// <summary>ISO 4217 currency in which the quoted prices are expressed.</summary>
    [JsonPropertyName("price_unit")] public string? PriceUnit { get; init; }
    /// <summary>Absolute URL of this resource.</summary>
    [JsonPropertyName("url")] public string? Url { get; init; }
}

/// <summary>Voice v2 rate quote qualified by destination and originating telephone numbers.</summary>
public sealed record PricingVoiceNumberV2
{
    /// <summary>Destination telephone number in E.164 format used to obtain a rate quote.</summary>
    [JsonPropertyName("destination_number")] public string? DestinationNumber { get; init; }
    /// <summary>Source telephone number in E.164 format used to obtain a rate quote.</summary>
    [JsonPropertyName("origination_number")] public string? OriginationNumber { get; init; }
    /// <summary>Human-readable name of the country to which these rates apply.</summary>
    [JsonPropertyName("country")] public string? Country { get; init; }
    /// <summary>ISO 3166-1 alpha-2 country code to which these rates apply.</summary>
    [JsonPropertyName("iso_country")] public string? IsoCountry { get; init; }
    /// <summary>The returned outbound call prices in this page.</summary>
    [JsonPropertyName("outbound_call_prices")] public List<PricingOutboundCallPriceWithOrigin> OutboundCallPrices { get; init; } = new();
    /// <summary>Rate for receiving calls, represented as a decimal string.</summary>
    [JsonPropertyName("inbound_call_price")] public PricingInboundCallPrice? InboundCallPrice { get; init; }
    /// <summary>ISO 4217 currency in which the quoted prices are expressed.</summary>
    [JsonPropertyName("price_unit")] public string? PriceUnit { get; init; }
    /// <summary>Absolute URL of this resource.</summary>
    [JsonPropertyName("url")] public string? Url { get; init; }
}

/// <summary>SIP trunking origination and termination rates for a country.</summary>
public sealed record PricingTrunkingCountry
{
    /// <summary>Human-readable name of the country to which these rates apply.</summary>
    [JsonPropertyName("country")] public string? Country { get; init; }
    /// <summary>ISO 3166-1 alpha-2 country code to which these rates apply.</summary>
    [JsonPropertyName("iso_country")] public string? IsoCountry { get; init; }
    /// <summary>The returned terminating prefix prices in this page.</summary>
    [JsonPropertyName("terminating_prefix_prices")] public List<PricingOutboundPrefixPriceWithOrigin> TerminatingPrefixPrices { get; init; } = new();
    /// <summary>The returned originating call prices in this page.</summary>
    [JsonPropertyName("originating_call_prices")] public List<PricingInboundCallPrice> OriginatingCallPrices { get; init; } = new();
    /// <summary>ISO 4217 currency in which the quoted prices are expressed.</summary>
    [JsonPropertyName("price_unit")] public string? PriceUnit { get; init; }
    /// <summary>Absolute URL of this resource.</summary>
    [JsonPropertyName("url")] public string? Url { get; init; }
}

/// <summary>SIP trunking rate quote for an individual telephone number.</summary>
public sealed record PricingTrunkingNumber
{
    /// <summary>Destination telephone number in E.164 format used to obtain a rate quote.</summary>
    [JsonPropertyName("destination_number")] public string? DestinationNumber { get; init; }
    /// <summary>Source telephone number in E.164 format used to obtain a rate quote.</summary>
    [JsonPropertyName("origination_number")] public string? OriginationNumber { get; init; }
    /// <summary>Human-readable name of the country to which these rates apply.</summary>
    [JsonPropertyName("country")] public string? Country { get; init; }
    /// <summary>ISO 3166-1 alpha-2 country code to which these rates apply.</summary>
    [JsonPropertyName("iso_country")] public string? IsoCountry { get; init; }
    /// <summary>The returned terminating prefix prices in this page.</summary>
    [JsonPropertyName("terminating_prefix_prices")] public List<PricingOutboundPrefixPriceWithOrigin> TerminatingPrefixPrices { get; init; } = new();
    /// <summary>Rate for originating calls, represented as a decimal string.</summary>
    [JsonPropertyName("originating_call_price")] public PricingInboundCallPrice? OriginatingCallPrice { get; init; }
    /// <summary>ISO 4217 currency in which the quoted prices are expressed.</summary>
    [JsonPropertyName("price_unit")] public string? PriceUnit { get; init; }
    /// <summary>Absolute URL of this resource.</summary>
    [JsonPropertyName("url")] public string? Url { get; init; }
}
