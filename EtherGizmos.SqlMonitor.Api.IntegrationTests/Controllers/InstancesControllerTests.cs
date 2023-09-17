using EtherGizmos.SqlMonitor.Api.IntegrationTests.Extensions;
using System.Net;

namespace EtherGizmos.SqlMonitor.Api.IntegrationTests.Controllers;

internal class InstancesControllerTests : IntegrationTestBase
{
    private HttpClient Client { get; set; }

    [SetUp]
    public void SetUp()
    {
        Client = Global.GetClient();
    }

    [Test]
    public async Task Search_Returns200Ok()
    {
        var response = await Client.GetAsync("https://localhost:7200/api/v1/instances");

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
        var body = new
        {
            name = "Test",
            address = "localhost"
        };

        var response = await Client.PostAsync("https://localhost:7200/api/v1/instances", body.AsJsonContent());

        Assert.Multiple(async () =>
        {
            Assert.That(response, Is.Not.Null);
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Created));
            var contentRead = await response.Content.ReadAsStringAsync();
            Assert.That(response.Content, Is.Not.Null);
        });
    }

    [Test]
    public async Task Update_Returns200Ok()
    {
        var recordId = await CreateTest();

        var body = new
        {
            name = "New Test"
        };

        var response = await Client.PatchAsync($"https://localhost:7200/api/v1/instances({recordId})", body.AsJsonContent());

        Assert.Multiple(async () =>
        {
            Assert.That(response, Is.Not.Null);
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            var contentRead = await response.Content.ReadAsStringAsync();
            Assert.That(response.Content, Is.Not.Null);
        });
    }

    private async Task<Guid> CreateTest()
    {
        var body = new
        {
            name = "Test",
            address = "localhost"
        };

        var response = await Client.PostAsync("https://localhost:7200/api/v1/instances", body.AsJsonContent());

        Assert.Multiple(() =>
        {
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Created));
            Assert.That(response.Content, Is.Not.Null);
        });

        var data = await response.Content.ReadFromJsonModelAsync(new { id = default(Guid) });

        Assert.That(data, Is.Not.Null);

        return data.id;
    }
}
