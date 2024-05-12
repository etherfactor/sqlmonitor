using EtherGizmos.SqlMonitor.Models.Database;
using System.Text.RegularExpressions;

namespace EtherGizmos.SqlMonitor.Api.Services.Scripts;

/// <summary>
/// A set of results produced by a script.
/// </summary>
public partial class ScriptExecutionResultSet
{
    /// <summary>
    /// The server against which the script was run.
    /// </summary>
    public required MonitoredScriptTarget MonitoredScriptTarget { get; set; }

    /// <summary>
    /// The script that was run.
    /// </summary>
    public required ScriptVariant ScriptVariant { get; set; }

    /// <summary>
    /// The duration the script took to run.
    /// </summary>
    public required long ExecutionMilliseconds { get; set; }

    /// <summary>
    /// The key-value pairs returned by the script.
    /// </summary>
    public required IEnumerable<ScriptExecutionResult> Results { get; set; }

    /// <summary>
    /// A metric used to extract metric outputs from the script stdout.
    /// </summary>
    /// <returns></returns>
    [GeneratedRegex(@"^##metric:?(?:\s*?([A-Za-z_]+)=([^ """"]+|""(?:[^""""\\]|\\"")*?(?<!\\)""))+$")]
    private static partial Regex MetricValueRegex();

    /// <summary>
    /// Create a result set from a script stdout.
    /// </summary>
    /// <param name="scriptTarget">The server against which the script was run.</param>
    /// <param name="scriptVariant">The script that was run.</param>
    /// <param name="outputText">The script stdout.</param>
    /// <param name="executionMilliseconds">The duration the script took to run.</param>
    /// <returns>The set of results produced by the script.</returns>
    public static ScriptExecutionResultSet FromResults(
        MonitoredScriptTarget scriptTarget,
        ScriptVariant scriptVariant,
        string outputText,
        long executionMilliseconds)
    {
        //Not sure which newline format the script has, so just convert to \n
        var resultLines = outputText
            //If \r\n is found, this should replace all the \r
            .Replace("\r\n", "\n")
            //If any \r are found, likely wasn't \r\n
            .Replace("\r", "\n")
            //Now split everything into lines
            .Split("\n");

        var executionResults = new List<ScriptExecutionResult>();
        var metricValueRegex = MetricValueRegex();
        foreach (var line in resultLines)
        {
            //Attempt to match the regex with the stdout line
            var match = metricValueRegex.Match(line);
            if (match.Success)
            {
                var executionResult = new ScriptExecutionResult() { Values = new Dictionary<string, string>() };
                for (int i = 0; i < match.Groups[1].Captures.Count; i++)
                {
                    //Extract the key=value capture groups and set them in the output
                    var variableName = match.Groups[1].Captures[i].Value;
                    var variableValue = match.Groups[2].Captures[i].Value;

                    if (variableValue.Length >= 2 && variableValue[0] == '"')
                    {
                        variableValue = variableValue.Substring(1, variableValue.Length - 2)
                            .Replace("\\\"", "\"");
                    }

                    executionResult.Values.TryAdd(variableName, variableValue);
                }

                executionResults.Add(executionResult);
            }
        }

        return new()
        {
            MonitoredScriptTarget = scriptTarget,
            ScriptVariant = scriptVariant,
            ExecutionMilliseconds = executionMilliseconds,
            Results = executionResults,
        };
    }
}
