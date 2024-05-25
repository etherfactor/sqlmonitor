using EtherGizmos.SqlMonitor.Shared.Models.Database.Enums;
using EtherGizmos.SqlMonitor.Shared.Utilities.Annotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EtherGizmos.SqlMonitor.Shared.Models.Database;

[Table("securables")]
public class Securable
{
    [Column("securable_id")]
    [SqlDefaultValue]
    public virtual int Id { get; set; }

    [Column("securable_type_id")]
    public virtual SecurableType Type { get; set; }
}
