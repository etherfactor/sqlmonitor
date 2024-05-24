using EtherGizmos.SqlMonitor.Api.Services.Caching.Abstractions;

namespace EtherGizmos.SqlMonitor.Api.Services.Caching;

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
