using EtherGizmos.SqlMonitor.Api.Services.Caching;
using EtherGizmos.SqlMonitor.Api.Services.Messaging;
using EtherGizmos.SqlMonitor.Models.Database;
using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;

namespace EtherGizmos.SqlMonitor.Api.UnitTests.Services.Messaging;

internal class RunQueryConsumerTests
{
    private IServiceProvider _serviceProvider;
    private RunQueryConsumer _service;

    private readonly Guid _queryId = Guid.NewGuid();
    private readonly Guid _instanceId = Guid.NewGuid();

    [SetUp]
    public async Task SetUp()
    {
        _serviceProvider = Global.CreateScope();

        var memoryCache = new InMemoryRecordCache(_serviceProvider);

        await memoryCache.EntitySet<Query>()
            .AddAsync(new Query()
            {
                Id = _queryId,
                SqlText = "select 1 as [value];"
            });

        await memoryCache.EntitySet<Instance>()
            .AddAsync(new Instance()
            {
                Id = _instanceId,
                Address = "(localdb)\\mssqllocaldb"
            });

        var logger = _serviceProvider.GetRequiredService<ILogger<RunQueryConsumer>>();
        _service = new RunQueryConsumer(logger, memoryCache, _serviceProvider);
    }

    [Test]
    public async Task Consume_WorksAsExpected()
    {
        //Arrange
        var mockContext = _serviceProvider.GetRequiredService<Mock<ConsumeContext<RunQuery>>>();
        mockContext.Setup(@interface =>
            @interface.Message)
            .Returns(new RunQuery()
            {
                InstanceId = _instanceId,
                QueryId = _queryId
            });

        //Act
        await _service.Consume(mockContext.Object);

        //Assert
        //TODO: Add assertions based on consumption results. For the time being, since this directly connects to SQL,
        //getting to this point should suffice as a success.
    }
}
