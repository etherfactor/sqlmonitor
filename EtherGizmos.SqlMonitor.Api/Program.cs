using EtherGizmos.SqlMonitor.Api.Data.Access;
using EtherGizmos.SqlMonitor.Api.Data.Migrations;
using EtherGizmos.SqlMonitor.Api.Extensions;
using EtherGizmos.SqlMonitor.Api.OData.Metadata;
using EtherGizmos.SqlMonitor.Api.Services.Abstractions;
using EtherGizmos.SqlMonitor.Api.Services.Background;
using EtherGizmos.SqlMonitor.Api.Services.Data.Access;
using EtherGizmos.SqlMonitor.Api.Services.Data.Storage;
using EtherGizmos.SqlMonitor.Api.Services.Data.Validation;
using EtherGizmos.SqlMonitor.Api.Services.Filters;
using Microsoft.AspNetCore.OData;
using Microsoft.EntityFrameworkCore;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

//**********************************************************
// Configuration

builder.Configuration.AddJsonFile("appsettings.Local.json", true, true);

//**********************************************************
// Add Services

builder.Services
    .AddSerilog(opt =>
    {
        opt.ReadFrom.Configuration(builder.Configuration);
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

builder.Services.AddScoped<IDatabaseConnectionProvider, DatabaseConnectionProvider>();

builder.Services.AddSingleton<IRecordCacheService, RecordCacheService>();
builder.Services.AddScoped<ISaveService, SaveService>();

builder.Services.AddScoped<IInstanceService, InstanceService>();
builder.Services.AddScoped<IPermissionService, PermissionService>();
builder.Services.AddScoped<IQueryService, QueryService>();
builder.Services.AddScoped<ISecurableService, SecurableService>();
builder.Services.AddScoped<IUserService, UserService>();

builder.Services.AddHostedService<QueryRunner>();

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

app.UseAuthentication();
app.UseAuthorization();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
});

//**********************************************************
// Run Application

app.Run();
