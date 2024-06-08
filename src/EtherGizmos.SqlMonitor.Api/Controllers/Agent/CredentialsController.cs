using Asp.Versioning;
using EtherGizmos.SqlMonitor.Shared.Database.Services;
using EtherGizmos.SqlMonitor.Shared.Messaging;
using EtherGizmos.SqlMonitor.Shared.Models.Communication;
using EtherGizmos.SqlMonitor.Shared.Models.Database.Enums;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using OpenIddict.Abstractions;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

namespace EtherGizmos.SqlMonitor.Api.Controllers.Agent;

[ApiController]
public class CredentialsController : ControllerBase
{
    private const string BasePath = "agent/v{version:apiVersion}/credentials";

    private readonly ILogger _logger;
    private readonly ApplicationContext _context;

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

        var handler = new JwtSecurityTokenHandler();
        var validations = new TokenValidationParameters()
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("abcdef")),
            ValidateLifetime = true,
            ValidateIssuer = false,
            ValidateAudience = false,
        };

        string targetType;
        string targetIdString;
        int targetId;
        try
        {
            var claims = handler.ValidateToken(connectionToken, validations, out var tokenSecure);

            targetType = claims.GetClaim(MessagingConstants.Claims.TargetType)
                ?? throw new InvalidOperationException($"The connection token lacked claim '{MessagingConstants.Claims.TargetType}'");

            targetIdString = claims.GetClaim(MessagingConstants.Claims.Id)
                ?? throw new InvalidOperationException($"The connection token lacked claim '{MessagingConstants.Claims.Id}'");

            if (!int.TryParse(targetIdString, out targetId))
                throw new InvalidOperationException($"The connection id '{targetIdString}' was not a valid integer");
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Received an invalid connection token");
            return BadRequest();
        }

        if (targetType == "query")
        {
            return NotFound();
        }
        else if (targetType == "script")
        {
            var target = await _context.MonitoredScriptTargets.SingleOrDefaultAsync(e => e.Id == targetId);
            if (target is null)
                return BadRequest();

            if (target.ExecType == ExecType.Ssh)
            {
                var config = new SshConfiguration()
                {
                    HostName = target.HostName,
                    Port = target.Port ?? 22,
                    FilePath = target.RunInPath,
                    AuthenticationType = target.SshAuthenticationType!.Value,
                    Username = target.SshUsername!,
                    Password = target.SshPassword,
                    Command = target.ScriptInterpreter.Command,
                    Arguments = target.ScriptInterpreter.Arguments,
                    PrivateKey = target.SshPrivateKey,
                    PrivateKeyPassword = target.SshPrivateKeyPassword,
                };

                return Ok(config);
            }
            else if (target.ExecType == ExecType.WinRm)
            {
                var config = new WinRmConfiguration()
                {
                    HostName = target.HostName,
                    Port = target.Port ?? (target.WinRmUseSsl == true ? 5986 : 5985),
                    Protocol = target.WinRmUseSsl == true ? "https" : "http",
                    FilePath = target.RunInPath,
                    AuthenticationType = target.WinRmAuthenticationType!.Value,
                    Username = target.WinRmUsername!,
                    Password = target.WinRmPassword,
                    Command = target.ScriptInterpreter.Command,
                    Arguments = target.ScriptInterpreter.Arguments,
                };

                return Ok(config);
            }
        }

        return BadRequest();
    }
}
