﻿using EtherGizmos.SqlMonitor.Shared.Utilities.Abstractions;
using EtherGizmos.SqlMonitor.Shared.Utilities.Annotations;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EtherGizmos.SqlMonitor.Shared.Models.Database;

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

    [Column("extension")]
    public virtual string Extension { get; set; }

    [Column("is_soft_deleted")]
    [SqlDefaultValue]
    public virtual bool IsSoftDeleted { get; set; }

    [Column("securable_id")]
    public virtual int SecurableId { get; set; }

    public virtual Securable Securable { get; set; }

    /// <summary>
    /// Not intended for direct use.
    /// </summary>
    public ScriptInterpreter()
    {
        Name = null!;
        Command = null!;
        Arguments = null!;
        Extension = null!;
        Securable = null!;
    }
}
