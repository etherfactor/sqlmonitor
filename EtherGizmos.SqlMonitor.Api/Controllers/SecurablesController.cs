using AutoMapper;
using EtherGizmos.SqlMonitor.Api.Extensions;
using EtherGizmos.SqlMonitor.Api.Services.Abstractions;
using EtherGizmos.SqlMonitor.Models.Api.v1;
using EtherGizmos.SqlMonitor.Models.Database;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Microsoft.EntityFrameworkCore;

namespace EtherGizmos.SqlMonitor.Api.Controllers;

[Route(BasePath)]
public class SecurablesController : ODataController
{
    private const string BasePath = "/api/v1/securables";

    private IMapper Mapper { get; }

    private ISecurableService SecurableService { get; }

    private IQueryable<Securable> Securables => SecurableService.GetQueryable();

    public SecurablesController(IMapper mapper, ISecurableService securableService)
    {
        Mapper = mapper;
        SecurableService = securableService;
    }

    [HttpGet]
    [Route(BasePath)]
    public async Task<IActionResult> Search(ODataQueryOptions<SecurableDTO> queryOptions)
    {
        var finished = await Securables.MapExplicitlyAndApplyQueryOptions(Mapper, queryOptions);
        return Ok(finished);
    }

    [HttpGet]
    [Route(BasePath + "({id})")]
    public async Task<IActionResult> Get(string id, ODataQueryOptions<SecurableDTO> queryOptions)
    {
        Securable? record = await Securables.SingleOrDefaultAsync(e => e.Id == id);
        if (record == null)
            return NotFound();

        var finished = record.MapExplicitlyAndApplyQueryOptions(Mapper, queryOptions);
        return Ok(finished);
    }
}
