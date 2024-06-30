using EtherGizmos.SqlMonitor.Shared.Configuration.Providers;
using Microsoft.Extensions.Configuration;

namespace EtherGizmos.SqlMonitor.Shared.Configuration;

public static class IConfigurationExtensions
{
    public static IConfigurationBuilder AddSqlServer(
        this IConfigurationBuilder @this, string connectionString, string? query = null)
    {
        var source = new SqlServerConfigurationSource()
        {
            ConnectionString = connectionString
        };

        if (query is not null)
        {
            source.Query = query;
        }

        return @this.Add(source);
    }

    public static IConfigurationBuilder AddSqlServer(
        this IConfigurationBuilder @this, Action<SqlServerConfigurationSource> configureSource)
    {
        return @this.Add(configureSource);
    }
}
