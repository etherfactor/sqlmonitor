using EtherGizmos.SqlMonitor.Api;
using EtherGizmos.SqlMonitor.Api.Configuration;
using EtherGizmos.SqlMonitor.Api.Consumers;
using EtherGizmos.SqlMonitor.Api.Data.Access;
using EtherGizmos.SqlMonitor.Api.Data.Migrations;
using EtherGizmos.SqlMonitor.Api.Extensions;
using EtherGizmos.SqlMonitor.Api.OData.Metadata;
using EtherGizmos.SqlMonitor.Api.Services.Background;
using EtherGizmos.SqlMonitor.Api.Services.Caching;
using EtherGizmos.SqlMonitor.Api.Services.Caching.Abstractions;
using EtherGizmos.SqlMonitor.Api.Services.Data;
using EtherGizmos.SqlMonitor.Api.Services.Data.Abstractions;
using EtherGizmos.SqlMonitor.Api.Services.Filters;
using EtherGizmos.SqlMonitor.Api.Services.Validation;
using MassTransit;
using Medallion.Threading;
using Medallion.Threading.Redis;
using Microsoft.AspNetCore.OData;
using Microsoft.EntityFrameworkCore;
using Serilog;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);

//**********************************************************
// Configuration

builder.Configuration.AddJsonFile("appsettings.Local.json", true, true);

Shared.Initialize(builder.Configuration);

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
        var connectionProvider = services.GetRequiredService<IDatabaseConnectionProvider>();

        opt.UseSqlServer(connectionProvider.GetConnectionString(), conf =>
        {
            conf.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery);
        });

        opt.UseLazyLoadingProxies(true);
    });

builder.Services
    .AddMassTransit(opt =>
    {
        var options = builder.Configuration.GetSection("Connections:MassTransit")
            .Get<MassTransitOptions>() ?? new MassTransitOptions();

        opt.AddConsumer<RunQueryConsumer>();

        if (options.Use == MassTransitServiceBusType.InMemory)
        {
            opt.UsingInMemory((context, conf) =>
            {
                conf.ConfigureEndpoints(context);
                conf.Host();

                conf.ReceiveEndpoint(RunQueryConsumer.Queue, opt =>
                {
                    opt.Consumer<RunQueryConsumer>(context);
                });
            });
        }
        else if (options.Use == MassTransitServiceBusType.RabbitMQ)
        {
            opt.UsingRabbitMq((context, conf) =>
            {
                conf.ConfigureEndpoints(context);
                conf.Host(options.RabbitMQ.Host, opt =>
                {
                    opt.Username(options.RabbitMQ.Username);
                    opt.Password(options.RabbitMQ.Password);
                });
            });
        }
        else
        {
            throw new InvalidOperationException(string.Format("Unknown service bus type: {0}", options.Use));
        }
    });

//builder.Services
//    .Configure<ConfigurationOptions>(opt =>
//    {
//        builder.Configuration.GetSection("Connections:Redis").Bind(opt);
//    })
//    .AddSingleton<IConnectionMultiplexer>(e =>
//    {
//        var options = e.CreateScope().ServiceProvider.GetRequiredService<IOptionsSnapshot<ConfigurationOptions>>();
//        var config = options.Value;
//        config.EndPoints.Add("192.168.1.5", 6379);
//        return ConnectionMultiplexer.Connect(config);
//    })
//    .AddTransient<IDatabase>(e => e.GetRequiredService<IConnectionMultiplexer>().GetDatabase());

builder.Services
    //.Configure<ConfigurationOptions>(opt =>
    //{
    //    var section = builder.Configuration.GetSection("Connections:Redis");
    //    section.Bind(opt);

    //    var endpoints = section.GetSection("EndPoints").Get<RedisHost[]>()
    //        ?? Array.Empty<RedisHost>();
    //    foreach (var endpoint in endpoints)
    //    {
    //        opt.EndPoints.Add(endpoint.Host, endpoint.Port);
    //    }
    //})
    //.AddStackExchangeRedisCache(opt =>
    //{
    //    opt.conn
    //    opt.ConfigurationOptions
    //        ??= new ConfigurationOptions();

    //    var section = builder.Configuration.GetSection("Connections:Redis");
    //    section.Bind(opt.ConfigurationOptions);

    //    var endpoints = section.GetSection("EndPoints").Get<RedisHost[]>()
    //        ?? Array.Empty<RedisHost>();
    //    foreach (var endpoint in endpoints)
    //    {
    //        opt.ConfigurationOptions.EndPoints.Add(endpoint.Host, endpoint.Port);
    //    }
    //})
    .AddRedisCache(builder.Configuration.GetSection("Connections:Redis"))
    .AddSingleton<IDistributedLockProvider>(services =>
    {
        var multiplexer = services.GetRequiredService<IConnectionMultiplexer>();
        return new RedisDistributedSynchronizationProvider(multiplexer.GetDatabase());
    });

builder.Services
    .AddOptions<CachingOptions>()
    .Configure(options =>
    {
        builder.Configuration.GetSection("Caching")
            .Bind(options);
    });

builder.Services.AddTransient<IDatabaseConnectionProvider, DatabaseConnectionProvider>();

builder.Services.AddSingleton<ILockedDistributedCache, LockedDistributedCache>();

builder.Services.AddScoped<ISaveService, SaveService>();

builder.Services.AddScoped<IInstanceService, InstanceService>();
builder.Services.AddScoped<IPermissionService, PermissionService>();
builder.Services.AddScoped<IQueryService, QueryService>();
builder.Services.AddScoped<ISecurableService, SecurableService>();
builder.Services.AddScoped<IUserService, UserService>();

builder.Services.AddMapper();

builder.Services.AddHostedService<EnqueueMonitorQueries>();

//**********************************************************
// Add Middleware

var app = builder.Build();

var serviceProvider = app.Services
    .CreateScope()
    .ServiceProvider;

//var testKey1 = CacheKey.Create<string>("Key1", true);
//var testKey2 = CacheKey.Create<Query>("Key2", true);

//var test = serviceProvider.GetRequiredService<IDistributedCache>();
//var test2 = serviceProvider.GetRequiredService<IConnectionMultiplexer>();
//var test3 = serviceProvider.GetRequiredService<IDistributedLockProvider>();
//var test4 = serviceProvider.GetRequiredService<ILockedDistributedCache>();

//CacheLock<Query> testLock2 = null!;
//await test4.TryAcquireLockAsync(testKey2, TimeSpan.FromDays(1), @out => testLock2 = @out);
//await test4.TrySetWithLockAsync(testKey2, testLock2, new Query() { Id = Guid.NewGuid() });

//Query? value = null;
//await test4.TryGetAsync(testKey2, @out => value = @out);

//var testKey1_2 = CacheKey.Create<string>("Key1", true);
//var testKey1_3 = CacheKey.Create<string>("Key1", false);

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
