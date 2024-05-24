using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;

namespace EtherGizmos.SqlMonitor.Api.Controllers.Agent;

public class CredentialsController : ControllerBase
{
    private const string BasePath = "agent/v{version:apiVersion}/credentials";

    [ApiVersion("0.1")]
    [HttpPost(BasePath)]
    public async Task<IActionResult> Index([FromQuery] string connectionToken)
    {
        await Task.Delay(0);

        if (string.IsNullOrWhiteSpace(connectionToken))
            return BadRequest();

        return Ok(new { value = "Test" });
    }
}
