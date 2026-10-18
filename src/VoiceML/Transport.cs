using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using VoiceML.Exceptions;

namespace VoiceML;

/// <summary>HTTP transport for the VoiceML API. Owns (or borrows) a single <see cref="HttpClient"/>,
/// builds Basic auth headers, serializes form bodies, deserializes JSON, and runs retry/backoff.
/// <para>This type is internal — the public surface goes through <see cref="VoiceMLClient"/>.</para>
/// <para>Retry policy: idempotent requests (GET/HEAD/PUT/DELETE) retry on 429, 500, 502, 503, 504
/// and on any transport error. Non-idempotent requests (POST/PATCH) retry only on 429 and on
/// connection-establishment failures where the request provably never left the machine (DNS
/// failure, connection refused, connect timeout, TLS handshake failure) — never on 5xx, read
/// timeouts or connection resets, which could duplicate a call or SMS. <c>Retry-After</c> is
/// honoured in both delta-seconds and HTTP-date form; when it exceeds the 60-second cap the SDK
/// raises the rate-limit error immediately instead of sleeping. Exponential backoff with full
/// jitter (0.5 s base, 8 s ceiling) is used when the server does not direct a wait.</para>
/// </summary>
internal sealed class Transport : IDisposable
{
    private static readonly TimeSpan RetryAfterCap = TimeSpan.FromSeconds(60);

    private static readonly TimeSpan BackoffBase = TimeSpan.FromSeconds(0.5);

    private static readonly TimeSpan BackoffCeiling = TimeSpan.FromSeconds(8);

