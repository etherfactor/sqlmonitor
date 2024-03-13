using EtherGizmos.SqlMonitor.Api.Controllers;
using EtherGizmos.SqlMonitor.Api.Services.Data.Abstractions;
using EtherGizmos.SqlMonitor.Api.UnitTests.Extensions;
using EtherGizmos.SqlMonitor.Models;
using EtherGizmos.SqlMonitor.Models.Api.v1;
using EtherGizmos.SqlMonitor.Models.Database;
using EtherGizmos.SqlMonitor.Models.Exceptions;
using Microsoft.AspNetCore.OData.Deltas;
using Microsoft.AspNetCore.OData.Query.Wrapper;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OData;
using MockQueryable.Moq;
using Moq;
using System.Net;

namespace EtherGizmos.SqlMonitor.Api.UnitTests.Controllers;

internal class MonitoredSystemsControllerTests
{
    private IServiceProvider Provider { get; set; }

    private MonitoredSystemsController Controller { get; set; }

    private List<MonitoredSystem> Data { get; set; }

    private Guid RecordId { get; } = Guid.NewGuid();

    [SetUp]
    public void SetUp()
    {
        Provider = Global.CreateScope();
        Controller = Provider.GetRequiredService<MonitoredSystemsController>();
        Data = new List<MonitoredSystem>()
        {
            new MonitoredSystem()
            {
                Id = RecordId,
                Name = "Test 1",
                Description = null,
                IsSoftDeleted = false,
            },
            new MonitoredSystem()
            {
                Id = Guid.NewGuid(),
                Name = "Test 2",
                Description = null,
                IsSoftDeleted = false,
            },
            new MonitoredSystem()
            {
                Id = Guid.NewGuid(),
                Name = "Test 3",
                Description = null,
                IsSoftDeleted = false,
            },
            new MonitoredSystem()
            {
                Id = Guid.NewGuid(),
                Name = "Test 4",
                Description = null,
                IsSoftDeleted = true,
            }
        };

        var mockData = Data.AsQueryable().BuildMock();

        var mockServ = Provider.GetRequiredService<Mock<IMonitoredSystemService>>();
        mockServ.Setup(service => service.GetQueryable()).Returns(mockData);

        var mockSave = Provider.GetRequiredService<Mock<ISaveService>>();
        mockSave.Setup(service => service.SaveChangesAsync()).Returns(Task.CompletedTask);
    }

    [Test]
    public async Task Search_IsValid_Returns200Ok()
    {
        var model = ApiVersions.V0_1.GenerateEdmModel(); ;
        var queryOptions = ODataQueryOptionsHelper.CreateOptions<MonitoredSystemDTO>(
            model,
            "GET",
            "https://localhost:7200",
            "api/v0.1",
            "monitoredSystems",
            "",
            "");

        var result = await Controller.Search(queryOptions);
        var status = result.GetStatusCode();
        var content = result.GetContent();

        Assert.Multiple(() =>
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(status, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(content, Is.AssignableTo<IEnumerable<MonitoredSystemDTO>>());
        });

        var mockServ = Provider.GetRequiredService<Mock<IMonitoredSystemService>>();
        mockServ.Verify(service => service.GetQueryable(), Times.AtLeastOnce());

        var mockSave = Provider.GetRequiredService<Mock<ISaveService>>();
        mockSave.Verify(service => service.SaveChangesAsync(), Times.Never());
    }

    [Test]
    public async Task Search_IsValid_WithFilter_Returns200Ok()
    {
        var model = ApiVersions.V0_1.GenerateEdmModel(); ;
        var queryOptions = ODataQueryOptionsHelper.CreateOptions<MonitoredSystemDTO>(
            model,
            "GET",
            "https://localhost:7200",
            "api/v0.1",
            "monitoredSystems",
            "",
            $"$filter=id eq {RecordId}");

        var result = await Controller.Search(queryOptions);
        var status = result.GetStatusCode();
        var content = result.GetContent();

        Assert.Multiple(() =>
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(status, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(content, Is.AssignableTo<IEnumerable<MonitoredSystemDTO>>());
        });

        var mockServ = Provider.GetRequiredService<Mock<IMonitoredSystemService>>();
        mockServ.Verify(service => service.GetQueryable(), Times.AtLeastOnce());

        var mockSave = Provider.GetRequiredService<Mock<ISaveService>>();
        mockSave.Verify(service => service.SaveChangesAsync(), Times.Never());
    }

