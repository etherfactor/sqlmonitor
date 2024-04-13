using EtherGizmos.SqlMonitor.Models.Database;
using System.Text.RegularExpressions;

namespace EtherGizmos.SqlMonitor.Api.Services.Scripts;

public partial class ScriptExecutionResultSet
{
    public required MonitoredScriptTarget MonitoredScriptTarget { get; set; }

    public required ScriptVariant ScriptVariant { get; set; }

    public required long ExecutionMilliseconds { get; set; }

    public required IEnumerable<ScriptExecutionResult> Results { get; set; }

    [GeneratedRegex(@"^##metric:?(?:\s*?([A-Za-z_]+)=([^ """"]+|""(?:[^""""\\]|\\"")*?(?<!\\)""))+$")]
    private static partial Regex MetricValueRegex();

    public static ScriptExecutionResultSet FromResults(
        MonitoredScriptTarget scriptTarget,
        ScriptVariant scriptVariant,
        string outputText,
        long executionMilliseconds)
    {
        var resultLines = outputText
            .Replace("\r\n", "\n")
            .Split("\n");

        var executionResults = new List<ScriptExecutionResult>();
        var metricValueRegex = MetricValueRegex();
        foreach (var line in resultLines)
        {
            var match = metricValueRegex.Match(line);
            if (match.Success)
            {
                var executionResult = new ScriptExecutionResult() { Values = new Dictionary<string, string>() };
                for (int i = 0; i < match.Groups[1].Captures.Count; i++)
                {
                    var variableName = match.Groups[1].Captures[i].Value;
                    var variableValue = match.Groups[2].Captures[i].Value;

                    if (variableValue.Length >= 2 && variableValue[0] == '"')
                    {
                        variableValue = variableValue.Substring(1, variableValue.Length - 2)
                            .Replace("\\\"", "\"");
                    }

                    executionResult.Values.Add(variableName, variableValue);
                }

                executionResults.Add(executionResult);
            }
        }

        return new ScriptExecutionResultSet()
        {
            MonitoredScriptTarget = scriptTarget,
            ScriptVariant = scriptVariant,
            Results = executionResults,
            ExecutionMilliseconds = executionMilliseconds,
        };
    }
}
