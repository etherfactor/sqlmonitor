namespace EtherGizmos.SqlMonitor.Api.Data.Access;

public class MassTransitOptions
{
    public MassTransitServiceBusType Use { get; set; }

    public RabbitMQOptions RabbitMQ { get; set; } = new RabbitMQOptions();

    public class RabbitMQOptions
    {
        public string? Host { get; set; }

        public string? Username { get; set; }

        public string? Password { get; set; }
    }
}

public enum MassTransitServiceBusType
{
    Unknown,
    InMemory,
    RabbitMQ
}
