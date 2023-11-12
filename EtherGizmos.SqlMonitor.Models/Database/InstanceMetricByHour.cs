using System.ComponentModel.DataAnnotations.Schema;

namespace EtherGizmos.SqlMonitor.Models.Database;

[Table("instance_metrics_by_hour")]
public class InstanceMetricByHour : InstanceMetricByTime
{
}
