using Microsoft.Extensions.DependencyInjection;

namespace EtherGizmos.SqlMonitor.Shared.Redis.Caching.Abstractions;

public interface ICachingConfigurator
{
    public IServiceCollection Services { get; }
}
