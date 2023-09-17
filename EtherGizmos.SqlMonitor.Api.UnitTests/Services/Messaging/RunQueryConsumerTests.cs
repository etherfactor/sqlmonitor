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

    [SetUp]
    public void SetUp()
    {
        _serviceProvider = Global.CreateScope();

        var logger = _serviceProvider.GetRequiredService<ILogger<RunQueryConsumer>>();
        _service = new RunQueryConsumer(logger);
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
                Instance = new Instance()
                {
                    Address = "(localdb)\\mssqllocaldb"
                },
                Query = new Query()
                {
                    SqlText = "select 1 as [value];"
                }
            });

        //Act
        await _service.Consume(mockContext.Object);

        //Assert
        //TODO: Add assertions based on consumption results. For the time being, since this directly connects to SQL,
        //getting to this point should suffice as a success.
    }
}
