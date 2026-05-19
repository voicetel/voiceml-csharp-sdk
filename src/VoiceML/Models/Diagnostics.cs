namespace VoiceML.Models;

// HealthStatus / HealthFailure live in Common.cs so they're available alongside the other
// shared types. This file is intentionally light; it exists so the file layout matches the
// SDK-layout spec (Diagnostics.cs as a peer to Calls.cs / Conferences.cs / etc.) and gives
// callers an obvious place to add future diagnostic models without churning Common.cs.
