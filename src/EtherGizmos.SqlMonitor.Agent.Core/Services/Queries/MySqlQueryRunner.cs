using EtherGizmos.SqlMonitor.Agent.Core.Helpers;
using EtherGizmos.SqlMonitor.Agent.Core.Services.Pooling.Abstractions;
using EtherGizmos.SqlMonitor.Agent.Core.Services.Queries.Abstractions;
using EtherGizmos.SqlMonitor.Shared.Messaging.Messages;
using EtherGizmos.SqlMonitor.Shared.Utilities.Extensions;
using Microsoft.Extensions.Logging;
using MySqlConnector;
using System.Diagnostics;

namespace EtherGizmos.SqlMonitor.Agent.Core.Services.Queries;

/// <summary>
/// Executes queries against a MySQL database.
/// </summary>
internal class MySqlQueryRunner : IQueryRunner, IDisposable
{
    private readonly ILogger _logger;
    private readonly ITicket<MySqlConnection> _connectionTicket;
    private bool _disposed;

    public MySqlQueryRunner(
        ILogger<MySqlQueryRunner> logger,
        ITicket<MySqlConnection> connectionTicket)
    {
        _logger = logger;
        _connectionTicket = connectionTicket;
    }

    /// <inheritdoc/>
    public async Task<QueryResultMessage> ExecuteAsync(
        QueryExecuteMessage queryMessage,
        CancellationToken cancellationToken = default)
    {
        var connection = _connectionTicket.Service;

        using var command = connection.CreateCommand();
        command.CommandText = queryMessage.Text;

        var executedAtUtc = DateTimeOffset.UtcNow;

        //Create a stopwatch so we know how long the script took to run
        var stopwatch = new Stopwatch();
        stopwatch.Start();

        using var reader = await command.ExecuteLoggedReaderAsync(_logger, cancellationToken);
        var executionMilliseconds = stopwatch.ElapsedMilliseconds;
        if (stopwatch.IsRunning && executionMilliseconds == 0)
            executionMilliseconds++;

        var result = new QueryResultMessage(
            queryMessage.QueryId,
            queryMessage.Name,
            queryMessage.MonitoredQueryTargetId,
            executionMilliseconds);

        await MessageHelper.ReadIntoMessageAsync(queryMessage, result, reader, executedAtUtc);

        return result;
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                _connectionTicket.Dispose();
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
