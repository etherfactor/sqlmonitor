using EtherGizmos.SqlMonitor.Shared.IntegrationTests;
using System.Runtime.InteropServices;

namespace EtherGizmos.SqlMonitor.Api.IntegrationTests.Ssh;

[SetUpFixture]
internal class DockerSetup : DockerSetupBase
{
    public const string PrivateKeyFilePath = "./Initialization/id_rsa";

    public override OSPlatform DockerOS => OSPlatform.Linux;

    public override string DockerComposeFile => "./Initialization/docker-compose.yml";
}
