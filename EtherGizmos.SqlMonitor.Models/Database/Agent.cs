using EtherGizmos.SqlMonitor.Models.Database.Abstractions;
using EtherGizmos.SqlMonitor.Models.Database.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace EtherGizmos.SqlMonitor.Models.Database;

[Table("agents")]
public class Agent : Auditable
{
    [Column("agent_id")]
    public Guid Id { get; set; }

    [Column("agent_type_id")]
    public AgentType Type { get; set; }

    [Column("name")]
    public string Name { get; set; }

    [Column("description")]
    public string? Description { get; set; }

    [Column("target_host")]
    public string? TargetHost { get; set; }

    [Column("active_count")]
    public int ActiveCount { get; set; }

    [Column("max_active_count")]
    public int MaxActiveCount { get; set; }

    [Column("is_active")]
    public bool IsActive { get; set; }

    [Column("is_soft_deleted")]
    public bool IsSoftDeleted { get; set; }

    [Column("oauth2_application_id")]
    public int ApplicationId { get; set; }

    /// <summary>
    /// Not intended for direct use.
    /// </summary>
    public Agent()
    {
        Name = null!;
    }
}
