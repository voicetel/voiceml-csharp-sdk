using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using VoiceML;
using VoiceML.Models;
using Xunit;

namespace VoiceML.Tests;

/// <summary>Wire-shape tests for the v0.9.1 Assistants v1 surface — 7 families,
/// 30 ops. Distinct from the rest of the SDK: request bodies are
/// <c>application/json</c> (not form-encoded), updates use HTTP PUT (not POST),
/// identifiers are opaque strings (not 34-char sids).</summary>
public class V091Tests
{
    private const string AccSid = "AC" + "ffffffffffffffffffffffffffffffff";
    private const string ApiKey = "secret-key";
    private const string BaseUrl = "https://voiceml.voicetel.com";

    private const string AssistantId = "aia_asst_alice";
    private const string ToolId = "aia_tool_lookup";
    private const string KnowledgeId = "aia_know_docs";
    private const string SessionId = "sess_xyz";
    private const string MessageId = "aia_msg_001";
    private const string FeedbackId = "aia_fdbk_001";

    // ---- Wiring sanity ------------------------------------------------------

    [Fact]
    public void AssistantsV1_AndAllSubResources_AreWired()
    {
        using var client = new VoiceMLClient(new ClientOptions { AccountSid = AccSid, ApiKey = ApiKey });
        Assert.NotNull(client.AssistantsV1);
        Assert.NotNull(client.AssistantsV1.Assistants);
        Assert.NotNull(client.AssistantsV1.Tools);
        Assert.NotNull(client.AssistantsV1.Knowledge);
        Assert.NotNull(client.AssistantsV1.Sessions);
        Assert.NotNull(client.AssistantsV1.Policies);

        // Per-Assistant scopes
        var aTools = client.AssistantsV1.Assistants.Tools(AssistantId);
        var aKnow = client.AssistantsV1.Assistants.Knowledge(AssistantId);
        var aFeed = client.AssistantsV1.Assistants.Feedbacks(AssistantId);
        var aMsgs = client.AssistantsV1.Assistants.Messages(AssistantId);
        Assert.Equal(AssistantId, aTools.AssistantId);
        Assert.Equal(AssistantId, aKnow.AssistantId);
        Assert.Equal(AssistantId, aFeed.AssistantId);
        Assert.Equal(AssistantId, aMsgs.AssistantId);

        // Per-Knowledge scopes
        var kStatus = client.AssistantsV1.Knowledge.Status(KnowledgeId);
        var kChunks = client.AssistantsV1.Knowledge.Chunks(KnowledgeId);
        Assert.Equal(KnowledgeId, kStatus.KnowledgeId);
        Assert.Equal(KnowledgeId, kChunks.KnowledgeId);

        // Per-Session scope
        var sMsgs = client.AssistantsV1.Sessions.Messages(SessionId);
        Assert.Equal(SessionId, sMsgs.SessionId);
    }

    // ---- Assistant family (5 CRUD) ------------------------------------------

    [Fact]
    public async Task Assistant_List_GET_WithPaging()
    {
        var handler = new MockHandler(req =>
        {
            Assert.Equal(HttpMethod.Get, req.Method);
            var u = req.RequestUri!.ToString();
            Assert.StartsWith($"{BaseUrl}/v1/Assistants", u);
            Assert.Contains("PageSize=25", u);
            Assert.Contains("PageToken=tok", u);
            Assert.DoesNotContain(AccSid, u);
            return Reply(HttpStatusCode.OK,
                "{\"assistants\":[" + AssistantJson(AssistantId) + "]," +
                "\"meta\":{\"page\":0,\"page_size\":25,\"key\":\"assistants\"}}");
        });
        using var client = NewClient(handler);
        var list = await client.AssistantsV1.Assistants.ListAsync(
            new ListAssistantsV1PageParams { PageSize = 25, PageToken = "tok" });
        Assert.Single(list.Assistants);
        Assert.Equal(AssistantId, list.Assistants[0].Id);
        Assert.Equal("assistants", list.Meta!.Key);
    }

