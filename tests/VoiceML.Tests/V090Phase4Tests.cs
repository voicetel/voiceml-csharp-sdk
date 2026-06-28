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

/// <summary>Wire-shape tests for the v0.9.0 Phase 4 surface — 15 service-scoped
/// resource families under <c>/v1/Services/{ChatServiceSid}/…</c>, 48 ops total.</summary>
public class V090Phase4Tests
{
    private const string AccSid = "AC" + "ffffffffffffffffffffffffffffffff";
    private const string ApiKey = "secret-key";
    private const string ChatSvcSid = "IS00000000000000000000000000000001";

    // ---- Wiring sanity ------------------------------------------------------

    [Fact]
    public void Phase4_Scope_AndAllSubResources_AreWired()
    {
        using var client = new VoiceMLClient(new ClientOptions { AccountSid = AccSid, ApiKey = ApiKey });
        var scope = client.ConversationsV1.Services.Scope(ChatSvcSid);
        Assert.NotNull(scope);
        Assert.Equal(ChatSvcSid, scope.ChatServiceSid);

        Assert.NotNull(scope.Conversations);
        Assert.NotNull(scope.Roles);
        Assert.NotNull(scope.Users);
        Assert.NotNull(scope.Bindings);
        Assert.NotNull(scope.Configuration);
        Assert.NotNull(scope.Configuration.Notifications);
        Assert.NotNull(scope.Configuration.Webhooks);
        Assert.NotNull(scope.ParticipantConversations);
        Assert.NotNull(scope.ConversationWithParticipants);

        // Doubly-scoped factories — these return new scopes each call.
        const string convSid = "CH00000000000000000000000000000099";
        const string msgSid = "IM00000000000000000000000000000099";
        const string userSid = "US00000000000000000000000000000099";
        Assert.NotNull(scope.Conversations.Messages(convSid));
        Assert.NotNull(scope.Conversations.Messages(convSid).Receipts(msgSid));
        Assert.NotNull(scope.Conversations.Participants(convSid));
        Assert.NotNull(scope.Conversations.Webhooks(convSid));
        Assert.NotNull(scope.Users.Conversations(userSid));
    }

    // ---- ServiceConversation (5 ops) ---------------------------------------

    [Fact]
    public async Task ServiceConversation_Create_BuildsServiceScopedPath()
    {
        const string convSid = "CH00000000000000000000000000000101";
        Uri? captured = null;
        var handler = new MockHandler(req =>
        {
            captured = req.RequestUri;
            Assert.Equal(HttpMethod.Post, req.Method);
            var form = ParseForm(req.Content!.ReadAsStringAsync().Result);
            Assert.Equal("eu-thread", form["FriendlyName"]);
            Assert.Equal("active", form["State"]);
            Assert.Equal("PT5M", form["Timers.Inactive"]);
            return Reply(HttpStatusCode.Created, ServiceConversationJson(convSid, state: "active"));
        });
        using var client = NewClient(handler);
        var c = await client.ConversationsV1.Services.Scope(ChatSvcSid).Conversations.CreateAsync(
            new CreateServiceConversationRequest
            {
                FriendlyName = "eu-thread",
                State = "active",
                TimersInactive = "PT5M",
            });
        Assert.Equal(convSid, c.Sid);
        Assert.Equal(ChatSvcSid, c.ChatServiceSid);
        Assert.NotNull(captured);
        Assert.Equal(
            $"https://voiceml.voicetel.com/v1/Services/{ChatSvcSid}/Conversations",
            captured!.ToString());
        Assert.DoesNotContain(AccSid, captured.ToString());
    }

    [Fact]
    public async Task ServiceConversation_List_HasMetaEnvelope()
    {
        const string convSid = "CH00000000000000000000000000000102";
        var handler = new MockHandler(req =>
        {
            Assert.Equal(HttpMethod.Get, req.Method);
            Assert.Equal(
                $"https://voiceml.voicetel.com/v1/Services/{ChatSvcSid}/Conversations?PageSize=10",
                req.RequestUri!.ToString());
            var body = "{\"conversations\":[" + ServiceConversationJson(convSid, state: "active") + "]," +
                       "\"meta\":{\"page\":0,\"page_size\":10,\"key\":\"conversations\"}}";
            return Reply(HttpStatusCode.OK, body);
        });
        using var client = NewClient(handler);
        var list = await client.ConversationsV1.Services.Scope(ChatSvcSid).Conversations.ListAsync(
            new ListV1PageParams { PageSize = 10 });
        Assert.Single(list.Conversations);
        Assert.Equal(convSid, list.Conversations[0].Sid);
        Assert.NotNull(list.Meta);
        Assert.Equal("conversations", list.Meta!.Key);
    }

