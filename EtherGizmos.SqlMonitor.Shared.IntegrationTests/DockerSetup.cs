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
            await PerformSetUp();
        });
    }

    [OneTimeTearDown]
    public async Task OneTimeTearDown()
    {
        await SemaphoreContext(async () =>
        {
            await SetContainerOS(DockerOS);
            await PerformTearDown();
        });
    }

    #region Inherited Methods
    /// <summary>
    /// Performs the Docker initialization. By default, calls docker-compose up on the compose file.
    /// </summary>
    /// <returns>An awaitable task.</returns>
    protected virtual async Task PerformSetUp()
    {
        await InvokeCommand("docker-compose", $"-f {DockerComposeFile} up -d");
    }

    /// <summary>
    /// Performs the Docker tear-down. By default, calls docker-compose down on the compose file.
    /// </summary>
    /// <returns>An awaitable task.</returns>
    protected virtual async Task PerformTearDown()
    {
        await InvokeCommand("docker-compose", $"-f {DockerComposeFile} down");
    }
    #endregion Inherited Methods

    #region Operating System
    /// <summary>
    /// Ensures the tests are running on a supported OS. If not, tests will be marked inconclusive (warning status).
    /// </summary>
    private void EnsureSupportedOS()
    {
        if (!IsSupportedOS())
        {
            Assert.Inconclusive("Tests can only be run on Windows Professional or Windows Server installations");
        }
    }

    /// <summary>
    /// Checks if the tests are running on a supported OS.
    /// </summary>
    /// <returns></returns>
    /// <exception cref="PlatformNotSupportedException"></exception>
    private bool IsSupportedOS()
    {
        //Docker for Windows requires Windows 10+
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows) && Environment.OSVersion.Version.Major < 10)
            return false;

        //Linux is always supported
        if (DockerOS == OSPlatform.Linux)
        {
            return true;
        }
        //Windows only works on Professional and Server
        else if (DockerOS == OSPlatform.Windows)
        {
            //So fail out if running Linux
            if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
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
    #endregion Operating System

    #region Test Coordination
    /// <summary>
    /// Attempts to get a Windows semaphore. On other operating systems, returns null.
    /// </summary>
    /// <param name="name">The name of the semaphore, if any.</param>
    /// <returns>The semaphore, if on Windows.</returns>
    private Semaphore? MaybeGetSemaphore(string? name = null)
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            return new Semaphore(1, 1, name);
        }

        return null;
    }

    /// <summary>
    /// Runs a task in a Windows semaphore context. If on Linux, the semaphore will not be created. This is fine as the
    /// semaphore is intended to coordinate swapping containers between Windows and Linux, which is only supported on
    /// Windows, not Linux.
    /// </summary>
    /// <param name="context">The task to run.</param>
    /// <returns>An awaitable task.</returns>
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
    #endregion Test Coordination

    #region Test Setup
    /// <summary>
    /// Invokes a command as a separate process and waits for it to complete.
    /// </summary>
    /// <param name="command">The command to run.</param>
    /// <param name="arguments">The arguments for the command.</param>
    /// <returns>An awaitable task.</returns>
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

    /// <summary>
    /// Switches the Docker container OS, if the current OS is Windows.
    /// </summary>
    /// <param name="dockerOS">The target Docker container OS.</param>
    /// <returns>An awaitable task.</returns>
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
    #endregion Test Setup
}
