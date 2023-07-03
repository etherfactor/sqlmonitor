using EtherGizmos.SqlMonitor.Models.Database.Abstractions;
using System.ComponentModel.DataAnnotations.Schema;

namespace EtherGizmos.SqlMonitor.Models.Database;

[Table("securables")]
public class Securable : Auditable
{
    [Column("securable_id")]
    public virtual string Id { get; set; }

    [Column("name")]
    public virtual string Name { get; set; }

    [Column("description")]
    public virtual string? Description { get; set; }

    /// <summary>
    /// Not intended for direct use.
    /// </summary>
    public Securable()
    {
        Id = null!;
        Name = null!;
    }
}
