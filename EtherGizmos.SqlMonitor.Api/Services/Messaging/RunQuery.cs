using EtherGizmos.SqlMonitor.Models.Database;

namespace EtherGizmos.SqlMonitor.Api.Services.Messaging;

public class RunQuery
{
    public Guid InstanceId { get; set; }

    public Guid QueryId { get; set; }

    /// <summary>
    /// Not intended for direct use.
    /// </summary>
    public RunQuery()
    {
    }

    public RunQuery(Guid instanceId, Guid queryId)
    {
        InstanceId = instanceId;
        QueryId = queryId;
    }
}
