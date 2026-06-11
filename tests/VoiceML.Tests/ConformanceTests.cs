// Twilio response-shape conformance tests (#330 Phase C). Mirrors the
// Go (voiceml-go-sdk@d6ac75c), Python (voiceml-python-sdk), TypeScript
// (voiceml-node-sdk@a11b0a1), and Java (voiceml-java-sdk@9178659)
// harnesses: load 132 canonical Twilio response examples from
// callBroadcast's cmd/twilio-conformance-fixtures, deserialise each
// into the matching SDK model via System.Text.Json, assert key fields.
// SKIPPED unless VOICEML_CONFORMANCE_FIXTURES env points at the corpus.
//
// Strictness: System.Text.Json throws on type mismatch (e.g. JSON number
// into string property) and on unknown enum values when the target is a
// strongly-typed enum (the SDK declares CallStatus, CallDirection,
// AnsweredBy, ConferenceStatus, RecordingStatus, etc. as concrete enums
// with [JsonConverter] mapping). Required-field enforcement is in the
// post-decode asserts.
//
// Run:
//   VOICEML_CONFORMANCE_FIXTURES=/path/to/callBroadcast/cmd/twilio-conformance-fixtures/fixtures \
//     dotnet test --filter "FullyQualifiedName~ConformanceTests"

using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using VoiceML.Models;
using Xunit;

// Disambiguate against System.IO.Stream.
using Stream = VoiceML.Models.Stream;

namespace VoiceML.Tests;

public class ConformanceTests
{
    private const string FixturesEnv = "VOICEML_CONFORMANCE_FIXTURES";

    private static readonly JsonSerializerOptions JsonOpts = new()
    {
        PropertyNameCaseInsensitive = false,
        ReadCommentHandling = JsonCommentHandling.Skip,
    };

    // Operation IDs with no SDK model — same skip set as the other SDKs.
    // Messages / Notifications / Events / UserDefinedMessage decode to raw
    // containers in Go / nothing in the other SDKs; we just skip here.
    private static readonly HashSet<string> SkipOps = new()
    {
        "ListCallEvent",
        "ListCallNotification",
        "FetchCallNotification",
        "ListNotification",
        "FetchNotification",
        "CreateUserDefinedMessage",
        "CreateMessage",
        "FetchMessage",
        "ListMessage",
        "UpdateMessage",
    };

    private sealed record ConformanceEntry(
        [property: JsonPropertyName("resource")] string Resource,
        [property: JsonPropertyName("method")] string Method,
        [property: JsonPropertyName("status")] string Status,
        [property: JsonPropertyName("operation_id")] string OperationId,
        [property: JsonPropertyName("example_name")] string ExampleName,
        [property: JsonPropertyName("path")] string Path,
        [property: JsonPropertyName("file")] string File);

    // Sentinel row yielded when the fixtures env var is unset or the
    // corpus is missing. xUnit treats an empty [MemberData] source as a
    // hard failure ("No data found") rather than a skip, so this row
    // gives the theory a single iteration that no-ops cleanly.
    private const string FixturesUnsetSentinel = "__fixtures_unset__";

    public static IEnumerable<object[]> LoadEntries()
    {
        var root = Environment.GetEnvironmentVariable(FixturesEnv);
        if (string.IsNullOrEmpty(root))
        {
            yield return new object[] { FixturesUnsetSentinel, "", "" };
            yield break;
        }
        var indexPath = Path.Combine(root, "index.json");
        if (!File.Exists(indexPath))
        {
            yield return new object[] { FixturesUnsetSentinel, "", "" };
            yield break;
        }
        var json = File.ReadAllText(indexPath);
        var entries = JsonSerializer.Deserialize<List<ConformanceEntry>>(json, JsonOpts)
                      ?? new List<ConformanceEntry>();
        foreach (var e in entries)
        {
            yield return new object[] { e.OperationId, e.ExampleName, Path.Combine(root, e.File) };
        }
    }

