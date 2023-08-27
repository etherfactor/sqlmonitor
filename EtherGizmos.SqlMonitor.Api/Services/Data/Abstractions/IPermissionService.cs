﻿using EtherGizmos.SqlMonitor.Models.Database;

namespace EtherGizmos.SqlMonitor.Api.Services.Data.Abstractions;

/// <summary>
/// Provides access to <see cref="Permission"/> records.
/// </summary>
public interface IPermissionService : IQueryableService<Permission>
{
}