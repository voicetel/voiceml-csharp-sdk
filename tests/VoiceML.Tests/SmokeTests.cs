using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using VoiceML;
using VoiceML.Exceptions;
using VoiceML.Models;
using Xunit;

namespace VoiceML.Tests;

/// <summary>End-to-end smoke tests for the VoiceML SDK. We mock HttpClient via a custom
/// <see cref="HttpMessageHandler"/>; the SDK never reaches the network.</summary>
public class SmokeTests
{
    private const string Sid = "AC" + "ffffffffffffffffffffffffffffffff";
    private const string ApiKey = "secret-key";
    private const string CallSid = "CA" + "ffffffffffffffffffffffffffffffff";

    // -----------------------------------------------------------------------
    // Configuration / construction
    // -----------------------------------------------------------------------

    [Fact]
    public void DefaultBaseUrl_IsVoiceMLProduction()
    {
        var opts = new ClientOptions { AccountSid = Sid, ApiKey = ApiKey };
        using var client = new VoiceMLClient(opts);
        Assert.Equal("https://voiceml.voicetel.com", client.BaseUrl);
    }

    [Fact]
    public void Construction_MissingAccountSid_Throws()
    {
        var opts = new ClientOptions { AccountSid = "", ApiKey = ApiKey };
        Assert.Throws<ConfigurationException>(() => new VoiceMLClient(opts));
    }

    [Fact]
    public void Construction_MissingApiKey_Throws()
    {
        var opts = new ClientOptions { AccountSid = Sid, ApiKey = "" };
        Assert.Throws<ConfigurationException>(() => new VoiceMLClient(opts));
    }

    [Fact]
    public void Construction_NegativeMaxRetries_Throws()
    {
        var opts = new ClientOptions { AccountSid = Sid, ApiKey = ApiKey, MaxRetries = -1 };
        Assert.Throws<ConfigurationException>(() => new VoiceMLClient(opts));
    }

    // -----------------------------------------------------------------------
    // Calls.CreateAsync — form body, auth header, URL path
    // -----------------------------------------------------------------------

    [Fact]
    public async Task CallsCreate_SendsExpectedRequest()
    {
        var handler = new MockHandler(req =>
        {
            // Method + path
            Assert.Equal(HttpMethod.Post, req.Method);
            Assert.NotNull(req.RequestUri);
            Assert.Equal(
                $"https://voiceml.voicetel.com/2010-04-01/Accounts/{Sid}/Calls",
                req.RequestUri!.ToString());

            // Authorization: Basic base64(AccountSid:ApiKey)
            Assert.NotNull(req.Headers.Authorization);
            Assert.Equal("Basic", req.Headers.Authorization!.Scheme);
            var expected = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{Sid}:{ApiKey}"));
            Assert.Equal(expected, req.Headers.Authorization.Parameter);

            // Form body
            var body = req.Content!.ReadAsStringAsync().Result;
            var form = ParseForm(body);
            Assert.Equal("+18005550000", form["To"]);
            Assert.Equal("+18005551234", form["From"]);
            Assert.Equal("https://example.com/twiml", form["Url"]);
            Assert.Equal("true", form["Record"]);

            return Reply(HttpStatusCode.Created, """{"sid":"CAabc","account_sid":"AC0","status":"queued","direction":"outbound-api","api_version":"2010-04-01","uri":"/2010-04-01/Accounts/AC0/Calls/CAabc","date_created":"2025-01-01","date_updated":"2025-01-01"}""");
        });

        using var client = NewClient(handler);
        var call = await client.Calls.CreateAsync(new CreateCallRequest
        {
            To = "+18005550000",
            From = "+18005551234",
            Url = "https://example.com/twiml",
            Record = true,
        });
        Assert.Equal("CAabc", call.Sid);
    }

    // -----------------------------------------------------------------------
    // Calls.ListAsync — Twilio-literal StartTime>= / StartTime<= query names
    // -----------------------------------------------------------------------

