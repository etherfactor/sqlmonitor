using EtherGizmos.SqlMonitor.Models.Api.v1.Enums;
using System.ComponentModel.DataAnnotations;

namespace EtherGizmos.SqlMonitor.Models.Api.v1;

[Display(Name = "MetricSeverity")]
public class MetricSeverityDTO
{
    [Display(Name = "metric_id")]
    public Guid MetricId { get; set; }

    [Display(Name = "severity_type")]
    public SeverityTypeDTO? SeverityType { get; set; }

    [Display(Name = "minimum_value")]
    public double? MinimumValue { get; set; }

    [Display(Name = "maximum_value")]
    public double? MaximumValue { get; set; }
}
