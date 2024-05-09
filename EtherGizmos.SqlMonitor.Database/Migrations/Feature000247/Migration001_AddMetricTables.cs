using EtherGizmos.SqlMonitor.Database.Core;
using EtherGizmos.SqlMonitor.Database.Extensions;
using FluentMigrator;
using System.Data;

namespace EtherGizmos.SqlMonitor.Database.Migrations.Feature000247;

[CreatedAt(year: 2024, month: 04, day: 13, hour: 21, minute: 00, description: "Create metric tables", trackingId: 247)]
public class Migration001_AddMetricTables : Migration
{
    public override void Up()
    {
        /*
         * Create [dbo].[aggregate_types]
         *  - the types of aggregate operations performed on metrics
         */
        Create.Table("aggregate_types")
            .WithColumn("aggregate_type_id").AsInt32().PrimaryKey()
            .WithAuditColumns()
            .WithColumn("name").AsString(200).NotNullable()
            .WithColumn("description").AsString(int.MaxValue).Nullable();

        this.AddAuditTriggerV1("aggregate_types",
            ("aggregate_type_id", DbType.Int32));

        /*
         * Create [dbo].[metrics]
         *  - statistics being tracked for systems, environments, and resources
         */
        Create.Table("metrics")
            .WithColumn("metric_id").AsInt32().PrimaryKey().Identity()
            .WithAuditColumns()
            .WithColumn("name").AsString(200).NotNullable()
            .WithColumn("description").AsString(int.MaxValue).Nullable()
            .WithColumn("aggregate_type_id").AsInt32().NotNullable()
            .WithColumn("is_soft_deleted").AsBoolean().NotNullable().WithDefaultValue(false);

        Create.ForeignKey("FK_metrics_aggregate_type_id")
            .FromTable("metrics").ForeignColumn("aggregate_type_id")
            .ToTable("aggregate_types").PrimaryColumn("aggregate_type_id");

        this.AddAuditTriggerV1("metrics",
            ("metric_id", DbType.Int32));
    }

    public override void Down()
    {
        Delete.Table("metrics");

        Delete.Table("aggregate_types");
    }
}
