using EtherGizmos.SqlMonitor.Database.Core;
using EtherGizmos.SqlMonitor.Database.Extensions;
using FluentMigrator;

namespace EtherGizmos.SqlMonitor.Database.Migrations.Feature000054;

[CreatedAt(year: 2023, month: 07, day: 14, hour: 21, minute: 00, description: "Create user tables", trackingId: 54)]
public class Migration001_CreateUserTables : AutoReversingMigration
{
    public override void Up()
    {
        /*
         * Create [dbo].[localizations]
         *  - languages supported by the application
         */
        Create.Table("localizations")
            .WithColumn("localization_id").AsAnsiString(20).PrimaryKey()
            .WithAuditColumns()
            .WithColumn("name").AsString(200).NotNullable();

        /*
         * Create [dbo].[principal_types]
         *  - entity types associated with principals
         *  - localized
         */
        Create.Table("principal_types")
            .WithColumn("principal_type_id").AsAnsiString(20).PrimaryKey()
            .WithAuditColumns()
            .WithColumn("name").AsString(200).NotNullable()
            .WithColumn("description").AsString(int.MaxValue).Nullable();

        Create.Table("principal_type_localizations")
            .WithColumn("principal_type_id").AsAnsiString(20).PrimaryKey()
            .WithColumn("localization_id").AsAnsiString(20).PrimaryKey()
            .WithAuditColumns()
            .WithColumn("name").AsString(200).NotNullable()
            .WithColumn("description").AsString(int.MaxValue).Nullable();

        Create.ForeignKey("FK_principal_type_localizations_principal_id")
            .FromTable("principal_type_localizations").ForeignColumn("principal_id")
            .ToTable("principal_types").PrimaryColumn("principal_id");

        Create.ForeignKey("FK_principal_type_localizations_localization_id")
            .FromTable("principal_type_localizations").ForeignColumn("localization_id")
            .ToTable("localizations").PrimaryColumn("localization_id");

        /*
         * Create [dbo].[principals]
         *  - security entities associated with other entities
         */
        Create.Table("principals")
            .WithColumn("principal_id").AsGuid().PrimaryKey()
            .WithAuditColumns()
            .WithColumn("principal_type_id").AsString(20).NotNullable();

        Create.ForeignKey("FK_principals_principal_type_id")
            .FromTable("principals").ForeignColumn("principal_type_id")
            .ToTable("principal_types").PrimaryColumn("principal_type_id");

        /*
         * Create [dbo].[users]
         *  - users of the application
         */
        Create.Table("users")
            .WithColumn("user_id").AsGuid().PrimaryKey().WithDefault(SystemMethods.NewGuid)
            .WithAuditColumns()
            .WithColumn("username").AsString(100).NotNullable()
            .WithColumn("password").AsAnsiString(int.MaxValue).NotNullable()
            .WithColumn("email_address").AsAnsiString(320).Nullable()
            .WithColumn("name").AsString(150).NotNullable()
            .WithColumn("is_active").AsBoolean().NotNullable().WithDefaultValue(true)
            .WithColumn("is_administrator").AsBoolean().NotNullable().WithDefaultValue(false)
            .WithColumn("last_login_at_utc").AsDateTime2().Nullable()
            .WithColumn("principal_id").AsGuid().NotNullable();

        Create.ForeignKey("FK_users_principal_id")
            .FromTable("users").ForeignColumn("principal_id")
            .ToTable("principals").PrimaryColumn("principal_id");
    }
}
