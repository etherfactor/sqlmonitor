using EtherGizmos.SqlMonitor.Api.Services.Caching;
using EtherGizmos.SqlMonitor.Api.Services.Caching.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using StackExchange.Redis;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EtherGizmos.SqlMonitor.Api.UnitTests.Services.Caching;

internal class RedisHelperTests
{
    private IServiceProvider _serviceProvider;
    private IRedisHelper<CacheTester> _helper;

    private readonly Guid _CacheTesterId = Guid.NewGuid();
    private readonly Guid _metricId = Guid.NewGuid();

    [SetUp]
    public void SetUp()
    {
        _serviceProvider = Global.CreateScope();

        _helper = _serviceProvider.GetRequiredService<IRedisHelperFactory>()
            .CreateHelper<CacheTester>();
    }

    [Test]
    public void AppendAddAction_Validates()
    {
        //Arrange
        var transactionMock = new Mock<ITransaction>();
        var transaction = transactionMock.Object;

        var databaseMock = new Mock<IDatabase>();
        databaseMock.Setup(@interface =>
            @interface.CreateTransaction(It.IsAny<object>()))
            .Returns(new Mock<ITransaction>().Object);
        var database = databaseMock.Object;

        var CacheTester = new CacheTester()
        {
            Id = _CacheTesterId,
            Name = "Test Cache Tester",
        };

        //Act
        _helper.AppendAddAction(database, transaction, CacheTester);

        //Assert
        transactionMock.Verify(@interface =>
            @interface.HashSetAsync($"sqlpulse:$$table:cache_entities:%22{_CacheTesterId}%22", It.IsAny<HashEntry[]>(), It.IsAny<CommandFlags>()),
            Times.Once());

        transactionMock.Verify(@interface =>
            @interface.SortedSetAddAsync($"sqlpulse:$$table:cache_entities:$$primary", $"%22{_CacheTesterId}%22", 0, It.IsAny<SortedSetWhen>(), It.IsAny<CommandFlags>()),
            Times.Once());
    }

    [Test]
    public void AppendDeleteAction_Validates()
    {
        //Arrange
        var transactionMock = new Mock<ITransaction>();
        var transaction = transactionMock.Object;

        var databaseMock = new Mock<IDatabase>();
        databaseMock.Setup(@interface =>
            @interface.CreateTransaction(It.IsAny<object>()))
            .Returns(new Mock<ITransaction>().Object);
        var database = databaseMock.Object;

        //Act
        _helper.AppendDeleteAction(database, transaction, new EntityCacheKey<CacheTester>("some_cache_tester"));

        //Assert
        transactionMock.Verify(@interface =>
            @interface.KeyDeleteAsync($"sqlpulse:$$entity:some_cache_tester", It.IsAny<CommandFlags>()),
            Times.Once());
    }

    [Test]
    public async Task AppendListAsync_Validates()
    {
        //Arrange
        var transactionMock = new Mock<ITransaction>();
        var transaction = transactionMock.Object;

        var databaseMock = new Mock<IDatabase>();
        databaseMock.Setup(@interface =>
            @interface.CreateTransaction(It.IsAny<object>()))
            .Returns(new Mock<ITransaction>().Object);
        var database = databaseMock.Object;

        //Act
        var action = _helper.AppendListAction(database, transaction);
        Assert.That(action, Is.Not.Null);

        var result = await action();
        Assert.That(result, Is.Not.Null);

        //Assert
        transactionMock.Verify(@interface =>
            @interface.SortAsync($"sqlpulse:$$table:cache_entities:$$primary", It.IsAny<long>(), It.IsAny<long>(), It.IsAny<Order>(), It.IsAny<SortType>(), "nosort", It.IsAny<RedisValue[]>(), It.IsAny<CommandFlags>()),
            Times.Once());

        transactionMock.Verify(@interface =>
            @interface.KeyDeleteAsync(It.IsAny<RedisKey>(), It.IsAny<CommandFlags>()),
            Times.Never());
    }

