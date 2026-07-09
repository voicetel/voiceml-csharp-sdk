using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using VoiceML;
using VoiceML.Models;
using Xunit;

namespace VoiceML.Tests;

/// <summary>Wire-shape tests for the v0.9.2 surface: per-product host routing, Messaging
/// Service (<c>client.MessagingV1.Services</c>), and Pricing v1/v2 (<c>client.Pricing</c>).
/// <para>Messaging Service must ride <c>messaging.voicetel.com</c> (that host is what
/// disambiguates it from a Conversation Service on the shared <c>/v1/Services</c> path).
/// Pricing rides the default host. Host derivation is unit-tested directly.</para></summary>
public class V092Tests
{
    private const string AccSid = "AC" + "ffffffffffffffffffffffffffffffff";
    private const string ApiKey = "secret-key";
    private const string Base = "https://voiceml.voicetel.com";
    private const string Msg = "https://messaging.voicetel.com";
    private const string Conv = "https://conversations.voicetel.com";

    // ---- Host resolution ----------------------------------------------------

    [Fact]
    public void HostDerivation_FromDefault()
    {
        var (def, messaging, conversations) = ProductHosts.Resolve(Base);
        Assert.Equal(Base, def);
        Assert.Equal(Msg, messaging);
        Assert.Equal(Conv, conversations);
    }

    [Fact]
    public void HostDerivation_Regional()
    {
        var (def, messaging, conversations) = ProductHosts.Resolve("https://east-1.us.voiceml.voicetel.com");
        Assert.Equal("https://east-1.us.voiceml.voicetel.com", def);
        Assert.Equal("https://east-1.us.messaging.voicetel.com", messaging);
        Assert.Equal("https://east-1.us.conversations.voicetel.com", conversations);
    }

    [Fact]
    public void HostDerivation_SelfHosted_FallsBackToSingleHost()
    {
        // A custom host has no `voiceml` label to swap — every product stays on it,
        // so a single-host deployment keeps working.
        var (def, messaging, conversations) = ProductHosts.Resolve("https://pbx.acme.com");
        Assert.Equal("https://pbx.acme.com", def);
        Assert.Equal("https://pbx.acme.com", messaging);
        Assert.Equal("https://pbx.acme.com", conversations);
    }

    [Fact]
    public void HostDerivation_ExplicitOverridesWin()
    {
        var (def, messaging, conversations) = ProductHosts.Resolve(
            "https://pbx.acme.com",
            messagingBaseUrl: "https://msg.acme.com",
            conversationsBaseUrl: "https://conv.acme.com/");
        Assert.Equal("https://pbx.acme.com", def);
        Assert.Equal("https://msg.acme.com", messaging);
        Assert.Equal("https://conv.acme.com", conversations);
    }

    [Fact]
    public void V092Resources_AreWired()
    {
        using var client = new VoiceMLClient(new ClientOptions { AccountSid = AccSid, ApiKey = ApiKey });
        Assert.NotNull(client.MessagingV1);
        Assert.NotNull(client.MessagingV1.Services);
        Assert.NotNull(client.Pricing.V1.Voice.Countries);
        Assert.NotNull(client.Pricing.V1.Voice.Numbers);
        Assert.NotNull(client.Pricing.V1.Messaging.Countries);
        Assert.NotNull(client.Pricing.V1.PhoneNumbers.Countries);
        Assert.NotNull(client.Pricing.V2.Voice.Countries);
        Assert.NotNull(client.Pricing.V2.Voice.Numbers);
        Assert.NotNull(client.Pricing.V2.Trunking.Countries);
        Assert.NotNull(client.Pricing.V2.Trunking.Numbers);
    }

    // ---- Messaging Service — CRUD on the messaging host ---------------------

