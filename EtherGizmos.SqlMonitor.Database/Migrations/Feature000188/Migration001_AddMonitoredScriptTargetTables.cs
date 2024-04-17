using EtherGizmos.SqlMonitor.Database.Core;
using EtherGizmos.SqlMonitor.Database.Extensions;
using FluentMigrator;

namespace EtherGizmos.SqlMonitor.Database.Migrations.Feature000188;

[CreatedAt(year: 2024, month: 03, day: 22, hour: 06, minute: 00, description: "Create monitored script target tables", trackingId: 188)]
public class Migration001_AddMonitoredScriptTargetTables : Migration
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

        Execute.Sql(@"create trigger [TR_ssh_authentication_types_audit]
on [ssh_authentication_types]
after insert, update
as
begin
    set nocount on;

    declare @RecordId int;

    --Get the id of the inserted record
    select @RecordId = inserted.ssh_authentication_type_id
        from inserted;

    --Set the last modified time of the record
    update [ssh_authentication_types]
      set [modified_at_utc] = getutcdate()
      where [ssh_authentication_type_id] = @RecordId;
end;");

        /*
         * Create [dbo].[winrm_authentication_types]
         *  - the types of authentication that can be used for WinRM
         */
        Create.Table("winrm_authentication_types")
            .WithColumn("winrm_authentication_type_id").AsInt32().PrimaryKey()
            .WithAuditColumns()
            .WithColumn("name").AsString(200).NotNullable()
            .WithColumn("description").AsString(int.MaxValue).Nullable();

        Execute.Sql(@"create trigger [TR_winrm_authentication_types_audit]
on [winrm_authentication_types]
after insert, update
as
begin
    set nocount on;

    declare @RecordId int;

    --Get the id of the inserted record
    select @RecordId = inserted.winrm_authentication_type_id
        from inserted;

    --Set the last modified time of the record
    update [winrm_authentication_types]
      set [modified_at_utc] = getutcdate()
      where [winrm_authentication_type_id] = @RecordId;
end;");

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

        Execute.Sql(@"create trigger [TR_monitored_script_targets_audit]
on [monitored_script_targets]
after insert, update
as
begin
    set nocount on;

    declare @RecordId int;

    --Get the id of the inserted record
    select @RecordId = inserted.monitored_script_target_id
        from inserted;

    --Set the last modified time of the record
    update [monitored_script_targets]
      set [modified_at_utc] = getutcdate()
      where [monitored_script_target_id] = @RecordId;
end;");

        Execute.Sql(@"create trigger [TR_monitored_script_targets_securable_id]
on [monitored_script_targets]
after insert, update, delete
as
begin
    set nocount on;

    declare @RecordId int;
    declare @SecurableId int;
    declare @SecurableTypeId int = 150;

    --Handle inserts/updates
    if exists ( select 1 from inserted )
    begin
        --Get the id of the inserted record
        select @RecordId = inserted.monitored_script_target_id,
          @SecurableId = inserted.securable_id
          from inserted;

        if @SecurableId is null
        begin
            --Insert a new row into [securables]
            insert into [securables] ( [securable_type_id] )
              values ( @SecurableTypeId );

            --Get the generated id
            select @SecurableId = scope_identity();

            --Update the [monitored_script_targets] table with the generated [securable_id]
            update [monitored_script_targets]
              set [securable_id] = @SecurableId
              where [monitored_script_target_id] = @RecordId;
        end;
    end
    --Handle deletes
    else
    begin
        --Get the id of the deleted record
        select @RecordId = deleted.monitored_script_target_id,
          @SecurableId = deleted.securable_id
          from deleted;

        --Delete the [securable_id] from the [securables] table
        delete from [securables]
          where [securable_id] = @SecurableId;
    end;
end;");
    }

    public override void Down()
    {
        Delete.Table("monitored_script_targets");

        Delete.Table("ssh_authentication_types");

        Delete.Table("winrm_authentication_types");

        Delete.Table("monitored_targets");
    }
}
