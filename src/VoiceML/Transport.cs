using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using VoiceML.Exceptions;

namespace VoiceML;

/// <summary>HTTP transport for the VoiceML API. Owns (or borrows) a single <see cref="HttpClient"/>,
/// builds Basic auth headers, serializes form bodies, deserializes JSON, and runs retry/backoff
/// on the retryable statuses (429, 500, 502, 503, 504) and transient transport errors.
/// <para>This type is internal — the public surface goes through <see cref="VoiceMLClient"/>.</para>
/// </summary>
public sealed class Transport : IDisposable
{
    private static readonly HashSet<int> RetryableStatuses = new() { 429, 500, 502, 503, 504 };

    /// <summary>Shared <see cref="JsonSerializerOptions"/> used for all (de)serialization.
    /// Configured to ignore null fields when writing (so optional request fields drop out)
    /// and to read snake_case wire shape via <c>[JsonPropertyName]</c> attributes on models.</summary>
    public static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = false,
        DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull,
        WriteIndented = false,
    };

    private readonly HttpClient _http;
    private readonly bool _ownsClient;
    private readonly string _baseUrl;
    private readonly string _accountSid;
    private readonly string _basicAuthHeader;
    private readonly int _maxRetries;
    private readonly string _userAgent;
    private readonly Action<string>? _logger;

    /// <summary>The configured AccountSid. Resources need this to build URL paths.</summary>
    public string AccountSid => _accountSid;

    /// <summary>The configured server base URL (no trailing slash).</summary>
    public string BaseUrl => _baseUrl;

    /// <summary>Construct from validated <see cref="ClientOptions"/>.</summary>
    public Transport(ClientOptions options)
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
            _http = new HttpClient
            {
                Timeout = options.Timeout,
            };
            _ownsClient = true;
        }

        // Pre-compute the Authorization header. Bytes used to be re-encoded per request;
        // since AccountSid + ApiKey are immutable here, do it once.
        var raw = $"{options.AccountSid}:{options.ApiKey}";
        _basicAuthHeader = "Basic " + Convert.ToBase64String(Encoding.UTF8.GetBytes(raw));
    }

    /// <summary>Send a request and deserialize the JSON response into <typeparamref name="T"/>.
    /// <paramref name="formBody"/> is form-urlencoded; <paramref name="jsonBody"/> is JSON.
    /// Pass at most one. Use <see cref="SendNoContentAsync"/> when the endpoint returns no body
    /// (e.g. DELETE).</summary>
    public async Task<T?> SendAsync<T>(
        HttpMethod method,
        string path,
        IEnumerable<KeyValuePair<string, string?>>? queryParams = null,
        IEnumerable<KeyValuePair<string, string?>>? formBody = null,
        object? jsonBody = null,
        CancellationToken ct = default)
    {
        using var response = await SendCoreAsync(method, path, queryParams, formBody, jsonBody, ct).ConfigureAwait(false);
        var content = await response.Content.ReadAsStringAsync(ct).ConfigureAwait(false);

        if (string.IsNullOrEmpty(content))
        {
            return default;
        }
        try
        {
            return JsonSerializer.Deserialize<T>(content, JsonOptions);
        }
        catch (JsonException)
        {
            throw new ApiException(
                $"non-JSON success response: {Truncate(content, 200)}",
                (int)response.StatusCode,
                code: null,
                body: content);
        }
    }

    /// <summary>Send a request and discard the response body. Throws on non-2xx as usual.</summary>
    public async Task SendNoContentAsync(
        HttpMethod method,
        string path,
        IEnumerable<KeyValuePair<string, string?>>? queryParams = null,
        IEnumerable<KeyValuePair<string, string?>>? formBody = null,
        object? jsonBody = null,
        CancellationToken ct = default)
    {
        using var response = await SendCoreAsync(method, path, queryParams, formBody, jsonBody, ct).ConfigureAwait(false);
        // body already drained by SendCoreAsync's error path on non-2xx; on 2xx just ignore.
    }

    /// <summary>Fetch a binary payload (audio/wav recordings). Follows the single 302→presigned
    /// redirect that <c>GET /Recordings/{sid}.wav</c> issues when the audio has been archived to S3.
    /// Returns the final status, bytes, and headers.</summary>
    public async Task<(int Status, byte[] Bytes, HttpResponseHeaders Headers, HttpContentHeaders ContentHeaders)> FetchBytesAsync(
        string path, CancellationToken ct = default)
    {
        var url = BuildUrl(path, queryParams: null);
        using var req = new HttpRequestMessage(HttpMethod.Get, url);
        ApplyHeaders(req);
        using var response = await _http.SendAsync(req, ct).ConfigureAwait(false);
        if (!response.IsSuccessStatusCode)
        {
            await ThrowFromResponseAsync(response, ct).ConfigureAwait(false);
        }
        var bytes = await response.Content.ReadAsByteArrayAsync(ct).ConfigureAwait(false);
        return ((int)response.StatusCode, bytes, response.Headers, response.Content.Headers);
    }

    private async Task<HttpResponseMessage> SendCoreAsync(
        HttpMethod method,
        string path,
        IEnumerable<KeyValuePair<string, string?>>? queryParams,
        IEnumerable<KeyValuePair<string, string?>>? formBody,
        object? jsonBody,
        CancellationToken ct)
    {
        if (formBody is not null && jsonBody is not null)
        {
            throw new ArgumentException("formBody and jsonBody are mutually exclusive");
        }

        var url = BuildUrl(path, queryParams);
        Exception? lastTransportEx = null;

        for (var attempt = 0; attempt <= _maxRetries; attempt++)
        {
            var req = BuildRequest(method, url, formBody, jsonBody);
            HttpResponseMessage response;
            try
            {
                response = await _http.SendAsync(req, ct).ConfigureAwait(false);
            }
            catch (OperationCanceledException) when (ct.IsCancellationRequested)
            {
                req.Dispose();
                throw;
            }
            catch (HttpRequestException ex)
            {
                req.Dispose();
                lastTransportEx = ex;
                _logger?.Invoke($"transport error on attempt {attempt + 1}: {ex.Message}");
                if (attempt >= _maxRetries)
                {
                    throw new ApiException(
                        $"transport error after {attempt + 1} attempts: {ex.Message}",
                        statusCode: 0,
                        inner: ex);
                }
                await BackoffAsync(attempt, null, ct).ConfigureAwait(false);
                continue;
            }
            finally
            {
                // We must NOT dispose req here on success — HttpClient is done with it once SendAsync returns,
                // but disposing it disposes the content stream we already consumed. Reaching this point we
                // either threw above or have a response; the response now owns the lifecycle.
            }

            var statusCode = (int)response.StatusCode;
            if (RetryableStatuses.Contains(statusCode) && attempt < _maxRetries)
            {
                _logger?.Invoke($"retryable {statusCode} on attempt {attempt + 1}, backing off");
                await BackoffAsync(attempt, response, ct).ConfigureAwait(false);
                response.Dispose();
                continue;
            }

            if (!response.IsSuccessStatusCode)
            {
                await ThrowFromResponseAsync(response, ct).ConfigureAwait(false);
            }
            return response;
        }

        // Unreachable — the loop either returns or throws.
        throw new ApiException(
            $"retry loop exhausted ({_maxRetries + 1} attempts)",
            statusCode: 0,
            inner: lastTransportEx ?? new InvalidOperationException("retry loop exhausted"));
    }

    private HttpRequestMessage BuildRequest(
        HttpMethod method,
        string url,
        IEnumerable<KeyValuePair<string, string?>>? formBody,
        object? jsonBody)
    {
        var req = new HttpRequestMessage(method, url);
        ApplyHeaders(req);

        if (formBody is not null)
        {
            // FormUrlEncodedContent takes IEnumerable<KeyValuePair<string?,string?>> on net8, but
            // we keep our signature non-null-key. Drop null values defensively here.
            var pairs = new List<KeyValuePair<string, string>>();
            foreach (var kv in formBody)
            {
                if (kv.Value is null)
                {
                    continue;
                }
                pairs.Add(new KeyValuePair<string, string>(kv.Key, kv.Value));
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

    private void ApplyHeaders(HttpRequestMessage req)
    {
        req.Headers.TryAddWithoutValidation("Authorization", _basicAuthHeader);
        req.Headers.TryAddWithoutValidation("User-Agent", _userAgent);
        req.Headers.Accept.Clear();
        req.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
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
                // Keep query-param names verbatim — Twilio uses literal `StartTime>=` / `StartTime<=`
                // which contain reserved characters. RFC 3986 allows `>=` only in the query, and we
                // need the server to see exactly that, so we URL-encode the *name* using the same
                // rules but explicitly leave `>` and `=` un-escaped only inside the name.
                sb.Append(EncodeQueryName(kv.Key));
                sb.Append('=');
                sb.Append(Uri.EscapeDataString(kv.Value));
            }
        }
        return sb.ToString();
    }

    private static string EncodeQueryName(string name)
    {
        // Twilio expects the literal names `StartTime>=` / `StartTime<=` on the wire. Most URL-encode
        // helpers percent-escape `>` and `<`, which the server then 400s on. We use Uri.EscapeDataString
        // on everything *except* `>=` / `<=` sequences, which we leave intact.
        // Cheap shortcut: if the name doesn't contain `>` or `<`, just escape it normally.
        if (name.IndexOf('>') < 0 && name.IndexOf('<') < 0)
        {
            return Uri.EscapeDataString(name);
        }
        var sb = new StringBuilder(name.Length);
        foreach (var c in name)
        {
            if (c is '>' or '<' or '=')
            {
                sb.Append(c);
            }
            else
            {
                sb.Append(Uri.EscapeDataString(c.ToString()));
            }
        }
        return sb.ToString();
    }

    private async Task BackoffAsync(int attempt, HttpResponseMessage? response, CancellationToken ct)
    {
        TimeSpan delay = ComputeBackoff(attempt, response);
        if (delay > TimeSpan.Zero)
        {
            try
            {
                await Task.Delay(delay, ct).ConfigureAwait(false);
            }
            catch (TaskCanceledException)
            {
                throw;
            }
        }
    }

    private static TimeSpan ComputeBackoff(int attempt, HttpResponseMessage? response)
    {
        if (response is not null && response.Headers.TryGetValues("Retry-After", out var values))
        {
            foreach (var v in values)
            {
                if (double.TryParse(v, NumberStyles.Float, CultureInfo.InvariantCulture, out var seconds) && seconds >= 0)
                {
                    return TimeSpan.FromSeconds(Math.Min(seconds, 60));
                }
            }
        }
        var delay = Math.Min(8.0, 0.5 * Math.Pow(2, attempt));
        return TimeSpan.FromSeconds(delay);
    }

    private static async Task ThrowFromResponseAsync(HttpResponseMessage response, CancellationToken ct)
    {
        var statusCode = (int)response.StatusCode;
        var body = await response.Content.ReadAsStringAsync(ct).ConfigureAwait(false);

        object? parsedBody = null;
        object? code = null;
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
                        code = codeEl.ValueKind switch
                        {
                            JsonValueKind.Number when codeEl.TryGetInt32(out var i) => i,
                            JsonValueKind.Number => codeEl.GetDouble(),
                            JsonValueKind.String => codeEl.GetString(),
                            _ => null,
                        };
                    }
                    if (doc.RootElement.TryGetProperty("message", out var msgEl) && msgEl.ValueKind == JsonValueKind.String)
                    {
                        var s = msgEl.GetString();
                        if (!string.IsNullOrEmpty(s))
                        {
                            message = s;
                        }
                    }
                }
            }
            catch (JsonException)
            {
                parsedBody = body;
            }
        }

        double? retryAfter = null;
        if (response.Headers.TryGetValues("Retry-After", out var raVals))
        {
            foreach (var v in raVals)
            {
                if (double.TryParse(v, NumberStyles.Float, CultureInfo.InvariantCulture, out var seconds))
                {
                    retryAfter = seconds;
                    break;
                }
            }
        }

        throw statusCode switch
        {
            400 => new BadRequestException(message, statusCode, code, parsedBody),
            401 => new AuthenticationException(message, statusCode, code, parsedBody),
            403 => new PermissionDeniedException(message, statusCode, code, parsedBody),
            404 => new NotFoundException(message, statusCode, code, parsedBody),
            409 => new ConflictException(message, statusCode, code, parsedBody),
            410 => new GoneException(message, statusCode, code, parsedBody),
            429 => new RateLimitException(message, statusCode, code, parsedBody, retryAfter),
            501 => new NotImplementedAPIException(message, statusCode, code, parsedBody),
            >= 500 and < 600 => new ServerException(message, statusCode, code, parsedBody),
            _ => new ApiException(message, statusCode, code, parsedBody),
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
