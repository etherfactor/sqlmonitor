using EtherGizmos.SqlMonitor.Api.Services.Caching;
using EtherGizmos.SqlMonitor.Models.Database.Abstractions;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EtherGizmos.SqlMonitor.Models.Database;

[Table("instances")]
public class Instance : Auditable
{
    [Column("instance_id")]
    [Key]
    public virtual Guid Id { get; set; }

    [Column("name")]
    public virtual string Name { get; set; }

    [Column("description")]
    public virtual string? Description { get; set; }

    [Column("is_active")]
    [Indexed]
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

    public virtual List<InstanceMetricByDay> MetricsByDay { get; set; } = new List<InstanceMetricByDay>();

    public virtual List<InstanceMetricByHour> MetricsByHour { get; set; } = new List<InstanceMetricByHour>();

    public virtual List<InstanceMetricByMinute> MetricsByMinute { get; set; } = new List<InstanceMetricByMinute>();

    public virtual List<InstanceMetricBySecond> MetricsBySecond { get; set; } = new List<InstanceMetricBySecond>();

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
