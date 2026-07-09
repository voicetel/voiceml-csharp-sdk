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

/// <summary>Wire-shape tests for the v0.9.0 surface: ConversationsV1 (15 resources),
/// VoiceV1 (6 resources), and RoutesV2 PhoneNumber.</summary>
public class V090Tests
{
    private const string AccSid = "AC" + "ffffffffffffffffffffffffffffffff";
    private const string ApiKey = "secret-key";

    // ---- Wiring sanity ------------------------------------------------------

    [Fact]
    public void V090Surfaces_AreWiredOnClient()
    {
        using var client = new VoiceMLClient(new ClientOptions { AccountSid = AccSid, ApiKey = ApiKey });
        Assert.NotNull(client.ConversationsV1);
        Assert.NotNull(client.ConversationsV1.Conversations);
        Assert.NotNull(client.ConversationsV1.Roles);
        Assert.NotNull(client.ConversationsV1.Users);
        Assert.NotNull(client.ConversationsV1.Credentials);
        Assert.NotNull(client.ConversationsV1.Configuration);
        Assert.NotNull(client.ConversationsV1.Configuration.Webhooks);
        Assert.NotNull(client.ConversationsV1.Configuration.Addresses);
        Assert.NotNull(client.ConversationsV1.ParticipantConversations);
        Assert.NotNull(client.ConversationsV1.ConversationWithParticipants);
        Assert.NotNull(client.ConversationsV1.Services);

        Assert.NotNull(client.VoiceV1);
        Assert.NotNull(client.VoiceV1.IpRecords);
        Assert.NotNull(client.VoiceV1.SourceIpMappings);
        Assert.NotNull(client.VoiceV1.ByocTrunks);
        Assert.NotNull(client.VoiceV1.ConnectionPolicies);
        Assert.NotNull(client.VoiceV1.Settings);

        Assert.NotNull(client.RoutesV2.PhoneNumbers);
    }

    // ---- ConversationsV1.Conversations -------------------------------------

    [Fact]
    public async Task ConversationsV1_Conversations_Create_UsesV1NamespaceNoAccountPrefix()
    {
        const string convSid = "CH00000000000000000000000000000001";
        Uri? captured = null;
        var handler = new MockHandler(req =>
        {
            captured = req.RequestUri;
            Assert.Equal(HttpMethod.Post, req.Method);
            var form = ParseForm(req.Content!.ReadAsStringAsync().Result);
            Assert.Equal("support thread", form["FriendlyName"]);
            Assert.Equal("active", form["State"]);
            Assert.Equal("PT5M", form["Timers.Inactive"]);
            return Reply(HttpStatusCode.Created, ConversationJson(convSid, state: "active"));
        });
        using var client = NewClient(handler);
        var c = await client.ConversationsV1.Conversations.CreateAsync(new CreateConversationRequest
        {
            FriendlyName = "support thread",
            State = "active",
            TimersInactive = "PT5M",
        });
        Assert.Equal(convSid, c.Sid);
        Assert.Equal("active", c.State);
        Assert.NotNull(captured);
        Assert.Equal("https://conversations.voicetel.com/v1/Conversations", captured!.ToString());
        Assert.DoesNotContain(AccSid, captured.ToString());
    }

    [Fact]
    public async Task ConversationsV1_Conversations_List_HasMetaEnvelope()
    {
        const string convSid = "CH00000000000000000000000000000002";
        var handler = new MockHandler(req =>
        {
            Assert.Equal(HttpMethod.Get, req.Method);
            Assert.Equal("https://conversations.voicetel.com/v1/Conversations?PageSize=25", req.RequestUri!.ToString());
            var body = "{\"conversations\":[" + ConversationJson(convSid, state: "active") + "]," +
                       "\"meta\":{\"page\":0,\"page_size\":25,\"first_page_url\":\"https://x/v1/Conversations?PageSize=25\",\"key\":\"conversations\"}}";
            return Reply(HttpStatusCode.OK, body);
        });
        using var client = NewClient(handler);
        var list = await client.ConversationsV1.Conversations.ListAsync(new ListV1PageParams { PageSize = 25 });
        Assert.Single(list.Conversations);
        Assert.Equal(convSid, list.Conversations[0].Sid);
        Assert.NotNull(list.Meta);
        Assert.Equal(0, list.Meta!.Page);
        Assert.Equal("conversations", list.Meta.Key);
    }

