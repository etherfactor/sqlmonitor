using EtherGizmos.SqlMonitor.Api.IntegrationTests.Extensions;
using System.Net;

namespace EtherGizmos.SqlMonitor.Api.IntegrationTests.Controllers;

internal class UsersControllerTests : IntegrationTestBase
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
        var response = await Client.GetAsync("https://localhost:7200/api/v1/users");

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
            username = "test123",
            password = "password",
            name = "User Name"
        };

        var response = await Client.PostAsync("https://localhost:7200/api/v1/users", body.AsJsonContent());

        Assert.Multiple(async () =>
        {
            Assert.That(response, Is.Not.Null);
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Created));
            var contentRead = await response.Content.ReadAsStringAsync();
            Assert.That(response.Content, Is.Not.Null);
        });
    }
}
