using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using VoiceML.Exceptions;
using VoiceML.Models;

namespace VoiceML.Resources;

/// <summary>Top-level <c>/v1</c> + <c>/v2</c> Pricing surface — Twilio <c>pricing.twilio.com</c>.
/// Read-only. Served on the default host (VoiceML has no pricing subdomain). Layout:
/// <code>
/// client.Pricing.V1.Voice.Countries.List / Fetch
/// client.Pricing.V1.Voice.Numbers.Fetch
/// client.Pricing.V1.Messaging.Countries.List / Fetch
/// client.Pricing.V1.PhoneNumbers.Countries.List / Fetch
/// client.Pricing.V2.Voice.Countries.List / Fetch
/// client.Pricing.V2.Voice.Numbers.Fetch
/// client.Pricing.V2.Trunking.Countries.List / Fetch
/// client.Pricing.V2.Trunking.Numbers.Fetch
/// </code>
/// Every <c>Countries.ListAsync</c> returns the shared <see cref="PricingCountriesList"/> envelope;
/// <c>FetchAsync</c> returns the product-specific country/number body.</summary>
public sealed class PricingResource
{
    /// <summary>Pricing <c>/v1</c> — Voice, Messaging, PhoneNumbers.</summary>
    public PricingV1Resource V1 { get; }

    /// <summary>Pricing <c>/v2</c> — Voice, Trunking.</summary>
    public PricingV2Resource V2 { get; }

    public PricingResource(Transport transport)
    {
        V1 = new PricingV1Resource(transport);
        V2 = new PricingV2Resource(transport);
    }
}

// ---- Shared helpers --------------------------------------------------------

/// <summary>A <c>.../Countries</c> list + per-country fetch. <typeparamref name="T"/> is the
/// product-specific fetch body.</summary>
public sealed class PricingCountriesResource<T> where T : class
{
    private readonly Transport _transport;
    private readonly string _basePath;

    internal PricingCountriesResource(Transport transport, string basePath)
    {
        _transport = transport;
        _basePath = basePath;
    }

    /// <summary>List countries. Optional <paramref name="pageSize"/> query.</summary>
    public async Task<PricingCountriesList> ListAsync(int? pageSize = null, CancellationToken ct = default)
    {
        var query = new[] { new KeyValuePair<string, string?>("PageSize", pageSize?.ToString()) };
        return await _transport.SendAsync<PricingCountriesList>(HttpMethod.Get,
            _basePath, queryParams: query, ct: ct).ConfigureAwait(false)
            ?? new PricingCountriesList();
    }

    /// <summary>Fetch pricing for one ISO country.</summary>
    public async Task<T> FetchAsync(string isoCountry, CancellationToken ct = default)
    {
        var r = await _transport.SendAsync<T>(HttpMethod.Get,
            $"{_basePath}/{isoCountry}", ct: ct).ConfigureAwait(false);
        return r ?? throw new ApiException($"empty body on GET {_basePath}/{{iso}}", 200);
    }
}

// ---- v1 products -----------------------------------------------------------

/// <summary>Pricing <c>/v1</c> group.</summary>
public sealed class PricingV1Resource
{
    public PricingV1VoiceResource Voice { get; }
    public PricingV1MessagingResource Messaging { get; }
    public PricingV1PhoneNumbersResource PhoneNumbers { get; }

    public PricingV1Resource(Transport transport)
    {
        Voice = new PricingV1VoiceResource(transport);
        Messaging = new PricingV1MessagingResource(transport);
        PhoneNumbers = new PricingV1PhoneNumbersResource(transport);
    }
}

/// <summary><c>/v1/Voice</c> — Countries + Numbers.</summary>
public sealed class PricingV1VoiceResource
{
    public PricingCountriesResource<PricingVoiceCountry> Countries { get; }
    public PricingV1VoiceNumbersResource Numbers { get; }

    public PricingV1VoiceResource(Transport transport)
    {
        Countries = new PricingCountriesResource<PricingVoiceCountry>(transport, "/v1/Voice/Countries");
        Numbers = new PricingV1VoiceNumbersResource(transport);
    }
}

/// <summary><c>/v1/Voice/Numbers/{Number}</c> — per-number fetch.</summary>
public sealed class PricingV1VoiceNumbersResource
{
    private readonly Transport _transport;
    public PricingV1VoiceNumbersResource(Transport transport) { _transport = transport; }

