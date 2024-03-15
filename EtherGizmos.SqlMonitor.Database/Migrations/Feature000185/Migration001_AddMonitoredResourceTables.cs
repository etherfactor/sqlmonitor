using EtherGizmos.SqlMonitor.Database.Core;
using EtherGizmos.SqlMonitor.Database.Extensions;
using FluentMigrator;

namespace EtherGizmos.SqlMonitor.Database.Migrations.Feature000185;

[CreatedAt(year: 2024, month: 03, day: 13, hour: 20, minute: 00, description: "Create monitored resource tables", trackingId: 185)]
public class Migration001_AddMonitoredResourceTables : AutoReversingMigration
{
    public override void Up()
    {
        /* 
         * Create [dbo].[monitored_resources]
         *  - resources in systems being monitored for metrics, alongside systems and environments
         */
        Create.Table("monitored_resources")
            .WithColumn("monitored_resource_id").AsGuid().PrimaryKey().WithDefault(SystemMethods.NewGuid)
            .WithAuditColumns()
            .WithColumn("name").AsString(200).NotNullable()
            .WithColumn("description").AsString(int.MaxValue).Nullable()
            .WithColumn("is_active").AsBoolean().NotNullable().WithDefaultValue(true)
            .WithColumn("is_soft_deleted").AsBoolean().NotNullable().WithDefaultValue(false)
            .WithColumn("securable_id").AsInt32().NotNullable();

        Create.ForeignKey("FK_monitored_resources")
            .FromTable("monitored_resources").ForeignColumn("securable_id")
            .ToTable("securables").PrimaryColumn("securable_id");

        Create.Index("IX_monitored_resources")
            .OnTable("monitored_resources")
            .OnColumn("securable_id");
    }
}
