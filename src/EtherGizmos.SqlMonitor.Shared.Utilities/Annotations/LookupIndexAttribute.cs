namespace EtherGizmos.SqlMonitor.Shared.Utilities.Annotations;

[AttributeUsage(AttributeTargets.Property)]
public class LookupIndexAttribute : Attribute
{
    public string Name { get; }

    public LookupIndexAttribute(string name)
    {
        Name = name;
    }
}