    [Fact]
    public async Task ConversationsV1_Conversations_FetchUpdateDelete()
    {
        const string convSid = "CH00000000000000000000000000000003";
        int call = 0;
        var handler = new MockHandler(req =>
        {
            call++;
            Assert.Equal($"https://conversations.voicetel.com/v1/Conversations/{convSid}", req.RequestUri!.ToString());
            return call switch
            {
                1 => Reply(HttpStatusCode.OK, ConversationJson(convSid, state: "active")),
                2 => Reply(HttpStatusCode.OK, ConversationJson(convSid, state: "closed")),
                _ => Reply(HttpStatusCode.NoContent, ""),
            };
        });
        using var client = NewClient(handler);
        var c1 = await client.ConversationsV1.Conversations.GetAsync(convSid);
        Assert.Equal("active", c1.State);
        var c2 = await client.ConversationsV1.Conversations.UpdateAsync(convSid, new UpdateConversationRequest { State = "closed" });
        Assert.Equal("closed", c2.State);
        await client.ConversationsV1.Conversations.DeleteAsync(convSid);
        Assert.Equal(3, call);
    }

    // ---- ConversationsV1.Conversations scoped sub-resources -----------------

    [Fact]
    public async Task ConversationsV1_Messages_ScopedFactory_BuildsCorrectPath()
    {
        const string convSid = "CH00000000000000000000000000000004";
        const string msgSid = "IM00000000000000000000000000000001";
        var handler = new MockHandler(req =>
        {
            Assert.Equal(HttpMethod.Post, req.Method);
            Assert.Equal($"https://conversations.voicetel.com/v1/Conversations/{convSid}/Messages", req.RequestUri!.ToString());
            var form = ParseForm(req.Content!.ReadAsStringAsync().Result);
            Assert.Equal("alice", form["Author"]);
            Assert.Equal("hello", form["Body"]);
            return Reply(HttpStatusCode.Created, MessageJson(convSid, msgSid));
        });
        using var client = NewClient(handler);
        var msg = await client.ConversationsV1.Conversations.Messages(convSid)
            .CreateAsync(new CreateConversationMessageRequest { Author = "alice", Body = "hello" });
        Assert.Equal(msgSid, msg.Sid);
        Assert.Equal(0, msg.Index);
    }

    [Fact]
    public async Task ConversationsV1_Receipts_DoublyScoped_BuildsCorrectPath()
    {
        const string convSid = "CH00000000000000000000000000000005";
        const string msgSid = "IM00000000000000000000000000000002";
        var handler = new MockHandler(req =>
        {
            Assert.Equal(HttpMethod.Get, req.Method);
            Assert.Equal(
                $"https://conversations.voicetel.com/v1/Conversations/{convSid}/Messages/{msgSid}/Receipts",
                req.RequestUri!.ToString());
            return Reply(HttpStatusCode.OK,
                "{\"delivery_receipts\":[], \"meta\":{\"page\":0,\"page_size\":50,\"key\":\"delivery_receipts\"}}");
        });
        using var client = NewClient(handler);
        var receipts = await client.ConversationsV1.Conversations.Messages(convSid).Receipts(msgSid).ListAsync();
        Assert.NotNull(receipts);
        Assert.Empty(receipts.DeliveryReceipts);
    }

    [Fact]
    public async Task ConversationsV1_Participants_Create_SendsDottedFormKeys()
    {
        const string convSid = "CH00000000000000000000000000000006";
        const string partSid = "MB00000000000000000000000000000001";
        var handler = new MockHandler(req =>
        {
            Assert.Equal(HttpMethod.Post, req.Method);
            Assert.Equal($"https://conversations.voicetel.com/v1/Conversations/{convSid}/Participants", req.RequestUri!.ToString());
            var form = ParseForm(req.Content!.ReadAsStringAsync().Result);
            Assert.Equal("+15551112222", form["MessagingBinding.Address"]);
            Assert.Equal("+15553334444", form["MessagingBinding.ProxyAddress"]);
            Assert.False(form.ContainsKey("Identity"));
            return Reply(HttpStatusCode.Created, ParticipantJson(convSid, partSid));
        });
        using var client = NewClient(handler);
        var p = await client.ConversationsV1.Conversations.Participants(convSid).CreateAsync(
            new CreateConversationParticipantRequest
            {
                MessagingBindingAddress = "+15551112222",
                MessagingBindingProxyAddress = "+15553334444",
            });
        Assert.Equal(partSid, p.Sid);
    }

