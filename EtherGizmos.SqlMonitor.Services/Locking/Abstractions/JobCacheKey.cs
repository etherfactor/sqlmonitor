﻿using EtherGizmos.SqlMonitor.Api.Extensions.Dotnet;

namespace EtherGizmos.SqlMonitor.Services.Locking.Abstractions;

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
    public JobCacheKey(Type jobType)
    {
        KeyName = jobType.Name.ToSnakeCase();
    }

    /// <inheritdoc/>
    public override int GetHashCode()
    {
        return KeyName.GetHashCode();
    }
}