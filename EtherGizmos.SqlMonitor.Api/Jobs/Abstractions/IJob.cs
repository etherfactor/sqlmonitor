namespace EtherGizmos.SqlMonitor.Api.Jobs.Abstractions;

public interface IJob
{
    Task RunAsync(CancellationToken cancellationToken);
}
