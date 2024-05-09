using EtherGizmos.SqlMonitor.Api.Helpers;

namespace EtherGizmos.SqlMonitor.Api.Services.Data.Configuration;

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
