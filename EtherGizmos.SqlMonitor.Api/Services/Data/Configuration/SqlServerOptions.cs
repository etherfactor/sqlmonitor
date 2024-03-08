using EtherGizmos.SqlMonitor.Api.Helpers;

namespace EtherGizmos.SqlMonitor.Api.Services.Data.Configuration;

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

    public void AssertValid(string rootPath)
    {
        if (GetProperty("Data Source") is null && GetProperty("Server") is null)
            ThrowHelper.ForMissingConfiguration(rootPath, this, "Data Source", typeof(string));
        if (GetProperty("Initial Catalog") is null && GetProperty("Database") is null)
            ThrowHelper.ForMissingConfiguration(rootPath, this, "Initial Catalog", typeof(string));
    }
}
