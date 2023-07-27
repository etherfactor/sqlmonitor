using EtherGizmos.SqlMonitor.Models.Database.Abstractions;
using System.ComponentModel.DataAnnotations.Schema;

namespace EtherGizmos.SqlMonitor.Models.Database;

[Table("instances")]
public class Instance : Auditable
{
    [Column("instance_id")]
    public Guid Id { get; set; }

    [Column("name")]
    public string Name { get; set; }

    [Column("description")]
    public string? Description { get; set; }

    [Column("address")]
    public string Address { get; set; }

    [Column("port")]
    public short? Port { get; set; }

    [Column("database")]
    public string? Database { get; set; }

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
