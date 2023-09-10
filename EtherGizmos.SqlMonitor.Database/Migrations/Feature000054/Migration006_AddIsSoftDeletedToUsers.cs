using EtherGizmos.SqlMonitor.Database.Core;
using FluentMigrator;

namespace EtherGizmos.SqlMonitor.Database.Migrations.Feature000054;

[CreatedAt(year: 2023, month: 08, day: 05, hour: 11, minute: 40, description: "Add is_soft_deleted to users", trackingId: 54)]
public class Migration006_AddIsSoftDeletedToUsers : Migration
{
    public override void Up()
    {
        /*
         * Add [is_soft_deleted] to [dbo].[users]
         *  - defaults to false to support existing records
         */
        Alter.Table("users")
            .AddColumn("is_soft_deleted").AsBoolean().NotNullable().WithDefaultValue(false);
    }

    public override void Down()
    {
        /*
         * Remove [is_soft_deleted] from [dbo].[users]
         */
        Delete.Column("is_soft_deleted")
            .FromTable("users");
    }
}
