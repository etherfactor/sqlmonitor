using EtherGizmos.SqlMonitor.Api.Services.Caching.Abstractions;

namespace EtherGizmos.SqlMonitor.Api.Services.Caching;

/// <summary>
/// Caches <see cref="IRedisHelper{TEntity}"/> instances.
/// </summary>
public class RedisHelperFactory : IRedisHelperFactory
{
    private static readonly IDictionary<Type, IRedisHelper> _helpers = new Dictionary<Type, IRedisHelper>();

    internal static IRedisHelperFactory Instance { get; } = new RedisHelperFactory();

    /// <summary>
    /// Constructs or returns a cached <see cref="RedisHelper{TEntity}"/>.
    /// </summary>
    /// <typeparam name="TEntity">The entity type.</typeparam>
    /// <returns>The <see cref="RedisHelper{TEntity}"/>.</returns>
    public IRedisHelper<TEntity> CreateHelper<TEntity>()
        where TEntity : class, new()
    {
        if (!_helpers.ContainsKey(typeof(TEntity)))
        {
            var helper = new RedisHelper<TEntity>(this);
            _helpers.Add(typeof(TEntity), helper);
            helper.Initialize();
        }

        return (IRedisHelper<TEntity>)_helpers[typeof(TEntity)];
    }
}
