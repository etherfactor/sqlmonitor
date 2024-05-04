using EtherGizmos.SqlMonitor.Shared.IntegrationTests;
using Microsoft.Data.SqlClient;
using System.Data;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

[assembly: ExcludeFromCodeCoverage]

namespace EtherGizmos.SqlMonitor.Api.IntegrationTests.SqlServer;

[SetUpFixture]
internal class Global : DockerSetup
{
    public override OSPlatform DockerOS => OSPlatform.Linux;

    public override string DockerComposeFile => "./Initialization/docker-compose.yml";

    protected override async Task PerformSetUp()
    {
        await base.PerformSetUp();

        var connection = new SqlConnection("Server=localhost,11433; User Id=sa; Password=b@y4PJFfkiOIK1Ma6tXs; TrustServerCertificate=true;");
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
            throw new InvalidOperationException("SQL Server connection failed to initialize.");
        }

        await Task.Delay(5000);
    }
}
