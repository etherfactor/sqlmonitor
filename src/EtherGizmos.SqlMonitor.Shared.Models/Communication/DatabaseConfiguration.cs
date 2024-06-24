namespace EtherGizmos.SqlMonitor.Shared.Models.Communication;

public class DatabaseConfiguration
{
    public string ConnectionString { get; set; }

    /// <summary>
    /// Not intended for direct use.
    /// </summary>
    public DatabaseConfiguration()
    {
        ConnectionString = null!;
    }

    public DatabaseConfiguration(
        string connectionString)
    {
        ConnectionString = connectionString;
    }
}