    [Theory]
    [MemberData(nameof(LoadEntries))]
    public void TwilioFixtureConforms(string opId, string exampleName, string fixturePath)
    {
        _ = exampleName; // surfaced via theory display name only
        if (opId == FixturesUnsetSentinel)
        {
            // Conformance corpus not mounted; this is the no-fixtures CI path.
            return;
        }
        if (SkipOps.Contains(opId))
        {
            return;
        }
        var body = File.ReadAllText(fixturePath);
        switch (opId)
        {
            case "CreateCall":
            case "FetchCall":
            case "UpdateCall":
            {
                var v = JsonSerializer.Deserialize<Call>(body, JsonOpts)!;
                Assert.False(string.IsNullOrEmpty(v.Sid), "Call.sid");
                Assert.False(string.IsNullOrEmpty(v.AccountSid), "Call.account_sid");
                break;
            }
            case "ListCall":
            {
                var v = JsonSerializer.Deserialize<CallList>(body, JsonOpts)!;
                Assert.False(string.IsNullOrEmpty(v.Uri), "CallList.uri");
                break;
            }
            case "FetchConference":
            case "UpdateConference":
            {
                var v = JsonSerializer.Deserialize<Conference>(body, JsonOpts)!;
                Assert.False(string.IsNullOrEmpty(v.Sid), "Conference.sid");
                Assert.False(string.IsNullOrEmpty(v.AccountSid), "Conference.account_sid");
                break;
            }
            case "ListConference":
            {
                var v = JsonSerializer.Deserialize<ConferenceList>(body, JsonOpts)!;
                Assert.False(string.IsNullOrEmpty(v.Uri), "ConferenceList.uri");
                break;
            }
            case "CreateParticipant":
            case "FetchParticipant":
            case "UpdateParticipant":
            {
                var v = JsonSerializer.Deserialize<Participant>(body, JsonOpts)!;
                Assert.False(string.IsNullOrEmpty(v.CallSid), "Participant.call_sid");
                Assert.False(string.IsNullOrEmpty(v.ConferenceSid), "Participant.conference_sid");
                break;
            }
            case "ListParticipant":
            {
                var v = JsonSerializer.Deserialize<ParticipantList>(body, JsonOpts)!;
                Assert.False(string.IsNullOrEmpty(v.Uri), "ParticipantList.uri");
                break;
            }
            case "CreateQueue":
            case "FetchQueue":
            case "UpdateQueue":
            {
                var v = JsonSerializer.Deserialize<Queue>(body, JsonOpts)!;
                Assert.False(string.IsNullOrEmpty(v.Sid), "Queue.sid");
                Assert.False(string.IsNullOrEmpty(v.AccountSid), "Queue.account_sid");
                break;
            }
            case "ListQueue":
            {
                var v = JsonSerializer.Deserialize<QueueList>(body, JsonOpts)!;
                Assert.False(string.IsNullOrEmpty(v.Uri), "QueueList.uri");
                break;
            }
            case "FetchMember":
            case "UpdateMember":
            {
                var v = JsonSerializer.Deserialize<QueueMember>(body, JsonOpts)!;
                Assert.False(string.IsNullOrEmpty(v.CallSid), "QueueMember.call_sid");
                break;
            }
            case "ListMember":
            {
                var v = JsonSerializer.Deserialize<QueueMemberList>(body, JsonOpts)!;
                Assert.False(string.IsNullOrEmpty(v.Uri), "QueueMemberList.uri");
                break;
            }
            case "CreateApplication":
            case "FetchApplication":
            case "UpdateApplication":
            {
                var v = JsonSerializer.Deserialize<Application>(body, JsonOpts)!;
                Assert.False(string.IsNullOrEmpty(v.Sid), "Application.sid");
                Assert.False(string.IsNullOrEmpty(v.AccountSid), "Application.account_sid");
                break;
            }
            case "ListApplication":
            {
                var v = JsonSerializer.Deserialize<ApplicationList>(body, JsonOpts)!;
                Assert.False(string.IsNullOrEmpty(v.Uri), "ApplicationList.uri");
                break;
            }
            case "CreateCallRecording":
            case "FetchCallRecording":
            case "UpdateCallRecording":
            case "FetchRecording":
            case "FetchConferenceRecording":
            case "UpdateConferenceRecording":
            {
                var v = JsonSerializer.Deserialize<Recording>(body, JsonOpts)!;
                Assert.False(string.IsNullOrEmpty(v.Sid), "Recording.sid");
                Assert.False(string.IsNullOrEmpty(v.AccountSid), "Recording.account_sid");
                break;
            }
            case "ListCallRecording":
            case "ListRecording":
            case "ListConferenceRecording":
            {
                var v = JsonSerializer.Deserialize<RecordingList>(body, JsonOpts)!;
                Assert.False(string.IsNullOrEmpty(v.Uri), "RecordingList.uri");
                break;
            }
            case "CreateIncomingPhoneNumber":
            case "CreateIncomingPhoneNumberLocal":
            case "CreateIncomingPhoneNumberMobile":
            case "CreateIncomingPhoneNumberTollFree":
            case "FetchIncomingPhoneNumber":
            case "UpdateIncomingPhoneNumber":
            {
                var v = JsonSerializer.Deserialize<IncomingPhoneNumber>(body, JsonOpts)!;
                Assert.False(string.IsNullOrEmpty(v.Sid), "IncomingPhoneNumber.sid");
                Assert.False(string.IsNullOrEmpty(v.AccountSid), "IncomingPhoneNumber.account_sid");
                break;
            }
            case "ListIncomingPhoneNumber":
            case "ListIncomingPhoneNumberLocal":
            case "ListIncomingPhoneNumberMobile":
            case "ListIncomingPhoneNumberTollFree":
            {
                var v = JsonSerializer.Deserialize<IncomingPhoneNumberList>(body, JsonOpts)!;
                Assert.False(string.IsNullOrEmpty(v.Uri), "IncomingPhoneNumberList.uri");
                break;
            }
            // Stream / SiprecSession / CallTranscription Create/Update responses don't
            // emit api_version in Twilio's documented examples — same drift the TS
            // harness fixed-forward by relaxing api_version to optional. Sid/AccountSid/
            // CallSid asserted; api_version not.
            case "CreateStream":
            case "UpdateStream":
            {
                var v = JsonSerializer.Deserialize<Stream>(body, JsonOpts)!;
                Assert.False(string.IsNullOrEmpty(v.Sid), "Stream.sid");
                Assert.False(string.IsNullOrEmpty(v.AccountSid), "Stream.account_sid");
                Assert.False(string.IsNullOrEmpty(v.CallSid), "Stream.call_sid");
                break;
            }
            case "CreateSiprec":
            case "UpdateSiprec":
            {
                var v = JsonSerializer.Deserialize<SiprecSession>(body, JsonOpts)!;
                Assert.False(string.IsNullOrEmpty(v.Sid), "SiprecSession.sid");
                Assert.False(string.IsNullOrEmpty(v.AccountSid), "SiprecSession.account_sid");
                Assert.False(string.IsNullOrEmpty(v.CallSid), "SiprecSession.call_sid");
                break;
            }
            case "CreateRealtimeTranscription":
            case "UpdateRealtimeTranscription":
            {
                var v = JsonSerializer.Deserialize<CallTranscription>(body, JsonOpts)!;
                Assert.False(string.IsNullOrEmpty(v.Sid), "CallTranscription.sid");
                Assert.False(string.IsNullOrEmpty(v.AccountSid), "CallTranscription.account_sid");
                Assert.False(string.IsNullOrEmpty(v.CallSid), "CallTranscription.call_sid");
                break;
            }
            default:
                throw new InvalidOperationException(
                    $"conformance harness: no mapping for operation_id={opId} (fixture={fixturePath}). " +
                    "Add a case or extend SkipOps.");
        }
    }
}
