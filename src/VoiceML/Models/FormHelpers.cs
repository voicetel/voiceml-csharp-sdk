using System.Collections.Generic;

namespace VoiceML.Models;

/// <summary>Implemented by request-body records that serialize to <c>application/x-www-form-urlencoded</c>.
/// The transport drops null values; <see cref="ToForm"/> may emit nulls freely (and should — they make
/// the per-property mapping easier to read).</summary>
public interface IFormSerializable
{
    /// <summary>Render this request to a sequence of form pairs. Nulls are filtered downstream.</summary>
    IEnumerable<KeyValuePair<string, string?>> ToForm();
}

/// <summary>Per-field encoders for form bodies. Twilio expects lowercase <c>"true"</c>/<c>"false"</c>
/// for booleans (not C#'s default <c>"True"</c>/<c>"False"</c>).</summary>
public static class FormHelpers
{
    /// <summary>Encode a nullable bool as Twilio expects: <c>"true"</c>, <c>"false"</c>, or <c>null</c>.</summary>
    public static string? BoolStr(bool? value) => value switch
    {
        true => "true",
        false => "false",
        null => null,
    };
}
