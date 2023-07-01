using Microsoft.AspNetCore.OData.Query;
using Microsoft.OData.Edm;
using Microsoft.OData.ModelBuilder.Annotations;
using Microsoft.OData.UriParser;

namespace EtherGizmos.SqlMonitor.Api.Extensions;

internal static class ODataQueryOptionsExtensions
{
    internal static IEnumerable<string> GetExpandedProperties<TEntity>(this ODataQueryOptions<TEntity> @this)
    {
        if (@this.SelectExpand == null)
        {
            yield break;
        }

        var structuredType = @this.Context.ElementType.AsStructuredType();
        foreach (string result in GetExpandPropertiesInternal(@this.Context.Model, structuredType, @this.SelectExpand.SelectExpandClause))
        {
            yield return result;
        }
    }

    private static IEnumerable<string> GetExpandPropertiesInternal(IEdmModel model, IEdmStructuredType currentType, SelectExpandClause selectExpand)
    {
        foreach (ExpandedNavigationSelectItem item in selectExpand.SelectedItems.Where(e => typeof(ExpandedNavigationSelectItem).IsAssignableFrom(e.GetType())))
        {
            var property = currentType.FindProperty(item.NavigationSource.Name);
            string propertyName = model.GetClrPropertyName(property);

            yield return propertyName;

            foreach (string subPropertyName in GetExpandPropertiesInternal(model, item.NavigationSource.Type.AsStructuredType(), item.SelectAndExpand))
            {
                yield return $"{propertyName}.{subPropertyName}";
            }
        }

        yield break;
    }
}
