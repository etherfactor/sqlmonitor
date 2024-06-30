using Asp.Versioning;
using EtherGizmos.SqlMonitor.Api;
using EtherGizmos.SqlMonitor.Api.Core;
using EtherGizmos.SqlMonitor.Api.Core.Services.Background;
using EtherGizmos.SqlMonitor.Api.Core.Services.Filters;
using EtherGizmos.SqlMonitor.Api.Core.Services.Messaging;
using EtherGizmos.SqlMonitor.Api.Core.Services.Validation;
using EtherGizmos.SqlMonitor.Shared.Configuration;
using EtherGizmos.SqlMonitor.Shared.Configuration.Data;
using EtherGizmos.SqlMonitor.Shared.Configuration.Sources;
using EtherGizmos.SqlMonitor.Shared.Database;
using EtherGizmos.SqlMonitor.Shared.Database.Services;
using EtherGizmos.SqlMonitor.Shared.Database.Services.Abstractions;
using EtherGizmos.SqlMonitor.Shared.Messaging;
using EtherGizmos.SqlMonitor.Shared.Messaging.Extensions;
using EtherGizmos.SqlMonitor.Shared.Models;
using EtherGizmos.SqlMonitor.Shared.OAuth;
using EtherGizmos.SqlMonitor.Shared.OAuth.Models;
using EtherGizmos.SqlMonitor.Shared.OAuth.Services;
using EtherGizmos.SqlMonitor.Shared.Redis;
using EtherGizmos.SqlMonitor.Shared.Redis.Caching.Abstractions;
using EtherGizmos.SqlMonitor.Shared.Redis.Locking.Abstractions;
using EtherGizmos.SqlMonitor.Shared.Utilities;
using Microsoft.AspNetCore.OData;
using Microsoft.Extensions.Options;
using Swashbuckle.AspNetCore.SwaggerGen;

var builder = WebApplication.CreateBuilder(args);

//**********************************************************
// Configuration

builder.Configuration
    .AddInMemoryCollection(new Dictionary<string, string?>()
    {
        { "Imports:DefaultEnvironment:Priority", "0" },
        { "Imports:DefaultEnvironment:Optional", "false" },
        { "Imports:DefaultEnvironment:Type", "Environment" },
        { "Imports:DefaultFileAppSettings:Priority", "-10" },
        { "Imports:DefaultFileAppSettings:Optional", "false" },
        { "Imports:DefaultFileAppSettings:Type", "File" },
        { "Imports:DefaultFileAppSettings:File:Path", "appsettings.json" },
        { "Imports:DefaultFileAppSettingsLocal:Priority", "-9" },
        { "Imports:DefaultFileAppSettingsLocal:Optional", "false" },
        { "Imports:DefaultFileAppSettingsLocal:Type", "File" },
        { "Imports:DefaultFileAppSettingsLocal:File:Path", "appsettings.Local.json" },
    })
    .AddJsonFile("appsettings.Local.json", true, true);

var connectionRoot = new ConnectionRoot();
builder.Configuration.Bind(connectionRoot);

var importRoot = new ImportRoot(connectionRoot);
builder.Configuration.Bind(importRoot);

importRoot.Apply(builder.Configuration);

builder.AddLoggingServices();

//**********************************************************
// Add Services

// General
builder.Services.AddUsageOptions();

// Caching
builder.Services.AddRedisOptions();

builder.Services.AddDistributedCaching();
builder.Services.AddDistributedLocking();

// Database
builder.Services.AddMySqlOptions();
builder.Services.AddPostgreSqlOptions();
builder.Services.AddSqlServerOptions();

builder.Services.AddDatabaseConnectionProvider();

builder.Services.AddDatabaseContext();

builder.Services.AddAuthorizationContext();

// Messaging
builder.Services.AddRabbitMQOptions();

builder.Services.AddConfiguredMassTransit(
    (context, opt) =>
    {
        opt.ReceiveQueue<QueryResultConsumer>(context, MessagingConstants.Queues.CoordinatorQueryResult);
        opt.ReceiveQueue<ScriptResultConsumer>(context, MessagingConstants.Queues.CoordinatorScriptResult);
    },
    typeof(ApiCore).Assembly)
    .ImportScoped<ApplicationContext>()
    .ImportScoped<ILockingCoordinator>()
    .ImportScoped<IMetricBucketLockFactory>()
    .ImportScoped<IMonitoredTargetMetricsBySecondService>()
    .ImportScoped<IRecordCache>()
    .ImportScoped<ISaveService>();

// Authentication
builder.Services.AddAuthentication();
builder.Services.AddAuthorization();

builder.Services.AddOpenIddict()
    .AddCore(opt =>
    {
        opt.UseEntityFrameworkCore()
            .UseDbContext<AuthorizationContext>()
            .ReplaceDefaultEntities<OAuth2Application, OAuth2Authorization, OAuth2Scope, OAuth2Token, int>();
    })
    .AddServer(opt =>
    {
        opt.SetTokenEndpointUris(Constants.OAuth2.Endpoints.Token);

        opt.AllowClientCredentialsFlow()
            .AllowRefreshTokenFlow();

        opt.AddDevelopmentEncryptionCertificate()
            .AddDevelopmentSigningCertificate();

        opt.UseAspNetCore()
            .EnableTokenEndpointPassthrough();

        opt.DisableAccessTokenEncryption();
    })
    .AddValidation(opt =>
    {
        opt.UseLocalServer();

        opt.UseAspNetCore();
    });

builder.Services.AddHostedService<OAuth2Seeder>();

// Models
builder.Services.AddMapper();

builder.Services.AddModelValidators();

// Controllers
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
        opt.ApiVersionReader = new UrlSegmentApiVersionReader();
    })
    .AddMvc()
    .AddApiExplorer(opt =>
    {
        opt.GroupNameFormat = "'v'VVV";
        opt.SubstituteApiVersionInUrl = true;
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

builder.Services
    .AddCors(opt =>
    {
        opt.AddPolicy("All", builder =>
        {
            builder.AllowAnyOrigin()
                .AllowAnyHeader()
                .AllowAnyMethod();
        });
    });

// Documentation
builder.Services.AddTransient<IConfigureOptions<SwaggerGenOptions>, ConfigureSwaggerOptions>();
builder.Services
    .AddSwaggerGen(opt =>
    {
        //Add a custom operation filter which sets default values
        opt.OperationFilter<SwaggerDefaultValues>();
    });

// Hosted services
builder.Services.AddHostedService<CacheLoadService>();
builder.Services.AddHostedService<EnqueueQueryMessagesService>();
builder.Services.AddHostedService<EnqueueScriptMessagesService>();

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
