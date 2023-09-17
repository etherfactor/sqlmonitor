using EtherGizmos.SqlMonitor.Api.Controllers;
using EtherGizmos.SqlMonitor.Api.OData.Metadata;
using EtherGizmos.SqlMonitor.Api.Services.Data.Abstractions;
using EtherGizmos.SqlMonitor.Api.UnitTests.Extensions;
using EtherGizmos.SqlMonitor.Models.Api.v1;
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

internal class UsersControllerTests
{
    private IServiceProvider Provider { get; set; }

    private UsersController Controller { get; set; }

    private List<User> Data { get; set; }

    private Guid RecordId { get; } = new Guid("3447e2d1-0a19-4272-94d2-4911bd5d7b43");

    [SetUp]
    public void SetUp()
    {
        Provider = Global.CreateScope();
        Controller = Provider.GetRequiredService<UsersController>();
        Data = new List<User>()
        {
            new User()
            {
                Id = RecordId,
                Username = "admin",
                PasswordHash = "$2a$12$tp9SfLVzKXxfr39Xy6nT7OZs1U5VLvsf84MN947mEHAVFGFTi6X.u",
                Name = "Administrator",
                EmailAddress = "test@domain.com",
                IsActive = true,
                IsAdministrator = true,
                Principal = new Principal()
                {
                    Type = PrincipalType.User
                }
            },
            new User()
            {
                Id = Guid.NewGuid(),
                Username = "test123",
                PasswordHash = "$2a$12$tp9SfLVzKXxfr39Xy6nT7OZs1U5VLvsf84MN947mEHAVFGFTi6X.u",
                Name = "Test User",
                IsActive = true,
                IsAdministrator = false,
                Principal = new Principal()
                {
                    Type = PrincipalType.User
                }
            },
            new User()
            {
                Id = Guid.NewGuid(),
                Username = "test456",
                PasswordHash = "$2a$12$tp9SfLVzKXxfr39Xy6nT7OZs1U5VLvsf84MN947mEHAVFGFTi6X.u",
                Name = "Test User",
                IsActive = true,
                IsAdministrator = false,
                Principal = new Principal()
                {
                    Type = PrincipalType.User
                }
            },
            new User()
            {
                Id = Guid.NewGuid(),
                Username = "test789",
                PasswordHash = "$2a$12$tp9SfLVzKXxfr39Xy6nT7OZs1U5VLvsf84MN947mEHAVFGFTi6X.u",
                Name = "Test User",
                IsActive = false,
                IsAdministrator = false,
                Principal = new Principal()
                {
                    Type = PrincipalType.User
                }
            }
        };

        var mockData = Data.AsQueryable().BuildMock();

        var mockServ = Provider.GetRequiredService<Mock<IUserService>>();
        mockServ.Setup(service => service.GetQueryable()).Returns(mockData);

        var mockSave = Provider.GetRequiredService<Mock<ISaveService>>();
        mockSave.Setup(service => service.SaveChangesAsync()).Returns(Task.CompletedTask);
    }

    [Test]
    public async Task Search_IsValid_Returns200Ok()
    {
        var model = ODataModel.GetEdmModel(1.0m);
        var queryOptions = ODataQueryOptionsHelper.CreateOptions<UserDTO>(
            model,
            "GET",
            "https://localhost:7200",
            "api/v1",
            "users",
            "",
            "");

        var result = await Controller.Search(queryOptions);
        var status = result.GetStatusCode();
        var content = result.GetContent();

        Assert.Multiple(() =>
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(status, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(content, Is.AssignableTo<IEnumerable<UserDTO>>());
        });

        var mockServ = Provider.GetRequiredService<Mock<IUserService>>();
        mockServ.Verify(service => service.GetQueryable(), Times.AtLeastOnce());

        var mockSave = Provider.GetRequiredService<Mock<ISaveService>>();
        mockSave.Verify(service => service.SaveChangesAsync(), Times.Never());
    }

