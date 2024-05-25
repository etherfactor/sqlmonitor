namespace EtherGizmos.SqlMonitor.Shared.Models.Annotations;

[AttributeUsage(AttributeTargets.Enum, AllowMultiple = false)]
public class EnumDisplayAttribute : Attribute
{
    public string? Name { get; set; }
}
