﻿using EtherGizmos.SqlMonitor.Models.Database.Abstractions;
using EtherGizmos.SqlMonitor.Models.Database.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace EtherGizmos.SqlMonitor.Models.Database;

[Table("query_metric_severities")]
public class QueryMetricSeverity : Auditable
{
    [Column("query_id")]
    public virtual Guid QueryId { get; set; }

    [Column("metric_id")]
    public virtual Guid MetricId { get; set; }

    public virtual QueryMetric QueryMetric { get; set; }

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
        QueryMetric = null!;
    }
}
