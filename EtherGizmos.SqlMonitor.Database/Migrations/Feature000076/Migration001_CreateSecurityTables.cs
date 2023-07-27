using EtherGizmos.SqlMonitor.Database.Core;
using EtherGizmos.SqlMonitor.Database.Extensions;
using FluentMigrator;
using System.Data;

namespace EtherGizmos.SqlMonitor.Database.Migrations.Feature000076;

[CreatedAt(year: 2023, month: 07, day: 15, hour: 12, minute: 00, description: "Create security tables", trackingId: 54)]
public class Migration001_CreateSecurityTables : AutoReversingMigration
{
    public override void Up()
    {
        /*
         * Create [dbo].[principal_securable_permissions]
         *  - permissions (grant/deny) for security entities
         */
        Create.Table("principal_securable_permissions")
            .WithColumn("principal_id").AsGuid().PrimaryKey()
            .WithColumn("securable_id").AsAnsiString(20).PrimaryKey()
            .WithColumn("permission_id").AsAnsiString(20).PrimaryKey()
            .WithAuditColumns()
            .WithColumn("is_grant").AsBoolean().NotNullable();

        Create.ForeignKey("FK_principal_securable_permissions_principal_id")
            .FromTable("principal_securable_permissions").ForeignColumn("principal_id")
            .ToTable("principals").PrimaryColumn("principal_id")
            .OnUpdate(Rule.Cascade)
            .OnDelete(Rule.Cascade);
        //If a principal is modified, update references to its grants.
        //If a principal is deleted, remove its associated grants.

        Create.ForeignKey("FK_principal_securable_permissions_securable_id")
            .FromTable("principal_securable_permissions").ForeignColumn("securable_id")
            .ToTable("securables").PrimaryColumn("securable_id")
            .OnUpdate(Rule.Cascade)
            .OnDelete(Rule.Cascade);
        //If a securable is modified, update references to its grants.
        //If a securable is deleted, remove its associated grants.

        Create.ForeignKey("FK_principal_securable_permissions_permission_id")
            .FromTable("principal_securable_permissions").ForeignColumn("permission_id")
            .ToTable("permissions").PrimaryColumn("permission_id")
            .OnUpdate(Rule.Cascade)
            .OnDelete(Rule.Cascade);
        //If a permission is modified, update references to its grants.
        //If a permission is deleted, remove its associated grants.
    }
}
