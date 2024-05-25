using EtherGizmos.SqlMonitor.Shared.IntegrationTests;
using System.Data;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace EtherGizmos.SqlMonitor.Api.IntegrationTests.PostgreSql;

[SetUpFixture]
internal class DockerSetup : DockerSetupBase
{
    public const string ServerHost = "localhost";
    public const int ServerPort = 55432;
    public const string ServerDatabase = "performance_pulse";
    public const string ServerAdminUsername = "postgres";
    public const string ServerAdminPassword = "4^vnFsrTyNJU2xCJ9YL*";
    public const string ServerDefaultUsername = "service";
    public const string ServerDefaultPassword = "YUx^S7phYwBMiL3QZes&";

    public override OSPlatform DockerOS => OSPlatform.Linux;

    public override string DockerComposeFile => "./Initialization/docker-compose.yml";

    protected override async Task PerformSetUp()
    {
        await base.PerformSetUp();

        var connection = new NpgsqlConnection("Host=localhost; Port=55432; User Id=postgres; Password=4^vnFsrTyNJU2xCJ9YL*;");
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
            throw new InvalidOperationException("PostgreSQL connection failed to initialize.");
        }
    }
}
