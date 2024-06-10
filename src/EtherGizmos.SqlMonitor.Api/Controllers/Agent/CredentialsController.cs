using Asp.Versioning;
using EtherGizmos.SqlMonitor.Shared.Database.Services;
using EtherGizmos.SqlMonitor.Shared.Messaging;
using EtherGizmos.SqlMonitor.Shared.Models.Communication;
using EtherGizmos.SqlMonitor.Shared.Models.Database.Enums;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace EtherGizmos.SqlMonitor.Api.Controllers.Agent;

[ApiController]
public class CredentialsController : ControllerBase
{
    private const string BasePath = "agent/v{version:apiVersion}/credentials";

    private readonly ILogger _logger;
    private readonly ApplicationContext _context;

    public const string SecurityKey =
        "1Q3V2h3ji3GJtIqntFdelvL88CV97NxU" +
        "lhSD6CVQVZ6WJYj9Ejcusq3XVhQANA6w" +
        "cx3jTOm3cZCi4urBjldPgfZlwu1ORev4" +
        "V4HNSh7MPAnd5QIxN7wed9WH5fDIDdWw" +
        "PwnjpQNlxEdSnpWefc873LrkqilGtqjl" +
        "XYNe6S72TL8NWyo6FjwTGDFtKglG2I0X" +
        "uOhluEFFz1Ye9eJm2flUREN6HsxKXiL4" +
        "8I3ncR8UvE9hcbil3On7HQ2swyfSezzE";

    public CredentialsController(
        ILogger<CredentialsController> logger,
        ApplicationContext context)
    {
        _logger = logger;
        _context = context;
    }

    [ApiVersion("0.1")]
    [HttpPost(BasePath)]
    public async Task<IActionResult> Index([FromQuery] string connectionToken)
    {
        await Task.Delay(0);

        if (string.IsNullOrWhiteSpace(connectionToken))
            return BadRequest();

        var handler = new JsonWebTokenHandler();
        var validations = new TokenValidationParameters()
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(SecurityKey)),
            ValidateLifetime = true,
            ValidateIssuer = false,
            ValidateAudience = false,
        };

        var result = await handler.ValidateTokenAsync(connectionToken, validations);

        if (!result.IsValid)
        {
            _logger.LogWarning(result.Exception, "Received an invalid connection token");
            return BadRequest();
        }

        var targetType = result.Claims.SingleOrDefault(e => e.Key == MessagingConstants.Claims.TargetType).Value?.ToString()
            ?? throw new InvalidOperationException($"The connection token lacked claim '{MessagingConstants.Claims.TargetType}'");

        var targetIdString = result.Claims.SingleOrDefault(e => e.Key == MessagingConstants.Claims.Id).Value?.ToString()
            ?? throw new InvalidOperationException($"The connection token lacked claim '{MessagingConstants.Claims.Id}'");

        if (!int.TryParse(targetIdString, out int targetId))
            throw new InvalidOperationException($"The connection id '{targetIdString}' was not a valid integer");

        if (targetType == "query")
        {
            var target = await _context.MonitoredQueryTargets.SingleOrDefaultAsync(e => e.Id == targetId);
            if (target is null)
                return BadRequest();

            if (target.SqlType == SqlType.MariaDb || target.SqlType == SqlType.MySql)
            {
                var config = new DatabaseConfiguration(
                    target.ConnectionString);

                return Ok(config);
            }
            else if (target.SqlType == SqlType.PostgreSql)
            {
                var config = new DatabaseConfiguration(
                    target.ConnectionString);

                return Ok(config);
            }
            else if (target.SqlType == SqlType.SqlServer)
            {
                var config = new DatabaseConfiguration(
                    target.ConnectionString);

                return Ok(config);
            }
        }
        else if (targetType == "script")
        {
            var target = await _context.MonitoredScriptTargets.SingleOrDefaultAsync(e => e.Id == targetId);
            if (target is null)
                return BadRequest();

            if (target.ExecType == ExecType.Ssh)
            {
                var config = new SshConfiguration(
                    target.HostName,
                    target.Port ?? 22,
                    target.RunInPath,
                    target.SshAuthenticationType ?? SshAuthenticationType.None,
                    target.SshUsername!,
                    target.SshPassword,
                    target.SshPrivateKey,
                    target.SshPrivateKeyPassword,
                    target.ScriptInterpreter.Command,
                    target.ScriptInterpreter.Arguments);

                return Ok(config);
            }
            else if (target.ExecType == ExecType.WinRm)
            {
                var config = new WinRmConfiguration(
                    target.WinRmUseSsl == true ? "https" : "http",
                    target.HostName,
                    target.Port ?? (target.WinRmUseSsl == true ? 5986 : 5985),
                    target.RunInPath,
                    target.WinRmAuthenticationType!.Value,
                    target.WinRmUsername!,
                    target.WinRmPassword,
                    target.ScriptInterpreter.Command,
                    target.ScriptInterpreter.Arguments);

                return Ok(config);
            }
        }

        return BadRequest();
    }
}