    [Test]
    public async Task Search_IsValid_WithFilter_Returns200Ok()
    {
        var model = ODataModel.GetEdmModel(1.0m);
        var queryOptions = ODataQueryOptionsHelper.CreateOptions<UserDTO>(
            model,
            "GET",
            "https://localhost:7200",
            "api/v1",
            "users",
            "",
            "$filter=id eq 3447e2d1-0a19-4272-94d2-4911bd5d7b43");

        var result = await Controller.Search(queryOptions);
        var status = result.GetStatusCode();
        var content = result.GetContent();

        Assert.Multiple(() =>
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(status, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(content, Is.AssignableTo<IEnumerable<UserDTO>>());
        });

        var mockServ = Provider.GetRequiredService<Mock<IUserService>>();
        mockServ.Verify(service => service.GetQueryable(), Times.AtLeastOnce());

        var mockSave = Provider.GetRequiredService<Mock<ISaveService>>();
        mockSave.Verify(service => service.SaveChangesAsync(), Times.Never());
    }

    //[Test]
    //public async Task Search_IsValid_WithExpand_Returns200Ok()
    //{
    //    var model = ODataModel.GetEdmModel(1.0m);
    //    var queryOptions = ODataQueryOptionsHelper.CreateOptions<UserDTO>(
    //        model,
    //        "GET",
    //        "https://localhost:7200",
    //        "api/v1",
    //        "users",
    //        "",
    //        "$expand=groups");

    //    var result = await Controller.Search(queryOptions);
    //    var status = result.GetStatusCode();
    //    var content = result.GetContent();

    //    Assert.Multiple(() =>
    //    {
    //        Assert.That(result, Is.Not.Null);
    //        Assert.That(status, Is.EqualTo(HttpStatusCode.OK));
    //        Assert.That(content, Is.AssignableTo<IEnumerable<ISelectExpandWrapper>>());
    //    });

    //    var mockServ = Provider.GetRequiredService<Mock<IUserService>>();
    //    mockServ.Verify(service => service.GetQueryable(), Times.AtLeastOnce());

    //    var mockSave = Provider.GetRequiredService<Mock<ISaveService>>();
    //    mockSave.Verify(service => service.SaveChangesAsync(), Times.Never());
    //}

