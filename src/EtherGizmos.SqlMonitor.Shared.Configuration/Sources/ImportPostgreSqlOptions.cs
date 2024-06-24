﻿using EtherGizmos.SqlMonitor.Shared.Configuration.Helpers;

namespace EtherGizmos.SqlMonitor.Shared.Configuration.Sources;

public class ImportPostgreSqlOptions : IValidatableOptions
{
    public string ConnectionId { get; set; } = null!;

    public void AssertValid(string rootPath)
    {
        if (string.IsNullOrWhiteSpace(ConnectionId))
            ThrowHelper.ForMissingConfiguration(rootPath, this, nameof(ConnectionId), typeof(string));
    }
}