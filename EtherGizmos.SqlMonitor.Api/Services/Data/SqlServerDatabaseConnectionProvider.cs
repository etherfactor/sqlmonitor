using EtherGizmos.SqlMonitor.Api.Services.Data.Abstractions;
using EtherGizmos.SqlMonitor.Services.Data.Configuration;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Options;
using System.Data.Common;

namespace EtherGizmos.SqlMonitor.Api.Services.Data;

/// <summary>
/// Provides a database connection string.
/// </summary>
public class SqlServerDatabaseConnectionProvider : IDatabaseConnectionProvider
{
    private readonly IOptions<SqlServerOptions> _options;

    /// <summary>
    /// Construct the service.
    /// </summary>
    /// <param name="options">The application's configuration.</param>
    public SqlServerDatabaseConnectionProvider(IOptions<SqlServerOptions> options)
    {
        _options = options;
    }

    /// <inheritdoc/>
    public DbConnection GetConnection()
    {
        var connection = new SqlConnection(GetConnectionString());
        return connection;
    }

    /// <inheritdoc/>
    public string GetConnectionString()
    {
        var optionsValue = _options.Value;

        var builder = new SqlConnectionStringBuilder();

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
        var builder = new SqlConnectionStringBuilder(GetConnectionString());
        builder.Remove("Initial Catalog");

        var connection = new SqlConnection(builder.ConnectionString);
        return connection;
    }
}
