using EtherGizmos.SqlMonitor.Models.Annotations;
using EtherGizmos.SqlMonitor.Models.Database.Abstractions;
using EtherGizmos.SqlMonitor.Models.Database.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EtherGizmos.SqlMonitor.Models.Database;

[Table("query_metric_severities")]
public class QueryMetricSeverity : Auditable
{
    [Key]
    [Column("query_id")]
    public virtual Guid QueryId { get; set; }

    [Key]
    [Column("metric_id")]
    public virtual Guid MetricId { get; set; }

    [Lookup(nameof(QueryId), nameof(MetricId),
        List = nameof(QueryMetric.Severities))]
    public virtual QueryMetric QueryMetric { get; set; } = null!;

    [Column("severity_type_id")]
    public virtual SeverityType SeverityType { get; set; }

    [Column("minimum_expression")]
    public virtual string? MinimumExpression { get; set; }

    [Column("maximum_expression")]
    public virtual string? MaximumExpression { get; set; }

    /// <summary>
    /// Not intended for direct use.
    /// </summary>
    public QueryMetricSeverity()
    {
    }
}
