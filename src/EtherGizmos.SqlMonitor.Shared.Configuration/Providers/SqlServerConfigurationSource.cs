using Microsoft.Extensions.Configuration;

namespace EtherGizmos.SqlMonitor.Shared.Configuration.Providers;

public class SqlServerConfigurationSource : IConfigurationSource
{
    public string ConnectionString { get; set; } = null!;

    public string Query { get; set; } = "select [key], [value] from [configuration];";

    public IConfigurationProvider Build(IConfigurationBuilder builder)
    {
        return new SqlServerConfigurationProvider(this);
    }
}
