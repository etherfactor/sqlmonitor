using EtherGizmos.SqlMonitor.Shared.Database.Core;

namespace EtherGizmos.SqlMonitor.Shared.Database.Migrations.Feature000188;

[CreatedAt(year: 2024, month: 04, day: 13, hour: 16, minute: 30, description: "Load monitored script target tables", trackingId: 188)]
public class Migration002_LoadMonitoredScriptTargetTables : MigrationExtension
{
    public override void Up()
    {
        Merge.IntoTable("ssh_authentication_types")
            .Row(new { ssh_authentication_type_id = 1, name = "None", description = "Does not perform any authentication when connecting via SSH." })
            .Row(new { ssh_authentication_type_id = 10, name = "Password", description = "Authenticates using a username and password when connecting via SSH." })
            .Row(new { ssh_authentication_type_id = 20, name = "PrivateKey", description = "Authenticates using a private key when connecting via SSH." })
            .Match(t => t.ssh_authentication_type_id);

        Merge.IntoTable("winrm_authentication_types")
            .Row(new { winrm_authentication_type_id = 10, name = "Kerberos", description = "Authenticates through a domain controller when connecting via WinRM." })
            .Row(new { winrm_authentication_type_id = 20, name = "Basic", description = "Authenticates using a username and password when connecting via WinRM." })
            .Match(t => t.winrm_authentication_type_id);
    }

    public override void Down()
    {
    }
}