    [Fact]
    public async Task MessagingService_Crud_OnMessagingHost()
    {
        const string sid = "MG" + "11111111111111111111111111111111";
        var requests = new List<(HttpMethod Method, Uri Uri, string Body)>();
        var handler = new RecordingHandler(req =>
        {
            var body = req.Content is null ? "" : req.Content.ReadAsStringAsync().Result;
            requests.Add((req.Method, req.RequestUri!, body));
            var path = req.RequestUri!.AbsolutePath;
            if (req.Method == HttpMethod.Post && path == "/v1/Services")
            {
                return Reply(HttpStatusCode.Created, MessagingServiceJson(sid));
            }
            if (req.Method == HttpMethod.Get && path == "/v1/Services")
            {
                return Reply(HttpStatusCode.OK,
                    "{\"services\":[" + MessagingServiceJson(sid) + "]," + MetaJson() + "}");
            }
            if (req.Method == HttpMethod.Delete)
            {
                return new HttpResponseMessage(HttpStatusCode.NoContent);
            }
            // GET or POST /v1/Services/{sid}
            return Reply(HttpStatusCode.OK, MessagingServiceJson(sid));
        });
        using var client = NewClient(handler);

        var created = await client.MessagingV1.Services.CreateAsync(new CreateMessagingServiceRequest
        {
            FriendlyName = "alerts",
            InboundRequestUrl = "https://example.com/in",
            StickySender = true,
        });
        var listed = await client.MessagingV1.Services.ListAsync(pageSize: 25);
        var fetched = await client.MessagingV1.Services.GetAsync(sid);
        var updated = await client.MessagingV1.Services.UpdateAsync(sid,
            new UpdateMessagingServiceRequest { FriendlyName = "renamed" });
        await client.MessagingV1.Services.DeleteAsync(sid);

        Assert.Equal(sid, created.Sid);
        Assert.StartsWith("MG", created.Sid);
        Assert.Single(listed.Services);
        Assert.Equal(sid, fetched.Sid);
        Assert.Equal(sid, updated.Sid);

        // Every request must have hit the messaging host, not the default one.
        Assert.All(requests, r => Assert.Equal("messaging.voicetel.com", r.Uri.Host));

        var createForm = ParseForm(requests[0].Body);
        Assert.Equal("alerts", createForm["FriendlyName"]);
        Assert.Equal("https://example.com/in", createForm["InboundRequestUrl"]);
        Assert.Equal("true", createForm["StickySender"]);

        Assert.Contains("PageSize=25", requests[1].Uri.Query);

        var updateForm = ParseForm(requests[3].Body);
        Assert.Single(updateForm);
        Assert.Equal("renamed", updateForm["FriendlyName"]);
    }

    [Fact]
    public async Task MessagingService_HostOverride()
    {
        Uri? captured = null;
        var handler = new RecordingHandler(req =>
        {
            captured = req.RequestUri;
            return Reply(HttpStatusCode.OK, "{\"services\":[]," + MetaJson() + "}");
        });
        var http = new HttpClient(handler) { Timeout = TimeSpan.FromSeconds(5) };
        using var client = new VoiceMLClient(new ClientOptions
        {
            AccountSid = AccSid,
            ApiKey = ApiKey,
            BaseUrl = "https://pbx.acme.com",
            MessagingBaseUrl = "https://msg.acme.com",
            HttpClient = http,
            MaxRetries = 0,
        });
        await client.MessagingV1.Services.ListAsync();
        Assert.NotNull(captured);
        Assert.Equal("msg.acme.com", captured!.Host);
    }

    // ---- Pricing v1/v2 — read-only on the default host ----------------------

