using EtherGizmos.SqlMonitor.Models.Annotations;
using EtherGizmos.SqlMonitor.Models.Database.Abstractions;
using System.ComponentModel.DataAnnotations.Schema;

namespace EtherGizmos.SqlMonitor.Models.Database;

[Table("metric_buckets")]
public class MetricBucket : Auditable
{
    [Column("metric_bucket_id")]
    [SqlDefaultValue]
    public int Id { get; set; }

    [Column("name")]
    public string Name { get; set; }

    /// <summary>
    /// Not intended for direct use.
    /// </summary>
    public MetricBucket()
    {
        Name = null!;
    }
}
