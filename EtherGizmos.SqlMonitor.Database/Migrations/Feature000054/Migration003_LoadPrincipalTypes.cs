using EtherGizmos.SqlMonitor.Database.Core;
using FluentMigrator;

namespace EtherGizmos.SqlMonitor.Database.Migrations.Feature000054;

[CreatedAt(year: 2023, month: 07, day: 14, hour: 21, minute: 30, description: "Load principal types", trackingId: 54)]
public class Migration003_LoadPrincipalTypes : Migration
{
    public override void Up()
    {
        /*
         * Load [dbo].[principal_types]
         *  - principal type for users
         */
        Insert.IntoTable("principal_types")
            .Row(new { principal_type_id = "USER", name = "User", description = "Holds the security grants for a single user." });
    }

    public override void Down()
    {
        /*
         * Revert [dbo].[principal_types]
         *  - principal type for users
         */
        Delete.FromTable("principal_types")
            .Row(new { principal_type_id = "USER" });
    }
}
