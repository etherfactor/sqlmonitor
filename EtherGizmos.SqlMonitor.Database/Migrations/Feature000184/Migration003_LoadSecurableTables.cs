using EtherGizmos.SqlMonitor.Database.Core;
using FluentMigrator;

namespace EtherGizmos.SqlMonitor.Database.Migrations.Feature000184;

[CreatedAt(year: 2024, month: 03, day: 13, hour: 18, minute: 45, description: "Load securable tables", trackingId: 184)]
public class Migration003_LoadSecurableTables : Migration
{
    public override void Up()
    {
        Insert.IntoTable("securable_types")
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
                securable_type_id = 150,
                name = "Monitored Script Target",
                description = "A server directory associated with a system, resource, and environment being monitored with scripts.",
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
            });
    }

    public override void Down()
    {
        Delete.FromTable("securable_types")
            .Row(new
            {
                securable_type_id = 120,
            })
            .Row(new
            {
                securable_type_id = 140,
            })
            .Row(new
            {
                securable_type_id = 160,
            })
            .Row(new
            {
                securable_type_id = 300,
            })
            .Row(new
            {
                securable_type_id = 390,
            });
    }
}
