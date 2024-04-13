using EtherGizmos.SqlMonitor.Models.Annotations;
using EtherGizmos.SqlMonitor.Models.Database.Abstractions;
using EtherGizmos.SqlMonitor.Models.Database.Enums;
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

    [Column("port")]
    public virtual int? Port { get; set; }

    [Column("file_path")]
    public virtual string FilePath { get; set; }

    [Column("use_ssl")]
    public virtual bool UseSsl { get; set; }

    [Column("ssh_authentication_type_id")]
    public virtual SshAuthenticationType AuthenticationType { get; set; }

    [Column("username")]
    public virtual string Username { get; set; }

    [Column("password")]
    public virtual string? Password { get; set; }

    [Column("private_key")]
    public virtual string? PrivateKey { get; set; }

    [Column("private_key_password")]
    public virtual string? PrivateKeyPassword { get; set; }

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
        Username = null!;
        Securable = null!;
    }

    public Task EnsureValid(IQueryable<MonitoredScriptTarget> records)
    {
        return Task.CompletedTask;
    }
}
