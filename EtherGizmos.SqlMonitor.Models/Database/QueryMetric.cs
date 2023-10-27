using EtherGizmos.SqlMonitor.Models.Annotations;
using EtherGizmos.SqlMonitor.Models.Database.Abstractions;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EtherGizmos.SqlMonitor.Models.Database;

[Table("query_metrics")]
public class QueryMetric : Auditable
{
    [Column("query_id")]
    [Key]
    public virtual Guid QueryId { get; set; }

    [Lookup(nameof(QueryId), nameof(Query.Id),
        List = nameof(Query.Metrics))]
    public virtual Query Query { get; set; } = null!;

    [Column("metric_id")]
    [Key]
    public virtual Guid MetricId { get; set; }

    [Lookup(nameof(MetricId), nameof(Metric.Id))]
    public virtual Metric Metric { get; set; } = null!;

    [Column("value_expression")]
    public virtual string ValueExpression { get; set; } = null!;

    [LookupIndex("severities")]
    public virtual List<QueryMetricSeverity> Severities { get; set; } = new List<QueryMetricSeverity>();

    /// <summary>
    /// Not intended for direct use.
    /// </summary>
    public QueryMetric()
    {
    }
}
