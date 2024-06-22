using Asp.Versioning;
using AutoMapper;
using EtherGizmos.SqlMonitor.Shared.Database.Services.Abstractions;
using EtherGizmos.SqlMonitor.Shared.Models.Api.v1;
using EtherGizmos.SqlMonitor.Shared.Models.Database;
using EtherGizmos.SqlMonitor.Shared.Models.Extensions;
using EtherGizmos.SqlMonitor.Shared.OData.Errors;
using EtherGizmos.SqlMonitor.Shared.OData.Exceptions;
using EtherGizmos.SqlMonitor.Shared.OData.Extensions;
using EtherGizmos.SqlMonitor.Shared.Redis.Caching.Abstractions;
using EtherGizmos.SqlMonitor.Shared.Utilities.Abstractions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Deltas;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Microsoft.EntityFrameworkCore;

namespace EtherGizmos.SqlMonitor.Api.Controllers.Api;

public class QueriesController : ODataController
{
    private const string BasePath = "api/v{version:apiVersion}/queries";

    private readonly ILogger _logger;
    private readonly IRecordCache _cache;
    private readonly IMapper _mapper;
    private readonly IModelValidatorFactory _modelValidatorFactory;
    private readonly IQueryService _queryService;
    private readonly ISaveService _saveService;
    private readonly IMetricService _metricService;

    /// <summary>
    /// Queries stored records.
    /// </summary>
    private IQueryable<Query> Queries => _queryService.GetQueryable();

    /// <summary>
    /// Constructs the controller.
    /// </summary>
    /// <param name="logger">The logger to utilize.</param>
    /// <param name="mapper">Allows conversion between database and DTO models.</param>
    /// <param name="queryService">Provides access to the storage of records.</param>
    /// <param name="saveService">Provides access to saving records.</param>
    /// <param name="metricService">Provides access to metric records.</param>
    public QueriesController(
        ILogger<QueriesController> logger,
        IRecordCache cache,
        IMapper mapper,
        IModelValidatorFactory modelValidatorFactory,
        IQueryService queryService,
        ISaveService saveService,
        IMetricService metricService)
    {
        _logger = logger;
        _cache = cache;
        _mapper = mapper;
        _modelValidatorFactory = modelValidatorFactory;
        _queryService = queryService;
        _saveService = saveService;
        _metricService = metricService;
    }

    /// <summary>
    /// Searches all records.
    /// </summary>
    /// <param name="queryOptions">The query options to use.</param>
    /// <returns>An awaitable task.</returns>
    [ApiVersion("0.1")]
    [HttpGet(BasePath)]
    public async Task<IActionResult> Search(ODataQueryOptions<QueryDTO> queryOptions)
    {
        var finished = await Queries.MapExplicitlyAndApplyQueryOptions(_mapper, queryOptions);
        return Ok(finished);
    }

    /// <summary>
    /// Finds a single record.
    /// </summary>
    /// <param name="id">The id of the record.</param>
    /// <param name="queryOptions">The query options to use.</param>
    /// <returns>An awaitable task.</returns>
    [ApiVersion("0.1")]
    [HttpGet(BasePath + "({id})")]
    public async Task<IActionResult> Get(Guid id, ODataQueryOptions<QueryDTO> queryOptions)
    {
        queryOptions.EnsureValidForSingle();

        Query? record = await Queries.SingleOrDefaultAsync(e => e.Id == id);
        if (record == null)
            return new ODataRecordNotFoundError<QueryDTO>((e => e.Id, id)).GetResponse();

        var finished = record.MapExplicitlyAndApplyQueryOptions(_mapper, queryOptions);
        return Ok(finished);
    }

