using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Sockets;
using System.Reflection;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using VoiceML.Exceptions;
using VoiceML.Models;
using Xunit;

namespace VoiceML.Tests;

[CollectionDefinition("Transport timing", DisableParallelization = true)]
public sealed class TimingCollection { }

[Collection("Transport timing")]
public sealed class RegressionTests : IDisposable
{
    private const string Key = "test-api-key-0123456789";
    private const string Account = "ACXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX";
    private const string Prefix = "https://voiceml.voicetel.com/2010-04-01/Accounts/" + Account;
    private readonly List<TimeSpan> _delays = new();

    public RegressionTests() => Transport.DelayOverride = (delay, ct) => { ct.ThrowIfCancellationRequested(); _delays.Add(delay); return Task.CompletedTask; };
    public void Dispose() => Transport.DelayOverride = null;

    private static HttpResponseMessage Reply(int status = 200, string body = "{}") => new((HttpStatusCode)status) { Content = new StringContent(body) };
    private static VoiceMLClient Client(HttpMessageHandler handler, int retries = 2) => new(new ClientOptions
    { AccountSid = Account, ApiKey = Key, HttpClient = new HttpClient(handler), MaxRetries = retries });
    private static CreateMessageRequest Message() => new() { To = "+14155550101", Body = "hello" };

