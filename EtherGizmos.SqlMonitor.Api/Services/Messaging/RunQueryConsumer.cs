using EtherGizmos.SqlMonitor.Api.Extensions;
using EtherGizmos.SqlMonitor.Api.Services.Caching.Abstractions;
using EtherGizmos.SqlMonitor.Api.Services.Data.Abstractions;
using EtherGizmos.SqlMonitor.Models.Database;
using EtherGizmos.SqlMonitor.Models.Database.Enums;
using MassTransit;
using Microsoft.Data.SqlClient;
using System.Data;
using System.Reflection;

namespace EtherGizmos.SqlMonitor.Api.Services.Messaging;

public class RunQueryConsumer : IConsumer<RunQuery>
{
    public const string Queue = "run-query";

    private readonly ILogger _logger;

    private readonly IServiceProvider _serviceProvider;
    private readonly IDistributedRecordCache _distributedRecordCache;

    public RunQueryConsumer(
        ILogger<RunQueryConsumer> logger,
        IDistributedRecordCache distributedRecordCache,
        IServiceProvider serviceProvider)
    {
        _logger = logger;

        _serviceProvider = serviceProvider;
        _distributedRecordCache = distributedRecordCache;
    }

    public async Task Consume(ConsumeContext<RunQuery> context)
    {
        _logger.Log(LogLevel.Information, "Received data: {Data}", context.Message);

        var message = context.Message;

        var instanceId = message.InstanceId;
        var instance = await _distributedRecordCache
            .EntitySet<Instance>()
            .GetAsync(new object[] { instanceId });

        if (instance is null)
            throw new InvalidOperationException($"Instance ({instanceId}) could not be read from the cache.");

        var queryId = message.QueryId;
        var query = await _distributedRecordCache
            .EntitySet<Query>()
            .GetAsync(new object[] { queryId });

        if (query is null)
            throw new InvalidOperationException($"Query ({queryId}) could not be read from the cache.");

        if (!query.Metrics.Any())
        {
            _logger.Log(LogLevel.Information, "No metrics associated with {QueryName}, so nothing will be run.", query.Name);
            return;
        }

        //Create a service scope for this run
        using var scope = _serviceProvider.CreateScope(); //the act of creating a scope here causes the parent scope to not exist, as this is only defined when the parent scope prompts a child scope to generate
        var provider = scope.ServiceProvider;

        //Pull out the necessary services
        var instanceMetricsBySecond = provider.GetRequiredService<IInstanceMetricBySecondService>();
        var metricBuckets = provider.GetRequiredService<IMetricBucketService>();
        var saveService = provider.GetRequiredService<ISaveService>();

        _logger.Log(LogLevel.Information, "Running query {QueryName} on instance {InstanceName}", query.Name, instance.Name);

        using var connection = await ConnectToInstanceAsync(instance);
        using var command = GenerateForQuery(connection, query);

        using (var reader = await command.ExecuteLoggedReaderAsync(_logger))
        {
            while (await reader.ReadAsync())
            {
                var utcTimestamp = reader.GetDateTime("__timestamp");

                string bucket = "";
                if (!reader.IsDBNull("__bucket"))
                    bucket = reader.GetString("__bucket");

                for (int m = 0; m < query.Metrics.Count; m++)
                {
                    var metric = query.Metrics[m];

                    double? metricValue = null;
                    if (!reader.IsDBNull($"metric_{m}"))
                        metricValue = reader.GetDouble($"metric_{m}");

                    if (metricValue is null)
                    {
                        _logger.Log(LogLevel.Warning, "Metric {MetricName} returned a null value at {MetricTimestamp}", metric.Metric.Name, utcTimestamp);
                    }
                    else
                    {
                        _logger.Log(LogLevel.Information, "Metric {MetricName} returned {MetricBucket}:{MetricValue} at {MetricTimestamp}", metric.Metric.Name, bucket, metricValue, utcTimestamp);
                    }
                }
            }
        }

        await saveService.SaveChangesAsync();
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
    /// Generates a command to run a <see cref="Query"/> on an active <see cref="Instance"/> connection.
    /// </summary>
    /// <param name="connection">The connection on which to run the query.</param>
    /// <param name="query">The query to run.</param>
    /// <returns>The generated command.</returns>
    /// <exception cref="InvalidOperationException"></exception>
    private SqlCommand GenerateForQuery(SqlConnection connection, Query query)
    {
        var dynamicQueryStream = Assembly.GetExecutingAssembly()
            .GetManifestResourceStream("EtherGizmos.SqlMonitor.Api.Resources.Sql.DynamicQuery.sql");

        if (dynamicQueryStream is null)
            throw new InvalidOperationException("Unable to locate resource stream: EtherGizmos.SqlMonitor.Api.Resources.Sql.DynamicQuery.sql");

        var dynamicQueryReader = new StreamReader(dynamicQueryStream);
        var dynamicQuery = dynamicQueryReader.ReadToEnd();

        var metricStrings = query.Metrics.Select(e => GetAggregateFunction(e.Metric.AggregateType) + ":" + e.ValueExpression.Replace(";", "%3B"));
        var metricString = string.Join(";", metricStrings);

        var command = new SqlCommand(dynamicQuery, connection);
        command.Parameters.Add("@Sql", SqlDbType.NVarChar).Value = query.SqlText;
        command.Parameters.Add("@TimestampUtcExpression", SqlDbType.NVarChar).Value = query.TimestampUtcExpression ?? (object)DBNull.Value;
        command.Parameters.Add("@BucketExpression", SqlDbType.NVarChar).Value = query.BucketExpression ?? (object)DBNull.Value;
        command.Parameters.Add("@MetricValues", SqlDbType.NVarChar).Value = metricString;

        return command;
    }

    /// <summary>
    /// Converts an <see cref="AggregateType"/> to an equivalent aggregate function.
    /// </summary>
    /// <param name="aggregate">The aggregate type to convert.</param>
    /// <returns>The aggregate function.</returns>
    /// <exception cref="InvalidOperationException"></exception>
    private string GetAggregateFunction(AggregateType aggregate)
    {
        switch (aggregate)
        {
            case AggregateType.Average:
                return "avg";

            case AggregateType.Maximum:
                return "max";

            case AggregateType.Minimum:
                return "min";

            case AggregateType.StandardDeviation:
                return "stdev";

            case AggregateType.Sum:
                return "sum";

            case AggregateType.Variance:
                return "var";

            default:
                throw new InvalidOperationException($"Unrecognized aggregate type: {aggregate}");
        }
    }
}
