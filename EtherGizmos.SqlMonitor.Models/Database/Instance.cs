using EtherGizmos.SqlMonitor.Models.Database.Abstractions;
using System.ComponentModel.DataAnnotations.Schema;

namespace EtherGizmos.SqlMonitor.Models.Database;

[Table("instances")]
public class Instance : Auditable
{
    [Column("instance_id")]
    public virtual Guid Id { get; set; }

    [Column("name")]
    public virtual string Name { get; set; }

    [Column("description")]
    public virtual string? Description { get; set; }

    [Column("address")]
    public virtual string Address { get; set; }

    [Column("port")]
    public virtual short? Port { get; set; }

    [Column("database")]
    public virtual string? Database { get; set; }

    public virtual List<InstanceQueryBlacklist> QueryBlacklist { get; set; } = new List<InstanceQueryBlacklist>();

    public virtual List<InstanceQueryWhitelist> QueryWhitelist { get; set; } = new List<InstanceQueryWhitelist>();

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
