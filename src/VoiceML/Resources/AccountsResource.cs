using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using VoiceML.Exceptions;
using VoiceML.Models;

namespace VoiceML.Resources;

/// <summary>Operations on the authenticated account itself
/// (<c>/2010-04-01/Accounts/{Sid}.json</c>) — self-serve fetch and update. An account may
/// only read and write its own record.</summary>
public sealed class AccountsResource : ResourceBase
{
    internal AccountsResource(Transport transport) : base(transport) { }

    /// <summary>Fetch the authenticated account. The response carries the canonical fields
    /// plus VoiceML extension keys (voice routing, BYO STT/TTS, AWS recording storage, IP
    /// allow-list); the account secret is never emitted.</summary>
    public async Task<Account> GetAsync(string accountSid, CancellationToken ct = default)
    {
        var result = await Transport.SendAsync<Account>(
            HttpMethod.Get,
            "/2010-04-01/Accounts/" + PathSegment.Encode(accountSid) + ".json",
            ct: ct).ConfigureAwait(false);
        return result ?? throw new ApiException("empty body on GET /Accounts/{sid}", 200);
    }

    /// <summary>Update the authenticated account. Only the fields present in the request are
    /// changed. A stale update (the account was modified by another request) fails with
    /// <see cref="ConflictException"/> (409 / error 20409) — refresh the resource and retry.</summary>
    public async Task<Account> UpdateAsync(string accountSid, UpdateAccountRequest request, CancellationToken ct = default)
    {
        var result = await Transport.SendAsync<Account>(
            HttpMethod.Post,
            "/2010-04-01/Accounts/" + PathSegment.Encode(accountSid) + ".json",
            formBody: request.ToForm(),
            ct: ct).ConfigureAwait(false);
        return result ?? throw new ApiException("empty body on POST /Accounts/{sid}", 200);
    }

    /// <summary>Rotate the authenticated account's API key
    /// (<c>POST /2010-04-01/Accounts/{AccountSid}/RotateKey.json</c>, no request body). The
    /// previous key stops working immediately. The new key is returned exactly once in
    /// <see cref="AccountWithKey.ApiKey"/> — no read endpoint exposes it, so store it right
    /// away. The key is redacted from the response's default string rendering; treat it as
    /// secret material. A concurrent modification fails with
    /// <see cref="ConflictException"/> (409).</summary>
    public async Task<AccountWithKey> RotateKeyAsync(string accountSid, CancellationToken ct = default)
    {
        var result = await Transport.SendAsync<AccountWithKey>(
            HttpMethod.Post,
            "/2010-04-01/Accounts/" + PathSegment.Encode(accountSid) + "/RotateKey.json",
            ct: ct).ConfigureAwait(false);
        return result ?? throw new ApiException("empty body on POST /Accounts/{sid}/RotateKey", 200);
    }
}

/// <summary>The account balance (<c>GET /2010-04-01/Accounts/{AccountSid}/Balance.json</c>).
/// VoiceML is operator-billed — there is no per-tenant runtime balance ledger — so the
/// balance is reported as a static zero in the account's currency.</summary>
public sealed class BalanceResource : ResourceBase
{
    internal BalanceResource(Transport transport) : base(transport) { }

    /// <summary>Fetch the account balance.</summary>
    public async Task<Balance> FetchAsync(CancellationToken ct = default)
    {
        var result = await Transport.SendAsync<Balance>(
            HttpMethod.Get,
            Path("Balance"),
            ct: ct).ConfigureAwait(false);
        return result ?? throw new ApiException("empty body on GET /Balance", 200);
    }
}
