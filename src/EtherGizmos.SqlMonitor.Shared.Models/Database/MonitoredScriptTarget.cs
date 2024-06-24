using EtherGizmos.SqlMonitor.Shared.Models.Database.Enums;
using EtherGizmos.SqlMonitor.Shared.Utilities.Abstractions;
using EtherGizmos.SqlMonitor.Shared.Utilities.Annotations;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EtherGizmos.SqlMonitor.Shared.Models.Database;

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
    [Indexed]
    public virtual int ScriptInterpreterId { get; set; }

    public virtual ScriptInterpreter ScriptInterpreter { get; set; }

    [Column("exec_type_id")]
    public virtual ExecType ExecType { get; set; }

    [Column("host")]
    public virtual string HostName { get; set; }

    [Column("port")]
    public virtual int? Port { get; set; }

    [Column("run_in_path")]
    public virtual string RunInPath { get; set; }

    [Column("ssh_authentication_type_id")]
    public virtual SshAuthenticationType? SshAuthenticationType { get; set; }

    [Column("ssh_username")]
    public virtual string? SshUsername { get; set; }

    [Column("ssh_password")]
    public virtual string? SshPassword { get; set; }

    [Column("ssh_private_key")]
    public virtual string? SshPrivateKey { get; set; }

    [Column("ssh_private_key_password")]
    public virtual string? SshPrivateKeyPassword { get; set; }

    [Column("winrm_authentication_type_id")]
    public virtual WinRmAuthenticationType? WinRmAuthenticationType { get; set; }

    [Column("winrm_use_ssl")]
    public virtual bool? WinRmUseSsl { get; set; }

    [Column("winrm_username")]
    public virtual string? WinRmUsername { get; set; }

    [Column("winrm_password")]
    public virtual string? WinRmPassword { get; set; }

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
        RunInPath = null!;
        SshUsername = null!;
        Securable = null!;
    }
}
