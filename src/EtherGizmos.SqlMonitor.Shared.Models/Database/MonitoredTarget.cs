using EtherGizmos.SqlMonitor.Shared.Models.Annotations;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EtherGizmos.SqlMonitor.Shared.Models.Database;

[Table("monitored_targets")]
public class MonitoredTarget
{
    [Column("monitored_target_id")]
    [Key, SqlDefaultValue]
    public virtual int Id { get; set; }

    [Column("monitored_system_id")]
    public virtual Guid MonitoredSystemId { get; set; }

    public virtual MonitoredSystem MonitoredSystem { get; set; }

    [Column("monitored_resource_id")]
    public virtual Guid MonitoredResourceId { get; set; }

    public virtual MonitoredResource MonitoredResource { get; set; }

    [Column("monitored_environment_id")]
    public virtual Guid MonitoredEnvironmentId { get; set; }

    public virtual MonitoredEnvironment MonitoredEnvironment { get; set; }

    /// <summary>
    /// Not intended for direct use.
    /// </summary>
    public MonitoredTarget()
    {
        MonitoredSystem = null!;
        MonitoredResource = null!;
        MonitoredEnvironment = null!;
    }

    public Task EnsureValid(IQueryable<MonitoredTarget> records)
    {
        return Task.CompletedTask;
    }
}
