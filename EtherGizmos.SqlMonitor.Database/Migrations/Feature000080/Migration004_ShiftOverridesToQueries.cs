using EtherGizmos.SqlMonitor.Database.Core;
using FluentMigrator;

namespace EtherGizmos.SqlMonitor.Database.Migrations.Feature000080;

[CreatedAt(year: 2023, month: 09, day: 17, hour: 18, minute: 45, description: "Shift whitelist/blacklist/overrides to queries", trackingId: 80)]
public class Migration004_ShiftOverridesToQueries : Migration
{
    public override void Up()
    {
        /*
         * Rename [dbo].[instance_query_blacklists] to [dbo].[query_instance_blacklists]
         */
        Delete.PrimaryKey("PK_instance_query_blacklists")
            .FromTable("instance_query_blacklists");

        Rename.Table("instance_query_blacklists")
            .To("query_instance_blacklists");

        Create.PrimaryKey("PK_query_instance_blacklists")
            .OnTable("query_instance_blacklists")
            .Columns("query_id", "instance_id");

        /*
         * Rename [dbo].[instance_query_whitelists] to [dbo].[query_instance_whitelists]
         */
        Delete.PrimaryKey("PK_instance_query_whitelists")
            .FromTable("instance_query_whitelists");

        Rename.Table("instance_query_whitelists")
            .To("query_instance_whitelists");

        Create.PrimaryKey("PK_query_instance_whitelists")
            .OnTable("query_instance_whitelists")
            .Columns("query_id", "instance_id");

        /*
         * Rename [dbo].[instance_query_databases] to [dbo].[query_instance_databases]
         */
        Delete.PrimaryKey("PK_instance_query_databases")
            .FromTable("instance_query_databases");

        Rename.Table("instance_query_databases")
            .To("query_instance_databases");

        Create.PrimaryKey("PK_query_instance_databases")
            .OnTable("query_instance_databases")
            .Columns("query_id", "instance_id");
    }

    public override void Down()
    {
        /*
         * Revert [dbo].[instance_query_blacklists]
         */
        Delete.PrimaryKey("PK_query_instance_blacklists")
            .FromTable("query_instance_blacklists");

        Rename.Table("query_instance_blacklists")
            .To("instance_query_blacklists");

        Create.PrimaryKey("PK_instance_query_blacklists")
            .OnTable("instance_query_blacklists")
            .Columns("instance_id", "query_id");

        /*
         * Revert [dbo].[instance_query_whitelists]
         */
        Delete.PrimaryKey("PK_query_instance_whitelists")
            .FromTable("query_instance_whitelists");

        Rename.Table("query_instance_whitelists")
            .To("instance_query_whitelists");

        Create.PrimaryKey("PK_instance_query_whitelists")
            .OnTable("instance_query_whitelists")
            .Columns("instance_id", "query_id");

        /*
         * Revert [dbo].[instance_query_databases]
         */
        Delete.PrimaryKey("PK_query_instance_databases")
            .FromTable("query_instance_databases");

        Rename.Table("query_instance_databases")
            .To("instance_query_databases");

        Create.PrimaryKey("PK_instance_query_databases")
            .OnTable("instance_query_databases")
            .Columns("instance_id", "query_id");
    }
}
