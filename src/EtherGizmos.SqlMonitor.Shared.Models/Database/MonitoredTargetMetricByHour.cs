using EtherGizmos.SqlMonitor.Shared.Models.Database.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace EtherGizmos.SqlMonitor.Shared.Models.Database;

[Table("monitored_target_metrics_by_hour")]
public class MonitoredTargetMetricByHour
{
    [Column("monitored_target_id")]
    public virtual int MonitoredTargetId { get; set; }

    [Column("metric_id")]
    public virtual int MetricId { get; set; }

    [Column("metric_bucket_id")]
    public virtual int MetricBucketId { get; set; }

    [Column("measured_at_utc")]
    public virtual DateTimeOffset MeasuredAtUtc { get; set; }

    [Column("value")]
    public virtual double Value { get; set; }

    [Column("severity_type_id")]
    public virtual SeverityType SeverityType { get; set; }
}
