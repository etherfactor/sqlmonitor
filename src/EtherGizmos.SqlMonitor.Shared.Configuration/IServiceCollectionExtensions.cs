using EtherGizmos.SqlMonitor.Shared.Configuration.Caching;
using EtherGizmos.SqlMonitor.Shared.Configuration.Data;
using EtherGizmos.SqlMonitor.Shared.Configuration.Messaging;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace EtherGizmos.SqlMonitor.Shared.Configuration;

public static class IServiceCollectionExtensions
{
    public static IServiceCollection AddRabbitMQOptions(this IServiceCollection @this, string path = "Connections:RabbitMQ")
    {
        @this.AddOptions()
            .AddOptions<RabbitMQOptions>()
            .Configure<IConfiguration, IOptions<UsageOptions>>((opt, conf, usage) =>
            {
                conf.GetSection(path)
                    .Bind(opt);

                if (usage.Value.MessageBroker == MessageBrokerType.RabbitMQ)
                {
                    opt.AssertValid(path);
                }
            });

        return @this;
    }

    public static IServiceCollection AddRedisOptions(this IServiceCollection @this, string path = "Connections:Redis")
    {
        @this.AddOptions()
            .AddOptions<RedisOptions>()
            .Configure<IConfiguration, IOptions<UsageOptions>>((opt, conf, usage) =>
            {
                conf.GetSection(path)
                    .Bind(opt);

                if (usage.Value.Cache == CacheType.Redis)
                {
                    opt.AssertValid(path);
                }
            });

        return @this;
    }

    public static IServiceCollection AddMySqlOptions(this IServiceCollection @this, string path = "Connections:MySql")
    {
        @this.AddOptions()
            .AddOptions<ConnectionMySqlOptions>()
            .Configure<IConfiguration, IOptions<UsageOptions>>((opt, conf, usage) =>
            {
                var section = conf.GetSection(path);

                section.Bind(opt);
                opt.ConnectionValues = section.GetChildren()
                    .Where(e => !typeof(ConnectionMySqlOptions).GetProperties().Any(p => p.Name == e.Key))
                    .ToDictionary(e => e.Key, e => e.Value);

                if (usage.Value.Database == DatabaseType.MySql)
                {
                    opt.AssertValid(path);
                }
            });

        return @this;
    }

    public static IServiceCollection AddPostgreSqlOptions(this IServiceCollection @this, string path = "Connections:PostgreSql")
    {
        @this.AddOptions()
            .AddOptions<ConnectionPostgreSqlOptions>()
            .Configure<IConfiguration, IOptions<UsageOptions>>((opt, conf, usage) =>
            {
                var section = conf.GetSection(path);

                section.Bind(opt);
                opt.AllProperties = section.GetChildren()
                    .Where(e => !typeof(ConnectionPostgreSqlOptions).GetProperties().Any(p => p.Name == e.Key))
                    .ToDictionary(e => e.Key, e => e.Value);

                if (usage.Value.Database == DatabaseType.PostgreSql)
                {
                    opt.AssertValid(path);
                }
            });

        return @this;
    }

    public static IServiceCollection AddSqlServerOptions(this IServiceCollection @this, string path = "Connections:SqlServer")
    {
        @this.AddOptions()
            .AddOptions<ConnectionSqlServerOptions>()
            .Configure<IConfiguration, IOptions<UsageOptions>>((opt, conf, usage) =>
            {
                var section = conf.GetSection(path);

                section.Bind(opt);
                opt.AllProperties = section.GetChildren()
                    .Where(e => !typeof(ConnectionSqlServerOptions).GetProperties().Any(p => p.Name == e.Key))
                    .ToDictionary(e => e.Key, e => e.Value);

                if (usage.Value.Database == DatabaseType.SqlServer)
                {
                    opt.AssertValid(path);
                }
            });

        return @this;
    }

    public static IServiceCollection AddUsageOptions(this IServiceCollection @this, string path = "Connections:Use")
    {
        @this.AddOptions()
            .AddOptions<UsageOptions>()
            .Configure<IConfiguration>((opt, conf) =>
            {
                conf.GetSection(path)
                    .Bind(opt);

                opt.AssertValid(path);
            });

        return @this;
    }
}
