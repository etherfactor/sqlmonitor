using EtherGizmos.SqlMonitor.Database.Core;
using EtherGizmos.SqlMonitor.Database.Extensions;
using FluentMigrator;

namespace EtherGizmos.SqlMonitor.Database.Migrations.FeatureTBD;

[CreatedAt(year: 2023, month: 07, day: 23, hour: 17, minute: 00, description: "Create instance tables", trackingId: -1)] //TODO: add tracking
public class Migration001_CreateInstanceTables : AutoReversingMigration
{
    public override void Up()
    {
        /*
         * Create [dbo].[instances]
         *  - SQL instances being monitored
         */
        Create.Table("instances")
            .WithColumn("instance_id").AsGuid().PrimaryKey().WithDefault(SystemMethods.NewGuid)
            .WithAuditColumns()
            .WithColumn("name").AsString(100).NotNullable()
            .WithColumn("description").AsString(int.MaxValue).Nullable()
            .WithColumn("is_active").AsBoolean().NotNullable()
            .WithColumn("is_soft_deleted").AsBoolean().NotNullable()
            .WithColumn("address").AsString(255).NotNullable()
            .WithColumn("port").AsInt16().Nullable()
            .WithColumn("database").AsString(128).Nullable();
    }
}
