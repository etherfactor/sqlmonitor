using EtherGizmos.SqlMonitor.Shared.Models.Database.Enums;

namespace EtherGizmos.SqlMonitor.Shared.Models.Communication;

public class WinRmConfiguration
{
    public required string Protocol { get; set; }

    public required string HostName { get; set; }

    public required int Port { get; set; }

    public required string FilePath { get; set; }

    public required WinRmAuthenticationType AuthenticationType { get; set; }

    public string? Username { get; set; }

    public string? Password { get; set; }

    public required string Command { get; set; }

    public required string Arguments { get; set; }
}
