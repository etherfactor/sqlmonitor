using EtherGizmos.SqlMonitor.Shared.Models.Database.Enums;
using EtherGizmos.SqlMonitor.Shared.Utilities.Abstractions;
using EtherGizmos.SqlMonitor.Shared.Utilities.Annotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EtherGizmos.SqlMonitor.Shared.Models.Database;

[Table("query_variants")]
public class QueryVariant : Auditable
{
    [Column("query_variant_id")]
    public virtual int Id { get; set; }

    [Column("query_id")]
    public virtual Guid QueryId { get; set; }

    [Lookup(nameof(QueryId), nameof(Query.Id),
        List = nameof(Query.Variants))]
    public virtual Query Query { get; set; }

    [Column("sql_type_id")]
    public virtual SqlType SqlType { get; set; }

    [Column("query_text")]
    public virtual string QueryText { get; set; }

    /// <summary>
    /// Not intended for direct use.
    /// </summary>
    public QueryVariant()
    {
        Query = null!;
        QueryText = null!;
    }
}
