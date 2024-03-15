using EtherGizmos.SqlMonitor.Database.Core;
using EtherGizmos.SqlMonitor.Database.Extensions;
using FluentMigrator;

namespace EtherGizmos.SqlMonitor.Database.Migrations.Feature000186;

[CreatedAt(year: 2024, month: 03, day: 13, hour: 21, minute: 30, description: "Create monitored environment tables", trackingId: 186)]
public class Migration001_AddMonitoredEnvironmentTables : AutoReversingMigration
{
    public override void Up()
    {
        /* 
         * Create [dbo].[monitored_environments]
         *  - environments of systems being monitored for metrics, alongside systems and resources
         */
        Create.Table("monitored_environments")
            .WithColumn("monitored_environment_id").AsGuid().PrimaryKey().WithDefault(SystemMethods.NewGuid)
            .WithAuditColumns()
            .WithColumn("name").AsString(200).NotNullable()
            .WithColumn("description").AsString(int.MaxValue).Nullable()
            .WithColumn("is_active").AsBoolean().NotNullable().WithDefaultValue(true)
            .WithColumn("is_soft_deleted").AsBoolean().NotNullable().WithDefaultValue(false)
            .WithColumn("securable_id").AsInt32().NotNullable();

        Create.ForeignKey("FK_monitored_environments")
            .FromTable("monitored_environments").ForeignColumn("securable_id")
            .ToTable("securables").PrimaryColumn("securable_id");

        Create.Index("IX_monitored_environments")
            .OnTable("monitored_environments")
            .OnColumn("securable_id");
    }
}
