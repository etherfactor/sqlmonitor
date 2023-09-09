namespace EtherGizmos.SqlMonitor.Api.Services.Caching.Abstractions;

public interface ICacheFiltered<TEntity> : ICanFilter<TEntity>, ICanList<TEntity>
{
}
