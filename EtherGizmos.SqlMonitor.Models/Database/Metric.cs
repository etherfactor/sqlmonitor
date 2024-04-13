﻿using EtherGizmos.SqlMonitor.Models.Annotations;
using EtherGizmos.SqlMonitor.Models.Database.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EtherGizmos.SqlMonitor.Models.Database;

[Table("metrics")]
public class Metric
{
    [Column("metric_id")]
    [Key, SqlDefaultValue]
    public virtual int Id { get; set; }

    [Column("name")]
    public virtual string Name { get; set; }

    [Column("description")]
    public virtual string? Description { get; set; }

    [Column("aggregate_type_id")]
    public virtual AggregateType AggregateType { get; set; }

    /// <summary>
    /// Not intended for direct use.
    /// </summary>
    public Metric()
    {
        Name = null!;
    }
}