    [Fact]
    public async Task ServiceConversation_FetchUpdateDelete()
    {
        const string convSid = "CH00000000000000000000000000000103";
        int call = 0;
        var handler = new MockHandler(req =>
        {
            call++;
            Assert.Equal(
                $"https://voiceml.voicetel.com/v1/Services/{ChatSvcSid}/Conversations/{convSid}",
                req.RequestUri!.ToString());
            return call switch
            {
                1 => Reply(HttpStatusCode.OK, ServiceConversationJson(convSid, state: "active")),
                2 => Reply(HttpStatusCode.OK, ServiceConversationJson(convSid, state: "closed")),
                _ => Reply(HttpStatusCode.NoContent, ""),
            };
        });
        using var client = NewClient(handler);
        var scope = client.ConversationsV1.Services.Scope(ChatSvcSid);
        var c1 = await scope.Conversations.GetAsync(convSid);
        Assert.Equal("active", c1.State);
        var c2 = await scope.Conversations.UpdateAsync(convSid,
            new UpdateServiceConversationRequest { State = "closed" });
        Assert.Equal("closed", c2.State);
        await scope.Conversations.DeleteAsync(convSid);
        Assert.Equal(3, call);
    }

    // ---- ServiceConversationMessage + Receipts ------------------------------

    [Fact]
    public async Task ServiceMessage_Scoped_BuildsCorrectPath()
    {
        const string convSid = "CH00000000000000000000000000000104";
        const string msgSid = "IM00000000000000000000000000000101";
        var handler = new MockHandler(req =>
        {
            Assert.Equal(HttpMethod.Post, req.Method);
            Assert.Equal(
                $"https://voiceml.voicetel.com/v1/Services/{ChatSvcSid}/Conversations/{convSid}/Messages",
                req.RequestUri!.ToString());
            var form = ParseForm(req.Content!.ReadAsStringAsync().Result);
            Assert.Equal("alice", form["Author"]);
            Assert.Equal("hi", form["Body"]);
            return Reply(HttpStatusCode.Created, ServiceMessageJson(convSid, msgSid));
        });
        using var client = NewClient(handler);
        var msg = await client.ConversationsV1.Services.Scope(ChatSvcSid).Conversations.Messages(convSid)
            .CreateAsync(new CreateServiceConversationMessageRequest { Author = "alice", Body = "hi" });
        Assert.Equal(msgSid, msg.Sid);
        Assert.Equal(ChatSvcSid, msg.ChatServiceSid);
    }

    [Fact]
    public async Task ServiceMessage_FetchUpdateDelete()
    {
        const string convSid = "CH00000000000000000000000000000105";
        const string msgSid = "IM00000000000000000000000000000102";
        int call = 0;
        var handler = new MockHandler(req =>
        {
            call++;
            Assert.Equal(
                $"https://voiceml.voicetel.com/v1/Services/{ChatSvcSid}/Conversations/{convSid}/Messages/{msgSid}",
                req.RequestUri!.ToString());
            return call < 3
                ? Reply(HttpStatusCode.OK, ServiceMessageJson(convSid, msgSid))
                : Reply(HttpStatusCode.NoContent, "");
        });
        using var client = NewClient(handler);
        var msgs = client.ConversationsV1.Services.Scope(ChatSvcSid).Conversations.Messages(convSid);
        await msgs.GetAsync(msgSid);
        await msgs.UpdateAsync(msgSid, new UpdateServiceConversationMessageRequest { Body = "edited" });
        await msgs.DeleteAsync(msgSid);
        Assert.Equal(3, call);
    }

