namespace EtherGizmos.SqlMonitor.Shared.Messaging.Messages;

public class ScriptExecuteMessage
{
    public Guid ScriptId { get; set; }

    public string Name { get; set; }

    public int MonitoredScriptTargetId { get; set; }

    public string ConnectionRequestToken { get; set; }

    public ScriptExecuteMessageInterpreter Interpreter { get; set; }

    public string Text { get; set; }

    public string? BucketKey { get; set; }

    public string? TimestampKey { get; set; }

    public List<ScriptExecuteMessageMetric> Metrics { get; set; } = new();

    /// <summary>
    /// Not intended for direct use.
    /// </summary>
    public ScriptExecuteMessage()
    {
        Name = null!;
        ConnectionRequestToken = null!;
        Interpreter = null!;
        Text = null!;
    }

    public ScriptExecuteMessage(
        Guid scriptId,
        string name,
        int monitoredScriptTargetId,
        string connectionRequestToken,
        ScriptExecuteMessageInterpreter interpreter,
        string scriptText,
        string? bucketKey,
        string? timestampKey)
    {
        ScriptId = scriptId;
        Name = name;
        MonitoredScriptTargetId = monitoredScriptTargetId;
        ConnectionRequestToken = connectionRequestToken;
        Interpreter = interpreter;
        Text = scriptText;
        BucketKey = bucketKey;
        TimestampKey = timestampKey;
    }

    public void AddMetric(
        int metricId,
        string valueKey)
    {
        Metrics.Add(new(metricId, valueKey));
    }
}

public class ScriptExecuteMessageInterpreter
{
    public string Name { get; set; }

    public string Command { get; set; }

    public string Arguments { get; set; }

    public string Extension { get; set; }

    /// <summary>
    /// Not intended for direct use.
    /// </summary>
    public ScriptExecuteMessageInterpreter()
    {
        Name = null!;
        Command = null!;
        Arguments = null!;
        Extension = null!;
    }

    public ScriptExecuteMessageInterpreter(
        string name,
        string command,
        string arguments,
        string extension)
    {
        Name = name;
        Command = command;
        Arguments = arguments;
        Extension = extension;
    }
}

public class ScriptExecuteMessageMetric
{
    public int MetricId { get; set; }

    public string ValueKey { get; set; }

    /// <summary>
    /// Not intended for direct use.
    /// </summary>
    public ScriptExecuteMessageMetric()
    {
        ValueKey = null!;
    }

    public ScriptExecuteMessageMetric(
        int metricId,
        string valueKey)
    {
        MetricId = metricId;
        ValueKey = valueKey;
    }
}