    [Test]
    public async Task Search_IsValid_WithSelect_Returns200Ok()
    {
        var model = ApiVersions.V0_1.GenerateEdmModel(); ;
        var queryOptions = ODataQueryOptionsHelper.CreateOptions<MonitoredSystemDTO>(
            model,
            "GET",
            "https://localhost:7200",
            "api/v0.1",
            "monitoredSystems",
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

        var mockServ = Provider.GetRequiredService<Mock<IMonitoredSystemService>>();
        mockServ.Verify(service => service.GetQueryable(), Times.AtLeastOnce());

        var mockSave = Provider.GetRequiredService<Mock<ISaveService>>();
        mockSave.Verify(service => service.SaveChangesAsync(), Times.Never());
    }

    [Test]
    public async Task Search_IsValid_WithOrderBy_Returns200Ok()
    {
        var model = ApiVersions.V0_1.GenerateEdmModel(); ;
        var queryOptions = ODataQueryOptionsHelper.CreateOptions<MonitoredSystemDTO>(
            model,
            "GET",
            "https://localhost:7200",
            "api/v0.1",
            "monitoredSystems",
            "",
            "$orderby=name");

        var result = await Controller.Search(queryOptions);
        var status = result.GetStatusCode();
        var content = result.GetContent();

        Assert.Multiple(() =>
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(status, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(content, Is.AssignableTo<IEnumerable<MonitoredSystemDTO>>());
        });

        var mockServ = Provider.GetRequiredService<Mock<IMonitoredSystemService>>();
        mockServ.Verify(service => service.GetQueryable(), Times.AtLeastOnce());

        var mockSave = Provider.GetRequiredService<Mock<ISaveService>>();
        mockSave.Verify(service => service.SaveChangesAsync(), Times.Never());
    }

    [Test]
    public async Task Search_IsValid_WithTop_Returns200Ok()
    {
        var model = ApiVersions.V0_1.GenerateEdmModel(); ;
        var queryOptions = ODataQueryOptionsHelper.CreateOptions<MonitoredSystemDTO>(
            model,
            "GET",
            "https://localhost:7200",
            "api/v0.1",
            "monitoredSystems",
            "",
            "$top=1");

        var result = await Controller.Search(queryOptions);
        var status = result.GetStatusCode();
        var content = result.GetContent();

        Assert.Multiple(() =>
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(status, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(content, Is.AssignableTo<IEnumerable<MonitoredSystemDTO>>());
        });

        var mockServ = Provider.GetRequiredService<Mock<IMonitoredSystemService>>();
        mockServ.Verify(service => service.GetQueryable(), Times.AtLeastOnce());

        var mockSave = Provider.GetRequiredService<Mock<ISaveService>>();
        mockSave.Verify(service => service.SaveChangesAsync(), Times.Never());
    }

    [Test]
    public async Task Search_IsValid_WithSkip_Returns200Ok()
    {
        var model = ApiVersions.V0_1.GenerateEdmModel(); ;
        var queryOptions = ODataQueryOptionsHelper.CreateOptions<MonitoredSystemDTO>(
            model,
            "GET",
            "https://localhost:7200",
            "api/v0.1",
            "monitoredSystems",
            "",
            "$skip=1");

        var result = await Controller.Search(queryOptions);
        var status = result.GetStatusCode();
        var content = result.GetContent();

        Assert.Multiple(() =>
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(status, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(content, Is.AssignableTo<IEnumerable<MonitoredSystemDTO>>());
        });

        var mockServ = Provider.GetRequiredService<Mock<IMonitoredSystemService>>();
        mockServ.Verify(service => service.GetQueryable(), Times.AtLeastOnce());

        var mockSave = Provider.GetRequiredService<Mock<ISaveService>>();
        mockSave.Verify(service => service.SaveChangesAsync(), Times.Never());
    }

    [Test]
    public async Task Search_IsValid_WithCount_Returns200Ok()
    {
        var model = ApiVersions.V0_1.GenerateEdmModel(); ;
        var queryOptions = ODataQueryOptionsHelper.CreateOptions<MonitoredSystemDTO>(
            model,
            "GET",
            "https://localhost:7200",
            "api/v0.1",
            "monitoredSystems",
            "",
            "$count=true");

        var result = await Controller.Search(queryOptions);
        var status = result.GetStatusCode();
        var content = result.GetContent();

        Assert.Multiple(() =>
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(status, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(content, Is.AssignableTo<IEnumerable<MonitoredSystemDTO>>());
        });

        var mockServ = Provider.GetRequiredService<Mock<IMonitoredSystemService>>();
        mockServ.Verify(service => service.GetQueryable(), Times.AtLeastOnce());

        var mockSave = Provider.GetRequiredService<Mock<ISaveService>>();
        mockSave.Verify(service => service.SaveChangesAsync(), Times.Never());
    }

    [Test]
    public async Task Get_IsInvalid_Returns404NotFound()
    {
        Guid recordId = new Guid();

        var model = ApiVersions.V0_1.GenerateEdmModel(); ;
        var queryOptions = ODataQueryOptionsHelper.CreateOptions<MonitoredSystemDTO>(
            model,
            "GET",
            "https://localhost:7200",
            "api/v0.1",
            "monitoredSystems",
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

        var mockServ = Provider.GetRequiredService<Mock<IMonitoredSystemService>>();
        mockServ.Verify(service => service.GetQueryable(), Times.AtLeastOnce());

        var mockSave = Provider.GetRequiredService<Mock<ISaveService>>();
        mockSave.Verify(service => service.SaveChangesAsync(), Times.Never());
    }

    [Test]
    public async Task Get_IsValid_Returns200Ok()
    {
        Guid recordId = RecordId;

        var model = ApiVersions.V0_1.GenerateEdmModel(); ;
        var queryOptions = ODataQueryOptionsHelper.CreateOptions<MonitoredSystemDTO>(
            model,
            "GET",
            "https://localhost:7200",
            "api/v0.1",
            "monitoredSystems",
            $"('{recordId}')",
            "");

        var result = await Controller.Get(recordId, queryOptions);
        var status = result.GetStatusCode();
        var content = result.GetContent();

        Assert.Multiple(() =>
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(status, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(content, Is.AssignableTo<MonitoredSystemDTO>());
        });

        var mockServ = Provider.GetRequiredService<Mock<IMonitoredSystemService>>();
        mockServ.Verify(service => service.GetQueryable(), Times.AtLeastOnce());

        var mockSave = Provider.GetRequiredService<Mock<ISaveService>>();
        mockSave.Verify(service => service.SaveChangesAsync(), Times.Never());
    }

    [Test]
    public void Get_IsValid_WithFilter_ThrowsReturnODataErrorException()
    {
        Guid recordId = RecordId;

        var model = ApiVersions.V0_1.GenerateEdmModel(); ;
        var queryOptions = ODataQueryOptionsHelper.CreateOptions<MonitoredSystemDTO>(
            model,
            "GET",
            "https://localhost:7200",
            "api/v0.1",
            "monitoredSystems",
            $"('{recordId}')",
            "$filter=id eq 00000000-0000-0000-0000-000000000000");

        Assert.ThrowsAsync<ReturnODataErrorException>(async () =>
        {
            var result = await Controller.Get(recordId, queryOptions);
        });
    }

    [Test]
    public async Task Get_IsValid_WithSelect_Returns200Ok()
    {
        Guid recordId = RecordId;

        var model = ApiVersions.V0_1.GenerateEdmModel(); ;
        var queryOptions = ODataQueryOptionsHelper.CreateOptions<MonitoredSystemDTO>(
            model,
            "GET",
            "https://localhost:7200",
            "api/v0.1",
            "monitoredSystems",
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

        var mockServ = Provider.GetRequiredService<Mock<IMonitoredSystemService>>();
        mockServ.Verify(service => service.GetQueryable(), Times.AtLeastOnce());

        var mockSave = Provider.GetRequiredService<Mock<ISaveService>>();
        mockSave.Verify(service => service.SaveChangesAsync(), Times.Never());
    }

    [Test]
    public void Get_IsValid_WithOrderBy_ThrowsReturnODataErrorException()
    {
        Guid recordId = RecordId;

        var model = ApiVersions.V0_1.GenerateEdmModel(); ;
        var queryOptions = ODataQueryOptionsHelper.CreateOptions<MonitoredSystemDTO>(
            model,
            "GET",
            "https://localhost:7200",
            "api/v0.1",
            "monitoredSystems",
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
        Guid recordId = RecordId;

        var model = ApiVersions.V0_1.GenerateEdmModel(); ;
        var queryOptions = ODataQueryOptionsHelper.CreateOptions<MonitoredSystemDTO>(
            model,
            "GET",
            "https://localhost:7200",
            "api/v0.1",
            "monitoredSystems",
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
        Guid recordId = RecordId;

        var model = ApiVersions.V0_1.GenerateEdmModel(); ;
        var queryOptions = ODataQueryOptionsHelper.CreateOptions<MonitoredSystemDTO>(
            model,
            "GET",
            "https://localhost:7200",
            "api/v0.1",
            "monitoredSystems",
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
        Guid recordId = RecordId;

        var model = ApiVersions.V0_1.GenerateEdmModel(); ;
        var queryOptions = ODataQueryOptionsHelper.CreateOptions<MonitoredSystemDTO>(
            model,
            "GET",
            "https://localhost:7200",
            "api/v0.1",
            "monitoredSystems",
            $"('{recordId}')",
            "$count=true");

        Assert.ThrowsAsync<ReturnODataErrorException>(async () =>
        {
            var result = await Controller.Get(recordId, queryOptions);
        });
    }

    [Test]
    public async Task Create_IsValid_Returns201Created()
    {
        var model = ApiVersions.V0_1.GenerateEdmModel();
        var queryOptions = ODataQueryOptionsHelper.CreateOptions<MonitoredSystemDTO>(
            model,
            "POST",
            "https://localhost:7200",
            "api/v0.1",
            "monitoredSystems",
            "",
            "");

        var record = new MonitoredSystemDTO()
        {
            Name = "Test",
        };

        var result = await Controller.Create(record, queryOptions);
        var status = result.GetStatusCode();
        var content = result.GetContent();

        Assert.Multiple(() =>
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(status, Is.EqualTo(HttpStatusCode.Created));
            Assert.That(content, Is.AssignableTo<MonitoredSystemDTO>());
        });

        var mockServ = Provider.GetRequiredService<Mock<IMonitoredSystemService>>();
        mockServ.Verify(service => service.GetQueryable(), Times.AtLeastOnce());

        var mockSave = Provider.GetRequiredService<Mock<ISaveService>>();
        mockSave.Verify(service => service.SaveChangesAsync(), Times.Once());
    }

    [Test]
    public void Create_IsValid_WithFilter_ThrowsReturnODataErrorException()
    {
        var model = ApiVersions.V0_1.GenerateEdmModel(); ;
        var queryOptions = ODataQueryOptionsHelper.CreateOptions<MonitoredSystemDTO>(
            model,
            "GET",
            "https://localhost:7200",
            "api/v0.1",
            "monitoredSystems",
            "",
            $"$filter=id eq {RecordId}");

        var record = new MonitoredSystemDTO()
        {
            Name = "Test",
        };

        Assert.ThrowsAsync<ReturnODataErrorException>(async () =>
        {
            var result = await Controller.Create(record, queryOptions);
        });
    }

    [Test]
    public async Task Create_IsValid_WithSelect_Returns201Created()
    {
        var model = ApiVersions.V0_1.GenerateEdmModel(); ;
        var queryOptions = ODataQueryOptionsHelper.CreateOptions<MonitoredSystemDTO>(
            model,
            "GET",
            "https://localhost:7200",
            "api/v0.1",
            "monitoredSystems",
            "",
            "$select=id");

        var record = new MonitoredSystemDTO()
        {
            Name = "Test",
        };

        var result = await Controller.Create(record, queryOptions);
        var status = result.GetStatusCode();
        var content = result.GetContent();

        Assert.Multiple(() =>
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(status, Is.EqualTo(HttpStatusCode.Created));
            Assert.That(content, Is.AssignableTo<ISelectExpandWrapper>());
        });

        var mockServ = Provider.GetRequiredService<Mock<IMonitoredSystemService>>();
        mockServ.Verify(service => service.GetQueryable(), Times.AtLeastOnce());

        var mockSave = Provider.GetRequiredService<Mock<ISaveService>>();
        mockSave.Verify(service => service.SaveChangesAsync(), Times.Once());
    }

    [Test]
    public void Create_IsValid_WithOrderBy_ThrowsReturnODataErrorException()
    {
        var model = ApiVersions.V0_1.GenerateEdmModel(); ;
        var queryOptions = ODataQueryOptionsHelper.CreateOptions<MonitoredSystemDTO>(
            model,
            "GET",
            "https://localhost:7200",
            "api/v0.1",
            "monitoredSystems",
            "",
            "$orderby=id");

        var record = new MonitoredSystemDTO()
        {
            Name = "Test",
        };

        Assert.ThrowsAsync<ReturnODataErrorException>(async () =>
        {
            var result = await Controller.Create(record, queryOptions);
        });
    }

    [Test]
    public void Create_IsValid_WithTop_ThrowsReturnODataErrorException()
    {
        var model = ApiVersions.V0_1.GenerateEdmModel(); ;
        var queryOptions = ODataQueryOptionsHelper.CreateOptions<MonitoredSystemDTO>(
            model,
            "GET",
            "https://localhost:7200",
            "api/v0.1",
            "monitoredSystems",
            "",
            "$top=1");

        var record = new MonitoredSystemDTO()
        {
            Name = "Test",
        };

        Assert.ThrowsAsync<ReturnODataErrorException>(async () =>
        {
            var result = await Controller.Create(record, queryOptions);
        });
    }

    [Test]
    public void Create_IsValid_WithSkip_ThrowsReturnODataErrorException()
    {
        var model = ApiVersions.V0_1.GenerateEdmModel(); ;
        var queryOptions = ODataQueryOptionsHelper.CreateOptions<MonitoredSystemDTO>(
            model,
            "GET",
            "https://localhost:7200",
            "api/v0.1",
            "monitoredSystems",
            "",
            "$skip=1");

        var record = new MonitoredSystemDTO()
        {
            Name = "Test",
        };

        Assert.ThrowsAsync<ReturnODataErrorException>(async () =>
        {
            var result = await Controller.Create(record, queryOptions);
        });
    }

    [Test]
    public void Create_IsValid_WithCount_ThrowsReturnODataErrorException()
    {
        var model = ApiVersions.V0_1.GenerateEdmModel(); ;
        var queryOptions = ODataQueryOptionsHelper.CreateOptions<MonitoredSystemDTO>(
            model,
            "GET",
            "https://localhost:7200",
            "api/v0.1",
            "monitoredSystems",
            "",
            "$count=true");

        var record = new MonitoredSystemDTO()
        {
            Name = "Test",
        };

        Assert.ThrowsAsync<ReturnODataErrorException>(async () =>
        {
            var result = await Controller.Create(record, queryOptions);
        });
    }

    [Test]
    public async Task Update_IsInvalid_Returns404NotFound()
    {
        var recordId = Guid.NewGuid();

        var model = ApiVersions.V0_1.GenerateEdmModel(); ;
        var queryOptions = ODataQueryOptionsHelper.CreateOptions<MonitoredSystemDTO>(
            model,
            "PATCH",
            "https://localhost:7200",
            "api/v0.1",
            "monitoredSystems",
            $"({recordId})",
            "");

        var record = new Delta<MonitoredSystemDTO>();
        record.TrySetPropertyValue(nameof(MonitoredSystemDTO.Name), "New Name");

        var result = await Controller.Update(recordId, record, queryOptions);
        var status = result.GetStatusCode();
        var content = result.GetContent();

        Assert.Multiple(() =>
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(status, Is.EqualTo(HttpStatusCode.NotFound));
            Assert.That(content, Is.AssignableTo<ODataError>());
        });

        var mockServ = Provider.GetRequiredService<Mock<IMonitoredSystemService>>();
        mockServ.Verify(service => service.GetQueryable(), Times.AtLeastOnce());

        var mockSave = Provider.GetRequiredService<Mock<ISaveService>>();
        mockSave.Verify(service => service.SaveChangesAsync(), Times.Never());
    }

    [Test]
    public async Task Update_IsValid_Returns200Ok()
    {
        var recordId = RecordId;

        var model = ApiVersions.V0_1.GenerateEdmModel(); ;
        var queryOptions = ODataQueryOptionsHelper.CreateOptions<MonitoredSystemDTO>(
            model,
            "POST",
            "https://localhost:7200",
            "api/v0.1",
            "monitoredSystems",
            $"({recordId})",
            "");

        var record = new Delta<MonitoredSystemDTO>();
        record.TrySetPropertyValue(nameof(MonitoredSystemDTO.Name), "New Name");

        var result = await Controller.Update(recordId, record, queryOptions);
        var status = result.GetStatusCode();
        var content = result.GetContent();

        Assert.Multiple(() =>
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(status, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(content, Is.AssignableTo<MonitoredSystemDTO>());
        });

        var mockServ = Provider.GetRequiredService<Mock<IMonitoredSystemService>>();
        mockServ.Verify(service => service.GetQueryable(), Times.AtLeastOnce());

        var mockSave = Provider.GetRequiredService<Mock<ISaveService>>();
        mockSave.Verify(service => service.SaveChangesAsync(), Times.Once());
    }

    [Test]
    public void Update_IsValid_WithFilter_ThrowsReturnODataErrorException()
    {
        var recordId = RecordId;

        var model = ApiVersions.V0_1.GenerateEdmModel(); ;
        var queryOptions = ODataQueryOptionsHelper.CreateOptions<MonitoredSystemDTO>(
            model,
            "GET",
            "https://localhost:7200",
            "api/v0.1",
            "monitoredSystems",
            $"({recordId})",
            $"$filter=id eq {RecordId}");

        var record = new Delta<MonitoredSystemDTO>();
        record.TrySetPropertyValue(nameof(MonitoredSystemDTO.Name), "New Name");

        Assert.ThrowsAsync<ReturnODataErrorException>(async () =>
        {
            var result = await Controller.Update(recordId, record, queryOptions);
        });
    }

    [Test]
    public async Task Update_IsValid_WithSelect_Returns200Ok()
    {
        var recordId = RecordId;

        var model = ApiVersions.V0_1.GenerateEdmModel(); ;
        var queryOptions = ODataQueryOptionsHelper.CreateOptions<MonitoredSystemDTO>(
            model,
            "GET",
            "https://localhost:7200",
            "api/v0.1",
            "monitoredSystems",
            $"({recordId})",
            "$select=id");

        var record = new Delta<MonitoredSystemDTO>();
        record.TrySetPropertyValue(nameof(MonitoredSystemDTO.Name), "New Name");

        var result = await Controller.Update(recordId, record, queryOptions);
        var status = result.GetStatusCode();
        var content = result.GetContent();

        Assert.Multiple(() =>
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(status, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(content, Is.AssignableTo<ISelectExpandWrapper>());
        });

        var mockServ = Provider.GetRequiredService<Mock<IMonitoredSystemService>>();
        mockServ.Verify(service => service.GetQueryable(), Times.AtLeastOnce());

        var mockSave = Provider.GetRequiredService<Mock<ISaveService>>();
        mockSave.Verify(service => service.SaveChangesAsync(), Times.Once());
    }

    [Test]
    public void Update_IsValid_WithOrderBy_ThrowsReturnODataErrorException()
    {
        var recordId = RecordId;

        var model = ApiVersions.V0_1.GenerateEdmModel(); ;
        var queryOptions = ODataQueryOptionsHelper.CreateOptions<MonitoredSystemDTO>(
            model,
            "GET",
            "https://localhost:7200",
            "api/v0.1",
            "monitoredSystems",
            $"({RecordId})",
            "$orderby=id");

        var record = new Delta<MonitoredSystemDTO>();
        record.TrySetPropertyValue(nameof(MonitoredSystemDTO.Name), "New Name");

        Assert.ThrowsAsync<ReturnODataErrorException>(async () =>
        {
            var result = await Controller.Update(recordId, record, queryOptions);
        });
    }

    [Test]
    public void Update_IsValid_WithTop_ThrowsReturnODataErrorException()
    {
        var recordId = RecordId;

        var model = ApiVersions.V0_1.GenerateEdmModel(); ;
        var queryOptions = ODataQueryOptionsHelper.CreateOptions<MonitoredSystemDTO>(
            model,
            "GET",
            "https://localhost:7200",
            "api/v0.1",
            "monitoredSystems",
            $"({recordId})",
            "$top=1");

        var record = new Delta<MonitoredSystemDTO>();
        record.TrySetPropertyValue(nameof(MonitoredSystemDTO.Name), "New Name");

        Assert.ThrowsAsync<ReturnODataErrorException>(async () =>
        {
            var result = await Controller.Update(recordId, record, queryOptions);
        });
    }

    [Test]
    public void Update_IsValid_WithSkip_ThrowsReturnODataErrorException()
    {
        var recordId = RecordId;

        var model = ApiVersions.V0_1.GenerateEdmModel(); ;
        var queryOptions = ODataQueryOptionsHelper.CreateOptions<MonitoredSystemDTO>(
            model,
            "GET",
            "https://localhost:7200",
            "api/v0.1",
            "monitoredSystems",
            $"({recordId})",
            "$skip=1");

        var record = new Delta<MonitoredSystemDTO>();
        record.TrySetPropertyValue(nameof(MonitoredSystemDTO.Name), "New Name");

        Assert.ThrowsAsync<ReturnODataErrorException>(async () =>
        {
            var result = await Controller.Update(recordId, record, queryOptions);
        });
    }

    [Test]
    public void Update_IsValid_WithCount_ThrowsReturnODataErrorException()
    {
        var recordId = RecordId;

        var model = ApiVersions.V0_1.GenerateEdmModel(); ;
        var queryOptions = ODataQueryOptionsHelper.CreateOptions<MonitoredSystemDTO>(
            model,
            "GET",
            "https://localhost:7200",
            "api/v0.1",
            "monitoredSystems",
            $"({recordId})",
            "$count=true");

        var record = new Delta<MonitoredSystemDTO>();
        record.TrySetPropertyValue(nameof(MonitoredSystemDTO.Name), "New Name");

        Assert.ThrowsAsync<ReturnODataErrorException>(async () =>
        {
            var result = await Controller.Update(recordId, record, queryOptions);
        });
    }

    [Test]
    public async Task Delete_IsInvalid_Returns404NotFound()
    {
        Guid recordId = new Guid();

        var model = ApiVersions.V0_1.GenerateEdmModel(); ;
        var queryOptions = ODataQueryOptionsHelper.CreateOptions<MonitoredSystemDTO>(
            model,
            "DELETE",
            "https://localhost:7200",
            "api/v0.1",
            "monitoredSystems",
            $"('{recordId}')",
            "");

        var result = await Controller.Delete(recordId);
        var status = result.GetStatusCode();
        var content = result.GetContent();

        Assert.Multiple(() =>
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(status, Is.EqualTo(HttpStatusCode.NotFound));
            Assert.That(content, Is.AssignableTo<ODataError>());
        });

        var mockServ = Provider.GetRequiredService<Mock<IMonitoredSystemService>>();
        mockServ.Verify(service => service.GetQueryable(), Times.AtLeastOnce());

        var mockSave = Provider.GetRequiredService<Mock<ISaveService>>();
        mockSave.Verify(service => service.SaveChangesAsync(), Times.Never());
    }

    [Test]
    public async Task Delete_IsValid_Returns204NoContent()
    {
        Guid recordId = RecordId;

        var model = ApiVersions.V0_1.GenerateEdmModel(); ;
        var queryOptions = ODataQueryOptionsHelper.CreateOptions<MonitoredSystemDTO>(
            model,
            "DELETE",
            "https://localhost:7200",
            "api/v0.1",
            "monitoredSystems",
            $"('{recordId}')",
            "");

        var result = await Controller.Delete(recordId);
        var status = result.GetStatusCode();
        var content = result.GetContent();

        Assert.Multiple(() =>
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(status, Is.EqualTo(HttpStatusCode.NoContent));
            Assert.That(content, Is.Null);
        });

        var mockServ = Provider.GetRequiredService<Mock<IMonitoredSystemService>>();
        mockServ.Verify(service => service.GetQueryable(), Times.AtLeastOnce());

        var mockSave = Provider.GetRequiredService<Mock<ISaveService>>();
        mockSave.Verify(service => service.SaveChangesAsync(), Times.Once());
    }
}
