using EtherGizmos.SqlMonitor.Api;
using EtherGizmos.SqlMonitor.Api.Extensions;
using EtherGizmos.SqlMonitor.Api.OData.Metadata;
using EtherGizmos.SqlMonitor.Api.Services.Background;
using EtherGizmos.SqlMonitor.Api.Services.Caching;
using EtherGizmos.SqlMonitor.Api.Services.Caching.Abstractions;
using EtherGizmos.SqlMonitor.Api.Services.Caching.Configuration;
using EtherGizmos.SqlMonitor.Api.Services.Configuration;
using EtherGizmos.SqlMonitor.Api.Services.Data;
using EtherGizmos.SqlMonitor.Api.Services.Data.Abstractions;
using EtherGizmos.SqlMonitor.Api.Services.Data.Configuration;
using EtherGizmos.SqlMonitor.Api.Services.Filters;
using EtherGizmos.SqlMonitor.Api.Services.Messaging;
using EtherGizmos.SqlMonitor.Api.Services.Messaging.Configuration;
using EtherGizmos.SqlMonitor.Api.Services.Validation;
using EtherGizmos.SqlMonitor.Database;
using FluentMigrator.Runner;
using MassTransit;
using Microsoft.AspNetCore.OData;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

//**********************************************************
// Configuration

builder.Configuration.AddJsonFile("appsettings.Local.json", true, true);

Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .CreateLogger();

//**********************************************************
// Add Services

builder.Services
    .AddOptions();

builder.Services
    .AddOptions<UsageOptions>()
    .Configure<IConfiguration>((opt, conf) =>
    {
        var path = "Connections:Use";

        conf.GetSection(path)
            .Bind(opt);

        opt.AssertValid(path);
    });

builder.Services
    .AddOptions<RabbitMQOptions>()
    .Configure<IConfiguration, IOptions<UsageOptions>>((opt, conf, usage) =>
    {
        var path = "Connections:RabbitMQ";

        conf.GetSection(path)
            .Bind(opt);

        if (usage.Value.MessageBroker == MessageBrokerType.RabbitMQ)
        {
            opt.AssertValid(path);
        }
    });

builder.Services
    .AddOptions<RedisOptions>()
    .Configure<IConfiguration, IOptions<UsageOptions>>((opt, conf, usage) =>
    {
        var path = "Connections:Redis";

        conf.GetSection(path)
            .Bind(opt);

        if (usage.Value.Cache == CacheType.Redis)
        {
            opt.AssertValid(path);
        }
    });

builder.Services
    .AddOptions<SqlServerOptions>()
    .Configure<IConfiguration, IOptions<UsageOptions>>((opt, conf, usage) =>
    {
        var path = "Connections:SqlServer";

        var section = conf.GetSection(path);

        section.Bind(opt);
        opt.AllProperties = section.GetChildren()
            .Where(e => !typeof(SqlServerOptions).GetProperties().Any(p => p.Name == e.Key))
            .ToDictionary(e => e.Key, e => e.Value);

        if (usage.Value.Database == DatabaseType.SqlServer)
        {
            opt.AssertValid(path);
        }
    });

builder.Services
    .AddSerilog(Log.Logger);

builder.Services.AddAuthentication();
builder.Services.AddAuthorization();

builder.Services
    .AddControllers(opt =>
    {
        opt.Filters.Add<ReturnODataErrorFilter>();
        opt.Filters.Add<ModelStateFilter>();
    })
    .AddOData(opt =>
    {
        opt.AddRouteComponents("/api/v1", ODataModel.GetEdmModel(1.0m));
    })
    .AddMvcOptions(opt =>
    {
        opt.ModelMetadataDetailsProviders.Add(new AttributeDisplayMetadataProvider());
    });

builder.Services.AddSwaggerGen();

builder.Services
    .AddDbContext<DatabaseContext>((services, opt) =>
    {
        var usageOptions = services
            .GetRequiredService<IOptions<UsageOptions>>()
            .Value;

        if (usageOptions.Database == DatabaseType.SqlServer)
        {
            var connectionProvider = services.GetRequiredService<IDatabaseConnectionProvider>();

            opt.UseSqlServer(connectionProvider.GetConnectionString(), conf =>
            {
                conf.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery);
            });
        }
        else
        {
            throw new InvalidOperationException(string.Format("Unknown database type: {0}", usageOptions.Database));
        }

        opt.UseLazyLoadingProxies(true);
    })
    .AddScoped<ISaveService, SaveService>()
    .AddScoped<IInstanceService, InstanceService>()
    .AddScoped<IInstanceMetricBySecondService, InstanceMetricBySecondService>()
    .AddScoped<IMetricBucketService, MetricBucketService>()
    .AddScoped<IMetricService, MetricService>()
    .AddScoped<IPermissionService, PermissionService>()
    .AddScoped<IQueryService, QueryService>()
    .AddScoped<ISecurableService, SecurableService>()
    .AddScoped<IUserService, UserService>();

builder.Services
    .AddChildContainer((childServices, parentServices) =>
    {
        var usageOptions = parentServices
            .GetRequiredService<IOptions<UsageOptions>>()
            .Value;

        if (usageOptions.Database == DatabaseType.SqlServer)
        {
            childServices.AddTransient<IDatabaseConnectionProvider, SqlServerDatabaseConnectionProvider>(); 
        }
    })
    .ImportSingleton<IOptions<SqlServerOptions>>()
    .ForwardTransient<IDatabaseConnectionProvider>();

