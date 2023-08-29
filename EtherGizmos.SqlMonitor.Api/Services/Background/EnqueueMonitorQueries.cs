using EtherGizmos.SqlMonitor.Api.Consumers;
using EtherGizmos.SqlMonitor.Api.Extensions;
using EtherGizmos.SqlMonitor.Api.Helpers;
using EtherGizmos.SqlMonitor.Api.Services.Background.Abstractions;
using EtherGizmos.SqlMonitor.Api.Services.Caching;
using EtherGizmos.SqlMonitor.Api.Services.Caching.Abstractions;
using EtherGizmos.SqlMonitor.Api.Services.Data.Abstractions;
using EtherGizmos.SqlMonitor.Models.Database;
using MassTransit;
using Medallion.Threading;
using NCrontab;

namespace EtherGizmos.SqlMonitor.Api.Services.Background;

/// <summary>
/// Runs queries against instances on a periodic timer.
/// </summary>
public class EnqueueMonitorQueries : GlobalBackgroundService
{
    private const string CronExpression = "0/15 * * * * *";
    private const string SubCronExpression = "0/1 * * * * *";

    private readonly ILogger _logger;
    private readonly IServiceProvider _serviceProvider;

    private readonly CrontabSchedule _subSchedule;

    /// <summary>
    /// Construct the service.
    /// </summary>
    /// <param name="logger">The logger to use.</param>
    /// <param name="serviceProvider">Provides access to services.</param>
    public EnqueueMonitorQueries(
        ILogger<EnqueueMonitorQueries> logger,
        IServiceProvider serviceProvider,
        IDistributedLockProvider lockProvider)
        : base(logger, lockProvider, CronExpression)
    {
        _logger = logger;
        _serviceProvider = serviceProvider;
        _subSchedule = CrontabSchedule.Parse(SubCronExpression, new CrontabSchedule.ParseOptions()
        {
            IncludingSeconds = true
        });
    }

    /// <inheritdoc/>
    protected override async Task DoGlobalWorkAsync(CancellationToken stoppingToken)
    {
        var scope = _serviceProvider.CreateScope().ServiceProvider;

        var queryService = scope.GetRequiredService<IQueryService>();
        var allQueries = await queryService.GetOrLoadCacheAsync(stoppingToken);

        var instanceService = scope.GetRequiredService<IInstanceService>();
        var allInstances = await instanceService.GetOrLoadCacheAsync(stoppingToken);

        var startTime = DateTime.UtcNow;
        var nextOccurrence = Schedule.GetNextOccurrence(startTime);

        var subOccurrences = _subSchedule.GetNextOccurrences(startTime.AddMilliseconds(100), nextOccurrence.AddMilliseconds(100));
        await Parallel.ForEachAsync(subOccurrences, async (occurrence, cancellationToken) =>
        {
            await Task.Delay(occurrence - DateTime.UtcNow, cancellationToken);

            _logger.Log(LogLevel.Information, "{ServiceName} - Scheduling queries", GetType().Name);
            await ScheduleQueriesAsync(scope, allInstances, allQueries, cancellationToken);
        });
    }

