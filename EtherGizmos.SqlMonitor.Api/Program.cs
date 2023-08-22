using EtherGizmos.SqlMonitor.Api;
using EtherGizmos.SqlMonitor.Api.Consumers;
using EtherGizmos.SqlMonitor.Api.Data.Access;
using EtherGizmos.SqlMonitor.Api.Data.Migrations;
using EtherGizmos.SqlMonitor.Api.Extensions;
using EtherGizmos.SqlMonitor.Api.Jobs;
using EtherGizmos.SqlMonitor.Api.Jobs.Abstractions;
using EtherGizmos.SqlMonitor.Api.OData.Metadata;
using EtherGizmos.SqlMonitor.Api.Services.Abstractions;
using EtherGizmos.SqlMonitor.Api.Services.Data.Access;
using EtherGizmos.SqlMonitor.Api.Services.Data.Storage;
using EtherGizmos.SqlMonitor.Api.Services.Data.Validation;
using EtherGizmos.SqlMonitor.Api.Services.Filters;
using Hangfire;
using Hangfire.SqlServer;
using MassTransit;
using Microsoft.AspNetCore.OData;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Serilog;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);

//**********************************************************
// Configuration

builder.Configuration.AddJsonFile("appsettings.Local.json", true, true);

Shared.Initialize(builder.Configuration);

//**********************************************************
// Add Services

builder.Services
    .AddSerilog(opt =>
    {
        opt.ReadFrom.Configuration(builder.Configuration);
    });

builder.Services
    .AddHangfire((services, opt) =>
    {
        var connectionProvider = services.GetRequiredService<IDatabaseConnectionProvider>();

        opt.SetDataCompatibilityLevel(CompatibilityLevel.Version_170);
        opt.UseSimpleAssemblyNameTypeSerializer();
        opt.UseRecommendedSerializerSettings();
        opt.UseSqlServerStorage(connectionProvider.GetConnectionString(), new SqlServerStorageOptions()
        {
            SchemaName = "hangfire",
            JobExpirationCheckInterval = TimeSpan.FromSeconds(30)
        });
    })
    .AddHangfireServer((services, opt) =>
    {
        builder.Configuration.GetSection("Hangfire")
            .Bind(opt);
    })
    .AddHangfireJob<IEnqueueMonitorQueries, EnqueueMonitorQueries>();

builder.Services
    .AddStackExchangeRedisCache(opt =>
    {
        builder.Configuration.GetSection("Connections:Redis")
            .Bind(opt.ConfigurationOptions);
    });

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

                conf.ReceiveEndpoint("run-query", opt =>
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

builder.Services
    .Configure<ConfigurationOptions>(opt =>
    {
        builder.Configuration.GetSection("Connections:Redis").Bind(opt);
    })
    .AddSingleton<IConnectionMultiplexer>(e =>
    {
        var options = e.GetRequiredService<IOptionsSnapshot<ConfigurationOptions>>();
        return ConnectionMultiplexer.Connect(options.Value);
    })
    .AddScoped<IDatabase>(e => e.GetRequiredService<IConnectionMultiplexer>().GetDatabase());

builder.Services.AddTransient<IDatabaseConnectionProvider, DatabaseConnectionProvider>();

builder.Services.AddSingleton<IRecordCacheService, RecordCacheService>();
builder.Services.AddScoped<ISaveService, SaveService>();

builder.Services.AddScoped<IInstanceService, InstanceService>();
builder.Services.AddScoped<IPermissionService, PermissionService>();
builder.Services.AddScoped<IQueryService, QueryService>();
builder.Services.AddScoped<ISecurableService, SecurableService>();
builder.Services.AddScoped<IUserService, UserService>();

builder.Services.AddMapper();

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

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseODataRouteDebug();
}

app.UseHangfireDashboard();
app.UseHangfireJob<IEnqueueMonitorQueries>("EnqueueMonitorQueries", "0/1 * * * * *");

app.UseAuthentication();
app.UseAuthorization();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
});

//**********************************************************
// Run Application

app.Run();