    [Fact]
    public async Task Assistant_Create_POST_JsonBody_WithCustomerAi()
    {
        var handler = new MockHandler(req =>
        {
            Assert.Equal(HttpMethod.Post, req.Method);
            Assert.Equal($"{BaseUrl}/v1/Assistants", req.RequestUri!.ToString());
            Assert.Equal("application/json", req.Content!.Headers.ContentType!.MediaType);
            var body = req.Content.ReadAsStringAsync().Result;
            using var doc = JsonDocument.Parse(body);
            var root = doc.RootElement;
            Assert.Equal("alice", root.GetProperty("name").GetString());
            Assert.Equal("ops", root.GetProperty("owner").GetString());
            Assert.Equal("Be terse.", root.GetProperty("personality_prompt").GetString());
            Assert.Equal("gpt-4o-mini", root.GetProperty("model").GetString());
            var ca = root.GetProperty("customer_ai");
            Assert.True(ca.GetProperty("perception_engine_enabled").GetBoolean());
            Assert.False(ca.GetProperty("personalization_engine_enabled").GetBoolean());
            return Reply(HttpStatusCode.Created, AssistantJson(AssistantId));
        });
        using var client = NewClient(handler);
        var a = await client.AssistantsV1.Assistants.CreateAsync(new CreateAssistantRequest
        {
            Name = "alice",
            Owner = "ops",
            PersonalityPrompt = "Be terse.",
            Model = "gpt-4o-mini",
            CustomerAi = new AssistantsV1CustomerAi
            {
                PerceptionEngineEnabled = true,
                PersonalizationEngineEnabled = false,
            },
        });
        Assert.Equal(AssistantId, a.Id);
        Assert.Equal("alice", a.Name);
    }

    [Fact]
    public async Task Assistant_Get_GET_ReturnsWithToolsAndKnowledge()
    {
        var handler = new MockHandler(req =>
        {
            Assert.Equal(HttpMethod.Get, req.Method);
            Assert.Equal($"{BaseUrl}/v1/Assistants/{AssistantId}", req.RequestUri!.ToString());
            return Reply(HttpStatusCode.OK,
                "{\"id\":\"" + AssistantId + "\",\"name\":\"alice\"," +
                "\"account_sid\":\"" + AccSid + "\",\"owner\":\"ops\",\"model\":\"gpt-4o-mini\"," +
                "\"personality_prompt\":\"Be terse.\",\"customer_ai\":{}," +
                "\"date_created\":\"2026-06-27T00:00:00Z\",\"date_updated\":\"2026-06-27T00:00:00Z\"," +
                "\"tools\":[" + ToolJson(ToolId) + "]," +
                "\"knowledge\":[" + KnowledgeJson(KnowledgeId) + "]}");
        });
        using var client = NewClient(handler);
        var a = await client.AssistantsV1.Assistants.GetAsync(AssistantId);
        Assert.Equal(AssistantId, a.Id);
        Assert.Single(a.Tools);
        Assert.Equal(ToolId, a.Tools[0].Id);
        Assert.Single(a.Knowledge);
        Assert.Equal(KnowledgeId, a.Knowledge[0].Id);
    }

    [Fact]
    public async Task Assistant_Update_PUT_JsonBody()
    {
        var handler = new MockHandler(req =>
        {
            Assert.Equal(HttpMethod.Put, req.Method);
            Assert.Equal($"{BaseUrl}/v1/Assistants/{AssistantId}", req.RequestUri!.ToString());
            Assert.Equal("application/json", req.Content!.Headers.ContentType!.MediaType);
            var body = req.Content.ReadAsStringAsync().Result;
            using var doc = JsonDocument.Parse(body);
            Assert.Equal("Be very terse.", doc.RootElement.GetProperty("personality_prompt").GetString());
            // omitted nullable fields must NOT appear (DefaultIgnoreCondition.WhenWritingNull)
            Assert.False(doc.RootElement.TryGetProperty("name", out _));
            return Reply(HttpStatusCode.OK, AssistantJson(AssistantId));
        });
        using var client = NewClient(handler);
        var a = await client.AssistantsV1.Assistants.UpdateAsync(AssistantId,
            new UpdateAssistantRequest { PersonalityPrompt = "Be very terse." });
        Assert.Equal(AssistantId, a.Id);
    }

