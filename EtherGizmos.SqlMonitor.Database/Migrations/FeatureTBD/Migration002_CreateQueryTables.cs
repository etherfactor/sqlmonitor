using EtherGizmos.SqlMonitor.Database.Core;
using EtherGizmos.SqlMonitor.Database.Extensions;
using FluentMigrator;

namespace EtherGizmos.SqlMonitor.Database.Migrations.FeatureTBD;

[CreatedAt(year: 2023, month: 07, day: 23, hour: 17, minute: 15, description: "Create query tables", trackingId: -1)] //TODO: add tracking
public class Migration002_CreateQueryTables : AutoReversingMigration
{
    public override void Up()
    {
        /*
         * Create [dbo].[queries]
         *  - queries run on the instances being monitored
         */
        Create.Table("queries")
            .WithColumn("query_id").AsGuid().PrimaryKey().WithDefault(SystemMethods.NewGuid)
            .WithAuditColumns()
            .WithColumn("system_id").AsGuid().NotNullable().WithDefault(SystemMethods.NewGuid)
            .WithColumn("name").AsString(100).NotNullable()
            .WithColumn("description").AsString(int.MaxValue).Nullable()
            .WithColumn("sql_text").AsString(int.MaxValue).NotNullable()
            .WithColumn("run_frequency").AsTime().NotNullable()
            .WithColumn("last_run_at_utc").AsDateTime2().Nullable()
            .WithColumn("timestamp_utc_expression").AsString(500).Nullable()
            .WithColumn("bucket_expression").AsString(500).Nullable();

        Create.Index("IX_queries_last_run_at_utc")
            .OnTable("queries")
            .OnColumn("last_run_at_utc")
            .Ascending();

        /*
         * Create [dbo].[instance_query_blacklists]
         *  - these queries will not run on the instance
         */
        Create.Table("instance_query_blacklists")
            .WithColumn("instance_id").AsGuid().PrimaryKey()
            .WithColumn("query_id").AsGuid().PrimaryKey()
            .WithAuditColumns();

        /*
         * Create [dbo].[instance_query_whitelists]
         *  - if specified, only these queries will run on the instance
         */
        Create.Table("instance_query_whitelists")
            .WithColumn("instance_id").AsGuid().PrimaryKey()
            .WithColumn("query_id").AsGuid().PrimaryKey()
            .WithAuditColumns();

        /*
         * Create [dbo].[instance_query_databases]
         *  - overridden databases for queries on given instances
         */
        Create.Table("instance_query_databases")
            .WithColumn("instance_id").AsGuid().PrimaryKey()
            .WithColumn("query_id").AsGuid().PrimaryKey()
            .WithAuditColumns()
            .WithColumn("override_database").AsString(128).Nullable();
    }
}