    [Fact]
    public async Task Pricing_V1_VoiceCountriesAndNumber()
    {
        var requests = new List<Uri>();
        var handler = new RecordingHandler(req =>
        {
            requests.Add(req.RequestUri!);
            var path = req.RequestUri!.AbsolutePath;
            if (path == "/v1/Voice/Countries")
            {
                return Reply(HttpStatusCode.OK,
                    "{\"countries\":[{\"country\":\"United States\",\"iso_country\":\"US\"," +
                    "\"url\":\"" + Base + "/v1/Voice/Countries/US\"}]," +
                    "\"meta\":{\"page\":0,\"page_size\":50}}");
            }
            if (path == "/v1/Voice/Countries/US")
            {
                return Reply(HttpStatusCode.OK,
                    "{\"country\":\"United States\",\"iso_country\":\"US\"," +
                    "\"outbound_prefix_prices\":[{\"prefixes\":[\"1\"],\"base_price\":\"0.013\"," +
                    "\"current_price\":\"0.013\",\"friendly_name\":\"United States & Canada\"}]," +
                    "\"inbound_call_prices\":[{\"base_price\":\"0.0085\",\"current_price\":\"0.0085\"," +
                    "\"number_type\":\"local\"}],\"price_unit\":\"USD\"," +
                    "\"url\":\"" + Base + "/v1/Voice/Countries/US\"}");
            }
            // /v1/Voice/Numbers/%2B18005551234
            return Reply(HttpStatusCode.OK,
                "{\"number\":\"+18005551234\",\"country\":\"United States\",\"iso_country\":\"US\"," +
                "\"outbound_call_price\":{\"base_price\":\"0.013\",\"current_price\":\"0.013\"}," +
                "\"inbound_call_price\":{\"base_price\":\"0.0085\",\"current_price\":\"0.0085\"," +
                "\"number_type\":\"toll free\"},\"price_unit\":\"USD\"," +
                "\"url\":\"" + Base + "/v1/Voice/Numbers/+18005551234\"}");
        });
        using var client = NewClient(handler);

        var listed = await client.Pricing.V1.Voice.Countries.ListAsync();
        var fetched = await client.Pricing.V1.Voice.Countries.FetchAsync("US");
        var num = await client.Pricing.V1.Voice.Numbers.FetchAsync("+18005551234");

        Assert.Equal("US", listed.Countries[0].IsoCountry);
        Assert.Equal(new[] { "1" }, fetched.OutboundPrefixPrices[0].Prefixes);
        Assert.Equal("toll free", num.InboundCallPrice!.NumberType);
        Assert.All(requests, u => Assert.Equal("voiceml.voicetel.com", u.Host));
        // E.164 `+` must be percent-encoded in the number path segment.
        Assert.Equal("/v1/Voice/Numbers/%2B18005551234", requests[2].AbsolutePath);
    }

    [Fact]
    public async Task Pricing_V2_VoiceNumber_WithOrigination()
    {
        Uri? captured = null;
        var handler = new RecordingHandler(req =>
        {
            captured = req.RequestUri;
            return Reply(HttpStatusCode.OK,
                "{\"destination_number\":\"+18005551234\",\"origination_number\":\"+15551112222\"," +
                "\"country\":\"United States\",\"iso_country\":\"US\"," +
                "\"outbound_call_prices\":[{\"origination_prefixes\":[\"1\"],\"base_price\":\"0.013\"," +
                "\"current_price\":\"0.013\"}]," +
                "\"inbound_call_price\":{\"base_price\":\"0.0085\",\"current_price\":\"0.0085\"," +
                "\"number_type\":\"local\"},\"price_unit\":\"USD\"," +
                "\"url\":\"" + Base + "/v2/Voice/Numbers/+18005551234\"}");
        });
        using var client = NewClient(handler);

        var got = await client.Pricing.V2.Voice.Numbers.FetchAsync("+18005551234", "+15551112222");
        Assert.Equal("+15551112222", got.OriginationNumber);
        Assert.NotNull(captured);
        Assert.Equal("/v2/Voice/Numbers/%2B18005551234", captured!.AbsolutePath);
        Assert.Contains("OriginationNumber=%2B15551112222", captured.Query);
        Assert.Equal("voiceml.voicetel.com", captured.Host);
    }

