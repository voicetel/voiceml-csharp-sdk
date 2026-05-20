using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using VoiceML.Exceptions;
using VoiceML.Models;

namespace VoiceML.Resources;

/// <summary>Operations on <c>/IncomingPhoneNumbers</c> — tenant-self-serve DID management.
/// <para>Twilio shape: list/create/fetch/update/delete. Numbers are identified by their
/// canonical <c>PN</c>-prefixed sid in path parameters; the E.164 string is on the
/// <see cref="IncomingPhoneNumber.PhoneNumber"/> field of the returned resource.</para></summary>
public sealed class IncomingPhoneNumbersResource : ResourceBase
{
    /// <summary>Construct with the shared transport.</summary>
    public IncomingPhoneNumbersResource(Transport transport) : base(transport) { }

    /// <summary>List DIDs assigned to the authenticated tenant. Use
    /// <see cref="ListIncomingPhoneNumbersOptions.PhoneNumber"/> for the
    /// Twilio-canonical exact-match lookup pattern.</summary>
    public async Task<IncomingPhoneNumberList> ListAsync(
        ListIncomingPhoneNumbersOptions? options = null, CancellationToken ct = default)
    {
        var opts = options ?? new ListIncomingPhoneNumbersOptions();
        var result = await Transport.SendAsync<IncomingPhoneNumberList>(
            HttpMethod.Get,
            Path("IncomingPhoneNumbers"),
            queryParams: opts.ToQuery(),
            ct: ct).ConfigureAwait(false);
        return result ?? new IncomingPhoneNumberList();
    }

    /// <summary>Synchronous list wrapper. Blocks; prefer <see cref="ListAsync"/>.</summary>
    public IncomingPhoneNumberList List(ListIncomingPhoneNumbersOptions? options = null)
        => ListAsync(options).GetAwaiter().GetResult();

    /// <summary>Assign a DID to the authenticated tenant. Idempotent: re-POSTing the same
    /// number rebinds its voice routing without erroring.</summary>
    public async Task<IncomingPhoneNumber> CreateAsync(
        CreateIncomingPhoneNumberOptions options, CancellationToken ct = default)
    {
        var result = await Transport.SendAsync<IncomingPhoneNumber>(
            HttpMethod.Post,
            Path("IncomingPhoneNumbers"),
            formBody: options.ToForm(),
            ct: ct).ConfigureAwait(false);
        return result ?? throw new ApiException("empty body on POST /IncomingPhoneNumbers", 200);
    }

    /// <summary>Synchronous create wrapper. Blocks; prefer <see cref="CreateAsync"/>.</summary>
    public IncomingPhoneNumber Create(CreateIncomingPhoneNumberOptions options)
        => CreateAsync(options).GetAwaiter().GetResult();

    /// <summary>Fetch a single DID by SID.</summary>
    public async Task<IncomingPhoneNumber> GetAsync(string sid, CancellationToken ct = default)
    {
        var result = await Transport.SendAsync<IncomingPhoneNumber>(
            HttpMethod.Get,
            Path("IncomingPhoneNumbers", sid),
            ct: ct).ConfigureAwait(false);
        return result ?? throw new ApiException("empty body on GET /IncomingPhoneNumbers/{sid}", 200);
    }

    /// <summary>Synchronous fetch wrapper. Blocks; prefer <see cref="GetAsync"/>.</summary>
    public IncomingPhoneNumber Get(string sid)
        => GetAsync(sid).GetAwaiter().GetResult();

    /// <summary>Update voice routing on an assigned DID. Only-set-fields-touched.</summary>
    public async Task<IncomingPhoneNumber> UpdateAsync(
        string sid, UpdateIncomingPhoneNumberOptions options, CancellationToken ct = default)
    {
        var result = await Transport.SendAsync<IncomingPhoneNumber>(
            HttpMethod.Post,
            Path("IncomingPhoneNumbers", sid),
            formBody: options.ToForm(),
            ct: ct).ConfigureAwait(false);
        return result ?? throw new ApiException("empty body on POST /IncomingPhoneNumbers/{sid}", 200);
    }

    /// <summary>Synchronous update wrapper. Blocks; prefer <see cref="UpdateAsync"/>.</summary>
    public IncomingPhoneNumber Update(string sid, UpdateIncomingPhoneNumberOptions options)
        => UpdateAsync(sid, options).GetAwaiter().GetResult();

    /// <summary>Release a DID from the authenticated tenant. Idempotent: 204 on success
    /// OR if the number was already gone.</summary>
    public Task DeleteAsync(string sid, CancellationToken ct = default)
        => Transport.SendNoContentAsync(HttpMethod.Delete, Path("IncomingPhoneNumbers", sid), ct: ct);

    /// <summary>Synchronous delete wrapper. Blocks; prefer <see cref="DeleteAsync"/>.</summary>
    public void Delete(string sid)
        => DeleteAsync(sid).GetAwaiter().GetResult();
}
