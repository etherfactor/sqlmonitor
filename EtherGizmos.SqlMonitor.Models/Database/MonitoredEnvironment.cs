﻿using EtherGizmos.SqlMonitor.Models.Annotations;
using EtherGizmos.SqlMonitor.Models.Database.Abstractions;
using EtherGizmos.SqlMonitor.Models.Database.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EtherGizmos.SqlMonitor.Models.Database;

[Table("monitored_environments")]
public class MonitoredEnvironment : Auditable
{
    [Column("monitored_environment_id")]
    [Key, SqlDefaultValue]
    public virtual Guid Id { get; set; }

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

    public virtual Securable Securable { get; set; } = new Securable() { Type = SecurableType.MonitoredEnvironment };

    /// <summary>
    /// Not intended for direct use.
    /// </summary>
    public MonitoredEnvironment()
    {
        Name = null!;
    }

    public Task EnsureValid(IQueryable<MonitoredEnvironment> records)
    {
        return Task.CompletedTask;
    }
}
