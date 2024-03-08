using EtherGizmos.SqlMonitor.Api.Controllers;
using EtherGizmos.SqlMonitor.Api.Services.Data.Abstractions;
using EtherGizmos.SqlMonitor.Api.UnitTests.Extensions;
using EtherGizmos.SqlMonitor.Models;
using EtherGizmos.SqlMonitor.Models.Api.v1;
using EtherGizmos.SqlMonitor.Models.Api.v1.Enums;
using EtherGizmos.SqlMonitor.Models.Database;
using EtherGizmos.SqlMonitor.Models.Database.Enums;
using EtherGizmos.SqlMonitor.Models.Exceptions;
using Microsoft.AspNetCore.OData.Deltas;
using Microsoft.AspNetCore.OData.Query.Wrapper;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OData;
using MockQueryable.Moq;
using Moq;
using System.Net;

namespace EtherGizmos.SqlMonitor.Api.UnitTests.Controllers;

internal class MetricsControllerTests
{
    private IServiceProvider _serviceProvider;
    private MetricsController _controller;

    private List<Metric> _data;
    private readonly Guid _metricId = Guid.NewGuid();

    [SetUp]
    public void SetUp()
    {
        _serviceProvider = Global.CreateScope();
        _controller = _serviceProvider.GetRequiredService<MetricsController>();
        _data = new List<Metric>()
        {
            new Metric()
            {
                Id = _metricId,
                Name = "Test 1",
                Description = null,
                AggregateType = AggregateType.Average
            },
            new Metric()
            {
                Id = Guid.NewGuid(),
                Name = "Test 2",
                Description = null,
                AggregateType = AggregateType.Sum
            },
            new Metric()
            {
                Id = Guid.NewGuid(),
                Name = "Test 3",
                Description = null,
                AggregateType = AggregateType.Maximum
            },
            new Metric()
            {
                Id = Guid.NewGuid(),
                Name = "Test 4",
                Description = null,
                AggregateType = AggregateType.Minimum
            }
        };

        var mockData = _data.AsQueryable().BuildMock();

        var mockServ = _serviceProvider.GetRequiredService<Mock<IMetricService>>();
        mockServ.Setup(service => service.GetQueryable()).Returns(mockData);

        var mockSave = _serviceProvider.GetRequiredService<Mock<ISaveService>>();
        mockSave.Setup(service => service.SaveChangesAsync()).Returns(Task.CompletedTask);
    }

    [Test]
    public async Task Search_IsValid_Returns200Ok()
    {
        var model = ApiVersions.V0_1.GenerateEdmModel(); ;
        var queryOptions = ODataQueryOptionsHelper.CreateOptions<MetricDTO>(
            model,
            "GET",
            "https://localhost:7200",
            "api/v0.1",
            "metrics",
            "",
            "");

        var result = await _controller.Search(queryOptions);
        var status = result.GetStatusCode();
        var content = result.GetContent();

        Assert.Multiple(() =>
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(status, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(content, Is.AssignableTo<IEnumerable<MetricDTO>>());
        });

        var mockServ = _serviceProvider.GetRequiredService<Mock<IMetricService>>();
        mockServ.Verify(service => service.GetQueryable(), Times.AtLeastOnce());

        var mockSave = _serviceProvider.GetRequiredService<Mock<ISaveService>>();
        mockSave.Verify(service => service.SaveChangesAsync(), Times.Never());
    }

    [Test]
    public async Task Search_IsValid_WithFilter_Returns200Ok()
    {
        var model = ApiVersions.V0_1.GenerateEdmModel(); ;
        var queryOptions = ODataQueryOptionsHelper.CreateOptions<MetricDTO>(
            model,
            "GET",
            "https://localhost:7200",
            "api/v0.1",
            "metrics",
            "",
            $"$filter=id eq {_metricId}");

        var result = await _controller.Search(queryOptions);
        var status = result.GetStatusCode();
        var content = result.GetContent();

        Assert.Multiple(() =>
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(status, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(content, Is.AssignableTo<IEnumerable<MetricDTO>>());
        });

        var mockServ = _serviceProvider.GetRequiredService<Mock<IMetricService>>();
        mockServ.Verify(service => service.GetQueryable(), Times.AtLeastOnce());

        var mockSave = _serviceProvider.GetRequiredService<Mock<ISaveService>>();
        mockSave.Verify(service => service.SaveChangesAsync(), Times.Never());
    }

