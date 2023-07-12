using EtherGizmos.SqlMonitor.Api.OData.Errors;
using EtherGizmos.SqlMonitor.Api.Services.Middleware;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.OData.Edm;
using Microsoft.OData.ModelBuilder.Annotations;
using Microsoft.OData.UriParser;

namespace EtherGizmos.SqlMonitor.Api.Extensions;

/// <summary>
/// Provides extension methods for <see cref="ODataQueryOptions{TEntity}"/>.
/// </summary>
internal static class ODataQueryOptionsExtensions
{
    internal static void EnsureValidForSingle<T>(this ODataQueryOptions<T> @this)
    {
        if (@this == null)
            throw new ArgumentNullException(nameof(@this));

        if (@this.Filter != null)
            throw new ReturnODataErrorException(new ODataParameterDisallowedOnSingleError("$filter"));

        if (@this.OrderBy != null)
            throw new ReturnODataErrorException(new ODataParameterDisallowedOnSingleError("$orderby"));

        if (@this.Count != null)
            throw new ReturnODataErrorException(new ODataParameterDisallowedOnSingleError("$count"));

        if (@this.Skip != null)
            throw new ReturnODataErrorException(new ODataParameterDisallowedOnSingleError("$skip"));

        if (@this.Top != null)
            throw new ReturnODataErrorException(new ODataParameterDisallowedOnSingleError("$top"));
    }

    /// <summary>
    /// Gets a set of properties requested to be expanded.
    /// </summary>
    /// <typeparam name="TEntity">The type of entity.</typeparam>
    /// <param name="this">Itself.</param>
    /// <returns>A set of properties to be expanded.</returns>
    internal static IEnumerable<string> GetExpandedProperties<TEntity>(this ODataQueryOptions<TEntity> @this)
    {
        //Don't return anything if $select/$expand were unspecified
        if (@this.SelectExpand == null)
        {
            yield break;
        }

        //Get the current type as a structured type
        //Needed to get properties
        var structuredType = @this.Context.ElementType.AsStructuredType();

        //Iterate over expanded properties and return them
        foreach (string result in GetExpandPropertiesInternal(@this.Context.Model, structuredType, @this.SelectExpand.SelectExpandClause))
        {
            yield return result;
        }
    }

    private static IEnumerable<string> GetExpandPropertiesInternal(IEdmModel model, IEdmStructuredType currentType, SelectExpandClause selectExpand)
    {
        //Iterate over expanded properties
        foreach (ExpandedNavigationSelectItem item in selectExpand.SelectedItems.Where(e => typeof(ExpandedNavigationSelectItem).IsAssignableFrom(e.GetType())))
        {
            //Find the property on the current type and get the CLR name
            var property = currentType.FindProperty(item.NavigationSource.Name);
            string propertyName = model.GetClrPropertyName(property);

            //Return that property name
            yield return propertyName;

            //Then iterate over all expansions in that property
            foreach (string subPropertyName in GetExpandPropertiesInternal(model, item.NavigationSource.Type.AsStructuredType(), item.SelectAndExpand))
            {
                //Return the current property and all sub-expansions
                yield return $"{propertyName}.{subPropertyName}";
            }
        }
    }
}
