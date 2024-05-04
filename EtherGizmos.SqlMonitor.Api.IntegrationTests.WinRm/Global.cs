using EtherGizmos.SqlMonitor.Shared.IntegrationTests;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

[assembly: ExcludeFromCodeCoverage]

namespace EtherGizmos.SqlMonitor.Api.IntegrationTests.WinRm;

[SetUpFixture]
internal class Global : DockerSetupBase
{
    public override OSPlatform DockerOS => OSPlatform.Windows;

    public override string DockerComposeFile => GetDockerComposeFile();
    public const string DockerCompose2019FilePath = "./Initialization/docker-compose-2019.yml";
    public const string DockerCompose2022FilePath = "./Initialization/docker-compose-2022.yml";

    protected override async Task PerformSetUp()
    {
        await InvokeCommand("docker", $"network rm initialization_default");
        await base.PerformSetUp();
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
