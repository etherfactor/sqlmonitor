using EtherGizmos.SqlMonitor.Api.Controllers;
using EtherGizmos.SqlMonitor.Api.OData.Metadata;
using EtherGizmos.SqlMonitor.Api.Services.Data.Abstractions;
using EtherGizmos.SqlMonitor.Api.UnitTests.Extensions;
using EtherGizmos.SqlMonitor.Models.Api.v1;
using EtherGizmos.SqlMonitor.Models.Database;
using EtherGizmos.SqlMonitor.Models.Exceptions;
using Microsoft.AspNetCore.OData.Query.Wrapper;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OData;
using MockQueryable.Moq;
using Moq;
using System.Net;

namespace EtherGizmos.SqlMonitor.Api.UnitTests.Controllers;

internal class SecurablesControllerTests
{
    private IServiceProvider Provider { get; set; }

    private SecurablesController Controller { get; set; }

    private List<Securable> Data { get; set; }

    [SetUp]
    public void SetUp()
    {
        Provider = Global.CreateScope();
        Controller = Provider.GetRequiredService<SecurablesController>();
        Data = new List<Securable>()
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

        var mockData = Data.AsQueryable().BuildMock();

        var mockServ = Provider.GetRequiredService<Mock<ISecurableService>>();
        mockServ.Setup(service => service.GetQueryable()).Returns(mockData);

        var mockSave = Provider.GetRequiredService<Mock<ISaveService>>();
        mockSave.Setup(service => service.SaveChangesAsync()).Returns(Task.CompletedTask);
    }

    [Test]
    public async Task Search_IsValid_Returns200Ok()
    {
        var model = ODataModel.GetEdmModel(1.0m);
        var queryOptions = ODataQueryOptionsHelper.CreateOptions<SecurableDTO>(
            model,
            "GET",
            "https://localhost:7200",
            "api/v1",
            "securables",
            "",
            "");

        var result = await Controller.Search(queryOptions);
        var status = result.GetStatusCode();
        var content = result.GetContent();

        Assert.Multiple(() =>
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(status, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(content, Is.AssignableTo<IEnumerable<SecurableDTO>>());
        });

        var mockServ = Provider.GetRequiredService<Mock<ISecurableService>>();
        mockServ.Verify(service => service.GetQueryable(), Times.AtLeastOnce());

        var mockSave = Provider.GetRequiredService<Mock<ISaveService>>();
        mockSave.Verify(service => service.SaveChangesAsync(), Times.Never());
    }

    [Test]
    public async Task Search_IsValid_WithFilter_Returns200Ok()
    {
        var model = ODataModel.GetEdmModel(1.0m);
        var queryOptions = ODataQueryOptionsHelper.CreateOptions<SecurableDTO>(
            model,
            "GET",
            "https://localhost:7200",
            "api/v1",
            "securables",
            "",
            "$filter=id eq 'QUERY'");

        var result = await Controller.Search(queryOptions);
        var status = result.GetStatusCode();
        var content = result.GetContent();

        Assert.Multiple(() =>
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(status, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(content, Is.AssignableTo<IEnumerable<SecurableDTO>>());
        });

        var mockServ = Provider.GetRequiredService<Mock<ISecurableService>>();
        mockServ.Verify(service => service.GetQueryable(), Times.AtLeastOnce());

        var mockSave = Provider.GetRequiredService<Mock<ISaveService>>();
        mockSave.Verify(service => service.SaveChangesAsync(), Times.Never());
    }