    [Fact]
    public async Task ConversationsV1_Webhooks_Create_RequiresTarget()
    {
        const string convSid = "CH00000000000000000000000000000007";
        const string whSid = "WH00000000000000000000000000000001";
        var handler = new MockHandler(req =>
        {
            Assert.Equal($"https://conversations.voicetel.com/v1/Conversations/{convSid}/Webhooks", req.RequestUri!.ToString());
            var form = ParseForm(req.Content!.ReadAsStringAsync().Result);
            Assert.Equal("webhook", form["Target"]);
            Assert.Equal("https://hooks.example.com/c", form["Configuration.Url"]);
            Assert.Equal("POST", form["Configuration.Method"]);
            return Reply(HttpStatusCode.Created, WebhookJson(convSid, whSid));
        });
        using var client = NewClient(handler);
        await client.ConversationsV1.Conversations.Webhooks(convSid).CreateAsync(
            new CreateConversationScopedWebhookRequest
            {
                Target = "webhook",
                ConfigurationUrl = "https://hooks.example.com/c",
                ConfigurationMethod = "POST",
            });
    }

    // ---- Roles / Users / Credentials ----------------------------------------

    [Fact]
    public async Task ConversationsV1_Roles_Create_EmitsRepeatedPermissionField()
    {
        const string roleSid = "RL00000000000000000000000000000001";
        var handler = new MockHandler(req =>
        {
            Assert.Equal("https://conversations.voicetel.com/v1/Roles", req.RequestUri!.ToString());
            var raw = req.Content!.ReadAsStringAsync().Result;
            var perms = ExtractRepeated(raw, "Permission");
            Assert.Equal(2, perms.Count);
            Assert.Contains("sendMessage", perms);
            Assert.Contains("editAnyMessage", perms);
            return Reply(HttpStatusCode.Created, RoleJson(roleSid));
        });
        using var client = NewClient(handler);
        var r = await client.ConversationsV1.Roles.CreateAsync(new CreateConversationsRoleRequest
        {
            FriendlyName = "support",
            Type = "conversation",
            Permission = new[] { "sendMessage", "editAnyMessage" },
        });
        Assert.Equal(roleSid, r.Sid);
        Assert.NotNull(r.Permissions);
        Assert.Equal(2, r.Permissions!.Count);
    }

    [Fact]
    public async Task ConversationsV1_Users_ScopedConversations_BuildsPath()
    {
        const string userSid = "US00000000000000000000000000000001";
        var handler = new MockHandler(req =>
        {
            Assert.Equal($"https://conversations.voicetel.com/v1/Users/{userSid}/Conversations", req.RequestUri!.ToString());
            return Reply(HttpStatusCode.OK,
                "{\"conversations\":[], \"meta\":{\"page\":0,\"page_size\":50,\"key\":\"conversations\"}}");
        });
        using var client = NewClient(handler);
        var list = await client.ConversationsV1.Users.Conversations(userSid).ListAsync();
        Assert.Empty(list.Conversations);
    }

    [Fact]
    public async Task ConversationsV1_Credentials_Create_SerializesBoolSandbox()
    {
        const string credSid = "CR00000000000000000000000000000001";
        var handler = new MockHandler(req =>
        {
            var form = ParseForm(req.Content!.ReadAsStringAsync().Result);
            Assert.Equal("apn", form["Type"]);
            Assert.Equal("true", form["Sandbox"]);
            return Reply(HttpStatusCode.Created, CredentialJson(credSid));
        });
        using var client = NewClient(handler);
        var cr = await client.ConversationsV1.Credentials.CreateAsync(new CreateConversationsCredentialRequest
        {
            Type = "apn",
            Sandbox = true,
        });
        Assert.Equal("apn", cr.Type);
    }

    // ---- Configuration (+ Webhooks, Addresses) ------------------------------

    [Fact]
    public async Task ConversationsV1_Configuration_FetchAndUpdate()
    {
        int call = 0;
        var handler = new MockHandler(req =>
        {
            call++;
            Assert.Equal("https://conversations.voicetel.com/v1/Configuration", req.RequestUri!.ToString());
            return Reply(HttpStatusCode.OK,
                "{\"account_sid\":\"" + AccSid + "\"," +
                "\"url\":\"https://conversations.voicetel.com/v1/Configuration\"}");
        });
        using var client = NewClient(handler);
        var cfg = await client.ConversationsV1.Configuration.FetchAsync();
        Assert.Equal(AccSid, cfg.AccountSid);
        await client.ConversationsV1.Configuration.UpdateAsync(new UpdateConversationsConfigurationRequest
        {
            DefaultClosedTimer = "P7D",
        });
        Assert.Equal(2, call);
    }

