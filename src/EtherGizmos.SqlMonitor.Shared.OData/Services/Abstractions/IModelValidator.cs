namespace EtherGizmos.SqlMonitor.Shared.OData.Services.Abstractions;

public interface IModelValidator<TModel>
{
    public Task ValidateAsync(TModel model);
}
