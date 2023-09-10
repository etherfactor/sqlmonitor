using AutoMapper;
using EtherGizmos.SqlMonitor.Api.Controllers.Abstractions;
using EtherGizmos.SqlMonitor.Api.Extensions;
using EtherGizmos.SqlMonitor.Api.Services.Data.Abstractions;
using EtherGizmos.SqlMonitor.Models.Api.v1;
using EtherGizmos.SqlMonitor.Models.Database;
using EtherGizmos.SqlMonitor.Models.OData.Errors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.EntityFrameworkCore;

namespace EtherGizmos.SqlMonitor.Api.Controllers;

/// <summary>
/// Provides endpoints for <see cref="Permission"/> records.
/// </summary>
[Route(BasePath)]
public class PermissionsController : ExtendedODataController
{
    private const string BasePath = "/api/v1/permissions";

    private readonly ILogger _logger;
    private readonly IMapper _mapper;
    private readonly IPermissionService _permissionService;

    /// <summary>
    /// Queries stored records.
    /// </summary>
    private IQueryable<Permission> Permissions => _permissionService.GetQueryable();

    /// <summary>
    /// Constructs the controller.
    /// </summary>
    /// <param name="logger">The logger to utilize.</param>
    /// <param name="mapper">Allows conversion between database and DTO models.</param>
    /// <param name="permissionService">Provides access to the storage of records.</param>
    public PermissionsController(
        ILogger<PermissionsController> logger,
        IMapper mapper,
        IPermissionService permissionService)
    {
        _logger = logger;
        _mapper = mapper;
        _permissionService = permissionService;
    }

    /// <summary>
    /// Searches all records.
    /// </summary>
    /// <param name="queryOptions">The query options to use.</param>
    /// <returns>An awaitable task.</returns>
    [HttpGet]
    [Route(BasePath)]
    public async Task<IActionResult> Search(ODataQueryOptions<PermissionDTO> queryOptions)
    {
        var finished = await Permissions.MapExplicitlyAndApplyQueryOptions(_mapper, queryOptions);
        return Ok(finished);
    }

    /// <summary>
    /// Finds a single record.
    /// </summary>
    /// <param name="id">The id of the record.</param>
    /// <param name="queryOptions">The query options to use.</param>
    /// <returns>An awaitable task.</returns>
    [HttpGet]
    [Route(BasePath + "({id})")]
    public async Task<IActionResult> Get(string id, ODataQueryOptions<PermissionDTO> queryOptions)
    {
        queryOptions.EnsureValidForSingle();

        Permission? record = await Permissions.SingleOrDefaultAsync(e => e.Id == id);
        if (record == null)
            return new ODataRecordNotFoundError<PermissionDTO>((e => e.Id, id)).GetResponse();

        var finished = record.MapExplicitlyAndApplyQueryOptions(_mapper, queryOptions);
        return Ok(finished);
    }
}
