using Microsoft.Extensions.Hosting;
using Serilog;

namespace EtherGizmos.SqlMonitor.Shared.Utilities;

public static class IHostApplicationBuilderExtensions
{
    public static IHostApplicationBuilder AddLoggingServices(this IHostApplicationBuilder @this)
    {
        Log.Logger = new LoggerConfiguration()
            .ReadFrom.Configuration(@this.Configuration)
            .CreateLogger();

        @this.Services.AddSerilog(Log.Logger);

        return @this;
    }
}
