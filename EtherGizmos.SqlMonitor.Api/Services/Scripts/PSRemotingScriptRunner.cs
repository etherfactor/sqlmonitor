using EtherGizmos.SqlMonitor.Models.Database;
using System.Management.Automation;
using System.Management.Automation.Runspaces;
using System.Security;
using System.Security.Cryptography;
using System.Text;

namespace EtherGizmos.SqlMonitor.Api.Services.Scripts;

public class PSRemotingScriptRunner
{
    public async Task ExecuteAsync(MonitoredScriptTarget scriptTarget, ScriptVariant scriptVariant)
    {
        var protocol = scriptTarget.UseSsl ? "https" : "http";

        var hostName = scriptTarget.HostName;
        var port = scriptTarget.Port ?? (scriptTarget.UseSsl ? 5986 : 5985);

        var username = scriptTarget.Username;
        var password = scriptTarget.Password;

        PSCredential credentials;
        if (username is not null && password is not null)
        {
            var securePassword = new SecureString();
            foreach (var c in password)
            {
                securePassword.AppendChar(c);
            }

            credentials = new PSCredential(username, securePassword);
        }
        else
        {
            credentials = PSCredential.Empty;
        }

        var command = scriptVariant.ScriptInterpreter.Command;
        var arguments = scriptVariant.ScriptInterpreter.Arguments;

        var connectionInfo = new WSManConnectionInfo(new Uri($"{protocol}://{hostName}:{port}"), "http://schemas.microsoft.com/powershell/Microsoft.PowerShell", credentials);
        connectionInfo.AuthenticationMechanism = AuthenticationMechanism.Default;

        using var runspace = RunspaceFactory.CreateRunspace(connectionInfo);
        runspace.Open();

        using var powershell = PowerShell.Create();
        powershell.Runspace = runspace;

        var script = scriptVariant.ScriptText;
        var scriptVariable = EncodeScriptToVariable("Script", script);

        var scriptHash = CalculateMd5Checksum(script);

        var scriptExtension = scriptVariant.ScriptInterpreter.Extension;

        powershell.AddScript(@$"{scriptVariable}

if (!(Test-Path -Path ""{scriptHash}.{scriptExtension}"")) {{
    Set-Content -Path ""{scriptHash}.{scriptExtension}"" -Value $Script | Out-Null
}}

{command} {arguments.Replace("$Script", $@"""{scriptHash}.{scriptExtension}""")}");

        var result = await powershell.InvokeAsync();
    }

    private string EncodeScriptToVariable(string variableName, string script)
    {
        //Each script line needs to be separated. Each single quote needs to be doubled to be escaped, and the whole text
        //needs to be surrounded by single quotes
        var fragments = script.Replace("\r\n", "\n").Split("\n")
            .Select(e => $"'{e.Replace("'", "''")}'");

        //Combine the fragments, separated by environment-specific new-lines
        var variableValue = string.Join(" + [Environment]::NewLine + ", fragments);

        //Create a setter for a variable
        var variableSet = $"${variableName} = {variableValue}";

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
