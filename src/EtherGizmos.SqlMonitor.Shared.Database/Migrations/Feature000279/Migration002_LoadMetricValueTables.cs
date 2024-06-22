using EtherGizmos.SqlMonitor.Shared.Database.Core;

namespace EtherGizmos.SqlMonitor.Shared.Database.Migrations.Feature000279;

[CreatedAt(year: 2024, month: 06, day: 16, hour: 15, minute: 30, description: "Load metric value tables", trackingId: 279)]
public class Migration002_LoadMetricValueTables : MigrationExtension
{
    public override void Up()
    {
        Merge.IntoTable("severity_types")
            .Row(new { severity_type_id = 1, name = "Nominal", description = "The measured value is within the standard operating range." })
            .Row(new { severity_type_id = 10, name = "Warning", description = "A potential issue has been detected. Some functionality may be degraded. Monitor closely and take action if necessary." })
            .Row(new { severity_type_id = 20, name = "Error", description = "An issue requiring attention has been detected. Some functionality is at risk. Take action to resolve the issue." })
            .Row(new { severity_type_id = 30, name = "Critical", description = "A serious issue has been detected. A significant portion of functionality is at risk. Immediate intervention is required." })
            .Match(e => e.severity_type_id);
    }

    public override void Down()
    {
    }
}
