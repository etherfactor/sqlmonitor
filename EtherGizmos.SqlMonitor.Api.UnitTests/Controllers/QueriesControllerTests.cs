using EtherGizmos.SqlMonitor.Api.Controllers;
using EtherGizmos.SqlMonitor.Api.OData.Metadata;
using EtherGizmos.SqlMonitor.Api.Services.Abstractions;
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

internal class QueriesControllerTests
{
    private IServiceProvider Provider { get; set; }

    private QueriesController Controller { get; set; }

    private List<Query> Data { get; set; }

    private Guid RecordId { get; } = Guid.NewGuid();

    [SetUp]
    public void SetUp()
    {
        Provider = Global.CreateScope();
        Controller = Provider.GetRequiredService<QueriesController>();
        Data = new List<Query>()
        {
            new Query()
            {
                Id = RecordId,
                Name = "Test 1",
                Description = null,
                IsActive = true,
                IsSoftDeleted = false,
                SqlText = "select 1",
                RunFrequency = TimeSpan.FromSeconds(5),
                TimestampUtcExpression = null,
                BucketExpression = null
            },
            new Query()
            {
                Id = Guid.NewGuid(),
                Name = "Test 2",
                Description = null,
                IsActive = true,
                IsSoftDeleted = false,
                SqlText = "select 1",
                RunFrequency = TimeSpan.FromSeconds(5),
                TimestampUtcExpression = null,
                BucketExpression = null
            },
            new Query()
            {
                Id = Guid.NewGuid(),
                Name = "Test 3",
                Description = null,
                IsActive = false,
                IsSoftDeleted = false,
                SqlText = "select 1",
                RunFrequency = TimeSpan.FromSeconds(5),
                TimestampUtcExpression = null,
                BucketExpression = null
            },
            new Query()
            {
                Id = Guid.NewGuid(),
                Name = "Test 4",
                Description = null,
                IsActive = false,
                IsSoftDeleted = true,
                SqlText = "select 1",
                RunFrequency = TimeSpan.FromSeconds(5),
                TimestampUtcExpression = null,
                BucketExpression = null
            }
        };

        var mockData = Data.AsQueryable().BuildMock();

        var mockServ = Provider.GetRequiredService<Mock<IQueryService>>();
        mockServ.Setup(service => service.GetQueryable()).Returns(mockData);

        var mockSave = Provider.GetRequiredService<Mock<ISaveService>>();
        mockSave.Setup(service => service.SaveChangesAsync()).Returns(Task.CompletedTask);
    }

    [Test]
    public async Task Search_IsValid_Returns200Ok()
    {
        var model = ODataModel.GetEdmModel(1.0m);
        var queryOptions = ODataQueryOptionsHelper.CreateOptions<QueryDTO>(
            model,
            "GET",
            "https://localhost:7200",
            "api/v1",
            "queries",
            "",
            "");

        var result = await Controller.Search(queryOptions);
        var status = result.GetStatusCode();
        var content = result.GetContent();

        Assert.Multiple(() =>
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(status, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(content, Is.AssignableTo<IEnumerable<QueryDTO>>());
        });

        var mockServ = Provider.GetRequiredService<Mock<IQueryService>>();
        mockServ.Verify(service => service.GetQueryable(), Times.AtLeastOnce());

        var mockSave = Provider.GetRequiredService<Mock<ISaveService>>();
        mockSave.Verify(service => service.SaveChangesAsync(), Times.Never());
    }

    [Test]
    public async Task Search_IsValid_WithFilter_Returns200Ok()
    {
        var model = ODataModel.GetEdmModel(1.0m);
        var queryOptions = ODataQueryOptionsHelper.CreateOptions<QueryDTO>(
            model,
            "GET",
            "https://localhost:7200",
            "api/v1",
            "queries",
            "",
            $"$filter=id eq {RecordId}");

        var result = await Controller.Search(queryOptions);
        var status = result.GetStatusCode();
        var content = result.GetContent();

        Assert.Multiple(() =>
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(status, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(content, Is.AssignableTo<IEnumerable<QueryDTO>>());
        });

        var mockServ = Provider.GetRequiredService<Mock<IQueryService>>();
        mockServ.Verify(service => service.GetQueryable(), Times.AtLeastOnce());

        var mockSave = Provider.GetRequiredService<Mock<ISaveService>>();
        mockSave.Verify(service => service.SaveChangesAsync(), Times.Never());
    }

