namespace EtherGizmos.SqlMonitor.Api.Jobs.Abstractions;

public interface IHangfireRepeatedJob<TRepeatedJob>
    where TRepeatedJob : class, IJob
{
    Task RunAsync(string cronExpression, CancellationToken cancellationToken);
}
