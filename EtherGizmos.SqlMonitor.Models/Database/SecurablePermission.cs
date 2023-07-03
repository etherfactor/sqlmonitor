using EtherGizmos.SqlMonitor.Models.Database.Abstractions;
using System.ComponentModel.DataAnnotations.Schema;

namespace EtherGizmos.SqlMonitor.Models.Database;

[Table("securable_permissions")]
public class SecurablePermission : Auditable
{
    [Column("securable_id")]
    public virtual string SecurableId { get; set; }

    [Column("permission_id")]
    public virtual string PermissionId { get; set; }

    /// <summary>
    /// Not intended for direct use.
    /// </summary>
    public SecurablePermission()
    {
        SecurableId = null!;
        PermissionId = null!;
    }
}
