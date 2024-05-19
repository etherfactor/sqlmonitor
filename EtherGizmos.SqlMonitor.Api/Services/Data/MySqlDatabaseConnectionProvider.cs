using EtherGizmos.SqlMonitor.Api.Services.Data.Abstractions;
using EtherGizmos.SqlMonitor.Services.Data.Configuration;
using Microsoft.Extensions.Options;
using MySqlConnector;
using System.Data.Common;

namespace EtherGizmos.SqlMonitor.Api.Services.Data;

/// <summary>
/// Provides a database connection string.
/// </summary>
public class MySqlDatabaseConnectionProvider : IDatabaseConnectionProvider
{
    private readonly IOptions<MySqlOptions> _options;

    /// <summary>
    /// Construct the service.
    /// </summary>
    /// <param name="options">The application's configuration.</param>
    public MySqlDatabaseConnectionProvider(IOptions<MySqlOptions> options)
    {
        _options = options;
    }

    /// <inheritdoc/>
    public DbConnection GetConnection()
    {
        var connection = new MySqlConnection(GetConnectionString());
        return connection;
    }

    /// <inheritdoc/>
    public string GetConnectionString()
    {
        var optionsValue = _options.Value;

        var builder = new MySqlConnectionStringBuilder();

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
        var builder = new MySqlConnectionStringBuilder(GetConnectionString());
        builder.Remove("Database");

        var connection = new MySqlConnection(builder.ConnectionString);
        return connection;
    }
}
