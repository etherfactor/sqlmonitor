using EtherGizmos.SqlMonitor.Database.Core;
using EtherGizmos.SqlMonitor.Database.Extensions;

namespace EtherGizmos.SqlMonitor.Database.Migrations.Feature000229;

[CreatedAt(year: 2024, month: 04, day: 30, hour: 21, minute: 30, description: "Create query metric tables", trackingId: 229)]
public class Migration003_CreateQueryMetricTables : MigrationExtension
{
    public override void Up()
    {
        /*
         * Create [dbo].[query_metrics]
         *  - metrics being populated by a script and its variants
         */
        Create.Table("query_metrics")
            .WithColumn("query_metric_id").AsInt32().PrimaryKey().Identity()
            .WithAuditColumns()
            .WithColumn("query_id").AsGuid().NotNullable()
            .WithColumn("metric_id").AsInt32().NotNullable()
            .WithColumn("value_column").AsString(100).NotNullable()
            .WithColumn("is_active").AsBoolean().NotNullable().WithDefaultValue(true);
    }

    public override void Down()
    {
        Delete.Table("query_metrics");
    }
}
