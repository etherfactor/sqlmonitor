using EtherGizmos.SqlMonitor.Models.Database.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace EtherGizmos.SqlMonitor.Models.Database;

[Table("instance_metrics_by_second")]
public class InstanceMetricByTime
{
    [Column("instance_id")]
    public virtual Guid InstanceId { get; set; }

    public virtual Instance Instance { get; set; }

    [Column("measured_at_utc")]
    public virtual DateTimeOffset MeasuredAtUtc { get; set; }

    [Column("metric_id")]
    public virtual Guid MetricId { get; set; }

    public virtual Metric Metric { get; set; }

    [Column("metric_bucket_id")]
    public virtual int MetricBucketId { get; set; }

    public virtual MetricBucket MetricBucket { get; set; }

    [Column("value")]
    public virtual double Value { get; set; }

    [Column("severity_type")]
    public virtual SeverityType SeverityType { get; set; }

    /// <summary>
    /// Not intended for direct use.
    /// </summary>
    public InstanceMetricByTime()
    {
        Instance = null!;
        Metric = null!;
        MetricBucket = null!;
    }
}
