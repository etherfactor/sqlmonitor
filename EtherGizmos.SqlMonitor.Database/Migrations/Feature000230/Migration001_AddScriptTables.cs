using EtherGizmos.SqlMonitor.Database.Core;
using EtherGizmos.SqlMonitor.Database.Extensions;
using FluentMigrator;

namespace EtherGizmos.SqlMonitor.Database.Migrations.Feature000230;

[CreatedAt(year: 2024, month: 03, day: 18, hour: 20, minute: 00, description: "Create script tables", trackingId: 230)]
public class Migration001_AddScriptTables : Migration
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
            .WithColumn("system_id").AsGuid().NotNullable().WithDefault(SystemMethods.NewGuid)
            .WithColumn("securable_id").AsInt32().Nullable();

        Create.ForeignKey("FK_script_interpreters_securable_id")
            .FromTable("script_interpreters").ForeignColumn("securable_id")
            .ToTable("securables").PrimaryColumn("securable_id");

        Create.Index("IX_script_interpreters_securable_id")
            .OnTable("script_interpreters")
            .OnColumn("securable_id");

        Execute.Sql(@"create trigger [TR_script_interpreters_audit]
on [script_interpreters]
after insert, update
as
begin
    set nocount on;

    declare @RecordId int;

    --Get the id of the inserted record
    select @RecordId = inserted.script_interpreter_id
        from inserted;

    --Set the last modified time of the record
    update [script_interpreters]
      set [modified_at_utc] = getutcdate()
      where [script_interpreter_id] = @RecordId;
end;");

        Execute.Sql(@"create trigger [TR_script_interpreters_securable_id]
on [script_interpreters]
after insert, update, delete
as
begin
    set nocount on;

    declare @RecordId int;
    declare @SecurableId int;
    declare @SecurableTypeId int = 390;

    --Handle inserts/updates
    if exists ( select 1 from inserted )
    begin
        --Get the id of the inserted record
        select @RecordId = inserted.script_interpreter_id,
          @SecurableId = inserted.securable_id
          from inserted;

        if @SecurableId is null
        begin
            --Insert a new row into [securables]
            insert into [securables] ( [securable_type_id] )
              values ( @SecurableTypeId );

            --Get the generated id
            select @SecurableId = scope_identity();

            --Update the [script_interpreters] table with the generated [securable_id]
            update [script_interpreters]
              set [securable_id] = @SecurableId
              where [script_interpreter_id] = @RecordId;
        end;
    end
    --Handle deletes
    else
    begin
        --Get the id of the deleted record
        select @RecordId = deleted.script_interpreter_id,
          @SecurableId = deleted.securable_id
          from deleted;

        --Delete the [securable_id] from the [securables] table
        delete from [securables]
          where [securable_id] = @SecurableId;
    end;
end;");

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
            .WithColumn("system_id").AsGuid().NotNullable().WithDefault(SystemMethods.NewGuid)
            .WithColumn("securable_id").AsInt32().Nullable();

        Create.ForeignKey("FK_scripts_securable_id")
            .FromTable("scripts").ForeignColumn("securable_id")
            .ToTable("securables").PrimaryColumn("securable_id");

        Create.Index("IX_scripts_securable_id")
            .OnTable("scripts")
            .OnColumn("securable_id");

        Execute.Sql(@"create trigger [TR_scripts_audit]
on [scripts]
after insert, update
as
begin
    set nocount on;

    declare @RecordId uniqueidentifier;

    --Get the id of the inserted record
    select @RecordId = inserted.script_id
        from inserted;

    --Set the last modified time of the record
    update [scripts]
      set [modified_at_utc] = getutcdate()
      where [script_id] = @RecordId;
end;");

        Execute.Sql(@"create trigger [TR_scripts_securable_id]
on [scripts]
after insert, update, delete
as
begin
    set nocount on;

    declare @RecordId uniqueidentifier;
    declare @SecurableId int;
    declare @SecurableTypeId int = 390;

    --Handle inserts/updates
    if exists ( select 1 from inserted )
    begin
        --Get the id of the inserted record
        select @RecordId = inserted.script_id,
          @SecurableId = inserted.securable_id
          from inserted;

        if @SecurableId is null
        begin
            --Insert a new row into [securables]
            insert into [securables] ( [securable_type_id] )
              values ( @SecurableTypeId );

            --Get the generated id
            select @SecurableId = scope_identity();

            --Update the [scripts] table with the generated [securable_id]
            update [scripts]
              set [securable_id] = @SecurableId
              where [script_id] = @RecordId;
        end;
    end
    --Handle deletes
    else
    begin
        --Get the id of the deleted record
        select @RecordId = deleted.script_id,
          @SecurableId = deleted.securable_id
          from deleted;

        --Delete the [securable_id] from the [securables] table
        delete from [securables]
          where [securable_id] = @SecurableId;
    end;
end;");

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
            .WithColumn("timestamp_key").AsString(500)
            .WithColumn("bucket_key").AsString(500);

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

    public override void Down()
    {
        Delete.Table("script_variants");

        Execute.Sql(@"drop trigger [TR_scripts_audit];");
        Execute.Sql(@"drop trigger [TR_scripts_securable_id];");

        Delete.Table("scripts");

        Execute.Sql(@"drop trigger [TR_script_interpreters_audit];");
        Execute.Sql(@"drop trigger [TR_script_interpreters_securable_id];");

        Delete.Table("script_interpreters");
    }
}
