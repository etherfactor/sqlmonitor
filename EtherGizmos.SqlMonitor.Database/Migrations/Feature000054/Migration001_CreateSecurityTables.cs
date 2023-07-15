using EtherGizmos.SqlMonitor.Database.Core;
using EtherGizmos.SqlMonitor.Database.Extensions;
using FluentMigrator;
using System.Data;

namespace EtherGizmos.SqlMonitor.Database.Migrations.Feature000054;

[CreatedAt(year: 2023, month: 07, day: 14, hour: 21, minute: 00, description: "Create security tables", trackingId: 54)]
public class Migration001_CreateSecurityTables : AutoReversingMigration
{
    public override void Up()
    {
        /*
         * Map between [dbo].[securable_permissions]
         *  - [dbo].[securables]
         *  - [dbo].[permissions]
         */
        Create.ForeignKey("FK_securable_permissions_securable_id")
            .FromTable("securable_permissions").ForeignColumn("securable_id")
            .ToTable("securables").PrimaryColumn("securable_id")
            .OnUpdate(Rule.Cascade)
            .OnDelete(Rule.Cascade);
        //If a securable is modified, update references to its permissions.
        //If a securable is deleted, remove its associated permissions.

        Create.ForeignKey("FK_securable_permissions_permission_id")
            .FromTable("securable_permissions").ForeignColumn("permission_id")
            .ToTable("permissions").PrimaryColumn("permission_id")
            .OnUpdate(Rule.Cascade)
            .OnDelete(Rule.Cascade);
        //If a permission is modified, update references to its securables.
        //If a permission is deleted, remove its associated securables.

        /*
         * Create [dbo].[principal_types]
         *  - entity types associated with principals
         */
        Create.Table("principal_types")
            .WithColumn("principal_type_id").AsAnsiString(20).PrimaryKey()
            .WithAuditColumns()
            .WithColumn("name").AsString(200).NotNullable()
            .WithColumn("description").AsString(int.MaxValue).Nullable();

        /*
         * Create [dbo].[principals]
         *  - security entities associated with other entities
         */
        Create.Table("principals")
            .WithColumn("principal_id").AsGuid().PrimaryKey()
            .WithAuditColumns()
            .WithColumn("principal_type_id").AsAnsiString(20).NotNullable()
            .WithColumn("system_id").AsGuid().Nullable();
        //For system-generated records, use a pre-defined system GUID with cascade deletes, and on migration down, delete
        //the pre-known system GUID to delete the record.

        Create.ForeignKey("FK_principals_principal_type_id")
            .FromTable("principals").ForeignColumn("principal_type_id")
            .ToTable("principal_types").PrimaryColumn("principal_type_id");
    }
}
