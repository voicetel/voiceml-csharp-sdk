using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace VoiceML.Models;

/// <summary>A Twilio-shape Queue resource.</summary>
public sealed record Queue
{
    /// <summary>Queue SID (<c>QU</c> + 32 hex).</summary>
    [JsonPropertyName("sid")] public string Sid { get; init; } = "";

    /// <summary>Owning Account SID.</summary>
    [JsonPropertyName("account_sid")] public string AccountSid { get; init; } = "";

    /// <summary>Friendly name (unique per account).</summary>
    [JsonPropertyName("friendly_name")] public string FriendlyName { get; init; } = "";

    /// <summary>Current count of waiting members.</summary>
    [JsonPropertyName("current_size")] public int CurrentSize { get; init; }

    /// <summary>Maximum number of members allowed.</summary>
    [JsonPropertyName("max_size")] public int MaxSize { get; init; }

    /// <summary>Average wait time, in seconds.</summary>
    [JsonPropertyName("average_wait_time")] public int AverageWaitTime { get; init; }

    /// <summary>RFC 3339 creation timestamp.</summary>
    [JsonPropertyName("date_created")] public string DateCreated { get; init; } = "";

    /// <summary>RFC 3339 last-modification timestamp.</summary>
    [JsonPropertyName("date_updated")] public string DateUpdated { get; init; } = "";

    /// <summary>URI of this queue.</summary>
    [JsonPropertyName("uri")] public string Uri { get; init; } = "";
}

/// <summary>List response for <c>GET /Queues</c>.</summary>
public sealed record QueueList : Page
{
    /// <summary>The page of Queue resources.</summary>
    [JsonPropertyName("queues")] public List<Queue> Queues { get; init; } = new();
}

/// <summary>A call enqueued in a Queue.</summary>
public sealed record QueueMember
{
    /// <summary>SID of the enqueued call.</summary>
    [JsonPropertyName("call_sid")] public string CallSid { get; init; } = "";

    /// <summary>Owning Queue SID.</summary>
    [JsonPropertyName("queue_sid")] public string QueueSid { get; init; } = "";

    /// <summary>Owning Account SID.</summary>
    [JsonPropertyName("account_sid")] public string AccountSid { get; init; } = "";

    /// <summary>RFC 3339 timestamp the call was enqueued.</summary>
    [JsonPropertyName("date_enqueued")] public string DateEnqueued { get; init; } = "";

    /// <summary>Wait time in seconds at this moment.</summary>
    [JsonPropertyName("wait_time")] public int WaitTime { get; init; }

    /// <summary>Position in the queue (1 = front).</summary>
    [JsonPropertyName("position")] public int Position { get; init; }

    /// <summary>URI of this queue-member resource.</summary>
    [JsonPropertyName("uri")] public string Uri { get; init; } = "";
}

/// <summary>List response for <c>GET /Queues/{sid}/Members</c>.</summary>
public sealed record QueueMemberList : Page
{
    /// <summary>The page of QueueMember resources.</summary>
    [JsonPropertyName("queue_members")] public List<QueueMember> QueueMembers { get; init; } = new();
}

/// <summary>Query-string params for <c>GET /Queues/{sid}/Members</c>.</summary>
public sealed record ListQueueMembersParams
{
    /// <summary>Zero-based page index.</summary>
    public int? Page { get; init; }

    /// <summary>Page size.</summary>
    public int? PageSize { get; init; }

    /// <summary>Render as a query-parameter sequence.</summary>
    public IEnumerable<KeyValuePair<string, string?>> ToQuery()
    {
        yield return new("Page", Page?.ToString());
        yield return new("PageSize", PageSize?.ToString());
    }
}

/// <summary>Body for <c>POST /Queues</c>. Idempotent on <see cref="FriendlyName"/>.</summary>
public sealed record CreateQueueRequest : IFormSerializable
{
    /// <summary>Friendly name (max 64 chars).</summary>
    public required string FriendlyName { get; init; }

    /// <summary>Maximum queue size. <c>0</c> means unlimited (Twilio default).</summary>
    public int? MaxSize { get; init; }

    /// <inheritdoc/>
    public IEnumerable<KeyValuePair<string, string?>> ToForm()
    {
        yield return new("FriendlyName", FriendlyName);
        yield return new("MaxSize", MaxSize?.ToString());
    }
}

/// <summary>Body for <c>POST /Queues/{sid}</c>. Partial — only set fields are touched.</summary>
public sealed record UpdateQueueRequest : IFormSerializable
{
    /// <summary>New friendly name.</summary>
    public string? FriendlyName { get; init; }

    /// <summary>New maximum queue size. <c>0</c> means unlimited (Twilio default).</summary>
    public int? MaxSize { get; init; }

    /// <inheritdoc/>
    public IEnumerable<KeyValuePair<string, string?>> ToForm()
    {
        yield return new("FriendlyName", FriendlyName);
        yield return new("MaxSize", MaxSize?.ToString());
    }
}

/// <summary>Body for <c>POST /Queues/{sid}/Members/Front</c> and <c>POST /Queues/{sid}/Members/{CallSid}</c>.</summary>
public sealed record DequeueRequest : IFormSerializable
{
    /// <summary>URL fetched to render TwiML for the dequeued call.</summary>
    public required string Url { get; init; }

    /// <summary>HTTP method (<c>GET</c> or <c>POST</c>).</summary>
    public string? Method { get; init; }

    /// <inheritdoc/>
    public IEnumerable<KeyValuePair<string, string?>> ToForm()
    {
        yield return new("Url", Url);
        yield return new("Method", Method);
    }
}
