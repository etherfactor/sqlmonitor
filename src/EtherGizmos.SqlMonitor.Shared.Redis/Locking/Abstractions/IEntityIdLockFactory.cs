namespace EtherGizmos.SqlMonitor.Shared.Redis.Locking.Abstractions;

public interface IEntityIdLockFactory<TEntity, TId> : IEntityLockFactory
{
    IEntityKey<TEntity> CreateKey(TId id) => new EntityKey<TEntity>(this.CreateKeyInternal(id));
}

internal static class IEntityIdLockFactoryExtensions
{
    public static string CreateKeyInternal<TEntity, TId>(
        this IEntityIdLockFactory<TEntity, TId> @this,
        TId id)
    {
        return @this.CondenseKey([id]);
    }
}
