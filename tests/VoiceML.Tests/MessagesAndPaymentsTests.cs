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

/// <summary>End-to-end tests for the v0.7.0 Messages and CallPayment surfaces.</summary>
public class MessagesAndPaymentsTests
{
    private const string Sid = "AC" + "ffffffffffffffffffffffffffffffff";
    private const string ApiKey = "secret-key";
    private const string CallSid = "CA" + "ffffffffffffffffffffffffffffffff";
    private const string MsgSid = "SM" + "0123456789abcdef0123456789abcdef";
    private const string PaymentSid = "PY" + "0123456789abcdef0123456789abcdef";

    // -----------------------------------------------------------------------
    // Messages.CreateAsync — form body + path
    // -----------------------------------------------------------------------

    [Fact]
    public async Task MessagesCreate_SendsExpectedRequest()
    {
        var handler = new MockHandler(req =>
        {
            Assert.Equal(HttpMethod.Post, req.Method);
            Assert.Equal(
                $"https://voiceml.voicetel.com/2010-04-01/Accounts/{Sid}/Messages.json",
                req.RequestUri!.ToString());

            var body = req.Content!.ReadAsStringAsync().Result;
            var form = ParseForm(body);
            Assert.Equal("+18005550000", form["To"]);
            Assert.Equal("hello-world", form["Body"]);
            Assert.Equal("+18005551234", form["From"]);
            Assert.Equal("https://example.com/sms-status", form["StatusCallback"]);

            return Reply(HttpStatusCode.Created, $$"""
                {"sid":"{{MsgSid}}","account_sid":"{{Sid}}","api_version":"2010-04-01",
                 "to":"+18005550000","from":"+18005551234","body":"hello-world",
                 "status":"sent","num_segments":"1","num_media":"0","direction":"outbound-api",
                 "price":null,"price_unit":null,"error_code":null,"error_message":null,
                 "messaging_service_sid":null,
                 "date_created":"Mon, 01 Jun 2026 12:00:00 +0000",
                 "date_updated":"Mon, 01 Jun 2026 12:00:00 +0000",
                 "date_sent":"Mon, 01 Jun 2026 12:00:00 +0000",
                 "uri":"/2010-04-01/Accounts/AC0/Messages/SM0.json"}
                """);
        });
        using var client = NewClient(handler);
        var msg = await client.Messages.CreateAsync(new CreateMessageRequest
        {
            To = "+18005550000",
            Body = "hello-world",
            From = "+18005551234",
            StatusCallback = "https://example.com/sms-status",
        });
        Assert.Equal(MsgSid, msg.Sid);
        Assert.Equal(MessageStatus.Sent, msg.Status);
        Assert.Equal("1", msg.NumSegments);
        Assert.Equal("0", msg.NumMedia);
        Assert.Null(msg.Price);
        Assert.Null(msg.ErrorCode);
    }

    // -----------------------------------------------------------------------
    // Messages.FetchAsync — error_code typing
    // -----------------------------------------------------------------------

    [Fact]
    public async Task MessagesFetch_PathAndErrorCodeAsInt()
    {
        var handler = new MockHandler(req =>
        {
            Assert.Equal(HttpMethod.Get, req.Method);
            Assert.EndsWith($"/Messages/{MsgSid}.json", req.RequestUri!.AbsolutePath);
            return Reply(HttpStatusCode.OK, $$"""
                {"sid":"{{MsgSid}}","account_sid":"{{Sid}}","api_version":"2010-04-01",
                 "to":"+18005550000","from":"+18005551234","body":"oops","status":"failed",
                 "num_segments":"1","num_media":"0","direction":"outbound-api",
                 "price":null,"price_unit":null,
                 "error_code":21609,"error_message":"SMS gateway not configured",
                 "messaging_service_sid":null,
                 "date_created":"Mon, 01 Jun 2026 12:00:00 +0000",
                 "date_updated":"Mon, 01 Jun 2026 12:00:00 +0000",
                 "date_sent":null,
                 "uri":"/x"}
                """);
        });
        using var client = NewClient(handler);
        var msg = await client.Messages.FetchAsync(MsgSid);
        Assert.Equal(MessageStatus.Failed, msg.Status);
        Assert.Equal(21609, msg.ErrorCode);
        Assert.Equal("SMS gateway not configured", msg.ErrorMessage);
        Assert.Null(msg.DateSent);
    }

