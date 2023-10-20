namespace EtherGizmos.SqlMonitor.Models.Annotations;

[AttributeUsage(AttributeTargets.Property)]
public class LookupFromAttribute : Attribute
{
    public string[] Properties { get; }

    public LookupFromAttribute(params string[] properties)
    {
        Properties = properties;
    }
}
