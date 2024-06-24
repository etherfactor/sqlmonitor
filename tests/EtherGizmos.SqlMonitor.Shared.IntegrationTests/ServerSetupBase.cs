using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using ApiProgram = EtherGizmos.SqlMonitor.Api.Program;

namespace EtherGizmos.SqlMonitor.Shared.IntegrationTests;

/// <summary>
/// Assists in setting up a <see cref="WebApplicationFactory{TEntryPoint}"/> for testing.
/// </summary>
public abstract class ServerSetupBase
{
    /// <summary>
    /// The <see cref="WebApplicationFactory{TEntryPoint}"/>.
    /// </summary>
    private WebApplicationFactory<ApiProgram>? Factory { get; set; }

    /// <summary>
    /// Defines the override configuration values used in the integration test.
    /// </summary>
    /// <returns></returns>
    protected abstract IDictionary<string, string?> GetConfigurationValues();

    /// <summary>
    /// Creates an <see cref="HttpClient"/>, non-statically. Inheriting classes should expose this statically.
    /// </summary>
    /// <returns>The client.</returns>
    /// <exception cref="InvalidOperationException"></exception>
    protected HttpClient InternalGetClient()
    {
        if (Factory is null)
            throw new InvalidOperationException("Web application factory is not yet initialized.");

        return Factory.CreateDefaultClient();
    }

    /// <summary>
    /// Starts the <see cref="WebApplicationFactory{TEntryPoint}"/>.
    /// </summary>
    [OneTimeSetUp]
    public void InternalOneTimeSetUp()
    {
        string projectDirectory = Directory.GetCurrentDirectory();

        Factory = new WebApplicationFactory<ApiProgram>().WithWebHostBuilder(builder =>
        {
            builder.ConfigureAppConfiguration((context, config) =>
            {
                //Override the default settings for this particular database
                config.AddInMemoryCollection(GetConfigurationValues());
            });
        });
    }

    /// <summary>
    /// Disposes the <see cref="WebApplicationFactory{TEntryPoint}"/>.
    /// </summary>
    [OneTimeTearDown]
    public void InternalOneTimeTearDown()
    {
        Factory?.Dispose();
    }
}