    // -----------------------------------------------------------------------
    // Messages.ListAsync — filter encoding (DateSent< / DateSent>)
    // -----------------------------------------------------------------------

    [Fact]
    public async Task MessagesList_DateSentFilters_AreLiteralOnTheWire()
    {
        var handler = new MockHandler(req =>
        {
            var url = req.RequestUri!.ToString();
            Assert.Contains("To=%2B18005550000", url);
            Assert.Contains("From=%2B18005551234", url);
            Assert.Contains("DateSent=2026-06-01", url);
            Assert.Contains("DateSent<", url);
            Assert.Contains("DateSent>", url);
            Assert.Contains("PageSize=25", url);
            return Reply(HttpStatusCode.OK, """{"messages":[],"page":0,"page_size":25}""");
        });
        using var client = NewClient(handler);
        var list = await client.Messages.ListAsync(new ListMessagesParams
        {
            To = "+18005550000",
            From = "+18005551234",
            DateSent = "2026-06-01",
            DateSentLt = "2026-06-15",
            DateSentGt = "2026-05-01",
            PageSize = 25,
        });
        Assert.NotNull(list);
        Assert.Empty(list.Messages);
    }

    // -----------------------------------------------------------------------
    // Messages.UpdateAsync — Body="" redaction wire shape
    // -----------------------------------------------------------------------

    [Fact]
    public async Task MessagesUpdate_BodyRedaction_PostsEmptyBody()
    {
        var handler = new MockHandler(req =>
        {
            Assert.Equal(HttpMethod.Post, req.Method);
            Assert.EndsWith($"/Messages/{MsgSid}.json", req.RequestUri!.AbsolutePath);
            var body = req.Content!.ReadAsStringAsync().Result;
            var form = ParseForm(body);
            Assert.True(form.ContainsKey("Body"));
            Assert.Equal("", form["Body"]);
            // Status was not set on the request, so it must NOT be on the wire.
            Assert.False(form.ContainsKey("Status"));

            return Reply(HttpStatusCode.OK, $$"""
                {"sid":"{{MsgSid}}","account_sid":"{{Sid}}","api_version":"2010-04-01",
                 "to":"+18005550000","from":"+18005551234","body":"",
                 "status":"sent","num_segments":"1","num_media":"0","direction":"outbound-api",
                 "price":null,"price_unit":null,"error_code":null,"error_message":null,
                 "messaging_service_sid":null,
                 "date_created":"Mon, 01 Jun 2026 12:00:00 +0000",
                 "date_updated":"Mon, 01 Jun 2026 12:30:00 +0000",
                 "date_sent":"Mon, 01 Jun 2026 12:00:00 +0000",
                 "uri":"/x"}
                """);
        });
        using var client = NewClient(handler);
        var msg = await client.Messages.UpdateAsync(MsgSid, new UpdateMessageRequest { Body = "" });
        Assert.Equal("", msg.Body);
    }

    // -----------------------------------------------------------------------
    // Messages.DeleteAsync — 204 path
    // -----------------------------------------------------------------------

    [Fact]
    public async Task MessagesDelete_NoContent()
    {
        var handler = new MockHandler(req =>
        {
            Assert.Equal(HttpMethod.Delete, req.Method);
            Assert.EndsWith($"/Messages/{MsgSid}.json", req.RequestUri!.AbsolutePath);
            return new HttpResponseMessage(HttpStatusCode.NoContent);
        });
        using var client = NewClient(handler);
        await client.Messages.DeleteAsync(MsgSid); // must not throw
    }

