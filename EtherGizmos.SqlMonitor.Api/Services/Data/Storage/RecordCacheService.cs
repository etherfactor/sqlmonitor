using EtherGizmos.SqlMonitor.Api.Services.Abstractions;
using System.Collections.Concurrent;

namespace EtherGizmos.SqlMonitor.Api.Services.Data.Storage;

/// <summary>
/// Set as a singleton service to store records that can be shared across other services.
/// </summary>
public class RecordCacheService : IRecordCacheService
{
    /// <summary>
    /// The logger to use.
    /// </summary>
    private ILogger Logger { get; }

    /// <summary>
    /// Provides access to configuration files as they change.
    /// </summary>
    private IConfiguration Configuration { get; }

    /// <summary>
    /// Caches records.
    /// </summary>
    private ConcurrentDictionary<string, object?> Cache { get; } = new ConcurrentDictionary<string, object?>();

    /// <summary>
    /// The types of cached records.
    /// </summary>
    private ConcurrentDictionary<string, Type> CacheType { get; } = new ConcurrentDictionary<string, Type>();

    /// <summary>
    /// The times at which the records were cached.
    /// </summary>
    private ConcurrentDictionary<string, DateTimeOffset> CachedAt { get; } = new ConcurrentDictionary<string, DateTimeOffset>();

    /// <summary>
    /// Construct the service.
    /// </summary>
    /// <param name="logger">The logger to use.</param>
    /// <param name="configuration">Provides access to configuration files as they change.</param>
    public RecordCacheService(ILogger<RecordCacheService> logger, IConfiguration configuration)
    {
        Logger = logger;
        Configuration = configuration;
    }

    /// <inheritdoc/>
    public async Task<TEntity> GetOrLoadCacheAsync<TEntity>(string cacheName, Func<Task<TEntity>> loadCache)
    {
        var cacheLifetime = Configuration.GetValue<TimeSpan>($"Caching:{cacheName}:Lifetime");

        //Ensure the newly cached record shares the same type as the currently cached record
        if (CacheType.ContainsKey(cacheName) && CacheType[cacheName] != typeof(TEntity))
        {
            throw new InvalidOperationException(string.Format("Type {0} must be the same as currently cached type {1}", typeof(TEntity), CacheType[cacheName]));
        }

        //Populate the cache if it does not exist or is outdated
        TimeSpan cacheAge = TimeSpan.FromSeconds(86399);
        if (!CachedAt.ContainsKey(cacheName) || (cacheAge = DateTime.UtcNow - CachedAt[cacheName]) > cacheLifetime)
        {
            var inCache = Cache.ContainsKey(cacheName);
            var inCacheType = CacheType.ContainsKey(cacheName);
            var inCachedAt = CachedAt.ContainsKey(cacheName);

            if (!inCache)
            {
                Logger.Log(LogLevel.Information, "Cache {CacheName} not found", cacheName);
            }
            else
            {
                Logger.Log(LogLevel.Information, "Cache {CacheName} has age {CacheAge} of max {CacheMaxAge}", cacheName, cacheAge, cacheLifetime);
            }

            //Populate the cache
            Logger.Log(LogLevel.Information, "Populating cache {CacheName}", cacheName);
            var toCache = await loadCache();

            //Cache the results
            if (!inCache)
            {
                Cache.TryAdd(cacheName, toCache);
            }

            if (!inCacheType)
            {
                CacheType.TryAdd(cacheName, typeof(TEntity));
            }

            if (!inCachedAt)
            {
                CachedAt.TryAdd(cacheName, DateTime.UtcNow);
            }

            Cache[cacheName] = toCache;
            CacheType[cacheName] = typeof(TEntity);
            CachedAt[cacheName] = DateTime.UtcNow;

            Logger.Log(LogLevel.Information, "Cache {CacheName} populated", cacheName);
        }

        //Shouldn't be null unless a nullable value is used
        return (TEntity)Cache[cacheName]!;
    }

    /// <inheritdoc/>
    public void InvalidateCache(string cacheName)
    {
        Logger.Log(LogLevel.Information, "Invalidating cache {CacheName}", cacheName);

        if (Cache.ContainsKey(cacheName))
        {
            Cache.TryRemove(cacheName, out _);
        }

        if (CacheType.ContainsKey(cacheName))
        {
            CacheType.TryRemove(cacheName, out _);
        }

        if (CachedAt.ContainsKey(cacheName))
        {
            CachedAt.TryRemove(cacheName, out _);
        }
    }
}
