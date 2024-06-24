namespace EtherGizmos.SqlMonitor.Shared.Database.Services.Abstractions;

/// <summary>
/// Provides access to cached internal records.
/// </summary>
/// <typeparam name="T">The type of internal record.</typeparam>
public interface ICacheableQueryableService<T>
    where T : class
{
    /// <summary>
    /// Fetches or populates the cached record set.
    /// </summary>
    /// <returns>The cached record set.</returns>
    Task<IEnumerable<T>> GetOrLoadCacheAsync(CancellationToken cancellationToken = default);
}
