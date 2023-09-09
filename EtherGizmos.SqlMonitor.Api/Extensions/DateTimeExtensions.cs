namespace EtherGizmos.SqlMonitor.Api.Extensions;

public static class DateTimeExtensions
{
    public static DateTimeOffset Floor(this DateTimeOffset @this, TimeSpan interval)
    {
        var ticks = @this.UtcTicks / interval.Ticks;
        return @this.AddTicks(ticks * interval.Ticks - @this.UtcTicks);
    }

    public static DateTimeOffset Round(this DateTimeOffset @this, TimeSpan interval)
    {
        var ticks = (@this.UtcTicks + (interval.Ticks / 2) + 1) / interval.Ticks;
        return @this.AddTicks(ticks * interval.Ticks - @this.UtcTicks);
    }

    public static DateTimeOffset Ceiling(this DateTimeOffset @this, TimeSpan interval)
    {
        var ticks = (@this.UtcTicks + interval.Ticks - 1) / interval.Ticks;
        return @this.AddTicks(ticks * interval.Ticks - @this.UtcTicks);
    }
}
