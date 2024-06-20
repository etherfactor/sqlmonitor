using EtherGizmos.SqlMonitor.Shared.Database.Services;
using EtherGizmos.SqlMonitor.Shared.Messaging.Messages;
using EtherGizmos.SqlMonitor.Shared.Models.Database;
using EtherGizmos.SqlMonitor.Shared.Models.Database.Enums;
using EtherGizmos.SqlMonitor.Shared.Redis.Caching.Abstractions;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace EtherGizmos.SqlMonitor.Api.Core.Services.Messaging;

public class QueryResultConsumer : IConsumer<QueryResultMessage>
{
    private readonly ILogger _logger;
    private readonly IServiceProvider _serviceProvider;
    private readonly ApplicationContext _context;
    private readonly IDistributedRecordCache _cache;

    public QueryResultConsumer(
        ILogger<QueryResultConsumer> logger,
        IServiceProvider serviceProvider,
        ApplicationContext context,
        IDistributedRecordCache cache)
    {
        _logger = logger;
        _serviceProvider = serviceProvider;
        _context = context;
        _cache = cache;
    }

    public async Task Consume(ConsumeContext<QueryResultMessage> context)
    {
        var message = context.Message;

        _logger.LogInformation("Processing message {@QueryResultMessage}", message);

        var queryTargetSet = _cache.EntitySet<MonitoredQueryTarget>();
        var queryTarget = await queryTargetSet.GetAsync([message.MonitoredQueryTargetId])
            ?? throw new InvalidOperationException("The provided query target could not be found in the cache");

        var targetId = queryTarget.MonitoredTargetId;

        foreach (var metric in message.MetricValues)
        {
            var metricBucketId = await GetOrCreateBucket(metric.Bucket);

            var targetMetric = new MonitoredTargetMetricBySecond()
            {
                MonitoredTargetId = targetId,
                MetricId = metric.MetricId,
                MetricBucketId = metricBucketId,
                MeasuredAtUtc = metric.TimestampUtc,
                Value = metric.Value,
                SeverityType = SeverityType.Nominal,
            };

            _context.MonitoredTargetMetricsBySecond.Add(targetMetric);
        }

        await _context.SaveChangesAsync();
    }

    private async Task<int> GetOrCreateBucket(string? bucket)
    {
        var bucketName = bucket?.Trim() ?? "";

        using var subScope = _serviceProvider.CreateScope();
        var subContext = subScope.ServiceProvider.GetRequiredService<ApplicationContext>();

        var maybeBucket = await subContext.MetricBuckets.SingleOrDefaultAsync(e => e.Name == bucketName);

        if (maybeBucket is null)
        {
            maybeBucket = new()
            {
                Name = bucketName,
            };

            subContext.MetricBuckets.Add(maybeBucket);
            await subContext.SaveChangesAsync();
        }

        return maybeBucket.Id;
    }
}
