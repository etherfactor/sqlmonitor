﻿using EtherGizmos.SqlMonitor.Shared.Database.Services;
using EtherGizmos.SqlMonitor.Shared.Database.Services.Abstractions;
using EtherGizmos.SqlMonitor.Shared.Messaging.Messages;
using EtherGizmos.SqlMonitor.Shared.Models.Database;
using EtherGizmos.SqlMonitor.Shared.Models.Database.Enums;
using EtherGizmos.SqlMonitor.Shared.Redis.Caching.Abstractions;
using EtherGizmos.SqlMonitor.Shared.Redis.Locking.Abstractions;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace EtherGizmos.SqlMonitor.Api.Core.Services.Messaging;

public class ScriptResultConsumer : IConsumer<ScriptResultMessage>
{
    private readonly ILogger _logger;
    private readonly IServiceProvider _serviceProvider;
    private readonly IRecordCache _cache;
    private readonly ILockingCoordinator _coordinator;
    private readonly IMetricBucketLockFactory _metricBucketLockFactory;
    private readonly ISaveService _saveService;
    private readonly IMonitoredTargetMetricsBySecondService _targetMetricsService;

    public ScriptResultConsumer(
        ILogger<ScriptResultConsumer> logger,
        IServiceProvider serviceProvider,
        IRecordCache cache,
        ILockingCoordinator coordinator,
        IMetricBucketLockFactory metricBucketLockFactory,
        ISaveService saveService,
        IMonitoredTargetMetricsBySecondService targetMetricsService)
    {
        _logger = logger;
        _serviceProvider = serviceProvider;
        _cache = cache;
        _coordinator = coordinator;
        _metricBucketLockFactory = metricBucketLockFactory;
        _saveService = saveService;
        _targetMetricsService = targetMetricsService;
    }

    public async Task Consume(ConsumeContext<ScriptResultMessage> context)
    {
        var message = context.Message;

        _logger.LogInformation("Processing message {@ScriptResultMessage}", message);

        var scriptTargetSet = _cache.EntitySet<MonitoredScriptTarget>();
        var scriptTarget = await scriptTargetSet.GetAsync([message.MonitoredScriptTargetId])
            ?? throw new InvalidOperationException("The provided script target could not be found in the cache");

        var targetId = scriptTarget.MonitoredTargetId;

        foreach (var metric in message.MetricValues)
        {
            var metricBucketId = await GetOrCreateBucketAsync(metric.Bucket);

            var targetMetric = new MonitoredTargetMetricBySecond()
            {
                MonitoredTargetId = targetId,
                MetricId = metric.MetricId,
                MetricBucketId = metricBucketId,
                MeasuredAtUtc = metric.TimestampUtc,
                Value = metric.Value,
                SeverityType = SeverityType.Nominal,
            };

            _targetMetricsService.Add(targetMetric);
        }

        await _saveService.SaveChangesAsync();
    }

    private async Task<int> GetOrCreateBucketAsync(string? bucket, CancellationToken cancellationToken = default)
    {
        var bucketName = bucket?.Trim() ?? "";

        using var subScope = _serviceProvider.CreateScope();
        var subContext = subScope.ServiceProvider.GetRequiredService<ApplicationContext>();

        var maybeBucket = await subContext.MetricBuckets.SingleOrDefaultAsync(e => e.Name == bucketName);

        if (maybeBucket is null)
        {
            var key = _metricBucketLockFactory.CreateKey(bucketName);
            using var @lock = await _coordinator.AcquireLockAsync(key, TimeSpan.FromDays(1), cancellationToken);

            maybeBucket = await subContext.MetricBuckets.SingleOrDefaultAsync(e => e.Name == bucketName);

            if (maybeBucket is null)
            {
                maybeBucket = new()
                {
                    Name = bucketName,
                };

                subContext.MetricBuckets.Add(maybeBucket);
                await subContext.SaveChangesAsync();
            }
        }

        return maybeBucket.Id;
    }
}
