namespace EtherGizmos.SqlMonitor.Api.Services.Caching;

public static class CacheKey
{
    private static Dictionary<string, object> Keys { get; } = new Dictionary<string, object>();
    private static Dictionary<string, Type> KeyTypes { get; } = new Dictionary<string, Type>();
    private static Dictionary<string, int> KeyHashes { get; } = new Dictionary<string, int>();

    public static CacheKey<TEntity> Create<TEntity>(string key, bool requiresLock)
    {
        var newCacheKey = new CacheKey<TEntity>(key, requiresLock);
        if (Keys.ContainsKey(key))
        {
            if (KeyTypes[key] != typeof(TEntity) || KeyHashes[key] != newCacheKey.GetHashCode())
                throw new InvalidOperationException($"The cache key {key} already exists.");
        }
        else
        {
            Keys.Add(key, newCacheKey);
            KeyTypes.Add(key, typeof(TEntity));
            KeyHashes.Add(key, newCacheKey.GetHashCode());
        }

        var cacheKey = (CacheKey<TEntity>)Keys[key];

        return cacheKey;
    }
}

public struct CacheKey<TEntity>
{
    public readonly string Name { get; }

    public readonly string KeyName { get; }

    public readonly bool RequiresLock { get; }

    internal CacheKey(string name, bool requiresLock)
    {
        Name = name;
        KeyName = name; //TODO: convert title case to snake case
        RequiresLock = requiresLock;
    }

    public override int GetHashCode()
    {
        return (Name, RequiresLock).GetHashCode();
    }
}