    [Fact]
    public async Task Pricing_V2_TrunkingCountry()
    {
        var handler = new RecordingHandler(req =>
        {
            Assert.Equal($"{Base}/v2/Trunking/Countries/US", req.RequestUri!.ToString());
            return Reply(HttpStatusCode.OK,
                "{\"country\":\"United States\",\"iso_country\":\"US\"," +
                "\"terminating_prefix_prices\":[{\"origination_prefixes\":[\"1\"]," +
                "\"destination_prefixes\":[\"1\"],\"base_price\":\"0.013\",\"current_price\":\"0.013\"," +
                "\"friendly_name\":\"US\"}]," +
                "\"originating_call_prices\":[{\"base_price\":\"0.0085\",\"current_price\":\"0.0085\"," +
                "\"number_type\":\"local\"}],\"price_unit\":\"USD\"," +
                "\"url\":\"" + Base + "/v2/Trunking/Countries/US\"}");
        });
        using var client = NewClient(handler);

        var got = await client.Pricing.V2.Trunking.Countries.FetchAsync("US");
        Assert.Equal("US", got.TerminatingPrefixPrices[0].FriendlyName);
    }

    [Fact]
    public async Task Pricing_V1_MessagingCountries_List_OnDefaultHost()
    {
        Uri? captured = null;
        var handler = new RecordingHandler(req =>
        {
            captured = req.RequestUri;
            return Reply(HttpStatusCode.OK, "{\"countries\":[],\"meta\":{\"page\":0}}");
        });
        using var client = NewClient(handler);
        var listed = await client.Pricing.V1.Messaging.Countries.ListAsync();
        Assert.Empty(listed.Countries);
        Assert.NotNull(captured);
        Assert.Equal("voiceml.voicetel.com", captured!.Host);
    }

    // ---- JSON builders ------------------------------------------------------

    private static string MessagingServiceJson(string sid) =>
        "{\"sid\":\"" + sid + "\",\"account_sid\":\"" + AccSid + "\",\"friendly_name\":\"alerts\"," +
        "\"inbound_request_url\":\"https://example.com/in\",\"sticky_sender\":true," +
        "\"date_created\":\"2026-07-08T00:00:00Z\",\"date_updated\":\"2026-07-08T00:00:00Z\"," +
        "\"url\":\"" + Msg + "/v1/Services/" + sid + "\"}";

    private static string MetaJson() =>
        "\"meta\":{\"first_page_url\":\"" + Msg + "/v1/Services?Page=0\",\"next_page_url\":null," +
        "\"previous_page_url\":null,\"url\":\"" + Msg + "/v1/Services\",\"page\":0,\"page_size\":50," +
        "\"key\":\"services\"}";

    // ---- Test infrastructure ------------------------------------------------

    private static VoiceMLClient NewClient(HttpMessageHandler handler)
    {
        var http = new HttpClient(handler) { Timeout = TimeSpan.FromSeconds(5) };
        return new VoiceMLClient(new ClientOptions
        {
            AccountSid = AccSid,
            ApiKey = ApiKey,
            HttpClient = http,
            MaxRetries = 0,
        });
    }

    private static HttpResponseMessage Reply(HttpStatusCode status, string json)
        => new(status) { Content = new StringContent(json, Encoding.UTF8, "application/json") };

    private static string DecodeForm(string s) => Uri.UnescapeDataString(s.Replace('+', ' '));

    private static Dictionary<string, string> ParseForm(string body)
    {
        var map = new Dictionary<string, string>();
        if (string.IsNullOrEmpty(body)) return map;
        foreach (var pair in body.Split('&'))
        {
            if (string.IsNullOrEmpty(pair)) continue;
            var idx = pair.IndexOf('=');
            if (idx < 0) continue;
            map[DecodeForm(pair.Substring(0, idx))] = DecodeForm(pair.Substring(idx + 1));
        }
        return map;
    }

    private sealed class RecordingHandler : HttpMessageHandler
    {
        private readonly Func<HttpRequestMessage, HttpResponseMessage> _responder;
        public RecordingHandler(Func<HttpRequestMessage, HttpResponseMessage> responder) => _responder = responder;
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken ct)
            => Task.FromResult(_responder(request));
    }
}
