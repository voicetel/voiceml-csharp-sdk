using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using VoiceML.Exceptions;
using VoiceML.Models;

namespace VoiceML.Resources;

/// <summary>Account-scoped <c>/Notifications</c> compat stubs (always empty list; fetch returns 404).</summary>
public sealed class NotificationsResource : ResourceBase
{
    /// <summary>Construct with the shared transport.</summary>
    public NotificationsResource(Transport transport) : base(transport) { }

    /// <summary>List account notifications. Server always returns an empty page (compat stub).</summary>
    public async Task<NotificationsList> ListAsync(
        ListNotificationsParams? filter = null, CancellationToken ct = default)
    {
        var p = filter ?? new ListNotificationsParams();
        var result = await Transport.SendAsync<NotificationsList>(
            HttpMethod.Get,
            Path("Notifications"),
            queryParams: p.ToQuery(),
            ct: ct).ConfigureAwait(false);
        return result ?? new NotificationsList();
    }

    /// <summary>Fetch a single account notification. Always 404 today (compat stub).</summary>
    public async Task<Dictionary<string, object?>> GetAsync(
        string notificationSid, CancellationToken ct = default)
    {
        var result = await Transport.SendAsync<Dictionary<string, object?>>(
            HttpMethod.Get,
            Path("Notifications", notificationSid),
            ct: ct).ConfigureAwait(false);
        return result ?? throw new ApiException("empty body on GET /Notifications/{sid}", 200);
    }
}
