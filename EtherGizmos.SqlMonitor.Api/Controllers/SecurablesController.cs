using AutoMapper;
using EtherGizmos.SqlMonitor.Api.Controllers.Abstractions;
using EtherGizmos.SqlMonitor.Api.Extensions;
using EtherGizmos.SqlMonitor.Api.Services.Abstractions;
using EtherGizmos.SqlMonitor.Models.Api.v1;
using EtherGizmos.SqlMonitor.Models.Database;
using EtherGizmos.SqlMonitor.Models.OData.Errors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.EntityFrameworkCore;

namespace EtherGizmos.SqlMonitor.Api.Controllers;

/// <summary>
/// Provides endpoints for <see cref="Securable"/> records.
/// </summary>
[Route(BasePath)]
public class SecurablesController : ExtendedODataController
{
    private const string BasePath = "/api/v1/securables";

    /// <summary>
    /// The logger to utilize.
    /// </summary>
    private ILogger Logger { get; }

    /// <summary>
    /// Allows conversion between database and DTO models.
    /// </summary>
    private IMapper Mapper { get; }

    /// <summary>
    /// Provides access to the storage of records.
    /// </summary>
    private ISecurableService SecurableService { get; }

    /// <summary>
    /// Queries stored records.
    /// </summary>
    private IQueryable<Securable> Securables => SecurableService.GetQueryable();

    /// <summary>
    /// Constructs the controller.
    /// </summary>
    /// <param name="logger">The logger to utilize.</param>
    /// <param name="mapper">Allows conversion between database and DTO models.</param>
    /// <param name="securableService">Provides access to the storage of records.</param>
    public SecurablesController(ILogger<SecurablesController> logger, IMapper mapper, ISecurableService securableService)
    {
        Logger = logger;
        Mapper = mapper;
        SecurableService = securableService;
    }

    /// <summary>
    /// Searches all records.
    /// </summary>
    /// <param name="queryOptions">The query options to use.</param>
    /// <returns>An awaitable task.</returns>
    [HttpGet]
    [Route(BasePath)]
    public async Task<IActionResult> Search(ODataQueryOptions<SecurableDTO> queryOptions)
    {
        var finished = await Securables.MapExplicitlyAndApplyQueryOptions(Mapper, queryOptions);
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
    public async Task<IActionResult> Get(string id, ODataQueryOptions<SecurableDTO> queryOptions)
    {
        queryOptions.EnsureValidForSingle();

        Securable? record = await Securables.SingleOrDefaultAsync(e => e.Id == id);
        if (record == null)
            return new ODataRecordNotFoundError<SecurableDTO>((e => e.Id, id)).GetResponse();

        var finished = record.MapExplicitlyAndApplyQueryOptions(Mapper, queryOptions);
        return Ok(finished);
    }
}
