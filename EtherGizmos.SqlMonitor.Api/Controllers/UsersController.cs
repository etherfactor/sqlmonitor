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

[Route(BasePath)]
public class UsersController : ODataController
{
    private const string BasePath = "/api/v1/users";

    private readonly ILogger _logger;
    private readonly IMapper _mapper;
    private readonly IUserService _userService;
    private readonly ISaveService _saveService;

    /// <summary>
    /// Queries stored records.
    /// </summary>
    private IQueryable<User> Users => _userService.GetQueryable();

    /// <summary>
    /// Constructs the controller.
    /// </summary>
    /// <param name="logger">The logger to utilize.</param>
    /// <param name="mapper">Allows conversion between database and DTO models.</param>
    /// <param name="userService">Provides access to the storage of records.</param>
    /// <param name="saveService">Provides access to saving records.</param>
    public UsersController(
        ILogger<UsersController> logger,
        IMapper mapper,
        IUserService userService,
        ISaveService saveService)
    {
        _logger = logger;
        _mapper = mapper;
        _userService = userService;
        _saveService = saveService;
    }

    /// <summary>
    /// Searches all records.
    /// </summary>
    /// <param name="queryOptions">The query options to use.</param>
    /// <returns>An awaitable task.</returns>
    [HttpGet]
    [Route(BasePath)]
    public async Task<IActionResult> Search(ODataQueryOptions<UserDTO> queryOptions)
    {
        var finished = await Users.MapExplicitlyAndApplyQueryOptions(_mapper, queryOptions);
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
    public async Task<IActionResult> Get(Guid id, ODataQueryOptions<UserDTO> queryOptions)
    {
        queryOptions.EnsureValidForSingle();

        User? record = await Users.SingleOrDefaultAsync(e => e.Id == id);
        if (record == null)
            return new ODataRecordNotFoundError<UserDTO>((e => e.Id, id)).GetResponse();

        var finished = record.MapExplicitlyAndApplyQueryOptions(_mapper, queryOptions);
        return Ok(finished);
    }

    [HttpPost]
    [Route(BasePath)]
    public async Task<IActionResult> Create([FromBody] UserDTO record, ODataQueryOptions<UserDTO> queryOptions)
    {
        queryOptions.EnsureValidForSingle();

        await record.EnsureValid(Users);

        User newRecord = _mapper.Map<User>(record);

        await newRecord.EnsureValid(Users);
        _userService.Add(newRecord);

        await _saveService.SaveChangesAsync();

        var finished = newRecord.MapExplicitlyAndApplyQueryOptions(_mapper, queryOptions);
        return Created(finished);
    }

    [HttpPatch]
    [Route(BasePath + "({id})")]
    public async Task<IActionResult> Update(Guid id, [FromBody] Delta<UserDTO> patchRecord, ODataQueryOptions<UserDTO> queryOptions)
    {
        queryOptions.EnsureValidForSingle();

        var testRecord = new UserDTO();
        patchRecord.Patch(testRecord);

        await testRecord.EnsureValid(Users);

        User? record = await Users.SingleOrDefaultAsync(e => e.Id == id);
        if (record == null)
            return new ODataRecordNotFoundError<UserDTO>((e => e.Id, id)).GetResponse();

        var recordAsDto = _mapper.MapExplicitly(record).To<UserDTO>();
        patchRecord.Patch(recordAsDto);

        _mapper.MergeInto(record).Using(recordAsDto);

        await record.EnsureValid(Users);

        await _saveService.SaveChangesAsync();

        var finished = record.MapExplicitlyAndApplyQueryOptions(_mapper, queryOptions);
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
        User? record = await Users.SingleOrDefaultAsync(e => e.Id == id);
        if (record == null)
            return new ODataRecordNotFoundError<UserDTO>((e => e.Id, id)).GetResponse();

        _userService.Remove(record);

        await _saveService.SaveChangesAsync();

        return NoContent();
    }
}
