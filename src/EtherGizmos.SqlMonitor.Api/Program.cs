using EtherGizmos.Extensions.DependencyInjection;
using EtherGizmos.SqlMonitor.Api;
using EtherGizmos.SqlMonitor.Api.Extensions;
using EtherGizmos.SqlMonitor.Api.Services.Authorization;
using EtherGizmos.SqlMonitor.Api.Services.Background;
using EtherGizmos.SqlMonitor.Api.Services.Caching;
using EtherGizmos.SqlMonitor.Api.Services.Caching.Abstractions;
using EtherGizmos.SqlMonitor.Api.Services.Filters;
using EtherGizmos.SqlMonitor.Api.Services.Validation;
using EtherGizmos.SqlMonitor.Shared.Configuration;
using EtherGizmos.SqlMonitor.Shared.Configuration.Caching;
using EtherGizmos.SqlMonitor.Shared.Configuration.Data;
using EtherGizmos.SqlMonitor.Shared.Database;
using EtherGizmos.SqlMonitor.Shared.Database.Services;
using EtherGizmos.SqlMonitor.Shared.Database.Services.Abstractions;
using EtherGizmos.SqlMonitor.Shared.Models;
using EtherGizmos.SqlMonitor.Shared.OAuth.Models;
using EtherGizmos.SqlMonitor.Shared.Utilities;
using MassTransit;
using Microsoft.AspNetCore.OData;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using StackExchange.Redis;
using Swashbuckle.AspNetCore.SwaggerGen;

var builder = WebApplication.CreateBuilder(args);

//**********************************************************
// Configuration

builder.Configuration.AddJsonFile("appsettings.Local.json", true, true);

builder.AddLoggingServices();

//**********************************************************
// Add Services

builder.Services.AddUsageOptions();

builder.Services.AddRabbitMQOptions();

builder.Services.AddRedisOptions();

builder.Services.AddMySqlOptions();

builder.Services.AddPostgreSqlOptions();

builder.Services.AddSqlServerOptions();

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

builder.Services.AddDatabaseServices();

builder.Services
    .AddDbContext<AuthorizationContext>((services, opt) =>
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
    });

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

builder.Services.AddConfiguredMassTransit(typeof(Program).Assembly);

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
