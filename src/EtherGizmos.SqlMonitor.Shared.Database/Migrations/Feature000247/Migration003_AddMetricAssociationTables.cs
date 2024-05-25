using EtherGizmos.SqlMonitor.Shared.Database.Core;
using EtherGizmos.SqlMonitor.Shared.Database.Extensions;

namespace EtherGizmos.SqlMonitor.Shared.Database.Migrations.Feature000247;

[CreatedAt(year: 2024, month: 05, day: 12, hour: 17, minute: 00, description: "Create metric association tables", trackingId: 247)]
public class Migration003_AddMetricAssociationTables : MigrationExtension
{
    public override void Up()
    {
        /*
         * Create [dbo].[script_metrics]
         *  - metrics being populated by a script and its variants
         */
        Create.Table("script_metrics")
            .WithColumn("script_metric_id").AsInt32().PrimaryKey().Identity()
            .WithAuditColumns()
            .WithColumn("script_id").AsGuid().NotNullable()
            .WithColumn("metric_id").AsInt32().NotNullable()
            .WithColumn("value_key").AsString(100).NotNullable()
            .WithColumn("is_active").AsBoolean().NotNullable().WithDefaultValue(true);

        /*
         * Create [dbo].[query_metrics]
         *  - metrics being populated by a query and its variants
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

        Delete.Table("script_metrics");
    }
}
