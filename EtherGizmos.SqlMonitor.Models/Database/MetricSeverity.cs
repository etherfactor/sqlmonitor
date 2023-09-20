using EtherGizmos.SqlMonitor.Models.Database.Abstractions;
using EtherGizmos.SqlMonitor.Models.Database.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace EtherGizmos.SqlMonitor.Models.Database;

[Table("metric_severities")]
public class MetricSeverity : Auditable
{
    [Column("metric_id")]
    public virtual Guid MetricId { get; set; }

    public virtual Metric Metric { get; set; }

    [Column("severity_id")]
    public virtual SeverityType SeverityType { get; set; }

    [Column("minimum_value")]
    public virtual double MinimumValue { get; set; }

    [Column("maximum_value")]
    public virtual double MaximumValue { get; set; }

    /// <summary>
    /// Not intended for direct use.
    /// </summary>
    public MetricSeverity()
    {
        Metric = null!;
    }
}
