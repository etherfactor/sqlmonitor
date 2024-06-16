using EtherGizmos.SqlMonitor.Agent.Core.Services.Pooling;
using EtherGizmos.SqlMonitor.Shared.Models.Communication;
using EtherGizmos.SqlMonitor.Shared.Models.Database.Enums;
using Renci.SshNet;
using System.Text;

namespace EtherGizmos.SqlMonitor.Agent.Core.Services.Scripts;

internal class SshConnectionPool : ServicePool<SshClient>
{
    private readonly SshConfiguration _configuration;

    public SshConnectionPool(
        SshConfiguration configuration,
        int minPoolSize,
        int maxPoolSize) : base(
            () => CreateSshClient(configuration),
            minPoolSize,
            maxPoolSize)
    {
        _configuration = configuration;
    }

    /// <summary>
    /// Creates an SSH client.
    /// </summary>
    /// <param name="configuration">The SSH configuration.</param>
    /// <returns>The SSH client.</returns>
    private static async Task<SshClient> CreateSshClient(SshConfiguration configuration)
    {
        var authentication = GetAuthenticationMethod(configuration);
        var connectionInfo = new ConnectionInfo(configuration.HostName, configuration.Port, configuration.Username, authentication);
        var client = new SshClient(connectionInfo);

        await client.ConnectAsync(default);
        return client;
    }

    /// <summary>
    /// Creates an authentication configuration based on the SSH configuration.
    /// </summary>
    /// <param name="configuration">The SSH configuration.</param>
    /// <returns>The authentication configuration.</returns>
    /// <exception cref="NotSupportedException"></exception>
    private static AuthenticationMethod GetAuthenticationMethod(SshConfiguration configuration)
    {
        AuthenticationMethod authentication;
        if (configuration.AuthenticationType == SshAuthenticationType.None)
        {
            authentication = new NoneAuthenticationMethod(
                configuration.Username);
        }
        else if (configuration.AuthenticationType == SshAuthenticationType.Password)
        {
            authentication = new PasswordAuthenticationMethod(
                configuration.Username,
                configuration.Password);
        }
        else if (configuration.AuthenticationType == SshAuthenticationType.PrivateKey)
        {
            var privateKeyBytes = Encoding.UTF8.GetBytes(configuration.PrivateKey ?? "");
            var privateKeyStream = new MemoryStream(privateKeyBytes);

            if (configuration.PrivateKeyPassword is not null)
            {
                authentication = new PrivateKeyAuthenticationMethod(
                    configuration.Username,
                    new PrivateKeyFile(privateKeyStream, configuration.PrivateKeyPassword));
            }
            else
            {
                authentication = new PrivateKeyAuthenticationMethod(
                    configuration.Username,
                    new PrivateKeyFile(privateKeyStream));
            }
        }
        else
        {
            throw new NotSupportedException($"The authentication type {configuration.AuthenticationType} is not supported.");
        }

        return authentication;
    }
}
