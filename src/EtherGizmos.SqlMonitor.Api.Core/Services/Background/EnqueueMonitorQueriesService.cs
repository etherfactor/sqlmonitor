﻿using EtherGizmos.SqlMonitor.Shared.Redis.Locking.Abstractions;
using EtherGizmos.SqlMonitor.Shared.Redis.Services.Background.Abstractions;
using Microsoft.Extensions.Logging;

namespace EtherGizmos.SqlMonitor.Api.Core.Services.Background;

/// <summary>
/// Runs queries against instances on a periodic timer.
/// </summary>
public class EnqueueMonitorQueriesService : GlobalConstantBackgroundService
{
    private const string CronExpression = "0/15 * * * * *";
    private const string ConstantCronExpression = "0/1 * * * * *";

    private readonly ILogger _logger;
    private readonly IDistributedLockProvider _distributedLockProvider;

    /// <summary>
    /// Construct the service.
    /// </summary>
    /// <param name="logger">The logger to use.</param>
    /// <param name="serviceProvider">Provides access to services.</param>
    public EnqueueMonitorQueriesService(
        ILogger<EnqueueMonitorQueriesService> logger,
        IServiceProvider serviceProvider,
        IDistributedLockProvider distributedLockProvider)
        : base(logger, serviceProvider, distributedLockProvider, CronExpression, ConstantCronExpression)
    {
        _logger = logger;
        _distributedLockProvider = distributedLockProvider;
    }

    /// <inheritdoc/>
    protected override Task DoConstantGlobalWorkAsync(IServiceProvider scope, CancellationToken stoppingToken)
    {
        return Task.CompletedTask;
    }
}