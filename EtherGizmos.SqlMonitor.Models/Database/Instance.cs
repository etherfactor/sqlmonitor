using EtherGizmos.SqlMonitor.Models.Database.Abstractions;
using Redis.OM.Modeling;
using System.ComponentModel.DataAnnotations.Schema;

namespace EtherGizmos.SqlMonitor.Models.Database;

[Table("instances")]
[Document(StorageType = StorageType.Hash, Prefixes = new[] { "instances" })]
public class Instance : Auditable
{
    [Column("instance_id")]
    [RedisIdField]
    public virtual Guid Id { get; set; }

    [Column("name")]
    public virtual string Name { get; set; }

    [Column("description")]
    public virtual string? Description { get; set; }

    [Column("is_active")]
    public virtual bool IsActive { get; set; } = true;

    [Column("is_soft_deleted")]
    public virtual bool IsSoftDeleted { get; set; } = false;

    [Column("address")]
    public virtual string Address { get; set; }

    [Column("port")]
    public virtual short? Port { get; set; }

    [Column("database")]
    public virtual string? Database { get; set; }

    public virtual List<InstanceQueryBlacklist> QueryBlacklists { get; set; } = new List<InstanceQueryBlacklist>();

    public virtual List<InstanceQueryWhitelist> QueryWhitelists { get; set; } = new List<InstanceQueryWhitelist>();

    public virtual List<InstanceQueryDatabase> QueryDatabaseOverrides { get; set; } = new List<InstanceQueryDatabase>();

    /// <summary>
    /// Not intended for direct use.
    /// </summary>
    public Instance()
    {
        Name = null!;
        Address = null!;
    }

    public Task EnsureValid(IQueryable<Instance> records)
    {
        return Task.CompletedTask;
    }
}
