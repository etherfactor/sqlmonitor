using EtherGizmos.SqlMonitor.Shared.Messaging.Messages;
using System.Data.Common;

namespace EtherGizmos.SqlMonitor.Agent.Core.Helpers;

public static class MessageHelper
{
    public static async Task ReadIntoMessageAsync(
        QueryExecuteMessage executeMessage,
        QueryResultMessage resultMessage,
        DbDataReader reader)
    {
        var count = reader.FieldCount;
        while (await reader.ReadAsync())
        {
            var currentValues = new List<(int MetricId, double Value)>();
            string? currentBucket = null;
            DateTimeOffset? currentTimestampUtc = null;

            //Place each column as a key-value pair in the result
            for (int i = 0; i < count; i++)
            {
                var variableKey = reader.GetName(i);
                var variableValue = reader.GetValue(i);

                if (variableKey.Equals(executeMessage.BucketColumn, StringComparison.CurrentCultureIgnoreCase))
                {
                    currentBucket = variableValue.ToString();
                }
                else if (variableKey.Equals(executeMessage.TimestampUtcColumn, StringComparison.CurrentCultureIgnoreCase))
                {
                    if (DateTimeOffset.TryParse(variableValue.ToString(), out DateTimeOffset parsed))
                    {
                        currentTimestampUtc = parsed;
                    }
                }
                else
                {
                    var maybeMetric = executeMessage.Metrics.SingleOrDefault(e => variableKey.Equals(e.ValueColumn, StringComparison.InvariantCultureIgnoreCase));
                    if (maybeMetric is not null && double.TryParse(variableValue.ToString(), out double variableDouble))
                    {
                        currentValues.Add((maybeMetric.MetricId, variableDouble));
                    }
                }
            }

            foreach (var currentValue in currentValues)
            {
                resultMessage.AddMetricValue(currentValue.MetricId, currentBucket, currentTimestampUtc, currentValue.Value);
            }
        }
    }
}
