using EtherGizmos.Extensions.DependencyInjection;
using EtherGizmos.SqlMonitor.Shared.Configuration;
using EtherGizmos.SqlMonitor.Shared.Configuration.Data;
using EtherGizmos.SqlMonitor.Shared.Database.Remaps;
using EtherGizmos.SqlMonitor.Shared.Database.Services;
using EtherGizmos.SqlMonitor.Shared.Database.Services.Abstractions;
using FluentMigrator.Runner;
using FluentMigrator.Runner.Generators.MySql;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace EtherGizmos.SqlMonitor.Shared.Database;

public static class IServiceCollectionExtensions
{
    public static IServiceCollection AddDatabaseServices(this IServiceCollection @this)
    {
        //Add Entity Framework Core database context
        @this
            .AddDbContext<ApplicationContext>((services, opt) =>
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
                    opt.UseLoggerFactory(loggerFactory);

                    opt.UseMySQL(connectionString, conf =>
                    {
                        conf.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery);
                    });

                    opt.EnableSensitiveDataLogging();
                }
                else if (usageOptions.Database == DatabaseType.PostgreSql)
                {
                    opt.UseLoggerFactory(loggerFactory);

                    opt.UseNpgsql(connectionString, conf =>
                    {
                        conf.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery);
                    });

                    opt.EnableSensitiveDataLogging();
                }
                else if (usageOptions.Database == DatabaseType.SqlServer)
                {
                    opt.UseLoggerFactory(loggerFactory);

                    opt.UseSqlServer(connectionString, conf =>
                    {
                        conf.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery);
                    });

                    opt.EnableSensitiveDataLogging();
                }
                else
                {
                    throw new InvalidOperationException(string.Format("Unknown database type: {0}", usageOptions.Database));
                }

                opt.UseLazyLoadingProxies(true);
            })
            .AddScoped<ISaveService, SaveService>()
            .AddScoped<IMetricService, MetricService>()
            .AddScoped<IMonitoredEnvironmentService, MonitoredEnvironmentService>()
            .AddScoped<IMonitoredResourceService, MonitoredResourceService>()
            .AddScoped<IMonitoredScriptTargetService, MonitoredScriptTargetService>()
            .AddScoped<IMonitoredSystemService, MonitoredSystemService>()
            .AddScoped<IQueryService, QueryService>()
            .AddScoped<IScriptService, ScriptService>()
            .AddScoped<IScriptInterpreterService, ScriptInterpreterService>();

        //Add FluentMigrator
        @this
            .AddChildContainer((childServices, parentServices) =>
            {
                var usageOptions = parentServices
                    .GetRequiredService<IOptions<UsageOptions>>()
                    .Value;

                var connectionProvider = parentServices.GetRequiredService<IDatabaseConnectionProvider>();

                childServices
                    .AddLogging(opt => opt.AddFluentMigratorConsole())
                    .AddFluentMigratorCore()
                    .ConfigureRunner(opt =>
                    {
                        opt.WithGlobalConnectionString(connectionProvider.GetConnectionString())
                            .ScanIn(typeof(DatabaseMigrationTarget).Assembly).For.Migrations()
                            .WithVersionTable(new CustomVersionTableMetadata());

                        if (usageOptions.Database == DatabaseType.MySql)
                        {
                            opt.AddMySql8()
                                .Services.AddScoped<MySqlQuoter, MySqlQuoterRemap>();
                        }
                        else if (usageOptions.Database == DatabaseType.PostgreSql)
                        {
                            opt.AddPostgres();
                        }
                        else if (usageOptions.Database == DatabaseType.SqlServer)
                        {
                            opt.AddSqlServer();
                        }
                        else
                        {
                            throw new InvalidOperationException(string.Format("Unknown database type: {0}", usageOptions.Database));
                        }
                    });

                childServices.AddSingleton<IMigrationManager, MigrationManager>();
            })
            .ImportLogging()
            .ForwardSingleton<IMigrationManager>();

        return @this;
    }
}
