using EtherGizmos.SqlMonitor.Agent;
using EtherGizmos.SqlMonitor.Services.Configuration;
using EtherGizmos.SqlMonitor.Services.Messaging.Configuration;
using Microsoft.Extensions.Options;
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

builder.Services
    .AddOptions<UsageOptions>()
    .Configure<IConfiguration>((opt, conf) =>
    {
        var path = "Connections:Use";

        conf.GetSection(path)
            .Bind(opt);

        opt.AssertValid(path);
    });

builder.Services
    .AddOptions<RabbitMQOptions>()
    .Configure<IConfiguration, IOptions<UsageOptions>>((opt, conf, usage) =>
    {
        var path = "Connections:RabbitMQ";

        conf.GetSection(path)
            .Bind(opt);

        if (usage.Value.MessageBroker == MessageBrokerType.RabbitMQ)
        {
            opt.AssertValid(path);
        }
    });

builder.Services.AddHostedService<Worker>();

var host = builder.Build();

//**********************************************************
// Run Application

host.Run();
