using EtherGizmos.SqlMonitor.Database.Core;

namespace EtherGizmos.SqlMonitor.Database.Migrations.Feature000184;

[CreatedAt(year: 2024, month: 03, day: 13, hour: 18, minute: 45, description: "Load securable tables", trackingId: 184)]
public class Migration003_LoadSecurableTables : MigrationExtension
{
    public override void Up()
    {
        Merge.IntoTable("securable_types")
            .Row(new
            {
                securable_type_id = 110,
                name = "Monitored System",
                description = "A system on a server being monitored.",
            })
            .Row(new
            {
                securable_type_id = 120,
                name = "Monitored Resource",
                description = "A component of a system on a server being monitored.",
            })
            .Row(new
            {
                securable_type_id = 130,
                name = "Monitored Environment",
                description = "An environment of a system on a server being monitored.",
            })
            .Row(new
            {
                securable_type_id = 170,
                name = "Monitored Query Target",
                description = "A server database associated with a system, resource, and environment being monitored with queries.",
            })
            .Row(new
            {
                securable_type_id = 180,
                name = "Monitored Script Target",
                description = "A server directory associated with a system, resource, and environment being monitored with scripts.",
            })
            .Row(new
            {
                securable_type_id = 200,
                name = "Query",
                description = "A query run against a server used to gather metrics.",
            })
            .Row(new
            {
                securable_type_id = 300,
                name = "Script",
                description = "A script run against a server used to gather metrics.",
            })
            .Row(new
            {
                securable_type_id = 390,
                name = "Script Interpreter",
                description = "An application that is capable of running scripts.",
            })
            .Row(new
            {
                securable_type_id = 500,
                name = "Metric",
                description = "A data point tracked over time, with values returned via queries and scripts targeting systems.",
            })
            .Match(t => t.securable_type_id);
    }

    public override void Down()
    {
    }
}
