using EtherGizmos.SqlMonitor.Database.Core;
using EtherGizmos.SqlMonitor.Database.Extensions;
using FluentMigrator;

namespace EtherGizmos.SqlMonitor.Database.Migrations.Feature000230;

[CreatedAt(year: 2024, month: 04, day: 13, hour: 22, minute: 00, description: "Create script metric tables", trackingId: 230)]
public class Migration003_AddScriptMetricTables : AutoReversingMigration
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
    }
}
