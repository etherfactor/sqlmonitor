using Microsoft.OData.Edm;
using Microsoft.OData.ModelBuilder.Annotations;

namespace EtherGizmos.SqlMonitor.Api.Extensions;

internal static class IEdmNavigationSourceExtensions
{
    internal static string GetClrPropertyNameForModel(this IEdmNavigationSource @this, IEdmModel model)
    {
        var navigationType = @this.Type;
        if (@this.Type.TypeKind == EdmTypeKind.Collection)
        {
            IEdmCollectionType collectionType = (IEdmCollectionType)@this.Type;
            navigationType = collectionType.ElementType.Definition;
        }

        var structuredType = (IEdmStructuredType)navigationType;
        var property = structuredType.FindProperty(@this.Name);

        return model.GetClrPropertyName(property);
    }
}
