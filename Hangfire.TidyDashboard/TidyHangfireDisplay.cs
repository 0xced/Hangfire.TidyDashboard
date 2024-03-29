using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net;
using System.Text;
using Hangfire.Common;

// ReSharper disable once CheckNamespace
namespace Hangfire.Dashboard;

internal static class TidyHangfireDisplay
{
    public static NonEscapedString? SucceededRenderer(HtmlHelper html, IDictionary<string, string> stateData)
    {
        const string dlTag = "<dl class=\"dl-horizontal\">";
        var builder = new StringBuilder(dlTag);

        if (stateData.TryGetValue("Latency", out var latencyString))
        {
            var latency = TimeSpan.FromMilliseconds(long.Parse(latencyString, CultureInfo.InvariantCulture));
            builder.Append($"<dt>Latency:</dt><dd>{html.HtmlEncode(html.ToHumanDuration(latency, false))}</dd>");
        }

        if (stateData.TryGetValue("PerformanceDuration", out var durationString))
        {
            var duration = TimeSpan.FromMilliseconds(long.Parse(durationString, CultureInfo.InvariantCulture));
            builder.Append($"<dt>Duration:</dt><dd>{html.HtmlEncode(html.ToHumanDuration(duration, false))}</dd>");
        }

        if (stateData.TryGetValue("Result", out var resultString) && !string.IsNullOrWhiteSpace(resultString))
        {
            var result = stateData["Result"];
            builder.Append($"<dt>Result:</dt><dd>{TidyHtmlEncode(result)}</dd>");
        }

        if (builder.Length == dlTag.Length) return null;

        builder.Append("</dl>");

        return new NonEscapedString(builder.ToString());
    }

    private static string TidyHtmlEncode(string text)
    {
        try
        {
            return WebUtility.HtmlEncode(SerializationHelper.Deserialize<string>(text, SerializationOption.User)).ReplaceLineEndings("<br>");
        }
        catch
        {
            return WebUtility.HtmlEncode(text);
        }
    }
}