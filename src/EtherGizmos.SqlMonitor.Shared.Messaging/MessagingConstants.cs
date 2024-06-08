namespace EtherGizmos.SqlMonitor.Shared.Messaging;

/// <summary>
/// Constants used for message brokers.
/// </summary>
public static class MessagingConstants
{
    /// <summary>
    /// Constants for RabbitMQ.
    /// </summary>
    public static class RabbitMQ
    {
        /// <summary>
        /// The default RabbitMQ port.
        /// </summary>
        public const int Port = 5672;
    }

    /// <summary>
    /// Constants for queue names.
    /// </summary>
    public static class Queues
    {
        public const string CoordinatorAgentQueryExecute = "coordinator.agent.query.execute";

        public const string CoordinatorAgentScriptExecute = "coordinator.agent.script.execute";

        public const string AgentCoordinatorQueryResult = "agent.coordinator.query.result";

        public const string AgentCoordinatorScriptResult = "agent.coordinator.script.result";
    }

    public static class Claims
    {
        public const string Id = "id";

        public const string TargetType = "target_type";
    }
}
