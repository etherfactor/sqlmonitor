namespace EtherGizmos.SqlMonitor.Agent.Core.Models;

internal class QueryConnectionResponse
{
    public string Value { get; set; }

    /// <summary>
    /// Not intended for direct use.
    /// </summary>
    public QueryConnectionResponse()
    {
        Value = null!;
    }
}
