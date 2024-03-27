using EtherGizmos.SqlMonitor.Models.Database;
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

        var command = scriptVariant.ScriptInterpreter.Command;
        var arguments = scriptVariant.ScriptInterpreter.Arguments;

        var passwordAuthentication = new PasswordAuthenticationMethod(username, password);
        //new PrivateKeyAuthenticationMethod(username, new PrivateKeyFile());
        //new NoneAuthenticationMethod(username);
        var connectionInfo = new Renci.SshNet.ConnectionInfo(hostName, port, username, passwordAuthentication);

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

        Console.Out.WriteLine(result);
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