    [Test]
    public async Task AppendReadAction_Validates()
    {
        //Arrange
        var transactionMock = new Mock<ITransaction>();
        var transaction = transactionMock.Object;

        var databaseMock = new Mock<IDatabase>();
        databaseMock.Setup(@interface =>
            @interface.CreateTransaction(It.IsAny<object>()))
            .Returns(new Mock<ITransaction>().Object);
        var database = databaseMock.Object;

        //Act
        var action = _helper.AppendReadAction(database, transaction, new EntityCacheKey<CacheTester>("some_cache_tester"));
        Assert.That(action, Is.Not.Null);

        var result = await action();
        Assert.That(result, Is.Null);

        //Assert
        transactionMock.Verify(@interface =>
            @interface.HashGetAsync($"sqlpulse:$$entity:some_cache_tester", It.IsAny<RedisValue[]>(), It.IsAny<CommandFlags>()),
            Times.Once());
    }

    [Test]
    public void AppendRemoveAction_Validates()
    {
        //Arrange
        var transactionMock = new Mock<ITransaction>();
        var transaction = transactionMock.Object;

        var databaseMock = new Mock<IDatabase>();
        databaseMock.Setup(@interface =>
            @interface.CreateTransaction(It.IsAny<object>()))
            .Returns(new Mock<ITransaction>().Object);
        var database = databaseMock.Object;

        var CacheTester = new CacheTester()
        {
            Id = _CacheTesterId,
            Name = "Test Cache Tester",
        };

        //Act
        _helper.AppendRemoveAction(database, transaction, CacheTester);

        //Assert
        transactionMock.Verify(@interface =>
            @interface.KeyDeleteAsync($"sqlpulse:$$table:cache_entities:%22{_CacheTesterId}%22", It.IsAny<CommandFlags>()),
            Times.Once());

        transactionMock.Verify(@interface =>
            @interface.SortedSetRemoveAsync($"sqlpulse:$$table:cache_entities:$$primary", $"%22{_CacheTesterId}%22", It.IsAny<CommandFlags>()),
            Times.Once());
    }

    [Test]
    public void AppendSetAction_Validates()
    {
        //Arrange
        var transactionMock = new Mock<ITransaction>();
        var transaction = transactionMock.Object;

        var databaseMock = new Mock<IDatabase>();
        databaseMock.Setup(@interface =>
            @interface.CreateTransaction(It.IsAny<object>()))
            .Returns(new Mock<ITransaction>().Object);
        var database = databaseMock.Object;

        var CacheTester = new CacheTester()
        {
            Id = _CacheTesterId,
            Name = "Test Cache Tester",
        };

        //Act
        _helper.AppendSetAction(database, transaction, new EntityCacheKey<CacheTester>("some_cache_tester"), CacheTester);

        //Assert
        transactionMock.Verify(@interface =>
            @interface.HashSetAsync($"sqlpulse:$$entity:some_cache_tester", It.IsAny<HashEntry[]>(), It.IsAny<CommandFlags>()),
            Times.Once());
    }

    [Test]
    public void GetProperties_Validates()
    {
        //Arrange

        //Act
        var properties = _helper.GetProperties();
        var keys = _helper.GetKeyProperties();
        var lookupSingles = _helper.GetLookupSingleProperties();
        var lookupSets = _helper.GetLookupSetProperties();

        //Assert
        Assert.Multiple(() =>
        {
            Assert.That(properties.Count(), Is.GreaterThan(keys.Count()));
            Assert.That(lookupSingles.Count(), Is.EqualTo(0));
            Assert.That(lookupSets.Count(), Is.EqualTo(0));
        });
    }

    [Test]
    public void GetTempKey_StartsWithSchema()
    {
        //Arrange

        //Act
        var tempKey = (_helper as RedisHelper<CacheTester>)!.GetTempKey();

        //Assert
        Assert.That(tempKey.ToString(), Does.StartWith("sqlpulse:$$temp:"));
    }
}

[Table("cache_entities")]
internal class CacheTester
{
    [Column("cache_entity_id")]
    [Key]
    public Guid Id { get; set; }

    [Column("name")]
    public string? Name { get; set; }
}
