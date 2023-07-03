using AutoMapper;
using Microsoft.AspNetCore.OData.Query;

namespace EtherGizmos.SqlMonitor.Api.Extensions;

/// <summary>
/// Provides extension methods for <see cref="object"/>.
/// </summary>
internal static class ObjectExtensions
{
    /// <summary>
    /// Converts an object to an <see cref="IEnumerable{T}"/>.
    /// </summary>
    /// <typeparam name="T">The object type.</typeparam>
    /// <param name="this">Itself.</param>
    /// <returns>Itself, as an enumerable.</returns>
    internal static IEnumerable<T> Yield<T>(this T @this)
    {
        yield return @this;
    }

    /// <summary>
    /// Explicitly maps an object and applies OData query options.
    /// </summary>
    /// <typeparam name="TFrom">The initial type.</typeparam>
    /// <typeparam name="TTo">The final type.</typeparam>
    /// <param name="this">Itself.</param>
    /// <param name="mapper">The mapper to use.</param>
    /// <param name="queryOptions">The OData query options.</param>
    /// <returns>The mapped object. (Note: cannot be cast to <typeparamref name="TTo"/> if $select/$expand are used.)</returns>
    internal static object MapExplicitlyAndApplyQueryOptions<TFrom, TTo>(this TFrom @this, IMapper mapper, ODataQueryOptions<TTo> queryOptions)
        where TFrom : class
        where TTo : class
    {
        var membersToExpand = queryOptions.GetExpandedProperties().ToArray();

        var mapped = mapper.MapExplicitly(@this).To<TTo>(membersToExpand);
        object finished = queryOptions.ApplyTo(mapped, new ODataQuerySettings());

        return finished;
    }
}
