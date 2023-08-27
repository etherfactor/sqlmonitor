using AutoMapper;
using EtherGizmos.SqlMonitor.Api.Extensions;
using EtherGizmos.SqlMonitor.Api.Services.Data.Abstractions;
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
/// Provides endpoints for <see cref="Instance"/> records.
/// </summary>
[Route(BasePath)]
public class InstancesController : ODataController
{
    private const string BasePath = "/api/v1/instances";

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
    private IInstanceService InstanceService { get; }

    /// <summary>
    /// Provides access to saving records.
    /// </summary>
    private ISaveService SaveService { get; }

    /// <summary>
    /// Queries stored records.
    /// </summary>
    private IQueryable<Instance> Instances => InstanceService.GetQueryable();

    /// <summary>
    /// Constructs the controller.
    /// </summary>
    /// <param name="logger">The logger to utilize.</param>
    /// <param name="mapper">Allows conversion between database and DTO models.</param>
    /// <param name="instanceService">Provides access to the storage of records.</param>
    /// <param name="saveService">Provides access to saving records.</param>
    public InstancesController(ILogger<QueriesController> logger, IMapper mapper, IInstanceService instanceService, ISaveService saveService)
    {
        Logger = logger;
        Mapper = mapper;
        InstanceService = instanceService;
        SaveService = saveService;
    }

    /// <summary>
    /// Searches all records.
    /// </summary>
    /// <param name="queryOptions">The query options to use.</param>
    /// <returns>An awaitable task.</returns>
    [HttpGet]
    [Route(BasePath)]
    public async Task<IActionResult> Search(ODataQueryOptions<InstanceDTO> queryOptions)
    {
        var finished = await Instances.MapExplicitlyAndApplyQueryOptions(Mapper, queryOptions);
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
    public async Task<IActionResult> Get(Guid id, ODataQueryOptions<InstanceDTO> queryOptions)
    {
        queryOptions.EnsureValidForSingle();

        Instance? record = await Instances.SingleOrDefaultAsync(e => e.Id == id);
        if (record == null)
            return new ODataRecordNotFoundError<InstanceDTO>((e => e.Id, id)).GetResponse();

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
    public async Task<IActionResult> Create([FromBody] InstanceDTO newRecord, ODataQueryOptions<InstanceDTO> queryOptions)
    {
        queryOptions.EnsureValidForSingle();

        await newRecord.EnsureValid(Instances);

        Instance record = Mapper.Map<Instance>(newRecord);

        await record.EnsureValid(Instances);
        InstanceService.Add(record);

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
    public async Task<IActionResult> Update(Guid id, [FromBody] Delta<InstanceDTO> patchRecord, ODataQueryOptions<InstanceDTO> queryOptions)
    {
        queryOptions.EnsureValidForSingle();

        var testRecord = new InstanceDTO();
        patchRecord.Patch(testRecord);

        await testRecord.EnsureValid(Instances);

        Instance? record = await Instances.SingleOrDefaultAsync(e => e.Id == id);
        if (record == null)
            return new ODataRecordNotFoundError<InstanceDTO>((e => e.Id, id)).GetResponse();

        var recordAsDto = Mapper.MapExplicitly(record).To<InstanceDTO>();
        patchRecord.Patch(recordAsDto);

        Mapper.MergeInto(record).Using(recordAsDto);

        await record.EnsureValid(Instances);

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
        Instance? record = await Instances.SingleOrDefaultAsync(e => e.Id == id);
        if (record == null)
            return new ODataRecordNotFoundError<InstanceDTO>((e => e.Id, id)).GetResponse();

        InstanceService.Remove(record);

        await SaveService.SaveChangesAsync();

        return NoContent();
    }
}