    [Fact]
    public async Task CallsList_StartTimeFilters_AreLiteralOnTheWire()
    {
        var handler = new MockHandler(req =>
        {
            var url = req.RequestUri!.ToString();
            // Must contain BOTH literal `StartTime>=` and `StartTime<=` — not URL-escaped.
            Assert.Contains("StartTime>=", url);
            Assert.Contains("StartTime<=", url);
            return Reply(HttpStatusCode.OK, """{"calls":[],"page":0,"page_size":50}""");
        });
        using var client = NewClient(handler);
        var list = await client.Calls.ListAsync(new ListCallsParams
        {
            StartTimeGte = "2025-01-01T00:00:00Z",
            StartTimeLte = "2025-12-31T23:59:59Z",
        });
        Assert.NotNull(list);
        Assert.Empty(list.Calls);
    }

    // -----------------------------------------------------------------------
    // Boolean encoding: Muted=true / Hold=false → "true" / "false"
    // -----------------------------------------------------------------------

    [Fact]
    public async Task ParticipantUpdate_BooleansEncodedLowercase()
    {
        var handler = new MockHandler(req =>
        {
            var body = req.Content!.ReadAsStringAsync().Result;
            var form = ParseForm(body);
            Assert.Equal("true", form["Muted"]);
            Assert.Equal("false", form["Hold"]);
            return Reply(HttpStatusCode.OK, """{"call_sid":"CA1","conference_sid":"CF1","account_sid":"AC0","muted":true,"hold":false,"start_conference_on_enter":true,"end_conference_on_exit":false,"status":"connected","api_version":"2010-04-01","uri":"/x"}""");
        });
        using var client = NewClient(handler);
        var p = await client.Conferences.UpdateParticipantAsync(
            "CFconfconfconfconfconfconfconfconfco", CallSid,
            new UpdateParticipantRequest { Muted = true, Hold = false });
        Assert.True(p.Muted);
        Assert.False(p.Hold);
    }

    // -----------------------------------------------------------------------
    // Error mapping: 401 / 404 / 429 / 501 / 409
    // -----------------------------------------------------------------------

    [Fact]
    public async Task ErrorMapping_401_ThrowsAuthenticationException()
    {
        var handler = new MockHandler(_ => Reply(HttpStatusCode.Unauthorized,
            """{"code":20003,"message":"Authentication Error - No credentials provided"}"""));
        using var client = NewClient(handler);
        var ex = await Assert.ThrowsAsync<AuthenticationException>(() => client.Calls.GetAsync(CallSid));
        Assert.Equal(401, ex.StatusCode);
        Assert.Equal(20003, ex.Code);
    }

    [Fact]
    public async Task ErrorMapping_404_ThrowsNotFoundException()
    {
        var handler = new MockHandler(_ => Reply(HttpStatusCode.NotFound,
            """{"code":20404,"message":"Not Found","status":404}"""));
        using var client = NewClient(handler);
        var ex = await Assert.ThrowsAsync<NotFoundException>(() => client.Calls.GetAsync(CallSid));
        Assert.Equal(404, ex.StatusCode);
    }

    [Fact]
    public async Task ErrorMapping_429_ThrowsRateLimitException_WithRetryAfter()
    {
        var handler = new MockHandler(_ =>
        {
            var msg = Reply(HttpStatusCode.TooManyRequests,
                """{"code":20429,"message":"Rate Limit Exceeded"}""");
            msg.Headers.Add("Retry-After", "5");
            return msg;
        });
        // Disable retries so the 429 surfaces immediately.
        using var client = NewClient(handler, maxRetries: 0);
        var ex = await Assert.ThrowsAsync<RateLimitException>(() => client.Calls.GetAsync(CallSid));
        Assert.Equal(429, ex.StatusCode);
        Assert.Equal(5.0, ex.RetryAfterSeconds);
    }

