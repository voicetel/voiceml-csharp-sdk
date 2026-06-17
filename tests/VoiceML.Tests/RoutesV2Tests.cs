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

/// <summary>Wire-shape tests for the v0.8.1 Routes V2 (Inbound Processing Region) API.</summary>
public class RoutesV2Tests
{
    private const string AccSid = "AC" + "ffffffffffffffffffffffffffffffff";
    private const string ApiKey = "secret-key";
    private const string DomainName = "ingress.example.com";
    private const string QQSid = "QQ00000000000000000000000000000000";

    [Fact]
    public void RoutesV2_IsWiredOnClient()
    {
        using var client = new VoiceMLClient(new ClientOptions { AccountSid = AccSid, ApiKey = ApiKey });
        Assert.NotNull(client.RoutesV2);
        Assert.NotNull(client.RoutesV2.SipDomains);
    }

    [Fact]
    public async Task RoutesV2_SipDomains_Fetch_UsesV2NamespaceNoAccountPrefix()
    {
        Uri? capturedUri = null;
        var handler = new MockHandler(req =>
        {
            capturedUri = req.RequestUri;
            return Reply(HttpStatusCode.OK, PayloadJson());
        });
        using var client = NewClient(handler);
        var rv = await client.RoutesV2.SipDomains.GetAsync(DomainName);
        Assert.Equal(QQSid, rv.Sid);
        Assert.Equal("us1", rv.VoiceRegion);
        Assert.NotNull(capturedUri);
        Assert.Equal($"https://voiceml.voicetel.com/v2/SipDomains/{DomainName}", capturedUri!.ToString());
        Assert.DoesNotContain(AccSid, capturedUri.ToString());
    }

    [Fact]
    public async Task RoutesV2_SipDomains_Update_SendsForm()
    {
        var handler = new MockHandler(req =>
        {
            Assert.Equal(HttpMethod.Post, req.Method);
            var body = req.Content!.ReadAsStringAsync().Result;
            var form = ParseForm(body);
            Assert.Equal("ie1", form["VoiceRegion"]);
            Assert.Equal("renamed", form["FriendlyName"]);
            return Reply(HttpStatusCode.OK, PayloadJson());
        });
        using var client = NewClient(handler);
        await client.RoutesV2.SipDomains.UpdateAsync(DomainName,
            new UpdateRoutesV2SipDomainRequest { VoiceRegion = "ie1", FriendlyName = "renamed" });
    }

    [Fact]
    public async Task RoutesV2_SipDomains_Update_PartialBody()
    {
        var handler = new MockHandler(req =>
        {
            var body = req.Content!.ReadAsStringAsync().Result;
            var form = ParseForm(body);
            Assert.Equal("us1", form["VoiceRegion"]);
            Assert.False(form.ContainsKey("FriendlyName"));
            return Reply(HttpStatusCode.OK, PayloadJson());
        });
        using var client = NewClient(handler);
        await client.RoutesV2.SipDomains.UpdateAsync(DomainName,
            new UpdateRoutesV2SipDomainRequest { VoiceRegion = "us1" });
    }

    // --- Helpers ---

    private static string PayloadJson() => $$"""
        {"sid":"{{QQSid}}","sip_domain":"{{DomainName}}","account_sid":"{{AccSid}}",
         "friendly_name":"ingress","voice_region":"us1",
         "url":"https://voiceml.voicetel.com/v2/SipDomains/{{DomainName}}",
         "date_created":"2026-06-17T20:00:00Z","date_updated":"2026-06-17T20:00:00Z"}
        """;

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

    private static Dictionary<string, string> ParseForm(string body)
    {
        var map = new Dictionary<string, string>();
        foreach (var pair in body.Split('&'))
        {
            if (string.IsNullOrEmpty(pair)) continue;
            var idx = pair.IndexOf('=');
            if (idx < 0) continue;
            map[Uri.UnescapeDataString(pair.Substring(0, idx))] = Uri.UnescapeDataString(pair.Substring(idx + 1));
        }
        return map;
    }

    private sealed class MockHandler : HttpMessageHandler
    {
        private readonly Func<HttpRequestMessage, HttpResponseMessage> _responder;
        public MockHandler(Func<HttpRequestMessage, HttpResponseMessage> responder) => _responder = responder;
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken ct)
            => Task.FromResult(_responder(request));
    }
}
