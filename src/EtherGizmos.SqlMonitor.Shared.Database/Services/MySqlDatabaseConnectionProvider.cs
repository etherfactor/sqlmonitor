using EtherGizmos.SqlMonitor.Shared.Configuration.Data;
using EtherGizmos.SqlMonitor.Shared.Database.Services.Abstractions;
using Microsoft.Extensions.Options;
using MySql.Data.MySqlClient;
using System.Data.Common;

namespace EtherGizmos.SqlMonitor.Shared.Database.Services;

/// <summary>
/// Provides a database connection string.
/// </summary>
public class MySqlDatabaseConnectionProvider : IDatabaseConnectionProvider
{
    private readonly IOptions<ConnectionMySqlOptions> _options;

    /// <summary>
    /// Construct the service.
    /// </summary>
    /// <param name="options">The application's configuration.</param>
    public MySqlDatabaseConnectionProvider(IOptions<ConnectionMySqlOptions> options)
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

        foreach (string key in optionsValue.ConnectionValues.Keys)
        {
            string? value = optionsValue.ConnectionValues[key];
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
