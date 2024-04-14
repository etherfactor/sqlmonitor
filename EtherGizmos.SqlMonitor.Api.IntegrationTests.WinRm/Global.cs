using Microsoft.Win32;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

[assembly: ExcludeFromCodeCoverage]

namespace EtherGizmos.SqlMonitor.Api.IntegrationTests.WinRm;

[SetUpFixture]
internal static class Global
{
    public const string DockerComposeFilePath = "./Initialization/docker-compose.yml";

    [OneTimeSetUp]
    public static async Task OneTimeSetUp()
    {
        if (IsSupportedOS())
        {
            //Wait 5 seconds before trying to switch to Windows containers, to reduce likelihood of contention with Linux
            //containers that may be starting in other tests
            await Task.Delay(5000);

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
}
