using EtherGizmos.SqlMonitor.Models.Database.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace EtherGizmos.SqlMonitor.Models.Database;

[Table("metric_severities")]
public class MetricSeverity
{
    [Column("metric_id")]
    public virtual Guid MetricId { get; set; }

    [Column("severity_id")]
    public virtual SeverityType SeverityType { get; set; }

    [Column("minimum_value")]
    public virtual decimal MinimumValue { get; set; }

    [Column("maximum_value")]
    public virtual decimal MaximumValue { get; set; }
}
