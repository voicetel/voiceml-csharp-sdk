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

/// <summary>Wire-shape tests for the v0.8.0 SIP Trunking surface.</summary>
public class SipTests
{
    private const string AccSid = "AC" + "ffffffffffffffffffffffffffffffff";
    private const string ApiKey = "secret-key";
    private const string DomainSid = "SD" + "11111111111111111111111111111111";
    private const string CLSid = "CL" + "22222222222222222222222222222222";
    private const string CRSid = "CR" + "33333333333333333333333333333333";
    private const string ACLSid = "AL" + "44444444444444444444444444444444";
    private const string IPSid = "IP" + "55555555555555555555555555555555";
    private const string MappingSid = "CL" + "99999999999999999999999999999999";

    [Fact]
    public void SipResource_IsWiredOnClient()
    {
        var opts = new ClientOptions { AccountSid = AccSid, ApiKey = ApiKey };
        using var client = new VoiceMLClient(opts);
        Assert.NotNull(client.Sip);
        Assert.NotNull(client.Sip.Domains);
        Assert.NotNull(client.Sip.CredentialLists);
        Assert.NotNull(client.Sip.IpAccessControlLists);
    }

    [Fact]
    public async Task SipDomains_Create_SendsForm()
    {
        var handler = new MockHandler(req =>
        {
            Assert.Equal(HttpMethod.Post, req.Method);
            Assert.Equal($"https://voiceml.voicetel.com/2010-04-01/Accounts/{AccSid}/SIP/Domains.json", req.RequestUri!.ToString());
            var form = ParseForm(req.Content!.ReadAsStringAsync().Result);
            Assert.Equal("ingress.example.com", form["DomainName"]);
            Assert.Equal("ingress", form["FriendlyName"]);
            Assert.Equal("https://hooks/voice", form["VoiceUrl"]);
            Assert.Equal("POST", form["VoiceMethod"]);
            Assert.Equal("false", form["SipRegistration"]);
            Assert.Equal("true", form["Secure"]);
            return Reply(HttpStatusCode.OK, DomainJson());
        });
        using var client = NewClient(handler);
        var d = await client.Sip.Domains.CreateAsync(new CreateSipDomainRequest
        {
            DomainName = "ingress.example.com",
            FriendlyName = "ingress",
            VoiceUrl = "https://hooks/voice",
            VoiceMethod = "POST",
            SipRegistration = false,
            Secure = true,
        });
        Assert.Equal(DomainSid, d.Sid);
    }

    [Fact]
    public async Task SipDomains_ListFetchUpdateDelete()
    {
        int call = 0;
        var handler = new MockHandler(req =>
        {
            call++;
            return call switch
            {
                1 => Reply(HttpStatusCode.OK, $$"""{"domains":[{{DomainJson()}}],"page":0,"page_size":50,"total":1,"uri":""}"""),
                2 => Reply(HttpStatusCode.OK, DomainJson()),
                3 => Reply(HttpStatusCode.OK, DomainJson()),
                _ => Reply(HttpStatusCode.NoContent, ""),
            };
        });
        using var client = NewClient(handler);
        var list = await client.Sip.Domains.ListAsync();
        Assert.Single(list.Domains);
        Assert.Equal(DomainSid, (await client.Sip.Domains.GetAsync(DomainSid)).Sid);
        await client.Sip.Domains.UpdateAsync(DomainSid, new UpdateSipDomainRequest { FriendlyName = "renamed" });
        await client.Sip.Domains.DeleteAsync(DomainSid);
        Assert.Equal(4, call);
    }

    [Fact]
    public async Task SipCredentialLists_CRUD()
    {
        int call = 0;
        var handler = new MockHandler(req =>
        {
            call++;
            return call switch
            {
                1 => Reply(HttpStatusCode.OK, CredentialListJson()),
                2 => Reply(HttpStatusCode.OK, CredentialListJson()),
                _ => Reply(HttpStatusCode.NoContent, ""),
            };
        });
        using var client = NewClient(handler);
        var cl = await client.Sip.CredentialLists.CreateAsync(new CreateSipCredentialListRequest { FriendlyName = "office-handsets" });
        Assert.Equal(CLSid, cl.Sid);
        await client.Sip.CredentialLists.GetAsync(CLSid);
        await client.Sip.CredentialLists.DeleteAsync(CLSid);
    }

