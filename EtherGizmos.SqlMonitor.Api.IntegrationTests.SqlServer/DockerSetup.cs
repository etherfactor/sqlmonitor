using EtherGizmos.SqlMonitor.Shared.IntegrationTests;
using Microsoft.Data.SqlClient;
using System.Data;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;

namespace EtherGizmos.SqlMonitor.Api.IntegrationTests.SqlServer;

[SetUpFixture]
internal class DockerSetup : DockerSetupBase
{
    public const string ServerHost = "localhost";
    public const int ServerPort = 11433;
    public const string ServerDatabase = "performance_pulse";
    public const string ServerAdminUsername = "sa";
    public const string ServerAdminPassword = "b@y4PJFfkiOIK1Ma6tXs";
    public const string ServerDefaultUsername = "service";
    public const string ServerDefaultPassword = "LO^9ZpGB8FiA*HMMQyfN";

    public override OSPlatform DockerOS => OSPlatform.Linux;

    public override string DockerComposeFile => "./Initialization/docker-compose.yml";

    public string PostComposeScript => "./Initialization/init.sql";

    protected override async Task PerformSetUp()
    {
        await base.PerformSetUp();

        var connection = new SqlConnection($"Server={ServerHost},{ServerPort}; User Id={ServerAdminUsername}; Password={ServerAdminPassword}; TrustServerCertificate=true;");
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

        var initScript = File.ReadAllText(PostComposeScript);
        SqlCommand command;

        foreach (var initScriptChunk in new Regex(@"\sgo\s").Split(initScript))
        {
            command = connection.CreateCommand();
            command.CommandText = initScriptChunk;

            await command.ExecuteNonQueryAsync();
        }
    }
}
