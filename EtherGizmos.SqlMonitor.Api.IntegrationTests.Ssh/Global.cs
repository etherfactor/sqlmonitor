using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

[assembly: ExcludeFromCodeCoverage]

namespace EtherGizmos.SqlMonitor.Api.IntegrationTests.Ssh;

[SetUpFixture]
internal static class Global
{
    public const string DockerCompose2019FilePath = "./Initialization/docker-compose-2019.yml";
    public const string DockerCompose2022FilePath = "./Initialization/docker-compose-2022.yml";
    public const string PrivateKeyFilePath = "./Initialization/id_rsa";

    [OneTimeSetUp]
    public static async Task OneTimeSetUp()
    {
        using var maybeSemaphore = MaybeGetSemaphore("DockerSemaphore");
        try
        {
            maybeSemaphore?.WaitOne();

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
        if (IsWindows10() || IsWindowsServer2019())
        {
            return DockerCompose2019FilePath;
        }
        else if (IsWindows11() || IsWindowsServer2022())
        {
            return DockerCompose2022FilePath;
        }
        else
        {
            throw new PlatformNotSupportedException();
        }
    }

    private static bool IsWindows10()
    {
        var osName = Environment.OSVersion.VersionString;
        return osName.Contains("Windows 10");
    }

    private static bool IsWindowsServer2019()
    {
        var osName = Environment.OSVersion.VersionString;
        return osName.Contains("Server 2019");
    }

    private static bool IsWindows11()
    {
        var osName = Environment.OSVersion.VersionString;
        return osName.Contains("Windows 11");
    }

    private static bool IsWindowsServer2022()
    {
        var osName = Environment.OSVersion.VersionString;
        return osName.Contains("Server 2022");
    }
}
