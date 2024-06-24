using EtherGizmos.SqlMonitor.Shared.Models.Database.Enums;

namespace EtherGizmos.SqlMonitor.Shared.Models.Communication;

public class SshConfiguration
{
    public string HostName { get; set; }

    public int Port { get; set; }

    public string FilePath { get; set; }

    public SshAuthenticationType AuthenticationType { get; set; }

    public string Username { get; set; }

    public string? Password { get; set; }

    public string? PrivateKey { get; set; }

    public string? PrivateKeyPassword { get; set; }

    public string Command { get; set; }

    public string Arguments { get; set; }

    /// <summary>
    /// Not intended for direct use.
    /// </summary>
    public SshConfiguration()
    {
        HostName = null!;
        FilePath = null!;
        Username = null!;
        Command = null!;
        Arguments = null!;
    }

    public SshConfiguration(
        string hostName,
        int port,
        string filePath,
        SshAuthenticationType authenticationType,
        string username,
        string? password,
        string? privateKey,
        string? privateKeyPassword,
        string command,
        string arguments)
    {
        HostName = hostName;
        Port = port;
        FilePath = filePath;
        AuthenticationType = authenticationType;
        Username = username;
        Password = password;
        PrivateKey = privateKey;
        PrivateKeyPassword = privateKeyPassword;
        Command = command;
        Arguments = arguments;
    }
}
