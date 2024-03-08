using EtherGizmos.SqlMonitor.Api.Extensions.Dotnet;
using EtherGizmos.SqlMonitor.Api.Services.Caching.Abstractions;

namespace EtherGizmos.SqlMonitor.Api.Services.Caching;

/// <summary>
/// A lockable job in a distributed cache.
/// </summary>
public readonly struct JobCacheKey : ICacheKey
{
    /// <inheritdoc/>
    public readonly string KeyName { get; }

    /// <summary>
    /// Use <see cref="CacheKey.ForJob(string)"/> instead!
    /// </summary>
    internal JobCacheKey(string name)
    {
        KeyName = name.ToSnakeCase();
    }

    /// <inheritdoc/>
    public override int GetHashCode()
    {
        return (KeyName).GetHashCode();
    }
}