    [Fact]
    public async Task ErrorMapping_501_ThrowsNotImplementedAPIException()
    {
        var handler = new MockHandler(_ => Reply(HttpStatusCode.NotImplemented,
            """{"code":20501,"message":"Not Implemented"}"""));
        using var client = NewClient(handler);
        var ex = await Assert.ThrowsAsync<NotImplementedAPIException>(
            () => client.Calls.SendUserDefinedMessageAsync(CallSid));
        Assert.Equal(501, ex.StatusCode);
    }

    [Fact]
    public async Task ErrorMapping_409_ThrowsConflictException_WithCode20409()
    {
        var handler = new MockHandler(_ => Reply(HttpStatusCode.Conflict,
            """{"code":20409,"message":"Queue has waiting members"}"""));
        using var client = NewClient(handler);
        var ex = await Assert.ThrowsAsync<ConflictException>(
            () => client.Queues.DeleteAsync("QU01234567890123456789012345678901"));
        Assert.Equal(409, ex.StatusCode);
        Assert.Equal(20409, ex.Code);
        Assert.IsAssignableFrom<ApiException>(ex);
    }

    // -----------------------------------------------------------------------
    // Retry behavior: 503 → 200 with MaxRetries=1
    // -----------------------------------------------------------------------

    [Fact]
    public async Task Retry_503Then200_Succeeds_WithMaxRetries1()
    {
        var calls = 0;
        var handler = new MockHandler(_ =>
        {
            calls++;
            if (calls == 1)
            {
                return Reply(HttpStatusCode.ServiceUnavailable, "{}");
            }
            return Reply(HttpStatusCode.OK,
                $$"""{"sid":"CA1","account_sid":"{{Sid}}","status":"completed","direction":"outbound-api","api_version":"2010-04-01","uri":"/x","date_created":"2025","date_updated":"2025"}""");
        });
        using var client = NewClient(handler, maxRetries: 1);
        var call = await client.Calls.GetAsync(CallSid);
        Assert.Equal("CA1", call.Sid);
        Assert.Equal(2, calls);
    }

    // -----------------------------------------------------------------------
    // Helpers
    // -----------------------------------------------------------------------

    private static VoiceMLClient NewClient(HttpMessageHandler handler, int maxRetries = 2)
    {
        var http = new HttpClient(handler) { Timeout = TimeSpan.FromSeconds(5) };
        return new VoiceMLClient(new ClientOptions
        {
            AccountSid = Sid,
            ApiKey = ApiKey,
            HttpClient = http,
            MaxRetries = maxRetries,
        });
    }

    private static HttpResponseMessage Reply(HttpStatusCode status, string json)
        => new(status)
        {
            Content = new StringContent(json, Encoding.UTF8, "application/json"),
        };

    private static Dictionary<string, string> ParseForm(string body)
    {
        var map = new Dictionary<string, string>(StringComparer.Ordinal);
        foreach (var pair in body.Split('&', StringSplitOptions.RemoveEmptyEntries))
        {
            var idx = pair.IndexOf('=');
            if (idx < 0)
            {
                map[Uri.UnescapeDataString(pair)] = "";
                continue;
            }
            var k = Uri.UnescapeDataString(pair.Substring(0, idx));
            var v = Uri.UnescapeDataString(pair.Substring(idx + 1));
            map[k] = v;
        }
        return map;
    }

    /// <summary>HttpMessageHandler that calls a user-supplied callback to build the response.
    /// Test-only; lives in this test file so we don't ship it in the SDK package.</summary>
    private sealed class MockHandler : HttpMessageHandler
    {
        private readonly Func<HttpRequestMessage, HttpResponseMessage> _responder;

        public MockHandler(Func<HttpRequestMessage, HttpResponseMessage> responder)
        {
            _responder = responder;
        }

        protected override Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request, CancellationToken cancellationToken)
        {
            return Task.FromResult(_responder(request));
        }
    }
}
