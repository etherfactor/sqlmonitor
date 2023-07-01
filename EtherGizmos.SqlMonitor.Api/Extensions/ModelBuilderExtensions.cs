using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace EtherGizmos.SqlMonitor.Api.Extensions;

internal static class ModelBuilderExtensions
{
    internal static ModelBuilder AddGlobalValueConverter<TProvider, TModel>(this ModelBuilder @this, ValueConverter<TModel, TProvider> converter)
    {
        foreach (var entityType in @this.Model.GetEntityTypes())
        {
            foreach (var property in entityType.GetProperties())
            {
                if (property.ClrType == typeof(TModel))
                {
                    property.SetValueConverter(converter);
                }
            }
        }

        return @this;
    }
}
