using FluentMigrator.Runner.Initialization;
using FluentMigrator.Runner.Processors;
using FluentMigrator.Runner.Processors.MySql;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace EtherGizmos.SqlMonitor.Database.Remaps;

internal class MySqlProcessorRemap : MySql8Processor
{
    public override string DatabaseType => base.DatabaseType + "Remap";

    public MySqlProcessorRemap(
        MySqlDbFactory factory,
        MySqlGeneratorRemap generator,
        ILogger<MySqlProcessorRemap> logger,
        IOptionsSnapshot<ProcessorOptions> options,
        IConnectionStringAccessor connectionStringAccessor)
        : base(factory, generator, logger, options, connectionStringAccessor)
    {
    }
}
