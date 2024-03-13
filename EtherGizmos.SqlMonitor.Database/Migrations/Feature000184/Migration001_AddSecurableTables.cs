using EtherGizmos.SqlMonitor.Database.Core;
using EtherGizmos.SqlMonitor.Database.Extensions;
using FluentMigrator;

namespace EtherGizmos.SqlMonitor.Database.Migrations.Feature000184;

[CreatedAt(year: 2024, month: 03, day: 13, hour: 18, minute: 00, description: "Create securable tables", trackingId: 184)]
public class Migration001_AddSecurableTables : AutoReversingMigration
{
    public override void Up()
    {
        /*
         * Create [dbo].[securable_types]
         *  - the type of entities able to be secured
         */
        Create.Table("securable_types")
            .WithColumn("securable_type_id").AsInt32().PrimaryKey()
            .WithAuditColumns()
            .WithColumn("name").AsString(200).NotNullable()
            .WithColumn("description").AsString(int.MaxValue).Nullable();

        /*
         * Create [dbo].[securables]
         *  - an entity able to be secured with permissions
         */
        Create.Table("securables")
            .WithColumn("securable_id").AsInt32().PrimaryKey().Identity()
            .WithColumn("securable_type_id").AsInt32().NotNullable();
    }
}
