using AutoMapper;
using EtherGizmos.SqlMonitor.Api.Extensions;
using EtherGizmos.SqlMonitor.Api.Services.Abstractions;
using EtherGizmos.SqlMonitor.Models.Api.v1;
using EtherGizmos.SqlMonitor.Models.Database;
using EtherGizmos.SqlMonitor.Models.OData.Errors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Deltas;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Microsoft.EntityFrameworkCore;

namespace EtherGizmos.SqlMonitor.Api.Controllers;

/// <summary>
/// Provides endpoints for <see cref="Query"/> records.
/// </summary>
[Route(BasePath)]
public class QueriesController : ODataController
{
    private const string BasePath = "/api/v1/queries";

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
    private IQueryService QueryService { get; }

    /// <summary>
    /// Provides access to saving records.
    /// </summary>
    private ISaveService SaveService { get; }

    /// <summary>
    /// Queries stored records.
    /// </summary>
    private IQueryable<Query> Queries => QueryService.GetQueryable();

    /// <summary>
    /// Constructs the controller.
    /// </summary>
    /// <param name="logger">The logger to utilize.</param>
    /// <param name="mapper">Allows conversion between database and DTO models.</param>
    /// <param name="queryService">Provides access to the storage of records.</param>
    /// <param name="saveService">Provides access to saving records.</param>
    public QueriesController(ILogger<QueriesController> logger, IMapper mapper, IQueryService queryService, ISaveService saveService)
    {
        Logger = logger;
        Mapper = mapper;
        QueryService = queryService;
        SaveService = saveService;
    }

    /// <summary>
    /// Searches all records.
    /// </summary>
    /// <param name="queryOptions">The query options to use.</param>
    /// <returns>An awaitable task.</returns>
    [HttpGet]
    [Route(BasePath)]
    public async Task<IActionResult> Search(ODataQueryOptions<QueryDTO> queryOptions)
    {
        var finished = await Queries.MapExplicitlyAndApplyQueryOptions(Mapper, queryOptions);
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
    public async Task<IActionResult> Get(Guid id, ODataQueryOptions<QueryDTO> queryOptions)
    {
        queryOptions.EnsureValidForSingle();

        Query? record = await Queries.SingleOrDefaultAsync(e => e.Id == id);
        if (record == null)
            return new ODataRecordNotFoundError<PermissionDTO>((e => e.Id, id)).GetResponse();

        var finished = record.MapExplicitlyAndApplyQueryOptions(Mapper, queryOptions);
        return Ok(finished);
    }

    /// <summary>
    /// Creates a record.
    /// </summary>
    /// <param name="newRecord">The record to create.</param>
    /// <param name="queryOptions">The query options to use.</param>
    /// <returns>An awaitable task.</returns>
    [HttpPost]
    [Route(BasePath)]
    public async Task<IActionResult> Create([FromBody] QueryDTO newRecord, ODataQueryOptions<QueryDTO> queryOptions)
    {
        queryOptions.EnsureValidForSingle();

        await newRecord.EnsureValid(Queries);

        Query record = Mapper.Map<Query>(newRecord);

        await record.EnsureValid(Queries);
        QueryService.Add(record);

        await SaveService.SaveChangesAsync();

        var finished = record.MapExplicitlyAndApplyQueryOptions(Mapper, queryOptions);
        return Created(finished);
    }

    /// <summary>
    /// Modifies a record.
    /// </summary>
    /// <param name="id">The id of the record to modify.</param>
    /// <param name="patchRecord">The delta patch to apply.</param>
    /// <param name="queryOptions">The query options to use.</param>
    /// <returns>An awaitable task.</returns>
    [HttpPatch]
    [Route(BasePath + "({id})")]
    public async Task<IActionResult> Update(Guid id, [FromBody] Delta<QueryDTO> patchRecord, ODataQueryOptions<QueryDTO> queryOptions)
    {
        queryOptions.EnsureValidForSingle();

        var testRecord = new QueryDTO();
        patchRecord.Patch(testRecord);

        await testRecord.EnsureValid(Queries);

        Query? record = await Queries.SingleOrDefaultAsync(e => e.Id == id);
        if (record == null)
            return new ODataRecordNotFoundError<QueryDTO>((e => e.Id, id)).GetResponse();

        var recordAsDto = Mapper.MapExplicitly(record).To<QueryDTO>();
        patchRecord.Patch(recordAsDto);

        Mapper.MergeInto(record).Using(recordAsDto);

        await record.EnsureValid(Queries);

        await SaveService.SaveChangesAsync();

        var finished = record.MapExplicitlyAndApplyQueryOptions(Mapper, queryOptions);
        return Ok(finished);
    }

    /// <summary>
    /// Soft-deletes a record.
    /// </summary>
    /// <param name="id">The id of the record to delete.</param>
    /// <returns>An awaitable task.</returns>
    [HttpDelete]
    [Route(BasePath + "({id})")]
    public async Task<IActionResult> Delete(Guid id)
    {
        Query? record = await Queries.SingleOrDefaultAsync(e => e.Id == id);
        if (record == null)
            return new ODataRecordNotFoundError<QueryDTO>((e => e.Id, id)).GetResponse();

        QueryService.Remove(record);

        await SaveService.SaveChangesAsync();

        return NoContent();
    }
}