    [Fact]
    public async Task Assistant_Delete_DELETE_NoBody()
    {
        var handler = new MockHandler(req =>
        {
            Assert.Equal(HttpMethod.Delete, req.Method);
            Assert.Equal($"{BaseUrl}/v1/Assistants/{AssistantId}", req.RequestUri!.ToString());
            Assert.Null(req.Content);
            return Reply(HttpStatusCode.NoContent, "");
        });
        using var client = NewClient(handler);
        await client.AssistantsV1.Assistants.DeleteAsync(AssistantId);
    }

    // ---- Tool family (5 CRUD) ----------------------------------------------

    [Fact]
    public async Task Tool_List_GET_WithAssistantIdFilter()
    {
        var handler = new MockHandler(req =>
        {
            Assert.Equal(HttpMethod.Get, req.Method);
            var u = req.RequestUri!.ToString();
            Assert.StartsWith($"{BaseUrl}/v1/Tools", u);
            Assert.Contains("AssistantId=" + AssistantId, u);
            Assert.Contains("PageSize=10", u);
            return Reply(HttpStatusCode.OK,
                "{\"tools\":[" + ToolJson(ToolId) + "]," +
                "\"meta\":{\"page\":0,\"page_size\":10,\"key\":\"tools\"}}");
        });
        using var client = NewClient(handler);
        var list = await client.AssistantsV1.Tools.ListAsync(
            new ListAssistantsV1ToolsParams { AssistantId = AssistantId, PageSize = 10 });
        Assert.Single(list.Tools);
    }

    [Fact]
    public async Task Tool_Create_POST_JsonBody_RequiredFields()
    {
        var handler = new MockHandler(req =>
        {
            Assert.Equal(HttpMethod.Post, req.Method);
            Assert.Equal($"{BaseUrl}/v1/Tools", req.RequestUri!.ToString());
            var body = req.Content!.ReadAsStringAsync().Result;
            using var doc = JsonDocument.Parse(body);
            Assert.Equal("lookup", doc.RootElement.GetProperty("name").GetString());
            Assert.Equal("webhook", doc.RootElement.GetProperty("type").GetString());
            Assert.True(doc.RootElement.GetProperty("enabled").GetBoolean());
            return Reply(HttpStatusCode.Created, ToolJson(ToolId));
        });
        using var client = NewClient(handler);
        var t = await client.AssistantsV1.Tools.CreateAsync(new CreateAssistantToolRequest
        {
            Name = "lookup",
            Type = "webhook",
            Enabled = true,
        });
        Assert.Equal(ToolId, t.Id);
    }

    [Fact]
    public async Task Tool_Get_GET_ReturnsWithPolicies()
    {
        var handler = new MockHandler(req =>
        {
            Assert.Equal(HttpMethod.Get, req.Method);
            Assert.Equal($"{BaseUrl}/v1/Tools/{ToolId}", req.RequestUri!.ToString());
            return Reply(HttpStatusCode.OK,
                "{\"id\":\"" + ToolId + "\",\"name\":\"lookup\",\"type\":\"webhook\"," +
                "\"enabled\":true,\"requires_auth\":false," +
                "\"date_created\":\"2026-06-27T00:00:00Z\",\"date_updated\":\"2026-06-27T00:00:00Z\"," +
                "\"policies\":[{\"id\":\"aia_plcy_xx\",\"type\":\"rate_limit\",\"policy_details\":{}," +
                "\"date_created\":\"2026-06-27T00:00:00Z\",\"date_updated\":\"2026-06-27T00:00:00Z\"}]}");
        });
        using var client = NewClient(handler);
        var t = await client.AssistantsV1.Tools.GetAsync(ToolId);
        Assert.Equal(ToolId, t.Id);
        Assert.Single(t.Policies);
        Assert.Equal("aia_plcy_xx", t.Policies[0].Id);
    }

