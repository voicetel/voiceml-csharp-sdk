using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace VoiceML.Models;

/// <summary>Twilio-compatible pagination envelope. Concrete list types
/// (e.g. <see cref="CallList"/>) inherit from this and add their typed item collection.</summary>
public abstract record Page
{
    /// <summary>Zero-based page index of this response. Wire name: <c>page</c>.</summary>
    [JsonPropertyName("page")] public int PageNumber { get; init; }

    /// <summary>Page size (count of items requested).</summary>
    [JsonPropertyName("page_size")] public int PageSize { get; init; } = 50;

    /// <summary>Total number of pages (when the server reports it; else <c>null</c>).</summary>
    [JsonPropertyName("num_pages")] public int? NumPages { get; init; }

    /// <summary>Total number of items across all pages (when reported).</summary>
    [JsonPropertyName("total")] public int? Total { get; init; }

    /// <summary>Absolute index of the first item in this page.</summary>
    [JsonPropertyName("start")] public int? Start { get; init; }

    /// <summary>Absolute index of the last item in this page.</summary>
    [JsonPropertyName("end")] public int? End { get; init; }

    /// <summary>URI to fetch the first page.</summary>
    [JsonPropertyName("first_page_uri")] public string? FirstPageUri { get; init; }

    /// <summary>URI to fetch the next page; <c>null</c> when this is the last page.</summary>
    [JsonPropertyName("next_page_uri")] public string? NextPageUri { get; init; }

    /// <summary>URI to fetch the previous page; <c>null</c> when this is the first page.</summary>
    [JsonPropertyName("previous_page_uri")] public string? PreviousPageUri { get; init; }

    /// <summary>URI of this resource collection.</summary>
    [JsonPropertyName("uri")] public string? Uri { get; init; }
}

/// <summary>Twilio-compatible error body. The transport raises a <see cref="VoiceML.Exceptions.ApiException"/>
/// (or subclass) with this payload attached on <c>Body</c>.</summary>
public sealed record ErrorBody
{
    /// <summary>Numeric Twilio-style error code (e.g. 21211).</summary>
    [JsonPropertyName("code")] public int? Code { get; init; }

    /// <summary>Human-readable error message.</summary>
    [JsonPropertyName("message")] public string? Message { get; init; }

    /// <summary>Documentation URL with more detail on this error class.</summary>
    [JsonPropertyName("more_info")] public string? MoreInfo { get; init; }

    /// <summary>HTTP status code, repeated in the body for clients that parse only JSON.</summary>
    [JsonPropertyName("status")] public int? Status { get; init; }
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

    /// <summary>Soft-check warnings — these do NOT flip <see cref="Ok"/>.</summary>
    [JsonPropertyName("warnings")] public List<HealthFailure> Warnings { get; init; } = new();

    /// <summary>Hard-check failures — at least one of these flips <see cref="Ok"/> to <c>false</c>.</summary>
    [JsonPropertyName("failures")] public List<HealthFailure> Failures { get; init; } = new();
}
