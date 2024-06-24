using EtherGizmos.SqlMonitor.Shared.Database.Core;
using EtherGizmos.SqlMonitor.Shared.Database.Extensions;

namespace EtherGizmos.SqlMonitor.Shared.Database.Migrations.Feature000279;

[CreatedAt(year: 2024, month: 06, day: 16, hour: 15, minute: 00, description: "Create metric value tables", trackingId: 279)]
public class Migration001_CreateMetricValueTables : MigrationExtension
{
    public override void Up()
    {
        /*
         * Create [dbo].[severity_types]
         *  - the severity of measured values
         */
        Create.Table("severity_types")
            .WithColumn("severity_type_id").AsInt32().PrimaryKey()
            .WithAuditColumns()
            .WithColumn("name").AsString(200).NotNullable()
            .WithColumn("description").AsString(int.MaxValue).Nullable();

        /*
         * Create [dbo].[metric_buckets]
         *  - the buckets by which metrics are grouped per target
         */
        Create.Table("metric_buckets")
            .WithColumn("metric_bucket_id").AsInt32().PrimaryKey().Identity()
            .WithAuditColumns()
            .WithColumn("name").AsString(200).NotNullable().Unique()
            .WithColumn("description").AsString(int.MaxValue).Nullable();

        /*
         * Create [dbo].[monitored_target_metrics_by_second]
         *  - metrics measured against targets, aggregared per second
         */
        Create.Table("monitored_target_metrics_by_second")
            .WithColumn("monitored_target_id").AsInt32().PrimaryKey()
            .WithColumn("metric_id").AsInt32().PrimaryKey()
            .WithColumn("metric_bucket_id").AsInt32().PrimaryKey()
            .WithColumn("measured_at_utc").AsDateTime2().PrimaryKey()
            .WithColumn("value").AsDouble().NotNullable()
            .WithColumn("severity_type_id").AsInt32().NotNullable();

        Create.Index("IX_monitored_target_metrics_by_second_metric_id_metric_bucket_i")
            .OnTable("monitored_target_metrics_by_second")
            .OnColumn("metric_id").Ascending()
            .OnColumn("metric_bucket_id").Ascending()
            .OnColumn("measured_at_utc").Ascending();

        Create.Index("IX_monitored_target_metrics_by_second_metric_id_measured_at_utc")
            .OnTable("monitored_target_metrics_by_second")
            .OnColumn("metric_id").Ascending()
            .OnColumn("measured_at_utc").Ascending();

        Create.ForeignKey("FK_monitored_target_metrics_by_second_metric_bucket_id")
            .FromTable("monitored_target_metrics_by_second").ForeignColumn("metric_bucket_id")
            .ToTable("metric_buckets").PrimaryColumn("metric_bucket_id");

        Create.ForeignKey("FK_monitored_target_metrics_by_second_severity_type_id")
            .FromTable("monitored_target_metrics_by_second").ForeignColumn("severity_type_id")
            .ToTable("severity_types").PrimaryColumn("severity_type_id");

        /*
         * Create [dbo].[monitored_target_metrics_by_minute]
         *  - metrics measured against targets, aggregared per minute
         */
        Create.Table("monitored_target_metrics_by_minute")
            .WithColumn("monitored_target_id").AsInt32().PrimaryKey()
            .WithColumn("metric_id").AsInt32().PrimaryKey()
            .WithColumn("metric_bucket_id").AsInt32().PrimaryKey()
            .WithColumn("measured_at_utc").AsDateTime2().PrimaryKey()
            .WithColumn("value").AsDouble().NotNullable()
            .WithColumn("severity_type_id").AsInt32().NotNullable();

        Create.Index("IX_monitored_target_metrics_by_minute_metric_id_metric_bucket_id")
            .OnTable("monitored_target_metrics_by_minute")
            .OnColumn("metric_id").Ascending()
            .OnColumn("metric_bucket_id").Ascending()
            .OnColumn("measured_at_utc").Ascending();

        Create.Index("IX_monitored_target_metrics_by_minute_metric_id_measured_at_utc")
            .OnTable("monitored_target_metrics_by_minute")
            .OnColumn("metric_id").Ascending()
            .OnColumn("measured_at_utc").Ascending();

        Create.ForeignKey("FK_monitored_target_metrics_by_minute_metric_bucket_id")
            .FromTable("monitored_target_metrics_by_minute").ForeignColumn("metric_bucket_id")
            .ToTable("metric_buckets").PrimaryColumn("metric_bucket_id");

        Create.ForeignKey("FK_monitored_target_metrics_by_minute_severity_type_id")
            .FromTable("monitored_target_metrics_by_minute").ForeignColumn("severity_type_id")
            .ToTable("severity_types").PrimaryColumn("severity_type_id");

        /*
         * Create [dbo].[monitored_target_metrics_by_hour]
         *  - metrics measured against targets, aggregared per hour
         */
        Create.Table("monitored_target_metrics_by_hour")
            .WithColumn("monitored_target_id").AsInt32().PrimaryKey()
            .WithColumn("metric_id").AsInt32().PrimaryKey()
            .WithColumn("metric_bucket_id").AsInt32().PrimaryKey()
            .WithColumn("measured_at_utc").AsDateTime2().PrimaryKey()
            .WithColumn("value").AsDouble().NotNullable()
            .WithColumn("severity_type_id").AsInt32().NotNullable();

        Create.Index("IX_monitored_target_metrics_by_hour_metric_id_metric_bucket_id_m")
            .OnTable("monitored_target_metrics_by_hour")
            .OnColumn("metric_id").Ascending()
            .OnColumn("metric_bucket_id").Ascending()
            .OnColumn("measured_at_utc").Ascending();

        Create.Index("IX_monitored_target_metrics_by_hour_metric_id_measured_at_utc")
            .OnTable("monitored_target_metrics_by_hour")
            .OnColumn("metric_id").Ascending()
            .OnColumn("measured_at_utc").Ascending();

        Create.ForeignKey("FK_monitored_target_metrics_by_hour_metric_bucket_id")
            .FromTable("monitored_target_metrics_by_hour").ForeignColumn("metric_bucket_id")
            .ToTable("metric_buckets").PrimaryColumn("metric_bucket_id");

        Create.ForeignKey("FK_monitored_target_metrics_by_hour_severity_type_id")
            .FromTable("monitored_target_metrics_by_hour").ForeignColumn("severity_type_id")
            .ToTable("severity_types").PrimaryColumn("severity_type_id");

        /*
         * Create [dbo].[monitored_target_metrics_by_day]
         *  - metrics measured against targets, aggregared per day
         */
        Create.Table("monitored_target_metrics_by_day")
            .WithColumn("monitored_target_id").AsInt32().PrimaryKey()
            .WithColumn("metric_id").AsInt32().PrimaryKey()
            .WithColumn("metric_bucket_id").AsInt32().PrimaryKey()
            .WithColumn("measured_at_utc").AsDateTime2().PrimaryKey()
            .WithColumn("value").AsDouble().NotNullable()
            .WithColumn("severity_type_id").AsInt32().NotNullable();

        Create.Index("IX_monitored_target_metrics_by_day_metric_id_metric_bucket_id_me")
            .OnTable("monitored_target_metrics_by_day")
            .OnColumn("metric_id").Ascending()
            .OnColumn("metric_bucket_id").Ascending()
            .OnColumn("measured_at_utc").Ascending();

        Create.Index("IX_monitored_target_metrics_by_day_metric_id_measured_at_utc")
            .OnTable("monitored_target_metrics_by_day")
            .OnColumn("metric_id").Ascending()
            .OnColumn("measured_at_utc").Ascending();

        Create.ForeignKey("FK_monitored_target_metrics_by_day_metric_bucket_id")
            .FromTable("monitored_target_metrics_by_day").ForeignColumn("metric_bucket_id")
            .ToTable("metric_buckets").PrimaryColumn("metric_bucket_id");

        Create.ForeignKey("FK_monitored_target_metrics_by_day_severity_type_id")
            .FromTable("monitored_target_metrics_by_day").ForeignColumn("severity_type_id")
            .ToTable("severity_types").PrimaryColumn("severity_type_id");
    }

    public override void Down()
    {
        Delete.Table("monitored_target_metrics_by_day");

        Delete.Table("monitored_target_metrics_by_hour");

        Delete.Table("monitored_target_metrics_by_minute");

        Delete.Table("monitored_target_metrics_by_second");

        Delete.Table("metric_buckets");

        Delete.Table("severity_types");
    }
}