    [Theory]
    [InlineData(500)]
    [InlineData(502)]
    [InlineData(503)]
    [InlineData(504)]
    public async Task PostIsNotRetriedOnServerFailure(int status)
    {
        var count = 0;
        using var client = Client(new SpecContractTests.Handler(_ => { count++; return Task.FromResult(Reply(status)); }));
        await Assert.ThrowsAsync<ServerException>(() => client.Messages.CreateAsync(Message()));
        Assert.Equal(1, count);
        Assert.Empty(_delays);
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task OnlyIdempotentRequestsRetryReadTimeouts(bool get)
    {
        var count = 0;
        var cause = new TaskCanceledException("read timed out");
        using var client = Client(new SpecContractTests.Handler(_ => { count++; throw cause; }));
        var ex = await Assert.ThrowsAsync<VoiceMLTimeoutException>(() => get ? (Task)client.Calls.ListAsync() : client.Messages.CreateAsync(Message()));
        Assert.Same(cause, ex.InnerException);
        Assert.Equal(get ? 3 : 1, count);
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task OnlyIdempotentRequestsRetryConnectionResets(bool get)
    {
        var count = 0;
        using var client = Client(new SpecContractTests.Handler(_ => { count++; throw new HttpRequestException("connection reset"); }));
        var ex = await Assert.ThrowsAsync<ApiException>(() => get ? (Task)client.Calls.ListAsync() : client.Messages.CreateAsync(Message()));
        Assert.IsType<HttpRequestException>(ex.InnerException);
        Assert.Equal(get ? 3 : 1, count);
    }

    [Theory]
    [InlineData(HttpRequestError.NameResolutionError)]
    [InlineData(HttpRequestError.SecureConnectionError)]
    [InlineData(HttpRequestError.ConnectionError)]
    public async Task PostRetriesProvableConnectionEstablishmentFailure(HttpRequestError kind)
    {
        var count = 0;
        using var client = Client(new SpecContractTests.Handler(_ =>
        {
            if (++count == 1) throw new HttpRequestException(kind, "not sent", new SocketException((int)SocketError.ConnectionRefused));
            return Task.FromResult(Reply());
        }));
        await client.Messages.CreateAsync(Message());
        Assert.Equal(2, count);
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task RateLimitRetriesHonorSecondsAndHttpDate(bool date)
    {
        var count = 0;
        using var client = Client(new SpecContractTests.Handler(_ =>
        {
            var response = Reply(++count == 1 ? 429 : 200);
            if (count == 1) response.Headers.RetryAfter = date
                ? new RetryConditionHeaderValue(DateTimeOffset.UtcNow.AddSeconds(3))
                : new RetryConditionHeaderValue(TimeSpan.FromSeconds(3));
            return Task.FromResult(response);
        }));
        await client.Messages.CreateAsync(Message());
        Assert.Equal(2, count);
        Assert.Single(_delays);
        Assert.InRange(_delays[0].TotalSeconds, date ? 1.8 : 3, 3.1);
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task RetryAfterAboveCapRaisesImmediately(bool date)
    {
        var count = 0;
        using var client = Client(new SpecContractTests.Handler(_ =>
        {
            count++;
            var response = Reply(429, "{\"code\":20429,\"status\":429}");
            response.Headers.RetryAfter = date ? new RetryConditionHeaderValue(DateTimeOffset.UtcNow.AddSeconds(120)) : new RetryConditionHeaderValue(TimeSpan.FromSeconds(120));
            return Task.FromResult(response);
        }));
        var ex = await Assert.ThrowsAsync<RateLimitException>(() => client.Calls.ListAsync());
        Assert.Equal(1, count);
        Assert.InRange(ex.RetryAfterSeconds ?? 0, 118, 120.1);
        Assert.NotNull(ex.RetryAfter);
        Assert.Empty(_delays);
    }

    [Fact]
    public async Task GetRetriesAndUsesBoundedFullJitter()
    {
        var count = 0;
        using var client = Client(new SpecContractTests.Handler(_ => Task.FromResult(Reply(++count < 3 ? 503 : 200))));
        await client.Calls.ListAsync();
        Assert.Equal(3, count);
        Assert.Equal(2, _delays.Count);
        Assert.InRange(_delays[0].TotalSeconds, 0, 0.5);
        Assert.InRange(_delays[1].TotalSeconds, 0, 1);
    }

    [Fact]
    public async Task CallerCancellationPropagatesWithoutRetries()
    {
        using var cts = new CancellationTokenSource();
        var count = 0;
        using var client = Client(new SpecContractTests.Handler(_ => { count++; cts.Cancel(); throw new TaskCanceledException("caller cancelled"); }));
        await Assert.ThrowsAnyAsync<OperationCanceledException>(() => client.Calls.ListAsync(ct: cts.Token));
        Assert.Equal(1, count);
        Assert.Empty(_delays);
    }

    [Fact]
    public void SecretsAreAbsentFromDefaultStringAndJsonOutput()
    {
        foreach (var alias in new[] { false, true })
        {
            var options = new ClientOptions { AccountSid = Account, ApiKey = alias ? null : Key, AuthToken = alias ? Key : null };
            Assert.DoesNotContain(Key, options.ToString());
            Assert.DoesNotContain(Key, JsonSerializer.Serialize(options));
            Assert.Contains("[REDACTED]", options.ToString());
            Assert.Null(typeof(ClientOptions).GetProperty("ResolvedCredential", BindingFlags.Public | BindingFlags.Instance));
            using var client = new VoiceMLClient(options);
            Assert.DoesNotContain(Key, client.ToString());
            Assert.DoesNotContain(Key, JsonSerializer.Serialize(client));
        }
    }

    [Fact]
    public void RotatedAccountKeyIsRedactedFromDefaultStringOutput()
    {
        var rotated = new AccountWithKey
        {
            Sid = Account,
            ApiKey = Key,
            SipUsername = "sip-user",
            Disabled = false,
            CreatedAt = "2026-10-18T00:00:00Z",
            UpdatedAt = "2026-10-18T00:00:00Z",
        };
        var rendered = rotated.ToString();
        Assert.DoesNotContain(Key, rendered);
        Assert.Contains("[REDACTED]", rendered);
        Assert.Contains(Account, rendered);
        Assert.Contains("sip-user", rendered);
    }

    [Fact]
    public async Task MonitorFeedbackAndRotateKeyOperationsUseExactRoutes()
    {
        var requests = new List<HttpRequestMessage>();
        string? formBody = null;
        using var client = Client(new SpecContractTests.Handler(async r =>
        {
            requests.Add(r);
            if (r.Content is not null) formBody = await r.Content.ReadAsStringAsync();
            return Reply();
        }));
        await client.MonitorAlerts.ListAsync(new ListMonitorAlertsParams
        {
            LogLevel = MonitorLogLevel.Error,
            StartDate = "2026-10-01",
            EndDate = "2026-10-17",
            PageSize = 25,
        });
        await client.MonitorAlerts.GetAsync("NO0123456789abcdef0123456789abcdef");
        await client.Messages.CreateFeedbackAsync(
            "SM0123456789abcdef0123456789abcdef",
            new CreateMessageFeedbackRequest { Outcome = MessageFeedbackOutcome.Confirmed });
        await client.Accounts.RotateKeyAsync(Account);
        Assert.Equal(new[] { "GET", "GET", "POST", "POST" }, requests.Select(r => r.Method.Method));
        Assert.Equal(new[]
        {
            "https://voiceml.voicetel.com/v1/Alerts?LogLevel=error&StartDate=2026-10-01&EndDate=2026-10-17&PageSize=25",
            "https://voiceml.voicetel.com/v1/Alerts/NO0123456789abcdef0123456789abcdef",
            Prefix + "/Messages/SM0123456789abcdef0123456789abcdef/Feedback.json",
            Prefix + "/RotateKey.json",
        }, requests.Select(r => r.RequestUri!.AbsoluteUri));
        Assert.Equal("Outcome=confirmed", formBody);
        Assert.Null(requests[3].Content);
        Assert.All(requests, r => Assert.NotNull(r.Headers.Authorization));
    }

    [Fact]
    public async Task MonitorFeedbackAndRotateKeyResponsesDecodeTypedModels()
    {
        var count = 0;
        using var client = Client(new SpecContractTests.Handler(_ => Task.FromResult(Reply(200, ++count switch
        {
            1 => "{\"alerts\":[{\"sid\":\"NO0123456789abcdef0123456789abcdef\",\"account_sid\":\"" + Account
               + "\",\"alert_text\":\"gateway error\",\"error_code\":\"11200\",\"log_level\":\"warning\","
               + "\"more_info\":\"https://voicetel.com/docs/errors/11200\",\"request_method\":\"POST\","
               + "\"request_url\":\"https://voiceml.voicetel.com/2010-04-01/Accounts/" + Account + "/Messages.json\","
               + "\"url\":\"https://voiceml.voicetel.com/v1/Alerts/NO0123456789abcdef0123456789abcdef\","
               + "\"date_created\":\"2026-10-17T12:00:00Z\",\"date_generated\":\"2026-10-17T12:00:00Z\","
               + "\"date_updated\":\"2026-10-17T12:00:00Z\",\"api_version\":null,\"resource_sid\":\"SM0123456789abcdef0123456789abcdef\","
               + "\"service_sid\":null,\"response_body\":\"upstream said no\",\"future_field\":1}],"
               + "\"meta\":{\"page\":0,\"page_size\":25,\"first_page_url\":null,\"next_page_url\":null,"
               + "\"previous_page_url\":null,\"url\":\"https://voiceml.voicetel.com/v1/Alerts\",\"key\":\"alerts\"}}",
            2 => "{\"account_sid\":\"" + Account + "\",\"message_sid\":\"SM0123456789abcdef0123456789abcdef\","
               + "\"outcome\":\"unconfirmed\",\"date_created\":\"Fri, 17 Oct 2026 12:00:00 +0000\","
               + "\"date_updated\":\"Fri, 17 Oct 2026 12:00:00 +0000\","
               + "\"uri\":\"/2010-04-01/Accounts/" + Account + "/Messages/SM0123456789abcdef0123456789abcdef/Feedback.json\"}",
            _ => "{\"sid\":\"" + Account + "\",\"api_key\":\"fresh-rotated-key\",\"sip_username\":\"sip-user\","
               + "\"phone_regions\":[\"US\"],\"allow_cidr\":[],\"disabled\":false,\"stt_http_token_set\":true,"
               + "\"tts_http_token_set\":false,\"stripe_secret_key_set\":false,\"pay_enabled\":true,"
               + "\"sms_allowed_providers\":[],\"stt_allowed_providers\":[],\"aws_secret_access_key_set\":false,"
               + "\"created_at\":\"2026-10-18T00:00:00Z\",\"updated_at\":\"2026-10-18T00:00:00Z\"}",
        }))));
        var alerts = await client.MonitorAlerts.ListAsync(new ListMonitorAlertsParams { PageSize = 25 });
        var feedback = await client.Messages.CreateFeedbackAsync(
            "SM0123456789abcdef0123456789abcdef", new CreateMessageFeedbackRequest());
        var rotated = await client.Accounts.RotateKeyAsync(Account);
        var alert = Assert.Single(alerts.Alerts);
        Assert.Equal("11200", alert.ErrorCode);
        Assert.Equal(MonitorLogLevel.Warning, alert.LogLevel);
        Assert.Equal("SM0123456789abcdef0123456789abcdef", alert.ResourceSid);
        Assert.Null(alert.ServiceSid);
        Assert.Null(alert.ApiVersion);
        Assert.Equal("upstream said no", alert.ResponseBody);
        Assert.Equal(25, alerts.Meta?.PageSize);
        Assert.Null(alerts.Meta?.NextPageUrl);
        Assert.Equal(MessageFeedbackOutcome.Unconfirmed, feedback.Outcome);
        Assert.Equal("SM0123456789abcdef0123456789abcdef", feedback.MessageSid);
        Assert.Equal("fresh-rotated-key", rotated.ApiKey);
        Assert.Equal("sip-user", rotated.SipUsername);
        Assert.Equal(new[] { "US" }, rotated.PhoneRegions);
        Assert.True(rotated.SttHttpTokenSet);
        Assert.True(rotated.PayEnabled);
        Assert.Equal("2026-10-18T00:00:00Z", rotated.CreatedAt);
    }

    [Fact]
    public async Task MonitorAlertsIterationTerminatesWithoutContinuation()
    {
        var count = 0;
        using var client = Client(new SpecContractTests.Handler(_ =>
        {
            count++;
            return Task.FromResult(Reply(200, "{\"alerts\":[{\"sid\":\"NO1\"},{\"sid\":\"NO2\"}],\"meta\":{\"next_page_url\":null}}"));
        }));
        var seen = new List<string>();
        await foreach (var alert in client.MonitorAlerts.IterateAsync()) seen.Add(alert.Sid);
        Assert.Equal(new[] { "NO1", "NO2" }, seen);
        Assert.Equal(1, count);
        Assert.Null(await client.MonitorAlerts.NextPageAsync(new MonitorAlertList()));
    }

    [Fact]
    public async Task ErrorCodesNormalizeAndDecodeFailuresKeepTheirCause()
    {
        using var client = Client(new SpecContractTests.Handler(_ => Task.FromResult(Reply(409, "{\"code\":\"20409\",\"status\":409,\"message\":\"conflict\"}"))));
        var ex = await Assert.ThrowsAsync<ConflictException>(() => client.Accounts.UpdateAsync(Account, new UpdateAccountRequest()));
        Assert.Equal(20409, ex.Code);
        Assert.Equal("20409", ex.CodeRaw);
        Assert.Equal(409, ex.Status);
        using var badJson = Client(new SpecContractTests.Handler(_ => Task.FromResult(Reply(200, "{\"price\":12}"))));
        var decode = await Assert.ThrowsAsync<ApiException>(() => badJson.Calls.GetAsync("CA1"));
        Assert.Contains("could not decode", decode.Message);
        Assert.IsType<JsonException>(decode.InnerException);
        using var html = Client(new SpecContractTests.Handler(_ => Task.FromResult(Reply(400, "<html>error</html>"))));
        Assert.Equal("<html>error</html>", (await Assert.ThrowsAsync<BadRequestException>(() => html.Calls.ListAsync())).Body);
    }

    [Theory]
    [InlineData("+ a/b?c#d%é", "%2B%20a%2Fb%3Fc%23d%25%C3%A9")]
    [InlineData("Twilio.CURRENT", "Twilio.CURRENT")]
    public async Task PathSegmentsUseExactRfc3986Bytes(string value, string encoded)
    {
        var urls = new List<string>();
        using var client = Client(new SpecContractTests.Handler(request => { urls.Add(request.RequestUri!.AbsoluteUri); return Task.FromResult(Reply()); }));
        await client.Calls.GetAsync(value);
        await client.ConversationsV1.Conversations.GetAsync(value);
        await client.RoutesV2.PhoneNumbers.GetAsync(value);
        await client.Pricing.V1.Voice.Numbers.FetchAsync(value);
        Assert.Equal(Prefix + "/Calls/" + encoded + ".json", urls[0]);
        Assert.Equal("https://conversations.voicetel.com/v1/Conversations/" + encoded, urls[1]);
        Assert.Equal("https://voiceml.voicetel.com/v2/PhoneNumbers/" + encoded, urls[2]);
        Assert.Equal("https://voiceml.voicetel.com/v1/Voice/Numbers/" + encoded, urls[3]);
    }

    [Fact]
    public async Task PaginationFollowsRawTokenLinkAndIsLazy()
    {
        var urls = new List<string>();
        var next = "/2010-04-01/Accounts/" + Account + "/Calls.json?PageToken=PA%2Btoken%2F%3D&Page=7&Status=completed";
        using var client = Client(new SpecContractTests.Handler(request =>
        {
            urls.Add(request.RequestUri!.AbsoluteUri);
            var body = urls.Count == 1 ? JsonSerializer.Serialize(new { calls = new[] { new { sid = "CA1" } }, next_page_uri = next }) : "{\"calls\":[{\"sid\":\"CA2\"}],\"next_page_uri\":null}";
            return Task.FromResult(Reply(200, body));
        }));
        var sequence = client.Calls.IterateAsync();
        Assert.Empty(urls);
        var items = new List<Call>();
        await foreach (var item in sequence) items.Add(item);
        Assert.Equal(2, items.Count);
        Assert.Equal("https://voiceml.voicetel.com" + next, urls[1]);
        urls.Clear();
        await foreach (var item in client.Calls.IterateAsync()) { _ = item; break; }
        Assert.Single(urls);
    }

    [Fact]
    public async Task V1ContinuationRequiresProductOrigin()
    {
        var requests = new List<string>();
        using var client = Client(new SpecContractTests.Handler(r => { requests.Add(r.RequestUri!.AbsoluteUri); return Task.FromResult(Reply()); }));
        var page = new ConversationsV1ConversationList { Meta = new VoiceV1Meta { NextPageUrl = "https://conversations.voicetel.com/v1/Conversations?PageToken=PA%2B" } };
        await client.ConversationsV1.Conversations.NextPageAsync(page);
        Assert.Equal(page.Meta.NextPageUrl, requests.Single());
        foreach (var bad in new[] { "https://evil.example/v1/Conversations", "http://conversations.voicetel.com/v1/Conversations", "https://conversations.voicetel.com:9999/v1/Conversations" })
            await Assert.ThrowsAsync<VoiceMLException>(() => client.ConversationsV1.Conversations.NextPageAsync(page with { Meta = page.Meta with { NextPageUrl = bad } }));
        Assert.Single(requests);
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task BinaryRedirectDropsAuthAndRefusesDowngrade(bool downgrade)
    {
        var count = 0;
        using var client = Client(new SpecContractTests.Handler(request =>
        {
            count++;
            Assert.Equal("*/*", request.Headers.Accept.ToString());
            if (count == 1)
            {
                Assert.NotNull(request.Headers.Authorization);
                var response = Reply(302);
                response.Headers.Location = new Uri((downgrade ? "http" : "https") + "://storage.example/archive");
                return Task.FromResult(response);
            }
            Assert.Null(request.Headers.Authorization);
            return Task.FromResult(Reply(200, "RIFF"));
        }));
        if (downgrade) await Assert.ThrowsAsync<ApiException>(() => client.Recordings.GetAudioAsync("RE1"));
        else Assert.Equal(4, (await client.Recordings.GetAudioAsync("RE1")).Content.Length);
        Assert.Equal(downgrade ? 1 : 2, count);
    }

    [Fact]
    public async Task DiagnosticsNeedNoCredentialsAndWrapParseAndServerErrors()
    {
        using var http = new HttpClient(new SpecContractTests.Handler(request =>
        {
            Assert.Null(request.Headers.Authorization);
            Assert.NotEmpty(request.Headers.UserAgent);
            return Task.FromResult(Reply(200, request.RequestUri!.AbsolutePath == "/health" ? "{\"ok\":true}" : ""));
        }));
        Assert.True((await VoiceMLDiagnostics.HealthAsync(httpClient: http)).Ok);
        var ex = await Assert.ThrowsAsync<ApiException>(() => VoiceMLDiagnostics.OpenApiJsonAsync(httpClient: http));
        Assert.IsAssignableFrom<JsonException>(ex.InnerException);
        using var unavailable = new HttpClient(new SpecContractTests.Handler(_ => Task.FromResult(Reply(503))));
        await Assert.ThrowsAsync<ServerException>(() => VoiceMLDiagnostics.HealthAsync(httpClient: unavailable));
    }

    [Fact]
    public async Task MmsUsesRepeatedKeysAndRejectsMissingContent()
    {
        string? body = null;
        using var client = Client(new SpecContractTests.Handler(async request => { body = await request.Content!.ReadAsStringAsync(); return Reply(); }));
        await client.Messages.CreateAsync(new CreateMessageRequest { To = "+14155550101", MediaUrl = new() { "https://example.com/a", "https://example.com/b" }, ValidityPeriod = 60, StatusCallbackMethod = "POST" });
        Assert.Equal("To=%2B14155550101&MediaUrl=https%3A%2F%2Fexample.com%2Fa&MediaUrl=https%3A%2F%2Fexample.com%2Fb&ValidityPeriod=60&StatusCallbackMethod=POST", body);
        await Assert.ThrowsAsync<VoiceMLException>(() => client.Messages.CreateAsync(new CreateMessageRequest { To = "+14155550101" }));
    }

    [Fact]
    public void SignatureVectorsAndPortVariants()
    {
        var pairs = new[] { Pair("CallSid", "CA0123456789abcdef0123456789abcdef"), Pair("From", "+14155550100"), Pair("To", "+14155550101"), Pair("Digits", "1234"), Pair("AccountSid", Account) };
        const string url = "https://example.com/voice/answer?case=1";
        const string withPort = "https://example.com:443/voice/answer?case=1";
        Assert.Equal("pk/0TPPT+p98NEZykeWQLqRXyGU=", RequestValidator.GetSignature(Key, url, pairs));
        Assert.Equal("/Bw0s+u5vlGd6zLAOTz/eMg33pk=", RequestValidator.GetSignature(Key, withPort, pairs));
        Assert.True(RequestValidator.ValidateRequest(Key, withPort, pairs, "pk/0TPPT+p98NEZykeWQLqRXyGU="));
        Assert.True(RequestValidator.ValidateRequest(Key, url, pairs, "/Bw0s+u5vlGd6zLAOTz/eMg33pk="));
        Assert.Equal("jCnpBgyg45aZYwUie5fmq4EYUic=", RequestValidator.GetSignature(Key, "https://example.com/hook", new[] { Pair("To", "+14155550101"), Pair("Event", "answered"), Pair("Event", "initiated") }));
        Assert.Equal("9I4odWBHhF7TMIkIrNtPEPjD9fU=", RequestValidator.GetSignature(Key, "https://example.com/voice/answer?CallSid=CA0123456789abcdef0123456789abcdef&From=%2B14155550100"));
        Assert.Equal(RequestValidator.GetSignature(Key, url, pairs), RequestValidator.GetSignature(Key, url + "#ignored", pairs));
        Assert.False(RequestValidator.ValidateRequest("wrong-key", url, pairs, "pk/0TPPT+p98NEZykeWQLqRXyGU="));
        Assert.False(RequestValidator.ValidateRequest(Key, url, pairs.Append(Pair("Tampered", "true")), "pk/0TPPT+p98NEZykeWQLqRXyGU="));
        const string raw = "https://EXAMPLE.com/a%2fb?x=%2b";
        const string rawWithPort = "https://EXAMPLE.com:443/a%2fb?x=%2b";
        Assert.True(RequestValidator.ValidateRequest(Key, raw, pairs, RequestValidator.GetSignature(Key, rawWithPort, pairs)));
        Assert.True(RequestValidator.ValidateRequest(Key, rawWithPort, pairs, RequestValidator.GetSignature(Key, raw, pairs)));
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task ActualHttpClientTimeoutIsAnSdkError(bool get)
    {
        using var handler = new DelayedHandler();
        using var http = new HttpClient(handler) { Timeout = TimeSpan.FromMilliseconds(30) };
        using var client = new VoiceMLClient(new ClientOptions { AccountSid = Account, ApiKey = Key, HttpClient = http, MaxRetries = 1 });
        var error = await Assert.ThrowsAsync<VoiceMLTimeoutException>(() => get ? (Task)client.Calls.ListAsync() : client.Messages.CreateAsync(Message()));
        Assert.IsAssignableFrom<OperationCanceledException>(error.InnerException);
        Assert.Equal(get ? 2 : 1, handler.Requests);
    }

    [Fact]
    public async Task RequestAndResponseContentAreDisposedAfterEveryAttempt()
    {
        var requests = new List<HttpRequestMessage>();
        var responses = new List<TrackedContent>();
        using var client = Client(new SpecContractTests.Handler(r =>
        {
            requests.Add(r);
            var content = new TrackedContent();
            responses.Add(content);
            return Task.FromResult(new HttpResponseMessage(requests.Count == 1 ? HttpStatusCode.TooManyRequests : HttpStatusCode.OK) { Content = content });
        }));
        await client.Messages.CreateAsync(Message());
        Assert.Equal(2, requests.Count);
        foreach (var request in requests)
            await Assert.ThrowsAsync<ObjectDisposedException>(() => request.Content!.ReadAsStringAsync());
        Assert.All(responses, content => Assert.True(content.Disposed));
    }

    [Fact]
    public async Task EmptyPageWithContinuationStillFetchesNextPage()
    {
        var count = 0;
        using var client = Client(new SpecContractTests.Handler(_ => Task.FromResult(Reply(200, ++count == 1
            ? "{\"calls\":[],\"next_page_uri\":\"/next?PageToken=PA1\"}"
            : "{\"calls\":[{\"sid\":\"CA1\"}]}"))));
        var items = new List<Call>();
        await foreach (var item in client.Calls.IterateAsync()) items.Add(item);
        Assert.Single(items);
        Assert.Equal(2, count);
    }

    [Fact]
    public async Task AccountPageHelpersRejectForeignOriginsAndDotSegments()
    {
        var count = 0;
        using var client = Client(new SpecContractTests.Handler(_ => { count++; return Task.FromResult(Reply()); }));
        await Assert.ThrowsAsync<VoiceMLException>(() => client.Calls.FetchNextPageAsync(new CallList { NextPageUri = "//evil.example/steal" }));
        await Assert.ThrowsAsync<VoiceMLException>(() => client.ConversationsV1.Conversations.Messages("..").ListAsync());
        Assert.Equal(0, count);
    }

    [Fact]
    public async Task JsonAttributesAreCompactedBeforeFormEncoding()
    {
        string? body = null;
        using var client = Client(new SpecContractTests.Handler(async r => { body = await r.Content!.ReadAsStringAsync(); return Reply(); }));
        await client.ConversationsV1.Conversations.CreateAsync(new CreateConversationRequest { Attributes = "{ \"a\" : 1 }" });
        Assert.Equal("Attributes=%7B%22a%22%3A1%7D", body);
    }

    [Fact]
    public void SpecNullableResponseAnnotationsAreHonest()
    {
        using var doc = JsonDocument.Parse(System.IO.File.ReadAllText(System.IO.Path.Combine(AppContext.BaseDirectory, "Fixtures", "voiceml-openapi.json")));
        var schemas = doc.RootElement.GetProperty("components").GetProperty("schemas");
        var nullability = new NullabilityInfoContext();
        foreach (var type in typeof(Call).Assembly.GetTypes().Where(t => t.Namespace == "VoiceML.Models"))
        {
            if (!schemas.TryGetProperty(type.Name, out var schema) || !schema.TryGetProperty("properties", out var props)) continue;
            foreach (var p in type.GetProperties())
            {
                var wire = p.GetCustomAttribute<System.Text.Json.Serialization.JsonPropertyNameAttribute>()?.Name;
                if (wire is null || !props.TryGetProperty(wire, out var field)) continue;
                var nullable = field.TryGetProperty("nullable", out var n) && n.GetBoolean();
                nullable |= field.TryGetProperty("type", out var t) && t.ValueKind == JsonValueKind.Array && t.EnumerateArray().Any(x => x.GetString() == "null");
                if (nullable) Assert.True(nullability.Create(p).ReadState != NullabilityState.NotNull || Nullable.GetUnderlyingType(p.PropertyType) is not null, type.Name + "." + p.Name);
            }
        }
    }

    private sealed class DelayedHandler : HttpMessageHandler
    {
        internal int Requests;
        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken ct)
        {
            Requests++;
            await Task.Delay(TimeSpan.FromSeconds(5), ct);
            return Reply();
        }
    }

    private sealed class TrackedContent : StringContent
    {
        internal bool Disposed;
        internal TrackedContent() : base("{}") { }
        protected override void Dispose(bool disposing) { Disposed = true; base.Dispose(disposing); }
    }

    private static KeyValuePair<string, string> Pair(string key, string value) => new(key, value);
}
