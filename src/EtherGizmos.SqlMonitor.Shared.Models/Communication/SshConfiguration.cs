using EtherGizmos.SqlMonitor.Shared.Models.Database.Enums;

namespace EtherGizmos.SqlMonitor.Shared.Models.Communication;

public class SshConfiguration
{
    public string? HostName { get; set; }

    public int Port { get; set; }

    public string? FilePath { get; set; }

    public SshAuthenticationType AuthenticationType { get; set; }

    public string? Username { get; set; }

    public string? Password { get; set; }

    public string? PrivateKey { get; set; }

    public string? PrivateKeyPassword { get; set; }

    public string? Command { get; set; }

    public string? Arguments { get; set; }
}
