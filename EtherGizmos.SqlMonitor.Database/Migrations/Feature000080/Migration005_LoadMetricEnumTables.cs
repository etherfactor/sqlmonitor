using EtherGizmos.SqlMonitor.Database.Core;
using FluentMigrator;

namespace EtherGizmos.SqlMonitor.Database.Migrations.Feature000080;

[CreatedAt(year: 2023, month: 09, day: 21, hour: 22, minute: 00, description: "Load metric enum tables", trackingId: 80)]
public class Migration005_LoadMetricEnumTables : Migration
{
    public override void Up()
    {
        Insert.IntoTable("aggregate_types")
            .Row(new { aggregate_type_id = "AVERAGE", name = "Average", description = "Aggregates data utilizing AVG()." })
            .Row(new { aggregate_type_id = "MAXIMUM", name = "Maximum", description = "Aggregates data utilizing MAX()." })
            .Row(new { aggregate_type_id = "MINIMUM", name = "Minimum", description = "Aggregates data utilizing MIN()." })
            .Row(new { aggregate_type_id = "STD_DEV", name = "Standard Deviation", description = "Aggregates data utilizing STDEV()." })
            .Row(new { aggregate_type_id = "SUM", name = "Sum", description = "Aggregates data utilizing SUM(). Use SUM(1) to equate to COUNT(), which is not available by design." })
            .Row(new { aggregate_type_id = "VARIANCE", name = "Variance", description = "Aggregates data utilizing VAR()." });

        Insert.IntoTable("severity_types")
            .Row(new { severity_type_id = "CRITICAL", name = "Critical", description = "Needs to be addressed immediately." })
            .Row(new { severity_type_id = "ERROR", name = "Error", description = "Needs to be addressed." })
            .Row(new { severity_type_id = "NOMINAL", name = "Nominal", description = "Standard operating state." })
            .Row(new { severity_type_id = "WARNING", name = "Warning", description = "Should be investigated." });
    }

    public override void Down()
    {
        Delete.FromTable("aggregate_types")
            .Row(new { aggregate_type_id = "AVERAGE" })
            .Row(new { aggregate_type_id = "MAXIMUM" })
            .Row(new { aggregate_type_id = "MINIMUM" })
            .Row(new { aggregate_type_id = "STD_DEV" })
            .Row(new { aggregate_type_id = "SUM" })
            .Row(new { aggregate_type_id = "VARIANCE" });

        Delete.FromTable("severity_types")
            .Row(new { severity_type_id = "CRITICAL" })
            .Row(new { severity_type_id = "ERROR" })
            .Row(new { severity_type_id = "NOMINAL" })
            .Row(new { severity_type_id = "WARNING" });
    }
}
