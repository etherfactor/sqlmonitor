﻿using EtherGizmos.SqlMonitor.Api.Controllers.Api;
using EtherGizmos.SqlMonitor.Shared.Database.Services.Abstractions;
using EtherGizmos.SqlMonitor.Shared.Models;
using EtherGizmos.SqlMonitor.Shared.Models.Api.v1;
using EtherGizmos.SqlMonitor.Shared.Models.Database;
using EtherGizmos.SqlMonitor.Shared.OData.Exceptions;
using EtherGizmos.SqlMonitor.UnitTests.Extensions;
using Microsoft.AspNetCore.OData.Deltas;
using Microsoft.AspNetCore.OData.Query.Wrapper;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OData;
using MockQueryable.Moq;
using Moq;
using System.Net;

namespace EtherGizmos.SqlMonitor.UnitTests.Controllers;

internal class MonitoredScriptTargetsControllerTests
{
    private IServiceProvider _provider;

    private MonitoredScriptTargetsController _controller;

    private List<MonitoredScriptTarget> _data;
    private List<MonitoredSystem> _systemData;
    private List<MonitoredResource> _resourceData;
    private List<MonitoredEnvironment> _environmentData;
    private List<MonitoredTarget> _targetData;

    private readonly int _recordId = new Random().Next(100, 10000);

    [SetUp]
    public void SetUp()
    {
        _provider = Global.CreateScope();
        _controller = _provider.GetRequiredService<MonitoredScriptTargetsController>();

        _targetData = new();

        var mockTargetData = _targetData.AsQueryable().BuildMock();

        var mockTargetServ = _provider.GetRequiredService<Mock<IMonitoredTargetService>>();
        mockTargetServ.Setup(service => service.GetQueryable()).Returns(mockTargetData);
        mockTargetServ.Setup(service => service.GetOrCreateAsync(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<CancellationToken>()).Result)
            .Returns(new MonitoredTarget() { Id = 1 });

        _systemData =
        [
            new()
            {
                Id = Guid.NewGuid(),
            },
        ];

        var mockSystemData = _systemData.AsQueryable().BuildMock();

        var mockSystemServ = _provider.GetRequiredService<Mock<IMonitoredSystemService>>();
        mockSystemServ.Setup(service => service.GetQueryable()).Returns(mockSystemData);

        _resourceData =
        [
            new()
            {
                Id = Guid.NewGuid(),
            },
        ];

        var mockResourceData = _resourceData.AsQueryable().BuildMock();

        var mockResourceServ = _provider.GetRequiredService<Mock<IMonitoredResourceService>>();
        mockResourceServ.Setup(service => service.GetQueryable()).Returns(mockResourceData);

        _environmentData =
        [
            new()
            {
                Id = Guid.NewGuid(),
            },
        ];

        var mockEnvironmentData = _environmentData.AsQueryable().BuildMock();

        var mockEnvironmentServ = _provider.GetRequiredService<Mock<IMonitoredEnvironmentService>>();
        mockEnvironmentServ.Setup(service => service.GetQueryable()).Returns(mockEnvironmentData);

        _data =
        [
            new()
            {
                Id = _recordId,
                MonitoredTarget = new()
                {
                    MonitoredSystemId = _systemData.First().Id,
                    MonitoredResourceId = _resourceData.First().Id,
                    MonitoredEnvironmentId = _environmentData.First().Id,
                },
            },
            new()
            {
                Id = new Random().Next(100, 10000),
                MonitoredTarget = new()
                {
                    MonitoredSystemId = _systemData.First().Id,
                    MonitoredResourceId = _resourceData.First().Id,
                    MonitoredEnvironmentId = _environmentData.First().Id,
                },
            },
            new()
            {
                Id = new Random().Next(100, 10000),
                MonitoredTarget = new()
                {
                    MonitoredSystemId = _systemData.First().Id,
                    MonitoredResourceId = _resourceData.First().Id,
                    MonitoredEnvironmentId = _environmentData.First().Id,
                },
            },
            new()
            {
                Id = new Random().Next(100, 10000),
                MonitoredTarget = new()
                {
                    MonitoredSystemId = _systemData.First().Id,
                    MonitoredResourceId = _resourceData.First().Id,
                    MonitoredEnvironmentId = _environmentData.First().Id,
                },
            }
        ];

        var mockData = _data.AsQueryable().BuildMock();

        var mockServ = _provider.GetRequiredService<Mock<IMonitoredScriptTargetService>>();
        mockServ.Setup(service => service.GetQueryable()).Returns(mockData);
        mockServ.Setup(service => service.Add(It.IsAny<MonitoredScriptTarget>()))
            .Callback((MonitoredScriptTarget record) => record.MonitoredTarget = new() { Id = 1 });

        var mockSave = _provider.GetRequiredService<Mock<ISaveService>>();
        mockSave.Setup(service => service.SaveChangesAsync()).Returns(Task.CompletedTask);
    }

