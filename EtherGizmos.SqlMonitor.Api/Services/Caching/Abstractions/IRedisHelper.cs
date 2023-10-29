using StackExchange.Redis;
using System.Collections.Concurrent;

namespace EtherGizmos.SqlMonitor.Api.Services.Caching.Abstractions;

public interface IRedisHelper { }

public interface IRedisHelper<TEntity> : IRedisHelper
    where TEntity : class, new()
{
    IEnumerable<IRedisKeyProperty<TEntity>> GetKeyProperties();
    IEnumerable<IRedisLookupFromOtherProperty<TEntity>> GetLookupSetProperties();
    IEnumerable<IRedisLookupToOtherProperty<TEntity>> GetLookupSingleProperties();
    IEnumerable<IRedisProperty<TEntity>> GetProperties();
    RedisKey GetEntitySetEntityKey(TEntity entity);
    RedisValue GetRecordId(TEntity entity);
    RedisValue GetRecordId(IEnumerable<object> keys);
    string GetTableName();
    void AppendAddAction(IDatabase database, ITransaction transaction, TEntity entity, ConcurrentDictionary<string, object>? savedObjects = null);
    Func<Task<TEntity?>> AppendReadAction(IDatabase database, ITransaction transaction, EntityCacheKey<TEntity> key, ConcurrentDictionary<string, object>? savedObjects = null);
    void AppendDeleteAction(IDatabase database, ITransaction transaction, EntityCacheKey<TEntity> key);
    void AppendSetAction(IDatabase database, ITransaction transaction, EntityCacheKey<TEntity> key, TEntity entity);
    void AppendRemoveAction(IDatabase database, ITransaction transaction, TEntity entity);
    Func<Task<List<TEntity>>> AppendListAction(IDatabase database, ITransaction transaction, IEnumerable<ICacheEntitySetFilter<TEntity>>? filters = null, RedisKey? lookupKey = null, ConcurrentDictionary<string, object>? savedObjects = null);
    Func<Task<TEntity?>> AppendReadAction(IDatabase database, ITransaction transaction, RedisKey key, ConcurrentDictionary<string, object>? savedObjects = null);
    void BuildAddAction(IDatabase database, ITransaction transaction, TEntity entity, ConcurrentDictionary<string, object> savedObjects);
    RedisKey GetEntitySetEntityKey(object[] keys);
}
