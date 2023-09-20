using EtherGizmos.SqlMonitor.Database.Core;
using EtherGizmos.SqlMonitor.Database.Extensions;
using FluentMigrator;

namespace EtherGizmos.SqlMonitor.Database.Migrations.Feature000080;

[CreatedAt(year: 2023, month: 09, day: 17, hour: 18, minute: 30, description: "Create query metric tables", trackingId: 80)]
public class Migration003_CreateQueryMetricTables : AutoReversingMigration
{
    public override void Up()
    {
        /*
         * Create [dbo].[query_metrics]
         */
        Create.Table("query_metrics")
            .WithColumn("query_id").AsGuid().PrimaryKey()
            .WithColumn("metric_id").AsGuid().PrimaryKey()
            .WithAuditColumns()
            .WithColumn("value_expression").AsString(500);

        Create.ForeignKey("FK_query_metrics_query_id")
            .FromTable("query_metrics").ForeignColumn("query_id")
            .ToTable("queries").PrimaryColumn("query_id");

        Create.ForeignKey("FK_query_metrics_metric_id")
            .FromTable("query_metrics").ForeignColumn("metric_id")
            .ToTable("metrics").PrimaryColumn("metric_id");

        /*
         * Create [dbo].[query_metric_severities]
         */
        Create.Table("query_metric_severities")
            .WithColumn("query_id").AsGuid().PrimaryKey()
            .WithColumn("metric_id").AsGuid().PrimaryKey()
            .WithColumn("severity_type_id").AsAnsiString(20).PrimaryKey()
            .WithAuditColumns()
            .WithColumn("minimum_expression").AsString(500).Nullable()
            .WithColumn("maximum_expression").AsString(500).Nullable();

        Create.ForeignKey("FK_query_metric_severities_query_id")
            .FromTable("query_metric_severities").ForeignColumn("query_id")
            .ToTable("queries").PrimaryColumn("query_id");

        Create.ForeignKey("FK_query_metric_severities_metric_id")
            .FromTable("query_metric_severities").ForeignColumn("metric_id")
            .ToTable("metrics").PrimaryColumn("metric_id");

        Create.ForeignKey("FK_query_metric_severities_severity_type_id")
            .FromTable("query_metric_severities").ForeignColumn("severity_type_id")
            .ToTable("severity_types").PrimaryColumn("severity_type_id");
    }
}