    [Test]
    public async Task Search_IsValid_WithSelect_Returns200Ok()
    {
        var model = ODataModel.GetEdmModel(1.0m);
        var queryOptions = ODataQueryOptionsHelper.CreateOptions<UserDTO>(
            model,
            "GET",
            "https://localhost:7200",
            "api/v1",
            "users",
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

        var mockServ = Provider.GetRequiredService<Mock<IUserService>>();
        mockServ.Verify(service => service.GetQueryable(), Times.AtLeastOnce());

        var mockSave = Provider.GetRequiredService<Mock<ISaveService>>();
        mockSave.Verify(service => service.SaveChangesAsync(), Times.Never());
    }

    [Test]
    public async Task Search_IsValid_WithOrderBy_Returns200Ok()
    {
        var model = ODataModel.GetEdmModel(1.0m);
        var queryOptions = ODataQueryOptionsHelper.CreateOptions<UserDTO>(
            model,
            "GET",
            "https://localhost:7200",
            "api/v1",
            "users",
            "",
            "$orderby=name");

        var result = await Controller.Search(queryOptions);
        var status = result.GetStatusCode();
        var content = result.GetContent();

        Assert.Multiple(() =>
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(status, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(content, Is.AssignableTo<IEnumerable<UserDTO>>());
        });

        var mockServ = Provider.GetRequiredService<Mock<IUserService>>();
        mockServ.Verify(service => service.GetQueryable(), Times.AtLeastOnce());

        var mockSave = Provider.GetRequiredService<Mock<ISaveService>>();
        mockSave.Verify(service => service.SaveChangesAsync(), Times.Never());
    }

    [Test]
    public async Task Search_IsValid_WithTop_Returns200Ok()
    {
        var model = ODataModel.GetEdmModel(1.0m);
        var queryOptions = ODataQueryOptionsHelper.CreateOptions<UserDTO>(
            model,
            "GET",
            "https://localhost:7200",
            "api/v1",
            "users",
            "",
            "$top=1");

        var result = await Controller.Search(queryOptions);
        var status = result.GetStatusCode();
        var content = result.GetContent();

        Assert.Multiple(() =>
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(status, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(content, Is.AssignableTo<IEnumerable<UserDTO>>());
        });

        var mockServ = Provider.GetRequiredService<Mock<IUserService>>();
        mockServ.Verify(service => service.GetQueryable(), Times.AtLeastOnce());

        var mockSave = Provider.GetRequiredService<Mock<ISaveService>>();
        mockSave.Verify(service => service.SaveChangesAsync(), Times.Never());
    }

    [Test]
    public async Task Search_IsValid_WithSkip_Returns200Ok()
    {
        var model = ODataModel.GetEdmModel(1.0m);
        var queryOptions = ODataQueryOptionsHelper.CreateOptions<UserDTO>(
            model,
            "GET",
            "https://localhost:7200",
            "api/v1",
            "users",
            "",
            "$skip=1");

        var result = await Controller.Search(queryOptions);
        var status = result.GetStatusCode();
        var content = result.GetContent();

        Assert.Multiple(() =>
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(status, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(content, Is.AssignableTo<IEnumerable<UserDTO>>());
        });

        var mockServ = Provider.GetRequiredService<Mock<IUserService>>();
        mockServ.Verify(service => service.GetQueryable(), Times.AtLeastOnce());

        var mockSave = Provider.GetRequiredService<Mock<ISaveService>>();
        mockSave.Verify(service => service.SaveChangesAsync(), Times.Never());
    }

    [Test]
    public async Task Search_IsValid_WithCount_Returns200Ok()
    {
        var model = ODataModel.GetEdmModel(1.0m);
        var queryOptions = ODataQueryOptionsHelper.CreateOptions<UserDTO>(
            model,
            "GET",
            "https://localhost:7200",
            "api/v1",
            "users",
            "",
            "$count=true");

        var result = await Controller.Search(queryOptions);
        var status = result.GetStatusCode();
        var content = result.GetContent();

        Assert.Multiple(() =>
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(status, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(content, Is.AssignableTo<IEnumerable<UserDTO>>());
        });

        var mockServ = Provider.GetRequiredService<Mock<IUserService>>();
        mockServ.Verify(service => service.GetQueryable(), Times.AtLeastOnce());

        var mockSave = Provider.GetRequiredService<Mock<ISaveService>>();
        mockSave.Verify(service => service.SaveChangesAsync(), Times.Never());
    }

    [Test]
    public async Task Get_IsInvalid_Returns404NotFound()
    {
        var recordId = Guid.NewGuid();

        var model = ODataModel.GetEdmModel(1.0m);
        var queryOptions = ODataQueryOptionsHelper.CreateOptions<UserDTO>(
            model,
            "GET",
            "https://localhost:7200",
            "api/v1",
            "users",
            $"({recordId})",
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

        var mockServ = Provider.GetRequiredService<Mock<IUserService>>();
        mockServ.Verify(service => service.GetQueryable(), Times.AtLeastOnce());

        var mockSave = Provider.GetRequiredService<Mock<ISaveService>>();
        mockSave.Verify(service => service.SaveChangesAsync(), Times.Never());
    }

    [Test]
    public async Task Get_IsValid_Returns200Ok()
    {
        var recordId = new Guid("3447e2d1-0a19-4272-94d2-4911bd5d7b43");

        var model = ODataModel.GetEdmModel(1.0m);
        var queryOptions = ODataQueryOptionsHelper.CreateOptions<UserDTO>(
            model,
            "GET",
            "https://localhost:7200",
            "api/v1",
            "users",
            $"({recordId})",
            "");

        var result = await Controller.Get(recordId, queryOptions);
        var status = result.GetStatusCode();
        var content = result.GetContent();

        Assert.Multiple(() =>
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(status, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(content, Is.AssignableTo<UserDTO>());
        });

        var mockServ = Provider.GetRequiredService<Mock<IUserService>>();
        mockServ.Verify(service => service.GetQueryable(), Times.AtLeastOnce());

        var mockSave = Provider.GetRequiredService<Mock<ISaveService>>();
        mockSave.Verify(service => service.SaveChangesAsync(), Times.Never());
    }

    [Test]
    public void Get_IsValid_WithFilter_ThrowsReturnODataErrorException()
    {
        var recordId = new Guid("3447e2d1-0a19-4272-94d2-4911bd5d7b43");

        var model = ODataModel.GetEdmModel(1.0m);
        var queryOptions = ODataQueryOptionsHelper.CreateOptions<UserDTO>(
            model,
            "GET",
            "https://localhost:7200",
            "api/v1",
            "users",
            $"({recordId})",
            "$filter=id eq 3447e2d1-0a19-4272-94d2-4911bd5d7b43");

        Assert.ThrowsAsync<ReturnODataErrorException>(async () =>
        {
            var result = await Controller.Get(recordId, queryOptions);
        });
    }

    //[Test]
    //public async Task Get_IsValid_WithExpand_Returns200Ok()
    //{
    //    var recordId = new Guid("3447e2d1-0a19-4272-94d2-4911bd5d7b43");

    //    var model = ODataModel.GetEdmModel(1.0m);
    //    var queryOptions = ODataQueryOptionsHelper.CreateOptions<UserDTO>(
    //        model,
    //        "GET",
    //        "https://localhost:7200",
    //        "api/v1",
    //        "users",
    //        $"({recordId})",
    //        "$expand=groups");

    //    var result = await Controller.Get(recordId, queryOptions);
    //    var status = result.GetStatusCode();
    //    var content = result.GetContent();

    //    Assert.Multiple(() =>
    //    {
    //        Assert.That(result, Is.Not.Null);
    //        Assert.That(status, Is.EqualTo(HttpStatusCode.OK));
    //        Assert.That(content, Is.AssignableTo<ISelectExpandWrapper>());
    //    });

    //    var mockServ = Provider.GetRequiredService<Mock<IUserService>>();
    //    mockServ.Verify(service => service.GetQueryable(), Times.AtLeastOnce());

    //    var mockSave = Provider.GetRequiredService<Mock<ISaveService>>();
    //    mockSave.Verify(service => service.SaveChangesAsync(), Times.Never());
    //}

    [Test]
    public async Task Get_IsValid_WithSelect_Returns200Ok()
    {
        var recordId = new Guid("3447e2d1-0a19-4272-94d2-4911bd5d7b43");

        var model = ODataModel.GetEdmModel(1.0m);
        var queryOptions = ODataQueryOptionsHelper.CreateOptions<UserDTO>(
            model,
            "GET",
            "https://localhost:7200",
            "api/v1",
            "users",
            $"({recordId})",
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

        var mockServ = Provider.GetRequiredService<Mock<IUserService>>();
        mockServ.Verify(service => service.GetQueryable(), Times.AtLeastOnce());

        var mockSave = Provider.GetRequiredService<Mock<ISaveService>>();
        mockSave.Verify(service => service.SaveChangesAsync(), Times.Never());
    }

    [Test]
    public void Get_IsValid_WithOrderBy_ThrowsReturnODataErrorException()
    {
        var recordId = new Guid("3447e2d1-0a19-4272-94d2-4911bd5d7b43");

        var model = ODataModel.GetEdmModel(1.0m);
        var queryOptions = ODataQueryOptionsHelper.CreateOptions<UserDTO>(
            model,
            "GET",
            "https://localhost:7200",
            "api/v1",
            "users",
            $"({recordId})",
            "$orderby=id");

        Assert.ThrowsAsync<ReturnODataErrorException>(async () =>
        {
            var result = await Controller.Get(recordId, queryOptions);
        });
    }

    [Test]
    public void Get_IsValid_WithTop_ThrowsReturnODataErrorException()
    {
        var recordId = new Guid("3447e2d1-0a19-4272-94d2-4911bd5d7b43");

        var model = ODataModel.GetEdmModel(1.0m);
        var queryOptions = ODataQueryOptionsHelper.CreateOptions<UserDTO>(
            model,
            "GET",
            "https://localhost:7200",
            "api/v1",
            "users",
            $"({recordId})",
            "$top=1");

        Assert.ThrowsAsync<ReturnODataErrorException>(async () =>
        {
            var result = await Controller.Get(recordId, queryOptions);
        });
    }

    [Test]
    public void Get_IsValid_WithSkip_ThrowsReturnODataErrorException()
    {
        var recordId = new Guid("3447e2d1-0a19-4272-94d2-4911bd5d7b43");

        var model = ODataModel.GetEdmModel(1.0m);
        var queryOptions = ODataQueryOptionsHelper.CreateOptions<UserDTO>(
            model,
            "GET",
            "https://localhost:7200",
            "api/v1",
            "users",
            $"({recordId})",
            "$skip=1");

        Assert.ThrowsAsync<ReturnODataErrorException>(async () =>
        {
            var result = await Controller.Get(recordId, queryOptions);
        });
    }

    [Test]
    public void Get_IsValid_WithCount_ThrowsReturnODataErrorException()
    {
        var recordId = new Guid("3447e2d1-0a19-4272-94d2-4911bd5d7b43");

        var model = ODataModel.GetEdmModel(1.0m);
        var queryOptions = ODataQueryOptionsHelper.CreateOptions<UserDTO>(
            model,
            "GET",
            "https://localhost:7200",
            "api/v1",
            "users",
            $"({recordId})",
            "$count=true");

        Assert.ThrowsAsync<ReturnODataErrorException>(async () =>
        {
            var result = await Controller.Get(recordId, queryOptions);
        });
    }

    [Test]
    public void Create_DuplicateUsername_ThrowsReturnODataErrorException()
    {
        var model = ODataModel.GetEdmModel(1.0m);
        var queryOptions = ODataQueryOptionsHelper.CreateOptions<UserDTO>(
            model,
            "POST",
            "https://localhost:7200",
            "api/v1",
            "users",
            "",
            "");

        var record = new UserDTO()
        {
            Username = "admin",
            Password = "password",
            Name = "New User",
            EmailAddress = "new@domain.com"
        };

        Assert.ThrowsAsync<ReturnODataErrorException>(async () =>
        {
            await Controller.Create(record, queryOptions);
        });
    }

    [Test]
    public void Create_DuplicateEmailAddress_ThrowsReturnODataErrorException()
    {
        var model = ODataModel.GetEdmModel(1.0m);
        var queryOptions = ODataQueryOptionsHelper.CreateOptions<UserDTO>(
            model,
            "POST",
            "https://localhost:7200",
            "api/v1",
            "users",
            "",
            "");

        var record = new UserDTO()
        {
            Username = "newuser",
            Password = "password",
            Name = "New User",
            EmailAddress = "test@domain.com"
        };

        Assert.ThrowsAsync<ReturnODataErrorException>(async () =>
        {
            await Controller.Create(record, queryOptions);
        });
    }

    [Test]
    public void Create_NullEmailAddress_DoesNotThrow()
    {
        var model = ODataModel.GetEdmModel(1.0m);
        var queryOptions = ODataQueryOptionsHelper.CreateOptions<UserDTO>(
            model,
            "POST",
            "https://localhost:7200",
            "api/v1",
            "users",
            "",
            "");

        var record = new UserDTO()
        {
            Username = "newuser",
            Password = "password",
            Name = "New User",
            EmailAddress = null
        };

        Assert.DoesNotThrowAsync(async () =>
        {
            await Controller.Create(record, queryOptions);
        });
    }

    [Test]
    public async Task Create_IsValid_Returns201Created()
    {
        var model = ODataModel.GetEdmModel(1.0m);
        var queryOptions = ODataQueryOptionsHelper.CreateOptions<UserDTO>(
            model,
            "POST",
            "https://localhost:7200",
            "api/v1",
            "users",
            "",
            "");

        var record = new UserDTO()
        {
            Username = "newuser",
            Password = "password",
            Name = "New User",
            EmailAddress = "new@domain.com"
        };

        var result = await Controller.Create(record, queryOptions);
        var status = result.GetStatusCode();
        var content = result.GetContent();

        Assert.Multiple(() =>
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(status, Is.EqualTo(HttpStatusCode.Created));
            Assert.That(content, Is.AssignableTo<UserDTO>());
        });

        var mockServ = Provider.GetRequiredService<Mock<IUserService>>();
        mockServ.Verify(service => service.GetQueryable(), Times.AtLeastOnce());

        var mockSave = Provider.GetRequiredService<Mock<ISaveService>>();
        mockSave.Verify(service => service.SaveChangesAsync(), Times.Once());
    }

    [Test]
    public void Create_IsValid_WithFilter_ThrowsReturnODataErrorException()
    {
        var model = ODataModel.GetEdmModel(1.0m);
        var queryOptions = ODataQueryOptionsHelper.CreateOptions<UserDTO>(
            model,
            "GET",
            "https://localhost:7200",
            "api/v1",
            "users",
            "",
            "$filter=id eq 3447e2d1-0a19-4272-94d2-4911bd5d7b43");

        var record = new UserDTO()
        {
            Username = "newuser",
            Password = "password",
            Name = "New User",
            EmailAddress = "new@domain.com"
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
    //    var queryOptions = ODataQueryOptionsHelper.CreateOptions<UserDTO>(
    //        model,
    //        "GET",
    //        "https://localhost:7200",
    //        "api/v1",
    //        "users",
    //        "",
    //        "$expand=groups");

    //    var record = new UserDTO()
    //    {
    //        Username = "newuser",
    //        Password = "password",
    //        Name = "New User",
    //        EmailAddress = "new@domain.com"
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

    //    var mockServ = Provider.GetRequiredService<Mock<IUserService>>();
    //    mockServ.Verify(service => service.GetQueryable(), Times.AtLeastOnce());

    //    var mockSave = Provider.GetRequiredService<Mock<ISaveService>>();
    //    mockSave.Verify(service => service.SaveChangesAsync(), Times.Once());
    //}

    [Test]
    public async Task Create_IsValid_WithSelect_Returns201Created()
    {
        var model = ODataModel.GetEdmModel(1.0m);
        var queryOptions = ODataQueryOptionsHelper.CreateOptions<UserDTO>(
            model,
            "GET",
            "https://localhost:7200",
            "api/v1",
            "users",
            "",
            "$select=id");

        var record = new UserDTO()
        {
            Username = "newuser",
            Password = "password",
            Name = "New User",
            EmailAddress = "new@domain.com"
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

        var mockServ = Provider.GetRequiredService<Mock<IUserService>>();
        mockServ.Verify(service => service.GetQueryable(), Times.AtLeastOnce());

        var mockSave = Provider.GetRequiredService<Mock<ISaveService>>();
        mockSave.Verify(service => service.SaveChangesAsync(), Times.Once());
    }

    [Test]
    public void Create_IsValid_WithOrderBy_ThrowsReturnODataErrorException()
    {
        var model = ODataModel.GetEdmModel(1.0m);
        var queryOptions = ODataQueryOptionsHelper.CreateOptions<UserDTO>(
            model,
            "GET",
            "https://localhost:7200",
            "api/v1",
            "users",
            "",
            "$orderby=id");

        var record = new UserDTO()
        {
            Username = "newuser",
            Password = "password",
            Name = "New User",
            EmailAddress = "new@domain.com"
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
        var queryOptions = ODataQueryOptionsHelper.CreateOptions<UserDTO>(
            model,
            "GET",
            "https://localhost:7200",
            "api/v1",
            "users",
            "",
            "$top=1");

        var record = new UserDTO()
        {
            Username = "newuser",
            Password = "password",
            Name = "New User",
            EmailAddress = "new@domain.com"
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
        var queryOptions = ODataQueryOptionsHelper.CreateOptions<UserDTO>(
            model,
            "GET",
            "https://localhost:7200",
            "api/v1",
            "users",
            "",
            "$skip=1");

        var record = new UserDTO()
        {
            Username = "newuser",
            Password = "password",
            Name = "New User",
            EmailAddress = "new@domain.com"
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
        var queryOptions = ODataQueryOptionsHelper.CreateOptions<UserDTO>(
            model,
            "GET",
            "https://localhost:7200",
            "api/v1",
            "users",
            "",
            "$count=true");

        var record = new UserDTO()
        {
            Username = "newuser",
            Password = "password",
            Name = "New User",
            EmailAddress = "new@domain.com"
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

        var model = ODataModel.GetEdmModel(1.0m);
        var queryOptions = ODataQueryOptionsHelper.CreateOptions<UserDTO>(
            model,
            "PATCH",
            "https://localhost:7200",
            "api/v1",
            "users",
            $"({recordId})",
            "");

        var record = new Delta<UserDTO>();
        record.TrySetPropertyValue(nameof(UserDTO.Name), "New Name");

        var result = await Controller.Update(recordId, record, queryOptions);
        var status = result.GetStatusCode();
        var content = result.GetContent();

        Assert.Multiple(() =>
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(status, Is.EqualTo(HttpStatusCode.NotFound));
            Assert.That(content, Is.AssignableTo<ODataError>());
        });

        var mockServ = Provider.GetRequiredService<Mock<IUserService>>();
        mockServ.Verify(service => service.GetQueryable(), Times.AtLeastOnce());

        var mockSave = Provider.GetRequiredService<Mock<ISaveService>>();
        mockSave.Verify(service => service.SaveChangesAsync(), Times.Never());
    }

    [Test]
    public async Task Update_IsValid_Returns200Ok()
    {
        var recordId = RecordId;

        var model = ODataModel.GetEdmModel(1.0m);
        var queryOptions = ODataQueryOptionsHelper.CreateOptions<UserDTO>(
            model,
            "POST",
            "https://localhost:7200",
            "api/v1",
            "users",
            $"({recordId})",
            "");

        var record = new Delta<UserDTO>();
        record.TrySetPropertyValue(nameof(UserDTO.Name), "New Name");

        var result = await Controller.Update(recordId, record, queryOptions);
        var status = result.GetStatusCode();
        var content = result.GetContent();

        Assert.Multiple(() =>
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(status, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(content, Is.AssignableTo<UserDTO>());
        });

        var mockServ = Provider.GetRequiredService<Mock<IUserService>>();
        mockServ.Verify(service => service.GetQueryable(), Times.AtLeastOnce());

        var mockSave = Provider.GetRequiredService<Mock<ISaveService>>();
        mockSave.Verify(service => service.SaveChangesAsync(), Times.Once());
    }

    [Test]
    public void Update_IsValid_WithFilter_ThrowsReturnODataErrorException()
    {
        var recordId = RecordId;

        var model = ODataModel.GetEdmModel(1.0m);
        var queryOptions = ODataQueryOptionsHelper.CreateOptions<UserDTO>(
            model,
            "GET",
            "https://localhost:7200",
            "api/v1",
            "users",
            $"({recordId})",
            $"$filter=id eq {RecordId}");

        var record = new Delta<UserDTO>();
        record.TrySetPropertyValue(nameof(UserDTO.Name), "New Name");

        Assert.ThrowsAsync<ReturnODataErrorException>(async () =>
        {
            var result = await Controller.Update(recordId, record, queryOptions);
        });
    }

    //[Test]
    //public async Task Update_IsValid_WithExpand_Returns200Ok()
    //{
    //    var recordId = RecordId;

    //    var model = ODataModel.GetEdmModel(1.0m);
    //    var queryOptions = ODataQueryOptionsHelper.CreateOptions<UserDTO>(
    //        model,
    //        "GET",
    //        "https://localhost:7200",
    //        "api/v1",
    //        "users",
    //        $"({recordId})",
    //        "$expand=...");

    //    var record = new Delta<UserDTO>();
    //    record.TrySetPropertyValue(nameof(UserDTO.Name), "New Name");

    //    var result = await Controller.Update(recordId, record, queryOptions);
    //    var status = result.GetStatusCode();
    //    var content = result.GetContent();

    //    Assert.Multiple(() =>
    //    {
    //        Assert.That(result, Is.Not.Null);
    //        Assert.That(status, Is.EqualTo(HttpStatusCode.OK));
    //        Assert.That(content, Is.AssignableTo<ISelectExpandWrapper>());
    //    });

    //    var mockServ = Provider.GetRequiredService<Mock<IUserService>>();
    //    mockServ.Verify(service => service.GetQueryable(), Times.AtLeastOnce());

    //    var mockSave = Provider.GetRequiredService<Mock<ISaveService>>();
    //    mockSave.Verify(service => service.SaveChangesAsync(), Times.Once());
    //}

    [Test]
    public async Task Update_IsValid_WithSelect_Returns200Ok()
    {
        var recordId = RecordId;

        var model = ODataModel.GetEdmModel(1.0m);
        var queryOptions = ODataQueryOptionsHelper.CreateOptions<UserDTO>(
            model,
            "GET",
            "https://localhost:7200",
            "api/v1",
            "users",
            $"({recordId})",
            "$select=id");

        var record = new Delta<UserDTO>();
        record.TrySetPropertyValue(nameof(UserDTO.Name), "New Name");

        var result = await Controller.Update(recordId, record, queryOptions);
        var status = result.GetStatusCode();
        var content = result.GetContent();

        Assert.Multiple(() =>
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(status, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(content, Is.AssignableTo<ISelectExpandWrapper>());
        });

        var mockServ = Provider.GetRequiredService<Mock<IUserService>>();
        mockServ.Verify(service => service.GetQueryable(), Times.AtLeastOnce());

        var mockSave = Provider.GetRequiredService<Mock<ISaveService>>();
        mockSave.Verify(service => service.SaveChangesAsync(), Times.Once());
    }

    [Test]
    public void Update_IsValid_WithOrderBy_ThrowsReturnODataErrorException()
    {
        var recordId = RecordId;

        var model = ODataModel.GetEdmModel(1.0m);
        var queryOptions = ODataQueryOptionsHelper.CreateOptions<UserDTO>(
            model,
            "GET",
            "https://localhost:7200",
            "api/v1",
            "users",
            $"({RecordId})",
            "$orderby=id");

        var record = new Delta<UserDTO>();
        record.TrySetPropertyValue(nameof(UserDTO.Name), "New Name");

        Assert.ThrowsAsync<ReturnODataErrorException>(async () =>
        {
            var result = await Controller.Update(recordId, record, queryOptions);
        });
    }

    [Test]
    public void Update_IsValid_WithTop_ThrowsReturnODataErrorException()
    {
        var recordId = RecordId;

        var model = ODataModel.GetEdmModel(1.0m);
        var queryOptions = ODataQueryOptionsHelper.CreateOptions<UserDTO>(
            model,
            "GET",
            "https://localhost:7200",
            "api/v1",
            "users",
            $"({recordId})",
            "$top=1");

        var record = new Delta<UserDTO>();
        record.TrySetPropertyValue(nameof(UserDTO.Name), "New Name");

        Assert.ThrowsAsync<ReturnODataErrorException>(async () =>
        {
            var result = await Controller.Update(recordId, record, queryOptions);
        });
    }

    [Test]
    public void Update_IsValid_WithSkip_ThrowsReturnODataErrorException()
    {
        var recordId = RecordId;

        var model = ODataModel.GetEdmModel(1.0m);
        var queryOptions = ODataQueryOptionsHelper.CreateOptions<UserDTO>(
            model,
            "GET",
            "https://localhost:7200",
            "api/v1",
            "users",
            $"({recordId})",
            "$skip=1");

        var record = new Delta<UserDTO>();
        record.TrySetPropertyValue(nameof(UserDTO.Name), "New Name");

        Assert.ThrowsAsync<ReturnODataErrorException>(async () =>
        {
            var result = await Controller.Update(recordId, record, queryOptions);
        });
    }

    [Test]
    public void Update_IsValid_WithCount_ThrowsReturnODataErrorException()
    {
        var recordId = RecordId;

        var model = ODataModel.GetEdmModel(1.0m);
        var queryOptions = ODataQueryOptionsHelper.CreateOptions<UserDTO>(
            model,
            "GET",
            "https://localhost:7200",
            "api/v1",
            "users",
            $"({recordId})",
            "$count=true");

        var record = new Delta<UserDTO>();
        record.TrySetPropertyValue(nameof(UserDTO.Name), "New Name");

        Assert.ThrowsAsync<ReturnODataErrorException>(async () =>
        {
            var result = await Controller.Update(recordId, record, queryOptions);
        });
    }
}