    [Fact]
    public async Task Tool_Update_PUT_JsonBody()
    {
        var handler = new MockHandler(req =>
        {
            Assert.Equal(HttpMethod.Put, req.Method);
            Assert.Equal($"{BaseUrl}/v1/Tools/{ToolId}", req.RequestUri!.ToString());
            var body = req.Content!.ReadAsStringAsync().Result;
            using var doc = JsonDocument.Parse(body);
            Assert.False(doc.RootElement.GetProperty("enabled").GetBoolean());
            return Reply(HttpStatusCode.OK, ToolJson(ToolId));
        });
        using var client = NewClient(handler);
        await client.AssistantsV1.Tools.UpdateAsync(ToolId,
            new UpdateAssistantToolRequest { Enabled = false });
    }

    [Fact]
    public async Task Tool_Delete_DELETE_NoBody()
    {
        var handler = new MockHandler(req =>
        {
            Assert.Equal(HttpMethod.Delete, req.Method);
            Assert.Equal($"{BaseUrl}/v1/Tools/{ToolId}", req.RequestUri!.ToString());
            return Reply(HttpStatusCode.NoContent, "");
        });
        using var client = NewClient(handler);
        await client.AssistantsV1.Tools.DeleteAsync(ToolId);
    }

    // ---- Per-Assistant Tools (list + attach + detach) -----------------------

    [Fact]
    public async Task AssistantTools_List_GET()
    {
        var handler = new MockHandler(req =>
        {
            Assert.Equal(HttpMethod.Get, req.Method);
            Assert.StartsWith(
                $"{BaseUrl}/v1/Assistants/{AssistantId}/Tools",
                req.RequestUri!.ToString());
            return Reply(HttpStatusCode.OK,
                "{\"tools\":[]," +
                "\"meta\":{\"page\":0,\"page_size\":50,\"key\":\"tools\"}}");
        });
        using var client = NewClient(handler);
        var list = await client.AssistantsV1.Assistants.Tools(AssistantId).ListAsync();
        Assert.Empty(list.Tools);
    }

    [Fact]
    public async Task AssistantTools_AttachDetach_POST_DELETE()
    {
        int call = 0;
        var handler = new MockHandler(req =>
        {
            call++;
            Assert.Equal(
                $"{BaseUrl}/v1/Assistants/{AssistantId}/Tools/{ToolId}",
                req.RequestUri!.ToString());
            Assert.Equal(call == 1 ? HttpMethod.Post : HttpMethod.Delete, req.Method);
            Assert.Null(req.Content);
            return Reply(HttpStatusCode.NoContent, "");
        });
        using var client = NewClient(handler);
        var tools = client.AssistantsV1.Assistants.Tools(AssistantId);
        await tools.AttachAsync(ToolId);
        await tools.DetachAsync(ToolId);
        Assert.Equal(2, call);
    }

    // ---- Knowledge family (5 CRUD + per-knowledge Status + Chunks + per-Assistant attach/detach = 10) ---

    [Fact]
    public async Task Knowledge_List_GET_WithAssistantIdFilter()
    {
        var handler = new MockHandler(req =>
        {
            Assert.Equal(HttpMethod.Get, req.Method);
            var u = req.RequestUri!.ToString();
            Assert.StartsWith($"{BaseUrl}/v1/Knowledge", u);
            Assert.Contains("AssistantId=" + AssistantId, u);
            return Reply(HttpStatusCode.OK,
                "{\"knowledge\":[" + KnowledgeJson(KnowledgeId) + "]," +
                "\"meta\":{\"page\":0,\"page_size\":50,\"key\":\"knowledge\"}}");
        });
        using var client = NewClient(handler);
        var list = await client.AssistantsV1.Knowledge.ListAsync(
            new ListAssistantsV1KnowledgeParams { AssistantId = AssistantId });
        Assert.Single(list.Knowledge);
        Assert.Equal(KnowledgeId, list.Knowledge[0].Id);
    }

