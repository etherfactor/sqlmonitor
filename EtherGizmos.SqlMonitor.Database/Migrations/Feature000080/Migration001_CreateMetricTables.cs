using EtherGizmos.SqlMonitor.Database.Core;
using EtherGizmos.SqlMonitor.Database.Extensions;
using FluentMigrator;

namespace EtherGizmos.SqlMonitor.Database.Migrations.Feature000080;

[CreatedAt(year: 2023, month: 09, day: 17, hour: 18, minute: 00, description: "Create metric tables", trackingId: 80)]
public class Migration001_CreateMetricTables : AutoReversingMigration
{
    public override void Up()
    {
        /*
         * Create [dbo].[aggregate_types]
         */
        Create.Table("aggregate_types")
            .WithColumn("aggregate_type_id").AsAnsiString(20).PrimaryKey()
            .WithAuditColumns()
            .WithColumn("name").AsString(200).NotNullable()
            .WithColumn("description").AsString(int.MaxValue).Nullable();

        /*
         * Create [dbo].[severity_types]
         */
        Create.Table("severity_types")
            .WithColumn("severity_type_id").AsAnsiString(20).PrimaryKey()
            .WithAuditColumns()
            .WithColumn("name").AsString(200).NotNullable()
            .WithColumn("description").AsString(int.MaxValue).Nullable();

        /*
         * Create [dbo].[metrics]
         */
        Create.Table("metrics")
            .WithColumn("metric_id").AsGuid().PrimaryKey().WithDefault(SystemMethods.NewGuid)
            .WithAuditColumns()
            .WithColumn("name").AsString(100).NotNullable()
            .WithColumn("description").AsString(int.MaxValue).Nullable()
            .WithColumn("aggregate_type_id").AsAnsiString(20).NotNullable();

        Create.ForeignKey("FK_metrics_aggregate_type_id")
            .FromTable("metrics").ForeignColumn("aggregate_type_id")
            .ToTable("aggregate_types").PrimaryColumn("aggregate_type_id");

        /*
         * Create [dbo].[metric_severities]
         */
        Create.Table("metric_severities")
            .WithColumn("metric_id").AsGuid().PrimaryKey()
            .WithColumn("severity_type_id").AsAnsiString(20).PrimaryKey()
            .WithAuditColumns()
            .WithColumn("minimum_value").AsDouble()
            .WithColumn("maximum_value").AsDouble();

        Create.ForeignKey("FK_metric_severities_metric_id")
            .FromTable("metric_severities").ForeignColumn("metric_id")
            .ToTable("metrics").PrimaryColumn("metric_id");

        Create.ForeignKey("FK_metric_severities_severity_type_id")
            .FromTable("metric_severities").ForeignColumn("severity_type_id")
            .ToTable("severity_types").PrimaryColumn("severity_type_id");

        /*
         * Create [dbo].[metric_buckets]
         */
        Create.Table("metric_buckets")
            .WithColumn("metric_bucket_id").AsInt32().PrimaryKey().Identity()
            .WithAuditColumns()
            .WithColumn("name").AsString(100).NotNullable();
    }
}
