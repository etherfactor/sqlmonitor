using EtherGizmos.SqlMonitor.Shared.OAuth.Services;
using EtherGizmos.SqlMonitor.Shared.Utilities.Services.Background.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OpenIddict.Abstractions;

namespace EtherGizmos.SqlMonitor.Api.Core.Services.Background;

public class OAuth2Seeder : OneTimeBackgroundService
{
    private readonly ILogger _logger;
    private readonly IServiceProvider _serviceProvider;

    public OAuth2Seeder(
        ILogger<OAuth2Seeder> logger,
        IServiceProvider serviceProvider) : base(logger)
    {
        _logger = logger;
        _serviceProvider = serviceProvider;
    }

    protected override async Task DoWorkAsync(CancellationToken stoppingToken)
    {
        using var scope = _serviceProvider.CreateScope();
        var provider = scope.ServiceProvider;

        var context = provider.GetRequiredService<AuthorizationContext>();

        var applicationManager = provider.GetRequiredService<IOpenIddictApplicationManager>();

        var application = await applicationManager.FindByClientIdAsync("69008023-2be5-4a2e-902d-0e8268947b25");
        if (application is null)
        {
            await applicationManager.CreateAsync(new OpenIddictApplicationDescriptor()
            {
                DisplayName = "Test Agent",
                ClientId = "69008023-2be5-4a2e-902d-0e8268947b25",
                ClientSecret = "SECRET",
                ClientType = OpenIddictConstants.ClientTypes.Confidential,
                ApplicationType = "agent",
                Permissions =
                {
                    OpenIddictConstants.Permissions.Endpoints.Token,
                    OpenIddictConstants.Permissions.GrantTypes.ClientCredentials,
                    OpenIddictConstants.Permissions.GrantTypes.RefreshToken,
                },
            }, stoppingToken);
        }
        else
        {
            await applicationManager.UpdateAsync(application, new OpenIddictApplicationDescriptor()
            {
                DisplayName = "Test Agent",
                ClientId = "69008023-2be5-4a2e-902d-0e8268947b25",
                ClientSecret = "SECRET",
                ClientType = OpenIddictConstants.ClientTypes.Confidential,
                ApplicationType = "agent",
                Permissions =
                {
                    OpenIddictConstants.Permissions.Endpoints.Token,
                    OpenIddictConstants.Permissions.GrantTypes.ClientCredentials,
                    OpenIddictConstants.Permissions.GrantTypes.RefreshToken,
                },
            }, stoppingToken);
        }
    }
}
