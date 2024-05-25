using EtherGizmos.SqlMonitor.Shared.Models.Database.Abstractions;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EtherGizmos.SqlMonitor.Shared.Models.Database;

[Table("script_metrics")]
public class ScriptMetric : Auditable
{
    [Column("script_metric_id")]
    [Key, SqlDefaultValue]
    public virtual int Id { get; set; }

    [Column("script_id")]
    public virtual Guid ScriptId { get; set; }

    public virtual Script Script { get; set; }

    [Column("metric_id")]
    public virtual int MetricId { get; set; }

    public virtual Metric Metric { get; set; }

    [Column("value_key")]
    public virtual string ValueKey { get; set; }

    [Column("is_active")]
    [SqlDefaultValue]
    public virtual bool IsActive { get; set; }

    /// <summary>
    /// Not intended for direct use.
    /// </summary>
    public ScriptMetric()
    {
        Script = null!;
        Metric = null!;
        ValueKey = null!;
    }
}
