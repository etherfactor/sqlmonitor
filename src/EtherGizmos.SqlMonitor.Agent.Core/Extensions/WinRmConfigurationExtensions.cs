using EtherGizmos.SqlMonitor.Shared.Models.Communication;
using EtherGizmos.SqlMonitor.Shared.Models.Database.Enums;
using System.Management.Automation;
using System.Management.Automation.Runspaces;
using System.Security;

namespace EtherGizmos.SqlMonitor.Agent.Core.Extensions;

internal static class WinRmConfigurationExtensions
{
    internal static Task<Runspace> CreateRunspaceAsync(this WinRmConfiguration @this)
    {
        var runspace = GetRunspace(@this);
        runspace.Open();

        return Task.FromResult(runspace);
    }

    internal static Runspace CreateRunspace(this WinRmConfiguration @this)
    {
        var runspace = GetRunspace(@this);
        runspace.Open();

        return runspace;
    }

    private static Runspace GetRunspace(WinRmConfiguration configuration)
    {
        PSCredential credentials;
        if (configuration.Username is not null && configuration.Password is not null)
        {
            var securePassword = new SecureString();
            foreach (var c in configuration.Password)
            {
                securePassword.AppendChar(c);
            }

            credentials = new PSCredential(configuration.Username, securePassword);
        }
        else
        {
            credentials = PSCredential.Empty;
        }

        var connectionInfo = new WSManConnectionInfo(new Uri($"{configuration.Protocol}://{configuration.HostName}:{configuration.Port}"), "http://schemas.microsoft.com/powershell/Microsoft.PowerShell", credentials);
        connectionInfo.AuthenticationMechanism =
            configuration.AuthenticationType == WinRmAuthenticationType.Kerberos ? AuthenticationMechanism.Kerberos
            : configuration.AuthenticationType == WinRmAuthenticationType.Basic ? AuthenticationMechanism.Basic
            : throw new NotSupportedException($"The authentication type {configuration.AuthenticationType} is not supported.");

        var runspace = RunspaceFactory.CreateRunspace(connectionInfo);

        return runspace;
    }
}
