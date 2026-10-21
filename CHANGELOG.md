# Changelog

All notable changes are recorded here in Keep a Changelog format.

## [0.9.5] - 2026-10-21

### Added

- Calls create accepts `TimeLimit` — max call duration in seconds. An explicit `0` or a value above `14400` is rejected (error 13216); a negative value disables the limit.
- Conference participant create accepts `ConferenceStatusCallback`, `ConferenceStatusCallbackMethod` and `ConferenceStatusCallbackEvent` for the participant's conference lifecycle webhook.
- Conference-recording updates accept `PauseBehavior` (`skip`/`silence`), mirroring call-recording updates.
- Application create and update accept `SmsUrl`, `SmsMethod`, `SmsFallbackUrl`, `SmsFallbackMethod` and `PublicApplicationConnectEnabled`.
- Message create accepts `SendAt` (RFC 3339 UTC) and `ScheduleType` (only `fixed` is accepted; both require a `MessagingServiceSid`) for scheduled sends.
- Call-transcription start accepts `SpeechModel` (engine-specific model hint), `EnableAutomaticPunctuation` and `IntelligenceService` (accepted for compatibility; VoiceML has no Conversation Intelligence, so a set value logs a 32655 Notification and transcription proceeds).

### Changed

- `StartTranscriptionRequest.StatusCallback` is renamed to `StatusCallbackUrl`, matching the wire name the server reads (the old wire name was never honored). `StatusCallbackMethod` and `StatusCallbackEvents` are unchanged.

## [0.9.4] - 2026-10-21

### Added

- IncomingPhoneNumbers create and update accept `VoiceApplicationSid`, `StatusCallback` and `StatusCallbackMethod`; OutgoingCallerIds create accepts `CallDelay`, `Extension`, `StatusCallback` and `StatusCallbackMethod`.

### Docs

- `VoiceML` 0.9.3 is now on nuget.org (published 2026-10-19 by the `voicetel` organization through trusted publishing). README, `docs/installation.md` and SECURITY.md now give `dotnet add package VoiceML` as the install path and keep source builds for unreleased changes. The README version line now reads SDK 0.9.3 (API spec 0.9.2).

### CI

- The local-feed smoke test reads the package version from the project file instead of a hard-coded version.

## [0.9.3] - 2026-10-18

### Breaking changes

- Remove GET `/2010-04-01/Accounts/{AccountSid}/Calls/{CallSid}/Streams.json` and `/Streams/{Sid}.json`, `/Siprec.json` and `/Siprec/{Sid}.json`, and `/Transcriptions.json` and `/Transcriptions/{Sid}.json`. The server returns 405 / error 20004 for these six endpoints. POST start/stop operations remain available; remove their obsolete list models.
- Remove `StartTimeGte` and `StartTimeLte` from calls-list parameters. Use the declared `StartTime`, `StartTimeLt`, `StartTimeGt` and EndTime equivalents.
- Call-recording lists take `ListCallRecordingsParams`; account-only CallSid/ConferenceSid/IncludeSoftDeleted filters are not sent to `/Calls/{CallSid}/Recordings.json`.
- Local/Mobile/TollFree incoming-number creates take `CreateTypedIncomingPhoneNumberOptions`; those endpoints do not declare VoiceFallbackUrl/VoiceFallbackMethod. Six Assistant list endpoints now take PageSize-only parameters.
- Conference-recording updates take `UpdateConferenceRecordingRequest`; PauseBehavior belongs only to call-recording updates.
- Remove blocking IncomingPhoneNumbers List/Create/Get/Update/Delete wrappers. Use the asynchronous methods and cancellation tokens.
- Iterators accept pagination settings through their initial filter and follow continuation links thereafter. Remove total/num_pages envelope fields; RecordingList and notification/event lists use the common Page envelope.
- Make Transport and resolved credentials internal. Normalize ApiException.Code to nullable integer; expose CodeRaw and body Status. Remove the unused ErrorBody model. Notification and event endpoints return typed models.
- Mark spec-nullable fields nullable, including Participant/Application booleans. Retain unsupported legacy response fields only as optional compatibility data.

### Fixed

- Follow server-provided PageToken links verbatim; validate `/v1` continuation origins and preserve lazy enumeration.
- Restrict POST/PATCH retries to rate limits and provable connection-establishment failures. Add HTTP-date Retry-After support, a 60-second no-wait cap, and full-jitter backoff.
- Wrap timeouts and decode failures in SDK exceptions with their causes; preserve caller cancellation and account optimistic-lock conflict details.
- Encode every caller-controlled path segment and query name, retaining route-defined `.json` suffixes.
- Add MMS parameters, number SMS routing, Application delivery callbacks, RecordingTrack/PauseBehavior and missing list filters.
- Recycle pooled HTTP connections and dispose requests/responses on each attempt. Apply authenticated, credential-stripping redirects to binary downloads.

### Added

- Account fetch/update, balance, outgoing caller IDs, message media, account/recording transcriptions, archive exports, conference recording start and OpenAPI YAML/YML operations.
- Nine Usage Records windows, SIP registrations and Messaging Service sender-pool operations.
- Monitor v1 alerts: list `GET /v1/Alerts` with `LogLevel`/`StartDate`/`EndDate`/`PageSize` filters and fetch `GET /v1/Alerts/{Sid}`, both on the base host with the `/v1` `meta` envelope. Alerts are a single `PageSize`-limited page with no continuation.
- Message delivery feedback: `POST /2010-04-01/Accounts/{AccountSid}/Messages/{Sid}/Feedback.json`, echoing the `Outcome` (`confirmed`/`unconfirmed`) as the Feedback resource.
- Account API key rotation: `POST /2010-04-01/Accounts/{AccountSid}/RotateKey.json` returns the account view plus the freshly generated API key exactly once; a 409 conflict is raised as `ConflictException`.
- Public next-page helpers, standalone credential-less diagnostics and webhook signature computation/validation.
- .NET 10 alongside .NET 8, SourceLink, deterministic packaging, XML documentation for the public API, spec-driven contract tests and request/security regression coverage.

### Security

- Redact ClientOptions credentials in default string output and exclude them from JSON serialization.
- Redact the rotated account API key from `AccountWithKey` default string output so logging the rotation response never leaks it.
- Restrict pagination origins, reject redirect downgrades, strip credentials across redirect origins and avoid retrying ambiguous non-idempotent failures.

### Docs

- State that VoiceML is not yet on nuget.org and document tested local-feed/ProjectReference installation.
- Refresh resource coverage, runtime support, authentication, retry/cancellation, pagination, webhook and client-lifecycle guidance; compile the README example in CI.
