﻿using EtherGizmos.SqlMonitor.Api.Data.Access;
using EtherGizmos.SqlMonitor.Api.Services.Abstractions;
using EtherGizmos.SqlMonitor.Models.Database;

namespace EtherGizmos.SqlMonitor.Api.Services.Data.Access;

/// <summary>
/// Provides access to <see cref="Permission"/> records.
/// </summary>
public class PermissionService : IPermissionService
{
    private DatabaseContext Context { get; }

    /// <summary>
    /// Construct the service.
    /// </summary>
    /// <param name="context">The internal database context.</param>
    public PermissionService(DatabaseContext context)
    {
        Context = context;
    }

    /// <inheritdoc/>
    public IQueryable<Permission> GetQueryable()
    {
        return Context.Permissions;
    }
}