    [Test]
    public async Task Search_IsValid_Returns200Ok()
    {
        var model = ApiVersions.V0_1.GenerateEdmModel();
        var queryOptions = ODataQueryOptionsHelper.CreateOptions<MonitoredScriptTargetDTO>(
            model,
            "GET",
            "https://localhost:7200",
            "api/v0.1",
            "monitoredResources",
            "",
            "");

        var result = await _controller.Search(queryOptions);
        var status = result.GetStatusCode();
        var content = result.GetContent();

        Assert.Multiple(() =>
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(status, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(content, Is.AssignableTo<IEnumerable<MonitoredScriptTargetDTO>>());
        });

        var mockServ = _provider.GetRequiredService<Mock<IMonitoredScriptTargetService>>();
        mockServ.Verify(service => service.GetQueryable(), Times.AtLeastOnce());

        var mockSave = _provider.GetRequiredService<Mock<ISaveService>>();
        mockSave.Verify(service => service.SaveChangesAsync(), Times.Never());
    }

    [Test]
    public async Task Search_IsValid_WithFilter_Returns200Ok()
    {
        var model = ApiVersions.V0_1.GenerateEdmModel();
        var queryOptions = ODataQueryOptionsHelper.CreateOptions<MonitoredScriptTargetDTO>(
            model,
            "GET",
            "https://localhost:7200",
            "api/v0.1",
            "monitoredResources",
            "",
            $"$filter=id eq {_recordId}");

        var result = await _controller.Search(queryOptions);
        var status = result.GetStatusCode();
        var content = result.GetContent();

        Assert.Multiple(() =>
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(status, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(content, Is.AssignableTo<IEnumerable<MonitoredScriptTargetDTO>>());
        });

        var mockServ = _provider.GetRequiredService<Mock<IMonitoredScriptTargetService>>();
        mockServ.Verify(service => service.GetQueryable(), Times.AtLeastOnce());

        var mockSave = _provider.GetRequiredService<Mock<ISaveService>>();
        mockSave.Verify(service => service.SaveChangesAsync(), Times.Never());
    }