    [Fact]
    public async Task Knowledge_Create_POST_JsonBody_RequiredFields()
    {
        var handler = new MockHandler(req =>
        {
            Assert.Equal(HttpMethod.Post, req.Method);
            Assert.Equal($"{BaseUrl}/v1/Knowledge", req.RequestUri!.ToString());
            var body = req.Content!.ReadAsStringAsync().Result;
            using var doc = JsonDocument.Parse(body);
            Assert.Equal("docs", doc.RootElement.GetProperty("name").GetString());
            Assert.Equal("web", doc.RootElement.GetProperty("type").GetString());
            Assert.Equal("text-embedding-3-small",
                doc.RootElement.GetProperty("embedding_model").GetString());
            return Reply(HttpStatusCode.Created, KnowledgeJson(KnowledgeId));
        });
        using var client = NewClient(handler);
        var k = await client.AssistantsV1.Knowledge.CreateAsync(new CreateAssistantKnowledgeRequest
        {
            Name = "docs",
            Type = "web",
            EmbeddingModel = "text-embedding-3-small",
        });
        Assert.Equal(KnowledgeId, k.Id);
    }

    [Fact]
    public async Task Knowledge_Get_PUT_Update_DELETE()
    {
        int call = 0;
        var handler = new MockHandler(req =>
        {
            call++;
            Assert.Equal($"{BaseUrl}/v1/Knowledge/{KnowledgeId}", req.RequestUri!.ToString());
            return call switch
            {
                1 => CheckMethod(req, HttpMethod.Get, KnowledgeJson(KnowledgeId)),
                2 => CheckMethod(req, HttpMethod.Put, KnowledgeJson(KnowledgeId)),
                _ => Reply(HttpStatusCode.NoContent, ""),
            };
        });
        using var client = NewClient(handler);
        var k1 = await client.AssistantsV1.Knowledge.GetAsync(KnowledgeId);
        Assert.Equal(KnowledgeId, k1.Id);
        await client.AssistantsV1.Knowledge.UpdateAsync(KnowledgeId,
            new UpdateAssistantKnowledgeRequest { Description = "updated" });
        await client.AssistantsV1.Knowledge.DeleteAsync(KnowledgeId);
        Assert.Equal(3, call);
    }

    [Fact]
    public async Task Knowledge_Status_GET()
    {
        var handler = new MockHandler(req =>
        {
            Assert.Equal(HttpMethod.Get, req.Method);
            Assert.Equal($"{BaseUrl}/v1/Knowledge/{KnowledgeId}/Status",
                req.RequestUri!.ToString());
            return Reply(HttpStatusCode.OK,
                "{\"status\":\"ready\",\"last_status\":\"ingesting\"," +
                "\"account_sid\":\"" + AccSid + "\"," +
                "\"date_updated\":\"2026-06-27T00:00:00Z\"}");
        });
        using var client = NewClient(handler);
        var s = await client.AssistantsV1.Knowledge.Status(KnowledgeId).FetchAsync();
        Assert.Equal("ready", s.Status);
        Assert.Equal("ingesting", s.LastStatus);
    }

    [Fact]
    public async Task Knowledge_Chunks_List_GET()
    {
        var handler = new MockHandler(req =>
        {
            Assert.Equal(HttpMethod.Get, req.Method);
            Assert.StartsWith($"{BaseUrl}/v1/Knowledge/{KnowledgeId}/Chunks",
                req.RequestUri!.ToString());
            return Reply(HttpStatusCode.OK,
                "{\"chunks\":[{\"content\":\"chunk1\",\"account_sid\":\"" + AccSid + "\"," +
                "\"date_created\":\"2026-06-27T00:00:00Z\",\"date_updated\":\"2026-06-27T00:00:00Z\"}]," +
                "\"meta\":{\"page\":0,\"page_size\":50,\"key\":\"chunks\"}}");
        });
        using var client = NewClient(handler);
        var list = await client.AssistantsV1.Knowledge.Chunks(KnowledgeId).ListAsync();
        Assert.Single(list.Chunks);
        Assert.Equal("chunk1", list.Chunks[0].Content);
    }

