# 📞 VoiceML .NET SDK

The official C# / .NET client for the [VoiceML REST API](https://voicetel.com/docs/api/v0.6/voiceml/) — Twilio-compatible outbound voice and answering-machine-detection from VoiceTel, with strongly-typed, `async`/`await`-friendly .NET.

![Version](https://img.shields.io/badge/version-0.7.0-blue)
![.NET](https://img.shields.io/badge/.NET-8.0-512BD4)
![License](https://img.shields.io/badge/license-MIT%20%2B%20Commons%20Clause-green)
![Tests](https://img.shields.io/badge/tests-55%20xunit-brightgreen)
![Typed](https://img.shields.io/badge/typed-nullable%20refs-blue)

## 📚 Table of Contents

- [Features](#-features)
- [Installation](#-installation)
- [Quickstart](#-quickstart)
- [Authentication](#-authentication)
- [Resource Reference](#-resource-reference)
- [Error Handling](#-error-handling)
- [Async Support](#-async-support)
- [Pagination](#-pagination)
- [Migration from Twilio.net](#-migration-from-twilionet)
- [Rate Limits](#-rate-limits)
- [Development](#-development)
- [API Documentation](#-api-documentation)
- [Contributors](#-contributors)
- [Sponsors](#-sponsors)
- [License](#-license)

## ✨ Features

### 🛡️ Strongly Typed End-to-End
- **Native C# request/response models** for every one of the 81 API operations across 9 resource families — serialized with `System.Text.Json`, no reflection-driven binding surprises.
- **Nullable reference types** enabled throughout — distinguish "not set" from "empty" cleanly when PATCH-ing.
- **`CancellationToken` everywhere.** Every async method takes a `CancellationToken` as the last argument; cancellation and timeouts propagate cleanly down to the HTTP layer.
- **Twilio-compatible wire shapes** — `AccountSid`, `From`, `To`, status callbacks, pagination envelopes — match what Twilio's Programmable Voice API documents.

### ⚡ Async-First Surface
- Every I/O method is `async` and returns `Task<T>` — there is no blocking variant to accidentally pick.
- Page iterators expose `IAsyncEnumerable<T>` so you can `await foreach` across all pages of a list.
- Built on `System.Net.Http.HttpClient` — zero third-party dependencies, HTTP/2 ready, bring-your-own `HttpClient` for connection pooling or instrumentation.

### 🔁 Production-Grade Transport
- **Automatic retry** with exponential backoff on 429 / 5xx and transient transport errors — honors `Retry-After` headers, capped at 8s.
- **Configurable timeouts and retry budget** via `ClientOptions` (immutable `record`).
- **HTTP Basic auth** with `AccountSid:ApiKey` — exactly what the Twilio SDK uses, so existing credentials work unchanged. `AuthToken` is accepted as a migration-compatible alias.
- **Structured exception hierarchy** rooted at `VoiceMLException` / `ApiException` — `RateLimitException`, `AuthenticationException`, `NotFoundException`, etc. — catch broadly or narrowly.

### 📞 Complete API Coverage
- **Calls** — originate, fetch, terminate, update + per-call recordings, streams, SIPREC, transcriptions, notifications, events, user-defined messages, and the `/Calls/{sid}/Payments` lifecycle (Pay TwiML companion).
- **Conferences** — list, fetch, end conferences, plus participants (mute / hold / kick) and conference-scoped recordings.
- **Queues** — create, list, update, delete, peek, dequeue (front or specific member).
- **Applications** — CRUD on stored TwiML + callback bundles.
- **Recordings** — account-wide list, metadata fetch, audio fetch (follows S3 redirect), delete.
- **Messages** — create, fetch, list (To/From/DateSent filters + pagination), update (Body redaction; `Status=canceled`), delete.
- **IncomingPhoneNumbers** — list (with `PhoneNumber` exact-match lookup), create (claim/rebind), fetch, update voice routing, release.
- **Notifications** — fetch, list.
- **Diagnostics** — `/health` deep probe, `/openapi.json`.

### 🧪 Tested
- **55 xUnit tests** — conformance, smoke, and Messages/Payments suites — exercising every resource and error path with mocked `HttpMessageHandler` (no network in unit tests).
- **`dotnet build` clean** with `TreatWarningsAsErrors=true` on `net8.0`.

### 📦 Clean Distribution
- Zero codegen footprint — every byte hand-written.
- Single NuGet package: `VoiceML`.
- Ships with symbol package (`.snupkg`) for step-into debugging.

## 🚀 Installation

```bash
dotnet add package VoiceML --version 0.7.0
```

Or `<PackageReference>` it directly:

```xml
<PackageReference Include="VoiceML" Version="0.7.0" />
```

Targets **.NET 8.0**.

## 🏁 Quickstart

```csharp
using VoiceML;
using VoiceML.Models;

using var client = new VoiceMLClient(new ClientOptions
{
    AccountSid = "AC...",
    ApiKey     = "your-api-key",
});

var call = await client.Calls.CreateAsync(new CreateCallRequest
{
    To               = "+18005551234",
    From             = "+18005550000",
    Url              = "https://example.com/twiml.xml",
    MachineDetection = "DetectMessageEnd",
});

Console.WriteLine($"placed call: {call.Sid} ({call.Status})");

var queues = await client.Queues.ListAsync();
foreach (var q in queues.Queues)
{
    Console.WriteLine($"{q.FriendlyName} ({q.CurrentSize})");
}
```

## 🔑 Authentication

Every endpoint uses **HTTP Basic** with your `AccountSid` as the username and your per-tenant API key as the password — identical to Twilio's auth shape, so credentials issued for Twilio code work here unchanged.

```csharp
using var client = new VoiceMLClient(new ClientOptions
{
    AccountSid = "AC...",
    ApiKey     = "...",   // or AuthToken — migration-compatible alias; set only one
});

var health = await client.Diagnostics.HealthAsync();
```

> Don't have credentials yet? See **[voicetel.com/docs/api/v0.6/voiceml/](https://voicetel.com/docs/api/v0.6/voiceml/)** for issuance and rotation.

`ClientOptions` is an immutable `record`:

```csharp
new ClientOptions
{
    AccountSid = "AC...",
    ApiKey     = "...",
    Timeout    = TimeSpan.FromSeconds(30),         // default
    MaxRetries = 2,                                // default; 0 disables retries
    UserAgent  = "my-app/1.0",                     // optional override
    HttpClient = injectedClient,                   // optional — reuse an existing HttpClient
    Logger     = msg => Console.WriteLine(msg),    // optional log sink
};
```

When you supply your own `HttpClient`, the SDK does **not** dispose it — your code retains lifetime ownership.

## 🗺️ Resource Reference

| Resource | Async methods | Covers |
|---|---|---|
| `client.Calls` | originate, fetch, list, terminate, update | + per-call recordings, streams, SIPREC, transcriptions, notifications, events, user-defined messages, payments |
| `client.Conferences` | list, fetch, end | participants (mute / hold / kick), conference-scoped recordings |
| `client.Queues` | create, list, update, delete | peek, dequeue (front or specific member) |
| `client.Applications` | CRUD on TwiML + callback bundles | |
| `client.Recordings` | account-wide list, metadata, audio fetch, delete | follows S3 redirect for audio |
| `client.Messages` | create, fetch, list, update, delete | To/From/DateSent filters; Body redaction; `Status=canceled` |
| `client.IncomingPhoneNumbers` | list, fetch, update | exact-match `PhoneNumber` lookup; claim/rebind; release |
| `client.Notifications` | fetch, list | |
| `client.Diagnostics` | `HealthAsync`, OpenAPI spec | |

Every method that takes a request body accepts a typed model from `VoiceML.Models`:

```csharp
using VoiceML;
using VoiceML.Models;

using var client = new VoiceMLClient(new ClientOptions { AccountSid = "AC...", ApiKey = "..." });

var call = await client.Calls.CreateAsync(new CreateCallRequest
{
    To   = "+18005551234",
    From = "+18005550000",
    Url  = "https://example.com/twiml.xml",
});

// On a live call, open a Pay session:
var session = await client.Calls.StartPaymentAsync(call.Sid, new StartPaymentRequest
{
    IdempotencyKey = "order-482917",
    StatusCallback = "https://example.com/pay-status",
});
Console.WriteLine($"{session.Sid} {session.Status}");
```

## 🚨 Error Handling

All exceptions inherit from `VoiceMLException`. `ApiException` is the catch-all for non-2xx responses; specific subclasses cover the common status families:

| Status | Exception                       |
|--------|---------------------------------|
| 400    | `BadRequestException`           |
| 401    | `AuthenticationException`       |
| 403    | `PermissionDeniedException`     |
| 404    | `NotFoundException`             |
| 409    | `ConflictException`             |
| 410    | `GoneException`                 |
| 429    | `RateLimitException`            |
| 501    | `NotImplementedAPIException`    |
| 5xx    | `ServerException`               |
| other  | `ApiException`                  |

Each carries `StatusCode`, `Code` (numeric Twilio code, when present), `MoreInfo` (documentation URL from the error body, when present), and `Body` (parsed JSON or raw string).

```csharp
using VoiceML;
using VoiceML.Exceptions;

try
{
    var call = await client.Calls.GetAsync("CA0000000000000000000000000000aaaa");
}
catch (NotFoundException)
{
    Console.WriteLine("That call isn't on your account.");
}
catch (RateLimitException ex)
{
    Console.WriteLine($"Slow down — server said {ex.Code} / {ex.MoreInfo}");
}
```

Configuration problems (missing `AccountSid`, both `ApiKey` and `AuthToken` set, etc.) raise `ConfigurationException` at construction time — before any HTTP request leaves the process.

## ⚡ Async Support

Every I/O method on the SDK is `async` and returns `Task<T>` (or `Task` for void). There is no blocking variant — async is the only surface. Every method accepts a `CancellationToken` as its last argument:

```csharp
using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(5));

var calls = await client.Calls.ListAsync(
    new ListCallsParams { Status = "in-progress" },
    cts.Token);

foreach (var call in calls.Calls)
{
    Console.WriteLine($"{call.Sid} {call.Duration}s");
}
```

## 📄 Pagination

List operations return a `…List` model with a Twilio-compatible pagination envelope (`Page`, `PageSize`, `Total`, `NextPageUri`, `PreviousPageUri`, …). For `/Calls`, `/Messages`, and call-scoped `/Recordings`, the SDK exposes `IAsyncEnumerable<T>` iterators that transparently walk all pages:

```csharp
await foreach (var call in client.Calls.IterateAsync(
    new ListCallsParams { Status = "completed", PageSize = 200 }))
{
    Process(call);
}

await foreach (var msg in client.Messages.IterateAsync(
    new ListMessagesParams { From = "+18005550000", PageSize = 200 }))
{
    Archive(msg);
}
```

For other resources, page manually with `await client.<Resource>.ListAsync(new ...Params { Page = n })`.

Twilio uses literal query-parameter names `StartTime>=` / `StartTime<=`. The SDK sends them verbatim on the wire; in model code they are surfaced as `StartTimeGte` / `StartTimeLte` on `ListCallsParams`.

## 🔁 Migration from Twilio.net

The `AccountSid` + `ApiKey` pair the official Twilio SDK initializes from works unchanged here:

```csharp
// Before — Twilio.net
using Twilio;
using Twilio.Rest.Api.V2010.Account;
TwilioClient.Init("AC...", "<token>");
var call = CallResource.Create(
    to: new PhoneNumber("+18005551234"),
    from: new PhoneNumber("+18005550000"),
    url: new Uri("https://example.com/twiml.xml"));

// After — VoiceML (Twilio-compatible)
using VoiceML;
using VoiceML.Models;
using var client = new VoiceMLClient(new ClientOptions
{
    AccountSid = "AC...",
    ApiKey     = "<api-key>",   // or AuthToken — migration-compatible alias
});
var call = await client.Calls.CreateAsync(new CreateCallRequest
{
    To   = "+18005551234",
    From = "+18005550000",
    Url  = "https://example.com/twiml.xml",
});
```

Method names follow the resource map above (`client.Calls.CreateAsync(...)`, `client.Queues.ListAsync()`, …) rather than Twilio's static `CallResource.Create(...)` style — instance-based, disposable, and async-first, with the same wire format on the way out.

## ⏱️ Rate Limits

VoiceML applies per-tenant rate limits at the edge. The SDK automatically retries 429 responses with `Retry-After` honored, plus 500 / 502 / 503 / 504 and transient transport errors, up to `MaxRetries` (default `2`). Backoff is exponential (0.5s, 1s, 2s, …, capped at 8s). To bump it:

```csharp
new ClientOptions
{
    AccountSid = "AC...",
    ApiKey     = "...",
    MaxRetries = 4,
    Timeout    = TimeSpan.FromSeconds(60),
};
```

Setting `MaxRetries = 0` disables retries entirely.

## 🛠️ Development

```bash
git clone https://github.com/voicetel/voiceml-csharp-sdk
cd voiceml-csharp-sdk

# Restore + build (clean, TreatWarningsAsErrors=true)
dotnet restore
dotnet build

# Unit tests (fast, mocked HttpMessageHandler — no network)
dotnet test

# Build the NuGet package + symbol package
dotnet pack -c Release
```

## 📖 API Documentation

- **Reference docs:** [voicetel.com/docs/api/v0.6/voiceml/](https://voicetel.com/docs/api/v0.6/voiceml/)
- **Validator:** [voicetel.com/voiceml/validator/](https://voicetel.com/voiceml/validator/)
- **SDK catalogue:** [voicetel.com/docs/voiceml-sdks/](https://voicetel.com/docs/voiceml-sdks/)
- **Type definitions:** see the `VoiceML.Models` namespace — every wire shape has a record / class.

## 🙌 Contributors

- [Michael Mavroudis](https://github.com/mavroudis) — Lead Developer

Contributions welcome. Open an issue describing the change you want to make, or send a pull request against `main`.

## 💖 Sponsors

| Sponsor | Contribution |
|---------|--------------|
| [VoiceTel Communications](https://voicetel.com) | Primary development and production hosting |

## 📄 License

MIT with the Commons Clause restriction. See [LICENSE](LICENSE) and [voicetel.com/legal/](https://voicetel.com/legal/).
