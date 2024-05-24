using EtherGizmos.SqlMonitor.Models.Annotations;
using EtherGizmos.SqlMonitor.Models.Database.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace EtherGizmos.SqlMonitor.Models.Database;

[Table("securables")]
public class Securable
{
    [Column("securable_id")]
    [SqlDefaultValue]
    public virtual int Id { get; set; }

    [Column("securable_type_id")]
    public virtual SecurableType Type { get; set; }
}
