using EtherGizmos.SqlMonitor.Shared.Models.Database;

namespace EtherGizmos.SqlMonitor.Api.Core.Services.Communication.Abstractions;

public interface IConnectionTokenFactory
{
    string CreateFor(MonitoredQueryTarget queryTarget, DateTimeOffset expiry);

    string CreateFor(MonitoredScriptTarget queryTarget, DateTimeOffset expiry);
}
