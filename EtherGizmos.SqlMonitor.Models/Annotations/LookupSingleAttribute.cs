namespace EtherGizmos.SqlMonitor.Models.Annotations;

[AttributeUsage(AttributeTargets.Property)]
public class LookupSingleAttribute : Attribute
{
    public string[] LookupIdProperties { get; }

    public LookupSingleAttribute(params string[] lookupIdProperties)
    {
        LookupIdProperties = lookupIdProperties;
    }
}
