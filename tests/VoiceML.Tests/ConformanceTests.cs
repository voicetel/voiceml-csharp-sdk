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
            // ----------------------------------------------------------------
            // Accounts / Balance
            // ----------------------------------------------------------------
            case "FetchAccount":
            case "UpdateAccount":
            {
                var v = JsonSerializer.Deserialize<Account>(body, JsonOpts)!;
                // Account responses use sid (the AC… itself) as the identity field.
                // There is no separate account_sid on the account resource.
                Assert.False(string.IsNullOrEmpty(v.Sid), "Account.sid");
                break;
            }
            case "FetchBalance":
            {
                var v = JsonSerializer.Deserialize<Balance>(body, JsonOpts)!;
                Assert.False(string.IsNullOrEmpty(v.AccountSid), "Balance.account_sid");
                break;
            }
            // ----------------------------------------------------------------
            // OutgoingCallerIds + Validation
            // ----------------------------------------------------------------
            case "FetchOutgoingCallerId":
            case "UpdateOutgoingCallerId":
            {
                var v = JsonSerializer.Deserialize<OutgoingCallerId>(body, JsonOpts)!;
                Assert.False(string.IsNullOrEmpty(v.Sid), "OutgoingCallerId.sid");
                Assert.False(string.IsNullOrEmpty(v.AccountSid), "OutgoingCallerId.account_sid");
                break;
            }
            case "ListOutgoingCallerId":
            {
                var v = JsonSerializer.Deserialize<OutgoingCallerIdList>(body, JsonOpts)!;
                Assert.False(string.IsNullOrEmpty(v.Uri), "OutgoingCallerIdList.uri");
                break;
            }
            case "CreateValidationRequest":
            {
                var v = JsonSerializer.Deserialize<ValidationRequest>(body, JsonOpts)!;
                // ValidationRequest is a synthetic response with no sid — Twilio
                // dispatches a call (call_sid) and returns the read-back code.
                Assert.False(string.IsNullOrEmpty(v.AccountSid), "ValidationRequest.account_sid");
                break;
            }
            // ----------------------------------------------------------------
            // Recording-derived (offline) Transcriptions — distinct from the
            // realtime CallTranscription handled above.
            // ----------------------------------------------------------------
            case "FetchTranscription":
            case "FetchRecordingTranscription":
            {
                var v = JsonSerializer.Deserialize<RecordingTranscription>(body, JsonOpts)!;
                Assert.False(string.IsNullOrEmpty(v.Sid), "RecordingTranscription.sid");
                Assert.False(string.IsNullOrEmpty(v.AccountSid), "RecordingTranscription.account_sid");
                break;
            }
            case "ListTranscription":
            case "ListRecordingTranscription":
            {
                var v = JsonSerializer.Deserialize<RecordingTranscriptionList>(body, JsonOpts)!;
                Assert.False(string.IsNullOrEmpty(v.Uri), "RecordingTranscriptionList.uri");
                break;
            }
            // ----------------------------------------------------------------
            // Message Media
            // ----------------------------------------------------------------
            case "FetchMedia":
            {
                var v = JsonSerializer.Deserialize<Media>(body, JsonOpts)!;
                Assert.False(string.IsNullOrEmpty(v.Sid), "Media.sid");
                Assert.False(string.IsNullOrEmpty(v.AccountSid), "Media.account_sid");
                break;
            }
            case "ListMedia":
            {
                var v = JsonSerializer.Deserialize<MediaList>(body, JsonOpts)!;
                Assert.False(string.IsNullOrEmpty(v.Uri), "MediaList.uri");
                break;
            }
            // ----------------------------------------------------------------
            // Payments (Pay verb REST companion)
            // ----------------------------------------------------------------
            case "CreatePayments":
            case "UpdatePayments":
            {
                var v = JsonSerializer.Deserialize<CallPayment>(body, JsonOpts)!;
                Assert.False(string.IsNullOrEmpty(v.Sid), "CallPayment.sid");
                Assert.False(string.IsNullOrEmpty(v.AccountSid), "CallPayment.account_sid");
                Assert.False(string.IsNullOrEmpty(v.CallSid), "CallPayment.call_sid");
                break;
            }
            // ----------------------------------------------------------------
            // SIP Domains
            // ----------------------------------------------------------------
            case "CreateSipDomain":
            case "FetchSipDomain":
            case "UpdateSipDomain":
            {
                var v = JsonSerializer.Deserialize<SipDomain>(body, JsonOpts)!;
                Assert.False(string.IsNullOrEmpty(v.Sid), "SipDomain.sid");
                Assert.False(string.IsNullOrEmpty(v.AccountSid), "SipDomain.account_sid");
                break;
            }
            case "ListSipDomain":
            {
                var v = JsonSerializer.Deserialize<SipDomainList>(body, JsonOpts)!;
                Assert.False(string.IsNullOrEmpty(v.Uri), "SipDomainList.uri");
                break;
            }
            // ----------------------------------------------------------------
            // SIP CredentialLists
            // ----------------------------------------------------------------
            case "CreateSipCredentialList":
            case "FetchSipCredentialList":
            case "UpdateSipCredentialList":
            {
                var v = JsonSerializer.Deserialize<SipCredentialList>(body, JsonOpts)!;
                Assert.False(string.IsNullOrEmpty(v.Sid), "SipCredentialList.sid");
                Assert.False(string.IsNullOrEmpty(v.AccountSid), "SipCredentialList.account_sid");
                break;
            }
            case "ListSipCredentialList":
            {
                var v = JsonSerializer.Deserialize<SipCredentialListList>(body, JsonOpts)!;
                Assert.False(string.IsNullOrEmpty(v.Uri), "SipCredentialListList.uri");
                break;
            }
            // ----------------------------------------------------------------
            // SIP Credentials (the individual username/password rows)
            // ----------------------------------------------------------------
            case "CreateSipCredential":
            case "FetchSipCredential":
            case "UpdateSipCredential":
            {
                var v = JsonSerializer.Deserialize<SipCredential>(body, JsonOpts)!;
                Assert.False(string.IsNullOrEmpty(v.Sid), "SipCredential.sid");
                Assert.False(string.IsNullOrEmpty(v.AccountSid), "SipCredential.account_sid");
                break;
            }
            case "ListSipCredential":
            {
                var v = JsonSerializer.Deserialize<SipCredentialListPage>(body, JsonOpts)!;
                Assert.False(string.IsNullOrEmpty(v.Uri), "SipCredentialListPage.uri");
                break;
            }
            // ----------------------------------------------------------------
            // SIP IpAccessControlLists
            // ----------------------------------------------------------------
            case "CreateSipIpAccessControlList":
            case "FetchSipIpAccessControlList":
            case "UpdateSipIpAccessControlList":
            {
                var v = JsonSerializer.Deserialize<SipIpAccessControlList>(body, JsonOpts)!;
                Assert.False(string.IsNullOrEmpty(v.Sid), "SipIpAccessControlList.sid");
                Assert.False(string.IsNullOrEmpty(v.AccountSid), "SipIpAccessControlList.account_sid");
                break;
            }
            case "ListSipIpAccessControlList":
            {
                var v = JsonSerializer.Deserialize<SipIpAccessControlListList>(body, JsonOpts)!;
                Assert.False(string.IsNullOrEmpty(v.Uri), "SipIpAccessControlListList.uri");
                break;
            }
            // ----------------------------------------------------------------
            // SIP IpAddresses (the individual CIDR rows)
            // ----------------------------------------------------------------
            case "CreateSipIpAddress":
            case "FetchSipIpAddress":
            case "UpdateSipIpAddress":
            {
                var v = JsonSerializer.Deserialize<SipIpAddress>(body, JsonOpts)!;
                Assert.False(string.IsNullOrEmpty(v.Sid), "SipIpAddress.sid");
                Assert.False(string.IsNullOrEmpty(v.AccountSid), "SipIpAddress.account_sid");
                break;
            }
            case "ListSipIpAddress":
            {
                var v = JsonSerializer.Deserialize<SipIpAddressList>(body, JsonOpts)!;
                Assert.False(string.IsNullOrEmpty(v.Uri), "SipIpAddressList.uri");
                break;
            }
            // ----------------------------------------------------------------
            // SIP Domain Mappings (legacy non-Auth namespace)
            // ----------------------------------------------------------------
            case "CreateSipCredentialListMapping":
            case "FetchSipCredentialListMapping":
            case "CreateSipIpAccessControlListMapping":
            case "FetchSipIpAccessControlListMapping":
            {
                var v = JsonSerializer.Deserialize<SipDomainMapping>(body, JsonOpts)!;
                Assert.False(string.IsNullOrEmpty(v.Sid), "SipDomainMapping.sid");
                Assert.False(string.IsNullOrEmpty(v.AccountSid), "SipDomainMapping.account_sid");
                break;
            }
            case "ListSipCredentialListMapping":
            {
                var v = JsonSerializer.Deserialize<SipCredentialListMappingList>(body, JsonOpts)!;
                Assert.False(string.IsNullOrEmpty(v.Uri), "SipCredentialListMappingList.uri");
                break;
            }
            case "ListSipIpAccessControlListMapping":
            {
                var v = JsonSerializer.Deserialize<SipIpAccessControlListMappingList>(body, JsonOpts)!;
                Assert.False(string.IsNullOrEmpty(v.Uri), "SipIpAccessControlListMappingList.uri");
                break;
            }
            // ----------------------------------------------------------------
            // SIP Auth Mappings (Calls + Registrations × Credential/IpAcl).
            // Items share the SipDomainMapping shape, list envelope uses the
            // common "contents" array key.
            // ----------------------------------------------------------------
            case "CreateSipAuthCallsCredentialListMapping":
            case "FetchSipAuthCallsCredentialListMapping":
            case "CreateSipAuthCallsIpAccessControlListMapping":
            case "FetchSipAuthCallsIpAccessControlListMapping":
            case "CreateSipAuthRegistrationsCredentialListMapping":
            case "FetchSipAuthRegistrationsCredentialListMapping":
            {
                var v = JsonSerializer.Deserialize<SipDomainMapping>(body, JsonOpts)!;
                Assert.False(string.IsNullOrEmpty(v.Sid), "SipDomainMapping.sid");
                Assert.False(string.IsNullOrEmpty(v.AccountSid), "SipDomainMapping.account_sid");
                break;
            }
            case "ListSipAuthCallsCredentialListMapping":
            case "ListSipAuthCallsIpAccessControlListMapping":
            case "ListSipAuthRegistrationsCredentialListMapping":
            {
                var v = JsonSerializer.Deserialize<SipAuthMappingList>(body, JsonOpts)!;
                Assert.False(string.IsNullOrEmpty(v.Uri), "SipAuthMappingList.uri");
                break;
            }
            default:
                throw new InvalidOperationException(
                    $"conformance harness: no mapping for operation_id={opId} (fixture={fixturePath}). " +
                    "Add a case or extend SkipOps.");
        }
    }
}
