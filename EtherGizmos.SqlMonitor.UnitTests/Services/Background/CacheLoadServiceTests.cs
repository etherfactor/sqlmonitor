using EtherGizmos.SqlMonitor.Api.Services.Background;

namespace EtherGizmos.SqlMonitor.Api.UnitTests.Services.Background;

internal class CacheLoadServiceTests
{
    private IServiceProvider _serviceProvider;
    private CacheLoadService _service;

    //private readonly IQueryable<Instance> _instances = new List<Instance>()
    //{
    //    new Instance()
    //    {
    //        Id = Guid.NewGuid(),
    //        Name = "Test 1"
    //    },
    //    new Instance()
    //    {
    //        Id = Guid.NewGuid(),
    //        Name = "Test 2"
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
    //        SqlText = "select 1 as [value];"
    //    },
    //    new Query()
    //    {
    //        Id = Guid.NewGuid(),
    //        Name = "Test 2",
    //        SqlText = "select 2 as [value];"
    //    },
    //    new Query()
    //    {
    //        Id = Guid.NewGuid(),
    //        Name = "Test 3",
    //        SqlText = "select 3 as [value];"
    //    }
    //}
    //.AsQueryable()
    //.BuildMock();

    //[SetUp]
    //public async Task SetUp()
    //{
    //    _serviceProvider = Global.CreateScope();

    //    var logger = _serviceProvider.GetRequiredService<ILogger<CacheLoadService>>();
    //    var cache = _serviceProvider.GetRequiredService<IDistributedRecordCache>();

    //    //Clear out the cache, since it's static (fine for the application, less fine for tests) to ensure that previous
    //    //tests do not dirty this test's results.
    //    var instanceCache = cache.EntitySet<Instance>();
    //    foreach (var instance in await instanceCache.ToListAsync())
    //    {
    //        await instanceCache.RemoveAsync(instance);
    //    }

    //    var queryCache = cache.EntitySet<Query>();
    //    foreach (var query in await queryCache.ToListAsync())
    //    {
    //        await queryCache.RemoveAsync(query);
    //    }

    //    var mockInstanceServ = _serviceProvider.GetRequiredService<Mock<IInstanceService>>();
    //    mockInstanceServ.Setup(service => service.GetQueryable()).Returns(_instances);

    //    var mockQueryServ = _serviceProvider.GetRequiredService<Mock<IQueryService>>();
    //    mockQueryServ.Setup(service => service.GetQueryable()).Returns(_queries);

    //    _service = new CacheLoadService(logger, _serviceProvider, cache);
    //}

    //[Test]
    //public async Task DoWorkAsync_WorksAsExpected()
    //{
    //    //Arrange
    //    var cache = _serviceProvider.GetRequiredService<IDistributedRecordCache>();
    //    var oldInstances = await cache.EntitySet<Instance>().ToListAsync();
    //    var oldQueries = await cache.EntitySet<Query>().ToListAsync();

    //    //Act
    //    await _service.DoWorkAsync(CancellationToken.None);
    //    var newInstances = await cache.EntitySet<Instance>().ToListAsync();
    //    var newQueries = await cache.EntitySet<Query>().ToListAsync();

    //    //Assert
    //    Assert.Multiple(() =>
    //    {
    //        Assert.That(oldInstances, Has.Count.EqualTo(0));
    //        Assert.That(oldQueries, Has.Count.EqualTo(0));

    //        Assert.That(newInstances, Has.Count.EqualTo(_instances.Count()));
    //        Assert.That(newQueries, Has.Count.EqualTo(_queries.Count()));
    //    });
    //}
}
