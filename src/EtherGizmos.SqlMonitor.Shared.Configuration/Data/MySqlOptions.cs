using EtherGizmos.SqlMonitor.Shared.Configuration.Helpers;

namespace EtherGizmos.SqlMonitor.Shared.Configuration.Data;

/// <summary>
/// Provides configuration options for a <see cref="MySqlConnectionStringBuilder"/>.
/// </summary>
public class MySqlOptions
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
        if (GetProperty("Server") is null)
            ThrowHelper.ForMissingConfiguration(rootPath, this, "Server", typeof(string));
        if (GetProperty("Database") is null)
            ThrowHelper.ForMissingConfiguration(rootPath, this, "Database", typeof(string));
        if (GetProperty("Uid") is null)
            ThrowHelper.ForMissingConfiguration(rootPath, this, "Uid", typeof(string));
        if (GetProperty("Pwd") is null)
            ThrowHelper.ForMissingConfiguration(rootPath, this, "Pwd", typeof(string));
    }
}
