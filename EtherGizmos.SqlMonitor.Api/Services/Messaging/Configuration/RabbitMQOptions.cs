namespace EtherGizmos.SqlMonitor.Api.Services.Messaging.Configuration;

public class RabbitMQOptions
{
    public string Host { get; set; } = "localhost";

    public short Port { get; set; } = 5672;

    public string? Username { get; set; }

    public string? Password { get; set; }

    public HostOptions[]? EndPoints { get; set; }

    public class HostOptions
    {
        public string Host { get; set; } = "localhost";

        public short Port { get; set; } = 5672;
    }
}
