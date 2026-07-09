using System;

namespace VoiceML;

/// <summary>Per-product host resolution for the VoiceML API.
/// <para>Twilio splits its products across dedicated subdomains
/// (<c>api.twilio.com</c>, <c>conversations.twilio.com</c>, <c>messaging.twilio.com</c>, …).
/// VoiceML mirrors that shape on <c>voicetel.com</c>: the Conversations product answers on
/// <c>conversations.voicetel.com</c> and the Messaging Service product on
/// <c>messaging.voicetel.com</c>, while everything else stays on the default
/// <c>voiceml.voicetel.com</c> host. Conversation Service and Messaging Service share the
/// identical <c>/v1/Services</c> path shape, so the <em>host</em> is what disambiguates them
/// on the wire.</para>
/// <para>Given the configured base URL this type derives the two product hosts by swapping the
/// leftmost <c>voiceml</c> label — but only for recognised <c>*.voicetel.com</c> hosts. For any
/// other base URL (a self-hosted callBroadcast instance, a test stub, a regional override) the
/// product hosts fall back to the configured host unchanged, so a single-host deployment keeps
/// working. A caller who needs Messaging Service against a custom host points
/// <see cref="ClientOptions.MessagingBaseUrl"/> (or <see cref="ClientOptions.ConversationsBaseUrl"/>)
/// at their own subdomain explicitly.</para>
/// </summary>
public static class ProductHosts
{
    /// <summary>Resolve the <c>(Default, Messaging, Conversations)</c> base URLs from a configured
    /// <paramref name="baseUrl"/>. Explicit overrides win; otherwise each product host is derived
    /// from <paramref name="baseUrl"/> (see the type docs). All three are returned without a
    /// trailing slash.</summary>
    public static (string Default, string Messaging, string Conversations) Resolve(
        string baseUrl,
        string? messagingBaseUrl = null,
        string? conversationsBaseUrl = null)
    {
        var def = baseUrl.TrimEnd('/');
        var messaging = (messagingBaseUrl ?? DeriveProductHost(def, "messaging")).TrimEnd('/');
        var conversations = (conversationsBaseUrl ?? DeriveProductHost(def, "conversations")).TrimEnd('/');
        return (def, messaging, conversations);
    }

    /// <summary>Swap the <c>voiceml</c> label of a <c>*.voicetel.com</c> host for
    /// <paramref name="product"/>. Returns <paramref name="baseUrl"/> unchanged when the host is
    /// not a <c>voiceml.*.voicetel.com</c> style host (e.g. a self-hosted instance), so single-host
    /// deployments keep working without special-casing.</summary>
    internal static string DeriveProductHost(string baseUrl, string product)
    {
        if (!Uri.TryCreate(baseUrl, UriKind.Absolute, out var uri))
        {
            return baseUrl;
        }
        var host = uri.Host;
        if (string.IsNullOrEmpty(host) || !host.EndsWith(".voicetel.com", StringComparison.Ordinal))
        {
            return baseUrl;
        }
        var labels = host.Split('.');
        var idx = Array.IndexOf(labels, "voiceml");
        if (idx < 0)
        {
            return baseUrl;
        }
        labels[idx] = product;
        var newHost = string.Join('.', labels);

        var portPart = uri.IsDefaultPort ? string.Empty : ":" + uri.Port;
        var path = uri.AbsolutePath == "/" ? string.Empty : uri.AbsolutePath;
        return $"{uri.Scheme}://{newHost}{portPart}{path}";
    }
}
