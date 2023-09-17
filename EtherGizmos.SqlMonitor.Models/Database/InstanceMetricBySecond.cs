using System.ComponentModel.DataAnnotations.Schema;

namespace EtherGizmos.SqlMonitor.Models.Database;

[Table("instance_metrics_by_second")]
public class InstanceMetricBySecond : InstanceMetricByTime
{
}
