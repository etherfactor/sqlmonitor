using Castle.DynamicProxy;
using EtherGizmos.SqlMonitor.Api.Extensions.Dotnet;
using EtherGizmos.SqlMonitor.Api.Services.Caching.Abstractions;
using EtherGizmos.SqlMonitor.Services;
using StackExchange.Redis;
using System.Collections.Concurrent;
using System.Reflection;

namespace EtherGizmos.SqlMonitor.Api.Services.Caching;

public class RedisLazyLoadingInterceptor<TEntity> : IInterceptor
    where TEntity : class, new()
{
    private readonly IRedisHelperFactory _factory;

    private readonly IDatabase _database;
    private readonly ConcurrentDictionary<string, object> _savedObjects;
    private readonly Dictionary<string, Func<Task>> _interceptorSingles;
    private readonly Dictionary<string, Func<Task>> _interceptorSets;
    private readonly Dictionary<string, bool> _loadedProperties;

    private bool _enabled;
    private Dictionary<string, object?> _defaultValues;

    public RedisLazyLoadingInterceptor(
        IRedisHelperFactory factory,
        IDatabase database,
        ConcurrentDictionary<string, object> savedObjects)
    {
        _factory = factory;

        _database = database;
        _savedObjects = savedObjects;
        _interceptorSingles = new Dictionary<string, Func<Task>>();
        _interceptorSets = new Dictionary<string, Func<Task>>();
        _loadedProperties = new Dictionary<string, bool>();

        _enabled = false;
        _defaultValues = new Dictionary<string, object?>();

        var helper = _factory.CreateHelper<TEntity>();

        var lookupSingles = helper.GetLookupSingleProperties();
        foreach (var lookup in lookupSingles)
        {
            var propertyName = lookup.PropertyName;
            var lookupType = lookup.PropertyType;

            var method = typeof(RedisLazyLoadingInterceptor<TEntity>)
                .GetMethod(nameof(BuildSingleInterceptor), BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)!
                .MakeGenericMethod(lookupType);

            method.Invoke(this, new object[] { propertyName, lookup, savedObjects });
        }

        var lookupSets = helper.GetLookupSetProperties();
        foreach (var lookup in lookupSets)
        {
            var propertyName = lookup.PropertyName;
            var lookupType = lookup.PropertyType.GenericTypeArguments[0];

            var method = typeof(RedisLazyLoadingInterceptor<TEntity>)
                .GetMethod(nameof(BuildSetInterceptor), BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)!
                .MakeGenericMethod(lookupType);

            method.Invoke(this, new object[] { propertyName, lookup, savedObjects });
        }
    }

    private void BuildSingleInterceptor<TSubEntity>(string propertyName, IRedisLookupToOtherProperty<TEntity> lookup, ConcurrentDictionary<string, object> savedObjects)
        where TSubEntity : class, new()
    {
        var helper = _factory.CreateHelper<TEntity>();
        var subHelper = _factory.CreateHelper<TSubEntity>();

        var keyProperties = subHelper.GetKeyProperties();

        var action = async () =>
        {
            var lookupEntity = lookup.GetLookupEntity<TSubEntity>(_defaultValues);

            var entityKey = subHelper.GetEntitySetEntityKey(lookupEntity);

            var transaction = _database.CreateTransaction();
            var subAction = subHelper.AppendReadAction(_database, transaction, key: entityKey, savedObjects: savedObjects);

            await transaction.ExecuteAsync();
            return await subAction();
        };

        AddSingleInterceptor(propertyName, action);
    }

    private void BuildSetInterceptor<TSubEntity>(string propertyName, IRedisLookupFromOtherProperty<TEntity> lookup, ConcurrentDictionary<string, object> savedObjects)
        where TSubEntity : class, new()
    {
        var helper = _factory.CreateHelper<TEntity>();
        var subHelper = _factory.CreateHelper<TSubEntity>();

        var primaryKeys = helper.GetKeyProperties();

        var action = async () =>
        {
            if (primaryKeys.All(e => _defaultValues.ContainsKey(e.DisplayName)))
            {
                var primaryKey = helper.GetRecordId(primaryKeys.Select(e => _defaultValues[e.DisplayName]!));
                var useLookupKey = new RedisKey($"{ServiceConstants.Cache.SchemaName}:$$table:{subHelper.GetTableName()}:{helper.GetTableName()}:{primaryKey}:${lookup.DisplayName.ToSnakeCase()}");

                var transaction = _database.CreateTransaction();
                var subAction = subHelper.AppendListAction(_database, transaction, lookupKey: useLookupKey, savedObjects: savedObjects);

                await transaction.ExecuteAsync();
                return await subAction();
            }
            else
            {
                return new List<TSubEntity>();
            }
        };

        AddSetInterceptor(propertyName, action);
    }

    internal void Enable()
    {
        _enabled = true;
    }

    internal void SetInitialValues(Dictionary<string, object?> values)
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
        if (_enabled)
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

        _loadedProperties.TryAdd(propertyName, true);
    }
}
