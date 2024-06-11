using EtherGizmos.SqlMonitor.Shared.Messaging.Messages;
using System.Data.Common;
using System.Text.RegularExpressions;

namespace EtherGizmos.SqlMonitor.Agent.Core.Helpers;

public static partial class MessageHelper
{
    public static async Task ReadIntoMessageAsync(
        QueryExecuteMessage executeMessage,
        QueryResultMessage resultMessage,
        DbDataReader reader,
        DateTimeOffset executedAtUtc)
    {
        var count = reader.FieldCount;
        while (await reader.ReadAsync())
        {
            var currentValues = new List<(int MetricId, double Value)>();
            string? currentBucket = null;
            DateTimeOffset? currentTimestampUtc = null;

            //Place each column as a key-value pair in the result
            for (int i = 0; i < count; i++)
            {
                var variableKey = reader.GetName(i);
                var variableValue = reader.GetValue(i);

                if (variableKey.Equals(executeMessage.BucketColumn, StringComparison.CurrentCultureIgnoreCase))
                {
                    currentBucket = variableValue.ToString();
                }
                else if (variableKey.Equals(executeMessage.TimestampUtcColumn, StringComparison.CurrentCultureIgnoreCase))
                {
                    if (DateTimeOffset.TryParse(variableValue.ToString(), out DateTimeOffset parsed))
                    {
                        currentTimestampUtc = parsed;
                    }
                }
                else
                {
                    var maybeMetric = executeMessage.Metrics.SingleOrDefault(e => variableKey.Equals(e.ValueColumn, StringComparison.InvariantCultureIgnoreCase));
                    if (maybeMetric is not null && double.TryParse(variableValue.ToString(), out double variableDouble))
                    {
                        currentValues.Add((maybeMetric.MetricId, variableDouble));
                    }
                }
            }

            foreach (var currentValue in currentValues)
            {
                resultMessage.AddMetricValue(currentValue.MetricId, currentBucket, currentTimestampUtc ?? executedAtUtc, currentValue.Value);
            }
        }
    }

    public static Task ReadIntoMessageAsync(
        ScriptExecuteMessage executeMessage,
        ScriptResultMessage resultMessage,
        string outputText,
        DateTimeOffset executedAtUtc)
    {
        //Not sure which newline format the script has, so just convert to \n
        var resultLines = outputText
            //If \r\n is found, this should replace all the \r
            .Replace("\r\n", "\n")
            //If any \r are found, likely wasn't \r\n
            .Replace("\r", "\n")
            //Now split everything into lines
            .Split("\n");

        var metricValueRegex = MetricValueRegex();
        foreach (var line in resultLines)
        {
            var currentValues = new List<(int MetricId, double Value)>();
            string? currentBucket = null;
            DateTimeOffset? currentTimestampUtc = null;

            //Attempt to match the regex with the stdout line
            var match = metricValueRegex.Match(line);
            if (match.Success)
            {
                var executionResult = new Dictionary<string, string>();
                for (int i = 0; i < match.Groups[1].Captures.Count; i++)
                {
                    //Extract the key=value capture groups and set them in the output
                    var variableKey = match.Groups[1].Captures[i].Value;
                    var variableValue = match.Groups[2].Captures[i].Value;

                    if (variableValue.Length >= 2 && variableValue[0] == '"')
                    {
                        variableValue = variableValue.Substring(1, variableValue.Length - 2)
                            .Replace("\\\"", "\"");
                    }

                    if (variableKey.Equals(executeMessage.BucketKey, StringComparison.CurrentCultureIgnoreCase))
                    {
                        currentBucket = variableValue.ToString();
                    }
                    else if (variableKey.Equals(executeMessage.TimestampUtcKey, StringComparison.CurrentCultureIgnoreCase))
                    {
                        if (DateTimeOffset.TryParse(variableValue.ToString(), out DateTimeOffset parsed))
                        {
                            currentTimestampUtc = parsed;
                        }
                    }
                    else
                    {
                        var maybeMetric = executeMessage.Metrics.SingleOrDefault(e => variableKey.Equals(e.ValueKey, StringComparison.InvariantCultureIgnoreCase));
                        if (maybeMetric is not null && double.TryParse(variableValue.ToString(), out double variableDouble))
                        {
                            currentValues.Add((maybeMetric.MetricId, variableDouble));
                        }
                    }
                }
            }

            foreach (var currentValue in currentValues)
            {
                resultMessage.AddMetricValue(currentValue.MetricId, currentBucket, currentTimestampUtc ?? executedAtUtc, currentValue.Value);
            }
        }

        return Task.CompletedTask;
    }

    [GeneratedRegex(@"^##metric:?(?:\s*?([A-Za-z_]+)=([^ """"]+|""(?:[^""""\\]|\\"")*?(?<!\\)""))+$")]
    private static partial Regex MetricValueRegex();
}
