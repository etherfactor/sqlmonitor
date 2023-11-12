--Default timestamps to the current UTC time and buckets to nothing
set @TimestampUtcExpression = coalesce( @TimestampUtcExpression, 'sysutcdatetime()' );
set @BucketExpression = coalesce( @BucketExpression, 'null' );

--Define a global temp table name; use a GUID to ensure uniqueness
declare @GlobalTempTable varchar(255) = quotename( '##sqlpulse_query_' + cast( newid() as varchar(36) ) );

--Create a table variable to store the output of sys.sp_describe_first_result_set
declare @Describe as table (
  is_hidden bit not null, 
  column_ordinal int not null, 
  name sysname null, 
  is_nullable bit not null, 
  system_type_id int not null, 
  system_type_name nvarchar(256) null, 
  max_length smallint not null, 
  precision tinyint not null, 
  scale tinyint not null, 
  collation_name sysname null, 
  user_type_id int null, 
  user_type_database sysname null, 
  user_type_schema sysname null, 
  user_type_name sysname null, 
  assembly_qualified_type_name nvarchar(4000), 
  xml_collection_id int null, 
  xml_collection_database sysname null, 
  xml_collection_schema sysname null, 
  xml_collection_name sysname null, 
  is_xml_document bit not null, 
  is_case_sensitive bit not null, 
  is_fixed_length_clr_type bit not null, 
  source_server nvarchar(128), 
  source_database nvarchar(128), 
  source_schema nvarchar(128), 
  source_table nvarchar(128), 
  source_column nvarchar(128), 
  is_identity_column bit null, 
  is_part_of_unique_key bit null, 
  is_updateable bit null, 
  is_computed_column bit null, 
  is_sparse_column_set bit null, 
  ordinal_in_order_by_list smallint null, 
  order_by_list_length smallint null, 
  order_by_is_descending smallint null, 
  tds_type_id int not null, 
  tds_length int not null, 
  tds_collation_id int null, 
  tds_collation_sort_id tinyint null
);

--Describe the columns of the provided SQL query
insert into @Describe
  exec sys.sp_describe_first_result_set @tsql = @Sql;

--Create a temp table containing the columns and types returned in the query
declare @TableSqlColumns nvarchar(max);
select @TableSqlColumns = string_agg( quotename( [d].[name] ) + ' ' + [d].[system_type_name], ', ' )
  within group ( order by [d].[column_ordinal] )
  from @Describe [d];

declare @TableSql nvarchar(max) = 'create table ' + @GlobalTempTable + ' (' + @TableSqlColumns + ');';

exec sp_executesql @TableSql;

--Execute the query into the temp table
declare @InsertSql nvarchar(max) = 
'insert into ' + @GlobalTempTable + '
  exec sp_executesql @Sql;';

exec sp_executesql @InsertSql, N'@Sql nvarchar(max)', @Sql = @Sql;

--Read the results of the query from the temp table, aggregating on time and bucket
declare @MetricValuesList table (
  [index] int primary key identity(0, 1),
  [aggregate] nvarchar(max),
  [value] nvarchar(max)
);

insert into @MetricValuesList ( [aggregate], [value] )
select substring( [s].[value], 0, charindex( ':', [s].[value] ) ),
  substring( [s].[value], charindex( ':', [s].[value] ) + 1, len( [s].[value] ) - charindex( ':', [s].[value] ) )
  from string_split( @MetricValues, ';' ) [s];

declare @CteSqlColumns nvarchar(max);
select @CteSqlColumns = string_agg( 'cast( ' + replace( [m].[value], '%3B', ';' ) + ' as float ) as ' + quotename( 'metric_' + cast( [m].[index] as varchar(max) ) ), ', ' )
  from @MetricValuesList [m];

declare @SelectSqlColumns nvarchar(max);
select @SelectSqlColumns = string_agg( [m].[aggregate] + '( ' + quotename( 'metric_' + cast( [m].[index] as varchar(max) ) ) + ' ) as ' + quotename( 'metric_' + cast( [m].[index] as varchar(max) ) ), ', ' )
  from @MetricValuesList [m];

declare @SelectSql nvarchar(max) =
'with [data] as (
  select cast( ' + @TimestampUtcExpression + ' as datetime2(0) ) as [__timestamp],
    ' + @BucketExpression + ' as [__bucket],
    ' + @CteSqlColumns + '
    from ' + @GlobalTempTable + '
)
select [__timestamp],
  [__bucket],
  ' + @SelectSqlColumns + '
  from [data]
  group by [__timestamp],
    [__bucket];';

exec sp_executesql @SelectSql;

--Drop the temp table
declare @DropSql nvarchar(max) = 'drop table ' + @GlobalTempTable + ';';

exec sp_executesql @DropSql;
