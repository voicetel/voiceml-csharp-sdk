using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using VoiceML.Exceptions;
using VoiceML.Models;

namespace VoiceML.Resources;

/// <summary>Usage Record operations
/// (<c>/2010-04-01/Accounts/{AccountSid}/Usage/Records*.json</c>) — one record per usage
/// category over the requested window. <see cref="ListAsync"/> (the bare
/// <c>/Usage/Records</c> endpoint) is the same resource as <see cref="ListAllTimeAsync"/>.
/// The eight window methods each aggregate over their own bucket: <c>Daily</c> and
/// <c>Yearly</c> emit one record per category per bucket, newest bucket first.</summary>
public sealed class UsageRecordsResource : ResourceBase
{
    internal UsageRecordsResource(Transport transport) : base(transport) { }

    /// <summary>List Usage Records over a window (all-time window by default).
    /// <see cref="ListUsageRecordsParams.StartDate"/> / <see cref="ListUsageRecordsParams.EndDate"/>
    /// are inclusive <c>YYYY-MM-DD</c> dates in GMT, or offsets such as <c>-30days</c>.</summary>
    public Task<UsageRecordList> ListAsync(ListUsageRecordsParams? filter = null, CancellationToken ct = default)
        => ListWindowAsync("Records", filter, ct);

    /// <summary>List Usage Records aggregated over all time.</summary>
    public Task<UsageRecordList> ListAllTimeAsync(ListUsageRecordsParams? filter = null, CancellationToken ct = default)
        => ListWindowAsync("Records/AllTime", filter, ct);

    /// <summary>List Usage Records per UTC day, newest day first.</summary>
    public Task<UsageRecordList> ListDailyAsync(ListUsageRecordsParams? filter = null, CancellationToken ct = default)
        => ListWindowAsync("Records/Daily", filter, ct);

    /// <summary>List Usage Records over the previous calendar month.</summary>
    public Task<UsageRecordList> ListLastMonthAsync(ListUsageRecordsParams? filter = null, CancellationToken ct = default)
        => ListWindowAsync("Records/LastMonth", filter, ct);

    /// <summary>List Usage Records per calendar month.</summary>
    public Task<UsageRecordList> ListMonthlyAsync(ListUsageRecordsParams? filter = null, CancellationToken ct = default)
        => ListWindowAsync("Records/Monthly", filter, ct);

    /// <summary>List Usage Records over the current calendar month.</summary>
    public Task<UsageRecordList> ListThisMonthAsync(ListUsageRecordsParams? filter = null, CancellationToken ct = default)
        => ListWindowAsync("Records/ThisMonth", filter, ct);

    /// <summary>List Usage Records over today.</summary>
    public Task<UsageRecordList> ListTodayAsync(ListUsageRecordsParams? filter = null, CancellationToken ct = default)
        => ListWindowAsync("Records/Today", filter, ct);

    /// <summary>List Usage Records per calendar year.</summary>
    public Task<UsageRecordList> ListYearlyAsync(ListUsageRecordsParams? filter = null, CancellationToken ct = default)
        => ListWindowAsync("Records/Yearly", filter, ct);

    /// <summary>List Usage Records over yesterday.</summary>
    public Task<UsageRecordList> ListYesterdayAsync(ListUsageRecordsParams? filter = null, CancellationToken ct = default)
        => ListWindowAsync("Records/Yesterday", filter, ct);

    /// <summary>Fetch the page a list response links to next, following its
    /// <see cref="Page.NextPageUri"/> verbatim. Returns <c>null</c> when the page is the last
    /// one.</summary>
    public Task<UsageRecordList?> NextPageAsync(UsageRecordList page, CancellationToken ct = default)
        => string.IsNullOrEmpty(page.NextPageUri)
            ? Task.FromResult<UsageRecordList?>(null)
            : Transport.SendPageUriAsync<UsageRecordList>(page.NextPageUri!, ct);

    /// <summary>Iterate through all usage records across pages, yielding one
    /// <see cref="UsageRecord"/> at a time.</summary>
    public async IAsyncEnumerable<UsageRecord> IterateAsync(
        ListUsageRecordsParams? filter = null,
        [EnumeratorCancellation] CancellationToken ct = default)
    {
        var page = await ListAsync(filter, ct).ConfigureAwait(false);
        while (true)
        {
            foreach (var item in page.UsageRecords)
            {
                yield return item;
            }
            if (string.IsNullOrEmpty(page.NextPageUri))
            {
                yield break;
            }
            page = await NextPageAsync(page, ct).ConfigureAwait(false)
                ?? throw new VoiceMLException("empty page while following next_page_uri");
        }
    }

    private async Task<UsageRecordList> ListWindowAsync(
        string window, ListUsageRecordsParams? filter, CancellationToken ct)
    {
        var p = filter ?? new ListUsageRecordsParams();
        var result = await Transport.SendAsync<UsageRecordList>(
            HttpMethod.Get,
            Path(new[] { "Usage" }.Concat(window.Split('/')).ToArray()),
            queryParams: p.ToQuery(),
            ct: ct).ConfigureAwait(false);
        return result ?? new UsageRecordList();
    }
}