    [Fact]
    public async Task ConversationsV1_ConfigurationWebhooks_FetchUpdate()
    {
        var handler = new MockHandler(req =>
        {
            Assert.Equal("https://conversations.voicetel.com/v1/Configuration/Webhooks", req.RequestUri!.ToString());
            if (req.Method == HttpMethod.Post)
            {
                var raw = req.Content!.ReadAsStringAsync().Result;
                var filters = ExtractRepeated(raw, "Filters");
                Assert.Contains("onMessageAdded", filters);
                Assert.Contains("onConversationUpdated", filters);
            }
            return Reply(HttpStatusCode.OK,
                "{\"method\":\"POST\",\"target\":\"webhook\"," +
                "\"filters\":[\"onMessageAdded\",\"onConversationUpdated\"]," +
                "\"pre_webhook_url\":null,\"post_webhook_url\":\"https://hooks.example.com/post\"," +
                "\"url\":\"https://conversations.voicetel.com/v1/Configuration/Webhooks\"}");
        });
        using var client = NewClient(handler);
        var cfg = await client.ConversationsV1.Configuration.Webhooks.FetchAsync();
        Assert.Equal("POST", cfg.Method);
        Assert.NotNull(cfg.Filters);
        Assert.Equal(2, cfg.Filters!.Count);

        await client.ConversationsV1.Configuration.Webhooks.UpdateAsync(
            new UpdateConversationsConfigurationWebhookRequest
            {
                Method = "POST",
                Filters = new[] { "onMessageAdded", "onConversationUpdated" },
                PostWebhookUrl = "https://hooks.example.com/post",
                Target = "webhook",
            });
    }

    [Fact]
    public async Task ConversationsV1_ConfigAddresses_Create()
    {
        const string addrSid = "IG00000000000000000000000000000001";
        var handler = new MockHandler(req =>
        {
            Assert.Equal("https://conversations.voicetel.com/v1/Configuration/Addresses", req.RequestUri!.ToString());
            var form = ParseForm(req.Content!.ReadAsStringAsync().Result);
            Assert.Equal("sms", form["Type"]);
            Assert.Equal("+15551234567", form["Address"]);
            Assert.Equal("true", form["AutoCreation.Enabled"]);
            Assert.Equal("webhook", form["AutoCreation.Type"]);
            return Reply(HttpStatusCode.Created, ConfigAddressJson(addrSid));
        });
        using var client = NewClient(handler);
        var addr = await client.ConversationsV1.Configuration.Addresses.CreateAsync(new CreateConfigAddressRequest
        {
            Type = "sms",
            Address = "+15551234567",
            AutoCreationEnabled = true,
            AutoCreationType = "webhook",
            AutoCreationWebhookUrl = "https://hooks.example.com/ac",
        });
        Assert.Equal(addrSid, addr.Sid);
    }

    // ---- ParticipantConversations / ConversationWithParticipants / Services

    [Fact]
    public async Task ConversationsV1_ParticipantConversations_QueryParams()
    {
        var handler = new MockHandler(req =>
        {
            Assert.Equal(HttpMethod.Get, req.Method);
            Assert.Contains("Identity=alice", req.RequestUri!.ToString());
            Assert.Contains("Address=%2B15551234567", req.RequestUri.ToString());
            return Reply(HttpStatusCode.OK,
                "{\"conversations\":[], \"meta\":{\"page\":0,\"page_size\":50,\"key\":\"conversations\"}}");
        });
        using var client = NewClient(handler);
        var list = await client.ConversationsV1.ParticipantConversations.ListAsync(
            new ListParticipantConversationsParams { Identity = "alice", Address = "+15551234567" });
        Assert.Empty(list.Conversations);
    }

