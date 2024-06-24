using AutoMapper;
using EtherGizmos.SqlMonitor.Shared.Models.Extensions;
using Microsoft.AspNetCore.OData.Query;

namespace EtherGizmos.SqlMonitor.Shared.OData.Extensions;

/// <summary>
/// Provides extension methods for <see cref="object"/>.
/// </summary>
public static class ObjectExtensions
{
    /// <summary>
    /// Explicitly maps an object and applies OData query options.
    /// </summary>
    /// <typeparam name="TFrom">The initial type.</typeparam>
    /// <typeparam name="TTo">The final type.</typeparam>
    /// <param name="this">Itself.</param>
    /// <param name="mapper">The mapper to use.</param>
    /// <param name="queryOptions">The OData query options.</param>
    /// <returns>The mapped object. (Note: cannot be cast to <typeparamref name="TTo"/> if $select/$expand are used.)</returns>
    public static object MapExplicitlyAndApplyQueryOptions<TFrom, TTo>(this TFrom @this, IMapper mapper, ODataQueryOptions<TTo> queryOptions)
        where TFrom : class
        where TTo : class
    {
        var membersToExpand = queryOptions.GetExpandedProperties().ToArray();

        var mapped = mapper.MapExplicitly(@this).To<TTo>(membersToExpand);
        object finished = queryOptions.ApplyTo(mapped, new ODataQuerySettings());

        return finished;
    }
}
