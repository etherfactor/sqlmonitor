using EtherGizmos.SqlMonitor.Database.Core;
using EtherGizmos.SqlMonitor.Database.Extensions;
using FluentMigrator;

namespace EtherGizmos.SqlMonitor.Database.Migrations.Feature000229;

[CreatedAt(year: 2024, month: 04, day: 28, hour: 11, minute: 00, description: "Create query tables", trackingId: 229)]
public class Migration001_AddQueryTables : Migration
{
    public override void Up()
    {
        /*
         * Create [dbo].[sql_types]
         *  - types of DBMS supported by the application
         */
        Create.Table("sql_types")
            .WithColumn("sql_type_id").AsInt32().PrimaryKey()
            .WithAuditColumns()
            .WithColumn("name").AsString(200).NotNullable()
            .WithColumn("description").AsString(int.MaxValue).Nullable();

        Execute.Sql(@"create trigger [TR_sql_types_audit]
on [sql_types]
after insert, update
as
begin
    set nocount on;

    declare @RecordId int;

    --Get the id of the inserted record
    select @RecordId = inserted.sql_type_id
        from inserted;

    --Set the last modified time of the record
    update [sql_types]
      set [modified_at_utc] = getutcdate()
      where [sql_type_id] = @RecordId;
end;");

        /*
         * Create [dbo].[queries]
         *  - query definitions being run against servers, without the actual query content
         */
        Create.Table("queries")
            .WithColumn("query_id").AsGuid().PrimaryKey().WithDefault(SystemMethods.NewGuid)
            .WithAuditColumns()
            .WithColumn("name").AsString(200).NotNullable()
            .WithColumn("description").AsString(int.MaxValue).Nullable()
            .WithColumn("run_frequency").AsTime().NotNullable()
            .WithColumn("last_run_at_utc").AsDateTime2().Nullable()
            .WithColumn("is_active").AsBoolean().NotNullable().WithDefaultValue(true)
            .WithColumn("is_soft_deleted").AsBoolean().NotNullable().WithDefaultValue(false)
            .WithColumn("bucket_column").AsString(100).Nullable()
            .WithColumn("timestamp_utc_column").AsString(100).Nullable()
            .WithColumn("system_id").AsGuid().NotNullable().WithDefault(SystemMethods.NewGuid)
            .WithColumn("securable_id").AsInt32().Nullable();

        Create.ForeignKey("FK_queries_securable_id")
            .FromTable("queries").ForeignColumn("securable_id")
            .ToTable("securables").PrimaryColumn("securable_id");

        Create.Index("IX_queries_securable_id")
            .OnTable("queries")
            .OnColumn("securable_id");

        Execute.Sql(@"create trigger [TR_queries_audit]
on [queries]
after insert, update
as
begin
    set nocount on;

    declare @RecordId uniqueidentifier;

    --Get the id of the inserted record
    select @RecordId = inserted.query_id
        from inserted;

    --Set the last modified time of the record
    update [queries]
      set [modified_at_utc] = getutcdate()
      where [query_id] = @RecordId;
end;");

        Execute.Sql(@"create trigger [TR_queries_securable_id]
on [queries]
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
        select @RecordId = inserted.query_id,
          @SecurableId = inserted.securable_id
          from inserted;

        if @SecurableId is null
        begin
            --Insert a new row into [securables]
            insert into [securables] ( [securable_type_id] )
              values ( @SecurableTypeId );

            --Get the generated id
            select @SecurableId = scope_identity();

            --Update the [queries] table with the generated [securable_id]
            update [queries]
              set [securable_id] = @SecurableId
              where [query_id] = @RecordId;
        end;
    end
    --Handle deletes
    else
    begin
        --Get the id of the deleted record
        select @RecordId = deleted.query_id,
          @SecurableId = deleted.securable_id
          from deleted;

        --Delete the [securable_id] from the [securables] table
        delete from [securables]
          where [securable_id] = @SecurableId;
    end;
end;");

        /*
         * Create [dbo].[query_variants]
         *  - actual query content and a mapping to their DBMS
         */
        Create.Table("query_variants")
            .WithColumn("query_variant_id").AsInt32().PrimaryKey().Identity()
            .WithAuditColumns()
            .WithColumn("query_id").AsGuid().NotNullable()
            .WithColumn("sql_type_id").AsInt32().NotNullable()
            .WithColumn("query_text").AsString(int.MaxValue).NotNullable();

        Create.ForeignKey("FK_query_variants_query_id")
            .FromTable("query_variants").ForeignColumn("query_id")
            .ToTable("queries").PrimaryColumn("query_id");

        Create.Index("IX_query_variants_query_id")
            .OnTable("query_variants")
            .OnColumn("query_id");

        Create.ForeignKey("FK_query_variants_sql_type_id")
            .FromTable("query_variants").ForeignColumn("sql_type_id")
            .ToTable("sql_types").PrimaryColumn("sql_type_id");

        Create.Index("IX_query_variants_sql_type_id")
            .OnTable("query_variants")
            .OnColumn("sql_type_id");

        Execute.Sql(@"create trigger [TR_query_variants_audit]
on [query_variants]
after insert, update
as
begin
    set nocount on;

    declare @RecordId int;

    --Get the id of the inserted record
    select @RecordId = inserted.query_variant_id
        from inserted;

    --Set the last modified time of the record
    update [query_variants]
      set [modified_at_utc] = getutcdate()
      where [query_variant_id] = @RecordId;
end;");
    }

    public override void Down()
    {
        Delete.Table("query_variants");

        Delete.Table("queries");

        Delete.Table("sql_types");
    }
}
