namespace EtherGizmos.SqlMonitor.Shared.Messaging.Messages;

public class QueryResultMessage
{
    public Guid QueryId { get; set; }

    public string Name { get; set; }

    public int MonitoredQueryTargetId { get; set; }

    public List<MetricValue> MetricValues { get; set; } = new();

    public QueryResultMessage()
    {
        Name = null!;
    }

    public QueryResultMessage(
        Guid queryId,
        string name,
        int monitoredQueryTargetId)
    {
        QueryId = queryId;
        Name = name;
        MonitoredQueryTargetId = monitoredQueryTargetId;
    }

    public void AddMetricValue(
        int metricId,
        string? bucket,
        DateTimeOffset? timestampUtc,
        double value)
    {
        MetricValues.Add(new(metricId, bucket, timestampUtc, value));
    }

    public class MetricValue
    {
        public int MetricId { get; set; }

        public string? Bucket { get; set; }

        public DateTimeOffset? TimestampUtc { get; set; }

        public double Value { get; set; }

        /// <summary>
        /// Not intended for direct use.
        /// </summary>
        public MetricValue()
        {
        }

        public MetricValue(
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
}