    [Test]
    public async Task Search_IsValid_WithExpand_Returns200Ok()
    {
        var model = ODataModel.GetEdmModel(1.0m);
        var queryOptions = ODataQueryOptionsHelper.CreateOptions<SecurableDTO>(
            model,
            "GET",
            "https://localhost:7200",
            "api/v1",
            "securables",
            "",
            "$expand=permissions");

        var result = await Controller.Search(queryOptions);
        var status = result.GetStatusCode();
        var content = result.GetContent();

        Assert.Multiple(() =>
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(status, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(content, Is.AssignableTo<IEnumerable<ISelectExpandWrapper>>());
        });

        var mockServ = Provider.GetRequiredService<Mock<ISecurableService>>();
        mockServ.Verify(service => service.GetQueryable(), Times.AtLeastOnce());

        var mockSave = Provider.GetRequiredService<Mock<ISaveService>>();
        mockSave.Verify(service => service.SaveChangesAsync(), Times.Never());
    }

    [Test]
    public async Task Search_IsValid_WithSelect_Returns200Ok()
    {
        var model = ODataModel.GetEdmModel(1.0m);
        var queryOptions = ODataQueryOptionsHelper.CreateOptions<SecurableDTO>(
            model,
            "GET",
            "https://localhost:7200",
            "api/v1",
            "securables",
            "",
            "$select=id");

        var result = await Controller.Search(queryOptions);
        var status = result.GetStatusCode();
        var content = result.GetContent();

        Assert.Multiple(() =>
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(status, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(content, Is.AssignableTo<IEnumerable<ISelectExpandWrapper>>());
        });

        var mockServ = Provider.GetRequiredService<Mock<ISecurableService>>();
        mockServ.Verify(service => service.GetQueryable(), Times.AtLeastOnce());

        var mockSave = Provider.GetRequiredService<Mock<ISaveService>>();
        mockSave.Verify(service => service.SaveChangesAsync(), Times.Never());
    }

    [Test]
    public async Task Search_IsValid_WithOrderBy_Returns200Ok()
    {
        var model = ODataModel.GetEdmModel(1.0m);
        var queryOptions = ODataQueryOptionsHelper.CreateOptions<SecurableDTO>(
            model,
            "GET",
            "https://localhost:7200",
            "api/v1",
            "securables",
            "",
            "$orderby=name");

        var result = await Controller.Search(queryOptions);
        var status = result.GetStatusCode();
        var content = result.GetContent();

        Assert.Multiple(() =>
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(status, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(content, Is.AssignableTo<IEnumerable<SecurableDTO>>());
        });

        var mockServ = Provider.GetRequiredService<Mock<ISecurableService>>();
        mockServ.Verify(service => service.GetQueryable(), Times.AtLeastOnce());

        var mockSave = Provider.GetRequiredService<Mock<ISaveService>>();
        mockSave.Verify(service => service.SaveChangesAsync(), Times.Never());
    }

    [Test]
    public async Task Search_IsValid_WithTop_Returns200Ok()
    {
        var model = ODataModel.GetEdmModel(1.0m);
        var queryOptions = ODataQueryOptionsHelper.CreateOptions<SecurableDTO>(
            model,
            "GET",
            "https://localhost:7200",
            "api/v1",
            "securables",
            "",
            "$top=1");

        var result = await Controller.Search(queryOptions);
        var status = result.GetStatusCode();
        var content = result.GetContent();

        Assert.Multiple(() =>
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(status, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(content, Is.AssignableTo<IEnumerable<SecurableDTO>>());
        });

        var mockServ = Provider.GetRequiredService<Mock<ISecurableService>>();
        mockServ.Verify(service => service.GetQueryable(), Times.AtLeastOnce());

        var mockSave = Provider.GetRequiredService<Mock<ISaveService>>();
        mockSave.Verify(service => service.SaveChangesAsync(), Times.Never());
    }

    [Test]
    public async Task Search_IsValid_WithSkip_Returns200Ok()
    {
        var model = ODataModel.GetEdmModel(1.0m);
        var queryOptions = ODataQueryOptionsHelper.CreateOptions<SecurableDTO>(
            model,
            "GET",
            "https://localhost:7200",
            "api/v1",
            "securables",
            "",
            "$skip=1");

        var result = await Controller.Search(queryOptions);
        var status = result.GetStatusCode();
        var content = result.GetContent();

        Assert.Multiple(() =>
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(status, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(content, Is.AssignableTo<IEnumerable<SecurableDTO>>());
        });

        var mockServ = Provider.GetRequiredService<Mock<ISecurableService>>();
        mockServ.Verify(service => service.GetQueryable(), Times.AtLeastOnce());

        var mockSave = Provider.GetRequiredService<Mock<ISaveService>>();
        mockSave.Verify(service => service.SaveChangesAsync(), Times.Never());
    }

