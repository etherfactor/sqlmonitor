namespace EtherGizmos.SqlMonitor.Shared.Redis.Caching.Abstractions;

public interface ICacheFiltered<TEntity> : ICanFilter<TEntity>, ICanList<TEntity>
{
}
