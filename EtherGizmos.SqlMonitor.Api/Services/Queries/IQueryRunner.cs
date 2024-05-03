using EtherGizmos.SqlMonitor.Models.Database;

namespace EtherGizmos.SqlMonitor.Api.Services.Queries;

public interface IQueryRunner
{
    Task<QueryExecutionResultSet> ExecuteAsync(MonitoredQueryTarget queryTarget, QueryVariant queryVariant, CancellationToken cancellationToken = default);
}