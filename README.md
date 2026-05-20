# VoiceML — .NET SDK

Official .NET SDK for [VoiceML](https://voicetel.com/docs/api/v0.6/voiceml/), VoiceTel's outbound voice + AMD
service with a Twilio-shaped REST surface.

- **Target framework:** `net8.0`
- **Dependencies:** none (the SDK ships with zero NuGet dependencies — uses `System.Net.Http` and
  `System.Text.Json` from the BCL).
- **Auth:** HTTP Basic. Username = your Twilio-format `AccountSid` (`AC…`); password = your API key.
- **Wire shape:** Twilio-compatible. Request bodies are `application/x-www-form-urlencoded`;
  responses are JSON.

## Install

```sh
dotnet add package VoiceML --version 0.5.0
```

## Quick start

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
    To   = "+18005551234",
    From = "+18005550000",
    Url  = "https://example.com/twiml.xml",
});

Console.WriteLine($"placed call: {call.Sid} ({call.Status})");

var page = await client.Calls.ListAsync(new ListCallsParams
{
    StartTimeGte = "2025-01-01T00:00:00Z",
    Status       = "completed",
    PageSize     = 50,
});

foreach (var c in page.Calls)
{
    Console.WriteLine($"{c.Sid} {c.From} -> {c.To} ({c.Duration}s)");
}
```

## Resources

- `client.Calls` — originate, fetch, update (terminate / redirect), list, delete. Includes
  call-scoped sub-resources for recordings, streams, SIPREC, transcriptions, notifications,
  events, and user-defined messages.
- `client.Conferences` — list, fetch, end; participants (list, fetch, update mute/hold, kick);
  conference recordings.
- `client.Queues` — CRUD; queue-member operations (list, peek-front, dequeue-front, fetch by call,
  dequeue by call).
- `client.Applications` — CRUD for persistent TwiML+callback bundles.
- `client.Recordings` — account-scoped list, fetch metadata, fetch WAV audio, delete.
- `client.IncomingPhoneNumbers` — tenant-self-serve DID management: list (with `PhoneNumber`
  exact-match lookup), create (claim/rebind), fetch, update voice routing, release.
- `client.Diagnostics` — `/health` and `/openapi.json`.

## Errors

All exceptions inherit from `VoiceMLException`. `ApiException` is the catch-all for non-2xx
responses; specific subclasses cover the common status families:

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

Each carries `StatusCode`, `Code` (numeric Twilio code, when present), `MoreInfo`
(documentation URL from the error body, when present), and `Body` (parsed JSON or raw string).

## Configuration

`ClientOptions` is an immutable `record`:

```csharp
new ClientOptions
{
    AccountSid = "AC...",
    ApiKey     = "...",                              // or AuthToken — Twilio-shape alias; set only one
    BaseUrl    = "https://voiceml.voicetel.com",   // default
    Timeout    = TimeSpan.FromSeconds(30),          // default
    MaxRetries = 2,                                  // default; 0 disables retries
    UserAgent  = "my-app/1.0",                       // optional override
    HttpClient = injectedClient,                     // optional — reuse an existing HttpClient
    Logger     = msg => Console.WriteLine(msg),      // optional log sink
};
```

Retries are applied on `429`, `500`, `502`, `503`, `504`, and transient transport errors. The
backoff is exponential (0.5s, 1s, 2s, …, capped at 8s) and honors `Retry-After` when present.

## Pagination

Twilio uses literal query-parameter names `StartTime>=` / `StartTime<=`. The SDK sends them
verbatim on the wire; in model code they are surfaced as `StartTimeGte` / `StartTimeLte` on
`ListCallsParams`.

## Building locally

```sh
dotnet restore
dotnet build
dotnet test
dotnet pack -c Release
```

## 📖 API Documentation

- **Reference docs:** [voicetel.com/docs/api/v0.6/voiceml/](https://voicetel.com/docs/api/v0.6/voiceml/)
- **Validator:** [voicetel.com/voiceml/validator/](https://voicetel.com/voiceml/validator/)
- **SDK catalogue:** [voicetel.com/docs/voiceml-sdks/](https://voicetel.com/docs/voiceml-sdks/)

## License

MIT with Commons Clause. See `LICENSE`.
