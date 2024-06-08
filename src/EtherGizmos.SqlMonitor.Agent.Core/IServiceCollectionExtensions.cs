using EtherGizmos.SqlMonitor.Agent.Core.Services.Queries;
using EtherGizmos.SqlMonitor.Agent.Core.Services.Queries.Abstractions;
using EtherGizmos.SqlMonitor.Agent.Core.Services.Scripts;
using EtherGizmos.SqlMonitor.Agent.Core.Services.Scripts.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace EtherGizmos.SqlMonitor.Agent.Core;

public static class IServiceCollectionExtensions
{
    public static IServiceCollection AddQueryRunnerFactory(this IServiceCollection @this)
    {
        @this.AddSingleton<IQueryRunnerFactory, QueryRunnerFactory>();
        return @this;
    }
    public static IServiceCollection AddScriptRunnerFactory(this IServiceCollection @this)
    {
        @this.AddSingleton<IScriptRunnerFactory, ScriptRunnerFactory>();
        return @this;
    }
}
