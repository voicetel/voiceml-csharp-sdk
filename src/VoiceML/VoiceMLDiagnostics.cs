using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using VoiceML.Exceptions;
using VoiceML.Models;
using VoiceML.Resources;

namespace VoiceML;

/// <summary>Standalone, credential-less access to the VoiceML diagnostic endpoints
/// (<c>/health</c>, <c>/openapi.json</c>, <c>/openapi.yaml</c>, <c>/openapi.yml</c>). These
/// endpoints are declared <c>security: []</c> in the spec, so no account or API key is needed.
/// Use this class from health-check jobs and probes that must not hold credentials; use
/// <see cref="VoiceMLClient.Diagnostics"/> when a client is already configured (both send no
/// <c>Authorization</c> header).
/// <para>Every call builds its own short-lived transport. For high-frequency probing, pass
/// your own shared <see cref="HttpClient"/> (it is not disposed).</para></summary>
public static class VoiceMLDiagnostics
{
    /// <summary>Hit <c>GET /health</c> on the given base URL. <c>200</c> = all hard checks
    /// pass; <c>503</c> throws <see cref="ServerException"/>.</summary>
    public static async Task<HealthStatus> HealthAsync(
        string baseUrl = ClientOptions.DefaultBaseUrl,
        HttpClient? httpClient = null,
        TimeSpan? timeout = null,
        CancellationToken ct = default)
    {
        using var transport = CreateTransport(baseUrl, httpClient, timeout);
        var result = await transport.SendAsync<HealthStatus>(
            HttpMethod.Get, "/health", sendAuth: false, ct: ct).ConfigureAwait(false);
        return result ?? new HealthStatus();
    }

    /// <summary>Fetch <c>GET /openapi.json</c> as a <see cref="JsonDocument"/>. Parse failures
    /// raise <see cref="ApiException"/> with the <see cref="JsonException"/> as the inner
    /// exception.</summary>
    public static async Task<JsonDocument> OpenApiJsonAsync(
        string baseUrl = ClientOptions.DefaultBaseUrl,
        HttpClient? httpClient = null,
        TimeSpan? timeout = null,
        CancellationToken ct = default)
    {
        using var transport = CreateTransport(baseUrl, httpClient, timeout);
        var (_, bytes, _, _, _) = await transport.FetchBytesAsync("/openapi.json", sendAuth: false, ct).ConfigureAwait(false);
        return DiagnosticsResource.ParseJsonDocument(bytes);
    }

    /// <summary>Fetch <c>GET /openapi.yaml</c> as raw YAML text.</summary>
    public static async Task<string> OpenApiYamlAsync(
        string baseUrl = ClientOptions.DefaultBaseUrl,
        HttpClient? httpClient = null,
        TimeSpan? timeout = null,
        CancellationToken ct = default)
    {
        using var transport = CreateTransport(baseUrl, httpClient, timeout);
        var (_, bytes, _, _, _) = await transport.FetchBytesAsync("/openapi.yaml", sendAuth: false, ct).ConfigureAwait(false);
        return System.Text.Encoding.UTF8.GetString(bytes);
    }

    /// <summary>Fetch <c>GET /openapi.yml</c> as raw YAML text.</summary>
    public static async Task<string> OpenApiYmlAsync(
        string baseUrl = ClientOptions.DefaultBaseUrl,
        HttpClient? httpClient = null,
        TimeSpan? timeout = null,
        CancellationToken ct = default)
    {
        using var transport = CreateTransport(baseUrl, httpClient, timeout);
        var (_, bytes, _, _, _) = await transport.FetchBytesAsync("/openapi.yml", sendAuth: false, ct).ConfigureAwait(false);
        return System.Text.Encoding.UTF8.GetString(bytes);
    }

    private static Transport CreateTransport(string baseUrl, HttpClient? httpClient, TimeSpan? timeout)
    {
        var options = new ClientOptions
        {
            AccountSid = "AC00000000000000000000000000000000",
            ApiKey = "diagnostics",
            BaseUrl = baseUrl,
            MaxRetries = 0,
            Timeout = timeout ?? ClientOptions.DefaultTimeout,
            HttpClient = httpClient,
        };
        return new Transport(options);
    }
}
