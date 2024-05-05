using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;

namespace EtherGizmos.SqlMonitor.Shared.IntegrationTests;

public abstract class ServerSetupBase
{
    private WebApplicationFactory<Program>? Factory { get; set; }

    protected abstract IDictionary<string, string?> GetConfigurationValues();

    protected HttpClient InternalGetClient()
    {
        if (Factory is null)
            throw new InvalidOperationException("Web application factory is not yet initialized.");

        return Factory.CreateDefaultClient();
    }

    [OneTimeSetUp]
    public void InternalOneTimeSetUp()
    {
        string projectDirectory = Directory.GetCurrentDirectory();

        Factory = new WebApplicationFactory<Program>().WithWebHostBuilder(builder =>
        {
            builder.ConfigureAppConfiguration((context, config) =>
            {
                //Override the default settings for this particular database
                config.AddInMemoryCollection(GetConfigurationValues());
            });
        });
    }

    [OneTimeTearDown]
    public void InternalOneTimeTearDown()
    {
        Factory?.Dispose();
    }
}
