﻿using FluentMigrator;
using System.Data;

namespace EtherGizmos.SqlMonitor.Shared.Database.Core;

internal static class Snippets
{
    /// <summary>
    /// Adds an audit trigger to a table.
    /// </summary>
    /// <param name="this">The migration.</param>
    /// <param name="table">The table name.</param>
    /// <param name="primaryKeys">The table's keys and their data types.</param>
    public static void AddAuditTriggerV1(this MigrationBase @this, string table, params (string Name, DbType Type)[] primaryKeys)
    {
        /*
         * MySQL
         */
        @this.IfDatabase(ProcessorId.MySql)
            .Execute.Sql($@"create trigger {MySqlHelper.Escape($"TR_{table}_audit_insert")}
before insert on {MySqlHelper.Escape(table)}
for each row
begin
    set new.`modified_at_utc` = utc_timestamp;
end;");

        @this.IfDatabase(ProcessorId.MySql)
            .Execute.Sql($@"create trigger {MySqlHelper.Escape($"TR_{table}_audit_update")}
before update on {MySqlHelper.Escape(table)}
for each row
begin
    set new.`modified_at_utc` = utc_timestamp;
end;");

        /*
         * PostgreSQL
         */
        @this.IfDatabase(ProcessorId.Postgres)
            .Execute.Sql($@"create or replace function {PostgreSqlHelper.Escape($"TR_{table}_audit")}()
returns trigger as $$
begin
    --Set the last modified time of the record
    new.""modified_at_utc"" := timezone('utc', now());
    return new;
end;
$$ language plpgsql;

create trigger {PostgreSqlHelper.Escape($"TR_{table}_audit")}
before insert or update
on {PostgreSqlHelper.Escape(table)}
for each row
execute function {PostgreSqlHelper.Escape($"TR_{table}_audit")}();");

        /*
         * Microsoft SQL Server
         */
        @this.IfDatabase(ProcessorId.SqlServer)
            .Execute.Sql($@"create trigger {SqlServerHelper.Escape($"TR_{table}_audit")}
on {SqlServerHelper.Escape(table)}
after insert, update
as
begin
    set nocount on;

{primaryKeys
    .Escape(SqlServerHelper.Escape)
    .Expand(Environment.NewLine, $"    declare @RecordId$$Index$$ $$Type$$;", converter: SqlServerHelper.ToDbString)}

    --Get the id of the inserted record
    select
{primaryKeys
    .Escape(SqlServerHelper.Escape)
    .Expand("," + Environment.NewLine, $"      @RecordId$$Index$$ = inserted.$$Name$$")}
      from inserted;

    --Set the last modified time of the record
    update {SqlServerHelper.Escape(table)}
      set [modified_at_utc] = getutcdate()
      where
{primaryKeys
    .Escape(SqlServerHelper.Escape)
    .Expand(" and" + Environment.NewLine, $"        $$Name$$ = @RecordId$$Index$$")};
end;");
    }

    /// <summary>
    /// Adds a securable trigger to a table.
    /// </summary>
    /// <param name="this">The migration.</param>
    /// <param name="table">The table name.</param>
    /// <param name="securableColumn">The table's securable id column.</param>
    /// <param name="securableTypeId">The type of securable being stored in this table.</param>
    /// <param name="primaryKeys">The table's primary keys and their data types.</param>
    public static void AddSecurableTriggerV1(this MigrationBase @this, string table, string securableColumn, int securableTypeId, params (string Name, DbType Type)[] primaryKeys)
    {
        /*
         * MySQL
         */
        @this.IfDatabase(ProcessorId.MySql)
            .Execute.Sql($@"create trigger {MySqlHelper.Escape($"TR_{table}_{securableColumn}_insert")}
before insert on {MySqlHelper.Escape(table)}
for each row
begin
    declare SecurableId int;
    declare SecurableTypeId int default {securableTypeId};

    if (new.{MySqlHelper.Escape(securableColumn)} is null) then
        #Insert a new row into securables
        insert into securables ( securable_type_id )
          values ( SecurableTypeId );

        #Get the generated id
        set SecurableId = last_insert_id();

        #Update the {table} table with the generated securable_id
        set new.{MySqlHelper.Escape(securableColumn)} = SecurableId;
    end if;
end;");

        @this.IfDatabase(ProcessorId.MySql)
            .Execute.Sql($@"create trigger {MySqlHelper.Escape($"TR_{table}_{securableColumn}_update")}
before update on {MySqlHelper.Escape(table)}
for each row
begin
    declare SecurableId int;
    declare SecurableTypeId int default {securableTypeId};

    if (new.{MySqlHelper.Escape(securableColumn)} is null) then
        #Insert a new row into securables
        insert into securables ( securable_type_id )
          values ( SecurableTypeId );

        #Get the generated id
        set SecurableId = last_insert_id();

        #Update the {table} table with the generated securable_id
        set new.{MySqlHelper.Escape(securableColumn)} = SecurableId;
    end if;
end;");

        @this.IfDatabase(ProcessorId.MySql)
            .Execute.Sql($@"create trigger {MySqlHelper.Escape($"TR_{table}_{securableColumn}_delete")}
after delete on {MySqlHelper.Escape(table)}
for each row
begin
    declare SecurableId int;
    declare SecurableTypeId int default {securableTypeId};

    #Get the id of the deleted record
    select old.{MySqlHelper.Escape(securableColumn)}
      into SecurableId;

    #Delete the securable_id from the securables table
    delete from `securables`
      where `securable_id` = SecurableId;
end;");

        /*
         * PostgreSQL
         */
        @this.IfDatabase(ProcessorId.Postgres)
            .Execute.Sql($@"create or replace function {PostgreSqlHelper.Escape($"{table}_{securableColumn}_insert")}()
returns trigger as $$
declare
    SecurableId INT;
    SecurableTypeId INT := {securableTypeId};
begin
    --Handle inserts/updates
    select new.""securable_id""
      into SecurableId;

    if SecurableId is null then
        --Insert a new row into securables
        insert into ""securables"" ( ""securable_type_id"" )
          values ( SecurableTypeId )
          returning ""securable_id"" into SecurableId;
        
        --Update the {table} table with the generated securable_id
        new.{PostgreSqlHelper.Escape(securableColumn)} := SecurableId;
    end if;

    return new;
end;
$$ language plpgsql;

create trigger {PostgreSqlHelper.Escape($"TR_{table}_{securableColumn}_insert")}
before insert or update
on {PostgreSqlHelper.Escape(table)}
for each row
execute function {PostgreSqlHelper.Escape($"{table}_{securableColumn}_insert")}();");

        @this.IfDatabase(ProcessorId.Postgres)
            .Execute.Sql($@"create or replace function {PostgreSqlHelper.Escape($"{table}_{securableColumn}_delete")}()
returns trigger as $$
declare
    SecurableId INT;
    SecurableTypeId INT := {securableTypeId};
begin
    --Handle deletes
    select old.""securable_id""
      into SecurableId;

    --Delete the securable_id from the securables table
    delete from ""securables""
      where ""securable_id"" = SecurableId;

    return new;
end;
$$ language plpgsql;

create trigger {PostgreSqlHelper.Escape($"TR_{table}_{securableColumn}_delete")}
after delete
on {PostgreSqlHelper.Escape(table)}
for each row
execute function {PostgreSqlHelper.Escape($"{table}_{securableColumn}_delete")}();");

        /*
         * Microsoft SQL Server
         */
        @this.IfDatabase(ProcessorId.SqlServer)
            .Execute.Sql($@"create trigger {SqlServerHelper.Escape($"TR_{table}_{securableColumn}")}
on {SqlServerHelper.Escape(table)}
after insert, update, delete
as
begin
    set nocount on;

{primaryKeys
    .Escape(SqlServerHelper.Escape)
    .Expand(Environment.NewLine, $"    declare @RecordId$$Index$$ $$Type$$;", converter: SqlServerHelper.ToDbString)}
    declare @SecurableId int;
    declare @SecurableTypeId int = {securableTypeId};

    --Handle inserts/updates
    if exists ( select 1 from inserted )
    begin
        --Get the id of the inserted record
        select
{primaryKeys
    .Escape(SqlServerHelper.Escape)
    .Expand("," + Environment.NewLine, $"          @RecordId$$Index$$ = inserted.$$Name$$")},
          @SecurableId = inserted.securable_id
          from inserted;

        if @SecurableId is null
        begin
            --Insert a new row into [securables]
            insert into [securables] ( [securable_type_id] )
              values ( @SecurableTypeId );

            --Get the generated id
            select @SecurableId = scope_identity();

            --Update the {table} table with the generated securable_id
            update {SqlServerHelper.Escape(table)}
              set {SqlServerHelper.Escape(securableColumn)} = @SecurableId
              where
{primaryKeys
    .Escape(SqlServerHelper.Escape)
    .Expand(" and" + Environment.NewLine, $"                $$Name$$ = @RecordId$$Index$$")};
        end;
    end
    --Handle deletes
    else
    begin
        --Get the id of the deleted record
        select
{primaryKeys
    .Escape(SqlServerHelper.Escape)
    .Expand("," + Environment.NewLine, $"          @RecordId$$Index$$ = deleted.$$Name$$")},
          @SecurableId = deleted.[securable_id]
          from deleted;

        --Delete the [securable_id] from the [securables] table
        delete from [securables]
          where [securable_id] = @SecurableId;
    end;
end;");
    }

    /// <summary>
    /// Fixes a MySQL time column, as there is now a TIME data type.
    /// </summary>
    /// <param name="this">The migration.</param>
    /// <param name="table">The table name.</param>
    /// <param name="column">The column to fix from DATETIME to TIME.</param>
    public static void FixTime(this MigrationBase @this, string table, string column)
    {
        @this.IfDatabase(ProcessorId.MySql)
            .Alter.Column(column).OnTable(table).AsCustom("TIME");
    }

    private static IEnumerable<(string Name, DbType Type)> Escape(this IEnumerable<(string Name, DbType Type)> @this, Func<string, string> escape)
    {
        return @this.Select(e => (escape(e.Name), e.Type));
    }

    private static string Expand(this IEnumerable<(string Name, DbType Type)> @this, string separator, string pattern, Func<DbType, string>? converter = null)
    {
        converter ??= type => type.ToString();
        var concat = @this.Select((e, i) => pattern
            .Replace("$$Index$$", i.ToString())
            .Replace("$$Name$$", e.Name)
            .Replace("$$Type$$", converter(e.Type)));

        var result = string.Join(separator, concat);

        return result;
    }
}
