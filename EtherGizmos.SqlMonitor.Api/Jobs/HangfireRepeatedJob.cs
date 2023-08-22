using EtherGizmos.SqlMonitor.Api.Jobs.Abstractions;
using Hangfire;
using NCrontab;

namespace EtherGizmos.SqlMonitor.Api.Jobs;

public class HangfireRepeatedJob<TRepeatedJob> : IHangfireRepeatedJob<TRepeatedJob>
    where TRepeatedJob : class, IJob
{
    private ILogger Logger { get; }

    private TRepeatedJob RepeatedJob { get; }

    public HangfireRepeatedJob(ILogger<HangfireRepeatedJob<TRepeatedJob>> logger, TRepeatedJob repeatedJob)
    {
        Logger = logger;
        RepeatedJob = repeatedJob;
    }

    [DisableConcurrentExecution(60)]
    public async Task RunAsync(string cronExpression, CancellationToken cancellationToken)
    {
        Logger.Log(LogLevel.Debug, "Running repeated job");

        var cron = CrontabSchedule.Parse(cronExpression, new CrontabSchedule.ParseOptions()
        {
            IncludingSeconds = true
        });

        var startedAt = DateTime.Now;
        var occurrences = cron.GetNextOccurrences(startedAt, startedAt.Add(Shared.HangfireInterval).AddMilliseconds(-100));

        var tasks = new List<Task>();
        foreach (var occurrence in occurrences)
        {
            var task = Task.Run(async () =>
            {
                var delta = occurrence - DateTime.Now;
                if (delta.Ticks < 0)
                {
                    //Don't run outdated jobs
                    Logger.Log(LogLevel.Warning, "Skipping sub job; date is in past");
                    return;
                }

                await Task.Delay(delta);

                Logger.Log(LogLevel.Debug, "Running sub job");

                await RepeatedJob.RunAsync(cancellationToken);
            }, cancellationToken);

            tasks.Add(task);
        }

        await Task.WhenAll(tasks);

        Logger.Log(LogLevel.Debug, "Finished repeated job");
    }
}
