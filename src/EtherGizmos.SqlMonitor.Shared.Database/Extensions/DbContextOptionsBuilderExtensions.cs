using EtherGizmos.SqlMonitor.Shared.Configuration;
using EtherGizmos.SqlMonitor.Shared.Configuration.Data;
using EtherGizmos.SqlMonitor.Shared.Database.Services.Abstractions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace EtherGizmos.SqlMonitor.Shared.Database.Extensions;

public static class DbContextOptionsBuilderExtensions
{
    public static DbContextOptionsBuilder ConfigureForServices(this DbContextOptionsBuilder @this, IServiceProvider services)
    {
        var usageOptions = services
            .GetRequiredService<IOptions<UsageOptions>>()
            .Value;

        var loggerFactory = services
            .GetRequiredService<ILoggerFactory>();

        var connectionProvider = services.GetRequiredService<IDatabaseConnectionProvider>();
        var connectionString = connectionProvider.GetConnectionString();
        if (usageOptions.Database == DatabaseType.MySql)
        {
            @this.UseLoggerFactory(loggerFactory);

            @this.UseMySQL(connectionString, conf =>
            {
                conf.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery);
            });
        }
        else if (usageOptions.Database == DatabaseType.PostgreSql)
        {
            @this.UseLoggerFactory(loggerFactory);

            @this.UseNpgsql(connectionString, conf =>
            {
                conf.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery);
            });
        }
        else if (usageOptions.Database == DatabaseType.SqlServer)
        {
            @this.UseLoggerFactory(loggerFactory);

            @this.UseSqlServer(connectionString, conf =>
            {
                conf.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery);
            });
        }
        else
        {
            throw new InvalidOperationException(string.Format("Unknown database type: {0}", usageOptions.Database));
        }

        @this.EnableSensitiveDataLogging();
        @this.UseLazyLoadingProxies(true);

        return @this;
    }
}
