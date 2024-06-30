using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace EtherGizmos.SqlMonitor.Shared.Configuration.Providers;

public class SqlServerConfigurationProvider : ConfigurationProvider
{
    private readonly SqlServerConfigurationSource _source;

    public SqlServerConfigurationProvider(SqlServerConfigurationSource source)
    {
        _source = source;
    }

    public override void Load()
    {
        var data = new Dictionary<string, string?>();

        var connection = new SqlConnection(_source.ConnectionString);
        connection.Open();

        using var query = new SqlCommand(_source.Query, connection);
        using var reader = query.ExecuteReader();

        while (reader.Read())
        {
            data.Add(reader.GetString(0), reader.GetString(1));
        }

        Data = data;
    }
}