    [Fact]
    public async Task ConversationsV1_ConversationWithParticipants_Create()
    {
        const string convSid = "CH00000000000000000000000000000008";
        var handler = new MockHandler(req =>
        {
            Assert.Equal("https://conversations.voicetel.com/v1/ConversationWithParticipants", req.RequestUri!.ToString());
            var raw = req.Content!.ReadAsStringAsync().Result;
            var participants = ExtractRepeated(raw, "Participant");
            Assert.Equal(2, participants.Count);
            return Reply(HttpStatusCode.Created, ConversationJson(convSid, state: "active"));
        });
        using var client = NewClient(handler);
        var c = await client.ConversationsV1.ConversationWithParticipants.CreateAsync(
            new CreateConversationWithParticipantsRequest
            {
                FriendlyName = "group",
                Participant = new[]
                {
                    "{\"identity\":\"alice\"}",
                    "{\"messaging_binding\":{\"address\":\"+15551112222\"}}",
                },
            });
        Assert.Equal(convSid, c.Sid);
    }

    [Fact]
    public async Task ConversationsV1_Services_Create()
    {
        const string svcSid = "IS00000000000000000000000000000001";
        var handler = new MockHandler(req =>
        {
            Assert.Equal("https://conversations.voicetel.com/v1/Services", req.RequestUri!.ToString());
            var form = ParseForm(req.Content!.ReadAsStringAsync().Result);
            Assert.Equal("eu-tenant", form["FriendlyName"]);
            return Reply(HttpStatusCode.Created, ServiceJson(svcSid));
        });
        using var client = NewClient(handler);
        var s = await client.ConversationsV1.Services.CreateAsync(new CreateConversationsServiceRequest { FriendlyName = "eu-tenant" });
        Assert.Equal(svcSid, s.Sid);
    }

    // ==================== VoiceV1 ============================================

    [Fact]
    public async Task VoiceV1_IpRecords_Create_FormAndPath()
    {
        const string sid = "IL00000000000000000000000000000001";
        var handler = new MockHandler(req =>
        {
            Assert.Equal(HttpMethod.Post, req.Method);
            Assert.Equal("https://voiceml.voicetel.com/v1/IpRecords", req.RequestUri!.ToString());
            var form = ParseForm(req.Content!.ReadAsStringAsync().Result);
            Assert.Equal("203.0.113.10", form["IpAddress"]);
            Assert.Equal("32", form["CidrPrefixLength"]);
            return Reply(HttpStatusCode.Created, IpRecordJson(sid));
        });
        using var client = NewClient(handler);
        var rec = await client.VoiceV1.IpRecords.CreateAsync(new CreateVoiceV1IpRecordRequest
        {
            IpAddress = "203.0.113.10",
            CidrPrefixLength = 32,
        });
        Assert.Equal(sid, rec.Sid);
        Assert.Equal(32, rec.CidrPrefixLength);
    }

    [Fact]
    public async Task VoiceV1_SourceIpMappings_Create()
    {
        const string sid = "IB00000000000000000000000000000001";
        const string ilSid = "IL11111111111111111111111111111111";
        const string sdSid = "SD22222222222222222222222222222222";
        var handler = new MockHandler(req =>
        {
            Assert.Equal("https://voiceml.voicetel.com/v1/SourceIpMappings", req.RequestUri!.ToString());
            var form = ParseForm(req.Content!.ReadAsStringAsync().Result);
            Assert.Equal(ilSid, form["IpRecordSid"]);
            Assert.Equal(sdSid, form["SipDomainSid"]);
            return Reply(HttpStatusCode.Created, SourceIpMappingJson(sid, ilSid, sdSid));
        });
        using var client = NewClient(handler);
        var m = await client.VoiceV1.SourceIpMappings.CreateAsync(new CreateVoiceV1SourceIpMappingRequest
        {
            IpRecordSid = ilSid,
            SipDomainSid = sdSid,
        });
        Assert.Equal(sid, m.Sid);
    }

    [Fact]
    public async Task VoiceV1_ByocTrunks_Create_BoolToLowerString()
    {
        const string sid = "BY00000000000000000000000000000001";
        var handler = new MockHandler(req =>
        {
            var form = ParseForm(req.Content!.ReadAsStringAsync().Result);
            Assert.Equal("https://voiceml.voicetel.com/v1/ByocTrunks", req.RequestUri!.ToString());
            Assert.Equal("true", form["CnamLookupEnabled"]);
            return Reply(HttpStatusCode.Created, ByocTrunkJson(sid));
        });
        using var client = NewClient(handler);
        var t = await client.VoiceV1.ByocTrunks.CreateAsync(new CreateVoiceV1ByocTrunkRequest
        {
            FriendlyName = "carrier-x",
            CnamLookupEnabled = true,
        });
        Assert.Equal(sid, t.Sid);
        Assert.True(t.CnamLookupEnabled);
    }

