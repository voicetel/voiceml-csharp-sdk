using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using VoiceML.Exceptions;
using VoiceML.Models;

namespace VoiceML.Resources;

/// <summary>Diagnostic endpoints — <c>/health</c>, <c>/openapi.json</c>, <c>/openapi.yaml</c>,
/// <c>/openapi.yml</c>.
/// <para>These are mounted at the server root, and the spec declares <c>security: []</c> on
/// them, so the SDK sends no <c>Authorization</c> header here. To call them without
/// configuring credentials at all, use the static <see cref="VoiceMLDiagnostics"/> helper.</para>
/// </summary>
public sealed class DiagnosticsResource
{
    private readonly Transport _transport;

    internal DiagnosticsResource(Transport transport)
    {
        _transport = transport;
    }

    /// <summary>Hit <c>GET /health</c>. <c>200</c> = all hard checks pass; <c>503</c> throws
    /// <see cref="ServerException"/> with the failure list on <see cref="ApiException.Body"/>.</summary>
    public async Task<HealthStatus> HealthAsync(CancellationToken ct = default)
    {
        var result = await _transport.SendAsync<HealthStatus>(
            HttpMethod.Get, "/health", sendAuth: false, ct: ct).ConfigureAwait(false);
        return result ?? new HealthStatus();
    }

    /// <summary>Fetch the OpenAPI spec as a JSON document. Parse failures raise
    /// <see cref="ApiException"/> with the <see cref="System.Text.Json.JsonException"/> as the
    /// inner exception.</summary>
    public async Task<JsonDocument> OpenApiJsonAsync(CancellationToken ct = default)
    {
        var (_, bytes, _, _, _) = await _transport.FetchBytesAsync("/openapi.json", sendAuth: false, ct).ConfigureAwait(false);
        return ParseJsonDocument(bytes);
    }

    /// <summary>Fetch the OpenAPI spec as raw YAML text (<c>GET /openapi.yaml</c>).</summary>
    public async Task<string> OpenApiYamlAsync(CancellationToken ct = default)
    {
        var (_, bytes, _, _, _) = await _transport.FetchBytesAsync("/openapi.yaml", sendAuth: false, ct).ConfigureAwait(false);
        return System.Text.Encoding.UTF8.GetString(bytes);
    }

    /// <summary>Fetch the OpenAPI spec as raw YAML text (<c>GET /openapi.yml</c>).</summary>
    public async Task<string> OpenApiYmlAsync(CancellationToken ct = default)
    {
        var (_, bytes, _, _, _) = await _transport.FetchBytesAsync("/openapi.yml", sendAuth: false, ct).ConfigureAwait(false);
        return System.Text.Encoding.UTF8.GetString(bytes);
    }

    internal static JsonDocument ParseJsonDocument(byte[] bytes)
    {
        try
        {
            return JsonDocument.Parse(bytes);
        }
        catch (JsonException ex)
        {
            throw new ApiException("could not decode /openapi.json response as JSON", 200, body: null, inner: ex);
        }
    }
}
