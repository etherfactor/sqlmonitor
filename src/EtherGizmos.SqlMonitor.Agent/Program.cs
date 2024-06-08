using EtherGizmos.SqlMonitor.Agent.Core;
using EtherGizmos.SqlMonitor.Agent.Core.Services.Communication;
using EtherGizmos.SqlMonitor.Agent.Core.Services.Communication.Abstractions;
using EtherGizmos.SqlMonitor.Shared.Configuration;
using EtherGizmos.SqlMonitor.Shared.Messaging;
using EtherGizmos.SqlMonitor.Shared.Utilities;

var builder = Host.CreateApplicationBuilder(args);

//**********************************************************
// Configuration

builder.Configuration.AddJsonFile("appsettings.Local.json", true, true)
    .AddInMemoryCollection(new Dictionary<string, string?>()
    {
        { "Connections:Use:Database", "MySql" },
        { "Connections:Use:Cache", "InMemory" },
    });

builder.AddLoggingServices();

//**********************************************************
// Add Services

// General
builder.Services.AddUsageOptions();

// Messaging
builder.Services.AddRabbitMQOptions();

builder.Services.AddConfiguredMassTransit(typeof(AgentCore).Assembly);

// Communication
builder.Services.AddSingleton<IConnectionRetriever, ConnectionRetriever>();

builder.Services
    .AddHttpClient("coordinator", opt =>
    {
        opt.BaseAddress = new Uri("https://localhost:7200");
    });

var host = builder.Build();

//**********************************************************
// Run Application

host.Run();
