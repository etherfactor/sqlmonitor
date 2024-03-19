﻿using EtherGizmos.SqlMonitor.Models.Database.Abstractions;
using EtherGizmos.SqlMonitor.Models.Database.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace EtherGizmos.SqlMonitor.Models.Database;

[Table("scripts")]
public class Script : Auditable
{
    [Column("script_id")]
    public virtual Guid Id { get; set; }

    [Column("name")]
    public virtual required string Name { get; set; }

    [Column("description")]
    public virtual string? Description { get; set; }

    [Column("run_frequency")]
    public virtual TimeSpan RunFrequency { get; set; }

    [Column("last_run_at_utc")]
    public virtual DateTimeOffset LastRunAtUtc { get; set; }

    [Column("is_active")]
    public virtual bool IsActive { get; set; }

    [Column("is_soft_deleted")]
    public virtual bool IsSoftDeleted { get; set; }

    [Column("securable_id")]
    public virtual int SecurableId { get; set; }

    public virtual Securable Securable { get; set; } = new Securable() { Type = SecurableType.Unknown };

    public Task EnsureValid(IQueryable<Script> records)
    {
        return Task.CompletedTask;
    }
}
