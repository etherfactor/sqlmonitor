using EtherGizmos.SqlMonitor.Api.Services.Caching.Abstractions;
using System.Collections.Concurrent;

namespace EtherGizmos.SqlMonitor.Api.Services.Caching;

/// <summary>
/// Caches <see cref="IRedisHelper{TEntity}"/> instances.
/// </summary>
public class RedisHelperFactory : IRedisHelperFactory
{
    private static readonly ConcurrentDictionary<Type, IRedisHelper> _helpers = new ConcurrentDictionary<Type, IRedisHelper>();

    internal static IRedisHelperFactory Instance { get; } = new RedisHelperFactory();

    /// <summary>
    /// Constructs or returns a cached <see cref="RedisHelper{TEntity}"/>.
    /// </summary>
    /// <typeparam name="TEntity">The entity type.</typeparam>
    /// <returns>The <see cref="RedisHelper{TEntity}"/>.</returns>
    public IRedisHelper<TEntity> CreateHelper<TEntity>()
        where TEntity : class, new()
    {
        var newlyAdded = false;

        //Attempt to get the helper, and if not, create a new one
        var helper = (IRedisHelper<TEntity>)_helpers.GetOrAdd(typeof(TEntity), _ =>
        {
            newlyAdded = true;
            return new RedisHelper<TEntity>(this);
        });

        //If the helper is new, initialize it
        if (newlyAdded)
            (helper as RedisHelper<TEntity>)?.Initialize();

        return helper;
    }
}
