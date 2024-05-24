using Microsoft.Extensions.Logging;

namespace EtherGizmos.SqlMonitor.UnitTests;

public class ProxyLogger<T> : ILogger<T>
{
    private readonly ILogger Logger;

    public ProxyLogger(ILogger proxee)
    {
        Logger = proxee;
    }

    public IDisposable? BeginScope<TState>(TState state) where TState : notnull
    {
        return Logger.BeginScope(state);
    }

    public bool IsEnabled(LogLevel logLevel)
    {
        return Logger.IsEnabled(logLevel);
    }

    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
    {
        Logger.Log(logLevel, eventId, state, exception, formatter);
    }
}
