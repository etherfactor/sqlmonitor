using EtherGizmos.SqlMonitor.Agent.Core.Helpers;
using EtherGizmos.SqlMonitor.Agent.Core.Services.Pooling.Abstractions;
using EtherGizmos.SqlMonitor.Agent.Core.Services.Scripts.Abstractions;
using EtherGizmos.SqlMonitor.Shared.Messaging.Messages;
using EtherGizmos.SqlMonitor.Shared.Models.Communication;
using Microsoft.Extensions.Logging;
using Renci.SshNet;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;

namespace EtherGizmos.SqlMonitor.Agent.Core.Services.Scripts;

/// <summary>
/// Executes scripts against servers, connecting via SSH.
/// </summary>
internal partial class SshScriptRunner : IScriptRunner, IDisposable
{
    private readonly ILogger _logger;
    private readonly SshConfiguration _configuration;
    private readonly ITicket<SshClient> _clientTicket;
    private bool _disposed;

    public SshScriptRunner(
        ILogger<SshScriptRunner> logger,
        SshConfiguration configuration,
        ITicket<SshClient> clientTicket)
    {
        _logger = logger;
        _configuration = configuration;
        _clientTicket = clientTicket;
    }

    /// <inheritdoc/>
    public async Task<ScriptResultMessage> ExecuteAsync(
        ScriptExecuteMessage scriptMessage,
        CancellationToken cancellationToken = default)
    {
        var client = _clientTicket.Service;

        var executedAtUtc = DateTimeOffset.UtcNow;

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

        await MessageHelper.ReadIntoMessageAsync(scriptMessage, result, output, executedAtUtc);

        return result;
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

        var stopwatch = new Stopwatch();
        stopwatch.Start();

        _logger.LogDebug(@"Executing script
{ScriptText}", commandText);

        string result;
        try
        {
            //Run the command, gathering the console output and returning it
            result = client.RunCommand(commandText)
                .Result;
        }
        catch (Exception ex)
        {
            _logger.Log(LogLevel.Error, ex, @"Encountered an unexpected error while running script
{ScriptText}", commandText);
            throw;
        }

        var scriptDuration = stopwatch.ElapsedMilliseconds;

        _logger.LogInformation(@"Executed script ({Duration}ms)
{ScriptText}", scriptDuration, commandText);

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

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                _clientTicket.Dispose();
            }

            _disposed = true;
        }
    }

    public void Dispose()
    {
        // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}
