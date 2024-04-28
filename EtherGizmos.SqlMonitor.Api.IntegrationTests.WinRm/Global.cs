using Microsoft.Win32;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

[assembly: ExcludeFromCodeCoverage]

namespace EtherGizmos.SqlMonitor.Api.IntegrationTests.WinRm;

[SetUpFixture]
internal static class Global
{
    public const string DockerCompose2019FilePath = "./Initialization/docker-compose-2019.yml";
    public const string DockerCompose2022FilePath = "./Initialization/docker-compose-2022.yml";

    [OneTimeSetUp]
    public static async Task OneTimeSetUp()
    {
        if (IsSupportedOS())
        {
            using var maybeSemaphore = MaybeGetSemaphore("DockerSemaphore");
            try
            {
                maybeSemaphore?.WaitOne();

                using var switchProcess = new Process()
                {
                    StartInfo = new()
                    {
                        FileName = "powershell",
                        Arguments = $"& $Env:ProgramFiles\\Docker\\Docker\\DockerCli.exe -SwitchWindowsEngine",
                        UseShellExecute = false,
                        RedirectStandardOutput = true,
                        RedirectStandardError = true
                    }
                };
                switchProcess.Start();
                await switchProcess.WaitForExitAsync();

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

                using var revertProcess = new Process()
                {
                    StartInfo = new()
                    {
                        FileName = "powershell",
                        Arguments = $"& $Env:ProgramFiles\\Docker\\Docker\\DockerCli.exe -SwitchLinuxEngine",
                        UseShellExecute = false,
                        RedirectStandardOutput = true,
                        RedirectStandardError = true
                    }
                };
                revertProcess.Start();
                await revertProcess.WaitForExitAsync();
            }
            finally
            {
                maybeSemaphore?.Release();
            }
        }
        else
        {
            Assert.Inconclusive("Tests can only be run on Windows Professional or Windows Server installations");
        }
    }

    [OneTimeTearDown]
    public static async Task OneTimeTearDown()
    {
        if (IsSupportedOS())
        {
            using var maybeSemaphore = MaybeGetSemaphore("DockerSemaphore");
            try
            {
                maybeSemaphore?.WaitOne();

                using var switchProcess = new Process()
                {
                    StartInfo = new()
                    {
                        FileName = "powershell",
                        Arguments = $"& $Env:ProgramFiles\\Docker\\Docker\\DockerCli.exe -SwitchWindowsEngine",
                        UseShellExecute = false,
                        RedirectStandardOutput = true,
                        RedirectStandardError = true
                    }
                };
                switchProcess.Start();
                await switchProcess.WaitForExitAsync();

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

                using var revertProcess = new Process()
                {
                    StartInfo = new()
                    {
                        FileName = "powershell",
                        Arguments = $"& $Env:ProgramFiles\\Docker\\Docker\\DockerCli.exe -SwitchLinuxEngine",
                        UseShellExecute = false,
                        RedirectStandardOutput = true,
                        RedirectStandardError = true
                    }
                };
                revertProcess.Start();
                await revertProcess.WaitForExitAsync();
            }
            finally
            {
                maybeSemaphore?.Release();
            }
        }
    }

    public static bool IsSupportedOS()
    {
        //This test project only runs on Windows Professional or Windows Server
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows) && Environment.OSVersion.Version.Major >= 10)
        {
            //Running on Windows, so check the registry for the Windows edition
            using var regKey = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows NT\CurrentVersion");

            if (regKey is null)
                return false;

            var editionId = regKey.GetValue("EditionID") as string;
            if (string.IsNullOrEmpty(editionId))
                return false;

            //Check if the edition is Professional or Server
            return editionId.Equals("Professional", StringComparison.OrdinalIgnoreCase) ||
                    editionId.Equals("ServerStandard", StringComparison.OrdinalIgnoreCase) ||
                    editionId.Equals("ServerDatacenter", StringComparison.OrdinalIgnoreCase);
        }

        return false;
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
            throw new PlatformNotSupportedException(Environment.OSVersion.VersionString);
        }
    }

    private static bool IsWindows10()
    {
        var osVersion = Environment.OSVersion.Version;
        return osVersion.Major == 10 && osVersion.Build < 22000;
    }

    private static bool IsWindowsServer2019()
    {
        var osVersion = Environment.OSVersion.Version;
        return osVersion.Major == 10 && osVersion.Build < 22000;
    }

    private static bool IsWindows11()
    {
        var osVersion = Environment.OSVersion.Version;
        return osVersion.Major == 10 && osVersion.Build >= 22000;
    }

    private static bool IsWindowsServer2022()
    {
        var osVersion = Environment.OSVersion.Version;
        return osVersion.Major == 10 && osVersion.Build >= 22000;
    }
}
