using EtherGizmos.SqlMonitor.Models.Annotations;
using EtherGizmos.SqlMonitor.Models.Database.Abstractions;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EtherGizmos.SqlMonitor.Models.Database;

[Table("monitored_script_targets")]
public class MonitoredScriptTarget : Auditable
{
    [Column("monitored_script_target_id")]
    [Key, SqlDefaultValue]
    public virtual int Id { get; set; }

    [Column("monitored_target_id")]
    public virtual int MonitoredTargetId { get; set; }

    public virtual MonitoredTarget MonitoredTarget { get; set; }

    [Column("script_interpreter_id")]
    public virtual int ScriptInterpreterId { get; set; }

    public virtual ScriptInterpreter ScriptInterpreter { get; set; }

    [Column("host_name")]
    public virtual string HostName { get; set; }

    [Column("file_path")]
    public virtual string FilePath { get; set; }

    [Column("securable_id")]
    public virtual int SecurableId { get; set; }

    public virtual Securable Securable { get; set; }

    /// <summary>
    /// Not intended for direct use.
    /// </summary>
    public MonitoredScriptTarget()
    {
        MonitoredTarget = null!;
        ScriptInterpreter = null!;
        HostName = null!;
        FilePath = null!;
        Securable = null!;
    }

    public Task EnsureValid(IQueryable<MonitoredScriptTarget> records)
    {
        return Task.CompletedTask;
    }
}