    [Test]
    public async Task Search_IsValid_WithSelect_Returns200Ok()
    {
        var model = ApiVersions.V0_1.GenerateEdmModel();
        var queryOptions = ODataQueryOptionsHelper.CreateOptions<MonitoredScriptTargetDTO>(
            model,
            "GET",
            "https://localhost:7200",
            "api/v0.1",
            "monitoredResources",
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

        var mockServ = _provider.GetRequiredService<Mock<IMonitoredScriptTargetService>>();
        mockServ.Verify(service => service.GetQueryable(), Times.AtLeastOnce());

        var mockSave = _provider.GetRequiredService<Mock<ISaveService>>();
        mockSave.Verify(service => service.SaveChangesAsync(), Times.Never());
    }

    [Test]
    public async Task Search_IsValid_WithOrderBy_Returns200Ok()
    {
        var model = ApiVersions.V0_1.GenerateEdmModel();
        var queryOptions = ODataQueryOptionsHelper.CreateOptions<MonitoredScriptTargetDTO>(
            model,
            "GET",
            "https://localhost:7200",
            "api/v0.1",
            "monitoredResources",
            "",
            "$orderby=scriptInterpreterId");

        var result = await _controller.Search(queryOptions);
        var status = result.GetStatusCode();
        var content = result.GetContent();

        Assert.Multiple(() =>
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(status, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(content, Is.AssignableTo<IEnumerable<MonitoredScriptTargetDTO>>());
        });

        var mockServ = _provider.GetRequiredService<Mock<IMonitoredScriptTargetService>>();
        mockServ.Verify(service => service.GetQueryable(), Times.AtLeastOnce());

        var mockSave = _provider.GetRequiredService<Mock<ISaveService>>();
        mockSave.Verify(service => service.SaveChangesAsync(), Times.Never());
    }

    [Test]
    public async Task Search_IsValid_WithTop_Returns200Ok()
    {
        var model = ApiVersions.V0_1.GenerateEdmModel();
        var queryOptions = ODataQueryOptionsHelper.CreateOptions<MonitoredScriptTargetDTO>(
            model,
            "GET",
            "https://localhost:7200",
            "api/v0.1",
            "monitoredResources",
            "",
            "$top=1");

        var result = await _controller.Search(queryOptions);
        var status = result.GetStatusCode();
        var content = result.GetContent();

        Assert.Multiple(() =>
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(status, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(content, Is.AssignableTo<IEnumerable<MonitoredScriptTargetDTO>>());
        });

        var mockServ = _provider.GetRequiredService<Mock<IMonitoredScriptTargetService>>();
        mockServ.Verify(service => service.GetQueryable(), Times.AtLeastOnce());

        var mockSave = _provider.GetRequiredService<Mock<ISaveService>>();
        mockSave.Verify(service => service.SaveChangesAsync(), Times.Never());
    }

    [Test]
    public async Task Search_IsValid_WithSkip_Returns200Ok()
    {
        var model = ApiVersions.V0_1.GenerateEdmModel();
        var queryOptions = ODataQueryOptionsHelper.CreateOptions<MonitoredScriptTargetDTO>(
            model,
            "GET",
            "https://localhost:7200",
            "api/v0.1",
            "monitoredResources",
            "",
            "$skip=1");

        var result = await _controller.Search(queryOptions);
        var status = result.GetStatusCode();
        var content = result.GetContent();

        Assert.Multiple(() =>
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(status, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(content, Is.AssignableTo<IEnumerable<MonitoredScriptTargetDTO>>());
        });

        var mockServ = _provider.GetRequiredService<Mock<IMonitoredScriptTargetService>>();
        mockServ.Verify(service => service.GetQueryable(), Times.AtLeastOnce());

        var mockSave = _provider.GetRequiredService<Mock<ISaveService>>();
        mockSave.Verify(service => service.SaveChangesAsync(), Times.Never());
    }

    [Test]
    public async Task Search_IsValid_WithCount_Returns200Ok()
    {
        var model = ApiVersions.V0_1.GenerateEdmModel();
        var queryOptions = ODataQueryOptionsHelper.CreateOptions<MonitoredScriptTargetDTO>(
            model,
            "GET",
            "https://localhost:7200",
            "api/v0.1",
            "monitoredResources",
            "",
            "$count=true");

        var result = await _controller.Search(queryOptions);
        var status = result.GetStatusCode();
        var content = result.GetContent();

        Assert.Multiple(() =>
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(status, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(content, Is.AssignableTo<IEnumerable<MonitoredScriptTargetDTO>>());
        });

        var mockServ = _provider.GetRequiredService<Mock<IMonitoredScriptTargetService>>();
        mockServ.Verify(service => service.GetQueryable(), Times.AtLeastOnce());

        var mockSave = _provider.GetRequiredService<Mock<ISaveService>>();
        mockSave.Verify(service => service.SaveChangesAsync(), Times.Never());
    }

    [Test]
    public async Task Get_IsInvalid_Returns404NotFound()
    {
        int recordId = -1;

        var model = ApiVersions.V0_1.GenerateEdmModel();
        var queryOptions = ODataQueryOptionsHelper.CreateOptions<MonitoredScriptTargetDTO>(
            model,
            "GET",
            "https://localhost:7200",
            "api/v0.1",
            "monitoredResources",
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

        var mockServ = _provider.GetRequiredService<Mock<IMonitoredScriptTargetService>>();
        mockServ.Verify(service => service.GetQueryable(), Times.AtLeastOnce());

        var mockSave = _provider.GetRequiredService<Mock<ISaveService>>();
        mockSave.Verify(service => service.SaveChangesAsync(), Times.Never());
    }

    [Test]
    public async Task Get_IsValid_Returns200Ok()
    {
        int recordId = _recordId;

        var model = ApiVersions.V0_1.GenerateEdmModel();
        var queryOptions = ODataQueryOptionsHelper.CreateOptions<MonitoredScriptTargetDTO>(
            model,
            "GET",
            "https://localhost:7200",
            "api/v0.1",
            "monitoredResources",
            $"('{recordId}')",
            "");

        var result = await _controller.Get(recordId, queryOptions);
        var status = result.GetStatusCode();
        var content = result.GetContent();

        Assert.Multiple(() =>
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(status, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(content, Is.AssignableTo<MonitoredScriptTargetDTO>());
        });

        var mockServ = _provider.GetRequiredService<Mock<IMonitoredScriptTargetService>>();
        mockServ.Verify(service => service.GetQueryable(), Times.AtLeastOnce());

        var mockSave = _provider.GetRequiredService<Mock<ISaveService>>();
        mockSave.Verify(service => service.SaveChangesAsync(), Times.Never());
    }

    [Test]
    public void Get_IsValid_WithFilter_ThrowsReturnODataErrorException()
    {
        int recordId = _recordId;

        var model = ApiVersions.V0_1.GenerateEdmModel();
        var queryOptions = ODataQueryOptionsHelper.CreateOptions<MonitoredScriptTargetDTO>(
            model,
            "GET",
            "https://localhost:7200",
            "api/v0.1",
            "monitoredResources",
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
        int recordId = _recordId;

        var model = ApiVersions.V0_1.GenerateEdmModel();
        var queryOptions = ODataQueryOptionsHelper.CreateOptions<MonitoredScriptTargetDTO>(
            model,
            "GET",
            "https://localhost:7200",
            "api/v0.1",
            "monitoredResources",
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

        var mockServ = _provider.GetRequiredService<Mock<IMonitoredScriptTargetService>>();
        mockServ.Verify(service => service.GetQueryable(), Times.AtLeastOnce());

        var mockSave = _provider.GetRequiredService<Mock<ISaveService>>();
        mockSave.Verify(service => service.SaveChangesAsync(), Times.Never());
    }

    [Test]
    public void Get_IsValid_WithOrderBy_ThrowsReturnODataErrorException()
    {
        int recordId = _recordId;

        var model = ApiVersions.V0_1.GenerateEdmModel();
        var queryOptions = ODataQueryOptionsHelper.CreateOptions<MonitoredScriptTargetDTO>(
            model,
            "GET",
            "https://localhost:7200",
            "api/v0.1",
            "monitoredResources",
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
        int recordId = _recordId;

        var model = ApiVersions.V0_1.GenerateEdmModel();
        var queryOptions = ODataQueryOptionsHelper.CreateOptions<MonitoredScriptTargetDTO>(
            model,
            "GET",
            "https://localhost:7200",
            "api/v0.1",
            "monitoredResources",
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
        int recordId = _recordId;

        var model = ApiVersions.V0_1.GenerateEdmModel();
        var queryOptions = ODataQueryOptionsHelper.CreateOptions<MonitoredScriptTargetDTO>(
            model,
            "GET",
            "https://localhost:7200",
            "api/v0.1",
            "monitoredResources",
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
        int recordId = _recordId;

        var model = ApiVersions.V0_1.GenerateEdmModel();
        var queryOptions = ODataQueryOptionsHelper.CreateOptions<MonitoredScriptTargetDTO>(
            model,
            "GET",
            "https://localhost:7200",
            "api/v0.1",
            "monitoredResources",
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
        var model = ApiVersions.V0_1.GenerateEdmModel();
        var queryOptions = ODataQueryOptionsHelper.CreateOptions<MonitoredScriptTargetDTO>(
            model,
            "POST",
            "https://localhost:7200",
            "api/v0.1",
            "monitoredResources",
            "",
            "");

        var record = new MonitoredScriptTargetDTO()
        {
            ScriptInterpreterId = 2,
        };

        var result = await _controller.Create(record, queryOptions);
        var status = result.GetStatusCode();
        var content = result.GetContent();

        Assert.Multiple(() =>
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(status, Is.EqualTo(HttpStatusCode.Created));
            Assert.That(content, Is.AssignableTo<MonitoredScriptTargetDTO>());
        });

        var mockServ = _provider.GetRequiredService<Mock<IMonitoredScriptTargetService>>();
        mockServ.Verify(service => service.Add(It.IsAny<MonitoredScriptTarget>()), Times.AtLeastOnce());

        var mockSave = _provider.GetRequiredService<Mock<ISaveService>>();
        mockSave.Verify(service => service.SaveChangesAsync(), Times.Once());
    }

    [Test]
    public void Create_IsValid_WithFilter_ThrowsReturnODataErrorException()
    {
        var model = ApiVersions.V0_1.GenerateEdmModel();
        var queryOptions = ODataQueryOptionsHelper.CreateOptions<MonitoredScriptTargetDTO>(
            model,
            "GET",
            "https://localhost:7200",
            "api/v0.1",
            "monitoredResources",
            "",
            $"$filter=id eq {_recordId}");

        var record = new MonitoredScriptTargetDTO()
        {
            ScriptInterpreterId = 2,
        };

        Assert.ThrowsAsync<ReturnODataErrorException>(async () =>
        {
            var result = await _controller.Create(record, queryOptions);
        });
    }

    [Test]
    public async Task Create_IsValid_WithSelect_Returns201Created()
    {
        var model = ApiVersions.V0_1.GenerateEdmModel();
        var queryOptions = ODataQueryOptionsHelper.CreateOptions<MonitoredScriptTargetDTO>(
            model,
            "GET",
            "https://localhost:7200",
            "api/v0.1",
            "monitoredResources",
            "",
            "$select=id");

        var record = new MonitoredScriptTargetDTO()
        {
            ScriptInterpreterId = 2,
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

        var mockServ = _provider.GetRequiredService<Mock<IMonitoredScriptTargetService>>();
        mockServ.Verify(service => service.Add(It.IsAny<MonitoredScriptTarget>()), Times.AtLeastOnce());

        var mockSave = _provider.GetRequiredService<Mock<ISaveService>>();
        mockSave.Verify(service => service.SaveChangesAsync(), Times.Once());
    }

    [Test]
    public void Create_IsValid_WithOrderBy_ThrowsReturnODataErrorException()
    {
        var model = ApiVersions.V0_1.GenerateEdmModel();
        var queryOptions = ODataQueryOptionsHelper.CreateOptions<MonitoredScriptTargetDTO>(
            model,
            "GET",
            "https://localhost:7200",
            "api/v0.1",
            "monitoredResources",
            "",
            "$orderby=id");

        var record = new MonitoredScriptTargetDTO()
        {
            ScriptInterpreterId = 2,
        };

        Assert.ThrowsAsync<ReturnODataErrorException>(async () =>
        {
            var result = await _controller.Create(record, queryOptions);
        });
    }

    [Test]
    public void Create_IsValid_WithTop_ThrowsReturnODataErrorException()
    {
        var model = ApiVersions.V0_1.GenerateEdmModel();
        var queryOptions = ODataQueryOptionsHelper.CreateOptions<MonitoredScriptTargetDTO>(
            model,
            "GET",
            "https://localhost:7200",
            "api/v0.1",
            "monitoredResources",
            "",
            "$top=1");

        var record = new MonitoredScriptTargetDTO()
        {
            ScriptInterpreterId = 2,
        };

        Assert.ThrowsAsync<ReturnODataErrorException>(async () =>
        {
            var result = await _controller.Create(record, queryOptions);
        });
    }

    [Test]
    public void Create_IsValid_WithSkip_ThrowsReturnODataErrorException()
    {
        var model = ApiVersions.V0_1.GenerateEdmModel();
        var queryOptions = ODataQueryOptionsHelper.CreateOptions<MonitoredScriptTargetDTO>(
            model,
            "GET",
            "https://localhost:7200",
            "api/v0.1",
            "monitoredResources",
            "",
            "$skip=1");

        var record = new MonitoredScriptTargetDTO()
        {
            ScriptInterpreterId = 2,
        };

        Assert.ThrowsAsync<ReturnODataErrorException>(async () =>
        {
            var result = await _controller.Create(record, queryOptions);
        });
    }

    [Test]
    public void Create_IsValid_WithCount_ThrowsReturnODataErrorException()
    {
        var model = ApiVersions.V0_1.GenerateEdmModel();
        var queryOptions = ODataQueryOptionsHelper.CreateOptions<MonitoredScriptTargetDTO>(
            model,
            "GET",
            "https://localhost:7200",
            "api/v0.1",
            "monitoredResources",
            "",
            "$count=true");

        var record = new MonitoredScriptTargetDTO()
        {
            ScriptInterpreterId = 2,
        };

        Assert.ThrowsAsync<ReturnODataErrorException>(async () =>
        {
            var result = await _controller.Create(record, queryOptions);
        });
    }

    [Test]
    public async Task Update_IsInvalid_Returns404NotFound()
    {
        var recordId = new Random().Next(100, 10000);

        var model = ApiVersions.V0_1.GenerateEdmModel();
        var queryOptions = ODataQueryOptionsHelper.CreateOptions<MonitoredScriptTargetDTO>(
            model,
            "PATCH",
            "https://localhost:7200",
            "api/v0.1",
            "monitoredResources",
            $"({recordId})",
            "");

        var record = new Delta<MonitoredScriptTargetDTO>();
        record.TrySetPropertyValue(nameof(MonitoredScriptTargetDTO.ScriptInterpreterId), 2);

        var result = await _controller.Update(recordId, record, queryOptions);
        var status = result.GetStatusCode();
        var content = result.GetContent();

        Assert.Multiple(() =>
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(status, Is.EqualTo(HttpStatusCode.NotFound));
            Assert.That(content, Is.AssignableTo<ODataError>());
        });

        var mockServ = _provider.GetRequiredService<Mock<IMonitoredScriptTargetService>>();
        mockServ.Verify(service => service.GetQueryable(), Times.AtLeastOnce());

        var mockSave = _provider.GetRequiredService<Mock<ISaveService>>();
        mockSave.Verify(service => service.SaveChangesAsync(), Times.Never());
    }

    [Test]
    public async Task Update_IsValid_Returns200Ok()
    {
        var recordId = _recordId;

        var model = ApiVersions.V0_1.GenerateEdmModel();
        var queryOptions = ODataQueryOptionsHelper.CreateOptions<MonitoredScriptTargetDTO>(
            model,
            "POST",
            "https://localhost:7200",
            "api/v0.1",
            "monitoredResources",
            $"({recordId})",
            "");

        var record = new Delta<MonitoredScriptTargetDTO>();
        record.TrySetPropertyValue(nameof(MonitoredScriptTargetDTO.ScriptInterpreterId), 2);

        var result = await _controller.Update(recordId, record, queryOptions);
        var status = result.GetStatusCode();
        var content = result.GetContent();

        Assert.Multiple(() =>
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(status, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(content, Is.AssignableTo<MonitoredScriptTargetDTO>());
        });

        var mockServ = _provider.GetRequiredService<Mock<IMonitoredScriptTargetService>>();
        mockServ.Verify(service => service.GetQueryable(), Times.AtLeastOnce());

        var mockSave = _provider.GetRequiredService<Mock<ISaveService>>();
        mockSave.Verify(service => service.SaveChangesAsync(), Times.Once());
    }

    [Test]
    public void Update_IsValid_WithFilter_ThrowsReturnODataErrorException()
    {
        var recordId = _recordId;

        var model = ApiVersions.V0_1.GenerateEdmModel();
        var queryOptions = ODataQueryOptionsHelper.CreateOptions<MonitoredScriptTargetDTO>(
            model,
            "GET",
            "https://localhost:7200",
            "api/v0.1",
            "monitoredResources",
            $"({recordId})",
            $"$filter=id eq {_recordId}");

        var record = new Delta<MonitoredScriptTargetDTO>();
        record.TrySetPropertyValue(nameof(MonitoredScriptTargetDTO.ScriptInterpreterId), 2);

        Assert.ThrowsAsync<ReturnODataErrorException>(async () =>
        {
            var result = await _controller.Update(recordId, record, queryOptions);
        });
    }

    [Test]
    public async Task Update_IsValid_WithSelect_Returns200Ok()
    {
        var recordId = _recordId;

        var model = ApiVersions.V0_1.GenerateEdmModel();
        var queryOptions = ODataQueryOptionsHelper.CreateOptions<MonitoredScriptTargetDTO>(
            model,
            "GET",
            "https://localhost:7200",
            "api/v0.1",
            "monitoredResources",
            $"({recordId})",
            "$select=id");

        var record = new Delta<MonitoredScriptTargetDTO>();
        record.TrySetPropertyValue(nameof(MonitoredScriptTargetDTO.ScriptInterpreterId), 2);

        var result = await _controller.Update(recordId, record, queryOptions);
        var status = result.GetStatusCode();
        var content = result.GetContent();

        Assert.Multiple(() =>
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(status, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(content, Is.AssignableTo<ISelectExpandWrapper>());
        });

        var mockServ = _provider.GetRequiredService<Mock<IMonitoredScriptTargetService>>();
        mockServ.Verify(service => service.GetQueryable(), Times.AtLeastOnce());

        var mockSave = _provider.GetRequiredService<Mock<ISaveService>>();
        mockSave.Verify(service => service.SaveChangesAsync(), Times.Once());
    }

    [Test]
    public void Update_IsValid_WithOrderBy_ThrowsReturnODataErrorException()
    {
        var recordId = _recordId;

        var model = ApiVersions.V0_1.GenerateEdmModel();
        var queryOptions = ODataQueryOptionsHelper.CreateOptions<MonitoredScriptTargetDTO>(
            model,
            "GET",
            "https://localhost:7200",
            "api/v0.1",
            "monitoredResources",
            $"({_recordId})",
            "$orderby=id");

        var record = new Delta<MonitoredScriptTargetDTO>();
        record.TrySetPropertyValue(nameof(MonitoredScriptTargetDTO.ScriptInterpreterId), 2);

        Assert.ThrowsAsync<ReturnODataErrorException>(async () =>
        {
            var result = await _controller.Update(recordId, record, queryOptions);
        });
    }

    [Test]
    public void Update_IsValid_WithTop_ThrowsReturnODataErrorException()
    {
        var recordId = _recordId;

        var model = ApiVersions.V0_1.GenerateEdmModel();
        var queryOptions = ODataQueryOptionsHelper.CreateOptions<MonitoredScriptTargetDTO>(
            model,
            "GET",
            "https://localhost:7200",
            "api/v0.1",
            "monitoredResources",
            $"({recordId})",
            "$top=1");

        var record = new Delta<MonitoredScriptTargetDTO>();
        record.TrySetPropertyValue(nameof(MonitoredScriptTargetDTO.ScriptInterpreterId), 2);

        Assert.ThrowsAsync<ReturnODataErrorException>(async () =>
        {
            var result = await _controller.Update(recordId, record, queryOptions);
        });
    }

    [Test]
    public void Update_IsValid_WithSkip_ThrowsReturnODataErrorException()
    {
        var recordId = _recordId;

        var model = ApiVersions.V0_1.GenerateEdmModel();
        var queryOptions = ODataQueryOptionsHelper.CreateOptions<MonitoredScriptTargetDTO>(
            model,
            "GET",
            "https://localhost:7200",
            "api/v0.1",
            "monitoredResources",
            $"({recordId})",
            "$skip=1");

        var record = new Delta<MonitoredScriptTargetDTO>();
        record.TrySetPropertyValue(nameof(MonitoredScriptTargetDTO.ScriptInterpreterId), "New Name");

        Assert.ThrowsAsync<ReturnODataErrorException>(async () =>
        {
            var result = await _controller.Update(recordId, record, queryOptions);
        });
    }

    [Test]
    public void Update_IsValid_WithCount_ThrowsReturnODataErrorException()
    {
        var recordId = _recordId;

        var model = ApiVersions.V0_1.GenerateEdmModel();
        var queryOptions = ODataQueryOptionsHelper.CreateOptions<MonitoredScriptTargetDTO>(
            model,
            "GET",
            "https://localhost:7200",
            "api/v0.1",
            "monitoredResources",
            $"({recordId})",
            "$count=true");

        var record = new Delta<MonitoredScriptTargetDTO>();
        record.TrySetPropertyValue(nameof(MonitoredScriptTargetDTO.ScriptInterpreterId), 2);

        Assert.ThrowsAsync<ReturnODataErrorException>(async () =>
        {
            var result = await _controller.Update(recordId, record, queryOptions);
        });
    }

    [Test]
    public async Task Delete_IsInvalid_Returns404NotFound()
    {
        int recordId = -1;

        var model = ApiVersions.V0_1.GenerateEdmModel();
        var queryOptions = ODataQueryOptionsHelper.CreateOptions<MonitoredScriptTargetDTO>(
            model,
            "DELETE",
            "https://localhost:7200",
            "api/v0.1",
            "monitoredResources",
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

        var mockServ = _provider.GetRequiredService<Mock<IMonitoredScriptTargetService>>();
        mockServ.Verify(service => service.GetQueryable(), Times.AtLeastOnce());

        var mockSave = _provider.GetRequiredService<Mock<ISaveService>>();
        mockSave.Verify(service => service.SaveChangesAsync(), Times.Never());
    }

    [Test]
    public async Task Delete_IsValid_Returns204NoContent()
    {
        int recordId = _recordId;

        var model = ApiVersions.V0_1.GenerateEdmModel();
        var queryOptions = ODataQueryOptionsHelper.CreateOptions<MonitoredScriptTargetDTO>(
            model,
            "DELETE",
            "https://localhost:7200",
            "api/v0.1",
            "monitoredResources",
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

        var mockServ = _provider.GetRequiredService<Mock<IMonitoredScriptTargetService>>();
        mockServ.Verify(service => service.GetQueryable(), Times.AtLeastOnce());

        var mockSave = _provider.GetRequiredService<Mock<ISaveService>>();
        mockSave.Verify(service => service.SaveChangesAsync(), Times.Once());
    }
}
