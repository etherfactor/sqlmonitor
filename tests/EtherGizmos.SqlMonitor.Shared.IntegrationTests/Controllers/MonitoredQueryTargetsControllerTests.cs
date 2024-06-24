using EtherGizmos.SqlMonitor.Shared.IntegrationTests.Extensions;
using EtherGizmos.SqlMonitor.Shared.Models.Api.v1;
using System.Net;

namespace EtherGizmos.SqlMonitor.Shared.IntegrationTests.Controllers;

public abstract class MonitoredQueryTargetsControllerTests : IntegrationTestBase
{
    private HttpClient _client = null!;

    protected abstract HttpClient GetClient();

    [SetUp]
    public void SetUp()
    {
        _client = GetClient();
    }

    [TearDown]
    public void TearDown()
    {
        _client?.Dispose();
    }

    [Test]
    public async Task Search_Returns200Ok()
    {
        var response = await _client.GetAsync("https://localhost:7200/api/v0.1/monitoredQueryTargets");

        Assert.Multiple(() =>
        {
            Assert.That(response, Is.Not.Null);
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(response.Content, Is.Not.Null);
        });
    }

    [Test]
    public async Task Create_Returns201Created()
    {
        var monitoredSystemResult = await _client.PostAsync("https://localhost:7200/api/v0.1/monitoredSystems", new { name = "Test" }.AsJsonContent());
        var monitoredSystemId = (await monitoredSystemResult.Content.ReadFromJsonModelAsync<MonitoredSystemDTO>())!.Id;

        var monitoredResourceResult = await _client.PostAsync("https://localhost:7200/api/v0.1/monitoredResources", new { name = "Test" }.AsJsonContent());
        var monitoredResourceId = (await monitoredResourceResult.Content.ReadFromJsonModelAsync<MonitoredResourceDTO>())!.Id;

        var monitoredEnvironmentResult = await _client.PostAsync("https://localhost:7200/api/v0.1/monitoredEnvironments", new { name = "Test" }.AsJsonContent());
        var monitoredEnvironmentId = (await monitoredEnvironmentResult.Content.ReadFromJsonModelAsync<MonitoredEnvironmentDTO>())!.Id;

        var body = new
        {
            monitoredSystemId,
            monitoredResourceId,
            monitoredEnvironmentId,
            sqlType = "MySql",
            hostName = "localhost",
            connectionString = "Server=localhost; Database=app_db; Uid=username; Pwd=password;",
        };

        var response = await _client.PostAsync("https://localhost:7200/api/v0.1/monitoredQueryTargets", body.AsJsonContent());

        Assert.Multiple(async () =>
        {
            Assert.That(response, Is.Not.Null);
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Created));
            Assert.That(response.Content, Is.Not.Null);

            if (response.StatusCode != HttpStatusCode.Created)
            {
                Console.Out.WriteLine("Returned response:"
                    + Environment.NewLine
                    + await response.Content.ReadAsStringAsync());
            }
        });
    }

    [Test]
    public async Task Update_Returns200Ok()
    {
        var recordId = await CreateTest();

        var body = new
        {
            hostName = "not-localhost"
        };

        var response = await _client.PatchAsync($"https://localhost:7200/api/v0.1/monitoredQueryTargets({recordId})", body.AsJsonContent());

        Assert.Multiple(async () =>
        {
            Assert.That(response, Is.Not.Null);
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(response.Content, Is.Not.Null);

            if (response.StatusCode != HttpStatusCode.OK)
            {
                var content = await response.Content.ReadAsStringAsync();
                Console.Out.WriteLine("Returned response:"
                    + Environment.NewLine
                    + content);
            }
        });
    }

    private async Task<int> CreateTest()
    {
        var monitoredSystemResult = await _client.PostAsync("https://localhost:7200/api/v0.1/monitoredSystems", new { name = "Test" }.AsJsonContent());
        var monitoredSystemId = (await monitoredSystemResult.Content.ReadFromJsonModelAsync<MonitoredSystemDTO>())!.Id;

        var monitoredResourceResult = await _client.PostAsync("https://localhost:7200/api/v0.1/monitoredResources", new { name = "Test" }.AsJsonContent());
        var monitoredResourceId = (await monitoredResourceResult.Content.ReadFromJsonModelAsync<MonitoredResourceDTO>())!.Id;

        var monitoredEnvironmentResult = await _client.PostAsync("https://localhost:7200/api/v0.1/monitoredEnvironments", new { name = "Test" }.AsJsonContent());
        var monitoredEnvironmentId = (await monitoredEnvironmentResult.Content.ReadFromJsonModelAsync<MonitoredEnvironmentDTO>())!.Id;

        var body = new
        {
            monitoredSystemId,
            monitoredResourceId,
            monitoredEnvironmentId,
            sqlType = "MySql",
            hostName = "localhost",
            connectionString = "Server=localhost; Database=app_db; Uid=username; Pwd=password;",
        };

        var response = await _client.PostAsync("https://localhost:7200/api/v0.1/monitoredQueryTargets", body.AsJsonContent());

        Assert.Multiple(async () =>
        {
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Created));
            Assert.That(response.Content, Is.Not.Null);

            if (response.StatusCode != HttpStatusCode.Created)
            {
                Console.Out.WriteLine("Returned response:"
                    + Environment.NewLine
                    + await response.Content.ReadAsStringAsync());
            }
        });

        var data = await response.Content.ReadFromJsonModelAsync(new { id = default(int) });

        Assert.That(data, Is.Not.Null);

        return data!.id;
    }
}
