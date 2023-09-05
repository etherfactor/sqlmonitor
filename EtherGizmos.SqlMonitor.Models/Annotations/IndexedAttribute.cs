namespace EtherGizmos.SqlMonitor.Api.Services.Caching;

[AttributeUsage(AttributeTargets.Property)]
public class IndexedAttribute : Attribute
{
    public string Name { get; }

    public IndexedAttribute(string name)
    {
        Name = name;
    }
}
