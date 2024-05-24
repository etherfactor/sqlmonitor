namespace EtherGizmos.SqlMonitor.Api.Helpers;

/// <summary>
/// Provides helper methods for <see cref="DateTime"/>, <see cref="DateTimeOffset"/>, <see cref="TimeSpan"/>, etc.
/// </summary>
public static class DateAndTimeHelper
{
    /// <summary>
    /// Returns the minimum of two <see cref="TimeSpan"/>.
    /// </summary>
    /// <param name="first">The first to compare.</param>
    /// <param name="second">The second to compare.</param>
    /// <returns>The minimum of the two.</returns>
    public static TimeSpan Min(TimeSpan first, TimeSpan second)
    {
        if (first <= second)
            return first;

        return second;
    }

    /// <summary>
    /// Returns the maximum of two <see cref="TimeSpan"/>.
    /// </summary>
    /// <param name="first">The first to compare.</param>
    /// <param name="second">The second to compare.</param>
    /// <returns>The maximum of the two.</returns>
    public static TimeSpan Max(TimeSpan first, TimeSpan second)
    {
        if (first >= second)
            return first;

        return second;
    }
}
