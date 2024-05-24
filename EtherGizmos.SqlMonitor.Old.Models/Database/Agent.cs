using EtherGizmos.SqlMonitor.Models.Database.Abstractions;
using EtherGizmos.SqlMonitor.Models.Database.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace EtherGizmos.SqlMonitor.Models.Database;

[Table("agents")]
public class Agent : Auditable
{
    [Column("agent_id")]
    public virtual Guid Id { get; set; }

    [Column("name")]
    public virtual string Name { get; set; }

    [Column("description")]
    public virtual string? Description { get; set; }

    [Column("agent_type_id")]
    public virtual AgentType AgentType { get; set; }

    [Column("dedicated_host")]
    public virtual string? DedicatedHost { get; set; }

    [Column("active_count")]
    public virtual int ActiveCount { get; set; }

    [Column("oauth2_application_id")]
    public virtual int ApplicationId { get; set; }

    [Column("is_active")]
    public virtual bool IsActive { get; set; }

    [Column("is_soft_deleted")]
    public virtual bool IsSoftDeleted { get; set; }

    /// <summary>
    /// Not intended for direct use.
    /// </summary>
    public Agent()
    {
        Name = null!;
    }
}
