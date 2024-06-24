namespace EtherGizmos.SqlMonitor.Shared.Utilities.Abstractions;

public interface IModelValidatorFactory
{
    IModelValidator<TModel> GetValidator<TModel>();
}
