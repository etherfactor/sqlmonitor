using EtherGizmos.SqlMonitor.Database.Core;
using EtherGizmos.SqlMonitor.Database.Extensions;
using FluentMigrator;

namespace EtherGizmos.SqlMonitor.Database.Migrations.Feature000184;

[CreatedAt(year: 2024, month: 03, day: 13, hour: 18, minute: 30, description: "Create monitored system tables", trackingId: 184)]
public class Migration002_AddMonitoredSystemTables : AutoReversingMigration
{
    public override void Up()
    {
        /* 
         * Create [dbo].[monitored_systems]
         *  - systems being monitored for metrics, alongside resources and environments
         */
        Create.Table("monitored_systems")
            .WithColumn("monitored_system_id").AsGuid().PrimaryKey().WithDefault(SystemMethods.NewGuid)
            .WithAuditColumns()
            .WithColumn("name").AsString(200).NotNullable()
            .WithColumn("description").AsString(int.MaxValue).Nullable()
            .WithColumn("is_active").AsBoolean().NotNullable().WithDefaultValue(true)
            .WithColumn("is_soft_deleted").AsBoolean().NotNullable().WithDefaultValue(false)
            .WithColumn("securable_id").AsInt32().NotNullable();

        Create.ForeignKey("FK_monitored_systems_securable_id")
            .FromTable("monitored_systems").ForeignColumn("securable_id")
            .ToTable("securables").PrimaryColumn("securable_id");

        Create.Index("IX_monitored_systems_securable_id")
            .OnTable("monitored_systems")
            .OnColumn("securable_id");
    }
}
