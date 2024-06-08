using EtherGizmos.Extensions.DependencyInjection;
using EtherGizmos.SqlMonitor.Shared.Configuration;
using EtherGizmos.SqlMonitor.Shared.Configuration.Data;
using EtherGizmos.SqlMonitor.Shared.Database.Extensions;
using EtherGizmos.SqlMonitor.Shared.Database.Remaps;
using EtherGizmos.SqlMonitor.Shared.Database.Services;
using EtherGizmos.SqlMonitor.Shared.Database.Services.Abstractions;
using FluentMigrator.Runner;
using FluentMigrator.Runner.Generators.MySql;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace EtherGizmos.SqlMonitor.Shared.Database;

public static class IServiceCollectionExtensions
{
    public static IServiceCollection AddDatabaseConnectionProvider(this IServiceCollection @this)
    {
        @this
            .AddChildContainer((childServices, parentServices) =>
            {
                var usageOptions = parentServices
                    .GetRequiredService<IOptions<UsageOptions>>()
                    .Value;

                if (usageOptions.Database == DatabaseType.MySql)
                {
                    childServices.AddTransient<IDatabaseConnectionProvider, MySqlDatabaseConnectionProvider>();
                }
                else if (usageOptions.Database == DatabaseType.PostgreSql)
                {
                    childServices.AddTransient<IDatabaseConnectionProvider, PostgreSqlDatabaseConnectionProvider>();
                }
                else if (usageOptions.Database == DatabaseType.SqlServer)
                {
                    childServices.AddTransient<IDatabaseConnectionProvider, SqlServerDatabaseConnectionProvider>();
                }
            })
            .ImportSingleton<IOptions<MySqlOptions>>()
            .ImportSingleton<IOptions<PostgreSqlOptions>>()
            .ImportSingleton<IOptions<SqlServerOptions>>()
            .ForwardTransient<IDatabaseConnectionProvider>();

        return @this;
    }

    public static IServiceCollection AddDatabaseContext(this IServiceCollection @this)
    {
        @this.AddMigrationManager()
            .AddDbContext<ApplicationContext>((services, opt) =>
            {
                opt.ConfigureForServices(services);
            })
            .AddScoped<ISaveService, SaveService>()
            .AddScoped<IMetricService, MetricService>()
            .AddScoped<IMonitoredEnvironmentService, MonitoredEnvironmentService>()
            .AddScoped<IMonitoredQueryTargetService, MonitoredQueryTargetService>()
            .AddScoped<IMonitoredResourceService, MonitoredResourceService>()
            .AddScoped<IMonitoredScriptTargetService, MonitoredScriptTargetService>()
            .AddScoped<IMonitoredSystemService, MonitoredSystemService>()
            .AddScoped<IQueryService, QueryService>()
            .AddScoped<IScriptService, ScriptService>()
            .AddScoped<IScriptInterpreterService, ScriptInterpreterService>();

        return @this;
    }

    public static IServiceCollection AddMigrationManager(this IServiceCollection @this)
    {
        //Add FluentMigrator if it has not been added already
        if (!@this.Any(e => e.ServiceType == typeof(IMigrationManager)))
        {
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
        }

        return @this;
    }
}
