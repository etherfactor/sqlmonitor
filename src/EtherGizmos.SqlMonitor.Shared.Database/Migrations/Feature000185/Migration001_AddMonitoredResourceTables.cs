using EtherGizmos.SqlMonitor.Shared.Database.Core;
using EtherGizmos.SqlMonitor.Shared.Database.Extensions;
using FluentMigrator;
using System.Data;

namespace EtherGizmos.SqlMonitor.Shared.Database.Migrations.Feature000185;

[CreatedAt(year: 2024, month: 03, day: 13, hour: 20, minute: 00, description: "Create monitored resource tables", trackingId: 185)]
public class Migration001_AddMonitoredResourceTables : MigrationExtension
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
            .WithColumn("securable_id").AsInt32().Nullable();

        Create.ForeignKey("FK_monitored_resources_securable_id")
            .FromTable("monitored_resources").ForeignColumn("securable_id")
            .ToTable("securables").PrimaryColumn("securable_id");

        Create.Index("IX_monitored_resources_securable_id")
            .OnTable("monitored_resources")
            .OnColumn("securable_id");

        this.AddAuditTriggerV1("monitored_resources",
            ("monitored_resource_id", DbType.Guid));

        this.AddSecurableTriggerV1("monitored_resources", "securable_id", 120,
            ("monitored_resource_id", DbType.Guid));
    }

    public override void Down()
    {
        Delete.Table("monitored_resources");
    }
}
