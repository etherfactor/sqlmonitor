using System.ComponentModel.DataAnnotations.Schema;

namespace EtherGizmos.SqlMonitor.Models.Database;

[Table("instance_metrics_by_day")]
public class InstanceMetricByDay : InstanceMetricByTime
{
}
