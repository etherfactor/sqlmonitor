using EtherGizmos.SqlMonitor.Api.Data.Access;
using EtherGizmos.SqlMonitor.Api.Services.Abstractions;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace EtherGizmos.SqlMonitor.Api.Services.Background;

public class QueryRunner : PeriodicBackgroundService
{
    protected override TimeSpan Period => TimeSpan.FromSeconds(15);

    private ILogger Logger { get; }

    private IServiceProvider ServiceProvider { get; }

    public QueryRunner(ILogger<QueryRunner> logger, IServiceProvider serviceProvider) : base(logger)
    {
        Logger = logger;
        ServiceProvider = serviceProvider;

        Logger.Log(LogLevel.Information, "Instantiated");
    }

    protected override async Task DoWorkAsync(CancellationToken stoppingToken)
    {
        Logger.Log(LogLevel.Information, "Running");

        using var serviceScope = ServiceProvider.CreateScope();
        var serviceProvider = serviceScope.ServiceProvider;

        using var context = serviceProvider.GetRequiredService<DatabaseContext>();

        var instances = await context.Instances.ToListAsync();
        var queries = await context.Queries.ToListAsync();
        foreach (var instance in instances)
        {
            Logger.Log(LogLevel.Information, "Running queries on instance {InstanceName}", instance.Name);

            var builder = new SqlConnectionStringBuilder();
            builder.DataSource = instance.Address;
            if ((instance.Port ?? 1433) != 1433)
            {
                builder.DataSource += $",{instance.Port}";
            }
            builder.InitialCatalog = instance.Database ?? "master";
            builder.ApplicationName = "SQL Monitor";

            var connectionString = builder.ToString();

            using var connection = new SqlConnection(connectionString);
            await connection.OpenAsync();
            foreach (var query in queries)
            {
                Logger.Log(LogLevel.Information, "Running query {QueryName}", query.Name);

                var queryBase = query.SqlText;

                var bucketExpression = query.BucketExpression ?? "null";
                var timestampExpression = query.TimestampUtcExpression ?? "getutcdate()";

                var fromRegex = new Regex(@"(?'SPACE'[\r\n \t]*)from[\s\[]");
                var finalFrom = fromRegex.Matches(queryBase).Last();

                var newLineSpace = finalFrom.Groups["SPACE"].Value;

                string queryText;
                Stopwatch queryWatch;
                long queryDuration;

                using var command = new SqlCommand(null, connection);

                //Run user-specified query and load into table
                queryText = queryBase.Substring(0, finalFrom.Index) +
                    newLineSpace +
                    "into #metric_result" +
                    queryBase.Substring(finalFrom.Index);
                command.CommandText = queryText;

                Logger.Log(LogLevel.Information, @"Executing query
{QueryText}", queryText);

                queryWatch = Stopwatch.StartNew();
                await command.ExecuteNonQueryAsync();
                queryDuration = queryWatch.ElapsedMilliseconds;

                Logger.Log(LogLevel.Information, @"Executed query ({QueryDuration}ms)
{QueryText}", queryDuration, queryText);

                //Select results out of result table
                queryText = $@"select {timestampExpression} as [timestamp_utc],
  {bucketExpression} as [bucket_name],
  [m].*
  from #metric_result [m];";
                command.CommandText = queryText;

                Logger.Log(LogLevel.Information, @"Executing query
{QueryText}", queryText);

                queryWatch = Stopwatch.StartNew();
                using (var reader = await command.ExecuteReaderAsync())
                {
                    queryDuration = queryWatch.ElapsedMilliseconds;

                    Logger.Log(LogLevel.Information, @"Executed query ({QueryDuration}ms)
{QueryText}", queryDuration, queryText);

                    while (await reader.ReadAsync())
                    {
                        var values = new object[reader.FieldCount];
                        int fieldCount = reader.GetValues(values);

                        var logContent = string.Join(", ", values.Select(e => e?.ToString()));
                        Logger.Log(LogLevel.Information, logContent);
                    }
                }

                //Drop result table
                queryText = "drop table #metric_result;";
                command.CommandText = queryText;

                Logger.Log(LogLevel.Information, @"Executing query
{QueryText}", queryText);

                queryWatch = Stopwatch.StartNew();
                await command.ExecuteNonQueryAsync();
                queryDuration = queryWatch.ElapsedMilliseconds;

                Logger.Log(LogLevel.Information, @"Executed query ({QueryDuration}ms)
{QueryText}", queryDuration, queryText);
            }
        }
    }
}