    [Fact]
    public async Task AssistantKnowledge_List_AttachDetach()
    {
        int call = 0;
        var handler = new MockHandler(req =>
        {
            call++;
            if (call == 1)
            {
                Assert.Equal(HttpMethod.Get, req.Method);
                Assert.StartsWith(
                    $"{BaseUrl}/v1/Assistants/{AssistantId}/Knowledge",
                    req.RequestUri!.ToString());
                return Reply(HttpStatusCode.OK,
                    "{\"knowledge\":[]," +
                    "\"meta\":{\"page\":0,\"page_size\":50,\"key\":\"knowledge\"}}");
            }
            Assert.Equal(
                $"{BaseUrl}/v1/Assistants/{AssistantId}/Knowledge/{KnowledgeId}",
                req.RequestUri!.ToString());
            Assert.Equal(call == 2 ? HttpMethod.Post : HttpMethod.Delete, req.Method);
            Assert.Null(req.Content);
            return Reply(HttpStatusCode.NoContent, "");
        });
        using var client = NewClient(handler);
        var k = client.AssistantsV1.Assistants.Knowledge(AssistantId);
        await k.ListAsync();
        await k.AttachAsync(KnowledgeId);
        await k.DetachAsync(KnowledgeId);
        Assert.Equal(3, call);
    }

    // ---- Session family (3) -------------------------------------------------

    [Fact]
    public async Task Session_List_GET()
    {
        var handler = new MockHandler(req =>
        {
            Assert.Equal(HttpMethod.Get, req.Method);
            Assert.StartsWith($"{BaseUrl}/v1/Sessions", req.RequestUri!.ToString());
            return Reply(HttpStatusCode.OK,
                "{\"sessions\":[" + SessionJson(SessionId) + "]," +
                "\"meta\":{\"page\":0,\"page_size\":50,\"key\":\"sessions\"}}");
        });
        using var client = NewClient(handler);
        var list = await client.AssistantsV1.Sessions.ListAsync();
        Assert.Single(list.Sessions);
        Assert.Equal(SessionId, list.Sessions[0].Id);
    }

    [Fact]
    public async Task Session_Get_GET()
    {
        var handler = new MockHandler(req =>
        {
            Assert.Equal(HttpMethod.Get, req.Method);
            Assert.Equal($"{BaseUrl}/v1/Sessions/{SessionId}", req.RequestUri!.ToString());
            return Reply(HttpStatusCode.OK, SessionJson(SessionId));
        });
        using var client = NewClient(handler);
        var s = await client.AssistantsV1.Sessions.GetAsync(SessionId);
        Assert.Equal(SessionId, s.Id);
        Assert.Equal(AssistantId, s.AssistantId);
    }

    [Fact]
    public async Task SessionMessages_List_GET()
    {
        var handler = new MockHandler(req =>
        {
            Assert.Equal(HttpMethod.Get, req.Method);
            Assert.StartsWith($"{BaseUrl}/v1/Sessions/{SessionId}/Messages",
                req.RequestUri!.ToString());
            return Reply(HttpStatusCode.OK,
                "{\"messages\":[" + MessageJson(MessageId) + "]," +
                "\"meta\":{\"page\":0,\"page_size\":50,\"key\":\"messages\"}}");
        });
        using var client = NewClient(handler);
        var list = await client.AssistantsV1.Sessions.Messages(SessionId).ListAsync();
        Assert.Single(list.Messages);
        Assert.Equal(MessageId, list.Messages[0].Id);
        Assert.Equal(SessionId, list.Messages[0].SessionId);
    }

    // ---- Message send (1) ---------------------------------------------------

