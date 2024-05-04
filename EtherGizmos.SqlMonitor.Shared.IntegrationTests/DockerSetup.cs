using Microsoft.Win32;
using NUnit.Framework;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

[assembly: ExcludeFromCodeCoverage]

namespace EtherGizmos.SqlMonitor.Shared.IntegrationTests;

[SetUpFixture]
public abstract class DockerSetup
{
    public abstract OSPlatform DockerOS { get; }

    public abstract string DockerComposeFile { get; }

    [OneTimeSetUp]
    public async Task OneTimeSetUp()
    {
        EnsureSupportedOS();

        await SemaphoreContext(async () =>
        {
            await SetContainerOS(DockerOS);

            await RunPreComposeUpCommands();
            await InvokeCommand("docker-compose", $"-f {DockerComposeFile} up -d");
        });
    }

    [OneTimeTearDown]
    public async Task OneTimeTearDown()
    {
        await SemaphoreContext(async () =>
        {
            await SetContainerOS(DockerOS);

            await InvokeCommand("docker-compose", $"-f {DockerComposeFile} down");
            await RunPostComposeDownCommands();
        });
    }

    public abstract Task RunPreComposeUpCommands();

    public abstract Task RunPostComposeDownCommands();

    private void EnsureSupportedOS()
    {
        if (!IsSupportedOS())
        {
            Assert.Inconclusive("Tests can only be run on Windows Professional or Windows Server installations");
        }
    }

    private bool IsSupportedOS()
    {
        //Linux is always supported
        if (DockerOS == OSPlatform.Linux)
        {
            //Unless you don't have Windows 10 or higher
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows) && Environment.OSVersion.Version.Major < 10)
                return false;

            return true;
        }
        //Windows only works on Professional and Server
        else if (DockerOS == OSPlatform.Windows)
        {
            //So fail out if running Linux
            if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                return false;

            //Or if running Windows < 10
            if (Environment.OSVersion.Version.Major < 10)
                return false;

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
        else
        {
            throw new PlatformNotSupportedException("Docker tests currently only support Linux and Windows.");
        }
    }

    private async Task SemaphoreContext(Func<Task> context)
    {
        using var maybeSemaphore = MaybeGetSemaphore("DockerSemaphore");
        try
        {
            maybeSemaphore?.WaitOne();
            await context();
        }
        finally
        {
            maybeSemaphore?.Release();
        }
    }

    protected async Task InvokeCommand(string command, string? arguments = null)
    {
        using var process = new Process()
        {
            StartInfo = new()
            {
                FileName = command,
                Arguments = arguments,
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true
            }
        };
        process.OutputDataReceived += new DataReceivedEventHandler((sender, e) =>
        {
            Console.Out.WriteLine(e.Data);
        });
        process.ErrorDataReceived += new DataReceivedEventHandler((sender, e) =>
        {
            Console.Error.WriteLine(e.Data);
        });
        process.Start();
        await process.WaitForExitAsync();
    }

    private async Task SetContainerOS(OSPlatform dockerOS)
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            if (dockerOS == OSPlatform.Linux)
            {
                await InvokeCommand("powershell", $"& $Env:ProgramFiles\\Docker\\Docker\\DockerCli.exe -SwitchLinuxEngine");
            }
            else if (dockerOS == OSPlatform.Windows)
            {
                await InvokeCommand("powershell", $"& $Env:ProgramFiles\\Docker\\Docker\\DockerCli.exe -SwitchWindowsEngine");
            }
            else
            {
                throw new PlatformNotSupportedException("Docker tests currently only support Linux and Windows.");
            }
        }
    }

    private Semaphore? MaybeGetSemaphore(string? name = null)
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            return new Semaphore(1, 1, name);
        }

        return null;
    }
}
