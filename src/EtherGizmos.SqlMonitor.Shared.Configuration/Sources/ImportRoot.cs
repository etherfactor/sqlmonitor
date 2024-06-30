using EtherGizmos.SqlMonitor.Shared.Configuration.Data;
using Microsoft.Extensions.Configuration;

namespace EtherGizmos.SqlMonitor.Shared.Configuration.Sources;

public class ImportRoot
{
    private readonly ConnectionRoot _connectionRoot;

    public ImportRoot(ConnectionRoot connectionRoot)
    {
        _connectionRoot = connectionRoot;
    }

    public Dictionary<string, ImportOptions> Imports { get; set; } = new();

    public void Apply(IConfigurationBuilder builder)
    {
        var sorted = Imports.OrderBy(e => e.Value.Priority)
            .ToList();

        foreach (var pair in sorted)
        {
            var value = pair.Value;
            switch (value.Type)
            {
                case ImportType.Environment:
                    builder.AddEnvironmentVariables();
                    break;

                case ImportType.File:
                    builder.AddJsonFile(value.File.Path, value.Optional, true);
                    break;

                case ImportType.SqlServer:
                    var sqlConnection = _connectionRoot.Connections[value.SqlServer.ConnectionId];
                    builder.AddSqlServer(sqlConnection.SqlServer.ConnectionStringValue, value.SqlServer.Query);
                    break;

                default:
                    throw new NotImplementedException();
            }
        }
    }
}
