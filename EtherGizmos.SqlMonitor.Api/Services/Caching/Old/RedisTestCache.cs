using StackExchange.Redis;

namespace EtherGizmos.SqlMonitor.Api.Services.Caching;

public class RedisTestCache
{
    private readonly IDatabase _database;

    public RedisTestCache(IDatabase database)
    {
        _database = database;
    }

    async Task Test()
    {
        var writeTransaction = _database.CreateTransaction();
        await writeTransaction.HashSetAsync(
            "sqlpulse:queries:49e24d60-591e-4ef2-d3ae-08dbac977560",
            new HashEntry[]
            {
                new HashEntry("created_at_utc", "2023-09-03 16:04:31.5300000"),
                new HashEntry("name", "Test 1"),
                new HashEntry("is_active", "true"),
                new HashEntry("is_soft_deleted", "false"),
                new HashEntry("sql_text", "select 1 as [value];"),
                new HashEntry("run_frequency", "PT5S"),
                new HashEntry("next_run_at_utc", "2023-09-03 16:05:00.0000000")
            });
        await writeTransaction.SortedSetAddAsync(
            "sqlpulse:queries:$index:$primary",
            "49e24d60-591e-4ef2-d3ae-08dbac977560", 0);
        await writeTransaction.SortedSetAddAsync(
            "sqlpulse:queries:$index:is_active",
            "49e24d60-591e-4ef2-d3ae-08dbac977560", 1);
        await writeTransaction.ExecuteAsync();

        var searchGuid = Guid.NewGuid();
        var readTransaction = _database.CreateTransaction();
        await readTransaction.SortedSetRangeAndStoreAsync(
            "sqlpulse:queries:$index:is_active",
            $"sqlpulse:queries:$index:is_active:{searchGuid}",
            1, 1, sortedSetOrder: SortedSetOrder.ByScore);
        await readTransaction.SortAsync(
            $"sqlpulse:queries:$index:is_active:{searchGuid}",
            sortType: SortType.Numeric,
            by: "nosort",
            get: new RedisValue[]
            {
                "#",
                "sqlpulse:queries:*->created_at_utc",
                "sqlpulse:queries:*->name",
                "sqlpulse:queries:*->is_active",
                "sqlpulse:queries:*->is_soft_deleted",
                "sqlpulse:queries:*->sql_text",
                "sqlpulse:queries:*->run_frequency",
                "sqlpulse:queries:*->next_run_at_utc"
            });
        await writeTransaction.ExecuteAsync();

        BitConverter.Int64BitsToDouble(1);
    }
}
