namespace EtherGizmos.SqlMonitor.Api.Extensions;

internal static class TypeExtensions
{
    internal static bool IsComplexType(this Type @this)
    {
        if (@this is null)
            throw new ArgumentNullException(nameof(@this));

        if (@this == typeof(string))
            return false;

        return @this.IsClass;
    }
}
