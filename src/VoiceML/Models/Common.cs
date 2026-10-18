using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace VoiceML.Models;

/// <summary>Twilio-compatible pagination envelope for the <c>2010-04-01</c> list endpoints.
/// Concrete list types (e.g. <see cref="CallList"/>) inherit from this and add their typed
/// item collection. Page traversal follows <see cref="NextPageUri"/> verbatim — never
/// synthesises a <c>Page=N</c> counter — and stops when it is <c>null</c> or empty.</summary>
public abstract record Page
{
    /// <summary>Zero-based page index of this response. Wire name: <c>page</c>.</summary>
    [JsonPropertyName("page")] public int PageNumber { get; init; }

    /// <summary>Page size (count of items requested).</summary>
    [JsonPropertyName("page_size")] public int PageSize { get; init; } = 50;

    /// <summary>Absolute index of the first item in this page.</summary>
    [JsonPropertyName("start")] public int? Start { get; init; }

    /// <summary>Absolute index of the last item in this page.</summary>
    [JsonPropertyName("end")] public int? End { get; init; }

    /// <summary>URI to fetch the first page.</summary>
    [JsonPropertyName("first_page_uri")] public string? FirstPageUri { get; init; }

    /// <summary>URI to fetch the next page; <c>null</c> or empty when this is the last page.
    /// Relative (path + query) — resolve against the scheme and host of the request that
    /// produced this page.</summary>
    [JsonPropertyName("next_page_uri")] public string? NextPageUri { get; init; }

    /// <summary>URI to fetch the previous page; <c>null</c> when this is the first page.</summary>
    [JsonPropertyName("previous_page_uri")] public string? PreviousPageUri { get; init; }

    /// <summary>URI of this resource collection.</summary>
    [JsonPropertyName("uri")] public string? Uri { get; init; }
}

/// <summary>Implemented by every <c>/v1</c> list envelope (Conversations, Assistants, Voice v1,
/// Pricing, Messaging Service). <c>/v1</c> lists page through the absolute
/// <see cref="VoiceV1Meta.NextPageUrl"/> carried in the <c>meta</c> object; the SDK only
/// follows it when its scheme and host match the configured product host.</summary>
public interface IV1PagedList
{
    /// <summary>The <c>meta</c> envelope, carrying <see cref="VoiceV1Meta.NextPageUrl"/> and
    /// <see cref="VoiceV1Meta.Key"/>.</summary>
    VoiceV1Meta? Meta { get; }
}

/// <summary>One tripped check from the <c>/health</c> deep probe.</summary>
public sealed record HealthFailure
{
    /// <summary>Internal name of the failing check.</summary>
    [JsonPropertyName("check")] public string Check { get; init; } = "";

    /// <summary>Human-readable detail describing the failure.</summary>
    [JsonPropertyName("detail")] public string Detail { get; init; } = "";
}

/// <summary><c>GET /health</c> response — composite probe result.</summary>
public sealed record HealthStatus
{
    /// <summary><c>true</c> when all hard checks pass.</summary>
    [JsonPropertyName("ok")] public bool Ok { get; init; }

    /// <summary>Soft-check warnings — these do NOT flip <see cref="Ok"/>. Optional: absent
    /// when the server reports a bare <c>{"ok":true}</c>.</summary>
    [JsonPropertyName("warnings")] public List<HealthFailure>? Warnings { get; init; }

    /// <summary>Hard-check failures — at least one of these flips <see cref="Ok"/> to
    /// <c>false</c>. Optional on the wire.</summary>
    [JsonPropertyName("failures")] public List<HealthFailure>? Failures { get; init; }
}
