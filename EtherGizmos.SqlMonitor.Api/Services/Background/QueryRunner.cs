using EtherGizmos.SqlMonitor.Api.Extensions;
using EtherGizmos.SqlMonitor.Api.Helpers;
using EtherGizmos.SqlMonitor.Api.Services.Abstractions;
using EtherGizmos.SqlMonitor.Models.Database;
using Microsoft.Data.SqlClient;
using System.Text.RegularExpressions;

namespace EtherGizmos.SqlMonitor.Api.Services.Background;

/// <summary>
/// Runs queries against instances on a periodic timer.
/// </summary>
public class QueryRunner : PeriodicBackgroundService
{
    /// <inheritdoc/>
    protected override TimeSpan Period => TimeSpan.FromSeconds(1);

    /// <summary>
    /// The logger to use.
    /// </summary>
    private ILogger Logger { get; }

    /// <summary>
    /// Provides access to services.
    /// </summary>
    private IServiceProvider ServiceProvider { get; }

    /// <summary>
    /// Construct the service.
    /// </summary>
    /// <param name="logger">The logger to use.</param>
    /// <param name="serviceProvider">Provides access to services.</param>
    public QueryRunner(ILogger<QueryRunner> logger, IServiceProvider serviceProvider) : base(logger)
    {
        Logger = logger;
        ServiceProvider = serviceProvider;
    }

    /// <inheritdoc/>
    protected override async Task DoWorkAsync(CancellationToken stoppingToken)
    {
        //Create a scope for services, so scoped services can be fetched
        using var serviceScope = ServiceProvider.CreateScope();
        var serviceProvider = serviceScope.ServiceProvider;

        //Get means for fetching records
        var instanceService = serviceProvider.GetRequiredService<IInstanceService>();
        var queryService = serviceProvider.GetRequiredService<IQueryService>();
        var saveService = serviceProvider.GetRequiredService<ISaveService>();

        //Get all instances and all queries ready to run
        //Queries are ready if their last runtime plus their frequency is less than the current time
        var allInstances = await instanceService.GetOrLoadCacheAsync();
        var allQueries = await queryService.GetOrLoadCacheAsync();

        var now = DateTimeOffset.UtcNow;
        var queries = allQueries
            //Convert the runtime to an age based on the current time
            .Where(e => now - (e.LastRunAtUtc ?? DateTimeOffset.MinValue)
                //Compare to the run frequency, but shave off a small portion to avoid waiting a whole extra run cycle if
                //the cycle is off by a few milliseconds
                >= e.RunFrequency - DateAndTimeHelper.Min(0.05 * e.RunFrequency, TimeSpan.FromMilliseconds(100)))
            .ToList();

        var instanceQueries = allInstances.CrossJoin(queries);

        var parallelOptions = new ParallelOptions()
        {
            CancellationToken = stoppingToken,
            MaxDegreeOfParallelism = 8
        };

        await Parallel.ForEachAsync(instanceQueries, parallelOptions, async (instanceQuery, cancellationToken) =>
        {
            var instance = instanceQuery.Item1;
            saveService.Attach(instance);

            var query = instanceQuery.Item2;
            saveService.Attach(query);

            await RunInstanceQuery(instanceQuery.Item1, instanceQuery.Item2, cancellationToken);
        });

        //Save changes to the modified records
        await saveService.SaveChangesAsync();
    }

    /// <summary>
    /// Opens a connection to an instance and runs a query on it.
    /// </summary>
    /// <param name="instance">The instance on which to run the query.</param>
    /// <param name="query">The query to run.</param>
    /// <param name="cancellationToken">The cancellation instruction.</param>
    /// <returns>An awaitable task.</returns>
    private async Task RunInstanceQuery(Instance instance, Query query, CancellationToken cancellationToken)
    {
        Logger.Log(LogLevel.Information, "Running query {QueryName} on instance {InstanceName}", query.Name, instance.Name);

        using var connection = await ConnectToInstanceAsync(instance);
        using var command = new SqlCommand(null, connection);

        command.CommandText = GenerateLoadForQuery(query);
        await command.ExecuteLoggedNonQueryAsync(Logger, cancellationToken);

        command.CommandText = GenerateReadForQuery(query);
        using (var reader = await command.ExecuteLoggedReaderAsync(Logger, cancellationToken))
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
        await command.ExecuteLoggedNonQueryAsync(Logger, cancellationToken);

        query.LastRunAtUtc = DateTimeOffset.UtcNow;
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
        var finalFrom = fromRegex.Matches(queryBase).Last();

        var newLineSpace = finalFrom.Groups["SPACE"].Value;

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
