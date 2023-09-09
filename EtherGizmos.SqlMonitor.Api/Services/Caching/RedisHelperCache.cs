namespace EtherGizmos.SqlMonitor.Api.Services.Caching;

/// <summary>
/// Caches <see cref="RedisHelper{TEntity}"/> instances.
/// </summary>
public static class RedisHelperCache
{
    private static readonly IDictionary<Type, object> _helpers = new Dictionary<Type, object>();

    /// <summary>
    /// Constructs or returns a cached <see cref="RedisHelper{TEntity}"/>.
    /// </summary>
    /// <typeparam name="TEntity">The entity type.</typeparam>
    /// <returns>The <see cref="RedisHelper{TEntity}"/>.</returns>
    public static RedisHelper<TEntity> For<TEntity>()
        where TEntity : new()
    {
        if (!_helpers.ContainsKey(typeof(TEntity)))
        {
            var helper = new RedisHelper<TEntity>();
            _helpers.Add(typeof(TEntity), helper);
        }

        return (RedisHelper<TEntity>)_helpers[typeof(TEntity)];
    }
}
