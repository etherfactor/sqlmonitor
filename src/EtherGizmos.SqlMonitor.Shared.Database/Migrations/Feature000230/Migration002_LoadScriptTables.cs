using EtherGizmos.SqlMonitor.Shared.Database.Core;

namespace EtherGizmos.SqlMonitor.Shared.Database.Migrations.Feature000230;

[CreatedAt(year: 2024, month: 03, day: 18, hour: 21, minute: 00, description: "Load script tables", trackingId: 230)]
public class Migration002_LoadScriptTables : MigrationExtension
{
    public override void Up()
    {
        /*
         * Populate [dbo].[script_interpreters]
         *  - add PowerShell 7, PowerShell 5, and Bash interpreters
         */
        Merge.IntoTable("script_interpreters")
            .Row(new
            {
                system_id = new Guid("6d20b673-9147-47d9-879d-06d4d72f155d"),
                name = "PowerShell 7",
                description = "PowerShell 7 is a cross-platform scripting language compatible with Windows, macOS, and Linux. It runs scripts using the \"pwsh\" command, offering enhanced performance, modern language features, and improved compatibility with modules and scripts across different platforms.",
                command = "pwsh",
                arguments = "-NoProfile -ExecutionPolicy Unrestricted -Command ./$Script",
                extension = "ps1",
            })
            .Row(new
            {
                system_id = new Guid("317e04c8-5855-43dc-8a68-d21c137b46e5"),
                name = "PowerShell 5",
                description = "PowerShell 5 is a Windows-native scripting language bundled with Windows operating systems. It executes scripts using the \"powershell\" command, providing comprehensive Windows systems management capabilities through its extensive commandlets library.",
                command = "powershell",
                arguments = "-NoProfile -ExecutionPolicy Unrestricted -Command $Script",
                extension = "ps1",
            })
            .Row(new
            {
                system_id = new Guid("7e730bac-2451-41f2-b1f5-2b9a88fd6535"),
                name = "Bash",
                description = "Bash is a widely-used Unix shell and command language available on most Unix-based systems, including macOS and Linux. It executes scripts using the \"bash\" command, providing powerful command-line capabilities and scripting functionalities for automating tasks, file management, and system administration tasks.",
                command = "bash",
                arguments = "-c $Script",
                extension = "sh",
            })
            .Match(t => t.system_id);
    }

    public override void Down()
    {
    }
}
