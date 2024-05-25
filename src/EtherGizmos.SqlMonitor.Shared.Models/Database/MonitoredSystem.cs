using EtherGizmos.SqlMonitor.Shared.Utilities.Abstractions;
using EtherGizmos.SqlMonitor.Shared.Utilities.Annotations;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EtherGizmos.SqlMonitor.Shared.Models.Database;

[Table("monitored_systems")]
public class MonitoredSystem : Auditable
{
    [Column("monitored_system_id")]
    [Key]
    public virtual Guid Id { get; set; } = Guid.NewGuid();

    [Column("name")]
    public virtual string Name { get; set; }

    [Column("description")]
    public virtual string? Description { get; set; }

    [Column("is_active")]
    [SqlDefaultValue]
    public virtual bool IsActive { get; set; }

    [Column("is_soft_deleted")]
    [SqlDefaultValue]
    public virtual bool IsSoftDeleted { get; set; }

    [Column("securable_id")]
    public virtual int SecurableId { get; set; }

    public virtual Securable Securable { get; set; }

    /// <summary>
    /// Not intended for direct use.
    /// </summary>
    public MonitoredSystem()
    {
        Name = null!;
        Securable = null!;
    }

    public Task EnsureValid(IQueryable<MonitoredSystem> records)
    {
        return Task.CompletedTask;
    }
}
