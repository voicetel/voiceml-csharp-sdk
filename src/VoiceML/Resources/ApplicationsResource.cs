using System.Collections.Generic;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using VoiceML.Exceptions;
using VoiceML.Models;

namespace VoiceML.Resources;

/// <summary>Operations on <c>/Applications</c>.</summary>
public sealed class ApplicationsResource : ResourceBase
{
    /// <summary>Construct with the shared transport.</summary>
    public ApplicationsResource(Transport transport) : base(transport) { }

    /// <summary>Create an application bundle.</summary>
    public async Task<Application> CreateAsync(CreateApplicationRequest request, CancellationToken ct = default)
    {
        var result = await Transport.SendAsync<Application>(
            HttpMethod.Post,
            Path("Applications"),
            formBody: request.ToForm(),
            ct: ct).ConfigureAwait(false);
        return result ?? throw new ApiException("empty body on POST /Applications", 200);
    }

    /// <summary>List applications.</summary>
    public async Task<ApplicationList> ListAsync(ListApplicationsParams? filter = null, CancellationToken ct = default)
    {
        var p = filter ?? new ListApplicationsParams();
        var result = await Transport.SendAsync<ApplicationList>(
            HttpMethod.Get, Path("Applications"), queryParams: p.ToQuery(), ct: ct).ConfigureAwait(false);
        return result ?? new ApplicationList();
    }

    /// <summary>Iterate through all applications across pages, yielding one
    /// <see cref="Application"/> at a time.</summary>
    public async IAsyncEnumerable<Application> IterateAsync(
        ListApplicationsParams? filter = null,
        int page = 0,
        int? pageSize = null,
        [EnumeratorCancellation] CancellationToken ct = default)
    {
        var p = filter ?? new ListApplicationsParams();
        while (true)
        {
            var chunk = await ListAsync(
                p with { Page = page, PageSize = pageSize ?? p.PageSize },
                ct).ConfigureAwait(false);
            foreach (var item in chunk.Applications) yield return item;
            if (string.IsNullOrEmpty(chunk.NextPageUri) || chunk.Applications.Count == 0) yield break;
            page++;
        }
    }

    /// <summary>Fetch an application by SID.</summary>
    public async Task<Application> GetAsync(string applicationSid, CancellationToken ct = default)
    {
        var result = await Transport.SendAsync<Application>(
            HttpMethod.Get, Path("Applications", applicationSid), ct: ct).ConfigureAwait(false);
        return result ?? throw new ApiException("empty body on GET /Applications/{sid}", 200);
    }

    /// <summary>Update an application — partial; only set fields are touched.</summary>
    public async Task<Application> UpdateAsync(
        string applicationSid, UpdateApplicationRequest request, CancellationToken ct = default)
    {
        var result = await Transport.SendAsync<Application>(
            HttpMethod.Post,
            Path("Applications", applicationSid),
            formBody: request.ToForm(),
            ct: ct).ConfigureAwait(false);
        return result ?? throw new ApiException("empty body on POST /Applications/{sid}", 200);
    }

    /// <summary>Delete an application.</summary>
    public Task DeleteAsync(string applicationSid, CancellationToken ct = default)
        => Transport.SendNoContentAsync(HttpMethod.Delete, Path("Applications", applicationSid), ct: ct);
}
