using EtherGizmos.SqlMonitor.Api;
using EtherGizmos.SqlMonitor.Api.Extensions;
using EtherGizmos.SqlMonitor.Api.OData.Metadata;
using EtherGizmos.SqlMonitor.Api.Services.Background;
using EtherGizmos.SqlMonitor.Api.Services.Caching.Configuration;
using EtherGizmos.SqlMonitor.Api.Services.Configuration;
using EtherGizmos.SqlMonitor.Api.Services.Data;
using EtherGizmos.SqlMonitor.Api.Services.Data.Abstractions;
using EtherGizmos.SqlMonitor.Api.Services.Data.Configuration;
using EtherGizmos.SqlMonitor.Api.Services.Filters;
using EtherGizmos.SqlMonitor.Api.Services.Messaging;
using EtherGizmos.SqlMonitor.Api.Services.Messaging.Configuration;
using EtherGizmos.SqlMonitor.Api.Services.Validation;
using MassTransit;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.OData;
using Microsoft.EntityFrameworkCore;
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
        var options = builder.Configuration.GetSection("Connections:Use")
            .Get<UsageOptions>() ?? new UsageOptions();

        if (options.Database == DatabaseType.SqlServer)
        {
            var connectionProvider = services.GetRequiredService<IDatabaseConnectionProvider>();

            opt.UseSqlServer(connectionProvider.GetConnectionString(), conf =>
            {
                conf.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery);
            });
        }
        else
        {
            throw new InvalidOperationException(string.Format("Unknown database type: {0}", options.Database));
        }

        opt.UseLazyLoadingProxies(true);
    })
    .AddTransient<IDatabaseConnectionProvider, DatabaseConnectionProvider>()
    .AddScoped<ISaveService, SaveService>()
    .AddScoped<IInstanceService, InstanceService>()
    .AddScoped<IMetricService, MetricService>()
    .AddScoped<IPermissionService, PermissionService>()
    .AddScoped<IQueryService, QueryService>()
    .AddScoped<ISecurableService, SecurableService>()
    .AddScoped<IUserService, UserService>();

builder.Services
    .AddMassTransit(opt =>
    {
        var options = builder.Configuration.GetSection("Connections:Use")
            .Get<UsageOptions>() ?? new UsageOptions();

        opt.AddConsumer<RunQueryConsumer>();

        if (options.MessageBroker == MessageBrokerType.InMemory)
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
        else if (options.MessageBroker == MessageBrokerType.RabbitMQ)
        {
            var rabbitMQOptions = builder.Configuration.GetSection("Connections:RabbitMQ")
                .Get<RabbitMQOptions>() ?? new RabbitMQOptions();

            opt.UsingRabbitMq((context, conf) =>
            {
                string useHost;
                if (rabbitMQOptions.EndPoints?.Length == 1)
                {
                    useHost = rabbitMQOptions.EndPoints.Single().Host;
                    if (rabbitMQOptions.Port != Constants.RabbitMQ.Port)
                        useHost = $"{useHost}:{rabbitMQOptions.Port}";
                }
                else if (rabbitMQOptions.EndPoints?.Length > 1)
                {
                    useHost = "cluster";
                }
                else
                {
                    throw new InvalidOperationException("Must specify at least one RabbitMQ endpoint.");
                }

                conf.Host(useHost, opt =>
                {
                    opt.Username(rabbitMQOptions.Username);
                    opt.Password(rabbitMQOptions.Password);

                    if (rabbitMQOptions.EndPoints?.Length > 1)
                    {
                        opt.UseCluster(conf =>
                        {
                            foreach (var node in rabbitMQOptions.EndPoints)
                            {
                                var useNode = node.Host;
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
            throw new InvalidOperationException(string.Format("Unknown message broker type: {0}", options.MessageBroker));
        }
    });

builder.Services
    .AddCaching(opt =>
    {
        var options = builder.Configuration.GetSection("Connections:Use")
            .Get<UsageOptions>() ?? new UsageOptions();

        if (options.Cache == CacheType.Redis)
        {
            opt.UsingRedis(builder.Configuration.GetSection("Connections:Redis"));
        }
        else if (options.Cache == CacheType.InMemory)
        {
            opt.UsingInMemory();
        }
        else
        {
            throw new InvalidOperationException(string.Format("Unknown cache type: {0}", options.Cache));
        }
    });

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
