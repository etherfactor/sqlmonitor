using EtherGizmos.SqlMonitor.Models.Annotations;
using EtherGizmos.SqlMonitor.Models.Database.Abstractions;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EtherGizmos.SqlMonitor.Models.Database;

[Table("query_metrics")]
public class QueryMetric : Auditable
{
    [Column("query_metric_id")]
    [Key, SqlDefaultValue]
    public virtual int Id { get; set; }

    [Column("query_id")]
    public virtual Guid QueryId { get; set; }

    public virtual Query Query { get; set; }

    [Column("metric_id")]
    public virtual int MetricId { get; set; }

    public virtual Metric Metric { get; set; }

    [Column("value_column")]
    public virtual string ValueKey { get; set; }

    [Column("is_active")]
    [SqlDefaultValue]
    public virtual bool IsActive { get; set; }

    /// <summary>
    /// Not intended for direct use.
    /// </summary>
    public QueryMetric()
    {
        Query = null!;
        Metric = null!;
        ValueKey = null!;
    }
}
