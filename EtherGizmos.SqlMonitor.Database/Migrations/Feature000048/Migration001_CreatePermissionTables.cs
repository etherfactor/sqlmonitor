using EtherGizmos.SqlMonitor.Database.Core;
using EtherGizmos.SqlMonitor.Database.Extensions;
using FluentMigrator;

namespace EtherGizmos.SqlMonitor.Database.Migrations.Feature000048;

[CreatedAt(year: 2023, month: 07, day: 03, hour: 21, minute: 00, description: "Create permission tables", trackingId: 48)]
public class Migration001_CreatePermissionTables : AutoReversingMigration
{
    public override void Up()
    {
        /*
         * Create [dbo].[permissions]
         *  - actions that can be taken on secured entities
         */
        Create.Table("permissions")
            .WithColumn("permission_id").AsAnsiString(20).PrimaryKey()
            .WithAuditColumns()
            .WithColumn("name").AsAnsiString(100).NotNullable()
            .WithColumn("description").AsAnsiString(200).Nullable();
    }
}
