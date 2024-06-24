using EtherGizmos.SqlMonitor.Shared.Utilities.Abstractions;
using System.ComponentModel.DataAnnotations.Schema;

namespace EtherGizmos.SqlMonitor.Shared.Models.Database;

[Table("metric_buckets")]
public class MetricBucket : Auditable
{
    [Column("metric_bucket_id")]
    public virtual int Id { get; set; }

    [Column("name")]
    public virtual string Name { get; set; }

    [Column("description")]
    public virtual string? Description { get; set; }

    /// <summary>
    /// Not intended for direct use.
    /// </summary>
    public MetricBucket()
    {
        Name = null!;
    }
}
