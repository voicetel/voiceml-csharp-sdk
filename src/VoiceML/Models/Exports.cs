using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace VoiceML.Models;

/// <summary>One nightly event-log archive available for self-service download
/// (<c>GET /2010-04-01/Accounts/{AccountSid}/Exports.json</c>). Archives cover calls,
/// messages, recordings metadata and payments; they are produced once per day and retained
/// for one year.</summary>
public sealed record Export
{
    /// <summary>Archive day (<c>YYYY-MM-DD</c>).</summary>
    [JsonPropertyName("date")] public string Date { get; init; } = "";

    /// <summary>Archive size in bytes.</summary>
    [JsonPropertyName("size")] public long Size { get; init; }

    /// <summary>Download path for this archive (relative to the API host). Fetching it
    /// redirects to a time-limited presigned URL.</summary>
    [JsonPropertyName("uri")] public string Uri { get; init; } = "";
}

/// <summary>List response for <c>GET /Exports</c> — the account's available archives,
/// newest first.</summary>
public sealed record ExportList
{
    /// <summary>Owning Account SID.</summary>
    [JsonPropertyName("account_sid")] public string AccountSid { get; init; } = "";

    /// <summary>The available archives, newest first.</summary>
    [JsonPropertyName("exports")] public List<Export> Exports { get; init; } = new();

    /// <summary>URI of this collection.</summary>
    [JsonPropertyName("uri")] public string? Uri { get; init; }
}

/// <summary>Result of downloading a daily event-log archive
/// (<c>GET /Exports/{Date}</c>). The endpoint answers with a 302 redirect to a presigned
/// URL; the SDK follows it (dropping credentials on the cross-host hop) and returns the
/// bytes.</summary>
public sealed record ExportArchive
{
    /// <summary>The archive day that was requested (<c>YYYY-MM-DD</c>).</summary>
    public string Date { get; init; } = "";

    /// <summary>The archive bytes.</summary>
    public byte[] Content { get; init; } = System.Array.Empty<byte>();

    /// <summary>Content-Type of the downloaded archive.</summary>
    public string ContentType { get; init; } = "application/octet-stream";

    /// <summary><c>true</c> when the bytes arrived via a redirect to a presigned URL.</summary>
    public bool ViaRedirect { get; init; }
}
