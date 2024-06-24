namespace EtherGizmos.SqlMonitor.Shared.Configuration.Data;

public class ConnectionOptions
{
    public ConnectionType Type { get; set; }

    #region Database
    public bool IsDatabase =>
        Type == ConnectionType.MySql ||
        Type == ConnectionType.PostgreSql ||
        Type == ConnectionType.SqlServer;

    public ConnectionMySqlOptions MySql { get; set; } = new();

    public ConnectionPostgreSqlOptions PostgreSql { get; set; } = new();

    public ConnectionSqlServerOptions SqlServer { get; set; } = new();
    #endregion Database

    #region Message Broker
    public bool IsMessageBroker =>
        Type == ConnectionType.RabbitMq;

    public ConnectionsRabbitMQOptions RabbitMQ { get; set; } = new();
    #endregion Message Broker

    #region Cache
    public bool IsCache =>
        Type == ConnectionType.Redis;

    public ConnectionRedisOptions Redis { get; set; } = new();
    #endregion Cache
}
