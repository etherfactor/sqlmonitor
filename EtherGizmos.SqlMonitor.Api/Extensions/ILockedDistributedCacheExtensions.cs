using EtherGizmos.SqlMonitor.Api.Services.Caching;
using EtherGizmos.SqlMonitor.Api.Services.Caching.Abstractions;

namespace EtherGizmos.SqlMonitor.Api.Extensions;

public static class ILockedDistributedCacheExtensions
{
    public static async Task<TEntity> GetOrCalculateAsync<TEntity>(
        this IDistributedRecordCache @this,
        EntityCacheKey<TEntity> key,
        Func<Task<TEntity>> calculateAsync,
        TimeSpan timeout = default,
        CancellationToken cancellationToken = default)
        where TEntity : class, new()
    {
        var entitySet = @this.Entity(key);
        TEntity? entity = await entitySet.GetAsync(cancellationToken);
        if (entity is null)
        {
            using var @lock = await @this.AcquireLockAsync(key, timeout, cancellationToken);
            if (@lock is null)
                throw new ApplicationException($"Failed to acquire a lock for {timeout.TotalSeconds} seconds.");

            entity = await calculateAsync();
            await entitySet.SetAsync(entity, cancellationToken);
        }

        return entity;
    }
}
