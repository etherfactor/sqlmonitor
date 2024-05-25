using Serilog;
using StackExchange.Redis;
using System.Text;

namespace EtherGizmos.SqlMonitor.Shared.Redis.Caching;

public static class RedisConnectionMultiplexer
{
    public static IConnectionMultiplexer Instance { get; private set; } = null!;

    public static void Initialize(ConfigurationOptions options)
    {
        if (Instance is not null)
            return;

        var writer = new TextWriterLogger();
        Instance = ConnectionMultiplexer.Connect(options, writer);
    }

    private class TextWriterLogger : TextWriter
    {
        public override Encoding Encoding => Encoding.Default;

        public override void Write(string? value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return;

            Log.Logger.Information(value);
        }
    }
}
