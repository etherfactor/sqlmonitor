using EtherGizmos.SqlMonitor.Models.Annotations;
using EtherGizmos.SqlMonitor.Models.Database.Abstractions;
using EtherGizmos.SqlMonitor.Models.Database.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EtherGizmos.SqlMonitor.Models.Database;

[Table("monitored_resources")]
public class MonitoredResource : Auditable
{
    [Column("monitored_resource_id")]
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

    public virtual Securable Securable { get; set; } = new Securable() { Type = SecurableType.MonitoredResource };

    public Task EnsureValid(IQueryable<MonitoredResource> records)
    {
        return Task.CompletedTask;
    }
}
