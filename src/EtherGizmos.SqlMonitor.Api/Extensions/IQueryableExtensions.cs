using AutoMapper;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.EntityFrameworkCore;

namespace EtherGizmos.SqlMonitor.Api.Extensions;

/// <summary>
/// Provides extension methods for <see cref="IQueryable{T}"/>.
/// </summary>
internal static class IQueryableExtensions
{
    /// <summary>
    /// Explicitly maps a queryable and applies OData query options.
    /// </summary>
    /// <typeparam name="TFrom">The initial type.</typeparam>
    /// <typeparam name="TTo">The final type.</typeparam>
    /// <param name="this">Itself.</param>
    /// <param name="mapper">The mapper to use.</param>
    /// <param name="queryOptions">The OData query options.</param>
    /// <returns>The mapped queryable. (Note: cannot be cast to <see cref="IQueryable{TFrom}"/> if $select/$expand are used.)</returns>
    internal static async Task<IQueryable> MapExplicitlyAndApplyQueryOptions<TFrom, TTo>(this IQueryable<TFrom> @this, IMapper mapper, ODataQueryOptions<TTo> queryOptions)
        where TFrom : class
        where TTo : class
    {
        string[] expanded = queryOptions.GetExpandedProperties().ToArray();
        IQueryable<TTo> queryable = mapper.ProjectTo<TTo>(@this, null, expanded);

        return await queryable.ApplyQueryOptions(queryOptions);
    }

    /// <summary>
    /// Applies OData query options.
    /// </summary>
    /// <typeparam name="TEntity">The initial type.</typeparam>
    /// <param name="this">Itself.</param>
    /// <param name="queryOptions">The OData query options.</param>
    /// <returns>The result queryable. (Note: cannot be cast to <see cref="IQueryable{TFrom}"/> if $select/$expand are used.)</returns>
    internal static async Task<IQueryable> ApplyQueryOptions<TEntity>(this IQueryable<TEntity> @this, ODataQueryOptions<TEntity> queryOptions)
        where TEntity : class
    {
        AllowedQueryOptions noSelectExpand = AllowedQueryOptions.Select | AllowedQueryOptions.Expand;
        AllowedQueryOptions onlySelectExpand = AllowedQueryOptions.All & ~noSelectExpand;

        IQueryable<TEntity> noSelectExpandQueryable = (IQueryable<TEntity>)queryOptions.ApplyTo(@this, noSelectExpand);

        List<TEntity> noSelectExpandList = await noSelectExpandQueryable.ToListAsync();
        IQueryable finished = queryOptions.ApplyTo(noSelectExpandList.AsQueryable(), onlySelectExpand);

        return finished;
    }
}
