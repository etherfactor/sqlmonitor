using Microsoft.OData.Edm;

namespace EtherGizmos.SqlMonitor.Shared.OData.Extensions;

/// <summary>
/// Provides extension methods for <see cref="IEdmType"/>.
/// </summary>
internal static class IEdmTypeExtensions
{
    /// <summary>
    /// Casts a type to an <see cref="IEdmStructuredType"/>, unwrapping a collection if needed.
    /// <para></para>
    /// Do not use on non-complex/navigation types/properties!
    /// </summary>
    /// <param name="this">Itself.</param>
    /// <returns>Itself, as a structured type.</returns>
    internal static IEdmStructuredType AsStructuredType(this IEdmType @this)
    {
        var useType = @this.AsElementType();
        return (IEdmStructuredType)useType;
    }
}
