﻿using EtherGizmos.SqlMonitor.Api.Services.Abstractions;
using Microsoft.Data.SqlClient;

namespace EtherGizmos.SqlMonitor.Api.Services.Data.Access;

/// <summary>
/// Provides a database connection string.
/// </summary>
public class DatabaseConnectionProvider : IDatabaseConnectionProvider
{
    /// <summary>
    /// The application's configuration.
    /// </summary>
    private IConfiguration Configuration { get; }

    /// <summary>
    /// Construct the service.
    /// </summary>
    /// <param name="configuration">The application's configuration.</param>
    public DatabaseConnectionProvider(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    /// <inheritdoc/>
    public string GetConnectionString()
    {
        var properties = Configuration.GetSection("Connections:Database").Get<Dictionary<string, string>>();
        if (properties == null)
            throw new InvalidOperationException("Must specify the Connections:Database portion of appsettings.json.");

        var builder = new SqlConnectionStringBuilder();
        foreach (string key in properties.Keys)
        {
            string value = properties[key];
            builder.Add(key, value);
        }

        return builder.ConnectionString;
    }
}