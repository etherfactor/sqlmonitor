using EtherGizmos.SqlMonitor.Api.Controllers;
using EtherGizmos.SqlMonitor.Api.Services.Data.Abstractions;
using EtherGizmos.SqlMonitor.Api.UnitTests.Extensions;
using EtherGizmos.SqlMonitor.Models;
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

internal class PermissionsControllerTests
{
    private IServiceProvider Provider { get; set; }

    private PermissionsController Controller { get; set; }

    private List<Permission> Data { get; set; }

    [SetUp]
    public void SetUp()
    {
        Provider = Global.CreateScope();
        Controller = Provider.GetRequiredService<PermissionsController>();
        Data = new List<Permission>()
        {
            new Permission()
            {
                Id = "CREATE",
                Name = "Create"
            },
            new Permission()
            {
                Id = "READ",
                Name = "Read"
            },
            new Permission()
            {
                Id = "UPDATE",
                Name = "Update"
            },
            new Permission()
            {
                Id = "DELETE",
                Name = "Delete"
            }
        };

        var mockData = Data.AsQueryable().BuildMock();

        var mockServ = Provider.GetRequiredService<Mock<IPermissionService>>();
        mockServ.Setup(service => service.GetQueryable()).Returns(mockData);

        var mockSave = Provider.GetRequiredService<Mock<ISaveService>>();
        mockSave.Setup(service => service.SaveChangesAsync()).Returns(Task.CompletedTask);
    }

    [Test]
    public async Task Search_IsValid_Returns200Ok()
    {
        var model = ApiVersions.V0_1.GenerateEdmModel(); ;
        var queryOptions = ODataQueryOptionsHelper.CreateOptions<PermissionDTO>(
            model,
            "GET",
            "https://localhost:7200",
            "api/v0.1",
            "permissions",
            "",
            "");

        var result = await Controller.Search(queryOptions);
        var status = result.GetStatusCode();
        var content = result.GetContent();

        Assert.Multiple(() =>
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(status, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(content, Is.AssignableTo<IEnumerable<PermissionDTO>>());
        });

        var mockServ = Provider.GetRequiredService<Mock<IPermissionService>>();
        mockServ.Verify(service => service.GetQueryable(), Times.AtLeastOnce());

        var mockSave = Provider.GetRequiredService<Mock<ISaveService>>();
        mockSave.Verify(service => service.SaveChangesAsync(), Times.Never());
    }

    [Test]
    public async Task Search_IsValid_WithFilter_Returns200Ok()
    {
        var model = ApiVersions.V0_1.GenerateEdmModel(); ;
        var queryOptions = ODataQueryOptionsHelper.CreateOptions<PermissionDTO>(
            model,
            "GET",
            "https://localhost:7200",
            "api/v0.1",
            "permissions",
            "",
            "$filter=id eq 'CREATE'");

        var result = await Controller.Search(queryOptions);
        var status = result.GetStatusCode();
        var content = result.GetContent();

        Assert.Multiple(() =>
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(status, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(content, Is.AssignableTo<IEnumerable<PermissionDTO>>());
        });

        var mockServ = Provider.GetRequiredService<Mock<IPermissionService>>();
        mockServ.Verify(service => service.GetQueryable(), Times.AtLeastOnce());

        var mockSave = Provider.GetRequiredService<Mock<ISaveService>>();
        mockSave.Verify(service => service.SaveChangesAsync(), Times.Never());
    }

    [Test]
    public async Task Search_IsValid_WithExpand_Returns200Ok()
    {
        var model = ApiVersions.V0_1.GenerateEdmModel(); ;
        var queryOptions = ODataQueryOptionsHelper.CreateOptions<PermissionDTO>(
            model,
            "GET",
            "https://localhost:7200",
            "api/v0.1",
            "permissions",
            "",
            "$expand=securables");

        var result = await Controller.Search(queryOptions);
        var status = result.GetStatusCode();
        var content = result.GetContent();

        Assert.Multiple(() =>
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(status, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(content, Is.AssignableTo<IEnumerable<ISelectExpandWrapper>>());
        });

        var mockServ = Provider.GetRequiredService<Mock<IPermissionService>>();
        mockServ.Verify(service => service.GetQueryable(), Times.AtLeastOnce());

        var mockSave = Provider.GetRequiredService<Mock<ISaveService>>();
        mockSave.Verify(service => service.SaveChangesAsync(), Times.Never());
    }

