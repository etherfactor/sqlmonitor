using EtherGizmos.SqlMonitor.Api.Services.Data.Abstractions;
using EtherGizmos.SqlMonitor.Api.Services.Data.Configuration;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Options;

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
    /// <param name="configuration">The application's configuration.</param>
    public SqlServerDatabaseConnectionProvider(IOptions<SqlServerOptions> options)
    {
        _options = options;
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
}
