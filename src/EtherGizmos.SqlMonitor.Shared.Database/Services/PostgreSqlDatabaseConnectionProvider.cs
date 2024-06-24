﻿using EtherGizmos.SqlMonitor.Shared.Configuration.Data;
using EtherGizmos.SqlMonitor.Shared.Database.Services.Abstractions;
using Microsoft.Extensions.Options;
using Npgsql;
using System.Data.Common;

namespace EtherGizmos.SqlMonitor.Shared.Database.Services;

/// <summary>
/// Provides a database connection string.
/// </summary>
public class PostgreSqlDatabaseConnectionProvider : IDatabaseConnectionProvider
{
    private readonly IOptions<ConnectionPostgreSqlOptions> _options;

    /// <summary>
    /// Construct the service.
    /// </summary>
    /// <param name="configuration">The application's configuration.</param>
    public PostgreSqlDatabaseConnectionProvider(IOptions<ConnectionPostgreSqlOptions> options)
    {
        _options = options;
    }

    /// <inheritdoc/>
    public DbConnection GetConnection()
    {
        var connection = new NpgsqlConnection(GetConnectionString());
        return connection;
    }

    /// <inheritdoc/>
    public string GetConnectionString()
    {
        var optionsValue = _options.Value;

        var builder = new NpgsqlConnectionStringBuilder();

        foreach (string key in optionsValue.AllProperties.Keys)
        {
            string? value = optionsValue.AllProperties[key];
            if (value is not null)
            {
                builder.Add(key, value);
            }
        }

        return builder.ConnectionString;
    }

    /// <inheritdoc/>
    public DbConnection GetDefaultConnection()
    {
        var builder = new NpgsqlConnectionStringBuilder(GetConnectionString());
        builder.Remove("Database");

        var connection = new NpgsqlConnection(builder.ConnectionString);
        return connection;
    }
}
