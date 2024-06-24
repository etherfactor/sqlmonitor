using EtherGizmos.SqlMonitor.Shared.OData.Errors;
using EtherGizmos.SqlMonitor.Shared.OData.Exceptions;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.OData.Edm;
using Microsoft.OData.ModelBuilder.Annotations;
using Microsoft.OData.UriParser;

namespace EtherGizmos.SqlMonitor.Shared.OData.Extensions;

/// <summary>
/// Provides extension methods for <see cref="ODataQueryOptions{TEntity}"/>.
/// </summary>
public static class ODataQueryOptionsExtensions
{
    /// <summary>
    /// Ensures that the query options object can be applied to a single entity. Throws exceptions for any parameters that
    /// are not applicable. The exceptions take the form of <see cref="ReturnODataErrorException"/>, which should be caught
    /// by ReturnODataErrorFilter.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="this"></param>
    /// <exception cref="ArgumentNullException"></exception>
    /// <exception cref="ReturnODataErrorException"></exception>
    public static void EnsureValidForSingle<T>(this ODataQueryOptions<T> @this)
    {
        if (@this == null)
            throw new ArgumentNullException(nameof(@this));

        if (@this.Filter != null)
            throw new ReturnODataErrorException(new ODataParameterNotApplicableOnSingleError("$filter"));

        if (@this.OrderBy != null)
            throw new ReturnODataErrorException(new ODataParameterNotApplicableOnSingleError("$orderby"));

        if (@this.Top != null)
            throw new ReturnODataErrorException(new ODataParameterNotApplicableOnSingleError("$top"));

        if (@this.Skip != null)
            throw new ReturnODataErrorException(new ODataParameterNotApplicableOnSingleError("$skip"));

        if (@this.Count != null)
            throw new ReturnODataErrorException(new ODataParameterNotApplicableOnSingleError("$count"));
    }

    /// <summary>
    /// Gets a set of properties requested to be expanded.
    /// </summary>
    /// <typeparam name="TEntity">The type of entity.</typeparam>
    /// <param name="this">Itself.</param>
    /// <returns>A set of properties to be expanded.</returns>
    public static IEnumerable<string> GetExpandedProperties<TEntity>(this ODataQueryOptions<TEntity> @this)
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
            //If that property is contained in a complex object, follow the navigation path first
            List<string> propertyNames = new List<string>();
            foreach (var navigationPath in item.PathToNavigationProperty)
            {
                var currentProperty = currentType.FindProperty(navigationPath.Identifier);
                currentType = currentProperty.Type.ToStructuredType();

                var currentPropertyName = model.GetClrPropertyName(currentProperty);
                propertyNames.Add(currentPropertyName);
            }

            var propertyName = string.Join(".", propertyNames);

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
