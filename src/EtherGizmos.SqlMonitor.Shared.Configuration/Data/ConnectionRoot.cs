namespace EtherGizmos.SqlMonitor.Shared.Configuration.Data;

public class ConnectionRoot
{
    public Dictionary<string, ConnectionOptions> Connections { get; set; } = new();
}
