using EtherGizmos.SqlMonitor.Shared.IntegrationTests;
using MySqlConnector;
using System.Data;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace EtherGizmos.SqlMonitor.Api.IntegrationTests.MySql;

[SetUpFixture]
internal class DockerSetup : DockerSetupBase
{
    public const string ServerHost = "localhost";
    public const int ServerPort = 33306;
    public const string ServerDatabase = "performance_pulse";
    public const string ServerAdminUsername = "root";
    public const string ServerAdminPassword = "3NU1Cs!GXJf3CvWqIpCs";
    public const string ServerDefaultUsername = "service";
    public const string ServerDefaultPassword = "jipEZk@7ui2lw&XUiw^W";

    public override OSPlatform DockerOS => OSPlatform.Linux;

    public override string DockerComposeFile => "./Initialization/docker-compose.yml";

    protected override async Task PerformSetUp()
    {
        await base.PerformSetUp();

        var connection = new MySqlConnection("Server=localhost; Port=33306; Uid=root; Pwd=3NU1Cs!GXJf3CvWqIpCs;");
        var stopwatch = new Stopwatch();
        stopwatch.Start();
        do
        {
            try
            {
                await connection.OpenAsync();
            }
            catch
            {
                await Task.Delay(1000);
            }
        }
        while (connection.State != ConnectionState.Open && stopwatch.ElapsedMilliseconds < 60000);

        if (connection.State != ConnectionState.Open)
        {
            throw new InvalidOperationException("MySQL connection failed to initialize.");
        }
    }
}
