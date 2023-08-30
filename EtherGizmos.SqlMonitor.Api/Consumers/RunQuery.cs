using EtherGizmos.SqlMonitor.Models.Database;

namespace EtherGizmos.SqlMonitor.Api.Consumers;

public class RunQuery
{
    public Instance Instance { get; set; }

    public Query Query { get; set; }

    /// <summary>
    /// Not intended for direct use.
    /// </summary>
    public RunQuery()
    {
        Instance = null!;
        Query = null!;
    }

    public RunQuery(Instance instance, Query query)
    {
        Instance = instance;
        Query = query;
    }
}
