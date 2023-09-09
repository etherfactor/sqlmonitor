using StackExchange.Redis;
using System.Reflection;

namespace EtherGizmos.SqlMonitor.Api.Services.Caching.Abstractions;

public interface ICacheEntitySetFilter<TEntity>
{
    /// <summary>
    /// Get the end score of the filter.
    /// </summary>
    /// <returns>The end score.</returns>
    RedisValue GetEndScore();

    /// <summary>
    /// Gets which of the filter's scores are inclusive vs. exclusive.
    /// </summary>
    /// <returns></returns>
    Exclude GetExclusivity();

    /// <summary>
    /// Get the indexed property of the filter.
    /// </summary>
    /// <returns>The indexed property.</returns>
    PropertyInfo GetProperty();

    /// <summary>
    /// Get the start score of the filter.
    /// </summary>
    /// <returns>The start score.</returns>
    RedisValue GetStartScore();
}
