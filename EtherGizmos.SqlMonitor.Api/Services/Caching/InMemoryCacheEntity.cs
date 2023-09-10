using EtherGizmos.SqlMonitor.Api.Services.Caching.Abstractions;
using System.Text.Json;

namespace EtherGizmos.SqlMonitor.Api.Services.Caching;

/// <summary>
/// Provides means for caching and retrieving a single entity, in memory.
/// </summary>
/// <typeparam name="TEntity">The type of entity being cached.</typeparam>
internal class InMemoryCacheEntity<TEntity> : ICacheEntity<TEntity>
    where TEntity : new()
{
    private static TEntity? _entity;

    private readonly IServiceProvider _serviceProvider;

    public InMemoryCacheEntity(
        IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

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
        //Forcefully detach the entity from any contexts
        var addEntity = JsonSerializer.Deserialize<TEntity>(JsonSerializer.Serialize(entity))!;
        _entity = addEntity;
        return Task.CompletedTask;
    }
}