    [Fact]
    public async Task ServiceMessage_List()
    {
        const string convSid = "CH00000000000000000000000000000106";
        var handler = new MockHandler(req =>
        {
            Assert.Equal(HttpMethod.Get, req.Method);
            Assert.Equal(
                $"https://voiceml.voicetel.com/v1/Services/{ChatSvcSid}/Conversations/{convSid}/Messages",
                req.RequestUri!.ToString());
            return Reply(HttpStatusCode.OK,
                "{\"messages\":[], \"meta\":{\"page\":0,\"page_size\":50,\"key\":\"messages\"}}");
        });
        using var client = NewClient(handler);
        var list = await client.ConversationsV1.Services.Scope(ChatSvcSid).Conversations.Messages(convSid).ListAsync();
        Assert.Empty(list.Messages);
    }

    [Fact]
    public async Task ServiceReceipts_TriplyScoped_BuildsCorrectPath()
    {
        const string convSid = "CH00000000000000000000000000000107";
        const string msgSid = "IM00000000000000000000000000000103";
        const string rcptSid = "DY00000000000000000000000000000001";
        int call = 0;
        var handler = new MockHandler(req =>
        {
            call++;
            Assert.Equal(HttpMethod.Get, req.Method);
            var basePath =
                $"https://voiceml.voicetel.com/v1/Services/{ChatSvcSid}/Conversations/{convSid}/Messages/{msgSid}/Receipts";
            if (call == 1)
            {
                Assert.Equal(basePath, req.RequestUri!.ToString());
                return Reply(HttpStatusCode.OK,
                    "{\"delivery_receipts\":[], \"meta\":{\"page\":0,\"page_size\":50,\"key\":\"delivery_receipts\"}}");
            }
            Assert.Equal($"{basePath}/{rcptSid}", req.RequestUri!.ToString());
            return Reply(HttpStatusCode.OK,
                "{\"sid\":\"" + rcptSid + "\",\"message_sid\":\"" + msgSid + "\",\"status\":\"delivered\"," +
                "\"error_code\":0,\"account_sid\":\"" + AccSid + "\"," +
                "\"date_created\":\"2026-06-27T00:00:00Z\",\"date_updated\":\"2026-06-27T00:00:00Z\",\"url\":\"\"}");
        });
        using var client = NewClient(handler);
        var receipts = client.ConversationsV1.Services.Scope(ChatSvcSid).Conversations.Messages(convSid).Receipts(msgSid);
        var list = await receipts.ListAsync();
        Assert.Empty(list.DeliveryReceipts);
        var r = await receipts.GetAsync(rcptSid);
        Assert.Equal("delivered", r.Status);
    }

    // ---- ServiceConversationParticipant -------------------------------------

    [Fact]
    public async Task ServiceParticipant_Create_SendsDottedFormKeys()
    {
        const string convSid = "CH00000000000000000000000000000108";
        const string partSid = "MB00000000000000000000000000000001";
        var handler = new MockHandler(req =>
        {
            Assert.Equal(
                $"https://voiceml.voicetel.com/v1/Services/{ChatSvcSid}/Conversations/{convSid}/Participants",
                req.RequestUri!.ToString());
            var form = ParseForm(req.Content!.ReadAsStringAsync().Result);
            Assert.Equal("+15551112222", form["MessagingBinding.Address"]);
            Assert.Equal("+15553334444", form["MessagingBinding.ProxyAddress"]);
            return Reply(HttpStatusCode.Created, ServiceParticipantJson(convSid, partSid));
        });
        using var client = NewClient(handler);
        var p = await client.ConversationsV1.Services.Scope(ChatSvcSid)
            .Conversations.Participants(convSid)
            .CreateAsync(new CreateServiceConversationParticipantRequest
            {
                MessagingBindingAddress = "+15551112222",
                MessagingBindingProxyAddress = "+15553334444",
            });
        Assert.Equal(partSid, p.Sid);
    }

    [Fact]
    public async Task ServiceParticipant_List_FetchUpdateDelete()
    {
        const string convSid = "CH00000000000000000000000000000109";
        const string partSid = "MB00000000000000000000000000000002";
        int call = 0;
        var handler = new MockHandler(req =>
        {
            call++;
            if (call == 1)
            {
                Assert.Equal(HttpMethod.Get, req.Method);
                return Reply(HttpStatusCode.OK,
                    "{\"participants\":[], \"meta\":{\"page\":0,\"page_size\":50,\"key\":\"participants\"}}");
            }
            return call < 4
                ? Reply(HttpStatusCode.OK, ServiceParticipantJson(convSid, partSid))
                : Reply(HttpStatusCode.NoContent, "");
        });
        using var client = NewClient(handler);
        var parts = client.ConversationsV1.Services.Scope(ChatSvcSid).Conversations.Participants(convSid);
        await parts.ListAsync();
        await parts.GetAsync(partSid);
        await parts.UpdateAsync(partSid, new UpdateServiceConversationParticipantRequest { RoleSid = "RLxx" });
        await parts.DeleteAsync(partSid);
        Assert.Equal(4, call);
    }

