# Security Policy

## Supported versions

| Version | Supported |
|---|---|
| 0.9.x | Yes |
| Earlier versions | No |

## Reporting a vulnerability

Please use [GitHub private vulnerability reporting](https://github.com/voicetel/voiceml-csharp-sdk/security/advisories/new), rather than a public issue. Include affected versions/configuration, impact and reproduction steps. We aim to acknowledge reports within a few business days; allow reasonable time to prepare a fix before disclosure.

## Credentials and transport

- Authenticate with a VoiceML AccountSid and VoiceML API key. `AuthToken` aliases the key; Twilio-issued credentials do not work with VoiceML.
- Options redact API keys in default string output and omit them from JSON serialization. Keep credentials and Authorization headers out of application logs and source control; use an environment variable or secret manager.
- Validate callback `X-Twilio-Signature` values with `RequestValidator.ValidateRequest`, using the account API key and the externally visible URL.
- Reuse clients. SDK-owned handlers use platform TLS validation, five-minute pooled connection lifetimes and manual redirects. Custom `HttpClient` handlers should retain platform certificate validation and set `AllowAutoRedirect = false` so the SDK can inspect each redirect.
- Recording audio and archive downloads strip authentication across host/scheme/port changes and reject HTTPS-to-HTTP redirects. Pagination links are restricted to the configured product origin.
- GET/HEAD/PUT/DELETE retry 429, 500, 502, 503, 504 and transport failures. POST/PATCH retry only 429 and provable connection-establishment failures; they never retry 5xx, read timeouts or connection resets. Caller cancellation is never retried. The default is two retries.
- Retry-After supports seconds and HTTP dates. Values above 60 seconds raise the error immediately with the parsed duration exposed. Other retries use full-jitter exponential backoff (0.5 seconds base, 8 seconds ceiling).

## Package availability

The official package is [`VoiceML` on nuget.org](https://www.nuget.org/packages/VoiceML), owned by the `voicetel` organization. Releases are pushed only by this repository's `publish.yml` workflow through nuget.org trusted publishing; no long-lived NuGet API key is stored. Each package's `.nuspec` records the source commit, which you can compare with the release tag. Treat packages with similar names from other owners as unofficial.
