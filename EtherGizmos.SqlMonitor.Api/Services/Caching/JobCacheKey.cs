using EtherGizmos.SqlMonitor.Api.Extensions;

namespace EtherGizmos.SqlMonitor.Api.Services.Caching;

/// <summary>
/// A lockable job in a distributed cache.
/// </summary>
public readonly struct JobCacheKey : ICacheKey
{
    /// <summary>
    /// The name of the job.
    /// </summary>
    public readonly string Name { get; }

    /// <inheritdoc/>
    public readonly string KeyName => $"{Constants.CacheSchemaName}:$job:{Name.ToSnakeCase()}";

    /// <summary>
    /// Use <see cref="CacheKey.CreateJob(string)"/> instead!
    /// </summary>
    internal JobCacheKey(string name)
    {
        Name = name;
    }

    /// <inheritdoc/>
    public override int GetHashCode()
    {
        return (Name).GetHashCode();
    }
}
