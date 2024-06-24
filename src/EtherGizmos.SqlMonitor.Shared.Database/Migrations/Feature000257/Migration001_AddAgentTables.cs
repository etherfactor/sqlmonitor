using EtherGizmos.SqlMonitor.Shared.Database.Core;
using EtherGizmos.SqlMonitor.Shared.Database.Extensions;
using FluentMigrator;
using System.Data;

namespace EtherGizmos.SqlMonitor.Shared.Database.Migrations.Feature000257;

[CreatedAt(year: 2024, month: 05, day: 18, hour: 10, minute: 00, description: "Create agent tables", trackingId: 257)]
public class Migration001_AddAgentTables : MigrationExtension
{
    public override void Up()
    {
        Create.Table("agents")
            .WithColumn("agent_id").AsGuid().PrimaryKey().WithDefault(SystemMethods.NewGuid)
            .WithAuditColumns()
            .WithColumn("name").AsString(200).NotNullable()
            .WithColumn("description").AsString(int.MaxValue).Nullable()
            .WithColumn("agent_type_id").AsInt32().NotNullable()
            .WithColumn("dedicated_host").AsString(255).Nullable()
            .WithColumn("active_count").AsInt32().NotNullable().WithDefaultValue(0)
            .WithColumn("oauth2_application_id").AsInt32().NotNullable()
            .WithColumn("is_active").AsBoolean().NotNullable().WithDefaultValue(true)
            .WithColumn("is_soft_deleted").AsBoolean().NotNullable().WithDefaultValue(false);

        Create.ForeignKey("FK_agents_oauth2_application_id")
            .FromTable("agents").ForeignColumn("oauth2_application_id")
            .ToTable("oauth2_applications").PrimaryColumn("oauth2_application_id");

        Create.Index("IX_agents_oauth2_application_id")
            .OnTable("agents")
            .OnColumn("oauth2_application_id")
            .Ascending();

        this.AddAuditTriggerV1("agents",
            ("agent_id", DbType.Guid));
    }

    public override void Down()
    {
        Delete.Table("agents");
    }
}
