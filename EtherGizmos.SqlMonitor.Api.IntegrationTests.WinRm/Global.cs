using Microsoft.Win32;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

[assembly: ExcludeFromCodeCoverage]

namespace EtherGizmos.SqlMonitor.Api.IntegrationTests.WinRm;

[SetUpFixture]
internal static class Global
{
    [OneTimeSetUp]
    public static async Task OneTimeSetUp()
    {
        if (IsSupportedOS())
        {
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
