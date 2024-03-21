using EtherGizmos.SqlMonitor.Models.Annotations;
using EtherGizmos.SqlMonitor.Models.Database.Abstractions;
using EtherGizmos.SqlMonitor.Models.Database.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EtherGizmos.SqlMonitor.Models.Database;

[Table("script_interpreters")]
public class ScriptInterpreter : Auditable
{
    [Column("script_interpreter_id")]
    [Key, SqlDefaultValue]
    public virtual int Id { get; set; }

    [Column("name")]
    public virtual string Name { get; set; }

    [Column("description")]
    public virtual string? Description { get; set; }

    [Column("command")]
    public virtual string Command { get; set; }

    [Column("arguments")]
    public virtual string Arguments { get; set; }

    [Column("is_soft_deleted")]
    [SqlDefaultValue]
    public virtual bool IsSoftDeleted { get; set; }

    [Column("securable_id")]
    public virtual int SecurableId { get; set; }

    public virtual Securable Securable { get; set; } = new Securable() { Type = SecurableType.ScriptInterpreter };

    /// <summary>
    /// Not intended for direct use.
    /// </summary>
    public ScriptInterpreter()
    {
        Name = null!;
        Command = null!;
        Arguments = null!;
    }

    public Task EnsureValid(IQueryable<ScriptInterpreter> records)
    {
        return Task.CompletedTask;
    }
}
