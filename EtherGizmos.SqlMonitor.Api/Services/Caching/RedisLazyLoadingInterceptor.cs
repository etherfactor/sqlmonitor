using Castle.DynamicProxy;
using StackExchange.Redis;
using System;
using System.Reflection;

namespace EtherGizmos.SqlMonitor.Api.Services.Caching;

public class RedisLazyLoadingInterceptor : IInterceptor
{
    private IDatabase _database;
    private Dictionary<string, Func<IDatabase, Task>> _interceptorSingles;
    private Dictionary<string, Func<IDatabase, Task>> _interceptorSets;
    private Dictionary<string, bool> _loadedProperties;

    public RedisLazyLoadingInterceptor(IDatabase database)
    {
        _database = database;
        _interceptorSingles = new Dictionary<string, Func<IDatabase, Task>>();
        _interceptorSets = new Dictionary<string, Func<IDatabase, Task>>();
        _loadedProperties = new Dictionary<string, bool>();
    }

    public void SetInterceptor<TData>(string propertyName, Func<IDatabase, Task<TData?>> action)
        where TData : class, new()
    {
        _interceptorSingles.Add(propertyName, action);
    }

    public void SetListInterceptor<TData>(string propertyName, Func<IDatabase, Task<List<TData>>> action)
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
            var helper = typeof(RedisLazyLoadingInterceptor)
                .GetMethod(nameof(HandleGetSingle), BindingFlags.NonPublic | BindingFlags.Instance)!
                .MakeGenericMethod(property.PropertyType);

            helper.Invoke(this, new object[] { invocation, property });
        }

        if (_interceptorSets.ContainsKey(propertyName))
        {
            var helper = typeof(RedisLazyLoadingInterceptor)
                .GetMethod(nameof(HandleGetSet), BindingFlags.NonPublic | BindingFlags.Instance)!
                .MakeGenericMethod(property.PropertyType.GenericTypeArguments[0]);

            helper.Invoke(this, new object[] { invocation, property });
        }

        _loadedProperties.TryAdd(propertyName, true);
    }

    private void HandleGetSingle<TData>(IInvocation invocation, PropertyInfo property)
        where TData : class, new()
    {
        var action = (Func<IDatabase, Task<TData?>>)_interceptorSingles[property.Name];
        var actionTask = action(_database);
        
        actionTask.Wait();
        var data = actionTask.Result;

        property.SetValue(invocation.InvocationTarget, data);
    }

    private void HandleGetSet<TData>(IInvocation invocation, PropertyInfo property)
        where TData : class, new()
    {
        var action = (Func<IDatabase, Task<List<TData>>>)_interceptorSets[property.Name];
        var actionTask = action(_database);

        actionTask.Wait();
        var data = actionTask.Result;

        property.SetValue(invocation.InvocationTarget, data);
    }

    private void HandleSet(IInvocation invocation)
    {
        var propertyName = invocation.Method.Name.Replace("set_", "");
        var property = invocation.Method.DeclaringType?.GetProperty(propertyName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

        _loadedProperties.TryAdd(propertyName, true);
    }
}
