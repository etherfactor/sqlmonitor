using Castle.DynamicProxy;
using EtherGizmos.SqlMonitor.Api.Extensions;
using EtherGizmos.SqlMonitor.Models.Annotations;
using StackExchange.Redis;
using System.Collections.Concurrent;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;

namespace EtherGizmos.SqlMonitor.Api.Services.Caching;

public class RedisLazyLoadingInterceptor<TEntity> : IInterceptor
    where TEntity : class, new()
{
    private readonly IDatabase _database;
    private readonly ConcurrentDictionary<string, object> _savedObjects;
    private readonly IEnumerable<(PropertyInfo, ColumnAttribute)> _keys;
    private readonly IEnumerable<(PropertyInfo, ColumnAttribute)> _all;
    private readonly Dictionary<string, Func<Task>> _interceptorSingles;
    private readonly Dictionary<string, Func<Task>> _interceptorSets;
    private readonly Dictionary<string, bool> _loadedProperties;

    private Dictionary<string, object?> _defaultValues;

    public RedisLazyLoadingInterceptor(
        IDatabase database,
        ConcurrentDictionary<string, object> savedObjects,
        IEnumerable<(PropertyInfo, ColumnAttribute)> keys,
        IEnumerable<(PropertyInfo, ColumnAttribute)> all,
        IEnumerable<(PropertyInfo, LookupAttribute)> lookupSingles,
        IEnumerable<(PropertyInfo, LookupIndexAttribute)> lookupSets)
    {
        _database = database;
        _savedObjects = savedObjects;
        _keys = keys;
        _all = all;
        _defaultValues = new Dictionary<string, object?>();
        _interceptorSingles = new Dictionary<string, Func<Task>>();
        _interceptorSets = new Dictionary<string, Func<Task>>();
        _loadedProperties = new Dictionary<string, bool>();

        var helper = RedisHelperCache.For<TEntity>();

        foreach (var lookup in lookupSingles)
        {
            var propertyName = lookup.Item1.Name;
            var lookupType = lookup.Item1.PropertyType;
            var lookupProperties = lookup.Item2.IdProperties;

            var lookupKeys = lookupProperties.Select(e => _all.Single(p => p.Item1.Name == e).Item1);

            var method = typeof(RedisLazyLoadingInterceptor<TEntity>)
                .GetMethod(nameof(BuildSingleInterceptor), BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)!
                .MakeGenericMethod(lookupType);

            method.Invoke(this, new object[] { propertyName, lookupKeys, savedObjects });
        }

        foreach (var lookup in lookupSets)
        {
            var propertyName = lookup.Item1.Name;
            var lookupType = lookup.Item1.PropertyType.GenericTypeArguments[0];
            var lookupKey = lookup.Item2.Name;

            var method = typeof(RedisLazyLoadingInterceptor<TEntity>)
                .GetMethod(nameof(BuildSetInterceptor), BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)!
                .MakeGenericMethod(lookupType);

            method.Invoke(this, new object[] { propertyName, lookupKey, savedObjects });
        }
    }

    private void BuildSingleInterceptor<TSubEntity>(string propertyName, IEnumerable<PropertyInfo> lookupKeys, ConcurrentDictionary<string, object> savedObjects)
        where TSubEntity : class, new()
    {
        var helper = RedisHelperCache.For<TEntity>();
        var subHelper = RedisHelperCache.For<TSubEntity>();

        var tableName = helper.GetTableName();
        var subTableName = subHelper.GetTableName();

        var action = async () =>
        {
            var tempEntity = new TSubEntity();
            foreach (var lookupKey in lookupKeys)
            {
                var lookupValue = _defaultValues[lookupKey.Name];
                lookupKey.SetValue(tempEntity, lookupValue);
            }

            var entityKey = subHelper.GetSetEntityKey(tempEntity);

            var transaction = _database.CreateTransaction();
            var subAction = subHelper.AppendReadAction(_database, transaction, key: entityKey, savedObjects: savedObjects);

            await transaction.ExecuteAsync();
            return await subAction();
        };

        AddSingleInterceptor(propertyName, action);
    }

    private void BuildSetInterceptor<TSubEntity>(string propertyName, string lookupKey, ConcurrentDictionary<string, object> savedObjects)
        where TSubEntity : class, new()
    {
        var helper = RedisHelperCache.For<TEntity>();
        var subHelper = RedisHelperCache.For<TSubEntity>();

        var useLookupKey = new RedisKey($"{Constants.Cache.SchemaName}:$$table:{subHelper.GetTableName()}:{helper.GetTableName()}:${lookupKey.ToSnakeCase()}");

        var action = async () =>
        {
            var transaction = _database.CreateTransaction();

            var tempKey = helper.GetTempKey();

            var useLookupValue = helper.GetRecordId(_keys.Select(e => _defaultValues[e.Item1.Name.ToString()]!));
            var test = transaction.SortedSetRangeAndStoreAsync(
                useLookupKey,
                tempKey,
                useLookupValue,
                useLookupValue,
                exclude: Exclude.None,
                sortedSetOrder: SortedSetOrder.ByLex);

            var subAction = subHelper.AppendListAction(_database, transaction, lookupKey: tempKey, savedObjects: savedObjects);

            //_ = transaction.KeyDeleteAsync(tempKey);

            await transaction.ExecuteAsync();
            return await subAction();
        };

        AddSetInterceptor(propertyName, action);
    }

    public void SetInitialValues(Dictionary<string, object?> values)
    {
        _defaultValues = values;
    }

    public void AddSingleInterceptor<TData>(string propertyName, Func<Task<TData?>> action)
        where TData : class, new()
    {
        _interceptorSingles.Add(propertyName, action);
    }

    public void AddSetInterceptor<TData>(string propertyName, Func<Task<List<TData>>> action)
        where TData : class, new()
    {
        _interceptorSets.Add(propertyName, action);
    }

    public void Intercept(IInvocation invocation)
    {
        var methodName = invocation.Method.Name;

        if (methodName.StartsWith("get_"))
        {
            HandleGet(invocation);
        }
        else if (methodName.StartsWith("set_"))
        {
            HandleSet(invocation);
        }

        invocation.Proceed();
        return;
    }

    private void HandleGet(IInvocation invocation)
    {
        var propertyName = invocation.Method.Name.Replace("get_", "");
        var property = invocation.Method.DeclaringType?.GetProperty(propertyName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

        if (property == null)
            return;

        if (!_interceptorSingles.ContainsKey(propertyName) && !_interceptorSets.ContainsKey(propertyName))
            return;

        if (_loadedProperties.ContainsKey(propertyName))
            return;

        if (_interceptorSingles.ContainsKey(propertyName))
        {
            var helper = typeof(RedisLazyLoadingInterceptor<TEntity>)
                .GetMethod(nameof(HandleGetSingle), BindingFlags.NonPublic | BindingFlags.Instance)!
                .MakeGenericMethod(property.PropertyType);

            helper.Invoke(this, new object[] { invocation, property });
        }

        if (_interceptorSets.ContainsKey(propertyName))
        {
            var helper = typeof(RedisLazyLoadingInterceptor<TEntity>)
                .GetMethod(nameof(HandleGetSet), BindingFlags.NonPublic | BindingFlags.Instance)!
                .MakeGenericMethod(property.PropertyType.GenericTypeArguments[0]);

            helper.Invoke(this, new object[] { invocation, property });
        }

        _loadedProperties.TryAdd(propertyName, true);
    }

    private void HandleGetSingle<TData>(IInvocation invocation, PropertyInfo property)
        where TData : class, new()
    {
        var action = (Func<Task<TData?>>)_interceptorSingles[property.Name];
        var actionTask = action();

        actionTask.Wait();
        var data = actionTask.Result;

        property.SetValue(invocation.InvocationTarget, data);
    }

    private void HandleGetSet<TData>(IInvocation invocation, PropertyInfo property)
        where TData : class, new()
    {
        var action = (Func<Task<List<TData>>>)_interceptorSets[property.Name];
        var actionTask = action();

        actionTask.Wait();
        var data = actionTask.Result;

        property.SetValue(invocation.InvocationTarget, data);
    }

    private void HandleSet(IInvocation invocation)
    {
        var propertyName = invocation.Method.Name.Replace("set_", "");
        //var property = invocation.Method.DeclaringType?.GetProperty(propertyName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

        _loadedProperties.TryAdd(propertyName, true);
    }
}