    [Fact]
    public async Task SipCredentials_NestedCreateAndFetch()
    {
        var handler = new MockHandler(req =>
        {
            if (req.Method == HttpMethod.Post)
            {
                Assert.Equal($"https://voiceml.voicetel.com/2010-04-01/Accounts/{AccSid}/SIP/CredentialLists/{CLSid}/Credentials.json", req.RequestUri!.ToString());
                var form = ParseForm(req.Content!.ReadAsStringAsync().Result);
                Assert.Equal("alice", form["Username"]);
                Assert.Equal("hunter2", form["Password"]);
            }
            return Reply(HttpStatusCode.OK, CredentialJson());
        });
        using var client = NewClient(handler);
        var cred = await client.Sip.CredentialLists.CreateCredentialAsync(CLSid,
            new CreateSipCredentialRequest { Username = "alice", Password = "hunter2" });
        Assert.Equal("alice", cred.Username);
        await client.Sip.CredentialLists.GetCredentialAsync(CLSid, CRSid);
    }

    [Fact]
    public async Task SipIpAccessControlLists_CRUD()
    {
        var handler = new MockHandler(req => Reply(HttpStatusCode.OK, AclJson()));
        using var client = NewClient(handler);
        var acl = await client.Sip.IpAccessControlLists.CreateAsync(new CreateSipIpAccessControlListRequest { FriendlyName = "carrier-allowlist" });
        Assert.Equal(ACLSid, acl.Sid);
        await client.Sip.IpAccessControlLists.GetAsync(ACLSid);
    }

    [Fact]
    public async Task SipIpAddresses_NestedCreate()
    {
        var handler = new MockHandler(req =>
        {
            Assert.Equal($"https://voiceml.voicetel.com/2010-04-01/Accounts/{AccSid}/SIP/IpAccessControlLists/{ACLSid}/IpAddresses.json", req.RequestUri!.ToString());
            var form = ParseForm(req.Content!.ReadAsStringAsync().Result);
            Assert.Equal("carrier-edge-1", form["FriendlyName"]);
            Assert.Equal("203.0.113.10", form["IpAddress"]);
            Assert.Equal("32", form["CidrPrefixLength"]);
            return Reply(HttpStatusCode.OK, IpJson());
        });
        using var client = NewClient(handler);
        await client.Sip.IpAccessControlLists.CreateIpAddressAsync(ACLSid, new CreateSipIpAddressRequest
        {
            FriendlyName = "carrier-edge-1",
            IpAddress = "203.0.113.10",
            CidrPrefixLength = 32,
        });
    }

    [Fact]
    public async Task SipDomainAuth_CallsCredentialListMappings_RoutesToAuthCalls()
    {
        var handler = new MockHandler(req =>
        {
            Assert.Equal($"https://voiceml.voicetel.com/2010-04-01/Accounts/{AccSid}/SIP/Domains/{DomainSid}/Auth/Calls/CredentialListMappings.json", req.RequestUri!.ToString());
            var form = ParseForm(req.Content!.ReadAsStringAsync().Result);
            Assert.Equal(CLSid, form["CredentialListSid"]);
            return Reply(HttpStatusCode.OK, MappingJson());
        });
        using var client = NewClient(handler);
        await client.Sip.Domains.CreateAuthCallsCredentialListMappingAsync(DomainSid,
            new CreateSipCredentialListMappingRequest { CredentialListSid = CLSid });
    }

    [Fact]
    public async Task SipDomainAuth_CallsIpAccessControlListMappings_RoutesToAuthCalls()
    {
        var handler = new MockHandler(req =>
        {
            Assert.Equal($"https://voiceml.voicetel.com/2010-04-01/Accounts/{AccSid}/SIP/Domains/{DomainSid}/Auth/Calls/IpAccessControlListMappings.json", req.RequestUri!.ToString());
            return Reply(HttpStatusCode.OK, MappingJson());
        });
        using var client = NewClient(handler);
        await client.Sip.Domains.CreateAuthCallsIpAccessControlListMappingAsync(DomainSid,
            new CreateSipIpAccessControlListMappingRequest { IpAccessControlListSid = ACLSid });
    }