    private async Task ScheduleQueriesAsync(
        IServiceProvider scope,
        IEnumerable<Instance> allInstances,
        IEnumerable<Query> allQueries,
        CancellationToken cancellationToken)
    {
        var now = DateTime.UtcNow;
        var queries = allQueries
            //Convert the runtime to an age based on the current time.
            .Where(e => now - (e.LastRunAtUtc ?? DateTimeOffset.MinValue)
                //Compare to the run frequency, but shave off a small portion to avoid waiting a whole extra run cycle if
                //the cycle is off by a few milliseconds.
                >= e.RunFrequency - DateAndTimeHelper.Min(0.05 * e.RunFrequency, TimeSpan.FromMilliseconds(100)));

        //Prepare to run only the queries due to be run, on all instances.
        //TODO: account for query whitelists/blacklists.
        var instanceQueries = allInstances.CrossJoin(queries);

        var sendEndpointProvider = scope.GetRequiredService<ISendEndpointProvider>();
        var endpoint = await sendEndpointProvider.GetSendEndpoint(new Uri($"queue:{RunQueryConsumer.Queue}"));
        var saveService = scope.GetRequiredService<ISaveService>();
        await Parallel.ForEachAsync(
            instanceQueries,
            new ParallelOptions()
            {
                CancellationToken = cancellationToken,
                MaxDegreeOfParallelism = 8
            },
            async (instanceQuery, cancellationToken) =>
            {
                await endpoint.Send(new RunQuery()
                {
                    InstanceId = instanceQuery.Item1.Id,
                    QueryId = instanceQuery.Item2.Id,
                });

                instanceQuery.Item2.LastRunAtUtc = DateTimeOffset.UtcNow;
                await saveService.SaveChangesAsync();
            });

        var distributedCache = scope.GetRequiredService<ILockedDistributedCache>();
        using var @lock = await distributedCache.AcquireLockAsync(CacheKeys.AllQueries, TimeSpan.FromSeconds(1), cancellationToken);
        if (@lock is not null)
        {
            await distributedCache.SetWithLockAsync(CacheKeys.AllQueries, @lock, allQueries.ToList(), cancellationToken);
        }
        else
        {
            _logger.Log(LogLevel.Warning, "Expected to receive a lock for {CacheKey}, but the request timed out.", nameof(CacheKeys.AllQueries));
        }
    }

    //  /// <inheritdoc/>
    //  protected override async Task DoWorkAsync(CancellationToken stoppingToken)
    //  {
    //      //Create a scope for services, so scoped services can be fetched
    //      using var serviceScope = _serviceProvider.CreateScope();
    //      var serviceProvider = serviceScope.ServiceProvider;

    //      //Get means for fetching records
    //      var instanceService = serviceProvider.GetRequiredService<IInstanceService>();
    //      var queryService = serviceProvider.GetRequiredService<IQueryService>();
    //      var saveService = serviceProvider.GetRequiredService<ISaveService>();

    //      //Get all instances and all queries ready to run
    //      //Queries are ready if their last runtime plus their frequency is less than the current time
    //      var allInstances = await instanceService.GetOrLoadCacheAsync();
    //      var allQueries = await queryService.GetOrLoadCacheAsync();

    //      var now = DateTimeOffset.UtcNow;
    //      var queries = allQueries
    //          //Convert the runtime to an age based on the current time
    //          .Where(e => now - (e.LastRunAtUtc ?? DateTimeOffset.MinValue)
    //              //Compare to the run frequency, but shave off a small portion to avoid waiting a whole extra run cycle if
    //              //the cycle is off by a few milliseconds
    //              >= e.RunFrequency - DateAndTimeHelper.Min(0.05 * e.RunFrequency, TimeSpan.FromMilliseconds(100)))
    //          .ToList();

    //      var instanceQueries = allInstances.CrossJoin(queries);

    //      var parallelOptions = new ParallelOptions()
    //      {
    //          CancellationToken = stoppingToken,
    //          MaxDegreeOfParallelism = 8
    //      };

    //      await Parallel.ForEachAsync(instanceQueries, parallelOptions, async (instanceQuery, cancellationToken) =>
    //      {
    //          var instance = instanceQuery.Item1;
    //          saveService.Attach(instance);

    //          var query = instanceQuery.Item2;
    //          saveService.Attach(query);

    //          await RunInstanceQuery(instanceQuery.Item1, instanceQuery.Item2, cancellationToken);
    //      });

    //      //Save changes to the modified records
    //      await saveService.SaveChangesAsync();
    //  }

    //  /// <summary>
    //  /// Opens a connection to an instance and runs a query on it.
    //  /// </summary>
    //  /// <param name="instance">The instance on which to run the query.</param>
    //  /// <param name="query">The query to run.</param>
    //  /// <param name="cancellationToken">The cancellation instruction.</param>
    //  /// <returns>An awaitable task.</returns>
    //  private async Task RunInstanceQuery(Instance instance, Query query, CancellationToken cancellationToken)
    //  {
    //      _logger.Log(LogLevel.Information, "Running query {QueryName} on instance {InstanceName}", query.Name, instance.Name);

