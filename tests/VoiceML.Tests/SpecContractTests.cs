using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using VoiceML.Models;
using Xunit;
using Xunit.Abstractions;

namespace VoiceML.Tests;

// Adapted from the review reflection harness. The public spec, rather than the
// implementation, supplies operation templates, allowed/required keys and auth.
public sealed class SpecContractTests
{
    private readonly ITestOutputHelper _output;
    public SpecContractTests(ITestOutputHelper output) => _output = output;
    private const string Account = "AC00000000000000000000000000000000";
    private readonly JsonDocument _document = JsonDocument.Parse(File.ReadAllText(
        Path.Combine(AppContext.BaseDirectory, "Fixtures", "voiceml-openapi.json")));
    private readonly HashSet<string> _covered = new();
    private readonly Dictionary<string, HashSet<string>> _sentParameters = new();
    private string _current = "";

    private JsonElement Resolve(JsonElement value)
    {
        if (value.ValueKind == JsonValueKind.Object && value.TryGetProperty("$ref", out var reference))
        {
            var node = _document.RootElement;
            foreach (var part in reference.GetString()!.Split('/').Skip(1)) node = node.GetProperty(part);
            return node;
        }
        return value;
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task EveryPublicOperationMatchesSpec(bool nullPayload)
    {
        _covered.Clear();
        _sentParameters.Clear();
        using var handler = new Handler(async request =>
        {
            var matches = new List<(string Path, JsonElement Item, JsonElement Op)>();
            foreach (var path in _document.RootElement.GetProperty("paths").EnumerateObject())
            {
                if (!path.Value.TryGetProperty(request.Method.Method.ToLowerInvariant(), out var op)) continue;
                var pattern = "^" + Regex.Escape(Regex.Replace(path.Name, @"\{[^}]+\}", "PARAMETER")).Replace("PARAMETER", "[^/]+") + "$";
                if (Regex.IsMatch(request.RequestUri!.AbsolutePath, pattern)) matches.Add((path.Name, path.Value, op));
            }
            Assert.True(matches.Count > 0, $"{_current}: {request.Method} {request.RequestUri}: matched no spec operation");
            // Literal routes such as Members/Front take precedence over {CallSid}.
            var match = matches.OrderBy(m => m.Path.Count(c => c == '{')).First();
            var id = match.Op.GetProperty("operationId").GetString()!;
            _covered.Add(id);
            var tags = match.Op.GetProperty("tags").EnumerateArray().Select(t => t.GetString()!).ToArray();
            var hosts = new HashSet<string>();
            if (tags.Any(t => t.StartsWith("ConversationsV1", StringComparison.Ordinal))) hosts.Add("conversations.voicetel.com");
            if (tags.Contains("MessagingV1Service")) hosts.Add("messaging.voicetel.com");
            if (hosts.Count == 0) hosts.Add("voiceml.voicetel.com");
            Assert.Contains(request.RequestUri!.Host, hosts);
            var noAuth = match.Op.TryGetProperty("security", out var security) && security.GetArrayLength() == 0;
            Assert.Equal(!noAuth, request.Headers.Authorization is not null);
            if (!noAuth)
                Assert.Equal("Basic " + Convert.ToBase64String(Encoding.UTF8.GetBytes(Account + ":contract-key")), request.Headers.Authorization!.ToString());

            var parameters = new List<JsonElement>();
            if (match.Item.TryGetProperty("parameters", out var pp)) parameters.AddRange(pp.EnumerateArray().Select(Resolve));
            if (match.Op.TryGetProperty("parameters", out var opParams)) parameters.AddRange(opParams.EnumerateArray().Select(Resolve));
            var query = ParsePairs(request.RequestUri.Query.TrimStart('?'));
            var allowedQuery = parameters.Where(p => p.GetProperty("in").GetString() == "query").ToArray();
            CheckKeys(id, query.Select(p => p.Key), allowedQuery.Select(p => p.GetProperty("name").GetString()!),
                allowedQuery.Where(p => p.TryGetProperty("required", out var r) && r.GetBoolean()).Select(p => p.GetProperty("name").GetString()!));

            var sentKeys = new List<string>();
            JsonElement bodySchema = default;
            if (match.Op.TryGetProperty("requestBody", out var rb))
            {
                rb = Resolve(rb);
                var content = rb.GetProperty("content");
                var ct = request.Content?.Headers.ContentType?.MediaType;
                Assert.NotNull(ct);
                Assert.True(content.TryGetProperty(ct!, out var media), $"{id}: unexpected content type {ct}");
                bodySchema = Resolve(media.GetProperty("schema"));
            }
            if (request.Content is not null)
            {
                var raw = await request.Content.ReadAsStringAsync();
                if (request.Content.Headers.ContentType!.MediaType == "application/json")
                {
                    using var json = JsonDocument.Parse(raw);
                    sentKeys.AddRange(json.RootElement.EnumerateObject().Select(p => p.Name));
                }
                else sentKeys.AddRange(ParsePairs(raw).Select(p => p.Key));
            }
            if (bodySchema.ValueKind == JsonValueKind.Object)
            {
                var allowed = bodySchema.TryGetProperty("properties", out var properties)
                    ? properties.EnumerateObject().Select(p => p.Name).ToList() : new List<string>();
                var required = bodySchema.TryGetProperty("required", out var req) ? req.EnumerateArray().Select(p => p.GetString()!) : Array.Empty<string>();
                CheckKeys(id, sentKeys, allowed, required);
            }
            else Assert.Empty(sentKeys);

            if (!_sentParameters.TryGetValue(id, out var sent)) _sentParameters[id] = sent = new();
            sent.UnionWith(query.Select(p => "query:" + p.Key));
            sent.UnionWith(sentKeys.Select(p => "body:" + p));

            var responseSchema = default(JsonElement);
            var status = 200;
            foreach (var r in match.Op.GetProperty("responses").EnumerateObject())
            {
                if (!int.TryParse(r.Name, out var code) || code < 200 || code >= 300) continue;
                status = code;
                var response = Resolve(r.Value);
                if (response.TryGetProperty("content", out var content) && content.TryGetProperty("application/json", out var json) && json.TryGetProperty("schema", out var declaredSchema)) responseSchema = declaredSchema;
                break;
            }
            var responseBody = responseSchema.ValueKind == JsonValueKind.Undefined ? "{}" : JsonSerializer.Serialize(Payload(responseSchema, nullPayload));
            return new HttpResponseMessage((HttpStatusCode)status) { Content = new StringContent(responseBody, Encoding.UTF8, "application/json") };
        });
        using var http = new HttpClient(handler);
        using var client = new VoiceMLClient(new ClientOptions { AccountSid = Account, ApiKey = "contract-key", HttpClient = http, MaxRetries = 0 });
        await Walk(client, "client", 0);
        var expected = _document.RootElement.GetProperty("paths").EnumerateObject()
            .SelectMany(p => p.Value.EnumerateObject().Where(o => o.Value.ValueKind == JsonValueKind.Object && o.Value.TryGetProperty("operationId", out _)))
            .Select(o => o.Value.GetProperty("operationId").GetString()!).ToHashSet();
        Assert.True(expected.SetEquals(_covered), "Uncovered operations: " + string.Join(", ", expected.Except(_covered)));
        _output.WriteLine($"Spec contract coverage: {_covered.Count}/{expected.Count}; nullable payload = {nullPayload}");
        Assert.Equal(333, _covered.Count);
        var missingOptional = new List<string>();
        foreach (var path in _document.RootElement.GetProperty("paths").EnumerateObject())
        {
            foreach (var operation in path.Value.EnumerateObject())
            {
                if (operation.Value.ValueKind != JsonValueKind.Object || !operation.Value.TryGetProperty("operationId", out var identifier)) continue;
                var id = identifier.GetString()!;
                var sent = _sentParameters[id];
                if (operation.Value.TryGetProperty("parameters", out var parameters))
                    foreach (var p in parameters.EnumerateArray().Select(Resolve).Where(p => p.GetProperty("in").GetString() == "query"))
                        if (!sent.Contains("query:" + p.GetProperty("name").GetString())) missingOptional.Add(id + " query:" + p.GetProperty("name").GetString());
                if (operation.Value.TryGetProperty("requestBody", out var requestBody))
                {
                    var content = Resolve(requestBody).GetProperty("content");
                    var schema = Resolve(content.EnumerateObject().First().Value.GetProperty("schema"));
                    if (schema.TryGetProperty("properties", out var properties))
                        foreach (var p in properties.EnumerateObject())
                            if (!sent.Contains("body:" + p.Name)) missingOptional.Add(id + " body:" + p.Name);
                }
            }
        }
        Assert.True(missingOptional.Count == 0, "Unexposed optional parameters: " + string.Join(", ", missingOptional));
    }

    private async Task Walk(object value, string prefix, int depth)
    {
        Assert.True(depth < 10, prefix);
        foreach (var p in value.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance))
            if (p.PropertyType.Namespace == "VoiceML.Resources" && p.GetValue(value) is { } child)
                await Walk(child, prefix + "." + p.Name, depth + 1);
        foreach (var method in value.GetType().GetMethods(BindingFlags.Public | BindingFlags.Instance))
        {
            if (method.IsSpecialName || method.DeclaringType?.Assembly != typeof(VoiceMLClient).Assembly) continue;
            // Continuation helpers require an actual returned URL; their origin checks and
            // verbatim wire behavior are exercised separately in RegressionTests.
            if (method.IsGenericMethodDefinition || method.Name.StartsWith("Next", StringComparison.Ordinal)) continue;
            var type = method.ReturnType;
            if (type.Namespace == "VoiceML.Resources")
            {
                var child = method.Invoke(value, method.GetParameters().Select(Dummy).ToArray());
                if (child is not null) await Walk(child, prefix + "." + method.Name, depth + 1);
                continue;
            }
            if (!typeof(Task).IsAssignableFrom(type) && !(type.IsGenericType && type.GetGenericTypeDefinition() == typeof(IAsyncEnumerable<>))) continue;
            _current = prefix + "." + method.Name;
            try
            {
                var result = method.Invoke(value, method.GetParameters().Select(Dummy).ToArray());
                if (result is Task task) await task;
                else if (result is not null)
                    await (Task)typeof(SpecContractTests).GetMethod(nameof(Drain), BindingFlags.NonPublic | BindingFlags.Static)!
                        .MakeGenericMethod(type.GetGenericArguments()[0]).Invoke(null, new[] { result })!;
            }
            catch (Exception ex) { throw new InvalidOperationException(_current, ex); }
        }
    }

