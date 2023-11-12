using EtherGizmos.SqlMonitor.Models.Database;
using EtherGizmos.SqlMonitor.Models.Database.Enums;
using Microsoft.Data.SqlClient;

namespace EtherGizmos.SqlMonitor.Api.Extensions;

internal static class QueryMetricExtensions
{
    internal static SeverityType GetSeverityForValue(this QueryMetric @this, SqlDataReader reader, double value)
    {
        return SeverityType.Nominal;
    }
}
