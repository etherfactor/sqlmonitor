using EtherGizmos.SqlMonitor.Api.Services.Background.Abstractions;
using EtherGizmos.SqlMonitor.Api.Services.Caching;
using EtherGizmos.SqlMonitor.Api.Services.Caching.Abstractions;
using EtherGizmos.SqlMonitor.Services.Locking.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;

namespace EtherGizmos.SqlMonitor.Api.UnitTests.Services.Background.Abstractions;

internal class GlobalConstantBackgroundServiceTests
{
    private IServiceProvider _serviceProvider;
    private ILogger<GlobalConstantBackgroundServiceImplemented> _logger;
    private GlobalConstantBackgroundServiceImplemented? _service;

    [SetUp]
    public void SetUp()
    {
        _serviceProvider = Global.CreateScope();
        _logger = _serviceProvider.GetRequiredService<ILogger<GlobalConstantBackgroundServiceImplemented>>();
    }

    [Test]
    public async Task DoConstantGlobalWorkAsync_WhenObtainsLock_DoesConstantWork()
    {
        //Arrange
        var handle = new TestSynchronizationHandle();
        var cacheMock = _serviceProvider.GetRequiredService<Mock<IDistributedRecordCache>>();
        cacheMock.Setup(@interface =>
            @interface.AcquireLockAsync(It.IsAny<JobCacheKey>(), It.IsAny<TimeSpan>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new CacheLock<JobCacheKey>(new JobCacheKey("test"), handle));
        var cache = cacheMock.Object;

        _service = new GlobalConstantBackgroundServiceImplemented(_logger, _serviceProvider, cache, new JobCacheKey("test"), "0/5 * * * * *", "0/1 * * * * *");

        //Act
        _ = _service.DoWorkAsync(CancellationToken.None);
        await Task.Delay(5000);
        handle.Dispose();

        //Assert
        Assert.That(_service.PerformedWorkCount, Is.EqualTo(5).Or.EqualTo(6));
    }

    [Test]
    public async Task DoConstantGlobalWorkAsync_WhenDoesNotObtainLock_DoesNotWork()
    {
        //Arrange
        var handle = new TestSynchronizationHandle();
        var cacheMock = _serviceProvider.GetRequiredService<Mock<IDistributedRecordCache>>();
        cacheMock.Setup(@interface =>
            @interface.AcquireLockAsync(It.IsAny<JobCacheKey>(), It.IsAny<TimeSpan>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(null as CacheLock<JobCacheKey>);
        var cache = cacheMock.Object;

        _service = new GlobalConstantBackgroundServiceImplemented(_logger, _serviceProvider, cache, new JobCacheKey("test"), "0/5 * * * * *", "0/1 * * * * *");

        //Act
        _ = _service.DoWorkAsync(CancellationToken.None);
        await Task.Delay(1000);
        handle.Dispose();

        //Assert
        Assert.That(_service.PerformedWorkCount, Is.EqualTo(0));
    }
}

internal class GlobalConstantBackgroundServiceImplemented : GlobalConstantBackgroundService
{
    public int PerformedWorkCount { get; private set; } = 0;

    public GlobalConstantBackgroundServiceImplemented(
        ILogger<GlobalConstantBackgroundServiceImplemented> logger,
        IServiceProvider serviceProvider,
        IDistributedRecordCache cache,
        JobCacheKey cacheKey,
        string lockCronExpression,
        string cronExpression)
        : base(logger, serviceProvider, cache, cacheKey, lockCronExpression, cronExpression) { }

    protected internal override Task DoConstantGlobalWorkAsync(IServiceProvider scope, CancellationToken stoppingToken)
    {
        PerformedWorkCount++;
        return Task.CompletedTask;
    }
}