    // -----------------------------------------------------------------------
    // Calls.StartPaymentAsync — form body, path, several fields
    // -----------------------------------------------------------------------

    [Fact]
    public async Task CallsStartPayment_SendsExpectedRequest()
    {
        var handler = new MockHandler(req =>
        {
            Assert.Equal(HttpMethod.Post, req.Method);
            Assert.EndsWith($"/Calls/{CallSid}/Payments.json", req.RequestUri!.AbsolutePath);

            var body = req.Content!.ReadAsStringAsync().Result;
            var form = ParseForm(body);
            Assert.Equal("idem-abc-123", form["IdempotencyKey"]);
            Assert.Equal("https://example.com/pay-status", form["StatusCallback"]);
            Assert.Equal("9.99", form["ChargeAmount"]);
            Assert.Equal("USD", form["Currency"]);
            Assert.Equal("Order-42", form["Description"]);
            Assert.Equal("dtmf", form["Input"]);
            Assert.Equal("5", form["MinPostalCodeLength"]);
            Assert.Equal("credit-card", form["PaymentMethod"]);
            Assert.Equal("true", form["PostalCode"]);
            Assert.Equal("false", form["SecurityCode"]);
            Assert.Equal("10", form["Timeout"]);
            Assert.Equal("one-time", form["TokenType"]);
            // Spaces are encoded as `+` by FormUrlEncodedContent; ParseForm() preserves that
            // literal `+`. The server side performs the inverse to recover the original string.
            Assert.Equal("visa+mastercard+amex", form["ValidCardTypes"]);
            Assert.Equal("true", form["Confirmation"]);

            return Reply(HttpStatusCode.Created, $$"""
                {"sid":"{{PaymentSid}}","account_sid":"{{Sid}}","call_sid":"{{CallSid}}",
                 "api_version":"2010-04-01",
                 "date_created":"Mon, 01 Jun 2026 12:00:00 +0000",
                 "date_updated":"Mon, 01 Jun 2026 12:00:00 +0000",
                 "uri":"/2010-04-01/Accounts/{{Sid}}/Calls/{{CallSid}}/Payments/{{PaymentSid}}.json"}
                """);
        });
        using var client = NewClient(handler);
        var pay = await client.Calls.StartPaymentAsync(CallSid, new StartPaymentRequest
        {
            IdempotencyKey = "idem-abc-123",
            StatusCallback = "https://example.com/pay-status",
            ChargeAmount = "9.99",
            Currency = "USD",
            Description = "Order-42",
            Input = PaymentInput.Dtmf,
            MinPostalCodeLength = 5,
            PaymentMethod = VoiceML.Models.PaymentMethod.CreditCard,
            PostalCode = true,
            SecurityCode = false,
            Timeout = 10,
            TokenType = VoiceML.Models.PaymentTokenType.OneTime,
            ValidCardTypes = "visa mastercard amex",
            Confirmation = true,
        });
        Assert.Equal(PaymentSid, pay.Sid);
        Assert.Equal(CallSid, pay.CallSid);
    }

    // -----------------------------------------------------------------------
    // Calls.UpdatePaymentAsync — Status=complete and Capture=security-code
    // -----------------------------------------------------------------------

    [Fact]
    public async Task CallsUpdatePayment_StatusComplete_PostsForm()
    {
        var handler = new MockHandler(req =>
        {
            Assert.Equal(HttpMethod.Post, req.Method);
            Assert.EndsWith($"/Calls/{CallSid}/Payments/{PaymentSid}.json", req.RequestUri!.AbsolutePath);
            var body = req.Content!.ReadAsStringAsync().Result;
            var form = ParseForm(body);
            Assert.Equal("complete", form["Status"]);
            Assert.False(form.ContainsKey("Capture"));

            return Reply(HttpStatusCode.Accepted, $$"""
                {"sid":"{{PaymentSid}}","account_sid":"{{Sid}}","call_sid":"{{CallSid}}",
                 "api_version":"2010-04-01",
                 "date_created":"Mon, 01 Jun 2026 12:00:00 +0000",
                 "date_updated":"Mon, 01 Jun 2026 12:05:00 +0000",
                 "uri":"/x"}
                """);
        });
        using var client = NewClient(handler);
        var pay = await client.Calls.UpdatePaymentAsync(CallSid, PaymentSid, new UpdatePaymentRequest
        {
            Status = PaymentSessionStatus.Complete,
        });
        Assert.Equal(PaymentSid, pay.Sid);
    }