    //      using var connection = await ConnectToInstanceAsync(instance);
    //      using var command = new SqlCommand(null, connection);

    //      command.CommandText = GenerateLoadForQuery(query);
    //      await command.ExecuteLoggedNonQueryAsync(_logger, cancellationToken);

    //      command.CommandText = GenerateReadForQuery(query);
    //      using (var reader = await command.ExecuteLoggedReaderAsync(_logger, cancellationToken))
    //      {
    //          while (await reader.ReadAsync())
    //          {
    //              var values = new object[reader.FieldCount];
    //              int fieldCount = reader.GetValues(values);

    //              var logContent = string.Join(", ", values.Select(e => e?.ToString()));
    //              _logger.Log(LogLevel.Information, logContent);
    //          }
    //      }

    //      command.CommandText = GenerateDropForQuery(query);
    //      await command.ExecuteLoggedNonQueryAsync(_logger, cancellationToken);

    //      query.LastRunAtUtc = DateTimeOffset.UtcNow;
    //  }

    //  /// <summary>
    //  /// Creates a connection for a given instance.
    //  /// </summary>
    //  /// <param name="instance">The instance to which to connect.</param>
    //  /// <returns>The connection.</returns>
    //  /// <exception cref="ArgumentNullException"></exception>
    //  private async Task<SqlConnection> ConnectToInstanceAsync(Instance instance)
    //  {
    //      if (instance is null)
    //          throw new ArgumentNullException(nameof(instance));

    //      var builder = new SqlConnectionStringBuilder();
    //      builder.DataSource = instance.Address;
    //      if ((instance.Port ?? 1433) != 1433)
    //      {
    //          builder.DataSource += $",{instance.Port}";
    //      }
    //      builder.InitialCatalog = instance.Database ?? "master";
    //      builder.ApplicationName = "SQL Monitor";

    //      var connectionString = builder.ToString();

    //      var connection = new SqlConnection(connectionString);
    //      await connection.OpenAsync();

    //      return connection;
    //  }

    //  /// <summary>
    //  /// Creates the load step for a given query.
    //  /// </summary>
    //  /// <param name="query">The query.</param>
    //  /// <returns>The load SQL.</returns>
    //  private string GenerateLoadForQuery(Query query)
    //  {
    //      var queryBase = query.SqlText;

    //      var fromRegex = new Regex(@"(?'SPACE'[\r\n \t]*)from[\s\[]");
    //      var finalFrom = fromRegex.Matches(queryBase).Last();

    //      var newLineSpace = finalFrom.Groups["SPACE"].Value;

    //      var queryText = queryBase.Substring(0, finalFrom.Index) +
    //          newLineSpace +
    //          "into #metric_result" +
    //          queryBase.Substring(finalFrom.Index);

    //      return queryText;
    //  }

    //  /// <summary>
    //  /// Creates the read step for a given query.
    //  /// </summary>
    //  /// <param name="query">The query.</param>
    //  /// <returns>The read SQL.</returns>
    //  private string GenerateReadForQuery(Query query)
    //  {
    //      var bucketExpression = query.BucketExpression ?? "null";
    //      var timestampExpression = query.TimestampUtcExpression ?? "getutcdate()";

    //      var queryText = $@"select {timestampExpression} as [timestamp_utc],
    //{bucketExpression} as [bucket_name],
    //[m].*
    //from #metric_result [m];";

    //      return queryText;
    //  }

    //  /// <summary>
    //  /// Creates the drop step for a given query.
    //  /// </summary>
    //  /// <param name="query">The query.</param>
    //  /// <returns>The drop SQL.</returns>
    //  private string GenerateDropForQuery(Query query)
    //  {
    //      var queryText = "drop table #metric_result;";

    //      return queryText;
    //  }
}
