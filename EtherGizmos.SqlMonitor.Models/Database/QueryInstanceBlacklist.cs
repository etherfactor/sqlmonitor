using System.ComponentModel.DataAnnotations.Schema;

namespace EtherGizmos.SqlMonitor.Models.Database;

[Table("instance_query_blacklists")]
public class QueryInstanceBlacklist
{
    [Column("instance_id")]
    public virtual Guid InstanceId { get; set; }

    public virtual Instance Instance { get; set; }

    [Column("query_id")]
    public virtual Guid QueryId { get; set; }

    public virtual Query Query { get; set; }

    /// <summary>
    /// Not intended for direct use.
    /// </summary>
    public QueryInstanceBlacklist()
    {
        Instance = null!;
        Query = null!;
    }
}
