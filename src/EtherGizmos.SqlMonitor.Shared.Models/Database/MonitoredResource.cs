using EtherGizmos.SqlMonitor.Shared.Models.Annotations;
using EtherGizmos.SqlMonitor.Shared.Utilities.Abstractions;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EtherGizmos.SqlMonitor.Shared.Models.Database;

[Table("monitored_resources")]
public class MonitoredResource : Auditable
{
    [Column("monitored_resource_id")]
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
    public MonitoredResource()
    {
        Name = null!;
        Securable = null!;
    }

    public Task EnsureValid(IQueryable<MonitoredResource> records)
    {
        return Task.CompletedTask;
    }
}
