using EtherGizmos.SqlMonitor.Database.Core;
using FluentMigrator;

namespace EtherGizmos.SqlMonitor.Database.Migrations.Feature000080;

[CreatedAt(year: 2023, month: 09, day: 17, hour: 18, minute: 15, description: "Create instance metric tables", trackingId: 80)]
public class Migration002_CreateInstanceMetricTables : AutoReversingMigration
{
    public override void Up()
    {
        /*
         * Create [dbo].[instance_metrics_by_day]
         */
        Create.Table("instance_metrics_by_day")
            .WithColumn("instance_id").AsGuid().PrimaryKey()
            .WithColumn("measured_at_utc").AsDateTime2().PrimaryKey()
            .WithColumn("metric_id").AsGuid().PrimaryKey()
            .WithColumn("metric_bucket_id").AsInt32().PrimaryKey()
            .WithColumn("value").AsFloat().NotNullable()
            .WithColumn("severity_type_id").AsAnsiString(20).NotNullable();

        Create.ForeignKey("FK_instance_metrics_by_day_instance_id")
            .FromTable("instance_metrics_by_day").ForeignColumn("instance_id")
            .ToTable("instances").PrimaryColumn("instance_id");

        Create.ForeignKey("FK_instance_metrics_by_day_metric_id")
            .FromTable("instance_metrics_by_day").ForeignColumn("metric_id")
            .ToTable("metrics").PrimaryColumn("metric_id");

        Create.ForeignKey("FK_instance_metrics_by_day_metric_bucket_id")
            .FromTable("instance_metrics_by_day").ForeignColumn("metric_bucket_id")
            .ToTable("metric_buckets").PrimaryColumn("metric_bucket_id");

        Create.ForeignKey("FK_instance_metrics_by_day_severity_type_id")
            .FromTable("instance_metrics_by_day").ForeignColumn("severity_type_id")
            .ToTable("severity_types").PrimaryColumn("severity_type_id");

        /*
         * Create [dbo].[instance_metrics_by_hour]
         */
        Create.Table("instance_metrics_by_hour")
            .WithColumn("instance_id").AsGuid().PrimaryKey()
            .WithColumn("measured_at_utc").AsDateTime2().PrimaryKey()
            .WithColumn("metric_id").AsGuid().PrimaryKey()
            .WithColumn("metric_bucket_id").AsInt32().PrimaryKey()
            .WithColumn("value").AsFloat().NotNullable()
            .WithColumn("severity_type_id").AsAnsiString(20).NotNullable();

        Create.ForeignKey("FK_instance_metrics_by_hour_instance_id")
            .FromTable("instance_metrics_by_hour").ForeignColumn("instance_id")
            .ToTable("instances").PrimaryColumn("instance_id");

        Create.ForeignKey("FK_instance_metrics_by_hour_metric_id")
            .FromTable("instance_metrics_by_hour").ForeignColumn("metric_id")
            .ToTable("metrics").PrimaryColumn("metric_id");

        Create.ForeignKey("FK_instance_metrics_by_hour_metric_bucket_id")
            .FromTable("instance_metrics_by_hour").ForeignColumn("metric_bucket_id")
            .ToTable("metric_buckets").PrimaryColumn("metric_bucket_id");

        Create.ForeignKey("FK_instance_metrics_by_hour_severity_type_id")
            .FromTable("instance_metrics_by_hour").ForeignColumn("severity_type_id")
            .ToTable("severity_types").PrimaryColumn("severity_type_id");

        /*
         * Create [dbo].[instance_metrics_by_minute]
         */
        Create.Table("instance_metrics_by_minute")
            .WithColumn("instance_id").AsGuid().PrimaryKey()
            .WithColumn("measured_at_utc").AsDateTime2().PrimaryKey()
            .WithColumn("metric_id").AsGuid().PrimaryKey()
            .WithColumn("metric_bucket_id").AsInt32().PrimaryKey()
            .WithColumn("value").AsFloat().NotNullable()
            .WithColumn("severity_type_id").AsAnsiString(20).NotNullable();

        Create.ForeignKey("FK_instance_metrics_by_minute_instance_id")
            .FromTable("instance_metrics_by_minute").ForeignColumn("instance_id")
            .ToTable("instances").PrimaryColumn("instance_id");

        Create.ForeignKey("FK_instance_metrics_by_minute_metric_id")
            .FromTable("instance_metrics_by_minute").ForeignColumn("metric_id")
            .ToTable("metrics").PrimaryColumn("metric_id");

        Create.ForeignKey("FK_instance_metrics_by_minute_metric_bucket_id")
            .FromTable("instance_metrics_by_minute").ForeignColumn("metric_bucket_id")
            .ToTable("metric_buckets").PrimaryColumn("metric_bucket_id");

        Create.ForeignKey("FK_instance_metrics_by_minute_severity_type_id")
            .FromTable("instance_metrics_by_minute").ForeignColumn("severity_type_id")
            .ToTable("severity_types").PrimaryColumn("severity_type_id");

        /*
         * Create [dbo].[instance_metrics_by_second]
         */
        Create.Table("instance_metrics_by_second")
            .WithColumn("instance_id").AsGuid().PrimaryKey()
            .WithColumn("measured_at_utc").AsDateTime2().PrimaryKey()
            .WithColumn("metric_id").AsGuid().PrimaryKey()
            .WithColumn("metric_bucket_id").AsInt32().PrimaryKey()
            .WithColumn("value").AsFloat().NotNullable()
            .WithColumn("severity_type_id").AsAnsiString(20).NotNullable();

        Create.ForeignKey("FK_instance_metrics_by_second_instance_id")
            .FromTable("instance_metrics_by_second").ForeignColumn("instance_id")
            .ToTable("instances").PrimaryColumn("instance_id");

        Create.ForeignKey("FK_instance_metrics_by_second_metric_id")
            .FromTable("instance_metrics_by_second").ForeignColumn("metric_id")
            .ToTable("metrics").PrimaryColumn("metric_id");

        Create.ForeignKey("FK_instance_metrics_by_second_metric_bucket_id")
            .FromTable("instance_metrics_by_second").ForeignColumn("metric_bucket_id")
            .ToTable("metric_buckets").PrimaryColumn("metric_bucket_id");

        Create.ForeignKey("FK_instance_metrics_by_second_severity_type_id")
            .FromTable("instance_metrics_by_second").ForeignColumn("severity_type_id")
            .ToTable("severity_types").PrimaryColumn("severity_type_id");
    }
}
