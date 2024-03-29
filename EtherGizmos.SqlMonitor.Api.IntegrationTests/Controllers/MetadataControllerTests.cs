﻿using System.Net;

namespace EtherGizmos.SqlMonitor.Api.IntegrationTests.Controllers;

internal class MetadataControllerTests : IntegrationTestBase
{
    private HttpClient Client { get; set; }

    [SetUp]
    public void SetUp()
    {
        Client = Global.GetClient();
    }

    [Test]
    public async Task GetMetadata_Returns200Ok()
    {
        var response = await Client.GetAsync("https://localhost:7200/api/v0.1/$metadata");

        Assert.Multiple(() =>
        {
            Assert.That(response, Is.Not.Null);
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(response.Content, Is.Not.Null);
        });
    }
}
