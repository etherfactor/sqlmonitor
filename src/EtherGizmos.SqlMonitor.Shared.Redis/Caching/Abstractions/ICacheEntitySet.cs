namespace EtherGizmos.SqlMonitor.Shared.Redis.Caching.Abstractions;

/// <summary>
/// Provides means for caching and retrieving entities in a set.
/// </summary>
/// <typeparam name="TEntity">The type of entity being cached.</typeparam>
public interface ICacheEntitySet<TEntity> : ICanAlterSet<TEntity>, ICanGetFromSet<TEntity>, ICacheFiltered<TEntity>
{
}
