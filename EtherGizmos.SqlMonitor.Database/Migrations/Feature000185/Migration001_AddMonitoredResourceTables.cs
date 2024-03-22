using EtherGizmos.SqlMonitor.Database.Core;
using EtherGizmos.SqlMonitor.Database.Extensions;
using FluentMigrator;

namespace EtherGizmos.SqlMonitor.Database.Migrations.Feature000185;

[CreatedAt(year: 2024, month: 03, day: 13, hour: 20, minute: 00, description: "Create monitored resource tables", trackingId: 185)]
public class Migration001_AddMonitoredResourceTables : Migration
{
    public override void Up()
    {
        /* 
         * Create [dbo].[monitored_resources]
         *  - resources in systems being monitored for metrics, alongside systems and environments
         */
        Create.Table("monitored_resources")
            .WithColumn("monitored_resource_id").AsGuid().PrimaryKey().WithDefault(SystemMethods.NewGuid)
            .WithAuditColumns()
            .WithColumn("name").AsString(200).NotNullable()
            .WithColumn("description").AsString(int.MaxValue).Nullable()
            .WithColumn("is_active").AsBoolean().NotNullable().WithDefaultValue(true)
            .WithColumn("is_soft_deleted").AsBoolean().NotNullable().WithDefaultValue(false)
            .WithColumn("securable_id").AsInt32().Nullable();

        Create.ForeignKey("FK_monitored_resources_securable_id")
            .FromTable("monitored_resources").ForeignColumn("securable_id")
            .ToTable("securables").PrimaryColumn("securable_id");

        Create.Index("IX_monitored_resources_securable_id")
            .OnTable("monitored_resources")
            .OnColumn("securable_id");

        Execute.Sql(@"create trigger [TR_monitored_resources_audit]
on [monitored_resources]
after insert, update
as
begin
    set nocount on;

    declare @RecordId uniqueidentifier;

    --Get the id of the inserted record
    select @RecordId = inserted.monitored_resource_id
        from inserted;

    --Set the last modified time of the record
    update [monitored_resources]
      set [modified_at_utc] = getutcdate()
      where [monitored_resource_id] = @RecordId;
end;");

        Execute.Sql(@"create trigger [TR_monitored_resources_securable_id]
on [monitored_resources]
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
        select @RecordId = inserted.monitored_resource_id,
          @SecurableId = inserted.securable_id
          from inserted;

        if @SecurableId is null
        begin
            --Insert a new row into [securables]
            insert into [securables] ( [securable_type_id] )
              values ( @SecurableTypeId );

            --Get the generated id
            select @SecurableId = scope_identity();

            --Update the [monitored_resources] table with the generated [securable_id]
            update [monitored_resources]
              set [securable_id] = @SecurableId
              where [monitored_resource_id] = @RecordId;
        end;
    end
    --Handle deletes
    else
    begin
        --Get the id of the deleted record
        select @RecordId = deleted.monitored_resource_id,
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
        Execute.Sql(@"drop trigger [TR_monitored_resources_audit];");
        Execute.Sql(@"drop trigger [TR_monitored_resources_securable_id];");

        Delete.Table("monitored_resources");
    }
}
