using AutoMapper;
using EtherGizmos.SqlMonitor.Api.Data.Access;
using EtherGizmos.SqlMonitor.Api.OData.Metadata;
using EtherGizmos.SqlMonitor.Api.Services;
using EtherGizmos.SqlMonitor.Api.Services.Abstractions;
using EtherGizmos.SqlMonitor.Database;
using EtherGizmos.SqlMonitor.Models.Api.v1;
using FluentMigrator.Runner;
using Microsoft.AspNetCore.OData;
using Microsoft.EntityFrameworkCore;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

//**********************************************************
// Add Services

builder.Services
    .AddSerilog(opt =>
    {
        opt.ReadFrom.Configuration(builder.Configuration);
    });

builder.Services.AddFluentMigratorCore()
    .ConfigureRunner(opt =>
    {
        opt.AddSqlServer2016()
            .WithGlobalConnectionString(builder.Configuration.GetConnectionString("Primary"))
            .ScanIn(typeof(DatabaseMigrationTarget).Assembly).For.Migrations()
            .WithVersionTable(new CustomVersionTableMetadata());
    });

builder.Services.AddAuthentication();
builder.Services.AddAuthorization();

builder.Services.AddControllers()
    .AddOData(opt =>
    {
        opt.AddRouteComponents("/api/v1", ODataModel.GetEdmModel(1.0m));
    });

builder.Services.AddSwaggerGen();

builder.Services
    .AddDbContext<DatabaseContext>(opt =>
    {
        opt.UseSqlServer(builder.Configuration.GetConnectionString("Primary"), conf =>
        {
            conf.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery);
        });

        opt.UseLazyLoadingProxies(true);
    });

builder.Services.AddScoped<ISecurableService, SecurableService>();

builder.Services
    .AddSingleton<IMapper>((provider) =>
    {
        MapperConfiguration configuration = new MapperConfiguration(opt =>
        {
            opt.AddSecurable();
        });

        return configuration.CreateMapper();
    });

//**********************************************************
// Add Middleware

var app = builder.Build();

//Perform the database migration
var serviceProvider = app.Services.CreateScope().ServiceProvider;
IMigrationRunner runner = serviceProvider.GetRequiredService<IMigrationRunner>();
runner.MigrateUp();

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