    /// <summary>Shared <see cref="JsonSerializerOptions"/> used for all (de)serialization.
    /// Configured to ignore null fields when writing (so optional request fields drop out)
    /// and to read snake_case wire names via <c>[JsonPropertyName]</c> attributes on models.</summary>
    internal static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = false,
        DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull,
        WriteIndented = false,
    };

    /// <summary>Test hook that replaces the backoff delay sleep, so retry
    /// tests run without wall-clock waits. Not set in production.</summary>
    internal static Func<TimeSpan, CancellationToken, Task>? DelayOverride = null;

    private readonly HttpClient _http;
    private readonly bool _ownsClient;
    private readonly string _baseUrl;
    private readonly string _accountSid;
    private readonly string? _basicAuthHeader;
    private readonly int _maxRetries;
    private readonly string _userAgent;
    private readonly Action<string>? _logger;

    /// <summary>The configured AccountSid. Resources need this to build URL paths.</summary>
    internal string AccountSid => _accountSid;

    /// <summary>The configured server base URL (no trailing slash).</summary>
    internal string BaseUrl => _baseUrl;

    /// <summary>Construct from validated <see cref="ClientOptions"/>.</summary>
    internal Transport(ClientOptions options)
    {
        options.Validate();
        _accountSid = options.AccountSid;
        _baseUrl = options.BaseUrl.TrimEnd('/');
        _maxRetries = options.MaxRetries;
        _userAgent = options.UserAgent;
        _logger = options.Logger;

        if (options.HttpClient is not null)
        {
            _http = options.HttpClient;
            _ownsClient = false;
        }
        else
        {
            // A pooled-connection lifetime lets DNS changes take effect for long-lived
            // singleton clients; automatic redirects are disabled so binary fetches
            // (recording audio, export archives) can apply the SDK redirect policy
            // manually: strip Authorization when the host or scheme changes and refuse
            // an https -> http downgrade.
            var handler = new SocketsHttpHandler
            {
                PooledConnectionLifetime = TimeSpan.FromMinutes(5),
                AllowAutoRedirect = false,
                AutomaticDecompression = DecompressionMethods.All,
            };
            _http = new HttpClient(handler, disposeHandler: true)
            {
                Timeout = options.Timeout,
            };
            _ownsClient = true;
        }

        // Pre-compute the Authorization header. AccountSid + credential are immutable
        // here, so encode the Basic pair once.
        var raw = $"{options.AccountSid}:{options.ResolvedCredential}";
        _basicAuthHeader = "Basic " + Convert.ToBase64String(Encoding.UTF8.GetBytes(raw));
    }

    /// <summary>Private scoped constructor: shares <paramref name="parent"/>'s
    /// <see cref="HttpClient"/>, auth, and retry policy but pins a different base URL. Used by
    /// <see cref="WithBaseUrl"/> to route a whole product resource group (Conversations, Messaging
    /// Service) at its own subdomain. Never owns the shared client, so it never disposes it — the
    /// parent transport retains lifetime ownership.</summary>
    private Transport(Transport parent, string baseUrl)
    {
        _http = parent._http;
        _ownsClient = false;
        _baseUrl = baseUrl.TrimEnd('/');
        _accountSid = parent._accountSid;
        _basicAuthHeader = parent._basicAuthHeader;
        _maxRetries = parent._maxRetries;
        _userAgent = parent._userAgent;
        _logger = parent._logger;
    }

    /// <summary>Return a transport view pinned to <paramref name="baseUrl"/>, sharing this
    /// transport's HTTP client, credentials, and retry policy. An entire resource group can be
    /// handed the returned transport so every request lands on the product subdomain
    /// (<c>conversations.voicetel.com</c> / <c>messaging.voicetel.com</c>) without each resource
    /// needing to know its own host. The returned transport does not own the HTTP client and must
    /// not be disposed independently.</summary>
    internal Transport WithBaseUrl(string baseUrl) => new(this, baseUrl);

    /// <summary>Send a request and deserialize the JSON response into <typeparamref name="T"/>.
    /// <paramref name="formBody"/> is form-urlencoded; <paramref name="jsonBody"/> is JSON.
    /// Pass at most one. Pass <paramref name="sendAuth"/><c> = false</c> for endpoints the spec
    /// declares <c>security: []</c> on. Use <see cref="SendNoContentAsync"/> when the endpoint
    /// returns no body (e.g. DELETE).</summary>
    internal async Task<T?> SendAsync<T>(
        HttpMethod method,
        string path,
        IEnumerable<KeyValuePair<string, string?>>? queryParams = null,
        IEnumerable<KeyValuePair<string, string?>>? formBody = null,
        object? jsonBody = null,
        bool sendAuth = true,
        CancellationToken ct = default)
    {
        var url = BuildUrl(path, queryParams);
        using var response = await SendCoreAsync(method, url, formBody, jsonBody, sendAuth, ct).ConfigureAwait(false);
        var content = await response.Content.ReadAsStringAsync(ct).ConfigureAwait(false);

        if (string.IsNullOrEmpty(content))
        {
            return default;
        }
        try
        {
            return JsonSerializer.Deserialize<T>(content, JsonOptions);
        }
        catch (JsonException ex)
        {
            throw new ApiException(
                $"could not decode response as {typeof(T).Name}: {Truncate(content, 200)}",
                (int)response.StatusCode,
                body: content,
                inner: ex);
        }
    }

    /// <summary>Send a request and discard the response body. Throws on non-2xx as usual.</summary>
    internal async Task SendNoContentAsync(
        HttpMethod method,
        string path,
        IEnumerable<KeyValuePair<string, string?>>? queryParams = null,
        IEnumerable<KeyValuePair<string, string?>>? formBody = null,
        object? jsonBody = null,
        bool sendAuth = true,
        CancellationToken ct = default)
    {
        var url = BuildUrl(path, queryParams);
        using var response = await SendCoreAsync(method, url, formBody, jsonBody, sendAuth, ct).ConfigureAwait(false);
    }

    /// <summary>Fetch a page by its <c>next_page_uri</c> (the Twilio-compatible 2010-04-01
    /// pagination envelope). The URI is relative (path + query); it is resolved against the
    /// scheme and host this transport is configured with.</summary>
    internal async Task<T?> SendPageUriAsync<T>(string pageUri, CancellationToken ct = default)
    {
        if (string.IsNullOrEmpty(pageUri))
        {
            throw new VoiceMLException("next page URI is empty");
        }
        var absolute = ResolveAgainstBase(pageUri);
        using var response = await SendCoreAsync(HttpMethod.Get, absolute, formBody: null, jsonBody: null, sendAuth: true, ct).ConfigureAwait(false);
        return await ReadSuccessBodyAsync<T>(response, ct).ConfigureAwait(false);
    }

    /// <summary>Fetch a <c>/v1</c> list page by its absolute <c>meta.next_page_url</c>. The URL's
    /// scheme and host must equal this transport's configured product host; otherwise the SDK
    /// refuses the request so credentials are never sent to a foreign host.</summary>
    internal async Task<T?> SendV1NextPageAsync<T>(string nextPageUrl, CancellationToken ct = default)
    {
        if (string.IsNullOrEmpty(nextPageUrl))
        {
            throw new VoiceMLException("next_page_url is empty");
        }
        var absolute = RequireSameHost(nextPageUrl);
        using var response = await SendCoreAsync(HttpMethod.Get, absolute, formBody: null, jsonBody: null, sendAuth: true, ct).ConfigureAwait(false);
        return await ReadSuccessBodyAsync<T>(response, ct).ConfigureAwait(false);
    }

    /// <summary>Fetch a binary payload (recording WAV audio, export archives). Follows up to five
    /// redirects manually: the <c>Authorization</c> header is stripped whenever the redirect
    /// target's host or scheme differs from the previous hop, and an <c>https</c> → <c>http</c>
    /// downgrade is refused. Returns the final status, bytes, and headers.</summary>
    internal async Task<(int Status, byte[] Bytes, HttpResponseHeaders Headers, HttpContentHeaders ContentHeaders, bool ViaRedirect)> FetchBytesAsync(
        string path, bool sendAuth = true, CancellationToken ct = default)
    {
        var url = BuildUrl(path, queryParams: null);
        var current = url;
        var sendAuthThisHop = sendAuth;
        const int maxRedirects = 5;

        for (var hop = 0; ; hop++)
        {
            using var response = await SendCoreAsync(HttpMethod.Get, current, null, null,
                sendAuthThisHop, ct, binary: true).ConfigureAwait(false);

            if ((int)response.StatusCode is >= 300 and < 400)
            {
                var location = response.Headers.Location;
                var body = await response.Content.ReadAsStringAsync(ct).ConfigureAwait(false);
                if (location is null)
                {
                    throw new ApiException(
                        $"redirect without Location header on {current}", (int)HttpStatusCode.Redirect, body: body);
                }
                if (hop >= maxRedirects)
                {
                    throw new ApiException(
                        $"too many redirects while fetching {path}", 0, body: body);
                }
                var next = location.IsAbsoluteUri ? location.AbsoluteUri : new Uri(new Uri(current), location).AbsoluteUri;
                var prevUri = new Uri(current);
                var nextUri = new Uri(next);
                if (nextUri.Scheme is not ("https" or "http") ||
                    (prevUri.Scheme == "https" && nextUri.Scheme == "http"))
                {
                    throw new ApiException(
                        $"refusing to follow redirect from https to http while fetching {path}", 0);
                }
                if (!string.Equals(prevUri.Host, nextUri.Host, StringComparison.OrdinalIgnoreCase)
                    || !string.Equals(prevUri.Scheme, nextUri.Scheme, StringComparison.OrdinalIgnoreCase)
                    || prevUri.Port != nextUri.Port)
                {
                    // Cross-origin hop: never forward credentials.
                    sendAuthThisHop = false;
                }
                current = next;
                continue;
            }

            if (!response.IsSuccessStatusCode)
            {
                var ex = await BuildExceptionAsync(response, ct).ConfigureAwait(false);
                response.Dispose();
                throw ex;
            }
            var bytes = await response.Content.ReadAsByteArrayAsync(ct).ConfigureAwait(false);
            var result = ((int)response.StatusCode, bytes, response.Headers, response.Content.Headers, hop > 0);
            return result;
        }
    }

    private static async Task<T?> ReadSuccessBodyAsync<T>(HttpResponseMessage response, CancellationToken ct)
    {
        var content = await response.Content.ReadAsStringAsync(ct).ConfigureAwait(false);
        if (string.IsNullOrEmpty(content))
        {
            return default;
        }
        try
        {
            return JsonSerializer.Deserialize<T>(content, JsonOptions);
        }
        catch (JsonException ex)
        {
            throw new ApiException(
                $"could not decode response as {typeof(T).Name}: {Truncate(content, 200)}",
                (int)response.StatusCode,
                body: content,
                inner: ex);
        }
    }

    private async Task<HttpResponseMessage> SendCoreAsync(
        HttpMethod method,
        string url,
        IEnumerable<KeyValuePair<string, string?>>? formBody,
        object? jsonBody,
        bool sendAuth,
        CancellationToken ct,
        bool binary = false)
    {
        if (formBody is not null && jsonBody is not null)
        {
            throw new ArgumentException("formBody and jsonBody are mutually exclusive");
        }

        var idempotent = IsIdempotent(method);
        // Materialize once so each retry sends exactly the same form, even when the
        // caller supplied a one-shot sequence. Validation also runs before allocation.
        var form = formBody?.ToArray();

        for (var attempt = 0; ; attempt++)
        {
            HttpResponseMessage? response = null;
            Exception? failure = null;
            var retryFailure = false;
            var req = BuildRequest(method, url, form, jsonBody, sendAuth, binary);
            try
            {
                response = await _http.SendAsync(req, ct).ConfigureAwait(false);
            }
            catch (OperationCanceledException) when (ct.IsCancellationRequested)
            {
                // Caller cancellation propagates unchanged and is never retried.
                throw;
            }
            catch (OperationCanceledException ex)
            {
                // HttpClient.Timeout elapses without the caller's token being cancelled.
                failure = new VoiceMLTimeoutException(
                    $"request timed out after {_http.Timeout.TotalSeconds:0.##}s: {method} {url}", ex);
                retryFailure = idempotent;
                _logger?.Invoke($"timeout on attempt {attempt + 1}: {ex.Message}");
            }
            catch (HttpRequestException ex)
            {
                var establishment = IsConnectionEstablishmentFailure(ex);
                failure = new ApiException(
                    $"transport error on attempt {attempt + 1}: {ex.Message}",
                    statusCode: 0,
                    inner: ex);
                // Establishment failures prove the request never went out, so they are
                // safe to retry even for POST/PATCH. Everything else (write error, read
                // error, connection reset mid-flight) is retried for idempotent verbs only.
                retryFailure = establishment || idempotent;
                _logger?.Invoke($"transport error on attempt {attempt + 1} ({(establishment ? "connection establishment" : "in-flight")}): {ex.Message}");
            }
            finally
            {
                // The response content is buffered by SendAsync (ResponseContentRead), so
                // the request can be disposed on every path.
                req.Dispose();
            }

            if (failure is not null)
            {
                if (attempt >= _maxRetries || !retryFailure)
                {
                    throw failure;
                }
                await DelayAsync(attempt, serverDirected: null, ct).ConfigureAwait(false);
                continue;
            }

            var statusCode = (int)response!.StatusCode;
            if (IsRetryableStatus(statusCode, idempotent) && attempt < _maxRetries)
            {
                var retryAfter = ParseRetryAfter(response);
                if (retryAfter > RetryAfterCap)
                {
                    // The server asked for a longer wait than the SDK will honour — surface
                    // the rate-limit error immediately instead of sleeping.
                    var ex = await BuildExceptionAsync(response, ct).ConfigureAwait(false);
                    response.Dispose();
                    throw ex;
                }
                _logger?.Invoke($"retryable {statusCode} on attempt {attempt + 1}, backing off");
                await DrainAsync(response, ct).ConfigureAwait(false);
                response.Dispose();
                await DelayAsync(attempt, retryAfter, ct).ConfigureAwait(false);
                continue;
            }

            if (!response.IsSuccessStatusCode && !(binary && (int)response.StatusCode is >= 300 and < 400))
            {
                var ex = await BuildExceptionAsync(response, ct).ConfigureAwait(false);
                response.Dispose();
                throw ex;
            }
            return response;
        }
    }

    private static bool IsIdempotent(HttpMethod method) =>
        method == HttpMethod.Get || method == HttpMethod.Head
            || method == HttpMethod.Put || method == HttpMethod.Delete;

    private static bool IsRetryableStatus(int statusCode, bool idempotent) =>
        idempotent
            ? statusCode is 429 or 500 or 502 or 503 or 504
            : statusCode == 429;

    private static bool IsConnectionEstablishmentFailure(HttpRequestException ex) =>
        ex.HttpRequestError is HttpRequestError.NameResolutionError
            or HttpRequestError.SecureConnectionError
        || (ex.HttpRequestError == HttpRequestError.ConnectionError && ex.InnerException is SocketException se
            && se.SocketErrorCode is SocketError.ConnectionRefused
                or SocketError.HostUnreachable
                or SocketError.NetworkUnreachable
                or SocketError.TimedOut);

    private HttpRequestMessage BuildRequest(
        HttpMethod method,
        string url,
        IEnumerable<KeyValuePair<string, string?>>? formBody,
        object? jsonBody,
        bool sendAuth,
        bool binary)
    {
        var req = new HttpRequestMessage(method, url);
        ApplyHeaders(req, sendAuth, binary);

        if (formBody is not null)
        {
            var pairs = new List<KeyValuePair<string, string>>();
            foreach (var kv in formBody)
            {
                if (kv.Value is null)
                {
                    continue;
                }
                var value = kv.Value;
                if (kv.Key == "Attributes" && value.Length > 0)
                {
                    try
                    {
                        using var attributes = JsonDocument.Parse(value);
                        value = JsonSerializer.Serialize(attributes.RootElement);
                    }
                    catch (JsonException ex)
                    {
                        req.Dispose();
                        throw new VoiceMLException("Attributes must be a JSON value.", ex);
                    }
                }
                pairs.Add(new KeyValuePair<string, string>(kv.Key, value));
            }
            req.Content = new FormUrlEncodedContent(pairs);
        }
        else if (jsonBody is not null)
        {
            var json = JsonSerializer.Serialize(jsonBody, jsonBody.GetType(), JsonOptions);
            req.Content = new StringContent(json, Encoding.UTF8, "application/json");
        }
        return req;
    }

    private void ApplyHeaders(HttpRequestMessage req, bool sendAuth, bool binary)
    {
        if (sendAuth && _basicAuthHeader is not null)
        {
            req.Headers.TryAddWithoutValidation("Authorization", _basicAuthHeader);
        }
        req.Headers.TryAddWithoutValidation("User-Agent", _userAgent);
        req.Headers.Accept.Clear();
        req.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue(binary ? "*/*" : "application/json"));
    }

    private string BuildUrl(string path, IEnumerable<KeyValuePair<string, string?>>? queryParams)
    {
        var pathPart = path.StartsWith('/') ? path : "/" + path;
        var sb = new StringBuilder(_baseUrl);
        sb.Append(pathPart);
        if (queryParams is not null)
        {
            var first = true;
            foreach (var kv in queryParams)
            {
                if (kv.Value is null)
                {
                    continue;
                }
                sb.Append(first ? '?' : '&');
                first = false;
                sb.Append(EncodeQueryName(kv.Key));
                sb.Append('=');
                sb.Append(Uri.EscapeDataString(kv.Value));
            }
        }
        return sb.ToString();
    }

    /// <summary>Percent-encode a query-parameter NAME. Names used by the API are plain
    /// identifiers except the literal Twilio filter names <c>StartTime&lt;</c>,
    /// <c>StartTime&gt;</c>, <c>EndTime&lt;</c>, <c>EndTime&gt;</c> and their Date* siblings.
    /// Those names must reach the server as percent-encoded bytes (<c>%3C</c> / <c>%3E</c>)
    /// so the <c>=</c> that separates name from value stays unambiguous.</summary>
    private static string EncodeQueryName(string name)
    {
        var escaped = Uri.EscapeDataString(name);
        return escaped;
    }

    private string ResolveAgainstBase(string pageUri)
    {
        var origin = new Uri(new Uri(_baseUrl).GetLeftPart(UriPartial.Authority) + "/");
        return RequireSameHost(new Uri(origin, pageUri).AbsoluteUri);
    }

    private string RequireSameHost(string url)
    {
        if (!Uri.TryCreate(url, UriKind.Absolute, out var target))
        {
            throw new VoiceMLException($"next_page_url is not an absolute URL: {url}");
        }
        var configured = new Uri(_baseUrl);
        if (!string.Equals(target.Host, configured.Host, StringComparison.OrdinalIgnoreCase)
            || !string.Equals(target.Scheme, configured.Scheme, StringComparison.OrdinalIgnoreCase)
            || target.Port != configured.Port || target.UserInfo.Length != 0)
        {
            throw new VoiceMLException(
                $"refusing to fetch a /v1 page from {target.Scheme}://{target.Host} — the configured product host is {configured.Scheme}://{configured.Host}");
        }
        return target.AbsoluteUri;
    }

    private static async Task DrainAsync(HttpResponseMessage response, CancellationToken ct)
    {
        try
        {
            await response.Content.ReadAsStringAsync(ct).ConfigureAwait(false);
        }
        catch (HttpRequestException)
        {
            // Draining is best-effort; disposing the response closes the connection when it fails.
        }
    }

    private static async Task DelayAsync(int attempt, TimeSpan? serverDirected, CancellationToken ct)
    {
        var delay = serverDirected ?? FullJitter(attempt);
        if (delay <= TimeSpan.Zero)
        {
            return;
        }
        if (DelayOverride is { } hook)
        {
            await hook(delay, ct).ConfigureAwait(false);
            return;
        }
        await Task.Delay(delay, ct).ConfigureAwait(false);
    }

    private static TimeSpan FullJitter(int attempt)
    {
        var ceilingSeconds = Math.Min(BackoffCeiling.TotalSeconds, BackoffBase.TotalSeconds * Math.Pow(2, attempt));
        return TimeSpan.FromSeconds(Random.Shared.NextDouble() * ceilingSeconds);
    }

    private static TimeSpan? ParseRetryAfter(HttpResponseMessage response)
    {
        var value = response.Headers.RetryAfter;
        if (value is null)
        {
            return null;
        }
        if (value.Delta is { } delta)
        {
            return delta;
        }
        if (value.Date is { } date)
        {
            var wait = date - DateTimeOffset.Now;
            return wait > TimeSpan.Zero ? wait : TimeSpan.Zero;
        }
        return null;
    }

    private static async Task<ApiException> BuildExceptionAsync(HttpResponseMessage response, CancellationToken ct)
    {
        var statusCode = (int)response.StatusCode;
        var body = await response.Content.ReadAsStringAsync(ct).ConfigureAwait(false);

        object? parsedBody = null;
        int? code = null;
        string? codeRaw = null;
        string? moreInfo = null;
        int? status = null;
        string message = $"HTTP {statusCode}";

        if (!string.IsNullOrEmpty(body))
        {
            try
            {
                using var doc = JsonDocument.Parse(body);
                parsedBody = JsonSerializer.Deserialize<object>(body, JsonOptions);
                if (doc.RootElement.ValueKind == JsonValueKind.Object)
                {
                    if (doc.RootElement.TryGetProperty("code", out var codeEl))
                    {
                        if (codeEl.ValueKind == JsonValueKind.Number && codeEl.TryGetInt32(out var i))
                        {
                            code = i;
                            codeRaw = codeEl.GetRawText();
                        }
                        else if (codeEl.ValueKind is JsonValueKind.String or JsonValueKind.Number)
                        {
                            codeRaw = codeEl.ValueKind == JsonValueKind.String ? codeEl.GetString() : codeEl.GetRawText();
                            if (int.TryParse(codeRaw, NumberStyles.Integer, CultureInfo.InvariantCulture, out var numeric))
                            {
                                code = numeric;
                            }
                        }
                    }
                    if (doc.RootElement.TryGetProperty("message", out var msgEl) && msgEl.ValueKind == JsonValueKind.String)
                    {
                        var s = msgEl.GetString();
                        if (!string.IsNullOrEmpty(s))
                        {
                            message = s;
                        }
                    }
                    if (doc.RootElement.TryGetProperty("more_info", out var miEl) && miEl.ValueKind == JsonValueKind.String)
                    {
                        var s = miEl.GetString();
                        if (!string.IsNullOrEmpty(s))
                        {
                            moreInfo = s;
                        }
                    }
                    if (doc.RootElement.TryGetProperty("status", out var stEl)
                        && stEl.ValueKind == JsonValueKind.Number && stEl.TryGetInt32(out var st))
                    {
                        status = st;
                    }
                }
            }
            catch (JsonException)
            {
                parsedBody = body;
            }
        }

        TimeSpan? retryAfter = ParseRetryAfter(response);
        double? retryAfterSeconds = retryAfter is null ? null : retryAfter.Value.TotalSeconds;

        return statusCode switch
        {
            400 => new BadRequestException(message, statusCode, code, parsedBody, moreInfo, codeRaw, status),
            401 => new AuthenticationException(message, statusCode, code, parsedBody, moreInfo, codeRaw, status),
            403 => new PermissionDeniedException(message, statusCode, code, parsedBody, moreInfo, codeRaw, status),
            404 => new NotFoundException(message, statusCode, code, parsedBody, moreInfo, codeRaw, status),
            409 => new ConflictException(message, statusCode, code, parsedBody, moreInfo, codeRaw, status),
            410 => new GoneException(message, statusCode, code, parsedBody, moreInfo, codeRaw, status),
            429 => new RateLimitException(message, statusCode, code, parsedBody, retryAfterSeconds, moreInfo, codeRaw, status, retryAfter),
            501 => new NotImplementedAPIException(message, statusCode, code, parsedBody, moreInfo, codeRaw, status),
            >= 500 and < 600 => new ServerException(message, statusCode, code, parsedBody, moreInfo, codeRaw, status),
            _ => new ApiException(message, statusCode, code, parsedBody, moreInfo, codeRaw, status),
        };
    }

    private static string Truncate(string s, int max) => s.Length <= max ? s : s.Substring(0, max);

    /// <summary>Dispose the underlying <see cref="HttpClient"/> only when we own it
    /// (i.e. when the caller did not supply one via <see cref="ClientOptions.HttpClient"/>).</summary>
    public void Dispose()
    {
        if (_ownsClient)
        {
            _http.Dispose();
        }
    }
}
