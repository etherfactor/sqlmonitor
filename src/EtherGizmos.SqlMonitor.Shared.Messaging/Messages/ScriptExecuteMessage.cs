namespace EtherGizmos.SqlMonitor.Shared.Messaging.Messages;

public class ScriptExecuteMessage
{
    public Guid ScriptId { get; set; }

    public string Name { get; set; }

    public int MonitoredScriptTargetId { get; set; }

    public string ConnectionRequestToken { get; set; }

    public string ScriptText { get; set; }

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
        ScriptText = null!;
    }

    public ScriptExecuteMessage(
        Guid scriptId,
        string name,
        int monitoredScriptTargetId,
        string connectionRequestToken,
        string scriptText,
        string? bucketKey,
        string? timestampKey)
    {
        ScriptId = scriptId;
        Name = name;
        MonitoredScriptTargetId = monitoredScriptTargetId;
        ConnectionRequestToken = connectionRequestToken;
        ScriptText = scriptText;
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