    private static async Task Drain<T>(IAsyncEnumerable<T> sequence) { await foreach (var item in sequence) { _ = item; } }

    private static object? Dummy(ParameterInfo p) => Sample(p.ParameterType, p.Name ?? "value", 0);

    private static object? Sample(Type type, string name, int depth)
    {
        var t = Nullable.GetUnderlyingType(type) ?? type;
        if (t == typeof(CancellationToken)) return CancellationToken.None;
        if (t == typeof(string)) return name == "date" ? "2026-01-02" : name == "Attributes" || name == "ConversationAttributes" ? "{\"k\":1}" : "sample" + name;
        if (t == typeof(int)) return 2;
        if (t == typeof(long)) return 2L;
        if (t == typeof(bool)) return false;
        if (t == typeof(double)) return 1.5;
        if (t == typeof(float)) return 1.5f;
        if (t == typeof(decimal)) return 1.5m;
        if (t == typeof(JsonElement)) return JsonSerializer.SerializeToElement(new { key = "value" });
        if (t.IsEnum) return Enum.GetValues(t).GetValue(0);
        if (t == typeof(IEnumerable<KeyValuePair<string, string?>>)) return Array.Empty<KeyValuePair<string, string?>>();
        if (t == typeof(List<string>) || t == typeof(IEnumerable<string>) || t == typeof(IReadOnlyList<string>)) return new List<string> { "first", "second" };
        if (t == typeof(string[])) return new[] { "first", "second" };
        if (t == typeof(object)) return new { key = "value" };
        if (depth > 3) return null;
        if (t.IsClass && !t.IsAbstract && (t.Namespace == "VoiceML.Models" || t.GetConstructor(Type.EmptyTypes) is not null))
        {
            var instance = Activator.CreateInstance(t);
            foreach (var p in t.GetProperties().Where(p => p.CanWrite && p.GetIndexParameters().Length == 0))
            {
                var v = Sample(p.PropertyType, p.Name, depth + 1);
                if (v is not null) p.SetValue(instance, v);
            }
            return instance;
        }
        return null;
    }

