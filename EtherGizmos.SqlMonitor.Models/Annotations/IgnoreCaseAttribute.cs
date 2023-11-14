namespace EtherGizmos.SqlMonitor.Models.Annotations;

[AttributeUsage(AttributeTargets.Property)]
public class IgnoreCaseAttribute : Attribute
{
    public IgnoreCaseAttribute()
    {
    }
}
