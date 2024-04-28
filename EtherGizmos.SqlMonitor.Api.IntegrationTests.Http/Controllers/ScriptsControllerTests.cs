using EtherGizmos.SqlMonitor.Api.IntegrationTests.Extensions;
using System.Net;

namespace EtherGizmos.SqlMonitor.Api.IntegrationTests.Controllers;

internal class ScriptsControllerTests : IntegrationTestBase
{
    private HttpClient _client;

    [SetUp]
    public void SetUp()
    {
        _client = Global.GetClient();
    }

    [Test]
    public async Task Search_Returns200Ok()
    {
        var response = await _client.GetAsync("https://localhost:7200/api/v0.1/scripts");

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
            runFrequency = "PT5S",
            variants = new[]
            {
                new
                {
                    scriptInterpreterId = 1,
                    scriptText = "Write-Host '##metric: value=123 bucket=A'",
                },
                new
                {
                    scriptInterpreterId = 2,
                    scriptText = "Write-Host '##metric: value=123 bucket=A'",
                }
            }
        };

        var response = await _client.PostAsync("https://localhost:7200/api/v0.1/scripts", body.AsJsonContent());

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
            name = "New Test"
        };

        var response = await _client.PatchAsync($"https://localhost:7200/api/v0.1/scripts({recordId})", body.AsJsonContent());

        Assert.Multiple(async () =>
        {
            Assert.That(response, Is.Not.Null);
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(response.Content, Is.Not.Null);

            if (response.StatusCode != HttpStatusCode.OK)
            {
                Console.Out.WriteLine("Returned response:"
                    + Environment.NewLine
                    + await response.Content.ReadAsStringAsync());
            }
        });
    }

    private async Task<Guid> CreateTest()
    {
        var body = new
        {
            name = "Test",
            runFrequency = "PT5S",
        };

        var response = await _client.PostAsync("https://localhost:7200/api/v0.1/scripts", body.AsJsonContent());

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

        var data = await response.Content.ReadFromJsonModelAsync(new { id = default(Guid) });

        Assert.That(data, Is.Not.Null);

        return data.id;
    }
}