    private object? Payload(JsonElement schema, bool nulls, int depth = 0)
    {
        schema = Resolve(schema);
        if (depth > 6) return new Dictionary<string, object?>();
        var type = schema.TryGetProperty("type", out var t) ? t : default;
        var nullable = (schema.TryGetProperty("nullable", out var n) && n.GetBoolean()) || (type.ValueKind == JsonValueKind.Array && type.EnumerateArray().Any(x => x.GetString() == "null"));
        if (nulls && nullable) return null;
        var kind = type.ValueKind == JsonValueKind.Array ? type.EnumerateArray().First(x => x.GetString() != "null").GetString() : type.ValueKind == JsonValueKind.String ? type.GetString() : "object";
        if (kind == "array") return new[] { Payload(schema.GetProperty("items"), nulls, depth + 1) };
        if (kind == "integer") return 1;
        if (kind == "number") return 1.25;
        if (kind == "boolean") return false;
        if (kind == "string") return "future-value";
        var result = new Dictionary<string, object?>();
        if (schema.TryGetProperty("properties", out var properties))
            foreach (var p in properties.EnumerateObject()) result[p.Name] = p.Name is "next_page_url" or "next_page_uri" ? null : Payload(p.Value, nulls, depth + 1);
        result["future_field"] = "ignored";
        return result;
    }

    private static void CheckKeys(string id, IEnumerable<string> actual, IEnumerable<string> allowed, IEnumerable<string> required)
    {
        var keys = actual.ToHashSet();
        Assert.True(keys.IsSubsetOf(allowed), id + " undeclared keys: " + string.Join(", ", keys.Except(allowed)));
        Assert.True(required.All(keys.Contains), id + " missing required keys: " + string.Join(", ", required.Except(keys)));
    }

    internal static List<KeyValuePair<string, string>> ParsePairs(string raw) => raw.Length == 0 ? new() : raw.Split('&').Select(pair =>
    {
        var parts = pair.Split('=', 2);
        return new KeyValuePair<string, string>(Uri.UnescapeDataString(parts[0].Replace("+", " ")), Uri.UnescapeDataString((parts.Length == 2 ? parts[1] : "").Replace("+", " ")));
    }).ToList();

    internal sealed class Handler : HttpMessageHandler
    {
        private readonly Func<HttpRequestMessage, Task<HttpResponseMessage>> _respond;
        internal Handler(Func<HttpRequestMessage, Task<HttpResponseMessage>> respond) => _respond = respond;
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken) => _respond(request);
    }
}
