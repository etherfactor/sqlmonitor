using EtherGizmos.SqlMonitor.Database.Remaps;
using FluentMigrator;
using FluentMigrator.Runner;
using FluentMigrator.Runner.Generators.MySql;
using FluentMigrator.Runner.Processors;
using Microsoft.Extensions.DependencyInjection;

namespace EtherGizmos.SqlMonitor.Database.Extensions;

public static class IMigrationRunnerBuilderExtensions
{
    public static IMigrationRunnerBuilder RemapMySql(this IMigrationRunnerBuilder @this)
    {
        @this.Services.AddScoped<MySqlQuoter, MySqlQuoterRemap>();
        @this.Services.AddScoped<IMySqlTypeMap, MySqlTypeRemap>();
        @this.Services
            .AddScoped<MySqlProcessorRemap>()
            .AddScoped<IMigrationProcessor>(sp => sp.GetRequiredService<MySqlProcessorRemap>())
            .AddScoped<MySqlGeneratorRemap>()
            .AddScoped<IMigrationGenerator>(sp => sp.GetRequiredService<MySqlGeneratorRemap>())
            .Configure<SelectingProcessorAccessorOptions>(opt => opt.ProcessorId = ProcessorId.MySql8 + "Remap");

        return @this;
    }
}
