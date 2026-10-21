# VoiceML .NET SDK

Official asynchronous .NET client for VoiceML, VoiceTel's Twilio-compatible voice and messaging API.

- SDK version **0.9.5** (API spec 0.9.4). Targets **.NET 8 and .NET 10**; .NET 10 is recommended for new applications. .NET 8 support ends on 2026-11-10.
- The committed public-spec contract tests exercise **333 of 333 operations** through mocked HTTP, including host routing, authentication, request keys, required parameters and nullable responses.
- Typed resources, nullable reference annotations, cancellation tokens and lazy asynchronous pagination. Future response enum values are retained as strings and unknown response fields are ignored.
- [API documentation](https://voicetel.com/docs/api/v0.9/voiceml/) · [Source](https://github.com/voicetel/voiceml-csharp-sdk) · [Changes](CHANGELOG.md)

## Installation

```sh
dotnet add package VoiceML
```

[`VoiceML`](https://www.nuget.org/packages/VoiceML) is published on nuget.org by the `voicetel` organization, from this repository's publish workflow using trusted publishing. Packages target `net8.0` and `net10.0` and include the repository's **MIT with Commons Clause** license text, XML API documentation, deterministic assemblies, SourceLink information and a symbol package. There are no runtime NuGet dependencies.

To build from source instead, for example to try unreleased changes, see [installation details](docs/installation.md).

## Quick start

Use your **VoiceML AccountSid and VoiceML API key** in the HTTP Basic username/password positions. `AuthToken` is an alias for `ApiKey`; configuring both raises `ConfigurationException`. Credentials issued for Twilio do not authenticate to VoiceML.

This complete example is maintained in `examples/Quickstart/Program.cs` and compiled in CI. It uses fictional example numbers; supply numbers assigned to your account when placing a call.

```csharp
using System;
using VoiceML;
using VoiceML.Exceptions;
using VoiceML.Models;

var accountSid = Environment.GetEnvironmentVariable("VOICEML_ACCOUNT_SID")
    ?? throw new InvalidOperationException("Set VOICEML_ACCOUNT_SID");
var apiKey = Environment.GetEnvironmentVariable("VOICEML_API_KEY")
    ?? throw new InvalidOperationException("Set VOICEML_API_KEY");

// Keep this client for the application lifetime; dispose it at shutdown.
using var client = new VoiceMLClient(new ClientOptions
{
    AccountSid = accountSid,
    ApiKey = apiKey,
    Timeout = TimeSpan.FromSeconds(30),
});

try
{
    var call = await client.Calls.CreateAsync(new CreateCallRequest
    {
        To = "+14155550101",
        From = "+14155550100",
        Url = "https://example.com/answer.xml",
    });
    Console.WriteLine(call.Sid);

    await foreach (var completed in client.Calls.IterateAsync(
        new ListCallsParams { Status = "completed", PageSize = 50 }))
    {
        Console.WriteLine(completed.Sid);
        break; // No later page is fetched after the consumer stops.
    }
}
catch (VoiceMLTimeoutException error)
{
    Console.Error.WriteLine(error.Message);
}
catch (ApiException error)
{
    Console.Error.WriteLine($"HTTP {error.StatusCode}; code {error.Code}: {error.Message}");
}
```

### Client and HTTP lifecycle

Reuse one client per account/configuration in a long-lived application. The SDK-owned `SocketsHttpHandler` recycles pooled connections after five minutes so DNS changes take effect. Requests and discarded responses are disposed on every attempt.

You may supply `ClientOptions.HttpClient`, including a client managed by `IHttpClientFactory`; its timeout and proxy settings apply, and the SDK does not dispose it. Configure that client's handler with `AllowAutoRedirect = false` so the SDK can inspect and enforce redirect policy. A handler that follows redirects internally hides those hops from the SDK.

Recording audio and archive downloads authenticate their first API request, manually follow redirects, drop Authorization on a host/scheme/port change, and reject HTTPS-to-HTTP redirects. They use the configured HTTP client and timeout. `Recordings.GetAudioAsync` returns bytes and content type; `Exports.DownloadAsync("2026-10-01")` does the same for an archive, on a route without a `.json` suffix.

## Resources

| Client surface | Operations |
|---|---|
| `Calls` | List, fetch, originate, update, delete; call recordings; media-stream, SIPREC and real-time-transcription **start/stop**; payments, notifications and events |
| `Conferences` | List, fetch, end; participant management; recording list/start/fetch/update/delete |
| `Recordings` | Account-wide list, metadata, audio and delete; `Transcriptions(recordingSid)` list/fetch/delete |
| `Messages` | SMS/MMS list/create/fetch/update/delete; `Media(messageSid)` list/fetch/delete; delivery-feedback create |
| `Applications` | Application bundles and message delivery-receipt callback configuration |
| `IncomingPhoneNumbers` | Async-only list/create/fetch/update/delete and Local/Mobile/TollFree list/create |
| `Queues` | Queue management and waiting-member operations |
| `Accounts`, `Balance` | Authenticated-account fetch/update and API key rotation; account balance fetch |
| `OutgoingCallerIds` | Verified caller-ID list/create/fetch/update/delete |
| `Transcriptions` | Account-wide recording-transcription list/fetch/delete |
| `Exports` | List daily archives and download an archive by date |
| `UsageRecords` | List and AllTime, Daily, LastMonth, Monthly, ThisMonth, Today, Yearly, Yesterday windows |
| `Sip` | Domains, credential lists, access-control lists and mappings; `Registrations.ListAsync` |
| `RoutesV2` | Inbound processing regions for phone numbers and SIP domains |
| `VoiceV1` | BYOC trunks, connection policies/targets, IP records, source-IP mappings, dialing permissions |
| `ConversationsV1` | Conversations, messages, participants, receipts, webhooks, users, roles, credentials, configuration and service-scoped resources |
| `MessagingV1.Services` | Messaging Services and `PhoneNumbers(serviceSid)` sender-pool list/create/fetch/delete |
| `AssistantsV1` | Assistants, tools, knowledge/chunks/status, sessions/messages, feedback and policies |
| `Pricing` | Voice, messaging, phone-number and trunking rate lookups |
| `MonitorAlerts` | Monitor v1 alerts: single-page list with `LogLevel`/`StartDate`/`EndDate`/`PageSize` filters, and fetch with stored request/response detail |
| `Notifications` | Account notification compatibility endpoints |
| `Diagnostics` | Health and OpenAPI JSON/YAML/YML, without authentication |

### Messages and routing configuration

`CreateMessageRequest` accepts optional `Body` and repeated `MediaUrl` values: provide text, at least one media URL, or both. It also exposes `ValidityPeriod`, `StatusCallback` and `StatusCallbackMethod`.

Incoming phone-number create/update requests expose `SmsUrl`, `SmsMethod`, `SmsFallbackUrl`, `SmsFallbackMethod`, `SmsApplicationSid`, `VoiceApplicationSid`, `StatusCallback` and `StatusCallbackMethod`. The Local/Mobile/TollFree creates use `CreateTypedIncomingPhoneNumberOptions`, which contains only the phone number and primary voice URL/method declared for those routes. Application create/update requests expose `MessageStatusCallback` and its legacy alias `SmsStatusCallback`.

Calls support `StartTime`, `StartTimeLt`, `StartTimeGt` and the corresponding EndTime filters. Use UTC ISO-8601 timestamps, or `YYYY-MM-DD` for date-only filters. The API does not declare inclusive `StartTimeGte`/`StartTimeLte` request properties. Booleans use lowercase `true`/`false`; lists use repeated form keys; JSON-valued form fields such as `Attributes` take compact JSON strings.

`Messages.CreateFeedbackAsync(messageSid, request)` submits delivery feedback for a message; `Outcome` accepts `confirmed` or `unconfirmed`, and any other value (including none) is echoed as `unconfirmed`.

### Account API key rotation

`Accounts.RotateKeyAsync(accountSid)` calls `POST /2010-04-01/Accounts/{AccountSid}/RotateKey.json` with no body. The previous key stops working immediately; the new key is returned exactly once in `AccountWithKey.ApiKey` and no read endpoint exposes it, so store it right away. The response record redacts the key from its default string rendering (`ToString`), but treat it as secret material. A concurrent account modification raises `ConflictException` (409).

Monitor v1 alerts (`client.MonitorAlerts`) list failure events with `LogLevel`, `StartDate`, `EndDate` and `PageSize` filters. Alerts are a single `PageSize`-limited page on the `/v1` `meta` envelope with no continuation; `GetAsync(sid)` returns the stored request/response detail when present.

Conversations requests use the derived `conversations.voicetel.com` product host; Messaging Services and their sender pools use `messaging.voicetel.com`. Other products use the configured base host. `ConversationsBaseUrl` and `MessagingBaseUrl` provide explicit overrides for custom deployments.

### Pagination

Account-scoped lists expose `PageNumber`, `PageSize`, `Start`, `End`, `Uri`, `FirstPageUri`, `PreviousPageUri` and `NextPageUri`. There is no total/page-count field. Iterators follow `next_page_uri` exactly, including the server-issued `PageToken`, and terminate when the link is absent, null or empty. The initial filter's `PageSize` controls the first request. Iterators are lazy and do not accumulate all pages in memory.

Use the resource's `NextPageAsync(page)` helper, or the shared `FetchNextPageAsync(page)` helper on account-scoped resources, for explicit page traversal. `/v1` list helpers follow `Meta.NextPageUrl` only when its scheme, host and port match the configured product origin; a foreign origin raises `VoiceMLException` before credentials are sent.

### Retry and error policy

The default `MaxRetries` is **2**, for at most three attempts. Set it to zero to disable retries.

| Method | Retried failures |
|---|---|
| GET, HEAD, PUT, DELETE | 429, 500, 502, 503, 504; connect/read failures and timeouts |
| POST, PATCH | 429; provable connection-establishment failures such as DNS failure, refused connection or TLS handshake failure |

POST/PATCH requests are not replayed after server errors, read timeouts or connection resets. The SDK does not assume an idempotency key makes replay safe. Caller cancellation propagates unchanged and is never retried.

`Retry-After` supports delta-seconds and HTTP dates. A value above **60 seconds** raises the rate-limit error immediately, exposing the parsed value as `RateLimitException.RetryAfter` and `RetryAfterSeconds`. Other retries use exponential backoff with full jitter: **0.5 seconds base, 8 seconds maximum**.

HTTP failures derive from `ApiException`, with `StatusCode`, nullable numeric `Code`, `CodeRaw`, body `Status`, `MoreInfo` and `Body`. Status-specific subclasses include `BadRequestException`, `AuthenticationException`, `ConflictException`, `RateLimitException` and `ServerException`. Account optimistic-lock conflicts map to `ConflictException` (409 / 20409). Non-JSON errors remain SDK exceptions; malformed successful payloads say “could not decode” and retain the underlying `JsonException`. Timeouts use `VoiceMLTimeoutException`; configuration errors use `ConfigurationException`.

### Webhook signatures

Use `RequestValidator.ValidateRequest(apiKey, url, formPairs, signature)` to verify `X-Twilio-Signature`; `GetSignature` computes signatures for tests. Supply the full externally visible URL and received form pairs. For GET callbacks pass an empty pair sequence. Repeated keys retain received value order; keys are sorted by UTF-8 bytes, fragments are excluded, and comparison is constant-time. URLs with the default port added or removed are accepted. VoiceML signs form callbacks and URLs, not JSON bodies.

### Diagnostics without credentials

`VoiceMLDiagnostics.HealthAsync(httpClient: sharedClient)` works without an account SID or key. The standalone helper also exposes `OpenApiJsonAsync`, `OpenApiYamlAsync` and `OpenApiYmlAsync`. The same methods are available at `client.Diagnostics`; neither surface sends Basic authentication. All requests use the normal transport, timeout, User-Agent and error mapping. Health warnings/failures are optional; 503 raises `ServerException`.

## Development

With the .NET 10 SDK and .NET 8 runtime installed:

```sh
dotnet build -c Release
dotnet test -c Release
dotnet format whitespace --verify-no-changes --no-restore
dotnet pack -c Release
dotnet list package --vulnerable --include-transitive
dotnet build examples/Quickstart/Quickstart.csproj -c Release
```

Both library and tests treat warnings as errors. The vendored public spec is under `tests/VoiceML.Tests/Fixtures` and is excluded from the package. The optional private response-fixture suite reports an explicit skip unless `VOICEML_CONFORMANCE_FIXTURES` points to the corpus; the public-spec contract suite always runs.

## License

MIT with Commons Clause. See [LICENSE](LICENSE) for the complete terms.
