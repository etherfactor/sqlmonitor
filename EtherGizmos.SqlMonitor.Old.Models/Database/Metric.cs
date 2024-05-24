using EtherGizmos.SqlMonitor.Models.Annotations;
using EtherGizmos.SqlMonitor.Models.Database.Abstractions;
using EtherGizmos.SqlMonitor.Models.Database.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EtherGizmos.SqlMonitor.Models.Database;

[Table("metrics")]
public class Metric : Auditable
{
    [Column("metric_id")]
    [Key, SqlDefaultValue]
    public virtual int Id { get; set; }

    [Column("name")]
    public virtual string Name { get; set; }

    [Column("description")]
    public virtual string? Description { get; set; }

    [Column("aggregate_type_id")]
    public virtual AggregateType AggregateType { get; set; }

    [Column("is_soft_deleted")]
    public virtual bool IsSoftDeleted { get; set; }

    [Column("securable_id")]
    public virtual int SecurableId { get; set; }

    public virtual Securable Securable { get; set; }

    /// <summary>
    /// Not intended for direct use.
    /// </summary>
    public Metric()
    {
        Name = null!;
        Securable = null!;
    }

    public Task EnsureValid(IQueryable<Metric> records)
    {
        return Task.CompletedTask;
    }
}