    // ---- ServiceConversationScopedWebhook -----------------------------------

    [Fact]
    public async Task ServiceConversationWebhook_Create_RequiresTarget()
    {
        const string convSid = "CH00000000000000000000000000000110";
        const string whSid = "WH00000000000000000000000000000001";
        var handler = new MockHandler(req =>
        {
            Assert.Equal(
                $"https://voiceml.voicetel.com/v1/Services/{ChatSvcSid}/Conversations/{convSid}/Webhooks",
                req.RequestUri!.ToString());
            var form = ParseForm(req.Content!.ReadAsStringAsync().Result);
            Assert.Equal("webhook", form["Target"]);
            Assert.Equal("https://hooks.example.com/c", form["Configuration.Url"]);
            return Reply(HttpStatusCode.Created, ServiceWebhookJson(convSid, whSid));
        });
        using var client = NewClient(handler);
        var wh = await client.ConversationsV1.Services.Scope(ChatSvcSid).Conversations.Webhooks(convSid)
            .CreateAsync(new CreateServiceConversationScopedWebhookRequest
            {
                Target = "webhook",
                ConfigurationUrl = "https://hooks.example.com/c",
                ConfigurationMethod = "POST",
            });
        Assert.Equal(whSid, wh.Sid);
    }

    [Fact]
    public async Task ServiceConversationWebhook_FetchUpdateDelete_List()
    {
        const string convSid = "CH00000000000000000000000000000111";
        const string whSid = "WH00000000000000000000000000000002";
        int call = 0;
        var handler = new MockHandler(req =>
        {
            call++;
            if (call == 1)
            {
                return Reply(HttpStatusCode.OK,
                    "{\"webhooks\":[], \"meta\":{\"page\":0,\"page_size\":50,\"key\":\"webhooks\"}}");
            }
            return call < 4
                ? Reply(HttpStatusCode.OK, ServiceWebhookJson(convSid, whSid))
                : Reply(HttpStatusCode.NoContent, "");
        });
        using var client = NewClient(handler);
        var whs = client.ConversationsV1.Services.Scope(ChatSvcSid).Conversations.Webhooks(convSid);
        await whs.ListAsync();
        await whs.GetAsync(whSid);
        await whs.UpdateAsync(whSid, new UpdateServiceConversationScopedWebhookRequest { ConfigurationMethod = "GET" });
        await whs.DeleteAsync(whSid);
        Assert.Equal(4, call);
    }

    // ---- ServiceConversationWithParticipants --------------------------------

    [Fact]
    public async Task ServiceConversationWithParticipants_Create()
    {
        const string convSid = "CH00000000000000000000000000000112";
        var handler = new MockHandler(req =>
        {
            Assert.Equal(HttpMethod.Post, req.Method);
            Assert.Equal(
                $"https://voiceml.voicetel.com/v1/Services/{ChatSvcSid}/ConversationWithParticipants",
                req.RequestUri!.ToString());
            var raw = req.Content!.ReadAsStringAsync().Result;
            var participants = ExtractRepeated(raw, "Participant");
            Assert.Equal(2, participants.Count);
            return Reply(HttpStatusCode.Created, ServiceConversationJson(convSid, state: "active"));
        });
        using var client = NewClient(handler);
        var c = await client.ConversationsV1.Services.Scope(ChatSvcSid).ConversationWithParticipants
            .CreateAsync(new CreateServiceConversationWithParticipantsRequest
            {
                FriendlyName = "kickoff",
                Participant = new[]
                {
                    "{\"identity\":\"alice\"}",
                    "{\"messaging_binding\":{\"address\":\"+15551112222\"}}",
                },
            });
        Assert.Equal(convSid, c.Sid);
    }

    // ---- ServiceParticipantConversation -------------------------------------

