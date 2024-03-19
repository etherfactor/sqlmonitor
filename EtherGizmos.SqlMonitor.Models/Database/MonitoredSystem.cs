using EtherGizmos.SqlMonitor.Models.Annotations;
using EtherGizmos.SqlMonitor.Models.Database.Abstractions;
using EtherGizmos.SqlMonitor.Models.Database.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EtherGizmos.SqlMonitor.Models.Database;

[Table("monitored_systems")]
public class MonitoredSystem : Auditable
{
    [Column("monitored_system_id")]
    [Key, SqlDefaultValue]
    public virtual Guid Id { get; set; }

    [Column("name")]
    public virtual required string Name { get; set; }

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

    public virtual Securable Securable { get; set; } = new Securable() { Type = SecurableType.MonitoredSystem };

    public Task EnsureValid(IQueryable<MonitoredSystem> records)
    {
        return Task.CompletedTask;
    }
}