    public async Task<PricingVoiceNumber> FetchAsync(string number, CancellationToken ct = default)
    {
        var r = await _transport.SendAsync<PricingVoiceNumber>(HttpMethod.Get,
            $"/v1/Voice/Numbers/{Uri.EscapeDataString(number)}", ct: ct).ConfigureAwait(false);
        return r ?? throw new ApiException("empty body on GET /v1/Voice/Numbers/{number}", 200);
    }
}

/// <summary><c>/v1/Messaging</c> — Countries only.</summary>
public sealed class PricingV1MessagingResource
{
    public PricingCountriesResource<PricingMessagingCountry> Countries { get; }

    public PricingV1MessagingResource(Transport transport)
    {
        Countries = new PricingCountriesResource<PricingMessagingCountry>(transport, "/v1/Messaging/Countries");
    }
}

/// <summary><c>/v1/PhoneNumbers</c> — Countries only.</summary>
public sealed class PricingV1PhoneNumbersResource
{
    public PricingCountriesResource<PricingPhoneNumberCountry> Countries { get; }

    public PricingV1PhoneNumbersResource(Transport transport)
    {
        Countries = new PricingCountriesResource<PricingPhoneNumberCountry>(transport, "/v1/PhoneNumbers/Countries");
    }
}

// ---- v2 products -----------------------------------------------------------

/// <summary>Pricing <c>/v2</c> group.</summary>
public sealed class PricingV2Resource
{
    public PricingV2VoiceResource Voice { get; }
    public PricingV2TrunkingResource Trunking { get; }

    public PricingV2Resource(Transport transport)
    {
        Voice = new PricingV2VoiceResource(transport);
        Trunking = new PricingV2TrunkingResource(transport);
    }
}

/// <summary><c>/v2/Voice</c> — Countries + Numbers.</summary>
public sealed class PricingV2VoiceResource
{
    public PricingCountriesResource<PricingVoiceCountryV2> Countries { get; }
    public PricingV2VoiceNumbersResource Numbers { get; }

    public PricingV2VoiceResource(Transport transport)
    {
        Countries = new PricingCountriesResource<PricingVoiceCountryV2>(transport, "/v2/Voice/Countries");
        Numbers = new PricingV2VoiceNumbersResource(transport);
    }
}

/// <summary><c>/v2/Voice/Numbers/{Destination}</c> — per-number fetch with optional origination.</summary>
public sealed class PricingV2VoiceNumbersResource
{
    private readonly Transport _transport;
    public PricingV2VoiceNumbersResource(Transport transport) { _transport = transport; }

    public async Task<PricingVoiceNumberV2> FetchAsync(
        string destinationNumber, string? originationNumber = null, CancellationToken ct = default)
    {
        var query = new[] { new KeyValuePair<string, string?>("OriginationNumber", originationNumber) };
        var r = await _transport.SendAsync<PricingVoiceNumberV2>(HttpMethod.Get,
            $"/v2/Voice/Numbers/{Uri.EscapeDataString(destinationNumber)}", queryParams: query, ct: ct).ConfigureAwait(false);
        return r ?? throw new ApiException("empty body on GET /v2/Voice/Numbers/{dest}", 200);
    }
}

/// <summary><c>/v2/Trunking</c> — Countries + Numbers.</summary>
public sealed class PricingV2TrunkingResource
{
    public PricingCountriesResource<PricingTrunkingCountry> Countries { get; }
    public PricingV2TrunkingNumbersResource Numbers { get; }

    public PricingV2TrunkingResource(Transport transport)
    {
        Countries = new PricingCountriesResource<PricingTrunkingCountry>(transport, "/v2/Trunking/Countries");
        Numbers = new PricingV2TrunkingNumbersResource(transport);
    }
}

/// <summary><c>/v2/Trunking/Numbers/{Destination}</c> — per-number fetch with optional origination.</summary>
public sealed class PricingV2TrunkingNumbersResource
{
    private readonly Transport _transport;
    public PricingV2TrunkingNumbersResource(Transport transport) { _transport = transport; }

    public async Task<PricingTrunkingNumber> FetchAsync(
        string destinationNumber, string? originationNumber = null, CancellationToken ct = default)
    {
        var query = new[] { new KeyValuePair<string, string?>("OriginationNumber", originationNumber) };
        var r = await _transport.SendAsync<PricingTrunkingNumber>(HttpMethod.Get,
            $"/v2/Trunking/Numbers/{Uri.EscapeDataString(destinationNumber)}", queryParams: query, ct: ct).ConfigureAwait(false);
        return r ?? throw new ApiException("empty body on GET /v2/Trunking/Numbers/{dest}", 200);
    }
}
