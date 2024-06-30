using EtherGizmos.SqlMonitor.Shared.Configuration.Helpers;
using Microsoft.Data.SqlClient;

namespace EtherGizmos.SqlMonitor.Shared.Configuration.Data;

/// <summary>
/// Provides configuration options for a SQL Server connection string builder.
/// </summary>
public class ConnectionSqlServerOptions
{
    public string? ConnectionString { get; set; }

    public Dictionary<string, string?> ConnectionValues { get; set; } = new();

    public string ConnectionStringValue => ConnectionStringBuilder.ConnectionString;

    private SqlConnectionStringBuilder ConnectionStringBuilder
    {
        get
        {
            var builder = new SqlConnectionStringBuilder();
            if (ConnectionString is not null)
            {
                builder.ConnectionString = ConnectionString;
            }

            if (ConnectionValues is not null)
            {
                foreach (var pair in ConnectionValues)
                {
                    builder.Add(pair.Key, pair.Value!);
                }
            }

            return builder;
        }
    }

    private string? GetProperty(string name)
    {
        var builder = ConnectionStringBuilder;
        var maybeValue = builder.TryGetValue(name, out var value) ? value : null;

        return maybeValue?.ToString();
    }

    /// <summary>
    /// Ensures the configuration is valid and contains all the required properties.
    /// </summary>
    /// <param name="rootPath">The root of the configuration in which these settings are located.</param>
    public void AssertValid(string rootPath)
    {
        if (GetProperty("Data Source") is null && GetProperty("Server") is null)
            ThrowHelper.ForMissingConfiguration(rootPath, this, "Data Source", typeof(string));
        if (GetProperty("Initial Catalog") is null && GetProperty("Database") is null)
            ThrowHelper.ForMissingConfiguration(rootPath, this, "Initial Catalog", typeof(string));
    }
}
