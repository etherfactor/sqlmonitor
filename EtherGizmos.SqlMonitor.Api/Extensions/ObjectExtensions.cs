using AutoMapper;
using Microsoft.AspNetCore.OData.Query;

namespace EtherGizmos.SqlMonitor.Api.Extensions;

internal static class ObjectExtensions
{
    internal static IEnumerable<T> Yield<T>(this T @this)
    {
        yield return @this;
    }

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
