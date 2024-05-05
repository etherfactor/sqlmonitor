using EtherGizmos.Extensions.DependencyInjection;
using EtherGizmos.SqlMonitor.Api;
using EtherGizmos.SqlMonitor.Api.Extensions;
using EtherGizmos.SqlMonitor.Api.Services.Background;
using EtherGizmos.SqlMonitor.Api.Services.Caching;
using EtherGizmos.SqlMonitor.Api.Services.Caching.Abstractions;
using EtherGizmos.SqlMonitor.Api.Services.Caching.Configuration;
using EtherGizmos.SqlMonitor.Api.Services.Configuration;
using EtherGizmos.SqlMonitor.Api.Services.Data;
using EtherGizmos.SqlMonitor.Api.Services.Data.Abstractions;
using EtherGizmos.SqlMonitor.Api.Services.Data.Configuration;
using EtherGizmos.SqlMonitor.Api.Services.Filters;
using EtherGizmos.SqlMonitor.Api.Services.Messaging.Configuration;
using EtherGizmos.SqlMonitor.Api.Services.Validation;
using EtherGizmos.SqlMonitor.Database;
using EtherGizmos.SqlMonitor.Models;
using FluentMigrator.Runner;
using MassTransit;
using Microsoft.AspNetCore.OData;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Serilog;
using StackExchange.Redis;
using Swashbuckle.AspNetCore.SwaggerGen;

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
    .AddOptions<MySqlOptions>()
    .Configure<IConfiguration, IOptions<UsageOptions>>((opt, conf, usage) =>
    {
        var path = "Connections:MySql";

        var section = conf.GetSection(path);

        section.Bind(opt);
        opt.AllProperties = section.GetChildren()
            .Where(e => !typeof(MySqlOptions).GetProperties().Any(p => p.Name == e.Key))
            .ToDictionary(e => e.Key, e => e.Value);

        if (usage.Value.Database == DatabaseType.MySql)
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
    })
    .AddMvcOptions(opt =>
    {
        opt.ModelMetadataDetailsProviders.Add(new AttributeDisplayMetadataProvider());
    });

builder.Services
    .AddApiVersioning(opt =>
    {
        opt.ReportApiVersions = true;
        opt.DefaultApiVersion = ApiVersions.V0_1;
        //opt.ApiVersionReader = new UrlSegmentApiVersionReader();
    })
    .AddOData(opt =>
    {
        opt.AddRouteComponents("api/v{version:apiVersion}");
    })
    .AddODataApiExplorer(opt =>
    {
        opt.GroupNameFormat = "'v'VVV";
        opt.SubstituteApiVersionInUrl = true;
    });

builder.Services.AddTransient<IConfigureOptions<SwaggerGenOptions>, ConfigureSwaggerOptions>();
builder.Services
    .AddSwaggerGen(opt =>
    {
        //Add a custom operation filter which sets default values
        opt.OperationFilter<SwaggerDefaultValues>();
    });

builder.Services
    .AddDbContext<DatabaseContext>((services, opt) =>
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

            opt.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString), conf =>
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

builder.Services
    .AddChildContainer((childServices, parentServices) =>
    {
        var usageOptions = parentServices
            .GetRequiredService<IOptions<UsageOptions>>()
            .Value;

        if (usageOptions.Database == DatabaseType.MySql)
        {
            childServices.AddTransient<IDatabaseConnectionProvider, MySqlDatabaseConnectionProvider>();
        }
        else if (usageOptions.Database == DatabaseType.SqlServer)
        {
            childServices.AddTransient<IDatabaseConnectionProvider, SqlServerDatabaseConnectionProvider>();
        }
    })
    .ImportSingleton<IOptions<MySqlOptions>>()
    .ImportSingleton<IOptions<SqlServerOptions>>()
    .ForwardTransient<IDatabaseConnectionProvider>();

builder.Services
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
                    opt.AddMySql8();
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
    .ImportLogging()
    .ForwardCaching();

builder.Services
    .AddChildContainer((childServices, parentServices) =>
    {
        var usageOptions = parentServices
            .GetRequiredService<IOptions<UsageOptions>>()
            .Value;

        childServices.AddMassTransit(opt =>
        {
            opt.AddConsumersFromNamespaceContaining<RootOfNamespace>();

            if (usageOptions.MessageBroker == MessageBrokerType.InMemory)
            {
                opt.UsingInMemory((context, conf) =>
                {
                    conf.Host();

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
                        var usePort = host.Port != 0 ? host.Port : Constants.RabbitMQ.Port;

                        useHost = $"{useHost}:{usePort}";
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
                                    var usePort = node.Port != 0 ? node.Port : Constants.RabbitMQ.Port;

                                    conf.Node(useNode);
                                }
                            });
                        }
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
    .ImportSingleton<IDistributedRecordCache>()
    .ImportScoped<ISaveService>()
    .ImportLogging()
    .ForwardMassTransit();

builder.Services
    .AddChildContainer((childCollection, parentServices) =>
    {
        var redisOptions = parentServices
            .GetRequiredService<IOptions<RedisOptions>>()
            .Value;

        childCollection.AddSingleton<IConnectionMultiplexer>(_ =>
        {
            var internalValue = redisOptions.ToStackExchangeRedisOptions();

            RedisConnectionMultiplexer.Initialize(internalValue);
            return RedisConnectionMultiplexer.Instance;
        });
    })
    .ImportSingleton<IOptions<ConfigurationOptions>>()
    .ForwardSingleton<IConnectionMultiplexer>();

builder.Services.AddMapper();

builder.Services.AddHostedService<CacheLoadService>();
//builder.Services.AddHostedService<EnqueueMonitorQueriesService>();

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
    app
        .UseSwaggerUI(opt =>
        {
            var descriptions = app.DescribeApiVersions();

            // build a swagger endpoint for each discovered API version
            foreach (var description in descriptions)
            {
                var url = $"/swagger/{description.GroupName}/swagger.json";
                var name = description.GroupName.ToUpperInvariant();
                opt.SwaggerEndpoint(url, name);
            }
        });
    app.UseODataRouteDebug();
}

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

//**********************************************************
// Run Application

app.Run();
