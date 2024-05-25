using EtherGizmos.SqlMonitor.Shared.Database;
using EtherGizmos.SqlMonitor.Shared.Database.Extensions;
using EtherGizmos.SqlMonitor.Shared.OAuth.Services;
using Microsoft.Extensions.DependencyInjection;

namespace EtherGizmos.SqlMonitor.Shared.OAuth;

public static class IServiceCollectionExtensions
{
    public static IServiceCollection AddAuthorizationContext(this IServiceCollection @this)
    {
        @this.AddMigrationManager()
            .AddDbContext<AuthorizationContext>((services, opt) =>
            {
                opt.ConfigureForServices(services);
            });

        return @this;
    }
}
