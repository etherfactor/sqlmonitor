using EtherGizmos.SqlMonitor.Api.Extensions;
using EtherGizmos.SqlMonitor.Models.Database;
using MassTransit;
using Microsoft.Data.SqlClient;
using System.Text.RegularExpressions;

namespace EtherGizmos.SqlMonitor.Api.Consumers;

public class RunQueryConsumer : IConsumer<RunQuery>
{
    public const string Queue = "run-query";

    private ILogger Logger { get; }

    public RunQueryConsumer(ILogger<RunQueryConsumer> logger)
    {
        Logger = logger;
    }

    public async Task Consume(ConsumeContext<RunQuery> context)
    {
        Logger.Log(LogLevel.Information, "Received data: {Data}", context.Message);

        var message = context.Message;

        var instance = message.Instance;
        var query = message.Query;

        Logger.Log(LogLevel.Information, "Running query {QueryName} on instance {InstanceName}", query.Name, instance.Name);

        using var connection = await ConnectToInstanceAsync(instance);
        using var command = new SqlCommand(null, connection);

        command.CommandText = GenerateLoadForQuery(query);
        await command.ExecuteLoggedNonQueryAsync(Logger);

        command.CommandText = GenerateReadForQuery(query);
        using (var reader = await command.ExecuteLoggedReaderAsync(Logger))
        {
            while (await reader.ReadAsync())
            {
                var values = new object[reader.FieldCount];
                int fieldCount = reader.GetValues(values);

                var logContent = string.Join(", ", values.Select(e => e?.ToString()));
                Logger.Log(LogLevel.Information, logContent);
            }
        }

        command.CommandText = GenerateDropForQuery(query);
        await command.ExecuteLoggedNonQueryAsync(Logger);
    }

    /// <summary>
    /// Creates a connection for a given instance.
    /// </summary>
    /// <param name="instance">The instance to which to connect.</param>
    /// <returns>The connection.</returns>
    /// <exception cref="ArgumentNullException"></exception>
    private async Task<SqlConnection> ConnectToInstanceAsync(Instance instance)
    {
        if (instance is null)
            throw new ArgumentNullException(nameof(instance));

        var builder = new SqlConnectionStringBuilder();
        builder.DataSource = instance.Address;
        if ((instance.Port ?? 1433) != 1433)
        {
            builder.DataSource += $",{instance.Port}";
        }
        builder.InitialCatalog = instance.Database ?? "master";
        builder.ApplicationName = "SQL Monitor";

        var connectionString = builder.ToString();

        var connection = new SqlConnection(connectionString);
        await connection.OpenAsync();

        return connection;
    }

    /// <summary>
    /// Creates the load step for a given query.
    /// </summary>
    /// <param name="query">The query.</param>
    /// <returns>The load SQL.</returns>
    private string GenerateLoadForQuery(Query query)
    {
        var queryBase = query.SqlText;

        var fromRegex = new Regex(@"(?'SPACE'[\r\n \t]*)from[\s\[]");
        var finalFrom = fromRegex.Matches(queryBase).LastOrDefault();
        string newLineSpace;

        if (finalFrom is not null)
        {
            newLineSpace = finalFrom.Groups["SPACE"].Value;
        }
        else
        {
            var endRegex = new Regex(@"(?'END';|$)");
            finalFrom = endRegex.Matches(queryBase).First();
            newLineSpace = " ";
        }

        var queryText = queryBase.Substring(0, finalFrom.Index) +
            newLineSpace +
            "into #metric_result" +
            queryBase.Substring(finalFrom.Index);

        return queryText;
    }

    /// <summary>
    /// Creates the read step for a given query.
    /// </summary>
    /// <param name="query">The query.</param>
    /// <returns>The read SQL.</returns>
    private string GenerateReadForQuery(Query query)
    {
        var bucketExpression = query.BucketExpression ?? "null";
        var timestampExpression = query.TimestampUtcExpression ?? "getutcdate()";

        var queryText = $@"select {timestampExpression} as [timestamp_utc],
  {bucketExpression} as [bucket_name],
  [m].*
  from #metric_result [m];";

        return queryText;
    }

    /// <summary>
    /// Creates the drop step for a given query.
    /// </summary>
    /// <param name="query">The query.</param>
    /// <returns>The drop SQL.</returns>
    private string GenerateDropForQuery(Query query)
    {
        var queryText = "drop table #metric_result;";

        return queryText;
    }
}
