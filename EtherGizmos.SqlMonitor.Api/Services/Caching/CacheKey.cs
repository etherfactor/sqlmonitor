namespace EtherGizmos.SqlMonitor.Api.Services.Caching;

/// <summary>
/// Provides methods for constructing keys. Ensures no key conflicts by tracking existing keys.
/// </summary>
public static class CacheKey
{
    private readonly static Dictionary<string, object> _keys = new Dictionary<string, object>();
    private readonly static Dictionary<string, int> _keyHashes = new Dictionary<string, int>();

    /// <summary>
    /// Creates an entity key.
    /// </summary>
    /// <typeparam name="TEntity">The type of entity.</typeparam>
    /// <param name="key">The name of the key.</param>
    /// <param name="requiresLock">Whether or not setting the key requires a lock.</param>
    /// <returns>The entity key.</returns>
    /// <exception cref="InvalidOperationException"></exception>
    public static EntityCacheKey<TEntity> CreateEntity<TEntity>(string key, bool requiresLock)
    {
        var newCacheKey = new EntityCacheKey<TEntity>(key, requiresLock);
        var useKey = newCacheKey.KeyName;
        if (_keys.ContainsKey(useKey))
        {
            if (_keyHashes[useKey] != newCacheKey.GetHashCode())
                throw new InvalidOperationException($"The cache key {useKey} already exists.");
        }
        else
        {
            _keys.Add(useKey, newCacheKey);
            _keyHashes.Add(useKey, newCacheKey.GetHashCode());
        }

        var cacheKey = (EntityCacheKey<TEntity>)_keys[useKey];

        return cacheKey;
    }

    /// <summary>
    /// Creates a job key.
    /// </summary>
    /// <param name="key">The name of the key.</param>
    /// <returns>The job key.</returns>
    /// <exception cref="InvalidOperationException"></exception>
    public static JobCacheKey CreateJob(string key)
    {
        var newCacheKey = new JobCacheKey(key);
        var useKey = newCacheKey.KeyName;
        if (_keys.ContainsKey(useKey))
        {
            if (_keyHashes[useKey] != newCacheKey.GetHashCode())
                throw new InvalidOperationException($"The cache key {useKey} already exists.");
        }
        else
        {
            _keys.Add(useKey, newCacheKey);
            _keyHashes.Add(useKey, newCacheKey.GetHashCode());
        }

        var cacheKey = (JobCacheKey)_keys[useKey];

        return cacheKey;
    }
}
