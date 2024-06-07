using EtherGizmos.SqlMonitor.Shared.Redis.Caching.Abstractions;
using EtherGizmos.SqlMonitor.Shared.Redis.Locking.Abstractions;
using EtherGizmos.SqlMonitor.Shared.Redis.Services.Background.Abstractions;
using Microsoft.Extensions.Logging;

namespace EtherGizmos.SqlMonitor.Api.Core.Services.Background;

internal class EnqueueScriptMessagesService : GlobalConstantBackgroundService
{
    private const string CronExpression = "0/15 * * * * *";
    private const string ConstantCronExpression = "0/1 * * * * *";

    private readonly ILogger _logger;
    private readonly IDistributedLockProvider _distributedLockProvider;
    private readonly IDistributedRecordCache _distributedRecordCache;

    /// <summary>
    /// Construct the service.
    /// </summary>
    /// <param name="logger">The logger to use.</param>
    /// <param name="serviceProvider">Provides access to services.</param>
    public EnqueueScriptMessagesService(
        ILogger<EnqueueScriptMessagesService> logger,
        IServiceProvider serviceProvider,
        IDistributedLockProvider distributedLockProvider,
        IDistributedRecordCache distributedRecordCache)
        : base(logger, serviceProvider, distributedLockProvider, CronExpression, ConstantCronExpression)
    {
        _logger = logger;
        _distributedLockProvider = distributedLockProvider;
        _distributedRecordCache = distributedRecordCache;
    }

    /// <inheritdoc/>
    protected override Task DoConstantGlobalWorkAsync(IServiceProvider scope, CancellationToken stoppingToken)
    {
        return Task.CompletedTask;
    }
}
