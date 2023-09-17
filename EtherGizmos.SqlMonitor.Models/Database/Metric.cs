using EtherGizmos.SqlMonitor.Models.Annotations;
using EtherGizmos.SqlMonitor.Models.Database.Abstractions;
using EtherGizmos.SqlMonitor.Models.Database.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace EtherGizmos.SqlMonitor.Models.Database;

[Table("metrics")]
public class Metric : Auditable
{
    [Column("metric_id")]
    [SqlDefaultValue]
    public virtual Guid Id { get; set; }

    [Column("name")]
    public virtual string Name { get; set; }

    [Column("description")]
    public virtual string Description { get; set; }

    [Column("aggregate_type_id")]
    public virtual AggregateType AggregateType { get; set; }

    public virtual List<MetricSeverity> Severities { get; set; } = new List<MetricSeverity>();

    /// <summary>
    /// Not intended for direct use.
    /// </summary>
    public Metric()
    {
        Name = null!;
        Description = null!;
    }

    public Task EnsureValid(IQueryable<Metric> records)
    {
        return Task.CompletedTask;
    }
}
