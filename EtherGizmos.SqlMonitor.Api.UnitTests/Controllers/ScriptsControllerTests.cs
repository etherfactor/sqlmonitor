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

internal class ScriptsControllerTests
{
    private IServiceProvider _provider;

    private ScriptsController _controller;

    private List<Script> _data;

    private readonly Guid _recordId = Guid.NewGuid();

    [SetUp]
    public void SetUp()
    {
        _provider = Global.CreateScope();
        _controller = _provider.GetRequiredService<ScriptsController>();
        _data = new List<Script>()
        {
            new()
            {
                Id = _recordId,
                Name = "Test 1",
                Description = null,
                IsSoftDeleted = false,
            },
            new()
            {
                Id = Guid.NewGuid(),
                Name = "Test 2",
                Description = null,
                IsSoftDeleted = false,
            },
            new()
            {
                Id = Guid.NewGuid(),
                Name = "Test 3",
                Description = null,
                IsSoftDeleted = false,
            },
            new()
            {
                Id = Guid.NewGuid(),
                Name = "Test 4",
                Description = null,
                IsSoftDeleted = true,
            }
        };

        var mockData = _data.AsQueryable().BuildMock();

        var mockServ = _provider.GetRequiredService<Mock<IScriptService>>();
        mockServ.Setup(service => service.GetQueryable()).Returns(mockData);

        var metricData = new List<Metric>()
        {
            new()
            {
                Id = 1,
                Name = "Test Metric",
            },
        };

        var mockMetricData = metricData.AsQueryable().BuildMock();

        var mockMetricServ = _provider.GetRequiredService<Mock<IMetricService>>();
        mockMetricServ.Setup(service => service.GetQueryable()).Returns(mockMetricData);

        var mockSave = _provider.GetRequiredService<Mock<ISaveService>>();
        mockSave.Setup(service => service.SaveChangesAsync()).Returns(Task.CompletedTask);
    }

    [Test]
    public async Task Search_IsValid_Returns200Ok()
    {
        var model = ApiVersions.V0_1.GenerateEdmModel(); ;
        var queryOptions = ODataQueryOptionsHelper.CreateOptions<ScriptDTO>(
            model,
            "GET",
            "https://localhost:7200",
            "api/v0.1",
            "scripts",
            "",
            "");

        var result = await _controller.Search(queryOptions);
        var status = result.GetStatusCode();
        var content = result.GetContent();

        Assert.Multiple(() =>
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(status, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(content, Is.AssignableTo<IEnumerable<ScriptDTO>>());
        });

        var mockServ = _provider.GetRequiredService<Mock<IScriptService>>();
        mockServ.Verify(service => service.GetQueryable(), Times.AtLeastOnce());

        var mockSave = _provider.GetRequiredService<Mock<ISaveService>>();
        mockSave.Verify(service => service.SaveChangesAsync(), Times.Never());
    }

    [Test]
    public async Task Search_IsValid_WithFilter_Returns200Ok()
    {
        var model = ApiVersions.V0_1.GenerateEdmModel(); ;
        var queryOptions = ODataQueryOptionsHelper.CreateOptions<ScriptDTO>(
            model,
            "GET",
            "https://localhost:7200",
            "api/v0.1",
            "scripts",
            "",
            $"$filter=id eq {_recordId}");

        var result = await _controller.Search(queryOptions);
        var status = result.GetStatusCode();
        var content = result.GetContent();

        Assert.Multiple(() =>
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(status, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(content, Is.AssignableTo<IEnumerable<ScriptDTO>>());
        });

        var mockServ = _provider.GetRequiredService<Mock<IScriptService>>();
        mockServ.Verify(service => service.GetQueryable(), Times.AtLeastOnce());

        var mockSave = _provider.GetRequiredService<Mock<ISaveService>>();
        mockSave.Verify(service => service.SaveChangesAsync(), Times.Never());
    }

