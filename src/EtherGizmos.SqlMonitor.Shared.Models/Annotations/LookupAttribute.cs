namespace EtherGizmos.SqlMonitor.Shared.Models.Annotations;

[AttributeUsage(AttributeTargets.Property)]
public class LookupAttribute : Attribute
{
    public IEnumerable<(string ForeignKey, string PrimaryKey)> IdProperties { get; }

    public string? List { get; set; }

    public string? Record { get; set; }

    public LookupAttribute(params string[] lookupIdProperties)
    {
        IdProperties = lookupIdProperties.Chunk(2)
            .Select(e => (e[0], e[1]));
    }
}
