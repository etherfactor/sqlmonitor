using EtherGizmos.SqlMonitor.Database.Core;
using EtherGizmos.SqlMonitor.Database.Extensions;
using FluentMigrator;
using System.Data;

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
            .WithColumn("is_active").AsBoolean().NotNullable()
            .WithColumn("is_soft_deleted").AsBoolean().NotNullable()
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

        Create.ForeignKey("FK_instance_query_blacklists_instance_id")
            .FromTable("instance_query_blacklists").ForeignColumn("instance_id")
            .ToTable("instances").PrimaryColumn("instance_id")
            .OnUpdate(Rule.Cascade)
            .OnDelete(Rule.Cascade);
        //If an instance is modified, update references to its blacklisted queries.
        //If an instance is deleted, remove its associated blacklisted queries.

        Create.ForeignKey("FK_instance_query_blacklists_query_id")
            .FromTable("instance_query_blacklists").ForeignColumn("query_id")
            .ToTable("queries").PrimaryColumn("query_id")
            .OnUpdate(Rule.Cascade)
            .OnDelete(Rule.Cascade);
        //If a query is modified, update references to its blacklisted instances.
        //If a query is deleted, remove its associated blacklisted instances.

        Create.Index("IX_instance_query_blacklists_query_id")
            .OnTable("instance_query_blacklists")
            .OnColumn("query_id")
            .Ascending();

        /*
         * Create [dbo].[instance_query_whitelists]
         *  - if specified, only these queries will run on the instance
         */
        Create.Table("instance_query_whitelists")
            .WithColumn("instance_id").AsGuid().PrimaryKey()
            .WithColumn("query_id").AsGuid().PrimaryKey()
            .WithAuditColumns();

        Create.ForeignKey("FK_instance_query_whitelists_instance_id")
            .FromTable("instance_query_whitelists").ForeignColumn("instance_id")
            .ToTable("instances").PrimaryColumn("instance_id")
            .OnUpdate(Rule.Cascade)
            .OnDelete(Rule.Cascade);
        //If an instance is modified, update references to its whitelisted queries.
        //If an instance is deleted, remove its associated whitelisted queries.

        Create.ForeignKey("FK_instance_query_whitelists_query_id")
            .FromTable("instance_query_whitelists").ForeignColumn("query_id")
            .ToTable("queries").PrimaryColumn("query_id")
            .OnUpdate(Rule.Cascade)
            .OnDelete(Rule.Cascade);
        //If a query is modified, update references to its whitelisted instances.
        //If a query is deleted, remove its associated whitelisted instances.

        Create.Index("IX_instance_query_whitelists_query_id")
            .OnTable("instance_query_whitelists")
            .OnColumn("query_id")
            .Ascending();

        /*
         * Create [dbo].[instance_query_databases]
         *  - overridden databases for queries on given instances
         */
        Create.Table("instance_query_databases")
            .WithColumn("instance_id").AsGuid().PrimaryKey()
            .WithColumn("query_id").AsGuid().PrimaryKey()
            .WithAuditColumns()
            .WithColumn("override_database").AsString(128).Nullable();

        Create.ForeignKey("FK_instance_query_databases_instance_id")
            .FromTable("instance_query_databases").ForeignColumn("instance_id")
            .ToTable("instances").PrimaryColumn("instance_id")
            .OnUpdate(Rule.Cascade)
            .OnDelete(Rule.Cascade);
        //If an instance is modified, update references to its query databases.
        //If an instance is deleted, remove its associated query databases.

        Create.ForeignKey("FK_instance_query_databases_query_id")
            .FromTable("instance_query_databases").ForeignColumn("query_id")
            .ToTable("queries").PrimaryColumn("query_id")
            .OnUpdate(Rule.Cascade)
            .OnDelete(Rule.Cascade);
        //If a query is modified, update references to its instance databases.
        //If a query is deleted, remove its associated instance databases.

        Create.Index("IX_instance_query_databases_query_id")
            .OnTable("instance_query_databases")
            .OnColumn("query_id")
            .Ascending();
    }
}
