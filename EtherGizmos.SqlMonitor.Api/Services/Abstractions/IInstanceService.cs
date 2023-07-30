﻿using EtherGizmos.SqlMonitor.Models.Database;

namespace EtherGizmos.SqlMonitor.Api.Services.Abstractions;

/// <summary>
/// Provides access to <see cref="Instance"/> records.
/// </summary>
public interface IInstanceService : ICacheableQueryableService<Instance>, IEditableQueryableService<Instance>
{
}
