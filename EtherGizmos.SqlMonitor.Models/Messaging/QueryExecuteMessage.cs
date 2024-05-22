﻿using EtherGizmos.SqlMonitor.Models.Database;

namespace EtherGizmos.SqlMonitor.Models.Messaging;

public class QueryExecuteMessage
{
    public int MonitoredQueryTargetId { get; set; }

    public string ConnectionRequestToken { get; set; }

    public QueryVariant QueryVariant { get; set; }

    /// <summary>
    /// Not intended for direct use.
    /// </summary>
    public QueryExecuteMessage()
    {
        ConnectionRequestToken = null!;
        QueryVariant = null!;
    }

    public QueryExecuteMessage(
        int monitoredQueryTargetId,
        string connectionRequestToken,
        QueryVariant queryVariant)
    {
        MonitoredQueryTargetId = monitoredQueryTargetId;
        ConnectionRequestToken = connectionRequestToken;
        QueryVariant = queryVariant;
    }
}
