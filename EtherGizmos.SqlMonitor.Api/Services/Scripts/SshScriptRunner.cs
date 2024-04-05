using EtherGizmos.SqlMonitor.Models.Database;
using EtherGizmos.SqlMonitor.Models.Database.Enums;
using Renci.SshNet;
using System.Security.Cryptography;
using System.Text;

namespace EtherGizmos.SqlMonitor.Api.Services.Scripts;

public class SshScriptRunner
{
    public async Task ExecuteAsync(MonitoredScriptTarget scriptTarget, ScriptVariant scriptVariant)
    {
        var hostName = scriptTarget.HostName;
        var port = scriptTarget.Port ?? 22;
        var filePath = scriptTarget.FilePath;

        var username = scriptTarget.Username;
        var password = scriptTarget.Password;

        var privateKey = scriptTarget.PrivateKey;
        var privateKeyPassword = scriptTarget.PrivateKeyPassword;

        var command = scriptVariant.ScriptInterpreter.Command;
        var arguments = scriptVariant.ScriptInterpreter.Arguments;

        AuthenticationMethod authentication;
        if (scriptTarget.AuthenticationType == SshAuthenticationType.None)
        {
            authentication = new NoneAuthenticationMethod(
                username);
        }
        else if (scriptTarget.AuthenticationType == SshAuthenticationType.Password)
        {
            authentication = new PasswordAuthenticationMethod(
                username,
                password);
        }
        else if (scriptTarget.AuthenticationType == SshAuthenticationType.PrivateKey)
        {
            var privateKeyBytes = Encoding.UTF8.GetBytes(privateKey ?? "");
            var privateKeyStream = new MemoryStream(privateKeyBytes);

            if (privateKeyPassword is not null)
            {
                authentication = new PrivateKeyAuthenticationMethod(
                    username,
                    new PrivateKeyFile(privateKeyStream, privateKeyPassword));
            }
            else
            {
                authentication = new PrivateKeyAuthenticationMethod(
                    username,
                    new PrivateKeyFile(privateKeyStream));
            }
        }
        else
        {
            throw new NotSupportedException($"The authentication type {scriptTarget.AuthenticationType} is not supported.");
        }

        var connectionInfo = new Renci.SshNet.ConnectionInfo(hostName, port, username, authentication);

        using var client = new SshClient(connectionInfo);
        await client.ConnectAsync(CancellationToken.None);

        var script = scriptVariant.ScriptText;
        var scriptVariableName = "script";
        var scriptVariable = EncodeScriptToVariable(scriptVariableName, script);

        var scriptHash = CalculateMd5Checksum(script);

        var scriptExtension = scriptVariant.ScriptInterpreter.Extension;

        var commandText = $@"cd {filePath}

{scriptVariable}

echo ""${scriptVariableName}"" > {scriptHash}.{scriptExtension}

{command} {arguments.Replace("$Script", $"{scriptHash}.{scriptExtension}")}".Replace("\r", "");

        var result = client.RunCommand(commandText);

        Console.Out.WriteLine(result.Result);
    }

    private string EncodeScriptToVariable(string variableName, string script)
    {
        //Create a setter for a variable
        var variableSet = $@"{variableName}=$(cat << 'EOF'
{script}
EOF
)";

        return variableSet;
    }

    private string CalculateMd5Checksum(string script)
    {
        var md5 = MD5.Create();

        var scriptBytes = Encoding.UTF8.GetBytes(script);
        var hashBytes = md5.ComputeHash(scriptBytes);
        var scriptHash = BitConverter.ToString(hashBytes).Replace("-", "").ToLower();

        return scriptHash;
    }
}
