using System.Text;

namespace EtherGizmos.SqlMonitor.Api.Extensions;

public static class ScoreExtensions
{
    public static double TryGetScore(this object @this)
    {
        if (@this is null)
            return 0;

        switch (@this)
        {
            case bool value:
                return value.GetScore();

            case DateTime value:
                return value.GetScore();

            case DateTimeOffset value:
                return value.GetScore();

            case double value:
                return value;

            case int value:
                return value.GetScore();

            case short value:
                return value.GetScore();

            case string value:
                return value.GetScore();

            default:
                throw new InvalidOperationException($"Unrecognized type: {@this.GetType()}.");
        }
    }

    public static double GetScore(this bool @this)
    {
        return @this ? 1 : 0;
    }

    public static double GetScore(this DateTime @this)
    {
        return @this.Ticks;
    }

    public static double GetScore(this DateTimeOffset @this)
    {
        return @this.UtcTicks;
    }

    public static double GetScore(this int @this)
    {
        return @this;
    }

    public static double GetScore(this long @this)
    {
        return @this;
    }

    public static double GetScore(this short @this)
    {
        return @this;
    }

    public static double GetScore(this string @this)
    {
        var length = Math.Min(8, @this.Length);
        var preProcessed = @this.Substring(0, length).ToLower();

        var builder = new StringBuilder(preProcessed);
        for (var i = length; i < 8; i++)
        {
            builder.Append('\0');
        }

        var processed = builder.ToString();
        var bytes = Encoding.UTF8.GetBytes(processed);
        if (BitConverter.IsLittleEndian)
        {
            Array.Reverse(bytes);
        }

        var score = BitConverter.ToDouble(bytes, 0);
        return score;
    }
}
