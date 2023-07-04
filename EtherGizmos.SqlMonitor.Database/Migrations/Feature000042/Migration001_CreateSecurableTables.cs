using EtherGizmos.SqlMonitor.Database.Core;
using EtherGizmos.SqlMonitor.Database.Extensions;
using FluentMigrator;

namespace EtherGizmos.SqlMonitor.Database.Migrations.Feature000042;

[CreatedAt(year: 2023, month: 06, day: 21, hour: 09, minute: 00, description: "Create securable tables", trackingId: 42)]
public class Migration001_CreateSecurableTables : AutoReversingMigration
{
    public override void Up()
    {
        /*
         * Create [dbo].[securables]
         *  - entity types available for securing
         */
        Create.Table("securables")
            .WithColumn("securable_id").AsAnsiString(20).PrimaryKey()
            .WithAuditColumns()
            .WithColumn("name").AsAnsiString(100).NotNullable()
            .WithColumn("description").AsAnsiString(200).Nullable();
    }
}
