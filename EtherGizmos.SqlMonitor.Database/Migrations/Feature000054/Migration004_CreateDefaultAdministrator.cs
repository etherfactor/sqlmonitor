using EtherGizmos.SqlMonitor.Database.Core;
using FluentMigrator;

namespace EtherGizmos.SqlMonitor.Database.Migrations.Feature000054;

[CreatedAt(year: 2023, month: 07, day: 14, hour: 21, minute: 45, description: "Load default administrator", trackingId: 54)]
public class Migration004_CreateDefaultAdministrator : Migration
{
    private Guid StaticSystemId { get; } = new Guid("8cec7d7d-2c4a-47c2-a88b-a9cbda80de19");

    public override void Up()
    {
        var principalId = Guid.NewGuid();

        /*
         * Load [dbo].[principals]
         *  - default administrator's security principal
         */
        Insert.IntoTable("principals")
            .Row(new { principal_id = principalId, principal_type_id = "USER", system_id = StaticSystemId });

        /*
         * Load [dbo].[users]
         *  - default administrator
         */
        Insert.IntoTable("users")
            .Row(new { username = "admin", password = "$2a$10$K7N2HU0GsX/YpNi1K4dZoe3sylWrziT668gUM0vNIK/ZBOOSUc/9q", name = "Administrator", is_administrator = true, principal_id = principalId });
    }

    public override void Down()
    {
        /*
         * Revert [dbo].[principals]
         *  - cascade deletes administrator user
         */
        Delete.FromTable("principals")
            .Row(new { system_id = StaticSystemId });
    }
}
