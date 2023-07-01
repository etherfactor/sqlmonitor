namespace EtherGizmos.SqlMonitor.Api.Services.Abstractions;

public interface IQueryableService<T>
    where T : class
{
    IQueryable<T> GetQueryable();

    Task AddOrUpdate(T record);
}
