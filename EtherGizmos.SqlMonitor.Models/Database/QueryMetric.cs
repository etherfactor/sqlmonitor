﻿using EtherGizmos.SqlMonitor.Models.Annotations;
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

    public virtual Query Query { get; set; }

    [Column("metric_id")]
    [Key]
    public virtual Guid MetricId { get; set; }

    [LookupSingle(nameof(MetricId))]
    public virtual Metric Metric { get; set; }

    [Column("value_expression")]
    public virtual string ValueExpression { get; set; }

    public virtual List<QueryMetricSeverity> Severities { get; set; } = new List<QueryMetricSeverity>();

    /// <summary>
    /// Not intended for direct use.
    /// </summary>
    public QueryMetric()
    {
        Query = null!;
        Metric = null!;
        ValueExpression = null!;
    }
}
