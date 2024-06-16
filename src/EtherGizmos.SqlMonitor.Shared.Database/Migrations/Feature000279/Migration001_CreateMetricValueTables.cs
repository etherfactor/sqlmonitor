using FluentMigrator;

namespace EtherGizmos.SqlMonitor.Shared.Database.Migrations.Feature000279;

public class Migration001_CreateMetricValueTables : Migration
{
    public override void Up()
    {
        Create.Table("monitored_target_metrics_by_second")
            .WithColumn("by_second_id").AsInt32().PrimaryKey().Identity()
            .WithColumn("monitored_target_id").AsInt32().NotNullable()
            .WithColumn("metric_id").AsInt32().NotNullable()
            .WithColumn("measured_at_utc").AsDateTime2().NotNullable()
            .WithColumn("value").AsDouble().NotNullable();

        Create.Table("monitored_target_metrics_by_minute")
            .WithColumn("by_minute_id").AsInt32().PrimaryKey().Identity()
            .WithColumn("monitored_target_id").AsInt32().NotNullable()
            .WithColumn("metric_id").AsInt32().NotNullable()
            .WithColumn("measured_at_utc").AsDateTime2().NotNullable()
            .WithColumn("value").AsDouble().NotNullable();

        Create.Table("monitored_target_metrics_by_hour")
            .WithColumn("by_hour_id").AsInt32().PrimaryKey().Identity()
            .WithColumn("monitored_target_id").AsInt32().NotNullable()
            .WithColumn("metric_id").AsInt32().NotNullable()
            .WithColumn("measured_at_utc").AsDateTime2().NotNullable()
            .WithColumn("value").AsDouble().NotNullable();

        Create.Table("monitored_target_metrics_by_day")
            .WithColumn("by_day_id").AsInt32().PrimaryKey().Identity()
            .WithColumn("monitored_target_id").AsInt32().NotNullable()
            .WithColumn("metric_id").AsInt32().NotNullable()
            .WithColumn("measured_at_utc").AsDateTime2().NotNullable()
            .WithColumn("value").AsDouble().NotNullable();
    }

    public override void Down()
    {
        throw new NotImplementedException();
    }
}
