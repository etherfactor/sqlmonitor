using EtherGizmos.SqlMonitor.Shared.Database.Services.Abstractions;
using EtherGizmos.SqlMonitor.Shared.Models.Api.v1;
using EtherGizmos.SqlMonitor.Shared.OData.Errors;
using EtherGizmos.SqlMonitor.Shared.OData.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace EtherGizmos.SqlMonitor.Shared.OData.Validators.Abstractions;

internal class MonitoredTargetValidatorBase
{
    private readonly IMonitoredEnvironmentService _monitoredEnvironmentService;
    private readonly IMonitoredResourceService _monitoredResourceService;
    private readonly IMonitoredSystemService _monitoredSystemService;

    public MonitoredTargetValidatorBase(
        IMonitoredEnvironmentService monitoredEnvironmentService,
        IMonitoredResourceService monitoredResourceService,
        IMonitoredSystemService monitoredSystemService)
    {
        _monitoredEnvironmentService = monitoredEnvironmentService;
        _monitoredResourceService = monitoredResourceService;
        _monitoredSystemService = monitoredSystemService;
    }

    protected async Task ValidateInternalAsync(Guid? systemId, Guid? resourceId, Guid? environmentId)
    {
        var systems = _monitoredSystemService.GetQueryable();
        var resources = _monitoredResourceService.GetQueryable();
        var environments = _monitoredEnvironmentService.GetQueryable();

        if (systemId is not null && !await systems.AnyAsync(e => e.Id == systemId))
        {
            var error = new ODataRecordNotFoundError<MonitoredSystemDTO>((e => e.Id, systemId));
            throw new ReturnODataErrorException(error);
        }

        if (resourceId is not null && !await resources.AnyAsync(e => e.Id == resourceId))
        {
            var error = new ODataRecordNotFoundError<MonitoredResourceDTO>((e => e.Id, resourceId));
            throw new ReturnODataErrorException(error);
        }

        if (environmentId is not null && !await environments.AnyAsync(e => e.Id == environmentId))
        {
            var error = new ODataRecordNotFoundError<MonitoredEnvironmentDTO>((e => e.Id, environmentId));
            throw new ReturnODataErrorException(error);
        }
    }
}
