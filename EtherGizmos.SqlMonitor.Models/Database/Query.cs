using EtherGizmos.SqlMonitor.Models.Database.Abstractions;
using System.ComponentModel.DataAnnotations.Schema;

namespace EtherGizmos.SqlMonitor.Models.Database;

[Table("queries")]
public class Query : Auditable
{
    [Column("query_id")]
    public Guid Id { get; set; }

    [Column("system_id")]
    public Guid SystemId { get; set; }

    [Column("name")]
    public string Name { get; set; }

    [Column("description")]
    public string? Description { get; set; }

    [Column("is_active")]
    public bool IsActive { get; set; }

    [Column("is_soft_deleted")]
    public bool IsSoftDeleted { get; set; }

    [Column("sql_text")]
    public string SqlText { get; set; }

    [Column("run_frequency")]
    public TimeSpan RunFrequency { get; set; }

    [Column("last_run_at_utc")]
    public DateTimeOffset? LastRunAtUtc { get; set; }

    [Column("timestamp_utc_expression")]
    public string? TimestampUtcExpression { get; set; }

    [Column("bucket_expression")]
    public string? BucketExpression { get; set; }

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
