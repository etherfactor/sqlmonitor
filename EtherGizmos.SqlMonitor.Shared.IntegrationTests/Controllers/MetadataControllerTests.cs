using System.Net;

namespace EtherGizmos.SqlMonitor.Shared.IntegrationTests.Controllers;

public abstract class MetadataControllerTests : IntegrationTestBase
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
    public async Task GetMetadata_Returns200Ok()
    {
        var response = await _client.GetAsync("https://localhost:7200/api/v0.1/$metadata");

        Assert.Multiple(() =>
        {
            Assert.That(response, Is.Not.Null);
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(response.Content, Is.Not.Null);
        });
    }
}
