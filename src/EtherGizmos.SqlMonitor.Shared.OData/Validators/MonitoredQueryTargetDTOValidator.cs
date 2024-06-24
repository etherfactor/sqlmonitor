using EtherGizmos.SqlMonitor.Shared.Database.Services.Abstractions;
using EtherGizmos.SqlMonitor.Shared.Models.Api.v1;
using EtherGizmos.SqlMonitor.Shared.OData.Validators.Abstractions;
using EtherGizmos.SqlMonitor.Shared.Utilities.Abstractions;

namespace EtherGizmos.SqlMonitor.Shared.OData.Validators;

internal class MonitoredQueryTargetDTOValidator : MonitoredTargetValidatorBase,
    IModelValidator<MonitoredQueryTargetDTO>
{
    public MonitoredQueryTargetDTOValidator(
        IMonitoredEnvironmentService monitoredEnvironmentService,
        IMonitoredResourceService monitoredResourceService,
        IMonitoredSystemService monitoredSystemService)
        : base(
            monitoredEnvironmentService,
            monitoredResourceService,
            monitoredSystemService)
    { }

    public async Task ValidateAsync(MonitoredQueryTargetDTO model)
    {
        await ValidateInternalAsync(
            model.MonitoredSystemId,
            model.MonitoredResourceId,
            model.MonitoredEnvironmentId);
    }
}
