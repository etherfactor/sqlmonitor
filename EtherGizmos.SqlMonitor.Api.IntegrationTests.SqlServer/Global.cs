using Microsoft.Data.SqlClient;
using System.Data;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

[assembly: ExcludeFromCodeCoverage]

namespace EtherGizmos.SqlMonitor.Api.IntegrationTests.SqlServer;

[SetUpFixture]
internal static class Global
{
    public const string DockerComposeFilePath = "./Initialization/docker-compose.yml";

    [OneTimeSetUp]
    public static async Task OneTimeSetUp()
    {
        using var maybeSemaphore = MaybeGetSemaphore("DockerSemaphore");
        try
        {
            maybeSemaphore?.WaitOne();

            var useDockerComposeFile = GetDockerComposeFile();
            using var dockerProcess = new Process()
            {
                StartInfo = new()
                {
                    FileName = "docker-compose",
                    Arguments = $"-f {useDockerComposeFile} up -d",
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true
                }
            };
            dockerProcess.OutputDataReceived += new DataReceivedEventHandler((sender, e) =>
            {
                Console.Out.WriteLine(e.Data);
            });
            dockerProcess.ErrorDataReceived += new DataReceivedEventHandler((sender, e) =>
            {
                Console.Error.WriteLine(e.Data);
            });
            dockerProcess.Start();
            await dockerProcess.WaitForExitAsync();

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
        finally
        {
            maybeSemaphore?.Release();
        }
    }

    [OneTimeTearDown]
    public static async Task OneTimeTearDown()
    {
        using var maybeSemaphore = MaybeGetSemaphore("DockerSemaphore");
        try
        {
            maybeSemaphore?.WaitOne();

            var useDockerComposeFile = GetDockerComposeFile();
            using var dockerProcess = new Process()
            {
                StartInfo = new()
                {
                    FileName = "docker-compose",
                    Arguments = $"-f {useDockerComposeFile} down",
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true
                }
            };
            dockerProcess.Start();
            await dockerProcess.WaitForExitAsync();
        }
        finally
        {
            maybeSemaphore?.Release();
        }
    }

    private static Semaphore? MaybeGetSemaphore(string? name = null)
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            return new Semaphore(1, 1, name);
        }

        return null;
    }

    private static string GetDockerComposeFile()
    {
        return DockerComposeFilePath;
    }
}
