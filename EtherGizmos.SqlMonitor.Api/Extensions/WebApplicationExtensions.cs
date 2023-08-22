using EtherGizmos.SqlMonitor.Api.Jobs.Abstractions;
using Hangfire;

namespace EtherGizmos.SqlMonitor.Api.Extensions;

/// <summary>
/// Provides extension methods for <see cref="WebApplication"/>.
/// </summary>
public static class WebApplicationExtensions
{
    //internal static WebApplication UseHangfireJobs(this WebApplication @this)
    //{
    //    RecurringJob.AddOrUpdate<IEnqueueMonitorQueries>("EnqueueMonitorQueries", service => service.RunAsync(CancellationToken.None), "0/5 * * * * *");

    //    return @this;
    //}

    public static WebApplication UseHangfireJob<TRepeatedJob>(this WebApplication @this, string jobName, string cronExpression)
        where TRepeatedJob : class, IJob
    {
        var recurringJob = @this.Services.GetRequiredService<IRecurringJobManager>();
        //GlobalJobFilters.Filters.Add()

        recurringJob.AddOrUpdate<IHangfireRepeatedJob<TRepeatedJob>>(
            jobName,
            service => service
                .RunAsync(cronExpression, CancellationToken.None),
            cronExpression);

        return @this;
    }
}