    //[Test]
    //public async Task Search_IsValid_WithExpand_Returns200Ok()
    //{
    //    var model = ODataModel.GetEdmModel(1.0m);
    //    var queryOptions = ODataQueryOptionsHelper.CreateOptions<QueryDTO>(
    //        model,
    //        "GET",
    //        "https://localhost:7200",
    //        "api/v1",
    //        "queries",
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

    //    var mockServ = Provider.GetRequiredService<Mock<IQueryService>>();
    //    mockServ.Verify(service => service.GetQueryable(), Times.AtLeastOnce());

    //    var mockSave = Provider.GetRequiredService<Mock<ISaveService>>();
    //    mockSave.Verify(service => service.SaveChangesAsync(), Times.Never());
    //}

    [Test]
    public async Task Search_IsValid_WithSelect_Returns200Ok()
    {
        var model = ODataModel.GetEdmModel(1.0m);
        var queryOptions = ODataQueryOptionsHelper.CreateOptions<QueryDTO>(
            model,
            "GET",
            "https://localhost:7200",
            "api/v1",
            "queries",
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

        var mockServ = Provider.GetRequiredService<Mock<IQueryService>>();
        mockServ.Verify(service => service.GetQueryable(), Times.AtLeastOnce());

        var mockSave = Provider.GetRequiredService<Mock<ISaveService>>();
        mockSave.Verify(service => service.SaveChangesAsync(), Times.Never());
    }

    [Test]
    public async Task Search_IsValid_WithOrderBy_Returns200Ok()
    {
        var model = ODataModel.GetEdmModel(1.0m);
        var queryOptions = ODataQueryOptionsHelper.CreateOptions<QueryDTO>(
            model,
            "GET",
            "https://localhost:7200",
            "api/v1",
            "queries",
            "",
            "$orderby=name");

        var result = await Controller.Search(queryOptions);
        var status = result.GetStatusCode();
        var content = result.GetContent();

        Assert.Multiple(() =>
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(status, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(content, Is.AssignableTo<IEnumerable<QueryDTO>>());
        });

        var mockServ = Provider.GetRequiredService<Mock<IQueryService>>();
        mockServ.Verify(service => service.GetQueryable(), Times.AtLeastOnce());

        var mockSave = Provider.GetRequiredService<Mock<ISaveService>>();
        mockSave.Verify(service => service.SaveChangesAsync(), Times.Never());
    }

    [Test]
    public async Task Search_IsValid_WithTop_Returns200Ok()
    {
        var model = ODataModel.GetEdmModel(1.0m);
        var queryOptions = ODataQueryOptionsHelper.CreateOptions<QueryDTO>(
            model,
            "GET",
            "https://localhost:7200",
            "api/v1",
            "queries",
            "",
            "$top=1");

        var result = await Controller.Search(queryOptions);
        var status = result.GetStatusCode();
        var content = result.GetContent();

        Assert.Multiple(() =>
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(status, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(content, Is.AssignableTo<IEnumerable<QueryDTO>>());
        });

        var mockServ = Provider.GetRequiredService<Mock<IQueryService>>();
        mockServ.Verify(service => service.GetQueryable(), Times.AtLeastOnce());

        var mockSave = Provider.GetRequiredService<Mock<ISaveService>>();
        mockSave.Verify(service => service.SaveChangesAsync(), Times.Never());
    }

    [Test]
    public async Task Search_IsValid_WithSkip_Returns200Ok()
    {
        var model = ODataModel.GetEdmModel(1.0m);
        var queryOptions = ODataQueryOptionsHelper.CreateOptions<QueryDTO>(
            model,
            "GET",
            "https://localhost:7200",
            "api/v1",
            "queries",
            "",
            "$skip=1");

        var result = await Controller.Search(queryOptions);
        var status = result.GetStatusCode();
        var content = result.GetContent();

        Assert.Multiple(() =>
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(status, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(content, Is.AssignableTo<IEnumerable<QueryDTO>>());
        });

        var mockServ = Provider.GetRequiredService<Mock<IQueryService>>();
        mockServ.Verify(service => service.GetQueryable(), Times.AtLeastOnce());

        var mockSave = Provider.GetRequiredService<Mock<ISaveService>>();
        mockSave.Verify(service => service.SaveChangesAsync(), Times.Never());
    }

    [Test]
    public async Task Search_IsValid_WithCount_Returns200Ok()
    {
        var model = ODataModel.GetEdmModel(1.0m);
        var queryOptions = ODataQueryOptionsHelper.CreateOptions<QueryDTO>(
            model,
            "GET",
            "https://localhost:7200",
            "api/v1",
            "queries",
            "",
            "$count=true");

        var result = await Controller.Search(queryOptions);
        var status = result.GetStatusCode();
        var content = result.GetContent();

        Assert.Multiple(() =>
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(status, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(content, Is.AssignableTo<IEnumerable<QueryDTO>>());
        });

        var mockServ = Provider.GetRequiredService<Mock<IQueryService>>();
        mockServ.Verify(service => service.GetQueryable(), Times.AtLeastOnce());

        var mockSave = Provider.GetRequiredService<Mock<ISaveService>>();
        mockSave.Verify(service => service.SaveChangesAsync(), Times.Never());
    }

    [Test]
    public async Task Get_IsInvalid_Returns404NotFound()
    {
        Guid recordId = new Guid();

        var model = ODataModel.GetEdmModel(1.0m);
        var queryOptions = ODataQueryOptionsHelper.CreateOptions<QueryDTO>(
            model,
            "GET",
            "https://localhost:7200",
            "api/v1",
            "queries",
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

        var mockServ = Provider.GetRequiredService<Mock<IQueryService>>();
        mockServ.Verify(service => service.GetQueryable(), Times.AtLeastOnce());

        var mockSave = Provider.GetRequiredService<Mock<ISaveService>>();
        mockSave.Verify(service => service.SaveChangesAsync(), Times.Never());
    }

    [Test]
    public async Task Get_IsValid_Returns200Ok()
    {
        Guid recordId = RecordId;

        var model = ODataModel.GetEdmModel(1.0m);
        var queryOptions = ODataQueryOptionsHelper.CreateOptions<QueryDTO>(
            model,
            "GET",
            "https://localhost:7200",
            "api/v1",
            "queries",
            $"('{recordId}')",
            "");

        var result = await Controller.Get(recordId, queryOptions);
        var status = result.GetStatusCode();
        var content = result.GetContent();

        Assert.Multiple(() =>
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(status, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(content, Is.AssignableTo<QueryDTO>());
        });

        var mockServ = Provider.GetRequiredService<Mock<IQueryService>>();
        mockServ.Verify(service => service.GetQueryable(), Times.AtLeastOnce());

        var mockSave = Provider.GetRequiredService<Mock<ISaveService>>();
        mockSave.Verify(service => service.SaveChangesAsync(), Times.Never());
    }

    [Test]
    public void Get_IsValid_WithFilter_ThrowsReturnODataErrorException()
    {
        Guid recordId = RecordId;

        var model = ODataModel.GetEdmModel(1.0m);
        var queryOptions = ODataQueryOptionsHelper.CreateOptions<QueryDTO>(
            model,
            "GET",
            "https://localhost:7200",
            "api/v1",
            "queries",
            $"('{recordId}')",
            "$filter=id eq 'CREATE'");

        Assert.ThrowsAsync<ReturnODataErrorException>(async () =>
        {
            var result = await Controller.Get(recordId, queryOptions);
        });
    }

    //[Test]
    //public async Task Get_IsValid_WithExpand_Returns200Ok()
    //{
    //    Guid recordId = RecordId;

    //    var model = ODataModel.GetEdmModel(1.0m);
    //    var queryOptions = ODataQueryOptionsHelper.CreateOptions<QueryDTO>(
    //        model,
    //        "GET",
    //        "https://localhost:7200",
    //        "api/v1",
    //        "queries",
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

    //    var mockServ = Provider.GetRequiredService<Mock<IQueryService>>();
    //    mockServ.Verify(service => service.GetQueryable(), Times.AtLeastOnce());

    //    var mockSave = Provider.GetRequiredService<Mock<ISaveService>>();
    //    mockSave.Verify(service => service.SaveChangesAsync(), Times.Never());
    //}

    [Test]
    public async Task Get_IsValid_WithSelect_Returns200Ok()
    {
        Guid recordId = RecordId;

        var model = ODataModel.GetEdmModel(1.0m);
        var queryOptions = ODataQueryOptionsHelper.CreateOptions<QueryDTO>(
            model,
            "GET",
            "https://localhost:7200",
            "api/v1",
            "queries",
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

        var mockServ = Provider.GetRequiredService<Mock<IQueryService>>();
        mockServ.Verify(service => service.GetQueryable(), Times.AtLeastOnce());

        var mockSave = Provider.GetRequiredService<Mock<ISaveService>>();
        mockSave.Verify(service => service.SaveChangesAsync(), Times.Never());
    }

    [Test]
    public void Get_IsValid_WithOrderBy_ThrowsReturnODataErrorException()
    {
        Guid recordId = RecordId;

        var model = ODataModel.GetEdmModel(1.0m);
        var queryOptions = ODataQueryOptionsHelper.CreateOptions<QueryDTO>(
            model,
            "GET",
            "https://localhost:7200",
            "api/v1",
            "queries",
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

        var model = ODataModel.GetEdmModel(1.0m);
        var queryOptions = ODataQueryOptionsHelper.CreateOptions<QueryDTO>(
            model,
            "GET",
            "https://localhost:7200",
            "api/v1",
            "queries",
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

        var model = ODataModel.GetEdmModel(1.0m);
        var queryOptions = ODataQueryOptionsHelper.CreateOptions<QueryDTO>(
            model,
            "GET",
            "https://localhost:7200",
            "api/v1",
            "queries",
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

        var model = ODataModel.GetEdmModel(1.0m);
        var queryOptions = ODataQueryOptionsHelper.CreateOptions<QueryDTO>(
            model,
            "GET",
            "https://localhost:7200",
            "api/v1",
            "queries",
            $"('{recordId}')",
            "$count=true");

        Assert.ThrowsAsync<ReturnODataErrorException>(async () =>
        {
            var result = await Controller.Get(recordId, queryOptions);
        });
    }

    [Test]
    public void Create_NullEmailAddress_ThrowsReturnODataException()
    {
        var model = ODataModel.GetEdmModel(1.0m);
        var queryOptions = ODataQueryOptionsHelper.CreateOptions<QueryDTO>(
            model,
            "POST",
            "https://localhost:7200",
            "api/v1",
            "queries",
            "",
            "");

        var record = new QueryDTO()
        {
            Name = "Test",
            SqlText = null,
            RunFrequency = TimeSpan.FromSeconds(5)
        };

        Assert.DoesNotThrowAsync(async () =>
        {
            await Controller.Create(record, queryOptions);
        });
    }

    [Test]
    public async Task Create_IsValid_Returns200Ok()
    {
        var model = ODataModel.GetEdmModel(1.0m);
        var queryOptions = ODataQueryOptionsHelper.CreateOptions<QueryDTO>(
            model,
            "POST",
            "https://localhost:7200",
            "api/v1",
            "queries",
            "",
            "");

        var record = new QueryDTO()
        {
            Name = "Test",
            SqlText = "select 1;",
            RunFrequency = TimeSpan.FromSeconds(5)
        };

        var result = await Controller.Create(record, queryOptions);
        var status = result.GetStatusCode();
        var content = result.GetContent();

        Assert.Multiple(() =>
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(status, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(content, Is.AssignableTo<QueryDTO>());
        });

        var mockServ = Provider.GetRequiredService<Mock<IQueryService>>();
        mockServ.Verify(service => service.GetQueryable(), Times.AtLeastOnce());

        var mockSave = Provider.GetRequiredService<Mock<ISaveService>>();
        mockSave.Verify(service => service.SaveChangesAsync(), Times.Once());
    }

    [Test]
    public void Create_IsValid_WithFilter_ThrowsReturnODataErrorException()
    {
        var model = ODataModel.GetEdmModel(1.0m);
        var queryOptions = ODataQueryOptionsHelper.CreateOptions<QueryDTO>(
            model,
            "GET",
            "https://localhost:7200",
            "api/v1",
            "queries",
            "",
            $"$filter=id eq {RecordId}");

        var record = new QueryDTO()
        {
            Name = "Test",
            SqlText = "select 1;",
            RunFrequency = TimeSpan.FromSeconds(5)
        };

        Assert.ThrowsAsync<ReturnODataErrorException>(async () =>
        {
            var result = await Controller.Create(record, queryOptions);
        });
    }

    //[Test]
    //public async Task Create_IsValid_WithExpand_Returns200Ok()
    //{
    //    var model = ODataModel.GetEdmModel(1.0m);
    //    var queryOptions = ODataQueryOptionsHelper.CreateOptions<QueryDTO>(
    //        model,
    //        "GET",
    //        "https://localhost:7200",
    //        "api/v1",
    //        "queries",
    //        "",
    //        "$expand=...");

    //    var record = new QueryDTO()
    //    {
    //        Name = "Test",
    //        SqlText = "select 1;",
    //        RunFrequency = TimeSpan.FromSeconds(5)
    //    };

    //    var result = await Controller.Create(record, queryOptions);
    //    var status = result.GetStatusCode();
    //    var content = result.GetContent();

    //    Assert.Multiple(() =>
    //    {
    //        Assert.That(result, Is.Not.Null);
    //        Assert.That(status, Is.EqualTo(HttpStatusCode.OK));
    //        Assert.That(content, Is.AssignableTo<ISelectExpandWrapper>());
    //    });

    //    var mockServ = Provider.GetRequiredService<Mock<IQueryService>>();
    //    mockServ.Verify(service => service.GetQueryable(), Times.AtLeastOnce());

    //    var mockSave = Provider.GetRequiredService<Mock<ISaveService>>();
    //    mockSave.Verify(service => service.SaveChangesAsync(), Times.Once());
    //}

    [Test]
    public async Task Create_IsValid_WithSelect_Returns200Ok()
    {
        var model = ODataModel.GetEdmModel(1.0m);
        var queryOptions = ODataQueryOptionsHelper.CreateOptions<QueryDTO>(
            model,
            "GET",
            "https://localhost:7200",
            "api/v1",
            "queries",
            "",
            "$select=id");

        var record = new QueryDTO()
        {
            Name = "Test",
            SqlText = "select 1;",
            RunFrequency = TimeSpan.FromSeconds(5)
        };

        var result = await Controller.Create(record, queryOptions);
        var status = result.GetStatusCode();
        var content = result.GetContent();

        Assert.Multiple(() =>
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(status, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(content, Is.AssignableTo<ISelectExpandWrapper>());
        });

        var mockServ = Provider.GetRequiredService<Mock<IQueryService>>();
        mockServ.Verify(service => service.GetQueryable(), Times.AtLeastOnce());

        var mockSave = Provider.GetRequiredService<Mock<ISaveService>>();
        mockSave.Verify(service => service.SaveChangesAsync(), Times.Once());
    }

    [Test]
    public void Create_IsValid_WithOrderBy_ThrowsReturnODataErrorException()
    {
        var model = ODataModel.GetEdmModel(1.0m);
        var queryOptions = ODataQueryOptionsHelper.CreateOptions<QueryDTO>(
            model,
            "GET",
            "https://localhost:7200",
            "api/v1",
            "queries",
            "",
            "$orderby=id");

        var record = new QueryDTO()
        {
            Name = "Test",
            SqlText = "select 1;",
            RunFrequency = TimeSpan.FromSeconds(5)
        };

        Assert.ThrowsAsync<ReturnODataErrorException>(async () =>
        {
            var result = await Controller.Create(record, queryOptions);
        });
    }

    [Test]
    public void Create_IsValid_WithTop_ThrowsReturnODataErrorException()
    {
        var model = ODataModel.GetEdmModel(1.0m);
        var queryOptions = ODataQueryOptionsHelper.CreateOptions<QueryDTO>(
            model,
            "GET",
            "https://localhost:7200",
            "api/v1",
            "queries",
            "",
            "$top=1");

        var record = new QueryDTO()
        {
            Name = "Test",
            SqlText = "select 1;",
            RunFrequency = TimeSpan.FromSeconds(5)
        };

        Assert.ThrowsAsync<ReturnODataErrorException>(async () =>
        {
            var result = await Controller.Create(record, queryOptions);
        });
    }

    [Test]
    public void Create_IsValid_WithSkip_ThrowsReturnODataErrorException()
    {
        var model = ODataModel.GetEdmModel(1.0m);
        var queryOptions = ODataQueryOptionsHelper.CreateOptions<QueryDTO>(
            model,
            "GET",
            "https://localhost:7200",
            "api/v1",
            "queries",
            "",
            "$skip=1");

        var record = new QueryDTO()
        {
            Name = "Test",
            SqlText = "select 1;",
            RunFrequency = TimeSpan.FromSeconds(5)
        };

        Assert.ThrowsAsync<ReturnODataErrorException>(async () =>
        {
            var result = await Controller.Create(record, queryOptions);
        });
    }

    [Test]
    public void Create_IsValid_WithCount_ThrowsReturnODataErrorException()
    {
        var model = ODataModel.GetEdmModel(1.0m);
        var queryOptions = ODataQueryOptionsHelper.CreateOptions<QueryDTO>(
            model,
            "GET",
            "https://localhost:7200",
            "api/v1",
            "queries",
            "",
            "$count=true");

        var record = new QueryDTO()
        {
            Name = "Test",
            SqlText = "select 1;",
            RunFrequency = TimeSpan.FromSeconds(5)
        };

        Assert.ThrowsAsync<ReturnODataErrorException>(async () =>
        {
            var result = await Controller.Create(record, queryOptions);
        });
    }

    [Test]
    public async Task Delete_IsInvalid_Returns404NotFound()
    {
        Guid recordId = new Guid();

        var model = ODataModel.GetEdmModel(1.0m);
        var queryOptions = ODataQueryOptionsHelper.CreateOptions<QueryDTO>(
            model,
            "DELETE",
            "https://localhost:7200",
            "api/v1",
            "queries",
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

        var mockServ = Provider.GetRequiredService<Mock<IQueryService>>();
        mockServ.Verify(service => service.GetQueryable(), Times.AtLeastOnce());

        var mockSave = Provider.GetRequiredService<Mock<ISaveService>>();
        mockSave.Verify(service => service.SaveChangesAsync(), Times.Never());
    }

    [Test]
    public async Task Delete_IsValid_Returns204NoContent()
    {
        Guid recordId = RecordId;

        var model = ODataModel.GetEdmModel(1.0m);
        var queryOptions = ODataQueryOptionsHelper.CreateOptions<QueryDTO>(
            model,
            "DELETE",
            "https://localhost:7200",
            "api/v1",
            "queries",
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

        var mockServ = Provider.GetRequiredService<Mock<IQueryService>>();
        mockServ.Verify(service => service.GetQueryable(), Times.AtLeastOnce());

        var mockSave = Provider.GetRequiredService<Mock<ISaveService>>();
        mockSave.Verify(service => service.SaveChangesAsync(), Times.Once());
    }
}
