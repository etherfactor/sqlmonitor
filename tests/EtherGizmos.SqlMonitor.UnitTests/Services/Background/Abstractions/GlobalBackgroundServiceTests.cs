using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;

namespace EtherGizmos.SqlMonitor.UnitTests.Services.Background.Abstractions;

internal class GlobalBackgroundServiceTests
{
    private IServiceProvider _serviceProvider;
    private ILogger<GlobalBackgroundServiceImplemented> _logger;
    private GlobalBackgroundServiceImplemented? _service;

    [SetUp]
    public void SetUp()
    {
        _serviceProvider = Global.CreateScope();
        _logger = _serviceProvider.GetRequiredService<ILogger<GlobalBackgroundServiceImplemented>>();
    }

    [Test]
    public async Task DoWorkAsync_WhenObtainsLock_DoesWork()
    {
        //Arrange
        var cacheMock = _serviceProvider.GetRequiredService<Mock<IDistributedLockProvider>>();
        cacheMock.Setup(@interface =>
            @interface.AcquireLockAsync(It.IsAny<JobCacheKey>(), It.IsAny<TimeSpan>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new CacheLock<JobCacheKey>(new JobCacheKey(typeof(GlobalBackgroundServiceImplemented)), new TestSynchronizationHandle()));
        var cache = cacheMock.Object;

        _service = new GlobalBackgroundServiceImplemented(_logger, cache, "0/1 * * * * *");

        //Act
        await _service.DoWorkAsync(CancellationToken.None);

        //Assert
        Assert.That(_service.HasPerformedWork, Is.True);
    }

    [Test]
    public async Task DoWorkAsync_WhenDoesNotObtainLock_DoesNotWork()
    {
        //Arrange
        var cacheMock = _serviceProvider.GetRequiredService<Mock<IDistributedLockProvider>>();
        cacheMock.Setup(@interface =>
            @interface.AcquireLockAsync(It.IsAny<JobCacheKey>(), It.IsAny<TimeSpan>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(null as CacheLock<JobCacheKey>);
        var cache = cacheMock.Object;

        _service = new GlobalBackgroundServiceImplemented(_logger, cache, "0/1 * * * * *");

        //Act
        await _service.DoWorkAsync(CancellationToken.None);

        //Assert
        Assert.That(_service.HasPerformedWork, Is.False);
    }
}

internal class GlobalBackgroundServiceImplemented : GlobalBackgroundService
{
    public bool HasPerformedWork { get; private set; } = false;

    public GlobalBackgroundServiceImplemented(
        ILogger<GlobalBackgroundServiceImplemented> logger,
        IDistributedLockProvider cache,
        string cronExpression)
        : base(logger, cache, cronExpression) { }

    protected internal override Task DoGlobalWorkAsync(CancellationToken stoppingToken)
    {
        HasPerformedWork = true;
        return Task.CompletedTask;
    }
}
