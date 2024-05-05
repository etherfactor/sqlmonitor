using EtherGizmos.SqlMonitor.Api.Helpers;

namespace EtherGizmos.SqlMonitor.Api.Services.Data.Configuration;

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
