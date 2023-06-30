using Microsoft.OData.ModelBuilder;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace EtherGizmos.SqlMonitor.Models.Extensions;

internal static class ODataModelBuilderExtensions
{
    internal static EntitySetConfiguration<TEntityType> EntitySetWithAnnotations<TEntityType>(this ODataModelBuilder @this)
        where TEntityType : class
    {
        DisplayAttribute? attribute = typeof(TEntityType).GetCustomAttribute<DisplayAttribute>();
        string setName = attribute?.GroupName
            ?? throw new InvalidOperationException(string.Format("Type '{0}' must be annotated with a '{1}' and specify the '{2}' property",
                typeof(TEntityType).Name, nameof(DisplayAttribute), nameof(attribute.GroupName)));

        var configuration = @this.EntitySet<TEntityType>(setName);
        return configuration;
    }

    internal static EntityTypeConfiguration<TEntityType> EntityTypeWithAnnotations<TEntityType>(this ODataModelBuilder @this)
        where TEntityType : class
    {
        DisplayAttribute? attribute = typeof(TEntityType).GetCustomAttribute<DisplayAttribute>();
        string typeName = attribute?.GroupName
            ?? throw new InvalidOperationException(string.Format("Type '{0}' must be annotated with a '{1}' and specify the '{2}' property",
                typeof(TEntityType).Name, nameof(DisplayAttribute), nameof(attribute.Name)));

        var configuration = @this.EntityType<TEntityType>();
        configuration.Name = typeName;

        return configuration;
    }
}
