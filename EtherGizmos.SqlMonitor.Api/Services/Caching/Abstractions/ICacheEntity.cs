namespace EtherGizmos.SqlMonitor.Api.Services.Caching.Abstractions;

/// <summary>
/// Provides means for caching and retrieving a single entity.
/// </summary>
/// <typeparam name="TEntity">The type of entity being cached.</typeparam>
public interface ICacheEntity<TEntity> : ICanAlter<TEntity>, ICanGet<TEntity>
{
}
