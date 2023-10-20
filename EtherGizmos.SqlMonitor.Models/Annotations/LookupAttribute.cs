namespace EtherGizmos.SqlMonitor.Models.Annotations;

[AttributeUsage(AttributeTargets.Property)]
public class LookupAttribute : Attribute
{
    public string[] IdProperties { get; }

    public string? List { get; set; }

    public string? Record { get; set; }

    public LookupAttribute(params string[] lookupIdProperties)
    {
        IdProperties = lookupIdProperties;
    }
}
