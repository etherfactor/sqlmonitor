namespace EtherGizmos.SqlMonitor.Shared.OData.Services.Abstractions;

public interface IModelValidatorFactory
{
    IModelValidator<TModel> GetValidator<TModel>();
}