    [Test]
    public async Task Search_IsValid_WithCount_Returns200Ok()
    {
        var model = ODataModel.GetEdmModel(1.0m);
        var queryOptions = ODataQueryOptionsHelper.CreateOptions<SecurableDTO>(
            model,
            "GET",
            "https://localhost:7200",
            "api/v1",
            "securables",
            "",
            "$count=true");

        var result = await Controller.Search(queryOptions);
        var status = result.GetStatusCode();
        var content = result.GetContent();

        Assert.Multiple(() =>
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(status, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(content, Is.AssignableTo<IEnumerable<SecurableDTO>>());
        });

        var mockServ = Provider.GetRequiredService<Mock<ISecurableService>>();
        mockServ.Verify(service => service.GetQueryable(), Times.AtLeastOnce());

        var mockSave = Provider.GetRequiredService<Mock<ISaveService>>();
        mockSave.Verify(service => service.SaveChangesAsync(), Times.Never());
    }

    [Test]
    public async Task Get_IsInvalid_Returns404NotFound()
    {
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

        var result = await Controller.Get(recordId, queryOptions);
        var status = result.GetStatusCode();
        var content = result.GetContent();

        Assert.Multiple(() =>
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(status, Is.EqualTo(HttpStatusCode.NotFound));
            Assert.That(content, Is.AssignableTo<ODataError>());
        });

        var mockServ = Provider.GetRequiredService<Mock<ISecurableService>>();
        mockServ.Verify(service => service.GetQueryable(), Times.AtLeastOnce());

