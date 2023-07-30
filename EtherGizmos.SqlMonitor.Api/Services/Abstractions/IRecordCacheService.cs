namespace EtherGizmos.SqlMonitor.Api.Services.Abstractions;

/// <summary>
/// Set as a singleton service to store records that can be shared across other services.
/// </summary>
public interface IRecordCacheService
{
    /// <summary>
    /// Fetches or populates the cached record set.
    /// </summary>
    /// <typeparam name="T">The type of cached record.</typeparam>
    /// <param name="cacheName">The name of the cache.</param>
    /// <param name="loadCache">A function to populate the cache, if expired.</param>
    /// <returns>The cached record.</returns>
    Task<T> GetOrLoadCacheAsync<T>(string cacheName, Func<Task<T>> loadCache);

    /// <summary>
    /// Purges a cache prior to its expiry.
    /// </summary>
    /// <param name="cacheName">The name of the cache to purge.</param>
    void InvalidateCache(string cacheName);
}