    [Fact]
    public async Task ServiceParticipantConversations_QueryParams()
    {
        var handler = new MockHandler(req =>
        {
            Assert.Equal(HttpMethod.Get, req.Method);
            Assert.StartsWith(
                $"https://voiceml.voicetel.com/v1/Services/{ChatSvcSid}/ParticipantConversations",
                req.RequestUri!.ToString());
            Assert.Contains("Identity=alice", req.RequestUri.ToString());
            Assert.Contains("Address=%2B15551234567", req.RequestUri.ToString());
            return Reply(HttpStatusCode.OK,
                "{\"conversations\":[], \"meta\":{\"page\":0,\"page_size\":50,\"key\":\"conversations\"}}");
        });
        using var client = NewClient(handler);
        var list = await client.ConversationsV1.Services.Scope(ChatSvcSid).ParticipantConversations.ListAsync(
            new ListServiceParticipantConversationsParams { Identity = "alice", Address = "+15551234567" });
        Assert.Empty(list.Conversations);
    }

    // ---- ServiceUserConversation (nested under Users) -----------------------

    [Fact]
    public async Task ServiceUserConversations_ScopedUnderUsers()
    {
        const string userSid = "US00000000000000000000000000000001";
        var handler = new MockHandler(req =>
        {
            Assert.Equal(HttpMethod.Get, req.Method);
            Assert.Equal(
                $"https://voiceml.voicetel.com/v1/Services/{ChatSvcSid}/Users/{userSid}/Conversations",
                req.RequestUri!.ToString());
            return Reply(HttpStatusCode.OK,
                "{\"conversations\":[], \"meta\":{\"page\":0,\"page_size\":50,\"key\":\"conversations\"}}");
        });
        using var client = NewClient(handler);
        var list = await client.ConversationsV1.Services.Scope(ChatSvcSid).Users.Conversations(userSid).ListAsync();
        Assert.Empty(list.Conversations);
    }

    // ---- ServiceRole --------------------------------------------------------

    [Fact]
    public async Task ServiceRole_Create_EmitsRepeatedPermissionField()
    {
        const string roleSid = "RL00000000000000000000000000000001";
        var handler = new MockHandler(req =>
        {
            Assert.Equal(
                $"https://voiceml.voicetel.com/v1/Services/{ChatSvcSid}/Roles",
                req.RequestUri!.ToString());
            var raw = req.Content!.ReadAsStringAsync().Result;
            var perms = ExtractRepeated(raw, "Permission");
            Assert.Equal(2, perms.Count);
            Assert.Contains("sendMessage", perms);
            Assert.Contains("editAnyMessage", perms);
            return Reply(HttpStatusCode.Created, ServiceRoleJson(roleSid));
        });
        using var client = NewClient(handler);
        var r = await client.ConversationsV1.Services.Scope(ChatSvcSid).Roles.CreateAsync(
            new CreateServiceRoleRequest
            {
                FriendlyName = "support",
                Type = "conversation",
                Permission = new[] { "sendMessage", "editAnyMessage" },
            });
        Assert.Equal(roleSid, r.Sid);
        Assert.Equal(ChatSvcSid, r.ChatServiceSid);
    }

    [Fact]
    public async Task ServiceRole_List_FetchUpdateDelete()
    {
        const string roleSid = "RL00000000000000000000000000000002";
        int call = 0;
        var handler = new MockHandler(req =>
        {
            call++;
            if (call == 1)
            {
                return Reply(HttpStatusCode.OK,
                    "{\"roles\":[], \"meta\":{\"page\":0,\"page_size\":50,\"key\":\"roles\"}}");
            }
            return call < 4
                ? Reply(HttpStatusCode.OK, ServiceRoleJson(roleSid))
                : Reply(HttpStatusCode.NoContent, "");
        });
        using var client = NewClient(handler);
        var roles = client.ConversationsV1.Services.Scope(ChatSvcSid).Roles;
        await roles.ListAsync();
        await roles.GetAsync(roleSid);
        await roles.UpdateAsync(roleSid, new UpdateServiceRoleRequest { Permission = new[] { "x" } });
        await roles.DeleteAsync(roleSid);
        Assert.Equal(4, call);
    }

    // ---- ServiceUser --------------------------------------------------------

