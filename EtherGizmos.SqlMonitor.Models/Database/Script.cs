using EtherGizmos.SqlMonitor.Models.Annotations;
using EtherGizmos.SqlMonitor.Models.Database.Abstractions;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EtherGizmos.SqlMonitor.Models.Database;

[Table("scripts")]
public class Script : Auditable
{
    [Column("script_id")]
    [Key, SqlDefaultValue]
    public virtual Guid Id { get; set; }

    [Column("name")]
    public virtual string Name { get; set; }

    [Column("description")]
    public virtual string? Description { get; set; }

    [Column("run_frequency")]
    public virtual TimeSpan RunFrequency { get; set; }

    [Column("last_run_at_utc")]
    public virtual DateTimeOffset LastRunAtUtc { get; set; }

    [Column("is_active")]
    [SqlDefaultValue]
    public virtual bool IsActive { get; set; }

    [Column("is_soft_deleted")]
    [SqlDefaultValue]
    public virtual bool IsSoftDeleted { get; set; }

    [Column("bucket_key")]
    public virtual string? BucketKey { get; set; }

    [Column("timestamp_utc_key")]
    public virtual string? TimestampUtcKey { get; set; }

    [Column("securable_id")]
    public virtual int SecurableId { get; set; }

    public virtual Securable Securable { get; set; }

    public virtual List<ScriptVariant> Variants { get; set; } = [];

    //public virtual List<ScriptMetric> Metrics { get; set; } = [];

    /// <summary>
    /// Not intended for direct use.
    /// </summary>
    public Script()
    {
        Name = null!;
        Securable = null!;
    }

    public Task EnsureValid(IQueryable<Script> records)
    {
        return Task.CompletedTask;
    }
}
