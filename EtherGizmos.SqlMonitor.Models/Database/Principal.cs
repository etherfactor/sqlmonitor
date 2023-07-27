using EtherGizmos.SqlMonitor.Models.Database.Abstractions;
using EtherGizmos.SqlMonitor.Models.Database.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace EtherGizmos.SqlMonitor.Models.Database;

[Table("principals")]
public class Principal : Auditable
{
    [Column("principal_id")]
    public Guid Id { get; set; }

    [Column("principal_type_id")]
    public PrincipalType Type { get; set; }
}