    [Fact]
    public async Task ServiceUser_Create_RequiresIdentity()
    {
        const string userSid = "US00000000000000000000000000000002";
        var handler = new MockHandler(req =>
        {
            Assert.Equal(
                $"https://voiceml.voicetel.com/v1/Services/{ChatSvcSid}/Users",
                req.RequestUri!.ToString());
            var form = ParseForm(req.Content!.ReadAsStringAsync().Result);
            Assert.Equal("alice", form["Identity"]);
            return Reply(HttpStatusCode.Created, ServiceUserJson(userSid));
        });
        using var client = NewClient(handler);
        var u = await client.ConversationsV1.Services.Scope(ChatSvcSid).Users.CreateAsync(
            new CreateServiceUserRequest { Identity = "alice" });
        Assert.Equal(userSid, u.Sid);
    }

    [Fact]
    public async Task ServiceUser_List_FetchUpdateDelete()
    {
        const string userSid = "US00000000000000000000000000000003";
        int call = 0;
        var handler = new MockHandler(req =>
        {
            call++;
            if (call == 1)
            {
                return Reply(HttpStatusCode.OK,
                    "{\"users\":[], \"meta\":{\"page\":0,\"page_size\":50,\"key\":\"users\"}}");
            }
            return call < 4
                ? Reply(HttpStatusCode.OK, ServiceUserJson(userSid))
                : Reply(HttpStatusCode.NoContent, "");
        });
        using var client = NewClient(handler);
        var users = client.ConversationsV1.Services.Scope(ChatSvcSid).Users;
        await users.ListAsync();
        await users.GetAsync(userSid);
        await users.UpdateAsync(userSid, new UpdateServiceUserRequest { FriendlyName = "Alice" });
        await users.DeleteAsync(userSid);
        Assert.Equal(4, call);
    }

    // ---- ServiceBinding (list+fetch+delete only) ----------------------------

    [Fact]
    public async Task ServiceBinding_List_QueryParams()
    {
        var handler = new MockHandler(req =>
        {
            Assert.Equal(HttpMethod.Get, req.Method);
            Assert.StartsWith(
                $"https://voiceml.voicetel.com/v1/Services/{ChatSvcSid}/Bindings",
                req.RequestUri!.ToString());
            Assert.Contains("BindingType=apn", req.RequestUri.ToString());
            Assert.Contains("Identity=alice", req.RequestUri.ToString());
            return Reply(HttpStatusCode.OK,
                "{\"bindings\":[], \"meta\":{\"page\":0,\"page_size\":50,\"key\":\"bindings\"}}");
        });
        using var client = NewClient(handler);
        var list = await client.ConversationsV1.Services.Scope(ChatSvcSid).Bindings.ListAsync(
            new ListServiceBindingsParams { BindingType = "apn", Identity = "alice" });
        Assert.Empty(list.Bindings);
    }

    [Fact]
    public async Task ServiceBinding_FetchAndDelete()
    {
        const string bSid = "BS00000000000000000000000000000001";
        int call = 0;
        var handler = new MockHandler(req =>
        {
            call++;
            Assert.Equal(
                $"https://voiceml.voicetel.com/v1/Services/{ChatSvcSid}/Bindings/{bSid}",
                req.RequestUri!.ToString());
            return call == 1
                ? Reply(HttpStatusCode.OK,
                    "{\"sid\":\"" + bSid + "\",\"binding_type\":\"apn\"," +
                    "\"account_sid\":\"" + AccSid + "\",\"chat_service_sid\":\"" + ChatSvcSid + "\"," +
                    "\"date_created\":\"2026-06-27T00:00:00Z\",\"date_updated\":\"2026-06-27T00:00:00Z\",\"url\":\"\"}")
                : Reply(HttpStatusCode.NoContent, "");
        });
        using var client = NewClient(handler);
        var bindings = client.ConversationsV1.Services.Scope(ChatSvcSid).Bindings;
        var b = await bindings.GetAsync(bSid);
        Assert.Equal(bSid, b.Sid);
        Assert.Equal("apn", b.BindingType);
        await bindings.DeleteAsync(bSid);
        Assert.Equal(2, call);
    }

    // ---- ServiceConfiguration (+ Notifications + Webhooks) ------------------

