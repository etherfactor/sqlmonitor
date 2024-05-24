using EtherGizmos.SqlMonitor.Database.Core;
using EtherGizmos.SqlMonitor.Database.Extensions;
using FluentMigrator;
using System.Data;

namespace EtherGizmos.SqlMonitor.Database.Migrations.Feature000186;

[CreatedAt(year: 2024, month: 03, day: 13, hour: 21, minute: 30, description: "Create monitored environment tables", trackingId: 186)]
public class Migration001_AddMonitoredEnvironmentTables : MigrationExtension
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
            .WithColumn("securable_id").AsInt32().Nullable();

        Create.ForeignKey("FK_monitored_environments_securable_id")
            .FromTable("monitored_environments").ForeignColumn("securable_id")
            .ToTable("securables").PrimaryColumn("securable_id");

        Create.Index("IX_monitored_environments_securable_id")
            .OnTable("monitored_environments")
            .OnColumn("securable_id");

        this.AddAuditTriggerV1("monitored_environments",
            ("monitored_environment_id", DbType.Guid));

        this.AddSecurableTriggerV1("monitored_environments", "securable_id", 130,
            ("monitored_environment_id", DbType.Guid));
    }

    public override void Down()
    {
        Delete.Table("monitored_environments");
    }
}