    [Fact]
    public async Task AssistantMessages_Send_POST_JsonBody_ReturnsSendResponse()
    {
        var handler = new MockHandler(req =>
        {
            Assert.Equal(HttpMethod.Post, req.Method);
            Assert.Equal($"{BaseUrl}/v1/Assistants/{AssistantId}/Messages",
                req.RequestUri!.ToString());
            Assert.Equal("application/json", req.Content!.Headers.ContentType!.MediaType);
            var body = req.Content.ReadAsStringAsync().Result;
            using var doc = JsonDocument.Parse(body);
            Assert.Equal("alice", doc.RootElement.GetProperty("identity").GetString());
            Assert.Equal("hello", doc.RootElement.GetProperty("body").GetString());
            Assert.Equal(SessionId, doc.RootElement.GetProperty("session_id").GetString());
            Assert.Equal("sync", doc.RootElement.GetProperty("mode").GetString());
            return Reply(HttpStatusCode.OK,
                "{\"status\":\"ok\",\"flagged\":false,\"aborted\":false," +
                "\"session_id\":\"" + SessionId + "\"," +
                "\"account_sid\":\"" + AccSid + "\",\"body\":\"hi there\"}");
        });
        using var client = NewClient(handler);
        var r = await client.AssistantsV1.Assistants.Messages(AssistantId).SendAsync(
            new SendAssistantMessageRequest
            {
                Identity = "alice",
                Body = "hello",
                SessionId = SessionId,
                Mode = "sync",
            });
        Assert.Equal("ok", r.Status);
        Assert.Equal(SessionId, r.SessionId);
        Assert.Equal("hi there", r.Body);
    }

    // ---- Feedback family (2) ------------------------------------------------

    [Fact]
    public async Task AssistantFeedbacks_List_GET()
    {
        var handler = new MockHandler(req =>
        {
            Assert.Equal(HttpMethod.Get, req.Method);
            Assert.StartsWith(
                $"{BaseUrl}/v1/Assistants/{AssistantId}/Feedbacks",
                req.RequestUri!.ToString());
            return Reply(HttpStatusCode.OK,
                "{\"feedbacks\":[" + FeedbackJson(FeedbackId) + "]," +
                "\"meta\":{\"page\":0,\"page_size\":50,\"key\":\"feedbacks\"}}");
        });
        using var client = NewClient(handler);
        var list = await client.AssistantsV1.Assistants.Feedbacks(AssistantId).ListAsync();
        Assert.Single(list.Feedbacks);
        Assert.Equal(FeedbackId, list.Feedbacks[0].Id);
    }

    [Fact]
    public async Task AssistantFeedbacks_Create_POST_JsonBody_RequiredSessionId()
    {
        var handler = new MockHandler(req =>
        {
            Assert.Equal(HttpMethod.Post, req.Method);
            Assert.Equal($"{BaseUrl}/v1/Assistants/{AssistantId}/Feedbacks",
                req.RequestUri!.ToString());
            var body = req.Content!.ReadAsStringAsync().Result;
            using var doc = JsonDocument.Parse(body);
            Assert.Equal(SessionId, doc.RootElement.GetProperty("session_id").GetString());
            Assert.Equal(MessageId, doc.RootElement.GetProperty("message_id").GetString());
            Assert.Equal(0.75f, doc.RootElement.GetProperty("score").GetSingle());
            Assert.Equal("great", doc.RootElement.GetProperty("text").GetString());
            return Reply(HttpStatusCode.Created, FeedbackJson(FeedbackId));
        });
        using var client = NewClient(handler);
        var f = await client.AssistantsV1.Assistants.Feedbacks(AssistantId).CreateAsync(
            new CreateAssistantFeedbackRequest
            {
                SessionId = SessionId,
                MessageId = MessageId,
                Score = 0.75f,
                Text = "great",
            });
        Assert.Equal(FeedbackId, f.Id);
    }

    // ---- Policy family (1) --------------------------------------------------

    [Fact]
    public async Task Policies_List_GET_WithToolIdAndKnowledgeIdFilters()
    {
        var handler = new MockHandler(req =>
        {
            Assert.Equal(HttpMethod.Get, req.Method);
            var u = req.RequestUri!.ToString();
            Assert.StartsWith($"{BaseUrl}/v1/Policies", u);
            Assert.Contains("ToolId=" + ToolId, u);
            Assert.Contains("KnowledgeId=" + KnowledgeId, u);
            return Reply(HttpStatusCode.OK,
                "{\"policies\":[{\"id\":\"aia_plcy_xx\",\"type\":\"rate_limit\"," +
                "\"policy_details\":{\"requests_per_minute\":60}," +
                "\"date_created\":\"2026-06-27T00:00:00Z\"," +
                "\"date_updated\":\"2026-06-27T00:00:00Z\"}]," +
                "\"meta\":{\"page\":0,\"page_size\":50,\"key\":\"policies\"}}");
        });
        using var client = NewClient(handler);
        var list = await client.AssistantsV1.Policies.ListAsync(
            new ListAssistantsV1PoliciesParams
            {
                ToolId = ToolId,
                KnowledgeId = KnowledgeId,
            });
        Assert.Single(list.Policies);
        Assert.Equal("rate_limit", list.Policies[0].Type);
    }