    /// <summary>
    /// Creates a record.
    /// </summary>
    /// <param name="newRecord">The record to create.</param>
    /// <param name="queryOptions">The query options to use.</param>
    /// <returns>An awaitable task.</returns>
    [ApiVersion("0.1")]
    [HttpPost(BasePath)]
    public async Task<IActionResult> Create([FromBody] QueryDTO newRecord, ODataQueryOptions<QueryDTO> queryOptions)
    {
        queryOptions.EnsureValidForSingle();

        var validator = _modelValidatorFactory.GetValidator<QueryDTO>();
        await validator.ValidateAsync(newRecord);

        var allMetrics = _metricService.GetQueryable();
        foreach (var metricId in newRecord.Metrics.Select(e => e.MetricId).Distinct())
        {
            if (!await allMetrics.AnyAsync(e => e.Id == metricId))
            {
                var error = new ODataRecordNotFoundError<MetricDTO>((e => e.Id, metricId!));
                throw new ReturnODataErrorException(error);
            }
        }

        Query record = _mapper.Map<Query>(newRecord);

        var dbValidator = _modelValidatorFactory.GetValidator<Query>();
        await dbValidator.ValidateAsync(record);

        _queryService.Add(record);

        await _saveService.SaveChangesAsync();

        var finished = record.MapExplicitlyAndApplyQueryOptions(_mapper, queryOptions);
        return Created(finished);
    }

    /// <summary>
    /// Modifies a record.
    /// </summary>
    /// <param name="id">The id of the record to modify.</param>
    /// <param name="patchRecord">The delta patch to apply.</param>
    /// <param name="queryOptions">The query options to use.</param>
    /// <returns>An awaitable task.</returns>
    [ApiVersion("0.1")]
    [HttpPatch(BasePath + "({id})")]
    public async Task<IActionResult> Update(Guid id, [FromBody] Delta<QueryDTO> patchRecord, ODataQueryOptions<QueryDTO> queryOptions)
    {
        queryOptions.EnsureValidForSingle();

        var testRecord = new QueryDTO();
        patchRecord.Patch(testRecord);

        var validator = _modelValidatorFactory.GetValidator<QueryDTO>();
        await validator.ValidateAsync(testRecord);

        Query? record = await Queries.SingleOrDefaultAsync(e => e.Id == id);
        if (record == null)
            return new ODataRecordNotFoundError<QueryDTO>((e => e.Id, id)).GetResponse();

        var allMetrics = _metricService.GetQueryable();
        foreach (var metricId in testRecord.Metrics.Select(e => e.MetricId).Distinct())
        {
            if (!await allMetrics.AnyAsync(e => e.Id == metricId))
            {
                var error = new ODataRecordNotFoundError<MetricDTO>((e => e.Id, metricId!));
                throw new ReturnODataErrorException(error);
            }
        }

        var recordAsDto = _mapper.MapExplicitly(record).To<QueryDTO>();
        patchRecord.Patch(recordAsDto);

        _mapper.MergeInto(record).Using(recordAsDto);

        var dbValidator = _modelValidatorFactory.GetValidator<Query>();
        await dbValidator.ValidateAsync(record);

        await _saveService.SaveChangesAsync();
        await _cache.EntitySet<Query>().AddAsync(record);

        var finished = record.MapExplicitlyAndApplyQueryOptions(_mapper, queryOptions);
        return Ok(finished);
    }

    /// <summary>
    /// Soft-deletes a record.
    /// </summary>
    /// <param name="id">The id of the record to delete.</param>
    /// <returns>An awaitable task.</returns>
    [ApiVersion("0.1")]
    [HttpDelete(BasePath + "({id})")]
    public async Task<IActionResult> Delete(Guid id)
    {
        Query? record = await Queries.SingleOrDefaultAsync(e => e.Id == id);
        if (record == null)
            return new ODataRecordNotFoundError<QueryDTO>((e => e.Id, id)).GetResponse();

        _queryService.Remove(record);

        await _saveService.SaveChangesAsync();
        await _cache.EntitySet<Query>().RemoveAsync(record);

        return NoContent();
    }
}
