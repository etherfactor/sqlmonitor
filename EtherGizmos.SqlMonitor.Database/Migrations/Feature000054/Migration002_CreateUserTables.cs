using EtherGizmos.SqlMonitor.Database.Core;
using EtherGizmos.SqlMonitor.Database.Extensions;
using FluentMigrator;
using System.Data;

namespace EtherGizmos.SqlMonitor.Database.Migrations.Feature000054;

[CreatedAt(year: 2023, month: 07, day: 14, hour: 21, minute: 15, description: "Create user tables", trackingId: 54)]
public class Migration002_CreateUserTables : AutoReversingMigration
{
    public override void Up()
    {
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
            .ToTable("principals").PrimaryColumn("principal_id")
            .OnDelete(Rule.Cascade);
    }
}
