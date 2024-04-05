﻿using EtherGizmos.SqlMonitor.Models.Database.Abstractions;
using System.ComponentModel.DataAnnotations.Schema;

namespace EtherGizmos.SqlMonitor.Models.Database;

[Table("script_variants")]
public class ScriptVariant : Auditable
{
    [Column("script_variant_id")]
    public virtual int Id { get; set; }

    [Column("script_id")]
    public virtual Guid ScriptId { get; set; }

    public virtual Script Script { get; set; }

    [Column("script_interpreter_id")]
    public virtual int ScriptInterpreterId { get; set; }

    public virtual ScriptInterpreter ScriptInterpreter { get; set; }

    [Column("script_text")]
    public virtual string ScriptText { get; set; }

    [Column("timestamp_key")]
    public virtual string? TimestampKey { get; set; }

    [Column("bucket_key")]
    public virtual string? BucketKey { get; set; }

    /// <summary>
    /// Not intended for direct use.
    /// </summary>
    public ScriptVariant()
    {
        Script = null!;
        ScriptInterpreter = null!;
        ScriptText = null!;
    }

    public Task EnsureValid(IQueryable<Script> records)
    {
        return Task.CompletedTask;
    }
}