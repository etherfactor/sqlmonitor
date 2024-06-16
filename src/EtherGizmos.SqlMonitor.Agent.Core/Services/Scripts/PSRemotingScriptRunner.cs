using EtherGizmos.SqlMonitor.Agent.Core.Helpers;
using EtherGizmos.SqlMonitor.Agent.Core.Services.Pooling.Abstractions;
using EtherGizmos.SqlMonitor.Agent.Core.Services.Scripts.Abstractions;
using EtherGizmos.SqlMonitor.Shared.Messaging.Messages;
using EtherGizmos.SqlMonitor.Shared.Models.Communication;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Management.Automation;
using System.Management.Automation.Runspaces;
using System.Security.Cryptography;
using System.Text;

namespace EtherGizmos.SqlMonitor.Agent.Core.Services.Scripts;

internal class PSRemotingScriptRunner : IScriptRunner, IDisposable
{
    private readonly ILogger _logger;
    private readonly WinRmConfiguration _configuration;
    private readonly ITicket<Runspace> _runspaceTicket;
    private bool _disposed;

    public PSRemotingScriptRunner(
        ILogger<PSRemotingScriptRunner> logger,
        WinRmConfiguration configuration,
        ITicket<Runspace> runspaceTicket)
    {
        _logger = logger;
        _configuration = configuration;
        _runspaceTicket = runspaceTicket;
    }

    public async Task<ScriptResultMessage> ExecuteAsync(
        ScriptExecuteMessage scriptMessage,
        CancellationToken cancellationToken = default)
    {
        using var powershell = PowerShell.Create();
        powershell.Runspace = _runspaceTicket.Service;

        var executedAtUtc = DateTimeOffset.UtcNow;

        //Create a stopwatch so we know how long the script took to run
        var stopwatch = new Stopwatch();
        stopwatch.Start();

        var output = await ExecuteScriptAsync(powershell, _configuration, scriptMessage);
        var executionMilliseconds = stopwatch.ElapsedMilliseconds;

        var result = new ScriptResultMessage(
            scriptMessage.ScriptId,
            scriptMessage.Name,
            scriptMessage.MonitoredScriptTargetId,
            executionMilliseconds);

        await MessageHelper.ReadIntoMessageAsync(scriptMessage, result, output, executedAtUtc);

        return result;
    }

    internal async Task<string> ExecuteScriptAsync(PowerShell session, WinRmConfiguration configuration, ScriptExecuteMessage scriptMessage)
    {
        var outputStringBuilder = new StringBuilder();

        void Verbose_DataAdded(object? sender, DataAddedEventArgs e)
        {
            var verbose = (sender as PSDataCollection<VerboseRecord>)?[e.Index];
            if (verbose is not null)
            {
                outputStringBuilder.AppendLine(verbose.Message);
            }
        }

        void Debug_DataAdded(object? sender, DataAddedEventArgs e)
        {
            var debug = (sender as PSDataCollection<DebugRecord>)?[e.Index];
            if (debug is not null)
            {
                outputStringBuilder.AppendLine(debug.Message);
            }
        }

        void Information_DataAdded(object? sender, DataAddedEventArgs e)
        {
            var information = (sender as PSDataCollection<InformationalRecord>)?[e.Index];
            if (information is not null)
            {
                outputStringBuilder.AppendLine(information.Message);
            }
        }

        void Warning_DataAdded(object? sender, DataAddedEventArgs e)
        {
            var warning = (sender as PSDataCollection<WarningRecord>)?[e.Index];
            if (warning is not null)
            {
                outputStringBuilder.AppendLine(warning.Message);
            }
        }

        void Error_DataAdded(object? sender, DataAddedEventArgs e)
        {
            var error = (sender as PSDataCollection<ErrorRecord>)?[e.Index];
            if (error is not null)
            {
                outputStringBuilder.AppendLine(error.Exception.Message);
            }
        }

        session.Streams.Verbose.DataAdded += Verbose_DataAdded;
        session.Streams.Debug.DataAdded += Debug_DataAdded;
        session.Streams.Information.DataAdded += Information_DataAdded;
        session.Streams.Warning.DataAdded += Warning_DataAdded;
        session.Streams.Error.DataAdded += Error_DataAdded;

        var script = scriptMessage.Text;
        var scriptVariable = EncodeScriptToVariable("Script", script);

        var scriptHash = CalculateMd5Checksum(script);

        var scriptExtension = scriptMessage.Interpreter.Extension;

        var scriptText = @$"{scriptVariable}

if (!(Test-Path -Path ""{scriptHash}.{scriptExtension}"")) {{
    Set-Content -Path ""{scriptHash}.{scriptExtension}"" -Value $Script | Out-Null
}}

{configuration.Command} {configuration.Arguments.Replace("$Script", $@"""{scriptHash}.{scriptExtension}""")}";

        session.AddScript(scriptText);

        var stopwatch = new Stopwatch();
        stopwatch.Start();

        _logger.LogDebug(@"Executing script
{ScriptText}", scriptText);

        PSDataCollection<PSObject> result;
        try
        {
            result = await session.InvokeAsync();
        }
        catch (Exception ex)
        {
            _logger.Log(LogLevel.Error, ex, @"Encountered an unexpected error while running script
{ScriptText}", scriptText);
            throw;
        }

        var scriptDuration = stopwatch.ElapsedMilliseconds;

        _logger.LogInformation(@"Executed script ({Duration}ms)
{ScriptText}", scriptDuration, scriptText);

        foreach (var item in result)
        {
            outputStringBuilder.AppendLine(item.ToString());
        }

        session.Streams.Verbose.DataAdded -= Verbose_DataAdded;
        session.Streams.Debug.DataAdded -= Debug_DataAdded;
        session.Streams.Information.DataAdded -= Information_DataAdded;
        session.Streams.Warning.DataAdded -= Warning_DataAdded;
        session.Streams.Error.DataAdded -= Error_DataAdded;

        return outputStringBuilder.ToString();
    }

    internal string EncodeScriptToVariable(string variableName, string script)
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
                _runspaceTicket.Dispose();
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
