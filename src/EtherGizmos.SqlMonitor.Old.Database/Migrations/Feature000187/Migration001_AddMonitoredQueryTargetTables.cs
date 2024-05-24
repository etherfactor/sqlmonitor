using EtherGizmos.SqlMonitor.Database.Core;
using EtherGizmos.SqlMonitor.Database.Extensions;
using System.Data;

namespace EtherGizmos.SqlMonitor.Database.Migrations.Feature000187;

[CreatedAt(year: 2024, month: 04, day: 28, hour: 12, minute: 00, description: "Create monitored query target tables", trackingId: 187)]
public class Migration001_AddMonitoredQueryTargetTables : MigrationExtension
{
    public override void Up()
    {
        /*
         * Create [dbo].[monitored_query_targets]
         *  - for a given target, a server & directory being targeted with monitoring queries
         */
        Create.Table("monitored_query_targets")
            .WithColumn("monitored_query_target_id").AsInt32().PrimaryKey().Identity()
            .WithAuditColumns()
            .WithColumn("monitored_target_id").AsInt32().NotNullable()
            .WithColumn("sql_type_id").AsInt32().NotNullable()
            .WithColumn("host").AsString(255).NotNullable()
            .WithColumn("securable_id").AsInt32().Nullable()
            .WithColumn("connection_string").AsString(int.MaxValue).NotNullable();

        Create.ForeignKey("FK_monitored_query_targets_monitored_target_id")
            .FromTable("monitored_query_targets").ForeignColumn("monitored_target_id")
            .ToTable("monitored_targets").PrimaryColumn("monitored_target_id");

        Create.Index("IX_monitored_query_targets_monitored_target_id")
            .OnTable("monitored_query_targets")
            .OnColumn("monitored_target_id");

        Create.ForeignKey("FK_monitored_query_targets_sql_type_id")
            .FromTable("monitored_query_targets").ForeignColumn("sql_type_id")
            .ToTable("sql_types").PrimaryColumn("sql_type_id");

        Create.Index("IX_monitored_query_targets_sql_type_id")
            .OnTable("monitored_query_targets")
            .OnColumn("sql_type_id");

        Create.ForeignKey("FK_monitored_query_targets_securable_id")
            .FromTable("monitored_query_targets").ForeignColumn("securable_id")
            .ToTable("securables").PrimaryColumn("securable_id");

        Create.Index("IX_monitored_query_targets_securable_id")
            .OnTable("monitored_query_targets")
            .OnColumn("securable_id");

        this.AddAuditTriggerV1("monitored_query_targets",
            ("monitored_query_target_id", DbType.Int32));

        this.AddSecurableTriggerV1("monitored_query_targets", "securable_id", 170,
            ("monitored_query_target_id", DbType.Int32));
    }

    public override void Down()
    {
        Delete.Table("monitored_query_targets");
    }
}
