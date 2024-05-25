using EtherGizmos.SqlMonitor.Shared.Models.Database.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EtherGizmos.SqlMonitor.Shared.Models.Database;

[Table("monitored_query_targets")]
public class MonitoredQueryTarget
{
    [Column("monitored_query_target_id")]
    [Key, SqlDefaultValue]
    public virtual int Id { get; set; }

    [Column("monitored_target_id")]
    public virtual int MonitoredTargetId { get; set; }

    public virtual MonitoredTarget MonitoredTarget { get; set; }

    [Column("sql_type_id")]
    public virtual SqlType SqlType { get; set; }

    [Column("host")]
    public virtual string HostName { get; set; }

    [Column("connection_string")]
    public virtual string ConnectionString { get; set; }

    [Column("securable_id")]
    public virtual int SecurableId { get; set; }

    public virtual Securable Securable { get; set; }

    public MonitoredQueryTarget()
    {
        MonitoredTarget = null!;
        HostName = null!;
        ConnectionString = null!;
        Securable = null!;
    }
}
