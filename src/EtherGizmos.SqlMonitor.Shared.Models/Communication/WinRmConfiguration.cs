using EtherGizmos.SqlMonitor.Shared.Models.Database.Enums;

namespace EtherGizmos.SqlMonitor.Shared.Models.Communication;

public class WinRmConfiguration
{
    public string Protocol { get; set; }

    public string HostName { get; set; }

    public int Port { get; set; }

    public string FilePath { get; set; }

    public WinRmAuthenticationType AuthenticationType { get; set; }

    public string? Username { get; set; }

    public string? Password { get; set; }

    public string Command { get; set; }

    public string Arguments { get; set; }

    /// <summary>
    /// Not intended for direct use.
    /// </summary>
    public WinRmConfiguration()
    {
        Protocol = null!;
        HostName = null!;
        FilePath = null!;
        Command = null!;
        Arguments = null!;
    }

    public WinRmConfiguration(
        string protocol,
        string hostName,
        int port,
        string filePath,
        WinRmAuthenticationType authenticationType,
        string? username,
        string? password,
        string command,
        string arguments)
    {
        Protocol = protocol;
        HostName = hostName;
        Port = port;
        FilePath = filePath;
        AuthenticationType = authenticationType;
        Username = username;
        Password = password;
        Command = command;
        Arguments = arguments;
    }
}
