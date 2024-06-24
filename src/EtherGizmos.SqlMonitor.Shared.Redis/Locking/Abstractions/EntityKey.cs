namespace EtherGizmos.SqlMonitor.Shared.Redis.Locking.Abstractions;

internal readonly struct EntityKey<TEntity> : IEntityKey<TEntity>
{
    public string Name { get; }

    public EntityKey(string name)
    {
        Name = name;
    }
}
