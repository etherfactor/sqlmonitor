using System.Text.Json;

namespace EtherGizmos.SqlMonitor.Api.UnitTests;

internal static class JsonHelper
{
    internal static TEntity Deserialize<TEntity>(string json, TEntity model)
    {
        var entity = JsonSerializer.Deserialize<TEntity>(json) ?? throw new InvalidDataException();

        return entity;
    }
}
