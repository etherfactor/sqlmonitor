using AutoMapper;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.EntityFrameworkCore;

namespace EtherGizmos.SqlMonitor.Api.Extensions;

internal static class IQueryableExtensions
{
    internal static async Task<IQueryable> MapExplicitlyAndApplyQueryOptions<TFrom, TTo>(this IQueryable<TFrom> @this, IMapper mapper, ODataQueryOptions<TTo> queryOptions)
        where TFrom : class
        where TTo : class
    {
        string[] expanded = queryOptions.GetExpandedProperties().ToArray();
        IQueryable<TTo> queryable = mapper.ProjectTo<TTo>(@this, null, expanded);

        return await queryable.ApplyQueryOptions(queryOptions);
    }

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
