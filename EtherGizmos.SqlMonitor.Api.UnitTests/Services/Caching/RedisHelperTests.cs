using EtherGizmos.SqlMonitor.Api.Services.Caching;
using EtherGizmos.SqlMonitor.Api.Services.Caching.Abstractions;
using EtherGizmos.SqlMonitor.Models.Database;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using StackExchange.Redis;

namespace EtherGizmos.SqlMonitor.Api.UnitTests.Services.Caching;

internal class RedisHelperTests
{
    private IServiceProvider _serviceProvider;
    private IRedisHelper<Query> _helper;

    private readonly Guid _queryId = Guid.NewGuid();
    private readonly Guid _metricId = Guid.NewGuid();

    [SetUp]
    public void SetUp()
    {
        _serviceProvider = Global.CreateScope();

        _helper = _serviceProvider.GetRequiredService<IRedisHelperFactory>()
            .CreateHelper<Query>();
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

        var query = new Query()
        {
            Id = _queryId,
            Name = "Test Query",
            SqlText = "select 1 as [value];",
            Metrics = new List<QueryMetric>()
            {
                new QueryMetric()
                {
                    QueryId = _queryId,
                    MetricId = _metricId,
                    Metric = new Metric()
                    {
                        Id = _metricId,
                        Name = "Test Metric"
                    }
                }
            }
        };

        //Act
        _helper.AppendAddAction(database, transaction, query);

        //Assert
        transactionMock.Verify(@interface =>
            @interface.HashSetAsync($"sqlpulse:$$table:queries:%22{_queryId}%22", It.IsAny<HashEntry[]>(), It.IsAny<CommandFlags>()),
            Times.Once());

        transactionMock.Verify(@interface =>
            @interface.SortedSetAddAsync($"sqlpulse:$$table:queries:$$primary", $"%22{_queryId}%22", 0, It.IsAny<SortedSetWhen>(), It.IsAny<CommandFlags>()),
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
        _helper.AppendDeleteAction(database, transaction, new EntityCacheKey<Query>("some_query"));

        //Assert
        transactionMock.Verify(@interface =>
            @interface.KeyDeleteAsync($"sqlpulse:$$entity:some_query", It.IsAny<CommandFlags>()),
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
            @interface.SortAsync($"sqlpulse:$$table:queries:$$primary", It.IsAny<long>(), It.IsAny<long>(), It.IsAny<Order>(), It.IsAny<SortType>(), "nosort", It.IsAny<RedisValue[]>(), It.IsAny<CommandFlags>()),
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
        var action = _helper.AppendReadAction(database, transaction, new EntityCacheKey<Query>("some_query"));
        Assert.That(action, Is.Not.Null);

        var result = await action();
        Assert.That(result, Is.Null);

        //Assert
        transactionMock.Verify(@interface =>
            @interface.HashGetAsync($"sqlpulse:$$entity:some_query", It.IsAny<RedisValue[]>(), It.IsAny<CommandFlags>()),
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

        var query = new Query()
        {
            Id = _queryId,
            Name = "Test Query",
            SqlText = "select 1 as [value];",
            Metrics = new List<QueryMetric>()
            {
                new QueryMetric()
                {
                    QueryId = _queryId,
                    MetricId = _metricId,
                    Metric = new Metric()
                    {
                        Id = _metricId,
                        Name = "Test Metric"
                    }
                }
            }
        };

        //Act
        _helper.AppendRemoveAction(database, transaction, query);

        //Assert
        transactionMock.Verify(@interface =>
            @interface.KeyDeleteAsync($"sqlpulse:$$table:queries:%22{_queryId}%22", It.IsAny<CommandFlags>()),
            Times.Once());

        transactionMock.Verify(@interface =>
            @interface.SortedSetRemoveAsync($"sqlpulse:$$table:queries:$$primary", $"%22{_queryId}%22", It.IsAny<CommandFlags>()),
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

        var query = new Query()
        {
            Id = _queryId,
            Name = "Test Query",
            SqlText = "select 1 as [value];",
            Metrics = new List<QueryMetric>()
            {
                new QueryMetric()
                {
                    QueryId = _queryId,
                    MetricId = _metricId,
                    Metric = new Metric()
                    {
                        Id = _metricId,
                        Name = "Test Metric"
                    }
                }
            }
        };

        //Act
        _helper.AppendSetAction(database, transaction, new EntityCacheKey<Query>("some_query"), query);

        //Assert
        transactionMock.Verify(@interface =>
            @interface.HashSetAsync($"sqlpulse:$$entity:some_query", It.IsAny<HashEntry[]>(), It.IsAny<CommandFlags>()),
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
            Assert.That(lookupSets.Count(), Is.EqualTo(1));
        });
    }

    [Test]
    public void GetTempKey_StartsWithSchema()
    {
        //Arrange

        //Act
        var tempKey = (_helper as RedisHelper<Query>)!.GetTempKey();

        //Assert
        Assert.That(tempKey.ToString(), Does.StartWith("sqlpulse:$$temp:"));
    }
}