    //[Test]
    //public async Task Search_IsValid_WithExpand_Returns200Ok()
    //{
    //    var model = ApiVersions.V0_1.GenerateEdmModel();;
    //    var queryOptions = ODataQueryOptionsHelper.CreateOptions<MetricDTO>(
    //        model,
    //        "GET",
    //        "https://localhost:7200",
    //        "api/v0.1",
    //        "metrics",
    //        "",
    //        "$expand=...");

    //    var result = await Controller.Search(queryOptions);
    //    var status = result.GetStatusCode();
    //    var content = result.GetContent();

    //    Assert.Multiple(() =>
    //    {
    //        Assert.That(result, Is.Not.Null);
    //        Assert.That(status, Is.EqualTo(HttpStatusCode.OK));
    //        Assert.That(content, Is.AssignableTo<IEnumerable<ISelectExpandWrapper>>());
    //    });

    //    var mockServ = Provider.GetRequiredService<Mock<IMetricService>>();
    //    mockServ.Verify(service => service.GetQueryable(), Times.AtLeastOnce());

    //    var mockSave = Provider.GetRequiredService<Mock<ISaveService>>();
    //    mockSave.Verify(service => service.SaveChangesAsync(), Times.Never());
    //}

    [Test]
    public async Task Search_IsValid_WithSelect_Returns200Ok()
    {
        var model = ApiVersions.V0_1.GenerateEdmModel(); ;
        var queryOptions = ODataQueryOptionsHelper.CreateOptions<MetricDTO>(
            model,
            "GET",
            "https://localhost:7200",
            "api/v0.1",
            "metrics",
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

        var mockServ = _serviceProvider.GetRequiredService<Mock<IMetricService>>();
        mockServ.Verify(service => service.GetQueryable(), Times.AtLeastOnce());

        var mockSave = _serviceProvider.GetRequiredService<Mock<ISaveService>>();
        mockSave.Verify(service => service.SaveChangesAsync(), Times.Never());
    }

    [Test]
    public async Task Search_IsValid_WithOrderBy_Returns200Ok()
    {
        var model = ApiVersions.V0_1.GenerateEdmModel(); ;
        var queryOptions = ODataQueryOptionsHelper.CreateOptions<MetricDTO>(
            model,
            "GET",
            "https://localhost:7200",
            "api/v0.1",
            "metrics",
            "",
            "$orderby=name");

        var result = await _controller.Search(queryOptions);
        var status = result.GetStatusCode();
        var content = result.GetContent();

        Assert.Multiple(() =>
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(status, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(content, Is.AssignableTo<IEnumerable<MetricDTO>>());
        });

        var mockServ = _serviceProvider.GetRequiredService<Mock<IMetricService>>();
        mockServ.Verify(service => service.GetQueryable(), Times.AtLeastOnce());

        var mockSave = _serviceProvider.GetRequiredService<Mock<ISaveService>>();
        mockSave.Verify(service => service.SaveChangesAsync(), Times.Never());
    }

    [Test]
    public async Task Search_IsValid_WithTop_Returns200Ok()
    {
        var model = ApiVersions.V0_1.GenerateEdmModel(); ;
        var queryOptions = ODataQueryOptionsHelper.CreateOptions<MetricDTO>(
            model,
            "GET",
            "https://localhost:7200",
            "api/v0.1",
            "metrics",
            "",
            "$top=1");

        var result = await _controller.Search(queryOptions);
        var status = result.GetStatusCode();
        var content = result.GetContent();

        Assert.Multiple(() =>
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(status, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(content, Is.AssignableTo<IEnumerable<MetricDTO>>());
        });

        var mockServ = _serviceProvider.GetRequiredService<Mock<IMetricService>>();
        mockServ.Verify(service => service.GetQueryable(), Times.AtLeastOnce());

        var mockSave = _serviceProvider.GetRequiredService<Mock<ISaveService>>();
        mockSave.Verify(service => service.SaveChangesAsync(), Times.Never());
    }

    [Test]
    public async Task Search_IsValid_WithSkip_Returns200Ok()
    {
        var model = ApiVersions.V0_1.GenerateEdmModel(); ;
        var queryOptions = ODataQueryOptionsHelper.CreateOptions<MetricDTO>(
            model,
            "GET",
            "https://localhost:7200",
            "api/v0.1",
            "metrics",
            "",
            "$skip=1");

        var result = await _controller.Search(queryOptions);
        var status = result.GetStatusCode();
        var content = result.GetContent();

        Assert.Multiple(() =>
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(status, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(content, Is.AssignableTo<IEnumerable<MetricDTO>>());
        });

        var mockServ = _serviceProvider.GetRequiredService<Mock<IMetricService>>();
        mockServ.Verify(service => service.GetQueryable(), Times.AtLeastOnce());

        var mockSave = _serviceProvider.GetRequiredService<Mock<ISaveService>>();
        mockSave.Verify(service => service.SaveChangesAsync(), Times.Never());
    }

    [Test]
    public async Task Search_IsValid_WithCount_Returns200Ok()
    {
        var model = ApiVersions.V0_1.GenerateEdmModel(); ;
        var queryOptions = ODataQueryOptionsHelper.CreateOptions<MetricDTO>(
            model,
            "GET",
            "https://localhost:7200",
            "api/v0.1",
            "metrics",
            "",
            "$count=true");

        var result = await _controller.Search(queryOptions);
        var status = result.GetStatusCode();
        var content = result.GetContent();

        Assert.Multiple(() =>
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(status, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(content, Is.AssignableTo<IEnumerable<MetricDTO>>());
        });

        var mockServ = _serviceProvider.GetRequiredService<Mock<IMetricService>>();
        mockServ.Verify(service => service.GetQueryable(), Times.AtLeastOnce());

        var mockSave = _serviceProvider.GetRequiredService<Mock<ISaveService>>();
        mockSave.Verify(service => service.SaveChangesAsync(), Times.Never());
    }

    [Test]
    public async Task Get_IsInvalid_Returns404NotFound()
    {
        Guid recordId = new Guid();

        var model = ApiVersions.V0_1.GenerateEdmModel(); ;
        var queryOptions = ODataQueryOptionsHelper.CreateOptions<MetricDTO>(
            model,
            "GET",
            "https://localhost:7200",
            "api/v0.1",
            "metrics",
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

        var mockServ = _serviceProvider.GetRequiredService<Mock<IMetricService>>();
        mockServ.Verify(service => service.GetQueryable(), Times.AtLeastOnce());

        var mockSave = _serviceProvider.GetRequiredService<Mock<ISaveService>>();
        mockSave.Verify(service => service.SaveChangesAsync(), Times.Never());
    }

    [Test]
    public async Task Get_IsValid_Returns200Ok()
    {
        Guid recordId = _metricId;

        var model = ApiVersions.V0_1.GenerateEdmModel(); ;
        var queryOptions = ODataQueryOptionsHelper.CreateOptions<MetricDTO>(
            model,
            "GET",
            "https://localhost:7200",
            "api/v0.1",
            "metrics",
            $"('{recordId}')",
            "");

        var result = await _controller.Get(recordId, queryOptions);
        var status = result.GetStatusCode();
        var content = result.GetContent();

        Assert.Multiple(() =>
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(status, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(content, Is.AssignableTo<MetricDTO>());
        });

        var mockServ = _serviceProvider.GetRequiredService<Mock<IMetricService>>();
        mockServ.Verify(service => service.GetQueryable(), Times.AtLeastOnce());

        var mockSave = _serviceProvider.GetRequiredService<Mock<ISaveService>>();
        mockSave.Verify(service => service.SaveChangesAsync(), Times.Never());
    }

    [Test]
    public void Get_IsValid_WithFilter_ThrowsReturnODataErrorException()
    {
        Guid recordId = _metricId;

        var model = ApiVersions.V0_1.GenerateEdmModel(); ;
        var queryOptions = ODataQueryOptionsHelper.CreateOptions<MetricDTO>(
            model,
            "GET",
            "https://localhost:7200",
            "api/v0.1",
            "metrics",
            $"('{recordId}')",
            "$filter=id eq 'CREATE'");

        Assert.ThrowsAsync<ReturnODataErrorException>(async () =>
        {
            var result = await _controller.Get(recordId, queryOptions);
        });
    }

    //[Test]
    //public async Task Get_IsValid_WithExpand_Returns200Ok()
    //{
    //    Guid recordId = RecordId;

    //    var model = ApiVersions.V0_1.GenerateEdmModel();;
    //    var queryOptions = ODataQueryOptionsHelper.CreateOptions<MetricDTO>(
    //        model,
    //        "GET",
    //        "https://localhost:7200",
    //        "api/v0.1",
    //        "metrics",
    //        $"('{recordId}')",
    //        "$expand=...");

    //    var result = await Controller.Get(recordId, queryOptions);
    //    var status = result.GetStatusCode();
    //    var content = result.GetContent();

    //    Assert.Multiple(() =>
    //    {
    //        Assert.That(result, Is.Not.Null);
    //        Assert.That(status, Is.EqualTo(HttpStatusCode.OK));
    //        Assert.That(content, Is.AssignableTo<ISelectExpandWrapper>());
    //    });

    //    var mockServ = Provider.GetRequiredService<Mock<IMetricService>>();
    //    mockServ.Verify(service => service.GetQueryable(), Times.AtLeastOnce());

    //    var mockSave = Provider.GetRequiredService<Mock<ISaveService>>();
    //    mockSave.Verify(service => service.SaveChangesAsync(), Times.Never());
    //}

    [Test]
    public async Task Get_IsValid_WithSelect_Returns200Ok()
    {
        Guid recordId = _metricId;

        var model = ApiVersions.V0_1.GenerateEdmModel(); ;
        var queryOptions = ODataQueryOptionsHelper.CreateOptions<MetricDTO>(
            model,
            "GET",
            "https://localhost:7200",
            "api/v0.1",
            "metrics",
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

        var mockServ = _serviceProvider.GetRequiredService<Mock<IMetricService>>();
        mockServ.Verify(service => service.GetQueryable(), Times.AtLeastOnce());

        var mockSave = _serviceProvider.GetRequiredService<Mock<ISaveService>>();
        mockSave.Verify(service => service.SaveChangesAsync(), Times.Never());
    }

    [Test]
    public void Get_IsValid_WithOrderBy_ThrowsReturnODataErrorException()
    {
        Guid recordId = _metricId;

        var model = ApiVersions.V0_1.GenerateEdmModel(); ;
        var queryOptions = ODataQueryOptionsHelper.CreateOptions<MetricDTO>(
            model,
            "GET",
            "https://localhost:7200",
            "api/v0.1",
            "metrics",
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
        Guid recordId = _metricId;

        var model = ApiVersions.V0_1.GenerateEdmModel(); ;
        var queryOptions = ODataQueryOptionsHelper.CreateOptions<MetricDTO>(
            model,
            "GET",
            "https://localhost:7200",
            "api/v0.1",
            "metrics",
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
        Guid recordId = _metricId;

        var model = ApiVersions.V0_1.GenerateEdmModel(); ;
        var queryOptions = ODataQueryOptionsHelper.CreateOptions<MetricDTO>(
            model,
            "GET",
            "https://localhost:7200",
            "api/v0.1",
            "metrics",
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
        Guid recordId = _metricId;

        var model = ApiVersions.V0_1.GenerateEdmModel(); ;
        var queryOptions = ODataQueryOptionsHelper.CreateOptions<MetricDTO>(
            model,
            "GET",
            "https://localhost:7200",
            "api/v0.1",
            "metrics",
            $"('{recordId}')",
            "$count=true");

        Assert.ThrowsAsync<ReturnODataErrorException>(async () =>
        {
            var result = await _controller.Get(recordId, queryOptions);
        });
    }

    [Test]
    public async Task Create_IsValid_Returns201Created()
    {
        var model = ApiVersions.V0_1.GenerateEdmModel(); ;
        var queryOptions = ODataQueryOptionsHelper.CreateOptions<MetricDTO>(
            model,
            "POST",
            "https://localhost:7200",
            "api/v0.1",
            "metrics",
            "",
            "");

        var record = new MetricDTO()
        {
            Name = "Test",
            AggregateType = AggregateTypeDTO.Average
        };

        var result = await _controller.Create(record, queryOptions);
        var status = result.GetStatusCode();
        var content = result.GetContent();

        Assert.Multiple(() =>
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(status, Is.EqualTo(HttpStatusCode.Created));
            Assert.That(content, Is.AssignableTo<MetricDTO>());
        });

        var mockServ = _serviceProvider.GetRequiredService<Mock<IMetricService>>();
        mockServ.Verify(service => service.GetQueryable(), Times.AtLeastOnce());

        var mockSave = _serviceProvider.GetRequiredService<Mock<ISaveService>>();
        mockSave.Verify(service => service.SaveChangesAsync(), Times.Once());
    }

    [Test]
    public void Create_IsValid_WithFilter_ThrowsReturnODataErrorException()
    {
        var model = ApiVersions.V0_1.GenerateEdmModel(); ;
        var queryOptions = ODataQueryOptionsHelper.CreateOptions<MetricDTO>(
            model,
            "GET",
            "https://localhost:7200",
            "api/v0.1",
            "metrics",
            "",
            $"$filter=id eq {_metricId}");

        var record = new MetricDTO()
        {
            Name = "Test",
            AggregateType = AggregateTypeDTO.Average
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
        var queryOptions = ODataQueryOptionsHelper.CreateOptions<MetricDTO>(
            model,
            "GET",
            "https://localhost:7200",
            "api/v0.1",
            "metrics",
            "",
            "$select=id");

        var record = new MetricDTO()
        {
            Name = "Test",
            AggregateType = AggregateTypeDTO.Average
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

        var mockServ = _serviceProvider.GetRequiredService<Mock<IMetricService>>();
        mockServ.Verify(service => service.GetQueryable(), Times.AtLeastOnce());

        var mockSave = _serviceProvider.GetRequiredService<Mock<ISaveService>>();
        mockSave.Verify(service => service.SaveChangesAsync(), Times.Once());
    }

    [Test]
    public void Create_IsValid_WithOrderBy_ThrowsReturnODataErrorException()
    {
        var model = ApiVersions.V0_1.GenerateEdmModel(); ;
        var queryOptions = ODataQueryOptionsHelper.CreateOptions<MetricDTO>(
            model,
            "GET",
            "https://localhost:7200",
            "api/v0.1",
            "metrics",
            "",
            "$orderby=id");

        var record = new MetricDTO()
        {
            Name = "Test",
            AggregateType = AggregateTypeDTO.Average
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
        var queryOptions = ODataQueryOptionsHelper.CreateOptions<MetricDTO>(
            model,
            "GET",
            "https://localhost:7200",
            "api/v0.1",
            "metrics",
            "",
            "$top=1");

        var record = new MetricDTO()
        {
            Name = "Test",
            AggregateType = AggregateTypeDTO.Average
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
        var queryOptions = ODataQueryOptionsHelper.CreateOptions<MetricDTO>(
            model,
            "GET",
            "https://localhost:7200",
            "api/v0.1",
            "metrics",
            "",
            "$skip=1");

        var record = new MetricDTO()
        {
            Name = "Test",
            AggregateType = AggregateTypeDTO.Average
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
        var queryOptions = ODataQueryOptionsHelper.CreateOptions<MetricDTO>(
            model,
            "GET",
            "https://localhost:7200",
            "api/v0.1",
            "metrics",
            "",
            "$count=true");

        var record = new MetricDTO()
        {
            Name = "Test",
            AggregateType = AggregateTypeDTO.Average
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
        var queryOptions = ODataQueryOptionsHelper.CreateOptions<MetricDTO>(
            model,
            "PATCH",
            "https://localhost:7200",
            "api/v0.1",
            "metrics",
            $"({recordId})",
            "");

        var record = new Delta<MetricDTO>();
        record.TrySetPropertyValue(nameof(MetricDTO.Name), "New Name");

        var result = await _controller.Update(recordId, record, queryOptions);
        var status = result.GetStatusCode();
        var content = result.GetContent();

        Assert.Multiple(() =>
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(status, Is.EqualTo(HttpStatusCode.NotFound));
            Assert.That(content, Is.AssignableTo<ODataError>());
        });

        var mockServ = _serviceProvider.GetRequiredService<Mock<IMetricService>>();
        mockServ.Verify(service => service.GetQueryable(), Times.AtLeastOnce());

        var mockSave = _serviceProvider.GetRequiredService<Mock<ISaveService>>();
        mockSave.Verify(service => service.SaveChangesAsync(), Times.Never());
    }

    [Test]
    public async Task Update_IsValid_Returns200Ok()
    {
        var recordId = _metricId;

        var model = ApiVersions.V0_1.GenerateEdmModel(); ;
        var queryOptions = ODataQueryOptionsHelper.CreateOptions<MetricDTO>(
            model,
            "POST",
            "https://localhost:7200",
            "api/v0.1",
            "metrics",
            $"({recordId})",
            "");

        var record = new Delta<MetricDTO>();
        record.TrySetPropertyValue(nameof(MetricDTO.Name), "New Name");

        var result = await _controller.Update(recordId, record, queryOptions);
        var status = result.GetStatusCode();
        var content = result.GetContent();

        Assert.Multiple(() =>
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(status, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(content, Is.AssignableTo<MetricDTO>());
        });

        var mockServ = _serviceProvider.GetRequiredService<Mock<IMetricService>>();
        mockServ.Verify(service => service.GetQueryable(), Times.AtLeastOnce());

        var mockSave = _serviceProvider.GetRequiredService<Mock<ISaveService>>();
        mockSave.Verify(service => service.SaveChangesAsync(), Times.Once());
    }

    [Test]
    public void Update_IsValid_WithFilter_ThrowsReturnODataErrorException()
    {
        var recordId = _metricId;

        var model = ApiVersions.V0_1.GenerateEdmModel(); ;
        var queryOptions = ODataQueryOptionsHelper.CreateOptions<MetricDTO>(
            model,
            "GET",
            "https://localhost:7200",
            "api/v0.1",
            "metrics",
            $"({recordId})",
            $"$filter=id eq {_metricId}");

        var record = new Delta<MetricDTO>();
        record.TrySetPropertyValue(nameof(MetricDTO.Name), "New Name");

        Assert.ThrowsAsync<ReturnODataErrorException>(async () =>
        {
            var result = await _controller.Update(recordId, record, queryOptions);
        });
    }

    [Test]
    public async Task Update_IsValid_WithSelect_Returns200Ok()
    {
        var recordId = _metricId;

        var model = ApiVersions.V0_1.GenerateEdmModel(); ;
        var queryOptions = ODataQueryOptionsHelper.CreateOptions<MetricDTO>(
            model,
            "GET",
            "https://localhost:7200",
            "api/v0.1",
            "metrics",
            $"({recordId})",
            "$select=id");

        var record = new Delta<MetricDTO>();
        record.TrySetPropertyValue(nameof(MetricDTO.Name), "New Name");

        var result = await _controller.Update(recordId, record, queryOptions);
        var status = result.GetStatusCode();
        var content = result.GetContent();

        Assert.Multiple(() =>
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(status, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(content, Is.AssignableTo<ISelectExpandWrapper>());
        });

        var mockServ = _serviceProvider.GetRequiredService<Mock<IMetricService>>();
        mockServ.Verify(service => service.GetQueryable(), Times.AtLeastOnce());

        var mockSave = _serviceProvider.GetRequiredService<Mock<ISaveService>>();
        mockSave.Verify(service => service.SaveChangesAsync(), Times.Once());
    }

    [Test]
    public void Update_IsValid_WithOrderBy_ThrowsReturnODataErrorException()
    {
        var recordId = _metricId;

        var model = ApiVersions.V0_1.GenerateEdmModel(); ;
        var queryOptions = ODataQueryOptionsHelper.CreateOptions<MetricDTO>(
            model,
            "GET",
            "https://localhost:7200",
            "api/v0.1",
            "metrics",
            $"({_metricId})",
            "$orderby=id");

        var record = new Delta<MetricDTO>();
        record.TrySetPropertyValue(nameof(MetricDTO.Name), "New Name");

        Assert.ThrowsAsync<ReturnODataErrorException>(async () =>
        {
            var result = await _controller.Update(recordId, record, queryOptions);
        });
    }

    [Test]
    public void Update_IsValid_WithTop_ThrowsReturnODataErrorException()
    {
        var recordId = _metricId;

        var model = ApiVersions.V0_1.GenerateEdmModel(); ;
        var queryOptions = ODataQueryOptionsHelper.CreateOptions<MetricDTO>(
            model,
            "GET",
            "https://localhost:7200",
            "api/v0.1",
            "metrics",
            $"({recordId})",
            "$top=1");

        var record = new Delta<MetricDTO>();
        record.TrySetPropertyValue(nameof(MetricDTO.Name), "New Name");

        Assert.ThrowsAsync<ReturnODataErrorException>(async () =>
        {
            var result = await _controller.Update(recordId, record, queryOptions);
        });
    }

    [Test]
    public void Update_IsValid_WithSkip_ThrowsReturnODataErrorException()
    {
        var recordId = _metricId;

        var model = ApiVersions.V0_1.GenerateEdmModel(); ;
        var queryOptions = ODataQueryOptionsHelper.CreateOptions<MetricDTO>(
            model,
            "GET",
            "https://localhost:7200",
            "api/v0.1",
            "metrics",
            $"({recordId})",
            "$skip=1");

        var record = new Delta<MetricDTO>();
        record.TrySetPropertyValue(nameof(MetricDTO.Name), "New Name");

        Assert.ThrowsAsync<ReturnODataErrorException>(async () =>
        {
            var result = await _controller.Update(recordId, record, queryOptions);
        });
    }

    [Test]
    public void Update_IsValid_WithCount_ThrowsReturnODataErrorException()
    {
        var recordId = _metricId;

        var model = ApiVersions.V0_1.GenerateEdmModel(); ;
        var queryOptions = ODataQueryOptionsHelper.CreateOptions<MetricDTO>(
            model,
            "GET",
            "https://localhost:7200",
            "api/v0.1",
            "metrics",
            $"({recordId})",
            "$count=true");

        var record = new Delta<MetricDTO>();
        record.TrySetPropertyValue(nameof(MetricDTO.Name), "New Name");

        Assert.ThrowsAsync<ReturnODataErrorException>(async () =>
        {
            var result = await _controller.Update(recordId, record, queryOptions);
        });
    }
}
