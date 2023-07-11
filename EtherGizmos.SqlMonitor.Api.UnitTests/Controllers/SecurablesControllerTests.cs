using EtherGizmos.SqlMonitor.Api.Controllers;
using EtherGizmos.SqlMonitor.Api.OData.Metadata;
using EtherGizmos.SqlMonitor.Api.Services.Abstractions;
using EtherGizmos.SqlMonitor.Api.UnitTests.Extensions;
using EtherGizmos.SqlMonitor.Models.Api.v1;
using EtherGizmos.SqlMonitor.Models.Database;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OData;
using MockQueryable.Moq;
using Moq;
using System.Net;

namespace EtherGizmos.SqlMonitor.Api.UnitTests.Controllers;

internal class SecurablesControllerTests
{
    private IServiceProvider Provider { get; set; }

    [SetUp]
    public void SetUp()
    {
        Provider = Services.CreateScope();
    }

    [Test]
    public async Task Search_IsValid_Returns200Ok()
    {
        var controller = Provider.GetRequiredService<SecurablesController>();

        var model = ODataModel.GetEdmModel(1.0m);
        var queryOptions = ODataQueryOptionsHelper.CreateOptions<SecurableDTO>(
            model,
            "GET",
            "https://localhost:7200",
            "api/v1",
            "securables",
            "",
            "");

        var data = new List<Securable>()
        {
            new Securable()
            {
                Id = "METRIC",
                Name = "Metrics"
            },
            new Securable()
            {
                Id = "QUERY",
                Name = "Queries"
            }
        };

        var mockData = data.AsQueryable().BuildMock();

        var mockServ = Provider.GetRequiredService<Mock<ISecurableService>>();
        mockServ.Setup(service => service.GetQueryable()).Returns(mockData);
        mockServ.Setup(service => service.AddOrUpdate(It.IsAny<Securable>()));

        var mockSave = Provider.GetRequiredService<Mock<ISaveService>>();
        mockSave.Setup(service => service.SaveChangesAsync()).Returns(Task.CompletedTask);

        var result = await controller.Search(queryOptions);
        var status = result.GetStatusCode();
        var content = result.GetContent();

        Assert.Multiple(() =>
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(status, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(content, Is.AssignableTo<EnumerableQuery<SecurableDTO>>());
        });

        mockServ.Verify(service => service.GetQueryable(), Times.Once());
        mockServ.Verify(service => service.AddOrUpdate(It.IsAny<Securable>()), Times.Never());
        mockSave.Verify(service => service.SaveChangesAsync(), Times.Never());
    }

    [Test]
    public async Task Get_IsInvalid_Returns404NotFound()
    {
        var controller = Provider.GetRequiredService<SecurablesController>();

        string recordId = "INVALID";

        var model = ODataModel.GetEdmModel(1.0m);
        var queryOptions = ODataQueryOptionsHelper.CreateOptions<SecurableDTO>(
            model,
            "GET",
            "https://localhost:7200",
            "api/v1",
            "securables",
            $"('{recordId}')",
            "");

        var data = new List<Securable>()
        {
            new Securable()
            {
                Id = "METRIC",
                Name = "Metrics"
            },
            new Securable()
            {
                Id = "QUERY",
                Name = "Queries"
            }
        };

        var mockData = data.AsQueryable().BuildMock();

        var mockServ = Provider.GetRequiredService<Mock<ISecurableService>>();
        mockServ.Setup(service => service.GetQueryable()).Returns(mockData);
        mockServ.Setup(service => service.AddOrUpdate(It.IsAny<Securable>()));

        var mockSave = Provider.GetRequiredService<Mock<ISaveService>>();
        mockSave.Setup(service => service.SaveChangesAsync()).Returns(Task.CompletedTask);

        var result = await controller.Get(recordId, queryOptions);
        var status = result.GetStatusCode();
        var content = result.GetContent();

        Assert.Multiple(() =>
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(status, Is.EqualTo(HttpStatusCode.NotFound));
            Assert.That(content, Is.AssignableTo<ODataError>());
        });

        mockServ.Verify(service => service.GetQueryable(), Times.Once());
        mockServ.Verify(service => service.AddOrUpdate(It.IsAny<Securable>()), Times.Never());
        mockSave.Verify(service => service.SaveChangesAsync(), Times.Never());
    }

    [Test]
    public async Task Get_IsValid_Returns200Ok()
    {
        var controller = Provider.GetRequiredService<SecurablesController>();

        string recordId = "QUERY";

        var model = ODataModel.GetEdmModel(1.0m);
        var queryOptions = ODataQueryOptionsHelper.CreateOptions<SecurableDTO>(
            model,
            "GET",
            "https://localhost:7200",
            "api/v1",
            "securables",
            $"('{recordId}')",
            "");

        var data = new List<Securable>()
        {
            new Securable()
            {
                Id = "METRIC",
                Name = "Metrics"
            },
            new Securable()
            {
                Id = "QUERY",
                Name = "Queries"
            }
        };

        var mockData = data.AsQueryable().BuildMock();

        var mockServ = Provider.GetRequiredService<Mock<ISecurableService>>();
        mockServ.Setup(service => service.GetQueryable()).Returns(mockData);
        mockServ.Setup(service => service.AddOrUpdate(It.IsAny<Securable>()));

        var mockSave = Provider.GetRequiredService<Mock<ISaveService>>();
        mockSave.Setup(service => service.SaveChangesAsync()).Returns(Task.CompletedTask);

        var result = await controller.Get(recordId, queryOptions);
        var status = result.GetStatusCode();
        var content = result.GetContent();

        Assert.Multiple(() =>
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(status, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(content, Is.AssignableTo<SecurableDTO>());
        });

        mockServ.Verify(service => service.GetQueryable(), Times.Once());
        mockServ.Verify(service => service.AddOrUpdate(It.IsAny<Securable>()), Times.Never());
        mockSave.Verify(service => service.SaveChangesAsync(), Times.Never());
    }
}