    // ---- JSON builders ------------------------------------------------------

    private static string AssistantJson(string id) =>
        "{\"id\":\"" + id + "\",\"name\":\"alice\"," +
        "\"account_sid\":\"" + AccSid + "\",\"owner\":\"ops\"," +
        "\"model\":\"gpt-4o-mini\",\"personality_prompt\":\"Be terse.\"," +
        "\"customer_ai\":{}," +
        "\"date_created\":\"2026-06-27T00:00:00Z\"," +
        "\"date_updated\":\"2026-06-27T00:00:00Z\"," +
        "\"url\":\"" + BaseUrl + "/v1/Assistants/" + id + "\"}";

    private static string ToolJson(string id) =>
        "{\"id\":\"" + id + "\",\"name\":\"lookup\",\"type\":\"webhook\"," +
        "\"enabled\":true,\"requires_auth\":false,\"meta\":{}," +
        "\"date_created\":\"2026-06-27T00:00:00Z\"," +
        "\"date_updated\":\"2026-06-27T00:00:00Z\"," +
        "\"url\":\"" + BaseUrl + "/v1/Tools/" + id + "\"}";

    private static string KnowledgeJson(string id) =>
        "{\"id\":\"" + id + "\",\"name\":\"docs\",\"type\":\"web\"," +
        "\"status\":\"ready\"," +
        "\"date_created\":\"2026-06-27T00:00:00Z\"," +
        "\"date_updated\":\"2026-06-27T00:00:00Z\"," +
        "\"url\":\"" + BaseUrl + "/v1/Knowledge/" + id + "\"}";

    private static string SessionJson(string id) =>
        "{\"id\":\"" + id + "\",\"account_sid\":\"" + AccSid + "\"," +
        "\"assistant_id\":\"" + AssistantId + "\",\"identity\":\"alice\"," +
        "\"verified\":true," +
        "\"date_created\":\"2026-06-27T00:00:00Z\"," +
        "\"date_updated\":\"2026-06-27T00:00:00Z\"}";

    private static string MessageJson(string id) =>
        "{\"id\":\"" + id + "\",\"account_sid\":\"" + AccSid + "\"," +
        "\"assistant_id\":\"" + AssistantId + "\",\"session_id\":\"" + SessionId + "\"," +
        "\"identity\":\"alice\",\"role\":\"user\",\"content\":{}," +
        "\"date_created\":\"2026-06-27T00:00:00Z\"," +
        "\"date_updated\":\"2026-06-27T00:00:00Z\"}";

    private static string FeedbackJson(string id) =>
        "{\"id\":\"" + id + "\",\"account_sid\":\"" + AccSid + "\"," +
        "\"assistant_id\":\"" + AssistantId + "\",\"session_id\":\"" + SessionId + "\"," +
        "\"message_id\":\"" + MessageId + "\",\"score\":0.75,\"text\":\"great\"," +
        "\"date_created\":\"2026-06-27T00:00:00Z\"," +
        "\"date_updated\":\"2026-06-27T00:00:00Z\"}";

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

    private static HttpResponseMessage CheckMethod(HttpRequestMessage req, HttpMethod expected, string json)
    {
        Assert.Equal(expected, req.Method);
        return Reply(HttpStatusCode.OK, json);
    }

    private sealed class MockHandler : HttpMessageHandler
    {
        private readonly Func<HttpRequestMessage, HttpResponseMessage> _responder;
        public MockHandler(Func<HttpRequestMessage, HttpResponseMessage> responder) => _responder = responder;
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken ct)
            => Task.FromResult(_responder(request));
    }
}
