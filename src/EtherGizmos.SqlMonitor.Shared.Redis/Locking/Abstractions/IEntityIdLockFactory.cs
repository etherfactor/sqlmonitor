namespace EtherGizmos.SqlMonitor.Shared.Redis.Locking.Abstractions;

public interface IEntityIdLockFactory<TEntity, TId> : IEntityLockFactory
{
    IEntityKey<TEntity> CreateKey(TId id) => new EntityKey<TEntity>(this.CondenseKey([id]));
}

public interface IEntityIdLockFactory<TEntity, TId1, TId2> : IEntityLockFactory
{
    IEntityKey<TEntity> CreateKey(TId1 id1, TId2 id2) => new EntityKey<TEntity>(this.CondenseKey([id1, id2]));
}

public interface IEntityIdLockFactory<TEntity, TId1, TId2, TId3> : IEntityLockFactory
{
    IEntityKey<TEntity> CreateKey(TId1 id1, TId2 id2, TId3 id3) => new EntityKey<TEntity>(this.CondenseKey([id1, id2, id3]));
}

public interface IEntityIdLockFactory<TEntity, TId1, TId2, TId3, TId4> : IEntityLockFactory
{
    IEntityKey<TEntity> CreateKey(TId1 id1, TId2 id2, TId3 id3, TId4 id4) => new EntityKey<TEntity>(this.CondenseKey([id1, id2, id3, id4]));
}
