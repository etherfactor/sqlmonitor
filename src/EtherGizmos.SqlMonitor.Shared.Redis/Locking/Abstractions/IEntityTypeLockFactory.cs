namespace EtherGizmos.SqlMonitor.Shared.Redis.Locking.Abstractions;

public interface IEntityTypeLockFactory<TEntity> : IEntityLockFactory
{
    Func<TEntity, object?[]> KeyResolver { get; }

    IEntityKey<TEntity> CreateKey(TEntity entity) => new EntityKey<TEntity>(this.CreateKeyInternal(entity));
}

internal static class IEntityTypeLockFactoryExtensions
{
    public static string CreateKeyInternal<TEntity>(
        this IEntityTypeLockFactory<TEntity> @this,
        TEntity entity)
    {
        var keys = @this.KeyResolver(entity);

        var keyName = @this.CondenseKey(keys);

        return keyName;
    }
}