    [Fact]
    public async Task ServiceConfiguration_FetchAndUpdate_SerializesBoolReachability()
    {
        int call = 0;
        var handler = new MockHandler(req =>
        {
            call++;
            Assert.Equal(
                $"https://voiceml.voicetel.com/v1/Services/{ChatSvcSid}/Configuration",
                req.RequestUri!.ToString());
            if (req.Method == HttpMethod.Post)
            {
                var form = ParseForm(req.Content!.ReadAsStringAsync().Result);
                Assert.Equal("true", form["ReachabilityEnabled"]);
                Assert.Equal("RLcreator", form["DefaultConversationCreatorRoleSid"]);
            }
            return Reply(HttpStatusCode.OK,
                "{\"chat_service_sid\":\"" + ChatSvcSid + "\",\"reachability_enabled\":true," +
                "\"url\":\"https://voiceml.voicetel.com/v1/Services/" + ChatSvcSid + "/Configuration\"}");
        });
        using var client = NewClient(handler);
        var cfg = await client.ConversationsV1.Services.Scope(ChatSvcSid).Configuration.FetchAsync();
        Assert.True(cfg.ReachabilityEnabled);
        await client.ConversationsV1.Services.Scope(ChatSvcSid).Configuration.UpdateAsync(
            new UpdateServiceConfigurationRequest
            {
                DefaultConversationCreatorRoleSid = "RLcreator",
                ReachabilityEnabled = true,
            });
        Assert.Equal(2, call);
    }

    [Fact]
    public async Task ServiceNotification_FetchAndUpdate_DottedKeys()
    {
        int call = 0;
        var handler = new MockHandler(req =>
        {
            call++;
            Assert.Equal(
                $"https://voiceml.voicetel.com/v1/Services/{ChatSvcSid}/Configuration/Notifications",
                req.RequestUri!.ToString());
            if (req.Method == HttpMethod.Post)
            {
                var form = ParseForm(req.Content!.ReadAsStringAsync().Result);
                Assert.Equal("true", form["LogEnabled"]);
                Assert.Equal("true", form["NewMessage.Enabled"]);
                Assert.Equal("ring.aiff", form["NewMessage.Sound"]);
                Assert.Equal("false", form["AddedToConversation.Enabled"]);
                Assert.Equal("default.aiff", form["RemovedFromConversation.Sound"]);
            }
            return Reply(HttpStatusCode.OK,
                "{\"chat_service_sid\":\"" + ChatSvcSid + "\",\"log_enabled\":true," +
                "\"url\":\"https://voiceml.voicetel.com/v1/Services/" + ChatSvcSid + "/Configuration/Notifications\"}");
        });
        using var client = NewClient(handler);
        var n = await client.ConversationsV1.Services.Scope(ChatSvcSid).Configuration.Notifications.FetchAsync();
        Assert.True(n.LogEnabled);
        await client.ConversationsV1.Services.Scope(ChatSvcSid).Configuration.Notifications.UpdateAsync(
            new UpdateServiceNotificationRequest
            {
                LogEnabled = true,
                NewMessageEnabled = true,
                NewMessageSound = "ring.aiff",
                AddedToConversationEnabled = false,
                RemovedFromConversationSound = "default.aiff",
            });
        Assert.Equal(2, call);
    }

    [Fact]
    public async Task ServiceWebhookConfiguration_FetchAndUpdate_FiltersRepeated()
    {
        int call = 0;
        var handler = new MockHandler(req =>
        {
            call++;
            Assert.Equal(
                $"https://voiceml.voicetel.com/v1/Services/{ChatSvcSid}/Configuration/Webhooks",
                req.RequestUri!.ToString());
            if (req.Method == HttpMethod.Post)
            {
                var raw = req.Content!.ReadAsStringAsync().Result;
                var filters = ExtractRepeated(raw, "Filters");
                Assert.Equal(2, filters.Count);
                Assert.Contains("onMessageAdded", filters);
                Assert.Contains("onConversationUpdated", filters);
                var form = ParseForm(raw);
                Assert.Equal("POST", form["Method"]);
                Assert.Equal("https://hooks.example.com/post", form["PostWebhookUrl"]);
            }
            return Reply(HttpStatusCode.OK,
                "{\"chat_service_sid\":\"" + ChatSvcSid + "\",\"method\":\"POST\"," +
                "\"filters\":[\"onMessageAdded\",\"onConversationUpdated\"]," +
                "\"pre_webhook_url\":null,\"post_webhook_url\":\"https://hooks.example.com/post\"," +
                "\"url\":\"https://voiceml.voicetel.com/v1/Services/" + ChatSvcSid + "/Configuration/Webhooks\"}");
        });
        using var client = NewClient(handler);
        var w = await client.ConversationsV1.Services.Scope(ChatSvcSid).Configuration.Webhooks.FetchAsync();
        Assert.Equal("POST", w.Method);
        Assert.NotNull(w.Filters);
        Assert.Equal(2, w.Filters!.Count);
        await client.ConversationsV1.Services.Scope(ChatSvcSid).Configuration.Webhooks.UpdateAsync(
            new UpdateServiceWebhookConfigurationRequest
            {
                Method = "POST",
                Filters = new[] { "onMessageAdded", "onConversationUpdated" },
                PostWebhookUrl = "https://hooks.example.com/post",
            });
        Assert.Equal(2, call);
    }

