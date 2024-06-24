using EtherGizmos.SqlMonitor.Shared.Redis.Caching.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace EtherGizmos.SqlMonitor.Shared.Redis.Caching;

internal class CachingConfigurator : ICachingConfigurator
{
    private readonly IServiceCollection _serviceCollection;

    public IServiceCollection Services => _serviceCollection;

    public CachingConfigurator(
        IServiceCollection serviceCollection)
    {
        _serviceCollection = serviceCollection;
    }
}
