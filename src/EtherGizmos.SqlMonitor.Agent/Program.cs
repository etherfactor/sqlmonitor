using EtherGizmos.SqlMonitor.Shared.Configuration;
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

//builder.Services.AddConfiguredMassTransit(typeof(Program).Assembly);

var host = builder.Build();

//**********************************************************
// Run Application

host.Run();
