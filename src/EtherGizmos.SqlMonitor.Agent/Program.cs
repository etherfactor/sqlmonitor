using EtherGizmos.SqlMonitor.Agent.Core.Services.Communication;
using EtherGizmos.SqlMonitor.Agent.Core.Services.Communication.Abstractions;
using EtherGizmos.SqlMonitor.Shared.Configuration;
using EtherGizmos.SqlMonitor.Shared.Messaging;
using Serilog;

var builder = Host.CreateApplicationBuilder(args);

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

builder.Services.AddUsageOptions();

builder.Services.AddRabbitMQOptions();

builder.Services.AddSingleton<IConnectionRetriever, ConnectionRetriever>();

builder.Services
    .AddHttpClient("coordinator", opt =>
    {
        opt.BaseAddress = new Uri("https://localhost:7200");
    });

builder.Services.AddConfiguredMassTransit(typeof(Program).Assembly);

var host = builder.Build();

//**********************************************************
// Run Application

host.Run();
