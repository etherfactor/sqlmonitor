using EtherGizmos.SqlMonitor.Database.Core;

namespace EtherGizmos.SqlMonitor.Database.Migrations.Feature000229;

[CreatedAt(year: 2024, month: 04, day: 28, hour: 11, minute: 30, description: "Load query tables", trackingId: 229)]
public class Migration002_LoadQueryTables : MigrationExtension
{
    public override void Up()
    {
        Merge.IntoTable("sql_types")
            .Row(new
            {
                sql_type_id = 10,
                name = "Microsoft SQL Server",
                description = "A relational database management system developed by Microsoft.",
            })
            .Row(new
            {
                sql_type_id = 20,
                name = "MySQL",
                description = "An open-source relational database management system developed by Oracle.",
            })
            .Row(new
            {
                sql_type_id = 25,
                name = "MariaDB",
                description = "An open-source relational database management system forked from MySQL.",
            })
            .Row(new
            {
                sql_type_id = 30,
                name = "PostgreSQL",
                description = "An open-source object-relational database management system.",
            })
            .Match(t => t.sql_type_id);
    }

    public override void Down()
    {
    }
}