    [Test]
    public async Task Search_IsValid_WithSelect_Returns200Ok()
    {
        var model = ApiVersions.V0_1.GenerateEdmModel(); ;
        var queryOptions = ODataQueryOptionsHelper.CreateOptions<ScriptDTO>(
            model,
            "GET",
            "https://localhost:7200",
            "api/v0.1",
            "scripts",
            "",
            "$select=id");

        var result = await _controller.Search(queryOptions);
        var status = result.GetStatusCode();
        var content = result.GetContent();

        Assert.Multiple(() =>
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(status, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(content, Is.AssignableTo<IEnumerable<ISelectExpandWrapper>>());
        });

        var mockServ = _provider.GetRequiredService<Mock<IScriptService>>();
        mockServ.Verify(service => service.GetQueryable(), Times.AtLeastOnce());

        var mockSave = _provider.GetRequiredService<Mock<ISaveService>>();
        mockSave.Verify(service => service.SaveChangesAsync(), Times.Never());
    }

    [Test]
    public async Task Search_IsValid_WithOrderBy_Returns200Ok()
    {
        var model = ApiVersions.V0_1.GenerateEdmModel(); ;
        var queryOptions = ODataQueryOptionsHelper.CreateOptions<ScriptDTO>(
            model,
            "GET",
            "https://localhost:7200",
            "api/v0.1",
            "scripts",
            "",
            "$orderby=name");

        var result = await _controller.Search(queryOptions);
        var status = result.GetStatusCode();
        var content = result.GetContent();

        Assert.Multiple(() =>
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(status, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(content, Is.AssignableTo<IEnumerable<ScriptDTO>>());
        });

        var mockServ = _provider.GetRequiredService<Mock<IScriptService>>();
        mockServ.Verify(service => service.GetQueryable(), Times.AtLeastOnce());

        var mockSave = _provider.GetRequiredService<Mock<ISaveService>>();
        mockSave.Verify(service => service.SaveChangesAsync(), Times.Never());
    }

    [Test]
    public async Task Search_IsValid_WithTop_Returns200Ok()
    {
        var model = ApiVersions.V0_1.GenerateEdmModel(); ;
        var queryOptions = ODataQueryOptionsHelper.CreateOptions<ScriptDTO>(
            model,
            "GET",
            "https://localhost:7200",
            "api/v0.1",
            "scripts",
            "",
            "$top=1");

        var result = await _controller.Search(queryOptions);
        var status = result.GetStatusCode();
        var content = result.GetContent();

        Assert.Multiple(() =>
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(status, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(content, Is.AssignableTo<IEnumerable<ScriptDTO>>());
        });

        var mockServ = _provider.GetRequiredService<Mock<IScriptService>>();
        mockServ.Verify(service => service.GetQueryable(), Times.AtLeastOnce());

        var mockSave = _provider.GetRequiredService<Mock<ISaveService>>();
        mockSave.Verify(service => service.SaveChangesAsync(), Times.Never());
    }

    [Test]
    public async Task Search_IsValid_WithSkip_Returns200Ok()
    {
        var model = ApiVersions.V0_1.GenerateEdmModel(); ;
        var queryOptions = ODataQueryOptionsHelper.CreateOptions<ScriptDTO>(
            model,
            "GET",
            "https://localhost:7200",
            "api/v0.1",
            "scripts",
            "",
            "$skip=1");

        var result = await _controller.Search(queryOptions);
        var status = result.GetStatusCode();
        var content = result.GetContent();

        Assert.Multiple(() =>
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(status, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(content, Is.AssignableTo<IEnumerable<ScriptDTO>>());
        });

        var mockServ = _provider.GetRequiredService<Mock<IScriptService>>();
        mockServ.Verify(service => service.GetQueryable(), Times.AtLeastOnce());

        var mockSave = _provider.GetRequiredService<Mock<ISaveService>>();
        mockSave.Verify(service => service.SaveChangesAsync(), Times.Never());
    }

