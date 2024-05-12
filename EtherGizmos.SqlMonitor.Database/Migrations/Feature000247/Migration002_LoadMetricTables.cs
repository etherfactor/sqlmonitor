using EtherGizmos.SqlMonitor.Database.Core;

namespace EtherGizmos.SqlMonitor.Database.Migrations.Feature000247;

[CreatedAt(year: 2024, month: 04, day: 13, hour: 21, minute: 30, description: "Load metric tables", trackingId: 247)]
public class Migration002_LoadMetricTables : MigrationExtension
{
    public override void Up()
    {
        Merge.IntoTable("aggregate_types")
            .Row(new { aggregate_type_id = 10, name = "Sum", description = "Sums up the returned metric values." })
            .Row(new { aggregate_type_id = 20, name = "Average", description = "Averages the returned metric values." })
            .Row(new { aggregate_type_id = 30, name = "Minimum", description = "Uses the minimum returned metric value." })
            .Row(new { aggregate_type_id = 35, name = "Maximum", description = "Uses the maximum returned metric value." })
            .Match(t => t.aggregate_type_id);
    }

    public override void Down()
    {
    }
}
