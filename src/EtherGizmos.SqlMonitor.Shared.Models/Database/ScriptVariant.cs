using EtherGizmos.SqlMonitor.Shared.Utilities.Abstractions;
using EtherGizmos.SqlMonitor.Shared.Utilities.Annotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EtherGizmos.SqlMonitor.Shared.Models.Database;

[Table("script_variants")]
public class ScriptVariant : Auditable
{
    [Column("script_variant_id")]
    public virtual int Id { get; set; }

    [Column("script_id")]
    public virtual Guid ScriptId { get; set; }

    [Lookup(nameof(ScriptId), nameof(Script.Id),
        List = nameof(Script.Variants))]
    public virtual Script Script { get; set; }

    [Column("script_interpreter_id")]
    public virtual int ScriptInterpreterId { get; set; }

    [Lookup(nameof(ScriptInterpreterId), nameof(ScriptInterpreter.Id))]
    public virtual ScriptInterpreter ScriptInterpreter { get; set; }

    [Column("script_text")]
    public virtual string ScriptText { get; set; }

    /// <summary>
    /// Not intended for direct use.
    /// </summary>
    public ScriptVariant()
    {
        Script = null!;
        ScriptInterpreter = null!;
        ScriptText = null!;
    }
}