    [Test]
    public async Task Search_IsValid_WithCount_Returns200Ok()
    {
        var model = ApiVersions.V0_1.GenerateEdmModel(); ;
        var queryOptions = ODataQueryOptionsHelper.CreateOptions<ScriptDTO>(
            model,
            "GET",
            "https://localhost:7200",
            "api/v0.1",
            "scripts",
            "",
            "$count=true");

        var result = await _controller.Search(queryOptions);
        var status = result.GetStatusCode();
        var content = result.GetContent();

        Assert.Multiple(() =>
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(status, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(content, Is.AssignableTo<IEnumerable<ScriptDTO>>());
        });

        var mockServ = _provider.GetRequiredService<Mock<IScriptService>>();
        mockServ.Verify(service => service.GetQueryable(), Times.AtLeastOnce());

        var mockSave = _provider.GetRequiredService<Mock<ISaveService>>();
        mockSave.Verify(service => service.SaveChangesAsync(), Times.Never());
    }

    [Test]
    public async Task Get_IsInvalid_Returns404NotFound()
    {
        Guid recordId = new Guid();

        var model = ApiVersions.V0_1.GenerateEdmModel(); ;
        var queryOptions = ODataQueryOptionsHelper.CreateOptions<ScriptDTO>(
            model,
            "GET",
            "https://localhost:7200",
            "api/v0.1",
            "scripts",
            $"('{recordId}')",
            "");

        var result = await _controller.Get(recordId, queryOptions);
        var status = result.GetStatusCode();
        var content = result.GetContent();

        Assert.Multiple(() =>
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(status, Is.EqualTo(HttpStatusCode.NotFound));
            Assert.That(content, Is.AssignableTo<ODataError>());
        });

        var mockServ = _provider.GetRequiredService<Mock<IScriptService>>();
        mockServ.Verify(service => service.GetQueryable(), Times.AtLeastOnce());

