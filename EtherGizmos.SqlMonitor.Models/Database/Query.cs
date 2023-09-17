using EtherGizmos.SqlMonitor.Api.Services.Caching;
using EtherGizmos.SqlMonitor.Models.Database.Abstractions;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EtherGizmos.SqlMonitor.Models.Database;

[Table("queries")]
public class Query : Auditable
{
    [Column("query_id")]
    [Key]
    public virtual Guid Id { get; set; }

    [Column("system_id")]
    public virtual Guid SystemId { get; set; }

    [Column("name")]
    public virtual string Name { get; set; }

    [Column("description")]
    public virtual string? Description { get; set; }

    [Column("is_active")]
    [Indexed]
    public virtual bool IsActive { get; set; } = true;

    [Column("is_soft_deleted")]
    public virtual bool IsSoftDeleted { get; set; } = false;

    [Column("sql_text")]
    public virtual string SqlText { get; set; }

    [Column("run_frequency")]
    public virtual TimeSpan RunFrequency { get; set; }

    [Column("last_run_at_utc")]
    public virtual DateTimeOffset? LastRunAtUtc { get; set; }

    [Column("timestamp_utc_expression")]
    public virtual string? TimestampUtcExpression { get; set; }

    [Column("bucket_expression")]
    public virtual string? BucketExpression { get; set; }

    [Column("next_run_at_utc")]
    [Indexed]
    public DateTimeOffset NextRunAtUtc => (LastRunAtUtc ?? DateTimeOffset.MinValue).Add(RunFrequency);

    public virtual List<QueryMetric> Metrics { get; set; } = new List<QueryMetric>();

    /// <summary>
    /// Not intended for direct use.
    /// </summary>
    public Query()
    {
        Name = null!;
        SqlText = null!;
    }

    public Task EnsureValid(IQueryable<Query> records)
    {
        return Task.CompletedTask;
    }
}
