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
         * Create [dbo].[script_interpreters]
         *  - types of scripts supported by their application, as well as how to run them
         */
        Create.Table("script_interpreters")
            .WithColumn("script_interpreter_id").AsInt32().PrimaryKey().Identity()
            .WithAuditColumns()
            .WithColumn("name").AsString(200).NotNullable()
            .WithColumn("description").AsString(int.MaxValue).Nullable()
            .WithColumn("command").AsString(int.MaxValue).NotNullable()
            .WithColumn("arguments").AsString(int.MaxValue).NotNullable()
            .WithColumn("is_soft_deleted").AsBoolean().NotNullable().WithDefaultValue(false)
            .WithColumn("securable_id").AsInt32().NotNullable();

        Create.ForeignKey("FK_script_interpreters_securable_id")
            .FromTable("script_interpreters").ForeignColumn("securable_id")
            .ToTable("securables").PrimaryColumn("securable_id");

        Create.Index("IX_script_interpreters_securable_id")
            .OnTable("script_interpreters")
            .OnColumn("securable_id");

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

        /*
         * Create [dbo].[script_variants]
         *  - actual script content and an indication of how to run them
         */
        Create.Table("script_variants")
            .WithColumn("script_variant_id").AsInt32().PrimaryKey().Identity()
            .WithAuditColumns()
            .WithColumn("script_id").AsGuid().NotNullable()
            .WithColumn("script_interpreter_id").AsInt32().NotNullable()
            .WithColumn("script_text").AsString(int.MaxValue).NotNullable()
            .WithColumn("timestamp_utc_name").AsString(500)
            .WithColumn("bucket_name").AsString(500);

        Create.ForeignKey("FK_script_variants_script_id")
            .FromTable("script_variants").ForeignColumn("script_id")
            .ToTable("scripts").PrimaryColumn("script_id");

        Create.Index("IX_script_variants_script_id")
            .OnTable("script_variants")
            .OnColumn("script_id");

        Create.ForeignKey("FK_script_variants_script_interpreter_id")
            .FromTable("script_variants").ForeignColumn("script_interpreter_id")
            .ToTable("script_interpreters").PrimaryColumn("script_interpreter_id");

        Create.Index("IX_script_variants_script_interpreter_id")
            .OnTable("script_variants")
            .OnColumn("script_interpreter_id");
    }
}
