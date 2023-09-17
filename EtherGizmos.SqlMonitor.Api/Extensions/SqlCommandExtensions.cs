using Microsoft.Data.SqlClient;
using System.Diagnostics;

namespace EtherGizmos.SqlMonitor.Api.Extensions;

/// <summary>
/// Provides extension methods for <see cref="SqlCommand"/>.
/// </summary>
public static class SqlCommandExtensions
{
    /// <summary>
    /// Executes a query, logging the duration and returning the results.
    /// </summary>
    /// <param name="this">Itself.</param>
    /// <param name="logger">The logger to use.</param>
    /// <param name="cancellationToken">The cancellation instruction.</param>
    /// <returns>The query results.</returns>
    public static async Task<SqlDataReader> ExecuteLoggedReaderAsync(this SqlCommand @this, ILogger logger, CancellationToken cancellationToken = default)
    {
        logger.Log(LogLevel.Debug, @"Executing query
{QueryText}", @this.CommandText);

        var queryWatch = Stopwatch.StartNew();
        SqlDataReader reader;
        try
        {
            reader = await @this.ExecuteReaderAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            logger.Log(LogLevel.Error, ex, "Encountered an unexpected error while running query {QueryText}", @this.CommandText);
            throw;
        }

        var queryDuration = queryWatch.ElapsedMilliseconds;

        logger.Log(LogLevel.Information, @"Executed query ({QueryDuration}ms)
{QueryText}", queryDuration, @this.CommandText);

        return reader;
    }

    /// <summary>
    /// Executes a query, logging the duration and returning the number of affected records.
    /// </summary>
    /// <param name="this">Itself.</param>
    /// <param name="logger">The logger to use.</param>
    /// <param name="cancellationToken">The cancellation instruction.</param>
    /// <returns>The number of affected records.</returns>
    public static async Task<int> ExecuteLoggedNonQueryAsync(this SqlCommand @this, ILogger logger, CancellationToken cancellationToken = default)
    {
        logger.Log(LogLevel.Debug, @"Executing query
{QueryText}", @this.CommandText);

        var queryWatch = Stopwatch.StartNew();

        int count;
        try
        {
            count = await @this.ExecuteNonQueryAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            logger.Log(LogLevel.Error, ex, "Encountered an unexpected error while running query {QueryText}", @this.CommandText);
            throw;
        }

        var queryDuration = queryWatch.ElapsedMilliseconds;

        logger.Log(LogLevel.Information, @"Executed query ({QueryDuration}ms)
{QueryText}", queryDuration, @this.CommandText);

        return count;
    }
}
