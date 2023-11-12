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
  [index] int,
  [aggregate] nvarchar(max),
  [value] nvarchar(max)
);

with [metric_values] as (
  select row_number() over ( order by ( select 0 ) ) - 1 as [index],
    substring( [s].[value], 0, charindex( ':', [s].[value] ) ) as [aggregate],
    substring( [s].[value], charindex( ':', [s].[value] ) + 1, len( [s].[value] ) - charindex( ':', [s].[value] ) ) as [value]
    from string_split( @MetricValues, ';' ) [s]
)
insert into @MetricValuesList ( [index], [aggregate], [value] )
select [mv].[index],
  [mv].[aggregate],
  replace( [mv].[value], '%3B', ';' )
  from [metric_values] [mv];

declare @CteSqlColumns nvarchar(max);
select @CteSqlColumns = string_agg( 'cast( ' + [m].[value] + ' as float ) as ' + quotename( 'metric_' + cast( [m].[index] as varchar(max) ) ), ', ' )
  from @MetricValuesList [m];
  
declare @SelectSqlValueColumns nvarchar(max);
select @SelectSqlValueColumns = string_agg(
    [m].[aggregate] + '( ' + quotename( 'metric_' + cast( [m].[index] as varchar(max) ) ) + ' ) as ' + quotename( 'metric_' + cast( [m].[index] as varchar(max) ) ),
    ', ' )
  from @MetricValuesList [m];

--Pull together the severity ranges into selects
declare @MetricSeveritiesList table (
  [index] int,
  [severity] nvarchar(max),
  [minimum_value] nvarchar(max),
  [maximum_value] nvarchar(max)
);

with [metric_rows] as (
  select row_number() over ( order by ( select 0 ) ) - 1 as [index],
    [s].[value]
    from string_split( @MetricSeverities, ';' ) [s]
),
[metric_severities] as (
  select [mr].[index],
    substring( [s].[value], 0, charindex( ':', [s].[value] ) ) as [severity],
    substring( [s].[value], charindex( ':', [s].[value] ) + 1, len( [s].[value] ) - charindex( ':', [s].[value] ) ) as [value_range]
    from [metric_rows] [mr]
      cross apply string_split( [mr].[value], '|' ) [s]
),
[metric_severity_values] as (
  select [ms].[index],
    [ms].[severity],
    substring( [ms].[value_range], 0, charindex( ':', [ms].[value_range] ) ) as [minimum_value],
    substring( [ms].[value_range], charindex( ':', [ms].[value_range] ) + 1, len( [ms].[value_range] ) - charindex( ':', [ms].[value_range] ) ) as [maximum_value]
    from [metric_severities] [ms]
)
insert into @MetricSeveritiesList ( [index], [severity], [minimum_value], [maximum_value] )
select [msv].[index],
  [msv].[severity],
  replace( replace( [msv].[minimum_value], '%3B', ';' ), '%7C', '|' ),
  replace( replace( [msv].[maximum_value], '%3B', ';' ), '%7C', '|' )
  from [metric_severity_values] [msv];

declare @SelectSqlSeverityColumns nvarchar(max);
select @SelectSqlSeverityColumns = string_agg(
    [m].[minimum_value] + ' as ' + quotename( 'metric_' + cast( [m].[index] as varchar(max) ) + '_' + [m].[severity] + '_min' )
    + ', '
    + [m].[maximum_value] + ' as ' + quotename( 'metric_' + cast( [m].[index] as varchar(max) ) + '_' + [m].[severity] + '_max' ),
    ', ' )
  from @MetricSeveritiesList [m];

--Combine values and severity ranges into selects
declare @SelectSqlColumns nvarchar(max);
set @SelectSqlColumns = @SelectSqlValueColumns + ', ' + @SelectSqlSeverityColumns;

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
