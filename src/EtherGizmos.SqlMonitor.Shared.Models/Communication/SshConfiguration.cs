using EtherGizmos.SqlMonitor.Shared.Models.Database.Enums;

namespace EtherGizmos.SqlMonitor.Shared.Models.Communication;

public class SshConfiguration
{
    public required string HostName { get; set; }

    public required int Port { get; set; }

    public required string FilePath { get; set; }

    public required SshAuthenticationType AuthenticationType { get; set; }

    public required string Username { get; set; }

    public string? Password { get; set; }

    public string? PrivateKey { get; set; }

    public string? PrivateKeyPassword { get; set; }

    public required string Command { get; set; }

    public required string Arguments { get; set; }
}
