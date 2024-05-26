using EtherGizmos.SqlMonitor.Shared.Models.Database.Enums;

namespace EtherGizmos.SqlMonitor.Shared.Messaging.Messages;

public class QueryExecuteMessage
{
    public Guid QueryId { get; set; }

    public string Name { get; set; }

    public int MonitoredQueryTargetId { get; set; }

    public string ConnectionRequestToken { get; set; }

    public SqlType SqlType { get; set; }

    public string QueryText { get; set; }

    public string? BucketColumn { get; set; }

    public string? TimestampUtcColumn { get; set; }

    public List<Metric> Metrics { get; set; } = new();

    /// <summary>
    /// Not intended for direct use.
    /// </summary>
    public QueryExecuteMessage()
    {
        ConnectionRequestToken = null!;
        Name = null!;
        QueryText = null!;
    }

    public QueryExecuteMessage(
        Guid queryId,
        string name,
        int monitoredQueryTargetId,
        string connectionRequestToken,
        SqlType sqlType,
        string queryText,
        string? bucketColumn,
        string? timestampUtcColumn)
    {
        QueryId = queryId;
        Name = name;
        MonitoredQueryTargetId = monitoredQueryTargetId;
        ConnectionRequestToken = connectionRequestToken;
        SqlType = sqlType;
        QueryText = queryText;
        BucketColumn = bucketColumn;
        TimestampUtcColumn = timestampUtcColumn;
    }

    public void AddMetric(
        int metricId,
        string valueColumn)
    {
        Metrics.Add(new(metricId, valueColumn));
    }

    public class Metric
    {
        public int MetricId { get; set; }

        public string ValueColumn { get; set; }

        /// <summary>
        /// Not intended for direct use.
        /// </summary>
        public Metric()
        {
            ValueColumn = null!;
        }

        public Metric(
            int metricId,
            string valueColumn)
        {
            MetricId = metricId;
            ValueColumn = valueColumn;
        }
    }
}
