namespace EtherGizmos.SqlMonitor.Api.Services.Data.Access;

public interface ISqlConnectionService
{
    Task<ISqlConnection> CreateConnectionAsync(string connectionString);
}

public interface ISqlConnection
{

}

public interface ISqlDataSet
{
    IAsyncEnumerable<ISqlDataRow> ReadRowsAsync();
}

public interface ISqlDataRow
{

}