        var mockSave = Provider.GetRequiredService<Mock<ISaveService>>();
        mockSave.Verify(service => service.SaveChangesAsync(), Times.Never());
    }

    [Test]
    public async Task Get_IsValid_Returns200Ok()
    {
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

        var result = await Controller.Get(recordId, queryOptions);
        var status = result.GetStatusCode();
        var content = result.GetContent();

        Assert.Multiple(() =>
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(status, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(content, Is.AssignableTo<SecurableDTO>());
        });

        var mockServ = Provider.GetRequiredService<Mock<ISecurableService>>();
        mockServ.Verify(service => service.GetQueryable(), Times.AtLeastOnce());

        var mockSave = Provider.GetRequiredService<Mock<ISaveService>>();
        mockSave.Verify(service => service.SaveChangesAsync(), Times.Never());
    }

    [Test]
    public void Get_IsValid_WithFilter_ThrowsReturnODataErrorException()
    {
        string recordId = "QUERY";

        var model = ODataModel.GetEdmModel(1.0m);
        var queryOptions = ODataQueryOptionsHelper.CreateOptions<SecurableDTO>(
            model,
            "GET",
            "https://localhost:7200",
            "api/v1",
            "securables",
            $"('{recordId}')",
            "$filter=id eq 'QUERY'");

        Assert.ThrowsAsync<ReturnODataErrorException>(async () =>
        {
            var result = await Controller.Get(recordId, queryOptions);
        });
    }

    [Test]
    public async Task Get_IsValid_WithExpand_Returns200Ok()
    {
        string recordId = "QUERY";

        var model = ODataModel.GetEdmModel(1.0m);
        var queryOptions = ODataQueryOptionsHelper.CreateOptions<SecurableDTO>(
            model,
            "GET",
            "https://localhost:7200",
            "api/v1",
            "securables",
            $"('{recordId}')",
            "$expand=permissions");

        var result = await Controller.Get(recordId, queryOptions);
        var status = result.GetStatusCode();
        var content = result.GetContent();

        Assert.Multiple(() =>
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(status, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(content, Is.AssignableTo<ISelectExpandWrapper>());
        });

        var mockServ = Provider.GetRequiredService<Mock<ISecurableService>>();
        mockServ.Verify(service => service.GetQueryable(), Times.AtLeastOnce());

        var mockSave = Provider.GetRequiredService<Mock<ISaveService>>();
        mockSave.Verify(service => service.SaveChangesAsync(), Times.Never());
    }

    [Test]
    public async Task Get_IsValid_WithSelect_Returns200Ok()
    {
        string recordId = "QUERY";

        var model = ODataModel.GetEdmModel(1.0m);
        var queryOptions = ODataQueryOptionsHelper.CreateOptions<SecurableDTO>(
            model,
            "GET",
            "https://localhost:7200",
            "api/v1",
            "securables",
            $"('{recordId}')",
            "$select=id");

        var result = await Controller.Get(recordId, queryOptions);
        var status = result.GetStatusCode();
        var content = result.GetContent();

        Assert.Multiple(() =>
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(status, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(content, Is.AssignableTo<ISelectExpandWrapper>());
        });

        var mockServ = Provider.GetRequiredService<Mock<ISecurableService>>();
        mockServ.Verify(service => service.GetQueryable(), Times.AtLeastOnce());

        var mockSave = Provider.GetRequiredService<Mock<ISaveService>>();
        mockSave.Verify(service => service.SaveChangesAsync(), Times.Never());
    }

    [Test]
    public void Get_IsValid_WithOrderBy_ThrowsReturnODataErrorException()
    {
        string recordId = "QUERY";

        var model = ODataModel.GetEdmModel(1.0m);
        var queryOptions = ODataQueryOptionsHelper.CreateOptions<SecurableDTO>(
            model,
            "GET",
            "https://localhost:7200",
            "api/v1",
            "securables",
            $"('{recordId}')",
            "$orderby=name");

        Assert.ThrowsAsync<ReturnODataErrorException>(async () =>
        {
            var result = await Controller.Get(recordId, queryOptions);
        });
    }

    [Test]
    public void Get_IsValid_WithTop_ThrowsReturnODataErrorException()
    {
        string recordId = "QUERY";

        var model = ODataModel.GetEdmModel(1.0m);
        var queryOptions = ODataQueryOptionsHelper.CreateOptions<SecurableDTO>(
            model,
            "GET",
            "https://localhost:7200",
            "api/v1",
            "securables",
            $"('{recordId}')",
            "$top=1");

        Assert.ThrowsAsync<ReturnODataErrorException>(async () =>
        {
            var result = await Controller.Get(recordId, queryOptions);
        });
    }

    [Test]
    public void Get_IsValid_WithSkip_ThrowsReturnODataErrorException()
    {
        string recordId = "QUERY";

        var model = ODataModel.GetEdmModel(1.0m);
        var queryOptions = ODataQueryOptionsHelper.CreateOptions<SecurableDTO>(
            model,
            "GET",
            "https://localhost:7200",
            "api/v1",
            "securables",
            $"('{recordId}')",
            "$skip=1");

        Assert.ThrowsAsync<ReturnODataErrorException>(async () =>
        {
            var result = await Controller.Get(recordId, queryOptions);
        });
    }

    [Test]
    public void Get_IsValid_WithCount_ThrowsReturnODataErrorException()
    {
        string recordId = "QUERY";

        var model = ODataModel.GetEdmModel(1.0m);
        var queryOptions = ODataQueryOptionsHelper.CreateOptions<SecurableDTO>(
            model,
            "GET",
            "https://localhost:7200",
            "api/v1",
            "securables",
            $"('{recordId}')",
            "$count=true");

        Assert.ThrowsAsync<ReturnODataErrorException>(async () =>
        {
            var result = await Controller.Get(recordId, queryOptions);
        });
    }
}