        var mockSave = _provider.GetRequiredService<Mock<ISaveService>>();
        mockSave.Verify(service => service.SaveChangesAsync(), Times.Never());
    }

    [Test]
    public async Task Get_IsValid_Returns200Ok()
    {
        Guid recordId = _recordId;

        var model = ApiVersions.V0_1.GenerateEdmModel(); ;
        var queryOptions = ODataQueryOptionsHelper.CreateOptions<ScriptDTO>(
            model,
            "GET",
            "https://localhost:7200",
            "api/v0.1",
            "scripts",
            $"('{recordId}')",
            "");

        var result = await _controller.Get(recordId, queryOptions);
        var status = result.GetStatusCode();
        var content = result.GetContent();

        Assert.Multiple(() =>
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(status, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(content, Is.AssignableTo<ScriptDTO>());
        });

        var mockServ = _provider.GetRequiredService<Mock<IScriptService>>();
        mockServ.Verify(service => service.GetQueryable(), Times.AtLeastOnce());

        var mockSave = _provider.GetRequiredService<Mock<ISaveService>>();
        mockSave.Verify(service => service.SaveChangesAsync(), Times.Never());
    }

    [Test]
    public void Get_IsValid_WithFilter_ThrowsReturnODataErrorException()
    {
        Guid recordId = _recordId;

        var model = ApiVersions.V0_1.GenerateEdmModel(); ;
        var queryOptions = ODataQueryOptionsHelper.CreateOptions<ScriptDTO>(
            model,
            "GET",
            "https://localhost:7200",
            "api/v0.1",
            "scripts",
            $"('{recordId}')",
            "$filter=id eq 00000000-0000-0000-0000-000000000000");

        Assert.ThrowsAsync<ReturnODataErrorException>(async () =>
        {
            var result = await _controller.Get(recordId, queryOptions);
        });
    }

    [Test]
    public async Task Get_IsValid_WithSelect_Returns200Ok()
    {
        Guid recordId = _recordId;

        var model = ApiVersions.V0_1.GenerateEdmModel(); ;
        var queryOptions = ODataQueryOptionsHelper.CreateOptions<ScriptDTO>(
            model,
            "GET",
            "https://localhost:7200",
            "api/v0.1",
            "scripts",
            $"('{recordId}')",
            "$select=id");

        var result = await _controller.Get(recordId, queryOptions);
        var status = result.GetStatusCode();
        var content = result.GetContent();

        Assert.Multiple(() =>
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(status, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(content, Is.AssignableTo<ISelectExpandWrapper>());
        });

        var mockServ = _provider.GetRequiredService<Mock<IScriptService>>();
        mockServ.Verify(service => service.GetQueryable(), Times.AtLeastOnce());

        var mockSave = _provider.GetRequiredService<Mock<ISaveService>>();
        mockSave.Verify(service => service.SaveChangesAsync(), Times.Never());
    }

    [Test]
    public void Get_IsValid_WithOrderBy_ThrowsReturnODataErrorException()
    {
        Guid recordId = _recordId;

        var model = ApiVersions.V0_1.GenerateEdmModel(); ;
        var queryOptions = ODataQueryOptionsHelper.CreateOptions<ScriptDTO>(
            model,
            "GET",
            "https://localhost:7200",
            "api/v0.1",
            "scripts",
            $"('{recordId}')",
            "$orderby=id");

        Assert.ThrowsAsync<ReturnODataErrorException>(async () =>
        {
            var result = await _controller.Get(recordId, queryOptions);
        });
    }

    [Test]
    public void Get_IsValid_WithTop_ThrowsReturnODataErrorException()
    {
        Guid recordId = _recordId;

        var model = ApiVersions.V0_1.GenerateEdmModel(); ;
        var queryOptions = ODataQueryOptionsHelper.CreateOptions<ScriptDTO>(
            model,
            "GET",
            "https://localhost:7200",
            "api/v0.1",
            "scripts",
            $"('{recordId}')",
            "$top=1");

        Assert.ThrowsAsync<ReturnODataErrorException>(async () =>
        {
            var result = await _controller.Get(recordId, queryOptions);
        });
    }

    [Test]
    public void Get_IsValid_WithSkip_ThrowsReturnODataErrorException()
    {
        Guid recordId = _recordId;

        var model = ApiVersions.V0_1.GenerateEdmModel(); ;
        var queryOptions = ODataQueryOptionsHelper.CreateOptions<ScriptDTO>(
            model,
            "GET",
            "https://localhost:7200",
            "api/v0.1",
            "scripts",
            $"('{recordId}')",
            "$skip=1");

        Assert.ThrowsAsync<ReturnODataErrorException>(async () =>
        {
            var result = await _controller.Get(recordId, queryOptions);
        });
    }

    [Test]
    public void Get_IsValid_WithCount_ThrowsReturnODataErrorException()
    {
        Guid recordId = _recordId;

        var model = ApiVersions.V0_1.GenerateEdmModel(); ;
        var queryOptions = ODataQueryOptionsHelper.CreateOptions<ScriptDTO>(
            model,
            "GET",
            "https://localhost:7200",
            "api/v0.1",
            "scripts",
            $"('{recordId}')",
            "$count=true");

        Assert.ThrowsAsync<ReturnODataErrorException>(async () =>
        {
            var result = await _controller.Get(recordId, queryOptions);
        });
    }

    [Test]
    public void Create_WithNonexistentMetric_Returns404NotFound()
    {
        var model = ApiVersions.V0_1.GenerateEdmModel();
        var queryOptions = ODataQueryOptionsHelper.CreateOptions<ScriptDTO>(
            model,
            "POST",
            "https://localhost:7200",
            "api/v0.1",
            "scripts",
            "",
            "");

        var record = new ScriptDTO()
        {
            Name = "Test",
            Metrics = new()
            {
                new()
                {
                    MetricId = -1,
                },
            },
        };

        Assert.ThrowsAsync<ReturnODataErrorException>(async () =>
        {
            var result = await _controller.Create(record, queryOptions);
        });
    }

    [Test]
    public void Create_WithDuplicateMetrics_Returns422UnprocessableEntity()
    {
        var model = ApiVersions.V0_1.GenerateEdmModel();
        var queryOptions = ODataQueryOptionsHelper.CreateOptions<ScriptDTO>(
            model,
            "POST",
            "https://localhost:7200",
            "api/v0.1",
            "scripts",
            "",
            "");

        var record = new ScriptDTO()
        {
            Name = "Test",
            Metrics = new()
            {
                new()
                {
                    MetricId = 1,
                },
                new()
                {
                    MetricId = 1,
                },
            },
        };

        Assert.ThrowsAsync<ReturnODataErrorException>(async () =>
        {
            var result = await _controller.Create(record, queryOptions);
        });
    }

    [Test]
    public async Task Create_IsValid_Returns201Created()
    {
        var model = ApiVersions.V0_1.GenerateEdmModel();
        var queryOptions = ODataQueryOptionsHelper.CreateOptions<ScriptDTO>(
            model,
            "POST",
            "https://localhost:7200",
            "api/v0.1",
            "scripts",
            "",
            "");

        var record = new ScriptDTO()
        {
            Name = "Test",
        };

        var result = await _controller.Create(record, queryOptions);
        var status = result.GetStatusCode();
        var content = result.GetContent();

        Assert.Multiple(() =>
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(status, Is.EqualTo(HttpStatusCode.Created));
            Assert.That(content, Is.AssignableTo<ScriptDTO>());
        });

        var mockServ = _provider.GetRequiredService<Mock<IScriptService>>();
        mockServ.Verify(service => service.GetQueryable(), Times.AtLeastOnce());

        var mockSave = _provider.GetRequiredService<Mock<ISaveService>>();
        mockSave.Verify(service => service.SaveChangesAsync(), Times.Once());
    }

    [Test]
    public void Create_IsValid_WithFilter_ThrowsReturnODataErrorException()
    {
        var model = ApiVersions.V0_1.GenerateEdmModel(); ;
        var queryOptions = ODataQueryOptionsHelper.CreateOptions<ScriptDTO>(
            model,
            "GET",
            "https://localhost:7200",
            "api/v0.1",
            "scripts",
            "",
            $"$filter=id eq {_recordId}");

        var record = new ScriptDTO()
        {
            Name = "Test",
        };

        Assert.ThrowsAsync<ReturnODataErrorException>(async () =>
        {
            var result = await _controller.Create(record, queryOptions);
        });
    }

    [Test]
    public async Task Create_IsValid_WithSelect_Returns201Created()
    {
        var model = ApiVersions.V0_1.GenerateEdmModel(); ;
        var queryOptions = ODataQueryOptionsHelper.CreateOptions<ScriptDTO>(
            model,
            "GET",
            "https://localhost:7200",
            "api/v0.1",
            "scripts",
            "",
            "$select=id");

        var record = new ScriptDTO()
        {
            Name = "Test",
        };

        var result = await _controller.Create(record, queryOptions);
        var status = result.GetStatusCode();
        var content = result.GetContent();

        Assert.Multiple(() =>
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(status, Is.EqualTo(HttpStatusCode.Created));
            Assert.That(content, Is.AssignableTo<ISelectExpandWrapper>());
        });

        var mockServ = _provider.GetRequiredService<Mock<IScriptService>>();
        mockServ.Verify(service => service.GetQueryable(), Times.AtLeastOnce());

        var mockSave = _provider.GetRequiredService<Mock<ISaveService>>();
        mockSave.Verify(service => service.SaveChangesAsync(), Times.Once());
    }

    [Test]
    public void Create_IsValid_WithOrderBy_ThrowsReturnODataErrorException()
    {
        var model = ApiVersions.V0_1.GenerateEdmModel(); ;
        var queryOptions = ODataQueryOptionsHelper.CreateOptions<ScriptDTO>(
            model,
            "GET",
            "https://localhost:7200",
            "api/v0.1",
            "scripts",
            "",
            "$orderby=id");

        var record = new ScriptDTO()
        {
            Name = "Test",
        };

        Assert.ThrowsAsync<ReturnODataErrorException>(async () =>
        {
            var result = await _controller.Create(record, queryOptions);
        });
    }

    [Test]
    public void Create_IsValid_WithTop_ThrowsReturnODataErrorException()
    {
        var model = ApiVersions.V0_1.GenerateEdmModel(); ;
        var queryOptions = ODataQueryOptionsHelper.CreateOptions<ScriptDTO>(
            model,
            "GET",
            "https://localhost:7200",
            "api/v0.1",
            "scripts",
            "",
            "$top=1");

        var record = new ScriptDTO()
        {
            Name = "Test",
        };

        Assert.ThrowsAsync<ReturnODataErrorException>(async () =>
        {
            var result = await _controller.Create(record, queryOptions);
        });
    }

    [Test]
    public void Create_IsValid_WithSkip_ThrowsReturnODataErrorException()
    {
        var model = ApiVersions.V0_1.GenerateEdmModel(); ;
        var queryOptions = ODataQueryOptionsHelper.CreateOptions<ScriptDTO>(
            model,
            "GET",
            "https://localhost:7200",
            "api/v0.1",
            "scripts",
            "",
            "$skip=1");

        var record = new ScriptDTO()
        {
            Name = "Test",
        };

        Assert.ThrowsAsync<ReturnODataErrorException>(async () =>
        {
            var result = await _controller.Create(record, queryOptions);
        });
    }

    [Test]
    public void Create_IsValid_WithCount_ThrowsReturnODataErrorException()
    {
        var model = ApiVersions.V0_1.GenerateEdmModel(); ;
        var queryOptions = ODataQueryOptionsHelper.CreateOptions<ScriptDTO>(
            model,
            "GET",
            "https://localhost:7200",
            "api/v0.1",
            "scripts",
            "",
            "$count=true");

        var record = new ScriptDTO()
        {
            Name = "Test",
        };

        Assert.ThrowsAsync<ReturnODataErrorException>(async () =>
        {
            var result = await _controller.Create(record, queryOptions);
        });
    }

    [Test]
    public async Task Update_IsInvalid_Returns404NotFound()
    {
        var recordId = Guid.NewGuid();

        var model = ApiVersions.V0_1.GenerateEdmModel(); ;
        var queryOptions = ODataQueryOptionsHelper.CreateOptions<ScriptDTO>(
            model,
            "PATCH",
            "https://localhost:7200",
            "api/v0.1",
            "scripts",
            $"({recordId})",
            "");

        var record = new Delta<ScriptDTO>();
        record.TrySetPropertyValue(nameof(ScriptDTO.Name), "New Name");

        var result = await _controller.Update(recordId, record, queryOptions);
        var status = result.GetStatusCode();
        var content = result.GetContent();

        Assert.Multiple(() =>
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(status, Is.EqualTo(HttpStatusCode.NotFound));
            Assert.That(content, Is.AssignableTo<ODataError>());
        });

        var mockServ = _provider.GetRequiredService<Mock<IScriptService>>();
        mockServ.Verify(service => service.GetQueryable(), Times.AtLeastOnce());

        var mockSave = _provider.GetRequiredService<Mock<ISaveService>>();
        mockSave.Verify(service => service.SaveChangesAsync(), Times.Never());
    }

    [Test]
    public void Update_WithNonexistentMetric_Returns404NotFound()
    {
        var recordId = _recordId;

        var model = ApiVersions.V0_1.GenerateEdmModel();
        var queryOptions = ODataQueryOptionsHelper.CreateOptions<ScriptDTO>(
            model,
            "POST",
            "https://localhost:7200",
            "api/v0.1",
            "scripts",
            "",
            "");

        var record = new Delta<ScriptDTO>();
        record.TrySetPropertyValue(nameof(ScriptDTO.Metrics), new List<ScriptMetricDTO>()
        {
            new()
            {
                MetricId = -1,
            },
        });

        Assert.ThrowsAsync<ReturnODataErrorException>(async () =>
        {
            var result = await _controller.Update(recordId, record, queryOptions);
        });
    }

    [Test]
    public void Update_WithDuplicateMetrics_Returns422UnprocessableEntity()
    {
        var recordId = _recordId;

        var model = ApiVersions.V0_1.GenerateEdmModel();
        var queryOptions = ODataQueryOptionsHelper.CreateOptions<ScriptDTO>(
            model,
            "POST",
            "https://localhost:7200",
            "api/v0.1",
            "scripts",
            "",
            "");

        var record = new Delta<ScriptDTO>();
        record.TrySetPropertyValue(nameof(ScriptDTO.Metrics), new List<ScriptMetricDTO>()
        {
            new()
            {
                MetricId = 1,
            },
            new()
            {
                MetricId = 1,
            },
        });

        Assert.ThrowsAsync<ReturnODataErrorException>(async () =>
        {
            var result = await _controller.Update(recordId, record, queryOptions);
        });
    }

    [Test]
    public async Task Update_IsValid_Returns200Ok()
    {
        var recordId = _recordId;

        var model = ApiVersions.V0_1.GenerateEdmModel();
        var queryOptions = ODataQueryOptionsHelper.CreateOptions<ScriptDTO>(
            model,
            "POST",
            "https://localhost:7200",
            "api/v0.1",
            "scripts",
            $"({recordId})",
            "");

        var record = new Delta<ScriptDTO>();
        record.TrySetPropertyValue(nameof(ScriptDTO.Name), "New Name");

        var result = await _controller.Update(recordId, record, queryOptions);
        var status = result.GetStatusCode();
        var content = result.GetContent();

        Assert.Multiple(() =>
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(status, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(content, Is.AssignableTo<ScriptDTO>());
        });

        var mockServ = _provider.GetRequiredService<Mock<IScriptService>>();
        mockServ.Verify(service => service.GetQueryable(), Times.AtLeastOnce());

        var mockSave = _provider.GetRequiredService<Mock<ISaveService>>();
        mockSave.Verify(service => service.SaveChangesAsync(), Times.Once());
    }

    [Test]
    public void Update_IsValid_WithFilter_ThrowsReturnODataErrorException()
    {
        var recordId = _recordId;

        var model = ApiVersions.V0_1.GenerateEdmModel(); ;
        var queryOptions = ODataQueryOptionsHelper.CreateOptions<ScriptDTO>(
            model,
            "GET",
            "https://localhost:7200",
            "api/v0.1",
            "scripts",
            $"({recordId})",
            $"$filter=id eq {_recordId}");

        var record = new Delta<ScriptDTO>();
        record.TrySetPropertyValue(nameof(ScriptDTO.Name), "New Name");

        Assert.ThrowsAsync<ReturnODataErrorException>(async () =>
        {
            var result = await _controller.Update(recordId, record, queryOptions);
        });
    }

    [Test]
    public async Task Update_IsValid_WithSelect_Returns200Ok()
    {
        var recordId = _recordId;

        var model = ApiVersions.V0_1.GenerateEdmModel(); ;
        var queryOptions = ODataQueryOptionsHelper.CreateOptions<ScriptDTO>(
            model,
            "GET",
            "https://localhost:7200",
            "api/v0.1",
            "scripts",
            $"({recordId})",
            "$select=id");

        var record = new Delta<ScriptDTO>();
        record.TrySetPropertyValue(nameof(ScriptDTO.Name), "New Name");

        var result = await _controller.Update(recordId, record, queryOptions);
        var status = result.GetStatusCode();
        var content = result.GetContent();

        Assert.Multiple(() =>
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(status, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(content, Is.AssignableTo<ISelectExpandWrapper>());
        });

        var mockServ = _provider.GetRequiredService<Mock<IScriptService>>();
        mockServ.Verify(service => service.GetQueryable(), Times.AtLeastOnce());

        var mockSave = _provider.GetRequiredService<Mock<ISaveService>>();
        mockSave.Verify(service => service.SaveChangesAsync(), Times.Once());
    }

    [Test]
    public void Update_IsValid_WithOrderBy_ThrowsReturnODataErrorException()
    {
        var recordId = _recordId;

        var model = ApiVersions.V0_1.GenerateEdmModel(); ;
        var queryOptions = ODataQueryOptionsHelper.CreateOptions<ScriptDTO>(
            model,
            "GET",
            "https://localhost:7200",
            "api/v0.1",
            "scripts",
            $"({_recordId})",
            "$orderby=id");

        var record = new Delta<ScriptDTO>();
        record.TrySetPropertyValue(nameof(ScriptDTO.Name), "New Name");

        Assert.ThrowsAsync<ReturnODataErrorException>(async () =>
        {
            var result = await _controller.Update(recordId, record, queryOptions);
        });
    }

    [Test]
    public void Update_IsValid_WithTop_ThrowsReturnODataErrorException()
    {
        var recordId = _recordId;

        var model = ApiVersions.V0_1.GenerateEdmModel(); ;
        var queryOptions = ODataQueryOptionsHelper.CreateOptions<ScriptDTO>(
            model,
            "GET",
            "https://localhost:7200",
            "api/v0.1",
            "scripts",
            $"({recordId})",
            "$top=1");

        var record = new Delta<ScriptDTO>();
        record.TrySetPropertyValue(nameof(ScriptDTO.Name), "New Name");

        Assert.ThrowsAsync<ReturnODataErrorException>(async () =>
        {
            var result = await _controller.Update(recordId, record, queryOptions);
        });
    }

    [Test]
    public void Update_IsValid_WithSkip_ThrowsReturnODataErrorException()
    {
        var recordId = _recordId;

        var model = ApiVersions.V0_1.GenerateEdmModel(); ;
        var queryOptions = ODataQueryOptionsHelper.CreateOptions<ScriptDTO>(
            model,
            "GET",
            "https://localhost:7200",
            "api/v0.1",
            "scripts",
            $"({recordId})",
            "$skip=1");

        var record = new Delta<ScriptDTO>();
        record.TrySetPropertyValue(nameof(ScriptDTO.Name), "New Name");

        Assert.ThrowsAsync<ReturnODataErrorException>(async () =>
        {
            var result = await _controller.Update(recordId, record, queryOptions);
        });
    }

    [Test]
    public void Update_IsValid_WithCount_ThrowsReturnODataErrorException()
    {
        var recordId = _recordId;

        var model = ApiVersions.V0_1.GenerateEdmModel(); ;
        var queryOptions = ODataQueryOptionsHelper.CreateOptions<ScriptDTO>(
            model,
            "GET",
            "https://localhost:7200",
            "api/v0.1",
            "scripts",
            $"({recordId})",
            "$count=true");

        var record = new Delta<ScriptDTO>();
        record.TrySetPropertyValue(nameof(ScriptDTO.Name), "New Name");

        Assert.ThrowsAsync<ReturnODataErrorException>(async () =>
        {
            var result = await _controller.Update(recordId, record, queryOptions);
        });
    }

    [Test]
    public async Task Delete_IsInvalid_Returns404NotFound()
    {
        Guid recordId = new Guid();

        var model = ApiVersions.V0_1.GenerateEdmModel(); ;
        var queryOptions = ODataQueryOptionsHelper.CreateOptions<ScriptDTO>(
            model,
            "DELETE",
            "https://localhost:7200",
            "api/v0.1",
            "scripts",
            $"('{recordId}')",
            "");

        var result = await _controller.Delete(recordId);
        var status = result.GetStatusCode();
        var content = result.GetContent();

        Assert.Multiple(() =>
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(status, Is.EqualTo(HttpStatusCode.NotFound));
            Assert.That(content, Is.AssignableTo<ODataError>());
        });

        var mockServ = _provider.GetRequiredService<Mock<IScriptService>>();
        mockServ.Verify(service => service.GetQueryable(), Times.AtLeastOnce());

        var mockSave = _provider.GetRequiredService<Mock<ISaveService>>();
        mockSave.Verify(service => service.SaveChangesAsync(), Times.Never());
    }

    [Test]
    public async Task Delete_IsValid_Returns204NoContent()
    {
        Guid recordId = _recordId;

        var model = ApiVersions.V0_1.GenerateEdmModel(); ;
        var queryOptions = ODataQueryOptionsHelper.CreateOptions<ScriptDTO>(
            model,
            "DELETE",
            "https://localhost:7200",
            "api/v0.1",
            "scripts",
            $"('{recordId}')",
            "");

        var result = await _controller.Delete(recordId);
        var status = result.GetStatusCode();
        var content = result.GetContent();

        Assert.Multiple(() =>
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(status, Is.EqualTo(HttpStatusCode.NoContent));
            Assert.That(content, Is.Null);
        });

        var mockServ = _provider.GetRequiredService<Mock<IScriptService>>();
        mockServ.Verify(service => service.GetQueryable(), Times.AtLeastOnce());

        var mockSave = _provider.GetRequiredService<Mock<ISaveService>>();
        mockSave.Verify(service => service.SaveChangesAsync(), Times.Once());
    }
}
