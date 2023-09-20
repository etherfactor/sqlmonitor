using System.ComponentModel.DataAnnotations.Schema;

namespace EtherGizmos.SqlMonitor.Models.Database;

[Table("instance_query_whitelists")]
public class QueryInstanceWhitelist
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
    public QueryInstanceWhitelist()
    {
        Instance = null!;
        Query = null!;
    }
}
