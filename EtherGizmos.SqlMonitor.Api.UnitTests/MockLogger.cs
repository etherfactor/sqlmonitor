using Microsoft.Extensions.Logging;

namespace EtherGizmos.SqlMonitor.Api.UnitTests;

public class ProxyLogger<T> : ILogger<T>
{
    private readonly ILogger Logger;

    public ProxyLogger(ILogger proxee)
    {
        this.Logger = proxee;
    }

    public IDisposable? BeginScope<TState>(TState state) where TState : notnull
    {
        return Logger.BeginScope<TState>(state);
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
