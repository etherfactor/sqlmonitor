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
            .WithColumn("timestamp_utc_expression").AsString(500).Nullable()
            .WithColumn("bucket_expression").AsString(500).Nullable();
    }
}
