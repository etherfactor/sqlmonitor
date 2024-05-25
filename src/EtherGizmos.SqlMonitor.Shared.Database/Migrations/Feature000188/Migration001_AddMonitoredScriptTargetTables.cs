using EtherGizmos.SqlMonitor.Shared.Database.Core;
using EtherGizmos.SqlMonitor.Shared.Database.Extensions;
using System.Data;

namespace EtherGizmos.SqlMonitor.Shared.Database.Migrations.Feature000188;

[CreatedAt(year: 2024, month: 03, day: 22, hour: 06, minute: 00, description: "Create monitored script target tables", trackingId: 188)]
public class Migration001_AddMonitoredScriptTargetTables : MigrationExtension
{
    public override void Up()
    {
        /*
         * Create [dbo].[monitored_targets]
         *  - combinations of systems, resources, and environments being monitored
         */
        Create.Table("monitored_targets")
            .WithColumn("monitored_target_id").AsInt32().PrimaryKey().Identity()
            .WithColumn("monitored_system_id").AsGuid().NotNullable()
            .WithColumn("monitored_resource_id").AsGuid().NotNullable()
            .WithColumn("monitored_environment_id").AsGuid().NotNullable();

        /*
         * Create [dbo].[ssh_authentication_types]
         *  - the types of authentication that can be used for SSH
         */
        Create.Table("ssh_authentication_types")
            .WithColumn("ssh_authentication_type_id").AsInt32().PrimaryKey()
            .WithAuditColumns()
            .WithColumn("name").AsString(200).NotNullable()
            .WithColumn("description").AsString(int.MaxValue).Nullable();

        this.AddAuditTriggerV1("ssh_authentication_types",
            ("ssh_authentication_type_id", DbType.Int32));

        /*
         * Create [dbo].[winrm_authentication_types]
         *  - the types of authentication that can be used for WinRM
         */
        Create.Table("winrm_authentication_types")
            .WithColumn("winrm_authentication_type_id").AsInt32().PrimaryKey()
            .WithAuditColumns()
            .WithColumn("name").AsString(200).NotNullable()
            .WithColumn("description").AsString(int.MaxValue).Nullable();

        this.AddAuditTriggerV1("winrm_authentication_types",
            ("winrm_authentication_type_id", DbType.Int32));

        /*
         * Create [dbo].[monitored_script_targets]
         *  - for a given target, a server & directory being targeted with monitoring scripts
         */
        Create.Table("monitored_script_targets")
            .WithColumn("monitored_script_target_id").AsInt32().PrimaryKey().Identity()
            .WithAuditColumns()
            .WithColumn("monitored_target_id").AsInt32().NotNullable()
            .WithColumn("script_interpreter_id").AsInt32().NotNullable()
            .WithColumn("host").AsString(255).NotNullable()
            .WithColumn("port").AsInt32().Nullable()
            .WithColumn("run_in_path").AsString(int.MaxValue).NotNullable()
            .WithColumn("securable_id").AsInt32().Nullable()
            .WithColumn("ssh_authentication_type_id").AsInt32().Nullable()
            .WithColumn("ssh_username").AsString(255).Nullable()
            .WithColumn("ssh_password").AsString(int.MaxValue).Nullable()
            .WithColumn("ssh_private_key").AsString(int.MaxValue).Nullable()
            .WithColumn("ssh_private_key_password").AsString(int.MaxValue).Nullable()
            .WithColumn("winrm_authentication_type_id").AsInt32().Nullable()
            .WithColumn("winrm_use_ssl").AsBoolean().Nullable()
            .WithColumn("winrm_username").AsString(255).Nullable()
            .WithColumn("winrm_password").AsString(int.MaxValue).Nullable();

        Create.ForeignKey("FK_monitored_script_targets_monitored_target_id")
            .FromTable("monitored_script_targets").ForeignColumn("monitored_target_id")
            .ToTable("monitored_targets").PrimaryColumn("monitored_target_id");

        Create.Index("IX_monitored_script_targets_monitored_target_id")
            .OnTable("monitored_script_targets")
            .OnColumn("monitored_target_id");

        Create.ForeignKey("FK_monitored_script_targets_script_interpreter_id")
            .FromTable("monitored_script_targets").ForeignColumn("script_interpreter_id")
            .ToTable("script_interpreters").PrimaryColumn("script_interpreter_id");

        Create.Index("IX_monitored_script_targets_script_interpreter_id")
            .OnTable("monitored_script_targets")
            .OnColumn("script_interpreter_id");

        Create.ForeignKey("FK_monitored_script_targets_securable_id")
            .FromTable("monitored_script_targets").ForeignColumn("securable_id")
            .ToTable("securables").PrimaryColumn("securable_id");

        Create.Index("IX_monitored_script_targets_securable_id")
            .OnTable("monitored_script_targets")
            .OnColumn("securable_id");

        Create.ForeignKey("IX_monitored_script_targets_ssh_authentication_type_id")
            .FromTable("monitored_script_targets").ForeignColumn("ssh_authentication_type_id")
            .ToTable("ssh_authentication_types").PrimaryColumn("ssh_authentication_type_id");

        Create.ForeignKey("IX_monitored_script_targets_winrm_authentication_type_id")
            .FromTable("monitored_script_targets").ForeignColumn("winrm_authentication_type_id")
            .ToTable("winrm_authentication_types").PrimaryColumn("winrm_authentication_type_id");

        this.AddAuditTriggerV1("monitored_script_targets",
            ("monitored_script_target_id", DbType.Int32));

        this.AddSecurableTriggerV1("monitored_script_targets", "securable_id", 180,
            ("monitored_script_target_id", DbType.Int32));
    }

    public override void Down()
    {
        Delete.Table("monitored_script_targets");

        Delete.Table("ssh_authentication_types");

        Delete.Table("winrm_authentication_types");

        Delete.Table("monitored_targets");
    }
}
