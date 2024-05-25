using EtherGizmos.SqlMonitor.Agent.Services.Scripts.Abstractions;
using EtherGizmos.SqlMonitor.Api.Services.Scripts;
using Renci.SshNet;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;

namespace EtherGizmos.SqlMonitor.Agent.Services.Scripts;

/// <summary>
/// Executes scripts against servers, connecting via SSH.
/// </summary>
public partial class SshScriptRunner : IScriptRunner
{
    /// <inheritdoc/>
    public async Task<ScriptExecutionResultSet> ExecuteAsync(
        MonitoredScriptTarget scriptTarget,
        ScriptVariant scriptVariant,
        CancellationToken cancellationToken = default)
    {
        var configuration = GetSshConfiguration(scriptTarget, scriptVariant);

        using var client = GetClient(configuration);
        await client.ConnectAsync(CancellationToken.None);

        //Create a stopwatch so we know how long the script took to run
        var stopwatch = new Stopwatch();
        stopwatch.Start();

        var output = ExecuteScript(client, configuration, scriptVariant);
        var executionMilliseconds = stopwatch.ElapsedMilliseconds;

        return ScriptExecutionResultSet.FromResults(scriptTarget, scriptVariant, output, executionMilliseconds);
    }

    /// <summary>
    /// Creates an SSH configuration based on the specified server and script.
    /// </summary>
    /// <param name="scriptTarget">The target server.</param>
    /// <param name="scriptVariant">The target script.</param>
    /// <returns>The SSH configuration.</returns>
    internal SshConfiguration GetSshConfiguration(MonitoredScriptTarget scriptTarget, ScriptVariant scriptVariant)
    {
        var hostName = scriptTarget.HostName;
        var port = scriptTarget.Port ?? 22;
        var filePath = scriptTarget.RunInPath;

        var username = scriptTarget.SshUsername;
        var password = scriptTarget.SshPassword;

        var privateKey = scriptTarget.SshPrivateKey;
        var privateKeyPassword = scriptTarget.SshPrivateKeyPassword;

        var command = scriptVariant.ScriptInterpreter.Command;
        var arguments = scriptVariant.ScriptInterpreter.Arguments;

        var config = new SshConfiguration()
        {
            HostName = hostName,
            Port = port,
            FilePath = filePath,

            AuthenticationType = scriptTarget.SshAuthenticationType ?? SshAuthenticationType.Unknown,

            Username = username ?? throw new InvalidOperationException("Must specify a username"),
            Password = password,

            PrivateKey = privateKey,
            PrivateKeyPassword = privateKeyPassword,

            Command = command,
            Arguments = arguments,
        };

        return config;
    }

    /// <summary>
    /// Creates an authentication configuration based on the SSH configuration.
    /// </summary>
    /// <param name="configuration">The SSH configuration.</param>
    /// <returns>The authentication configuration.</returns>
    /// <exception cref="NotSupportedException"></exception>
    internal AuthenticationMethod GetAuthenticationMethod(SshConfiguration configuration)
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

    /// <summary>
    /// Create an SSH client.
    /// </summary>
    /// <param name="configuration">The SSH configuration.</param>
    /// <returns>The SSH client.</returns>
    internal SshClient GetClient(SshConfiguration configuration)
    {
        var authentication = GetAuthenticationMethod(configuration);

        var connectionInfo = new Renci.SshNet.ConnectionInfo(configuration.HostName, configuration.Port, configuration.Username, authentication);
        var client = new SshClient(connectionInfo);

        return client;
    }

    /// <summary>
    /// Executes a script against the server.
    /// </summary>
    /// <param name="client">The SSH client.</param>
    /// <param name="configuration">The SSH configuration.</param>
    /// <param name="scriptVariant">The script to execute.</param>
    /// <returns>The console output of the script.</returns>
    internal string ExecuteScript(SshClient client, SshConfiguration configuration, ScriptVariant scriptVariant)
    {
        //Set the script to a variable
        var script = scriptVariant.ScriptText;
        var scriptVariableName = "script";
        var scriptVariable = EncodeScriptToVariable(scriptVariableName, script);

        var scriptHash = CalculateMd5Checksum(script);

        var scriptExtension = scriptVariant.ScriptInterpreter.Extension;

        //Write the script to a file, then execute that file
        var commandText = $@"cd {configuration.FilePath}

{scriptVariable}

echo ""${scriptVariableName}"" > {scriptHash}.{scriptExtension}

{configuration.Command} {configuration.Arguments.Replace("$Script", $"{scriptHash}.{scriptExtension}")}".Replace("\r", "");

        //Run the command, gathering the console output and returning it
        var result = client.RunCommand(commandText)
            .Result;

        return result;
    }

    /// <summary>
    /// Encodes a script to a bash variable.
    /// </summary>
    /// <param name="variableName">The name of the script variable.</param>
    /// <param name="script">The script to encode.</param>
    /// <returns>A setter for the variable.</returns>
    internal string EncodeScriptToVariable(string variableName, string script)
    {
        //Create a setter for a variable
        var variableSet = $@"{variableName}=$(cat << 'EOF'
{script}
EOF
)";

        return variableSet
            .Replace("\r\n", "\n");
    }

    /// <summary>
    /// Calculates an MD5 checksum of a script.
    /// </summary>
    /// <param name="script">The script.</param>
    /// <returns>The MD5 checksum.</returns>
    internal string CalculateMd5Checksum(string script)
    {
        var md5 = MD5.Create();

        var scriptBytes = Encoding.UTF8.GetBytes(script);
        var hashBytes = md5.ComputeHash(scriptBytes);
        var scriptHash = BitConverter.ToString(hashBytes).Replace("-", "").ToLower();

        return scriptHash;
    }

    internal class SshConfiguration
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
}
