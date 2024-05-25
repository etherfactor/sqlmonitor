namespace EtherGizmos.SqlMonitor.Shared.Utilities.Extensions;

public static class TypeExtensions
{
    public static bool IsComplexType(this Type @this)
    {
        if (@this is null)
            throw new ArgumentNullException(nameof(@this));

        if (@this == typeof(string))
            return false;

        return @this.IsClass;
    }
}