    [Fact]
    public async Task VoiceV1_ConnectionPolicies_TargetsFactory_BuildsPath()
    {
        const string policySid = "NY00000000000000000000000000000001";
        const string targetSid = "NE00000000000000000000000000000001";
        var handler = new MockHandler(req =>
        {
            Assert.Equal(HttpMethod.Post, req.Method);
            Assert.Equal(
                $"https://voiceml.voicetel.com/v1/ConnectionPolicies/{policySid}/Targets",
                req.RequestUri!.ToString());
            var form = ParseForm(req.Content!.ReadAsStringAsync().Result);
            Assert.Equal("sip:edge@example.com", form["Target"]);
            Assert.Equal("5", form["Priority"]);
            Assert.Equal("true", form["Enabled"]);
            return Reply(HttpStatusCode.Created, ConnectionPolicyTargetJson(policySid, targetSid));
        });
        using var client = NewClient(handler);
        var tgt = await client.VoiceV1.ConnectionPolicies.Targets(policySid).CreateAsync(
            new CreateVoiceV1ConnectionPolicyTargetRequest
            {
                Target = "sip:edge@example.com",
                Priority = 5,
                Enabled = true,
            });
        Assert.Equal(targetSid, tgt.Sid);
        Assert.Equal(5, tgt.Priority);
    }

    [Fact]
    public async Task VoiceV1_Settings_FetchAndUpdate()
    {
        int call = 0;
        var handler = new MockHandler(req =>
        {
            call++;
            Assert.Equal("https://voiceml.voicetel.com/v1/Settings", req.RequestUri!.ToString());
            if (call == 2)
            {
                var form = ParseForm(req.Content!.ReadAsStringAsync().Result);
                Assert.Equal("true", form["DialingPermissionsInheritance"]);
                return Reply(HttpStatusCode.Accepted,
                    "{\"dialing_permissions_inheritance\":true,\"url\":\"https://voiceml.voicetel.com/v1/Settings\"}");
            }
            return Reply(HttpStatusCode.OK,
                "{\"dialing_permissions_inheritance\":false,\"url\":\"https://voiceml.voicetel.com/v1/Settings\"}");
        });
        using var client = NewClient(handler);
        var s1 = await client.VoiceV1.Settings.FetchAsync();
        Assert.False(s1.DialingPermissionsInheritance);
        var s2 = await client.VoiceV1.Settings.UpdateAsync(new UpdateVoiceV1SettingsRequest
        {
            DialingPermissionsInheritance = true,
        });
        Assert.True(s2.DialingPermissionsInheritance);
    }

    // ==================== RoutesV2 PhoneNumbers ==============================

    [Fact]
    public async Task RoutesV2_PhoneNumbers_Fetch_UsesV2NamespaceNoAccountPrefix()
    {
        const string number = "+18005551234";
        const string qqSid = "QQ00000000000000000000000000000001";
        Uri? captured = null;
        var handler = new MockHandler(req =>
        {
            captured = req.RequestUri;
            return Reply(HttpStatusCode.OK, PhoneNumberJson(qqSid, number, voiceRegion: "us1"));
        });
        using var client = NewClient(handler);
        var pn = await client.RoutesV2.PhoneNumbers.GetAsync(number);
        Assert.Equal(qqSid, pn.Sid);
        Assert.Equal("us1", pn.VoiceRegion);
        Assert.NotNull(captured);
        Assert.Equal($"https://voiceml.voicetel.com/v2/PhoneNumbers/{number}", captured!.ToString());
        Assert.DoesNotContain(AccSid, captured.ToString());
    }

    [Fact]
    public async Task RoutesV2_PhoneNumbers_Update_SendsForm()
    {
        const string number = "+18005551234";
        const string qqSid = "QQ00000000000000000000000000000002";
        var handler = new MockHandler(req =>
        {
            Assert.Equal(HttpMethod.Post, req.Method);
            var form = ParseForm(req.Content!.ReadAsStringAsync().Result);
            Assert.Equal("ie1", form["VoiceRegion"]);
            Assert.Equal("renamed", form["FriendlyName"]);
            return Reply(HttpStatusCode.OK, PhoneNumberJson(qqSid, number, voiceRegion: "ie1", friendlyName: "renamed"));
        });
        using var client = NewClient(handler);
        var pn = await client.RoutesV2.PhoneNumbers.UpdateAsync(number,
            new UpdateRoutesV2PhoneNumberRequest { VoiceRegion = "ie1", FriendlyName = "renamed" });
        Assert.Equal("ie1", pn.VoiceRegion);
        Assert.Equal("renamed", pn.FriendlyName);
    }