builder.Services
    .AddChildContainer((childServices, parentServices) =>
    {
        var usageOptions = parentServices
            .GetRequiredService<IOptions<UsageOptions>>()
            .Value;

        var connectionProvider = parentServices.GetRequiredService<IDatabaseConnectionProvider>();

        childServices.AddFluentMigratorCore()
            .ConfigureRunner(opt =>
            {
                if (usageOptions.Database == DatabaseType.SqlServer)
                {
                    opt.AddSqlServer()
                        .WithGlobalConnectionString(connectionProvider.GetConnectionString())
                        .ScanIn(typeof(DatabaseMigrationTarget).Assembly).For.Migrations()
                        .WithVersionTable(new CustomVersionTableMetadata());
                }
                else
                {
                    throw new InvalidOperationException(string.Format("Unknown database type: {0}", usageOptions.Database));
                }
            });
    })
    .ForwardTransient<IMigrationRunner>();

builder.Services
    .AddChildContainer((childServices, parentServices) =>
    {
        var usageOptions = parentServices
            .GetRequiredService<IOptions<UsageOptions>>()
            .Value;

        childServices.AddCaching(opt =>
        {
            if (usageOptions.Cache == CacheType.InMemory)
            {
                opt.UsingInMemory();
            }
            else if (usageOptions.Cache == CacheType.Redis)
            {
                var redisOptions = parentServices
                    .GetRequiredService<IOptions<RedisOptions>>()
                    .Value;

                opt.UsingRedis(redisOptions);
            }
            else
            {
                throw new InvalidOperationException(string.Format("Unknown cache type: {0}", usageOptions.Cache));
            }
        });

        childServices.AddSingleton<IRedisHelperFactory>(e => RedisHelperFactory.Instance);
    })
    .ForwardCaching();

builder.Services
    .AddChildContainer((childServices, parentServices) =>
    {
        var usageOptions = parentServices
            .GetRequiredService<IOptions<UsageOptions>>()
            .Value;

        childServices.AddMassTransit(opt =>
        {
            if (usageOptions.MessageBroker == MessageBrokerType.InMemory)
            {
                opt.UsingInMemory((context, conf) =>
                {
                    conf.Host();

                    conf.ReceiveEndpoint(RunQueryConsumer.Queue, opt =>
                    {
                        opt.Consumer<RunQueryConsumer>(context);
                    });

                    //TODO: Configure in-memory retry and other options
                });
            }
            else if (usageOptions.MessageBroker == MessageBrokerType.RabbitMQ)
            {
                var rabbitMQOptions = parentServices
                    .GetRequiredService<IOptions<RabbitMQOptions>>()
                    .Value;

                opt.UsingRabbitMq((context, conf) =>
                {
                    string useHost;
                    if (rabbitMQOptions.Hosts.Count == 1)
                    {
                        var host = rabbitMQOptions.Hosts.Single();
                        useHost = host.Address;
                        if (rabbitMQOptions.Hosts.Single().Port != Constants.RabbitMQ.Port)
                            useHost = $"{useHost}:{host.Port}";
                    }
                    else
                    {
                        useHost = "cluster";
                    }

                    conf.Host(useHost, opt =>
                    {
                        opt.Username(rabbitMQOptions.Username);
                        opt.Password(rabbitMQOptions.Password);

                        if (rabbitMQOptions.Hosts.Count > 1)
                        {
                            opt.UseCluster(conf =>
                            {
                                foreach (var node in rabbitMQOptions.Hosts)
                                {
                                    var useNode = node.Address;
                                    if (node.Port != Constants.RabbitMQ.Port)
                                        useNode = $"{useNode}:{node.Port}";

                                    conf.Node(useNode);
                                }
                            });
                        }
                    });

                    conf.ReceiveEndpoint(RunQueryConsumer.Queue, opt =>
                    {
                        opt.Consumer<RunQueryConsumer>(context);
                    });

                    //TODO: Configure retry
                });
            }
            else
            {
                throw new InvalidOperationException(string.Format("Unknown message broker type: {0}", usageOptions.MessageBroker));
            }
        });
    })
    .ForwardMassTransit();

builder.Services.AddMapper();

builder.Services.AddHostedService<CacheLoadService>();
builder.Services.AddHostedService<EnqueueMonitorQueriesService>();

builder.Services.AddCors(opt =>
    {
        opt.AddPolicy("All", builder =>
        {
            builder.AllowAnyOrigin()
                .AllowAnyHeader()
                .AllowAnyMethod();
        });
    });

//**********************************************************
// Add Middleware

var app = builder.Build();

var serviceProvider = app.Services
    .CreateScope()
    .ServiceProvider;

var connectionProvider = serviceProvider.GetRequiredService<IDatabaseConnectionProvider>();

//Perform the database migration
DatabaseMigrationRunner.PerformMigration(connectionProvider);

//Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    //The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.UseCors("All");

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseODataRouteDebug();
}

app.UseAuthentication();
app.UseAuthorization();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
});

//**********************************************************
// Run Application

app.Run();
