using System.ComponentModel.DataAnnotations.Schema;

namespace EtherGizmos.SqlMonitor.Models.Database;

[Table("query_metrics")]
public class QueryMetric
{
    [Column("query_id")]
    public virtual Guid QueryId { get; set; }

    [Column("metric_id")]
    public virtual Guid MetricId { get; set; }

    [Column("value_expression")]
    public virtual string ValueExpression { get; set; }

    public virtual List<QueryMetricSeverity> Severities { get; set; } = new List<QueryMetricSeverity>();

    /// <summary>
    /// Not intended for direct use.
    /// </summary>
    public QueryMetric()
    {
        ValueExpression = null!;
    }
}
