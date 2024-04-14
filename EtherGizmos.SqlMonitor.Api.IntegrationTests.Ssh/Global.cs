using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

[assembly: ExcludeFromCodeCoverage]

namespace EtherGizmos.SqlMonitor.Api.IntegrationTests.Ssh;

[SetUpFixture]
internal static class Global
{
    public const string DockerComposeFilePath = "./Initialization/docker-compose.yml";
    public const string PrivateKeyFilePath = "./Initialization/id_rsa";

    [OneTimeSetUp]
    public static async Task OneTimeSetUp()
    {
        if (!File.Exists(PrivateKeyFilePath))
        {
            using var keyProcess = new Process()
            {
                StartInfo = new()
                {
                    FileName = "ssh-keygen",
                    Arguments = $"-t rsa -b 4096 -f {PrivateKeyFilePath} -N password",
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true
                }
            };

            keyProcess.Start();

            await keyProcess.WaitForExitAsync();
        }

        using var dockerProcess = new Process()
        {
            StartInfo = new()
            {
                FileName = "docker-compose",
                Arguments = $"-f {DockerComposeFilePath} up -d",
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true
            }
        };

        dockerProcess.Start();

        await dockerProcess.WaitForExitAsync();
    }

    [OneTimeTearDown]
    public static async Task OneTimeTearDown()
    {
        using var dockerProcess = new Process()
        {
            StartInfo = new()
            {
                FileName = "docker-compose",
                Arguments = $"-f {DockerComposeFilePath} down",
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true
            }
        };

        dockerProcess.Start();

        await dockerProcess.WaitForExitAsync();
    }
}