    // ---- JSON builders (plain strings, no raw-string interpolation) ---------

    private static string ConversationJson(string sid, string state) =>
        "{\"sid\":\"" + sid + "\",\"account_sid\":\"" + AccSid + "\",\"state\":\"" + state + "\"," +
        "\"attributes\":\"{}\",\"date_created\":\"2026-06-27T00:00:00Z\"," +
        "\"date_updated\":\"2026-06-27T00:00:00Z\",\"url\":\"https://conversations.voicetel.com/v1/Conversations/" + sid + "\"}";

    private static string MessageJson(string convSid, string msgSid) =>
        "{\"sid\":\"" + msgSid + "\",\"conversation_sid\":\"" + convSid + "\",\"index\":0," +
        "\"author\":\"alice\",\"body\":\"hello\",\"attributes\":\"{}\"," +
        "\"account_sid\":\"" + AccSid + "\",\"date_created\":\"2026-06-27T00:00:00Z\"," +
        "\"date_updated\":\"2026-06-27T00:00:00Z\",\"url\":\"\"}";

    private static string ParticipantJson(string convSid, string partSid) =>
        "{\"sid\":\"" + partSid + "\",\"conversation_sid\":\"" + convSid + "\",\"attributes\":\"{}\"," +
        "\"account_sid\":\"" + AccSid + "\",\"date_created\":\"2026-06-27T00:00:00Z\"," +
        "\"date_updated\":\"2026-06-27T00:00:00Z\",\"url\":\"\"}";

    private static string WebhookJson(string convSid, string whSid) =>
        "{\"sid\":\"" + whSid + "\",\"conversation_sid\":\"" + convSid + "\",\"target\":\"webhook\"," +
        "\"account_sid\":\"" + AccSid + "\",\"date_created\":\"2026-06-27T00:00:00Z\"," +
        "\"date_updated\":\"2026-06-27T00:00:00Z\",\"url\":\"\"}";

    private static string RoleJson(string sid) =>
        "{\"sid\":\"" + sid + "\",\"type\":\"conversation\"," +
        "\"permissions\":[\"sendMessage\",\"editAnyMessage\"]," +
        "\"account_sid\":\"" + AccSid + "\",\"date_created\":\"2026-06-27T00:00:00Z\"," +
        "\"date_updated\":\"2026-06-27T00:00:00Z\",\"url\":\"\"}";

    private static string CredentialJson(string sid) =>
        "{\"sid\":\"" + sid + "\",\"type\":\"apn\",\"account_sid\":\"" + AccSid + "\"," +
        "\"date_created\":\"2026-06-27T00:00:00Z\"," +
        "\"date_updated\":\"2026-06-27T00:00:00Z\",\"url\":\"\"}";

    private static string ConfigAddressJson(string sid) =>
        "{\"sid\":\"" + sid + "\",\"type\":\"sms\",\"address\":\"+15551234567\"," +
        "\"account_sid\":\"" + AccSid + "\",\"date_created\":\"2026-06-27T00:00:00Z\"," +
        "\"date_updated\":\"2026-06-27T00:00:00Z\",\"url\":\"\"}";

    private static string ServiceJson(string sid) =>
        "{\"sid\":\"" + sid + "\",\"friendly_name\":\"eu-tenant\"," +
        "\"account_sid\":\"" + AccSid + "\",\"date_created\":\"2026-06-27T00:00:00Z\"," +
        "\"date_updated\":\"2026-06-27T00:00:00Z\",\"url\":\"\"}";

    private static string IpRecordJson(string sid) =>
        "{\"sid\":\"" + sid + "\",\"ip_address\":\"203.0.113.10\",\"cidr_prefix_length\":32," +
        "\"account_sid\":\"" + AccSid + "\",\"date_created\":\"2026-06-27T00:00:00Z\"," +
        "\"date_updated\":\"2026-06-27T00:00:00Z\",\"url\":\"\"}";

