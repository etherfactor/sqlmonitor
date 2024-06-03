namespace EtherGizmos.SqlMonitor.Shared.Messaging.Messages;

public class ScriptResultMessage
{
    public Guid ScriptId { get; set; }

    public string Name { get; set; }

    public int MonitoredScriptTargetId { get; set; }

    public long ExecutionMilliseconds { get; set; }

    public List<ScriptResultMessageMetricValue> MetricValues { get; set; } = new();

    /// <summary>
    /// Not intended for direct use.
    /// </summary>
    public ScriptResultMessage()
    {
        Name = null!;
    }

    public ScriptResultMessage(
        Guid scriptId,
        string name,
        int monitoredScriptTargetId,
        long executionMilliseconds)
    {
        ScriptId = scriptId;
        Name = name;
        MonitoredScriptTargetId = monitoredScriptTargetId;
        ExecutionMilliseconds = executionMilliseconds;
    }

    public void AddMetricValue(
        int metricId,
        string? bucket,
        DateTimeOffset? timestampUtc,
        double value)
    {
        MetricValues.Add(new(metricId, bucket, timestampUtc, value));
    }
}

public class ScriptResultMessageMetricValue
{
    public int MetricId { get; set; }

    public string? Bucket { get; set; }

    public DateTimeOffset? TimestampUtc { get; set; }

    public double Value { get; set; }

    /// <summary>
    /// Not intended for direct use.
    /// </summary>
    public ScriptResultMessageMetricValue()
    {
    }

    public ScriptResultMessageMetricValue(
        int metricId,
        string? bucket,
        DateTimeOffset? timestampUtc,
        double value)
    {
        MetricId = metricId;
        Bucket = bucket;
        TimestampUtc = timestampUtc;
        Value = value;
    }
}
