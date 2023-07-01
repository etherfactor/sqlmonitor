using Microsoft.OData.Edm;

namespace EtherGizmos.SqlMonitor.Api.Extensions;

internal static class IEdmTypeExtensions
{
    internal static IEdmStructuredType AsStructuredType(this IEdmType @this)
    {
        var useType = @this.AsElementType();
        return (IEdmStructuredType)useType;
    }
}