    private static string SourceIpMappingJson(string sid, string il, string sd) =>
        "{\"sid\":\"" + sid + "\",\"ip_record_sid\":\"" + il + "\",\"sip_domain_sid\":\"" + sd + "\"," +
        "\"date_created\":\"2026-06-27T00:00:00Z\",\"date_updated\":\"2026-06-27T00:00:00Z\",\"url\":\"\"}";

    private static string ByocTrunkJson(string sid) =>
        "{\"sid\":\"" + sid + "\",\"cnam_lookup_enabled\":true," +
        "\"account_sid\":\"" + AccSid + "\",\"date_created\":\"2026-06-27T00:00:00Z\"," +
        "\"date_updated\":\"2026-06-27T00:00:00Z\",\"url\":\"\"}";

    private static string ConnectionPolicyTargetJson(string policySid, string targetSid) =>
        "{\"sid\":\"" + targetSid + "\",\"connection_policy_sid\":\"" + policySid + "\"," +
        "\"target\":\"sip:edge@example.com\",\"priority\":5,\"weight\":10,\"enabled\":true," +
        "\"account_sid\":\"" + AccSid + "\",\"date_created\":\"2026-06-27T00:00:00Z\"," +
        "\"date_updated\":\"2026-06-27T00:00:00Z\",\"url\":\"\"}";

    private static string PhoneNumberJson(string sid, string number, string? voiceRegion = null, string? friendlyName = null)
    {
        var fn = friendlyName is null ? "null" : "\"" + friendlyName + "\"";
        var vr = voiceRegion is null ? "null" : "\"" + voiceRegion + "\"";
        return "{\"sid\":\"" + sid + "\",\"phone_number\":\"" + number + "\"," +
               "\"account_sid\":\"" + AccSid + "\",\"friendly_name\":" + fn + "," +
               "\"voice_region\":" + vr + "," +
               "\"url\":\"https://voiceml.voicetel.com/v2/PhoneNumbers/" + number + "\"," +
               "\"date_created\":\"2026-06-27T00:00:00Z\",\"date_updated\":\"2026-06-27T00:00:00Z\"}";
    }

    // ---- Test infrastructure ------------------------------------------------

    private static VoiceMLClient NewClient(HttpMessageHandler handler)
    {
        var http = new HttpClient(handler) { Timeout = TimeSpan.FromSeconds(5) };
        return new VoiceMLClient(new ClientOptions
        {
            AccountSid = AccSid,
            ApiKey = ApiKey,
            HttpClient = http,
            MaxRetries = 0,
        });
    }

    private static HttpResponseMessage Reply(HttpStatusCode status, string json)
        => new(status) { Content = new StringContent(json, Encoding.UTF8, "application/json") };

    /// <summary>Decode an x-www-form-urlencoded token: <c>+</c> → space, then percent-unescape.</summary>
    private static string DecodeForm(string s) => Uri.UnescapeDataString(s.Replace('+', ' '));

    private static Dictionary<string, string> ParseForm(string body)
    {
        var map = new Dictionary<string, string>();
        if (string.IsNullOrEmpty(body)) return map;
        foreach (var pair in body.Split('&'))
        {
            if (string.IsNullOrEmpty(pair)) continue;
            var idx = pair.IndexOf('=');
            if (idx < 0) continue;
            map[DecodeForm(pair.Substring(0, idx))] = DecodeForm(pair.Substring(idx + 1));
        }
        return map;
    }

    /// <summary>Extract every value of a repeated form key (used for Permission, Filters, Participant).</summary>
    private static List<string> ExtractRepeated(string body, string key)
    {
        var values = new List<string>();
        if (string.IsNullOrEmpty(body)) return values;
        foreach (var pair in body.Split('&'))
        {
            if (string.IsNullOrEmpty(pair)) continue;
            var idx = pair.IndexOf('=');
            if (idx < 0) continue;
            if (DecodeForm(pair.Substring(0, idx)) == key)
            {
                values.Add(DecodeForm(pair.Substring(idx + 1)));
            }
        }
        return values;
    }

    private sealed class MockHandler : HttpMessageHandler
    {
        private readonly Func<HttpRequestMessage, HttpResponseMessage> _responder;
        public MockHandler(Func<HttpRequestMessage, HttpResponseMessage> responder) => _responder = responder;
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken ct)
            => Task.FromResult(_responder(request));
    }
}
