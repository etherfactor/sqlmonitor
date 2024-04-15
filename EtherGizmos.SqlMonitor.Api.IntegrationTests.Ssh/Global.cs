﻿using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

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
}
