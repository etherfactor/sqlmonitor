namespace EtherGizmos.SqlMonitor.Shared.Redis.Locking.Abstractions;

public interface IEntityLockFactory
{
    public string EntityType { get; }
}

public static class IEntityLockFactoryExtensions
{
    internal static string CondenseKey(
        this IEntityLockFactory @this,
        object?[] keys)
    {
        var keyFragment = string.Join(",", keys.Select(e => e.ToString()));
        var keyName = string.Join(":", @this.EntityType, keyFragment);

        return keyName;
    }
}
