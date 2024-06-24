using EtherGizmos.SqlMonitor.Api.Core.Services.Background;

namespace EtherGizmos.SqlMonitor.UnitTests.Services.Background;

internal class EnqueueMonitorQueriesServiceTests
{
    private IServiceProvider _serviceProvider;
    private EnqueueQueryMessagesService _service;

    //private readonly IQueryable<Instance> _instances = new List<Instance>()
    //{
    //    new Instance()
    //    {
    //        Id = Guid.NewGuid(),
    //        Name = "Test 1",
    //        IsActive = true
    //    },
    //    new Instance()
    //    {
    //        Id = Guid.NewGuid(),
    //        Name = "Test 2",
    //        IsActive = true
    //    },
    //    new Instance()
    //    {
    //        Id = Guid.NewGuid(),
    //        Name = "Test 3",
    //        IsActive = false
    //    }
    //}
    //.AsQueryable()
    //.BuildMock();

    //private readonly IQueryable<Query> _queries = new List<Query>()
    //{
    //    new Query()
    //    {
    //        Id = Guid.NewGuid(),
    //        Name = "Test 1",
    //        SqlText = "select 1 as [value];",
    //        LastRunAtUtc = DateTimeOffset.MinValue,
    //        RunFrequency = TimeSpan.FromSeconds(5),
    //        IsActive = true
    //    },
    //    new Query()
    //    {
    //        Id = Guid.NewGuid(),
    //        Name = "Test 2",
    //        SqlText = "select 2 as [value];",
    //        LastRunAtUtc = DateTimeOffset.MinValue,
    //        RunFrequency = TimeSpan.FromSeconds(5),
    //        IsActive = true
    //    },
    //    new Query()
    //    {
    //        Id = Guid.NewGuid(),
    //        Name = "Test 3",
    //        SqlText = "select 3 as [value];",
    //        LastRunAtUtc = DateTimeOffset.MinValue,
    //        RunFrequency = TimeSpan.FromSeconds(5),
    //        IsActive = false
    //    }
    //}
    //.AsQueryable()
    //.BuildMock();

    //[SetUp]
    //public async Task SetUp()
    //{
    //    _serviceProvider = Global.CreateScope();

    //    var logger = _serviceProvider.GetRequiredService<ILogger<EnqueueMonitorQueriesService>>();
    //    var cache = _serviceProvider.GetRequiredService<IDistributedRecordCache>();

    //    //Clear out the cache, since it's static (fine for the application, less fine for tests) to ensure that previous
    //    //tests do not dirty this test's results.
    //    var instanceCache = cache.EntitySet<Instance>();
    //    foreach (var instance in await instanceCache.ToListAsync())
    //    {
    //        await instanceCache.RemoveAsync(instance);
    //    }

    //    foreach (var instance in _instances)
    //    {
    //        await instanceCache.AddAsync(instance);
    //    }

    //    var queryCache = cache.EntitySet<Query>();
    //    foreach (var query in await queryCache.ToListAsync())
    //    {
    //        await queryCache.RemoveAsync(query);
    //    }

    //    foreach (var query in _queries)
    //    {
    //        await queryCache.AddAsync(query);
    //    }

    //    var mockInstanceServ = _serviceProvider.GetRequiredService<Mock<IInstanceService>>();
    //    mockInstanceServ.Setup(@interface =>
    //        @interface.GetQueryable())
    //        .Returns(_instances);

    //    var mockQueryServ = _serviceProvider.GetRequiredService<Mock<IQueryService>>();
    //    mockQueryServ.Setup(@interface =>
    //        @interface.GetQueryable())
    //        .Returns(_queries);

    //    var mockEndpoint = _serviceProvider.GetRequiredService<Mock<ISendEndpoint>>();
    //    mockEndpoint.Setup(@interface =>
    //        @interface.Send(It.IsAny<RunQuery>(), It.IsAny<CancellationToken>()))
    //        .Returns(Task.CompletedTask);

    //    var mockEndpointProvider = _serviceProvider.GetRequiredService<Mock<ISendEndpointProvider>>();
    //    mockEndpointProvider.Setup(@interface =>
    //        @interface.GetSendEndpoint(It.IsAny<Uri>()))
    //        .ReturnsAsync(mockEndpoint.Object);

    //    _service = new EnqueueMonitorQueriesService(logger, _serviceProvider, cache);
    //}

    //[Test]
    //public async Task DoWorkAsync_WorksAsExpected()
    //{
    //    //Arrange
    //    var mockEndpoint = _serviceProvider.GetRequiredService<Mock<ISendEndpoint>>();

    //    //Act
    //    await _service.DoConstantGlobalWorkAsync(_serviceProvider, CancellationToken.None);

    //    //Assert
    //    mockEndpoint.Verify(@interface =>
    //        @interface.Send(It.IsAny<RunQuery>(), It.IsAny<CancellationToken>()),
    //        Times.Exactly(4));
    //}
}
