using System.Collections.Generic;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using VoiceML.Exceptions;
using VoiceML.Models;

namespace VoiceML.Resources;

/// <summary>Operations on <c>/v1/Alerts</c> — the Monitor v1 view over failure events
/// (call-scoped errors and messaging delivery failures alike), rendered in the Alert shape.
/// Served on the base host; the account resolves from HTTP Basic auth and list responses
/// carry the <see cref="VoiceV1Meta"/> envelope. Obtain via
/// <see cref="VoiceML.VoiceMLClient.MonitorAlerts"/>.</summary>
public sealed class MonitorAlertsResource
{
    private readonly Transport _transport;

    /// <summary>Fetch the /v1 page a list response links to next, following its
    /// <see cref="VoiceV1Meta.NextPageUrl"/> verbatim — but only when its scheme and host
    /// equal the configured product host, so credentials never go to a foreign host. Returns
    /// <c>null</c> when <paramref name="page"/> is the last one. Alerts are served as a
    /// single page, so this returns <c>null</c> for well-formed responses.</summary>
    public Task<T?> NextPageAsync<T>(T page, CancellationToken ct = default) where T : class, IV1PagedList
        => string.IsNullOrEmpty(page.Meta?.NextPageUrl)
            ? Task.FromResult<T?>(null)
            : _transport.SendV1NextPageAsync<T>(page.Meta!.NextPageUrl!, ct);

    internal MonitorAlertsResource(Transport transport) { _transport = transport; }

    /// <summary>List alerts (single page). The list omits the request/response detail — fetch
    /// an individual alert for that. Pass an empty <see cref="ListMonitorAlertsParams"/> for
    /// an unfiltered list.</summary>
    public async Task<MonitorAlertList> ListAsync(ListMonitorAlertsParams? filter = null, CancellationToken ct = default)
    {
        var p = filter ?? new ListMonitorAlertsParams();
        return await _transport.SendAsync<MonitorAlertList>(HttpMethod.Get,
            "/v1/Alerts", queryParams: p.ToQuery(), ct: ct).ConfigureAwait(false)
            ?? new MonitorAlertList();
    }

    /// <summary>Fetch one alert by SID, with the stored request/response detail when the row
    /// carries any. Route: <c>/v1/Alerts/{PathSegment.Encode(sid)}</c>.</summary>
    public async Task<MonitorAlert> GetAsync(string sid, CancellationToken ct = default)
    {
        var r = await _transport.SendAsync<MonitorAlert>(HttpMethod.Get,
            $"/v1/Alerts/{PathSegment.Encode(sid)}", ct: ct).ConfigureAwait(false);
        return r ?? throw new ApiException("empty body on GET /v1/Alerts/{sid}", 200);
    }

    /// <summary>Iterate through the alerts on the fetched pages, yielding one
    /// <see cref="MonitorAlert"/> at a time. Alerts have no continuation, so this yields a
    /// single page and stops.</summary>
    public async IAsyncEnumerable<MonitorAlert> IterateAsync(
        ListMonitorAlertsParams? filter = null,
        [EnumeratorCancellation] CancellationToken ct = default)
    {
        var page = await ListAsync(filter, ct).ConfigureAwait(false);
        while (true)
        {
            foreach (var item in page.Alerts)
            {
                yield return item;
            }
            if (string.IsNullOrEmpty(page.Meta?.NextPageUrl))
            {
                yield break;
            }
            page = await NextPageAsync(page, ct).ConfigureAwait(false)
                ?? throw new VoiceMLException("empty page while following next_page_url");
        }
    }
}
