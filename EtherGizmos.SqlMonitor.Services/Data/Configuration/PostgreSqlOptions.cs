using EtherGizmos.SqlMonitor.Services.Helpers;

namespace EtherGizmos.SqlMonitor.Services.Data.Configuration;

/// <summary>
/// Provides configuration options for a MySQL connection string builder.
/// </summary>
public class PostgreSqlOptions
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
        if (GetProperty("Host") is null)
            ThrowHelper.ForMissingConfiguration(rootPath, this, "Host", typeof(string));
        if (GetProperty("Database") is null)
            ThrowHelper.ForMissingConfiguration(rootPath, this, "Database", typeof(string));
        if (GetProperty("User Id") is null && GetProperty("User ID") is null)
            ThrowHelper.ForMissingConfiguration(rootPath, this, "User Id", typeof(string));
        if (GetProperty("Password") is null)
            ThrowHelper.ForMissingConfiguration(rootPath, this, "Password", typeof(string));
    }
}
