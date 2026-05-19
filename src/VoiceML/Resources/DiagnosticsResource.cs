using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using VoiceML.Exceptions;
using VoiceML.Models;

namespace VoiceML.Resources;

/// <summary>Diagnostic endpoints — <c>/health</c> and <c>/openapi.json</c>.
/// <para>These do NOT sit under <c>/2010-04-01/Accounts/{AccountSid}/…</c>; they're mounted at
/// the server root and don't require authentication. To match Twilio behavior, we still
/// send the Basic-auth header (servers ignore it on these paths).</para>
/// </summary>
public sealed class DiagnosticsResource
{
    private readonly Transport _transport;

    /// <summary>Construct with the shared transport.</summary>
    public DiagnosticsResource(Transport transport)
    {
        _transport = transport;
    }

    /// <summary>Hit <c>GET /health</c>. <c>200</c> = all hard checks pass; <c>503</c> throws
    /// <see cref="ServerException"/> with the failure list on <c>error.Body</c>.</summary>
    public async Task<HealthStatus> HealthAsync(CancellationToken ct = default)
    {
        var result = await _transport.SendAsync<HealthStatus>(
            HttpMethod.Get, "/health", ct: ct).ConfigureAwait(false);
        return result ?? new HealthStatus();
    }

    /// <summary>Fetch the OpenAPI spec as a JSON document.</summary>
    public async Task<JsonDocument> OpenApiJsonAsync(CancellationToken ct = default)
    {
        var (_, bytes, _, _) = await _transport.FetchBytesAsync("/openapi.json", ct).ConfigureAwait(false);
        return JsonDocument.Parse(bytes);
    }
}