    [Fact]
    public async Task CallsUpdatePayment_CaptureSecurityCode_PostsForm()
    {
        var handler = new MockHandler(req =>
        {
            var body = req.Content!.ReadAsStringAsync().Result;
            var form = ParseForm(body);
            Assert.Equal("security-code", form["Capture"]);
            Assert.False(form.ContainsKey("Status"));
            return Reply(HttpStatusCode.Accepted, $$"""
                {"sid":"{{PaymentSid}}","account_sid":"{{Sid}}","call_sid":"{{CallSid}}",
                 "api_version":"2010-04-01",
                 "date_created":"Mon, 01 Jun 2026 12:00:00 +0000",
                 "date_updated":"Mon, 01 Jun 2026 12:05:00 +0000",
                 "uri":"/x"}
                """);
        });
        using var client = NewClient(handler);
        var pay = await client.Calls.UpdatePaymentAsync(CallSid, PaymentSid, new UpdatePaymentRequest
        {
            Capture = PaymentCapture.SecurityCode,
        });
        Assert.Equal(PaymentSid, pay.Sid);
    }

    // -----------------------------------------------------------------------
    // Messages.IterateAsync — two-page pagination
    // -----------------------------------------------------------------------

    [Fact]
    public async Task MessagesIterateAsync_TwoPages_YieldsAllItems()
    {
        var requestCount = 0;
        var handler = new MockHandler(req =>
        {
            requestCount++;
            var url = req.RequestUri!.ToString();
            if (!url.Contains("Page=1"))
            {
                return Reply(HttpStatusCode.OK, $$"""
                    {"messages":[
                        {"sid":"SM01","account_sid":"{{Sid}}","api_version":"2010-04-01","to":"+18005550000","from":"+18005551234","body":"a","status":"sent","num_segments":"1","num_media":"0","direction":"outbound-api","date_created":"x","date_updated":"x","uri":"/x"},
                        {"sid":"SM02","account_sid":"{{Sid}}","api_version":"2010-04-01","to":"+18005550000","from":"+18005551234","body":"b","status":"sent","num_segments":"1","num_media":"0","direction":"outbound-api","date_created":"x","date_updated":"x","uri":"/x"}
                    ],"page":0,"page_size":2,"next_page_uri":"/2010-04-01/Accounts/{{Sid}}/Messages.json?Page=1&PageSize=2"}
                    """);
            }
            return Reply(HttpStatusCode.OK, $$"""
                {"messages":[
                    {"sid":"SM03","account_sid":"{{Sid}}","api_version":"2010-04-01","to":"+18005550000","from":"+18005551234","body":"c","status":"sent","num_segments":"1","num_media":"0","direction":"outbound-api","date_created":"x","date_updated":"x","uri":"/x"}
                ],"page":1,"page_size":2,"next_page_uri":null}
                """);
        });
        using var client = NewClient(handler);
        var collected = new List<Message>();
        await foreach (var m in client.Messages.IterateAsync(pageSize: 2))
        {
            collected.Add(m);
        }
        Assert.Equal(3, collected.Count);
        Assert.Equal("SM01", collected[0].Sid);
        Assert.Equal("SM02", collected[1].Sid);
        Assert.Equal("SM03", collected[2].Sid);
        Assert.Equal(2, requestCount);
    }

    // -----------------------------------------------------------------------
    // Helpers (copied locally to keep this test file self-contained)
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
