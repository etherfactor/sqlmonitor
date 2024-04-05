﻿using EtherGizmos.SqlMonitor.Database.Core;
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
         * Create [dbo].[monitored_script_targets]
         *  - for a given target, a server & directory being targeted with monitoring scripts
         */
        Create.Table("monitored_script_targets")
            .WithColumn("monitored_script_target_id").AsInt32().PrimaryKey().Identity()
            .WithAuditColumns()
            .WithColumn("monitored_target_id").AsInt32().NotNullable()
            .WithColumn("script_interpreter_id").AsInt32().NotNullable()
            .WithColumn("host_name").AsString(255).NotNullable()
            .WithColumn("file_path").AsString(int.MaxValue).NotNullable()
            .WithColumn("securable_id").AsInt32().Nullable();

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

        Delete.Table("monitored_targets");
    }
}