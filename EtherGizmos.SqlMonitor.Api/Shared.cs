namespace EtherGizmos.SqlMonitor.Api;

internal static class Shared
{
    internal static TimeSpan HangfireInterval { get; private set; }

    internal static void Initialize(IConfiguration configuration)
    {
        HangfireInterval = configuration.GetValue<TimeSpan>("Hangfire:SchedulePollingInterval");
    }
}
