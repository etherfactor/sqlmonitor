using EtherGizmos.SqlMonitor.Api.Services.Caching.Abstractions;

namespace EtherGizmos.SqlMonitor.Api.Services.Caching;

/// <summary>
/// Provides means for caching and retrieving a single entity, in memory.
/// </summary>
/// <typeparam name="TEntity">The type of entity being cached.</typeparam>
internal class InMemoryCacheEntity<TEntity> : ICacheEntity<TEntity>
    where TEntity : new()
{
    private static TEntity? _entity;

    /// <inheritdoc/>
    public Task DeleteAsync(CancellationToken cancellationToken = default)
    {
        _entity = default;
        return Task.CompletedTask;
    }

    /// <inheritdoc/>
    public Task<TEntity?> GetAsync(CancellationToken cancellationToken = default)
    {
        return Task.FromResult(_entity);
    }

    /// <inheritdoc/>
    public Task SetAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        _entity = entity;
        return Task.CompletedTask;
    }
}
