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
public class PermissionsController : ODataController
{
    private const string BasePath = "/api/v1/permissions";

    private IMapper Mapper { get; }

    private IPermissionService PermissionService { get; }

    private IQueryable<Permission> Permissions => PermissionService.GetQueryable();

    public PermissionsController(IMapper mapper, IPermissionService permissionService)
    {
        Mapper = mapper;
        PermissionService = permissionService;
    }

    [HttpGet]
    [Route(BasePath)]
    public async Task<IActionResult> Search(ODataQueryOptions<PermissionDTO> queryOptions)
    {
        var finished = await Permissions.MapExplicitlyAndApplyQueryOptions(Mapper, queryOptions);
        return Ok(finished);
    }

    [HttpGet]
    [Route(BasePath + "({id})")]
    public async Task<IActionResult> Get(string id, ODataQueryOptions<PermissionDTO> queryOptions)
    {
        Permission? record = await Permissions.SingleOrDefaultAsync(e => e.Id == id);
        if (record == null)
            return NotFound();

        var finished = record.MapExplicitlyAndApplyQueryOptions(Mapper, queryOptions);
        return Ok(finished);
    }
}