    [Fact]
    public async Task SipDomainAuth_RegistrationsCredentialListMappings_RoutesToAuthRegistrations()
    {
        var handler = new MockHandler(req =>
        {
            Assert.Equal($"https://voiceml.voicetel.com/2010-04-01/Accounts/{AccSid}/SIP/Domains/{DomainSid}/Auth/Registrations/CredentialListMappings.json", req.RequestUri!.ToString());
            return Reply(HttpStatusCode.OK, MappingJson());
        });
        using var client = NewClient(handler);
        await client.Sip.Domains.CreateAuthRegistrationsCredentialListMappingAsync(DomainSid,
            new CreateSipCredentialListMappingRequest { CredentialListSid = CLSid });
    }

    [Fact]
    public async Task SipDomain_HistoricalCredentialListMappings_Create()
    {
        var handler = new MockHandler(req =>
        {
            Assert.Equal($"https://voiceml.voicetel.com/2010-04-01/Accounts/{AccSid}/SIP/Domains/{DomainSid}/CredentialListMappings.json", req.RequestUri!.ToString());
            return Reply(HttpStatusCode.OK, MappingJson());
        });
        using var client = NewClient(handler);
        await client.Sip.Domains.CreateCredentialListMappingAsync(DomainSid,
            new CreateSipCredentialListMappingRequest { CredentialListSid = CLSid });
    }

    // --- Helpers --------------------------------------------------------------

    private static string DomainJson() => $$"""
        {"sid":"{{DomainSid}}","account_sid":"{{AccSid}}","domain_name":"ingress.example.com",
         "api_version":"2010-04-01","friendly_name":"ingress","secure":true,
         "date_created":"Mon, 17 Jun 2026 12:00:00 +0000",
         "date_updated":"Mon, 17 Jun 2026 12:00:00 +0000",
         "uri":"/2010-04-01/Accounts/{{AccSid}}/SIP/Domains/{{DomainSid}}.json"}
        """;

    private static string CredentialListJson() => $$"""
        {"sid":"{{CLSid}}","account_sid":"{{AccSid}}","friendly_name":"office-handsets",
         "date_created":"Mon, 17 Jun 2026 12:00:00 +0000",
         "date_updated":"Mon, 17 Jun 2026 12:00:00 +0000",
         "uri":"/2010-04-01/Accounts/{{AccSid}}/SIP/CredentialLists/{{CLSid}}.json"}
        """;

    private static string CredentialJson() => $$"""
        {"sid":"{{CRSid}}","account_sid":"{{AccSid}}","credential_list_sid":"{{CLSid}}",
         "username":"alice",
         "date_created":"Mon, 17 Jun 2026 12:00:00 +0000",
         "date_updated":"Mon, 17 Jun 2026 12:00:00 +0000",
         "uri":"/2010-04-01/Accounts/{{AccSid}}/SIP/CredentialLists/{{CLSid}}/Credentials/{{CRSid}}.json"}
        """;

    private static string AclJson() => $$"""
        {"sid":"{{ACLSid}}","account_sid":"{{AccSid}}","friendly_name":"carrier-allowlist",
         "date_created":"Mon, 17 Jun 2026 12:00:00 +0000",
         "date_updated":"Mon, 17 Jun 2026 12:00:00 +0000",
         "uri":"/2010-04-01/Accounts/{{AccSid}}/SIP/IpAccessControlLists/{{ACLSid}}.json"}
        """;

    private static string IpJson() => $$"""
        {"sid":"{{IPSid}}","account_sid":"{{AccSid}}","ip_access_control_list_sid":"{{ACLSid}}",
         "friendly_name":"carrier-edge-1","ip_address":"203.0.113.10","cidr_prefix_length":32,
         "date_created":"Mon, 17 Jun 2026 12:00:00 +0000",
         "date_updated":"Mon, 17 Jun 2026 12:00:00 +0000",
         "uri":"/2010-04-01/Accounts/{{AccSid}}/SIP/IpAccessControlLists/{{ACLSid}}/IpAddresses/{{IPSid}}.json"}
        """;

    private static string MappingJson() => $$"""
        {"sid":"{{MappingSid}}","account_sid":"{{AccSid}}","domain_sid":"{{DomainSid}}",
         "date_created":"Mon, 17 Jun 2026 12:00:00 +0000",
         "date_updated":"Mon, 17 Jun 2026 12:00:00 +0000",
         "uri":"/2010-04-01/Accounts/{{AccSid}}/SIP/Domains/{{DomainSid}}/CredentialListMappings/{{MappingSid}}.json"}
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
            var k = Uri.UnescapeDataString(pair.Substring(0, idx));
            var v = Uri.UnescapeDataString(pair.Substring(idx + 1));
            map[k] = v;
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
