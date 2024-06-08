using EtherGizmos.SqlMonitor.Agent.Core.Helpers;
using EtherGizmos.SqlMonitor.Agent.Core.Services.Scripts.Abstractions;
using EtherGizmos.SqlMonitor.Shared.Messaging.Messages;
using EtherGizmos.SqlMonitor.Shared.Models.Communication;
using EtherGizmos.SqlMonitor.Shared.Models.Database.Enums;
using Renci.SshNet;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;

namespace EtherGizmos.SqlMonitor.Agent.Core.Services.Scripts;

/// <summary>
/// Executes scripts against servers, connecting via SSH.
/// </summary>
internal partial class SshScriptRunner : IScriptRunner
{
    private readonly SshConfiguration _configuration;

    public SshScriptRunner(
        SshConfiguration configuration)
    {
        _configuration = configuration;
    }

    /// <inheritdoc/>
    public async Task<ScriptResultMessage> ExecuteAsync(
        ScriptExecuteMessage scriptMessage,
        CancellationToken cancellationToken = default)
    {
        using var client = GetClient(_configuration);
        await client.ConnectAsync(cancellationToken);

        //Create a stopwatch so we know how long the script took to run
        var stopwatch = new Stopwatch();
        stopwatch.Start();

        var output = ExecuteScript(client, _configuration, scriptMessage);
        var executionMilliseconds = stopwatch.ElapsedMilliseconds;

        var result = new ScriptResultMessage(
            scriptMessage.ScriptId,
            scriptMessage.Name,
            scriptMessage.MonitoredScriptTargetId,
            executionMilliseconds);

        await MessageHelper.ReadIntoMessageAsync(scriptMessage, result, output);

        return result;
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

        var connectionInfo = new ConnectionInfo(configuration.HostName, configuration.Port, configuration.Username, authentication);
        var client = new SshClient(connectionInfo);

        return client;
    }

    /// <summary>
    /// Executes a script against the server.
    /// </summary>
    /// <param name="client">The SSH client.</param>
    /// <param name="configuration">The SSH configuration.</param>
    /// <param name="scriptMessage">The script to execute.</param>
    /// <returns>The console output of the script.</returns>
    internal string ExecuteScript(SshClient client, SshConfiguration configuration, ScriptExecuteMessage scriptMessage)
    {
        //Set the script to a variable
        var script = scriptMessage.Text;
        var scriptVariableName = "script";
        var scriptVariable = EncodeScriptToVariable(scriptVariableName, script);

        var scriptHash = CalculateMd5Checksum(script);

        var scriptExtension = scriptMessage.Interpreter.Extension;

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
}
