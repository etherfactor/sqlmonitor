using EtherGizmos.SqlMonitor.Database.Core;
using EtherGizmos.SqlMonitor.Database.Extensions;
using FluentMigrator;

namespace EtherGizmos.SqlMonitor.Database.Migrations.Feature000230;

[CreatedAt(year: 2024, month: 03, day: 18, hour: 20, minute: 00, description: "Create script tables", trackingId: 230)]
public class Migration001_AddScriptTables : AutoReversingMigration
{
    public override void Up()
    {
        /* 
         * Create [dbo].[scripts]
         *  - script definitions being run against servers, without the actual script content
         */
        Create.Table("scripts")
            .WithColumn("script_id").AsGuid().PrimaryKey().WithDefault(SystemMethods.NewGuid)
            .WithAuditColumns()
            .WithColumn("name").AsString(200).NotNullable()
            .WithColumn("description").AsString(int.MaxValue).Nullable()
            .WithColumn("run_frequency").AsTime().NotNullable()
            .WithColumn("last_run_at_utc").AsDateTime2().Nullable()
            .WithColumn("is_active").AsBoolean().NotNullable().WithDefaultValue(true)
            .WithColumn("is_soft_deleted").AsBoolean().NotNullable().WithDefaultValue(false)
            .WithColumn("securable_id").AsInt32().NotNullable();

        Create.ForeignKey("FK_scripts_securable_id")
            .FromTable("scripts").ForeignColumn("securable_id")
            .ToTable("securables").PrimaryColumn("securable_id");

        Create.Index("IX_scripts_securable_id")
            .OnTable("scripts")
            .OnColumn("securable_id");
    }
}
