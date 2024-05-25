using EtherGizmos.SqlMonitor.Shared.Configuration.Helpers;

namespace EtherGizmos.SqlMonitor.Shared.Configuration.Data;

/// <summary>
/// Provides configuration options for a SQL Server connection string builder.
/// </summary>
public class SqlServerOptions
{
    public Dictionary<string, string?> AllProperties { get; set; } = new();

    private string? GetProperty(string name)
    {
        if (AllProperties.TryGetValue(name, out string? value))
        {
            return value;
        }

        return null;
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
