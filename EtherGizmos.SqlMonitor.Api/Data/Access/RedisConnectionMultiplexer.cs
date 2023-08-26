using Serilog;
using StackExchange.Redis;
using System.Text;

namespace EtherGizmos.SqlMonitor.Api.Data.Access;

public static class RedisConnectionMultiplexer
{
    public static IConnectionMultiplexer Instance { get; private set; } = null!;

    public static void Initialize(ConfigurationOptions options)
    {
        if (Instance is not null)
            throw new InvalidOperationException("Can only initialize the connection multiplexer once.");

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
