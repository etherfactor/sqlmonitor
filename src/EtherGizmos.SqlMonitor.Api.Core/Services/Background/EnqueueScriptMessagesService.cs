﻿using EtherGizmos.SqlMonitor.Api.Core.Helpers;
using EtherGizmos.SqlMonitor.Shared.Database.Services.Abstractions;
using EtherGizmos.SqlMonitor.Shared.Messaging;
using EtherGizmos.SqlMonitor.Shared.Messaging.Messages;
using EtherGizmos.SqlMonitor.Shared.Models.Database;
using EtherGizmos.SqlMonitor.Shared.Redis.Caching.Abstractions;
using EtherGizmos.SqlMonitor.Shared.Redis.Locking.Abstractions;
using EtherGizmos.SqlMonitor.Shared.Redis.Services.Background.Abstractions;
using EtherGizmos.SqlMonitor.Shared.Utilities.Extensions;
using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace EtherGizmos.SqlMonitor.Api.Core.Services.Background;

public class EnqueueScriptMessagesService : GlobalConstantBackgroundService
{
    private const string CronExpression = "0/15 * * * * *";
    private const string ConstantCronExpression = "0/1 * * * * *";

    private readonly ILogger _logger;
    private readonly ILockingCoordinator _distributedLockProvider;
    private readonly IRecordCache _distributedRecordCache;

    /// <summary>
    /// Construct the service.
    /// </summary>
    /// <param name="logger">The logger to use.</param>
    /// <param name="serviceProvider">Provides access to services.</param>
    public EnqueueScriptMessagesService(
        ILogger<EnqueueScriptMessagesService> logger,
        IServiceProvider serviceProvider,
        ILockingCoordinator distributedLockProvider,
        IRecordCache distributedRecordCache)
        : base(logger, serviceProvider, distributedLockProvider, CronExpression, ConstantCronExpression)
    {
        _logger = logger;
        _distributedLockProvider = distributedLockProvider;
        _distributedRecordCache = distributedRecordCache;
    }

    /// <inheritdoc/>
    protected override async Task DoConstantGlobalWorkAsync(IServiceProvider provider, CancellationToken stoppingToken)
    {
        var now = DateTimeOffset.Now;
        var loadTargetTasks = new Dictionary<int, Task<List<MonitoredScriptTarget>>>();

        var scriptSet = _distributedRecordCache.EntitySet<Script>();
        var monitoredScriptTargetSet = _distributedRecordCache.EntitySet<MonitoredScriptTarget>();

        var scriptsToRun = await scriptSet.Where(e => e.NextRunAtUtc)
            .IsLessThanOrEqualTo(DateTimeOffset.UtcNow.Round(TimeSpan.FromSeconds(1)))
            .ToListAsync(stoppingToken);

        var sendEndpointProvider = provider.GetRequiredService<ISendEndpointProvider>();
        var sendEndpoint = await sendEndpointProvider.GetSendEndpoint(new Uri($"queue:{MessagingConstants.Queues.AgentScriptExecute}"));

        var scriptVariantsToRun = scriptsToRun.SelectMany(e => e.Variants)
            .ToList();

        //Return early if there's nothing to run
        if (scriptVariantsToRun.Count == 0)
            return;

        _logger.LogInformation("Identified {ScriptCount} scripts scheduled to run", scriptsToRun.Count);

        foreach (var scriptVariant in scriptVariantsToRun)
        {
            if (!loadTargetTasks.ContainsKey(scriptVariant.ScriptInterpreterId))
            {
                loadTargetTasks.Add(scriptVariant.ScriptInterpreterId, monitoredScriptTargetSet
                    .Where(e => e.ScriptInterpreterId)
                    .IsEqualTo(scriptVariant.ScriptInterpreterId)
                    .ToListAsync(stoppingToken));
            }

            var instancesToTarget = await loadTargetTasks[scriptVariant.ScriptInterpreterId];

            foreach (var instance in instancesToTarget)
            {
                var message = new ScriptExecuteMessage(
                    scriptVariant.Script.Id,
                    scriptVariant.Script.Name,
                    instance.Id,
                    ConnectionTokenHelper.CreateFor(instance, now.Add(scriptVariant.Script.RunFrequency)),
                    new()
                    {
                        Name = scriptVariant.ScriptInterpreter.Name,
                        Command = scriptVariant.ScriptInterpreter.Command,
                        Arguments = scriptVariant.ScriptInterpreter.Arguments,
                        Extension = scriptVariant.ScriptInterpreter.Extension,
                    },
                    instance.ExecType,
                    scriptVariant.ScriptText,
                    scriptVariant.Script.BucketKey,
                    scriptVariant.Script.TimestampUtcKey);

                foreach (var scriptMetric in scriptVariant.Script.Metrics)
                {
                    message.AddMetric(scriptMetric.MetricId, scriptMetric.ValueKey);
                }

                await sendEndpoint.Send(message, stoppingToken);
            }
        }

        var parallelOptions = new ParallelOptions()
        {
            MaxDegreeOfParallelism = 8,
        };

        await Parallel.ForEachAsync(scriptsToRun, parallelOptions, async (script, cancellationToken) =>
        {
            using var subScope = provider.CreateScope();
            var subProvider = subScope.ServiceProvider;

            var scriptService = subProvider.GetRequiredService<IScriptService>();
            scriptService.Add(script);

            var saveService = provider.GetRequiredService<ISaveService>();
            await saveService.SaveChangesAsync();

            script.LastRunAtUtc = DateTimeOffset.UtcNow.Round(TimeSpan.FromSeconds(1));
            await scriptSet.AddAsync(script, stoppingToken);
        });
    }
}
