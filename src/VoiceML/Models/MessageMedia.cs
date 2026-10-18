using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace VoiceML.Models;

/// <summary>A Twilio-compatible media attachment on a Message (<c>ME</c> + 32 hex).
/// Models the response shape of
/// <c>GET /2010-04-01/Accounts/{AccountSid}/Messages/{MessageSid}/Media/{MediaSid}.json</c>.
/// VoiceML's outbound surface today does not emit MMS, so this is primarily a
/// read-only compatibility model for callers migrating from Twilio.</summary>
public sealed record Media
{
    /// <summary>Media SID (<c>ME</c> + 32 hex).</summary>
    [JsonPropertyName("sid")] public string Sid { get; init; } = "";

    /// <summary>Owning Account SID.</summary>
    [JsonPropertyName("account_sid")] public string AccountSid { get; init; } = "";

    /// <summary>Parent Message SID (<c>SM</c>/<c>MM</c> + 32 hex).</summary>
    [JsonPropertyName("parent_sid")] public string? ParentSid { get; init; }

    /// <summary>MIME content type of the attachment (e.g. <c>image/jpeg</c>).</summary>
    [JsonPropertyName("content_type")] public string? ContentType { get; init; }

    /// <summary>RFC 2822 creation timestamp.</summary>
    [JsonPropertyName("date_created")] public string? DateCreated { get; init; }

    /// <summary>RFC 2822 last-modification timestamp.</summary>
    [JsonPropertyName("date_updated")] public string? DateUpdated { get; init; }

    /// <summary>URI of this media resource.</summary>
    [JsonPropertyName("uri")] public string? Uri { get; init; }
}

/// <summary>List response for <c>GET /Messages/{MessageSid}/Media</c>. The JSON wire
/// envelope uses <c>media_list</c> for the items array; the C# property is named
/// <c>Items</c> to avoid colliding with the enclosing type name.</summary>
public sealed record MediaList : Page
{
    /// <summary>The page of Media resources. Wire name: <c>media_list</c>.</summary>
    [JsonPropertyName("media_list")] public List<Media> Items { get; init; } = new();
}

/// <summary>Query-string params for <c>GET /Messages/{sid}/Media</c>.</summary>
public sealed record ListMediaParams
{
    /// <summary>Filter to media created on this UTC date (<c>YYYY-MM-DD</c>).</summary>
    public string? DateCreated { get; init; }

    /// <summary>Media created strictly before this UTC date/time. Wire name: <c>DateCreated&lt;</c>.</summary>
    public string? DateCreatedLt { get; init; }

    /// <summary>Media created strictly after this UTC date/time. Wire name: <c>DateCreated&gt;</c>.</summary>
    public string? DateCreatedGt { get; init; }

    /// <summary>Zero-based page index.</summary>
    public int? Page { get; init; }

    /// <summary>Page size.</summary>
    public int? PageSize { get; init; }

    /// <summary>Opaque cursor for the next page, provided by the API.</summary>
    public string? PageToken { get; init; }

    /// <summary>Render as a query-parameter sequence.</summary>
    public IEnumerable<KeyValuePair<string, string?>> ToQuery()
    {
        yield return new("DateCreated", DateCreated);
        yield return new("DateCreated<", DateCreatedLt);
        yield return new("DateCreated>", DateCreatedGt);
        yield return new("Page", Page?.ToString());
        yield return new("PageSize", PageSize?.ToString());
        yield return new("PageToken", PageToken);
    }
}