    // ---- JSON builders ------------------------------------------------------

    private static string ServiceConversationJson(string sid, string state) =>
        "{\"sid\":\"" + sid + "\",\"account_sid\":\"" + AccSid + "\"," +
        "\"chat_service_sid\":\"" + ChatSvcSid + "\",\"state\":\"" + state + "\"," +
        "\"attributes\":\"{}\",\"date_created\":\"2026-06-27T00:00:00Z\"," +
        "\"date_updated\":\"2026-06-27T00:00:00Z\"," +
        "\"url\":\"https://voiceml.voicetel.com/v1/Services/" + ChatSvcSid + "/Conversations/" + sid + "\"}";

    private static string ServiceMessageJson(string convSid, string msgSid) =>
        "{\"sid\":\"" + msgSid + "\",\"conversation_sid\":\"" + convSid + "\"," +
        "\"chat_service_sid\":\"" + ChatSvcSid + "\",\"index\":0," +
        "\"author\":\"alice\",\"body\":\"hi\",\"attributes\":\"{}\"," +
        "\"account_sid\":\"" + AccSid + "\",\"date_created\":\"2026-06-27T00:00:00Z\"," +
        "\"date_updated\":\"2026-06-27T00:00:00Z\",\"url\":\"\"}";

    private static string ServiceParticipantJson(string convSid, string partSid) =>
        "{\"sid\":\"" + partSid + "\",\"conversation_sid\":\"" + convSid + "\"," +
        "\"chat_service_sid\":\"" + ChatSvcSid + "\",\"attributes\":\"{}\"," +
        "\"account_sid\":\"" + AccSid + "\",\"date_created\":\"2026-06-27T00:00:00Z\"," +
        "\"date_updated\":\"2026-06-27T00:00:00Z\",\"url\":\"\"}";

    private static string ServiceWebhookJson(string convSid, string whSid) =>
        "{\"sid\":\"" + whSid + "\",\"conversation_sid\":\"" + convSid + "\"," +
        "\"chat_service_sid\":\"" + ChatSvcSid + "\",\"target\":\"webhook\"," +
        "\"account_sid\":\"" + AccSid + "\",\"date_created\":\"2026-06-27T00:00:00Z\"," +
        "\"date_updated\":\"2026-06-27T00:00:00Z\",\"url\":\"\"}";

    private static string ServiceRoleJson(string sid) =>
        "{\"sid\":\"" + sid + "\",\"type\":\"conversation\"," +
        "\"chat_service_sid\":\"" + ChatSvcSid + "\"," +
        "\"permissions\":[\"sendMessage\",\"editAnyMessage\"]," +
        "\"account_sid\":\"" + AccSid + "\",\"date_created\":\"2026-06-27T00:00:00Z\"," +
        "\"date_updated\":\"2026-06-27T00:00:00Z\",\"url\":\"\"}";

    private static string ServiceUserJson(string sid) =>
        "{\"sid\":\"" + sid + "\",\"identity\":\"alice\"," +
        "\"chat_service_sid\":\"" + ChatSvcSid + "\",\"attributes\":\"{}\"," +
        "\"account_sid\":\"" + AccSid + "\",\"date_created\":\"2026-06-27T00:00:00Z\"," +
        "\"date_updated\":\"2026-06-27T00:00:00Z\",\"url\":\"\"}";

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