    [Test]
    public async Task Search_IsValid_WithSelect_Returns200Ok()
    {
        var model = ApiVersions.V0_1.GenerateEdmModel(); ;
        var queryOptions = ODataQueryOptionsHelper.CreateOptions<PermissionDTO>(
            model,
            "GET",
            "https://localhost:7200",
            "api/v0.1",
            "permissions",
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

        var mockServ = Provider.GetRequiredService<Mock<IPermissionService>>();
        mockServ.Verify(service => service.GetQueryable(), Times.AtLeastOnce());

        var mockSave = Provider.GetRequiredService<Mock<ISaveService>>();
        mockSave.Verify(service => service.SaveChangesAsync(), Times.Never());
    }

    [Test]
    public async Task Search_IsValid_WithOrderBy_Returns200Ok()
    {
        var model = ApiVersions.V0_1.GenerateEdmModel(); ;
        var queryOptions = ODataQueryOptionsHelper.CreateOptions<PermissionDTO>(
            model,
            "GET",
            "https://localhost:7200",
            "api/v0.1",
            "permissions",
            "",
            "$orderby=name");

        var result = await Controller.Search(queryOptions);
        var status = result.GetStatusCode();
        var content = result.GetContent();

        Assert.Multiple(() =>
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(status, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(content, Is.AssignableTo<IEnumerable<PermissionDTO>>());
        });

        var mockServ = Provider.GetRequiredService<Mock<IPermissionService>>();
        mockServ.Verify(service => service.GetQueryable(), Times.AtLeastOnce());

        var mockSave = Provider.GetRequiredService<Mock<ISaveService>>();
        mockSave.Verify(service => service.SaveChangesAsync(), Times.Never());
    }

    [Test]
    public async Task Search_IsValid_WithTop_Returns200Ok()
    {
        var model = ApiVersions.V0_1.GenerateEdmModel(); ;
        var queryOptions = ODataQueryOptionsHelper.CreateOptions<PermissionDTO>(
            model,
            "GET",
            "https://localhost:7200",
            "api/v0.1",
            "permissions",
            "",
            "$top=1");

        var result = await Controller.Search(queryOptions);
        var status = result.GetStatusCode();
        var content = result.GetContent();

        Assert.Multiple(() =>
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(status, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(content, Is.AssignableTo<IEnumerable<PermissionDTO>>());
        });

        var mockServ = Provider.GetRequiredService<Mock<IPermissionService>>();
        mockServ.Verify(service => service.GetQueryable(), Times.AtLeastOnce());

        var mockSave = Provider.GetRequiredService<Mock<ISaveService>>();
        mockSave.Verify(service => service.SaveChangesAsync(), Times.Never());
    }

    [Test]
    public async Task Search_IsValid_WithSkip_Returns200Ok()
    {
        var model = ApiVersions.V0_1.GenerateEdmModel(); ;
        var queryOptions = ODataQueryOptionsHelper.CreateOptions<PermissionDTO>(
            model,
            "GET",
            "https://localhost:7200",
            "api/v0.1",
            "permissions",
            "",
            "$skip=1");

        var result = await Controller.Search(queryOptions);
        var status = result.GetStatusCode();
        var content = result.GetContent();

        Assert.Multiple(() =>
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(status, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(content, Is.AssignableTo<IEnumerable<PermissionDTO>>());
        });

        var mockServ = Provider.GetRequiredService<Mock<IPermissionService>>();
        mockServ.Verify(service => service.GetQueryable(), Times.AtLeastOnce());

        var mockSave = Provider.GetRequiredService<Mock<ISaveService>>();
        mockSave.Verify(service => service.SaveChangesAsync(), Times.Never());
    }

    [Test]
    public async Task Search_IsValid_WithCount_Returns200Ok()
    {
        var model = ApiVersions.V0_1.GenerateEdmModel(); ;
        var queryOptions = ODataQueryOptionsHelper.CreateOptions<PermissionDTO>(
            model,
            "GET",
            "https://localhost:7200",
            "api/v0.1",
            "permissions",
            "",
            "$count=true");

        var result = await Controller.Search(queryOptions);
        var status = result.GetStatusCode();
        var content = result.GetContent();

        Assert.Multiple(() =>
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(status, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(content, Is.AssignableTo<IEnumerable<PermissionDTO>>());
        });

        var mockServ = Provider.GetRequiredService<Mock<IPermissionService>>();
        mockServ.Verify(service => service.GetQueryable(), Times.AtLeastOnce());

        var mockSave = Provider.GetRequiredService<Mock<ISaveService>>();
        mockSave.Verify(service => service.SaveChangesAsync(), Times.Never());
    }

    [Test]
    public async Task Get_IsInvalid_Returns404NotFound()
    {
        string recordId = "INVALID";

        var model = ApiVersions.V0_1.GenerateEdmModel(); ;
        var queryOptions = ODataQueryOptionsHelper.CreateOptions<PermissionDTO>(
            model,
            "GET",
            "https://localhost:7200",
            "api/v0.1",
            "permissions",
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

        var mockServ = Provider.GetRequiredService<Mock<IPermissionService>>();
        mockServ.Verify(service => service.GetQueryable(), Times.AtLeastOnce());

        var mockSave = Provider.GetRequiredService<Mock<ISaveService>>();
        mockSave.Verify(service => service.SaveChangesAsync(), Times.Never());
    }

    [Test]
    public async Task Get_IsValid_Returns200Ok()
    {
        string recordId = "CREATE";

        var model = ApiVersions.V0_1.GenerateEdmModel(); ;
        var queryOptions = ODataQueryOptionsHelper.CreateOptions<PermissionDTO>(
            model,
            "GET",
            "https://localhost:7200",
            "api/v0.1",
            "permissions",
            $"('{recordId}')",
            "");

        var result = await Controller.Get(recordId, queryOptions);
        var status = result.GetStatusCode();
        var content = result.GetContent();

        Assert.Multiple(() =>
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(status, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(content, Is.AssignableTo<PermissionDTO>());
        });

        var mockServ = Provider.GetRequiredService<Mock<IPermissionService>>();
        mockServ.Verify(service => service.GetQueryable(), Times.AtLeastOnce());

        var mockSave = Provider.GetRequiredService<Mock<ISaveService>>();
        mockSave.Verify(service => service.SaveChangesAsync(), Times.Never());
    }

    [Test]
    public void Get_IsValid_WithFilter_ThrowsReturnODataErrorException()
    {
        string recordId = "CREATE";

        var model = ApiVersions.V0_1.GenerateEdmModel(); ;
        var queryOptions = ODataQueryOptionsHelper.CreateOptions<PermissionDTO>(
            model,
            "GET",
            "https://localhost:7200",
            "api/v0.1",
            "permissions",
            $"('{recordId}')",
            "$filter=id eq 'CREATE'");

        Assert.ThrowsAsync<ReturnODataErrorException>(async () =>
        {
            var result = await Controller.Get(recordId, queryOptions);
        });
    }

    [Test]
    public async Task Get_IsValid_WithExpand_Returns200Ok()
    {
        string recordId = "CREATE";

        var model = ApiVersions.V0_1.GenerateEdmModel(); ;
        var queryOptions = ODataQueryOptionsHelper.CreateOptions<PermissionDTO>(
            model,
            "GET",
            "https://localhost:7200",
            "api/v0.1",
            "permissions",
            $"('{recordId}')",
            "$expand=securables");

        var result = await Controller.Get(recordId, queryOptions);
        var status = result.GetStatusCode();
        var content = result.GetContent();

        Assert.Multiple(() =>
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(status, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(content, Is.AssignableTo<ISelectExpandWrapper>());
        });

        var mockServ = Provider.GetRequiredService<Mock<IPermissionService>>();
        mockServ.Verify(service => service.GetQueryable(), Times.AtLeastOnce());

        var mockSave = Provider.GetRequiredService<Mock<ISaveService>>();
        mockSave.Verify(service => service.SaveChangesAsync(), Times.Never());
    }

    [Test]
    public async Task Get_IsValid_WithSelect_Returns200Ok()
    {
        string recordId = "CREATE";

        var model = ApiVersions.V0_1.GenerateEdmModel(); ;
        var queryOptions = ODataQueryOptionsHelper.CreateOptions<PermissionDTO>(
            model,
            "GET",
            "https://localhost:7200",
            "api/v0.1",
            "permissions",
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

        var mockServ = Provider.GetRequiredService<Mock<IPermissionService>>();
        mockServ.Verify(service => service.GetQueryable(), Times.AtLeastOnce());

        var mockSave = Provider.GetRequiredService<Mock<ISaveService>>();
        mockSave.Verify(service => service.SaveChangesAsync(), Times.Never());
    }

    [Test]
    public void Get_IsValid_WithOrderBy_ThrowsReturnODataErrorException()
    {
        string recordId = "CREATE";

        var model = ApiVersions.V0_1.GenerateEdmModel(); ;
        var queryOptions = ODataQueryOptionsHelper.CreateOptions<PermissionDTO>(
            model,
            "GET",
            "https://localhost:7200",
            "api/v0.1",
            "permissions",
            $"('{recordId}')",
            "$orderby=id");

        Assert.ThrowsAsync<ReturnODataErrorException>(async () =>
        {
            var result = await Controller.Get(recordId, queryOptions);
        });
    }

    [Test]
    public void Get_IsValid_WithTop_ThrowsReturnODataErrorException()
    {
        string recordId = "CREATE";

        var model = ApiVersions.V0_1.GenerateEdmModel(); ;
        var queryOptions = ODataQueryOptionsHelper.CreateOptions<PermissionDTO>(
            model,
            "GET",
            "https://localhost:7200",
            "api/v0.1",
            "permissions",
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
        string recordId = "CREATE";

        var model = ApiVersions.V0_1.GenerateEdmModel(); ;
        var queryOptions = ODataQueryOptionsHelper.CreateOptions<PermissionDTO>(
            model,
            "GET",
            "https://localhost:7200",
            "api/v0.1",
            "permissions",
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
        string recordId = "CREATE";

        var model = ApiVersions.V0_1.GenerateEdmModel(); ;
        var queryOptions = ODataQueryOptionsHelper.CreateOptions<PermissionDTO>(
            model,
            "GET",
            "https://localhost:7200",
            "api/v0.1",
            "permissions",
            $"('{recordId}')",
            "$count=true");

        Assert.ThrowsAsync<ReturnODataErrorException>(async () =>
        {
            var result = await Controller.Get(recordId, queryOptions);
        });
    }
}
