using EtherGizmos.SqlMonitor.Shared.Models.Database.Enums;

namespace EtherGizmos.SqlMonitor.Shared.Models.Communication;

public class WinRmConfiguration
{
    public string? Protocol { get; set; }

    public string? HostName { get; set; }

    public int Port { get; set; }

    public string? FilePath { get; set; }

    public WinRmAuthenticationType AuthenticationType { get; set; }

    public string? Username { get; set; }

    public string? Password { get; set; }

    public string? Command { get; set; }

    public string? Arguments { get; set; }
}
