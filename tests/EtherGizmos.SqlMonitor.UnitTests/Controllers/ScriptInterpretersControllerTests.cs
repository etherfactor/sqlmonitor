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

internal class ScriptInterpretersControllerTests
{
    private IServiceProvider _provider;

    private ScriptInterpretersController _controller;

    private List<ScriptInterpreter> _data;

    private readonly int _recordId = 1;

    [SetUp]
    public void SetUp()
    {
        _provider = Global.CreateScope();
        _controller = _provider.GetRequiredService<ScriptInterpretersController>();
        _data = new List<ScriptInterpreter>()
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
                Id = 2,
                Name = "Test 2",
                Description = null,
                IsSoftDeleted = false,
            },
            new()
            {
                Id = 3,
                Name = "Test 3",
                Description = null,
                IsSoftDeleted = false,
            },
            new()
            {
                Id = 4,
                Name = "Test 4",
                Description = null,
                IsSoftDeleted = true,
            }
        };

        var mockData = _data.AsQueryable().BuildMock();

        var mockServ = _provider.GetRequiredService<Mock<IScriptInterpreterService>>();
        mockServ.Setup(service => service.GetQueryable()).Returns(mockData);

        var mockSave = _provider.GetRequiredService<Mock<ISaveService>>();
        mockSave.Setup(service => service.SaveChangesAsync()).Returns(Task.CompletedTask);
    }

    [Test]
    public async Task Search_IsValid_Returns200Ok()
    {
        var model = ApiVersions.V0_1.GenerateEdmModel();
        var queryOptions = ODataQueryOptionsHelper.CreateOptions<ScriptInterpreterDTO>(
            model,
            "GET",
            "https://localhost:7200",
            "api/v0.1",
            "scriptInterpreters",
            "",
            "");

        var result = await _controller.Search(queryOptions);
        var status = result.GetStatusCode();
        var content = result.GetContent();

        Assert.Multiple(() =>
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(status, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(content, Is.AssignableTo<IEnumerable<ScriptInterpreterDTO>>());
        });

        var mockServ = _provider.GetRequiredService<Mock<IScriptInterpreterService>>();
        mockServ.Verify(service => service.GetQueryable(), Times.AtLeastOnce());

        var mockSave = _provider.GetRequiredService<Mock<ISaveService>>();
        mockSave.Verify(service => service.SaveChangesAsync(), Times.Never());
    }

    [Test]
    public async Task Search_IsValid_WithFilter_Returns200Ok()
    {
        var model = ApiVersions.V0_1.GenerateEdmModel();
        var queryOptions = ODataQueryOptionsHelper.CreateOptions<ScriptInterpreterDTO>(
            model,
            "GET",
            "https://localhost:7200",
            "api/v0.1",
            "scriptInterpreters",
            "",
            $"$filter=id eq {_recordId}");

        var result = await _controller.Search(queryOptions);
        var status = result.GetStatusCode();
        var content = result.GetContent();

        Assert.Multiple(() =>
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(status, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(content, Is.AssignableTo<IEnumerable<ScriptInterpreterDTO>>());
        });

        var mockServ = _provider.GetRequiredService<Mock<IScriptInterpreterService>>();
        mockServ.Verify(service => service.GetQueryable(), Times.AtLeastOnce());

        var mockSave = _provider.GetRequiredService<Mock<ISaveService>>();
        mockSave.Verify(service => service.SaveChangesAsync(), Times.Never());
    }

    [Test]
    public async Task Search_IsValid_WithSelect_Returns200Ok()
    {
        var model = ApiVersions.V0_1.GenerateEdmModel();
        var queryOptions = ODataQueryOptionsHelper.CreateOptions<ScriptInterpreterDTO>(
            model,
            "GET",
            "https://localhost:7200",
            "api/v0.1",
            "scriptInterpreters",
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

        var mockServ = _provider.GetRequiredService<Mock<IScriptInterpreterService>>();
        mockServ.Verify(service => service.GetQueryable(), Times.AtLeastOnce());

        var mockSave = _provider.GetRequiredService<Mock<ISaveService>>();
        mockSave.Verify(service => service.SaveChangesAsync(), Times.Never());
    }

    [Test]
    public async Task Search_IsValid_WithOrderBy_Returns200Ok()
    {
        var model = ApiVersions.V0_1.GenerateEdmModel();
        var queryOptions = ODataQueryOptionsHelper.CreateOptions<ScriptInterpreterDTO>(
            model,
            "GET",
            "https://localhost:7200",
            "api/v0.1",
            "scriptInterpreters",
            "",
            "$orderby=name");

        var result = await _controller.Search(queryOptions);
        var status = result.GetStatusCode();
        var content = result.GetContent();

        Assert.Multiple(() =>
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(status, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(content, Is.AssignableTo<IEnumerable<ScriptInterpreterDTO>>());
        });

        var mockServ = _provider.GetRequiredService<Mock<IScriptInterpreterService>>();
        mockServ.Verify(service => service.GetQueryable(), Times.AtLeastOnce());

        var mockSave = _provider.GetRequiredService<Mock<ISaveService>>();
        mockSave.Verify(service => service.SaveChangesAsync(), Times.Never());
    }

    [Test]
    public async Task Search_IsValid_WithTop_Returns200Ok()
    {
        var model = ApiVersions.V0_1.GenerateEdmModel();
        var queryOptions = ODataQueryOptionsHelper.CreateOptions<ScriptInterpreterDTO>(
            model,
            "GET",
            "https://localhost:7200",
            "api/v0.1",
            "scriptInterpreters",
            "",
            "$top=1");

        var result = await _controller.Search(queryOptions);
        var status = result.GetStatusCode();
        var content = result.GetContent();

        Assert.Multiple(() =>
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(status, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(content, Is.AssignableTo<IEnumerable<ScriptInterpreterDTO>>());
        });

        var mockServ = _provider.GetRequiredService<Mock<IScriptInterpreterService>>();
        mockServ.Verify(service => service.GetQueryable(), Times.AtLeastOnce());

        var mockSave = _provider.GetRequiredService<Mock<ISaveService>>();
        mockSave.Verify(service => service.SaveChangesAsync(), Times.Never());
    }

    [Test]
    public async Task Search_IsValid_WithSkip_Returns200Ok()
    {
        var model = ApiVersions.V0_1.GenerateEdmModel();
        var queryOptions = ODataQueryOptionsHelper.CreateOptions<ScriptInterpreterDTO>(
            model,
            "GET",
            "https://localhost:7200",
            "api/v0.1",
            "scriptInterpreters",
            "",
            "$skip=1");

        var result = await _controller.Search(queryOptions);
        var status = result.GetStatusCode();
        var content = result.GetContent();

        Assert.Multiple(() =>
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(status, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(content, Is.AssignableTo<IEnumerable<ScriptInterpreterDTO>>());
        });

        var mockServ = _provider.GetRequiredService<Mock<IScriptInterpreterService>>();
        mockServ.Verify(service => service.GetQueryable(), Times.AtLeastOnce());

        var mockSave = _provider.GetRequiredService<Mock<ISaveService>>();
        mockSave.Verify(service => service.SaveChangesAsync(), Times.Never());
    }

    [Test]
    public async Task Search_IsValid_WithCount_Returns200Ok()
    {
        var model = ApiVersions.V0_1.GenerateEdmModel();
        var queryOptions = ODataQueryOptionsHelper.CreateOptions<ScriptInterpreterDTO>(
            model,
            "GET",
            "https://localhost:7200",
            "api/v0.1",
            "scriptInterpreters",
            "",
            "$count=true");

        var result = await _controller.Search(queryOptions);
        var status = result.GetStatusCode();
        var content = result.GetContent();

        Assert.Multiple(() =>
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(status, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(content, Is.AssignableTo<IEnumerable<ScriptInterpreterDTO>>());
        });

        var mockServ = _provider.GetRequiredService<Mock<IScriptInterpreterService>>();
        mockServ.Verify(service => service.GetQueryable(), Times.AtLeastOnce());

        var mockSave = _provider.GetRequiredService<Mock<ISaveService>>();
        mockSave.Verify(service => service.SaveChangesAsync(), Times.Never());
    }

    [Test]
    public async Task Get_IsInvalid_Returns404NotFound()
    {
        int recordId = new Random().Next(100, 10000);

        var model = ApiVersions.V0_1.GenerateEdmModel();
        var queryOptions = ODataQueryOptionsHelper.CreateOptions<ScriptInterpreterDTO>(
            model,
            "GET",
            "https://localhost:7200",
            "api/v0.1",
            "scriptInterpreters",
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

        var mockServ = _provider.GetRequiredService<Mock<IScriptInterpreterService>>();
        mockServ.Verify(service => service.GetQueryable(), Times.AtLeastOnce());

        var mockSave = _provider.GetRequiredService<Mock<ISaveService>>();
        mockSave.Verify(service => service.SaveChangesAsync(), Times.Never());
    }

    [Test]
    public async Task Get_IsValid_Returns200Ok()
    {
        int recordId = _recordId;

        var model = ApiVersions.V0_1.GenerateEdmModel();
        var queryOptions = ODataQueryOptionsHelper.CreateOptions<ScriptInterpreterDTO>(
            model,
            "GET",
            "https://localhost:7200",
            "api/v0.1",
            "scriptInterpreters",
            $"('{recordId}')",
            "");

        var result = await _controller.Get(recordId, queryOptions);
        var status = result.GetStatusCode();
        var content = result.GetContent();

        Assert.Multiple(() =>
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(status, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(content, Is.AssignableTo<ScriptInterpreterDTO>());
        });

        var mockServ = _provider.GetRequiredService<Mock<IScriptInterpreterService>>();
        mockServ.Verify(service => service.GetQueryable(), Times.AtLeastOnce());

        var mockSave = _provider.GetRequiredService<Mock<ISaveService>>();
        mockSave.Verify(service => service.SaveChangesAsync(), Times.Never());
    }

    [Test]
    public void Get_IsValid_WithFilter_ThrowsReturnODataErrorException()
    {
        int recordId = _recordId;

        var model = ApiVersions.V0_1.GenerateEdmModel();
        var queryOptions = ODataQueryOptionsHelper.CreateOptions<ScriptInterpreterDTO>(
            model,
            "GET",
            "https://localhost:7200",
            "api/v0.1",
            "scriptInterpreters",
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
        var queryOptions = ODataQueryOptionsHelper.CreateOptions<ScriptInterpreterDTO>(
            model,
            "GET",
            "https://localhost:7200",
            "api/v0.1",
            "scriptInterpreters",
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

        var mockServ = _provider.GetRequiredService<Mock<IScriptInterpreterService>>();
        mockServ.Verify(service => service.GetQueryable(), Times.AtLeastOnce());

        var mockSave = _provider.GetRequiredService<Mock<ISaveService>>();
        mockSave.Verify(service => service.SaveChangesAsync(), Times.Never());
    }

    [Test]
    public void Get_IsValid_WithOrderBy_ThrowsReturnODataErrorException()
    {
        int recordId = _recordId;

        var model = ApiVersions.V0_1.GenerateEdmModel();
        var queryOptions = ODataQueryOptionsHelper.CreateOptions<ScriptInterpreterDTO>(
            model,
            "GET",
            "https://localhost:7200",
            "api/v0.1",
            "scriptInterpreters",
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
        var queryOptions = ODataQueryOptionsHelper.CreateOptions<ScriptInterpreterDTO>(
            model,
            "GET",
            "https://localhost:7200",
            "api/v0.1",
            "scriptInterpreters",
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
        var queryOptions = ODataQueryOptionsHelper.CreateOptions<ScriptInterpreterDTO>(
            model,
            "GET",
            "https://localhost:7200",
            "api/v0.1",
            "scriptInterpreters",
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
        var queryOptions = ODataQueryOptionsHelper.CreateOptions<ScriptInterpreterDTO>(
            model,
            "GET",
            "https://localhost:7200",
            "api/v0.1",
            "scriptInterpreters",
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
        var queryOptions = ODataQueryOptionsHelper.CreateOptions<ScriptInterpreterDTO>(
            model,
            "POST",
            "https://localhost:7200",
            "api/v0.1",
            "scriptInterpreters",
            "",
            "");

        var record = new ScriptInterpreterDTO()
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
            Assert.That(content, Is.AssignableTo<ScriptInterpreterDTO>());
        });

        var mockServ = _provider.GetRequiredService<Mock<IScriptInterpreterService>>();
        mockServ.Verify(service => service.Add(It.IsAny<ScriptInterpreter>()), Times.AtLeastOnce());

        var mockSave = _provider.GetRequiredService<Mock<ISaveService>>();
        mockSave.Verify(service => service.SaveChangesAsync(), Times.Once());
    }

    [Test]
    public void Create_IsValid_WithFilter_ThrowsReturnODataErrorException()
    {
        var model = ApiVersions.V0_1.GenerateEdmModel();
        var queryOptions = ODataQueryOptionsHelper.CreateOptions<ScriptInterpreterDTO>(
            model,
            "GET",
            "https://localhost:7200",
            "api/v0.1",
            "scriptInterpreters",
            "",
            $"$filter=id eq {_recordId}");

        var record = new ScriptInterpreterDTO()
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
        var model = ApiVersions.V0_1.GenerateEdmModel();
        var queryOptions = ODataQueryOptionsHelper.CreateOptions<ScriptInterpreterDTO>(
            model,
            "GET",
            "https://localhost:7200",
            "api/v0.1",
            "scriptInterpreters",
            "",
            "$select=id");

        var record = new ScriptInterpreterDTO()
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

        var mockServ = _provider.GetRequiredService<Mock<IScriptInterpreterService>>();
        mockServ.Verify(service => service.Add(It.IsAny<ScriptInterpreter>()), Times.AtLeastOnce());

        var mockSave = _provider.GetRequiredService<Mock<ISaveService>>();
        mockSave.Verify(service => service.SaveChangesAsync(), Times.Once());
    }

    [Test]
    public void Create_IsValid_WithOrderBy_ThrowsReturnODataErrorException()
    {
        var model = ApiVersions.V0_1.GenerateEdmModel();
        var queryOptions = ODataQueryOptionsHelper.CreateOptions<ScriptInterpreterDTO>(
            model,
            "GET",
            "https://localhost:7200",
            "api/v0.1",
            "scriptInterpreters",
            "",
            "$orderby=id");

        var record = new ScriptInterpreterDTO()
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
        var model = ApiVersions.V0_1.GenerateEdmModel();
        var queryOptions = ODataQueryOptionsHelper.CreateOptions<ScriptInterpreterDTO>(
            model,
            "GET",
            "https://localhost:7200",
            "api/v0.1",
            "scriptInterpreters",
            "",
            "$top=1");

        var record = new ScriptInterpreterDTO()
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
        var model = ApiVersions.V0_1.GenerateEdmModel();
        var queryOptions = ODataQueryOptionsHelper.CreateOptions<ScriptInterpreterDTO>(
            model,
            "GET",
            "https://localhost:7200",
            "api/v0.1",
            "scriptInterpreters",
            "",
            "$skip=1");

        var record = new ScriptInterpreterDTO()
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
        var model = ApiVersions.V0_1.GenerateEdmModel();
        var queryOptions = ODataQueryOptionsHelper.CreateOptions<ScriptInterpreterDTO>(
            model,
            "GET",
            "https://localhost:7200",
            "api/v0.1",
            "scriptInterpreters",
            "",
            "$count=true");

        var record = new ScriptInterpreterDTO()
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
        var recordId = new Random().Next(100, 10000);

        var model = ApiVersions.V0_1.GenerateEdmModel();
        var queryOptions = ODataQueryOptionsHelper.CreateOptions<ScriptInterpreterDTO>(
            model,
            "PATCH",
            "https://localhost:7200",
            "api/v0.1",
            "scriptInterpreters",
            $"({recordId})",
            "");

        var record = new Delta<ScriptInterpreterDTO>();
        record.TrySetPropertyValue(nameof(ScriptInterpreterDTO.Name), "New Name");

        var result = await _controller.Update(recordId, record, queryOptions);
        var status = result.GetStatusCode();
        var content = result.GetContent();

        Assert.Multiple(() =>
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(status, Is.EqualTo(HttpStatusCode.NotFound));
            Assert.That(content, Is.AssignableTo<ODataError>());
        });

        var mockServ = _provider.GetRequiredService<Mock<IScriptInterpreterService>>();
        mockServ.Verify(service => service.GetQueryable(), Times.AtLeastOnce());

        var mockSave = _provider.GetRequiredService<Mock<ISaveService>>();
        mockSave.Verify(service => service.SaveChangesAsync(), Times.Never());
    }

    [Test]
    public async Task Update_IsValid_Returns200Ok()
    {
        var recordId = _recordId;

        var model = ApiVersions.V0_1.GenerateEdmModel();
        var queryOptions = ODataQueryOptionsHelper.CreateOptions<ScriptInterpreterDTO>(
            model,
            "POST",
            "https://localhost:7200",
            "api/v0.1",
            "scriptInterpreters",
            $"({recordId})",
            "");

        var record = new Delta<ScriptInterpreterDTO>();
        record.TrySetPropertyValue(nameof(ScriptInterpreterDTO.Name), "New Name");

        var result = await _controller.Update(recordId, record, queryOptions);
        var status = result.GetStatusCode();
        var content = result.GetContent();

        Assert.Multiple(() =>
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(status, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(content, Is.AssignableTo<ScriptInterpreterDTO>());
        });

        var mockServ = _provider.GetRequiredService<Mock<IScriptInterpreterService>>();
        mockServ.Verify(service => service.GetQueryable(), Times.AtLeastOnce());

        var mockSave = _provider.GetRequiredService<Mock<ISaveService>>();
        mockSave.Verify(service => service.SaveChangesAsync(), Times.Once());
    }

    [Test]
    public void Update_IsValid_WithFilter_ThrowsReturnODataErrorException()
    {
        var recordId = _recordId;

        var model = ApiVersions.V0_1.GenerateEdmModel();
        var queryOptions = ODataQueryOptionsHelper.CreateOptions<ScriptInterpreterDTO>(
            model,
            "GET",
            "https://localhost:7200",
            "api/v0.1",
            "scriptInterpreters",
            $"({recordId})",
            $"$filter=id eq {_recordId}");

        var record = new Delta<ScriptInterpreterDTO>();
        record.TrySetPropertyValue(nameof(ScriptInterpreterDTO.Name), "New Name");

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
        var queryOptions = ODataQueryOptionsHelper.CreateOptions<ScriptInterpreterDTO>(
            model,
            "GET",
            "https://localhost:7200",
            "api/v0.1",
            "scriptInterpreters",
            $"({recordId})",
            "$select=id");

        var record = new Delta<ScriptInterpreterDTO>();
        record.TrySetPropertyValue(nameof(ScriptInterpreterDTO.Name), "New Name");

        var result = await _controller.Update(recordId, record, queryOptions);
        var status = result.GetStatusCode();
        var content = result.GetContent();

        Assert.Multiple(() =>
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(status, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(content, Is.AssignableTo<ISelectExpandWrapper>());
        });

        var mockServ = _provider.GetRequiredService<Mock<IScriptInterpreterService>>();
        mockServ.Verify(service => service.GetQueryable(), Times.AtLeastOnce());

        var mockSave = _provider.GetRequiredService<Mock<ISaveService>>();
        mockSave.Verify(service => service.SaveChangesAsync(), Times.Once());
    }

    [Test]
    public void Update_IsValid_WithOrderBy_ThrowsReturnODataErrorException()
    {
        var recordId = _recordId;

        var model = ApiVersions.V0_1.GenerateEdmModel();
        var queryOptions = ODataQueryOptionsHelper.CreateOptions<ScriptInterpreterDTO>(
            model,
            "GET",
            "https://localhost:7200",
            "api/v0.1",
            "scriptInterpreters",
            $"({_recordId})",
            "$orderby=id");

        var record = new Delta<ScriptInterpreterDTO>();
        record.TrySetPropertyValue(nameof(ScriptInterpreterDTO.Name), "New Name");

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
        var queryOptions = ODataQueryOptionsHelper.CreateOptions<ScriptInterpreterDTO>(
            model,
            "GET",
            "https://localhost:7200",
            "api/v0.1",
            "scriptInterpreters",
            $"({recordId})",
            "$top=1");

        var record = new Delta<ScriptInterpreterDTO>();
        record.TrySetPropertyValue(nameof(ScriptInterpreterDTO.Name), "New Name");

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
        var queryOptions = ODataQueryOptionsHelper.CreateOptions<ScriptInterpreterDTO>(
            model,
            "GET",
            "https://localhost:7200",
            "api/v0.1",
            "scriptInterpreters",
            $"({recordId})",
            "$skip=1");

        var record = new Delta<ScriptInterpreterDTO>();
        record.TrySetPropertyValue(nameof(ScriptInterpreterDTO.Name), "New Name");

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
        var queryOptions = ODataQueryOptionsHelper.CreateOptions<ScriptInterpreterDTO>(
            model,
            "GET",
            "https://localhost:7200",
            "api/v0.1",
            "scriptInterpreters",
            $"({recordId})",
            "$count=true");

        var record = new Delta<ScriptInterpreterDTO>();
        record.TrySetPropertyValue(nameof(ScriptInterpreterDTO.Name), "New Name");

        Assert.ThrowsAsync<ReturnODataErrorException>(async () =>
        {
            var result = await _controller.Update(recordId, record, queryOptions);
        });
    }

    [Test]
    public async Task Delete_IsInvalid_Returns404NotFound()
    {
        int recordId = new Random().Next(100, 10000);

        var model = ApiVersions.V0_1.GenerateEdmModel();
        var queryOptions = ODataQueryOptionsHelper.CreateOptions<ScriptInterpreterDTO>(
            model,
            "DELETE",
            "https://localhost:7200",
            "api/v0.1",
            "scriptInterpreters",
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

        var mockServ = _provider.GetRequiredService<Mock<IScriptInterpreterService>>();
        mockServ.Verify(service => service.GetQueryable(), Times.AtLeastOnce());

        var mockSave = _provider.GetRequiredService<Mock<ISaveService>>();
        mockSave.Verify(service => service.SaveChangesAsync(), Times.Never());
    }

    [Test]
    public async Task Delete_IsValid_Returns204NoContent()
    {
        int recordId = _recordId;

        var model = ApiVersions.V0_1.GenerateEdmModel();
        var queryOptions = ODataQueryOptionsHelper.CreateOptions<ScriptInterpreterDTO>(
            model,
            "DELETE",
            "https://localhost:7200",
            "api/v0.1",
            "scriptInterpreters",
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

        var mockServ = _provider.GetRequiredService<Mock<IScriptInterpreterService>>();
        mockServ.Verify(service => service.GetQueryable(), Times.AtLeastOnce());

        var mockSave = _provider.GetRequiredService<Mock<ISaveService>>();
        mockSave.Verify(service => service.SaveChangesAsync(), Times.Once());
    }
}
