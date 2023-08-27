using EtherGizmos.SqlMonitor.Api.Services.Caching;
using EtherGizmos.SqlMonitor.Api.Services.Caching.Abstractions;

namespace EtherGizmos.SqlMonitor.Api.Extensions;

public static class ILockedDistributedCacheExtensions
{
    public static async Task<TEntity> GetOrCalculateAsync<TEntity>(
        this ILockedDistributedCache @this,
        CacheKey<TEntity> key,
        Func<Task<TEntity>> calculateAsync,
        TimeSpan timeout = default,
        CancellationToken cancellationToken = default)
    {
        using var @lock = await @this.AcquireLockAsync(key, timeout, cancellationToken);
        if (@lock is null)
            throw new ApplicationException("Failed to acquire a lock for 60+ seconds.");

        TEntity? entity = await @this.GetAsync(key, cancellationToken);
        if (entity is null)
        {
            entity = await calculateAsync();
            await @this.SetWithLockAsync(key, @lock, entity, cancellationToken);
        }

        return entity;
    }
}
