using EtherGizmos.SqlMonitor.Api.Services.Scripts.Abstractions;
using EtherGizmos.SqlMonitor.Models.Database;
using System.Diagnostics;
using System.Management.Automation;
using System.Management.Automation.Runspaces;
using System.Security;
using System.Security.Cryptography;
using System.Text;

namespace EtherGizmos.SqlMonitor.Api.Services.Scripts;

public class PSRemotingScriptRunner : IScriptRunner
{
    public async Task<ScriptExecutionResultSet> ExecuteAsync(
        MonitoredScriptTarget scriptTarget,
        ScriptVariant scriptVariant,
        CancellationToken cancellationToken = default)
    {
        var configuration = GetWinRmConfiguration(scriptTarget, scriptVariant);

        using var powershell = GetSession(configuration);

        //Create a stopwatch so we know how long the script took to run
        var stopwatch = new Stopwatch();
        stopwatch.Start();

        var output = await ExecuteScriptAsync(powershell, configuration, scriptVariant);
        var executionMilliseconds = stopwatch.ElapsedMilliseconds;

        return ScriptExecutionResultSet.FromResults(scriptTarget, scriptVariant, output, executionMilliseconds);
    }

    private WinRmConfiguration GetWinRmConfiguration(MonitoredScriptTarget scriptTarget, ScriptVariant scriptVariant)
    {
        var useSsl = scriptTarget.WinRmUseSsl ?? true;
        var protocol = useSsl ? "https" : "http";

        var hostName = scriptTarget.HostName;
        var port = scriptTarget.Port ?? (useSsl ? 5986 : 5985);
        var filePath = scriptTarget.RunInPath;

        var username = scriptTarget.SshUsername;
        var password = scriptTarget.SshPassword;

        var command = scriptVariant.ScriptInterpreter.Command;
        var arguments = scriptVariant.ScriptInterpreter.Arguments;

        var config = new WinRmConfiguration()
        {
            Protocol = protocol,

            HostName = hostName,
            Port = port,
            FilePath = filePath,

            Username = username,
            Password = password,

            Command = command,
            Arguments = arguments,
        };

        return config;
    }

    private PowerShell GetSession(WinRmConfiguration configuration)
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
        connectionInfo.AuthenticationMechanism = AuthenticationMechanism.Default;

        using var runspace = RunspaceFactory.CreateRunspace(connectionInfo);
        runspace.Open();

        using var powershell = PowerShell.Create();
        powershell.Runspace = runspace;

        return powershell;
    }

    private async Task<string> ExecuteScriptAsync(PowerShell session, WinRmConfiguration configuration, ScriptVariant scriptVariant)
    {
        var script = scriptVariant.ScriptText;
        var scriptVariable = EncodeScriptToVariable("Script", script);

        var scriptHash = CalculateMd5Checksum(script);

        var scriptExtension = scriptVariant.ScriptInterpreter.Extension;

        session.AddScript(@$"{scriptVariable}

if (!(Test-Path -Path ""{scriptHash}.{scriptExtension}"")) {{
    Set-Content -Path ""
        {scriptHash}.{scriptExtension}"" -Value $Script | Out-Null
}}

{configuration.Command} {configuration.Arguments.Replace("$Script", $@"""{scriptHash}.{scriptExtension}""")}");

        var result = await session.InvokeAsync();

        return result.ToString()!;
    }

    private string EncodeScriptToVariable(string variableName, string script)
    {
        //Each script line needs to be separated. Each single quote needs to be doubled to be escaped, and the whole text
        //needs to be surrounded by single quotes
        var fragments = script.Replace("\r\n", "\n").Split("\n")
            .Select(e => $"'{e.Replace("'", "''")}'");

        //Combine the fragments, separated by environment-specific new-lines
        var variableValue = string.Join(" + [Environment]::NewLine + ", fragments);

        //Create a setter for a variable
        var variableSet = $"${variableName} = {variableValue}";

        return variableSet;
    }

    private string CalculateMd5Checksum(string script)
    {
        var md5 = MD5.Create();

        var scriptBytes = Encoding.UTF8.GetBytes(script);
        var hashBytes = md5.ComputeHash(scriptBytes);
        var scriptHash = BitConverter.ToString(hashBytes).Replace("-", "").ToLower();

        return scriptHash;
    }

    internal class WinRmConfiguration
    {
        public required string Protocol { get; set; }

        public required string HostName { get; set; }

        public required int Port { get; set; }

        public required string FilePath { get; set; }

        public string? Username { get; set; }

        public string? Password { get; set; }

        public required string Command { get; set; }

        public required string Arguments { get; set; }
    }
}
