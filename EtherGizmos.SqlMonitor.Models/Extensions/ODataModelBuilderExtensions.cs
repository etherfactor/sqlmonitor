using Microsoft.OData.ModelBuilder;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace EtherGizmos.SqlMonitor.Models.Extensions;

/// <summary>
/// Provides extension methods for <see cref="ODataModelBuilder"/>.
/// </summary>
internal static class ODataModelBuilderExtensions
{
    /// <summary>
    /// Adds an entity set to an OData model, reading from annotations.
    /// </summary>
    /// <typeparam name="TEntityType">The type of entity.</typeparam>
    /// <param name="this">Itself.</param>
    /// <returns>The entity set builder.</returns>
    /// <exception cref="InvalidOperationException"></exception>
    internal static EntitySetConfiguration<TEntityType> EntitySetWithAnnotations<TEntityType>(this ODataModelBuilder @this)
        where TEntityType : class
    {
        //Get the DisplayAttribute
        DisplayAttribute? attribute = typeof(TEntityType).GetCustomAttribute<DisplayAttribute>();

        //Load the entity set name, and ensure it exists
        string setName = attribute?.GroupName
            ?? throw new InvalidOperationException(string.Format("Type '{0}' must be annotated with a '{1}' and specify the '{2}' property",
                typeof(TEntityType).Name, nameof(DisplayAttribute), nameof(attribute.GroupName)));

        //Create an entity set and name it
        var configuration = @this.EntitySet<TEntityType>(setName);
        return configuration;
    }

    /// <summary>
    /// Adds an entity type to an OData model, reading from annotations.
    /// </summary>
    /// <typeparam name="TEntityType">The type of entity.</typeparam>
    /// <param name="this">Itself.</param>
    /// <returns>The entity type builder.</returns>
    /// <exception cref="InvalidOperationException"></exception>
    internal static EntityTypeConfiguration<TEntityType> EntityTypeWithAnnotations<TEntityType>(this ODataModelBuilder @this)
        where TEntityType : class
    {
        //Get the DisplayAttribute
        DisplayAttribute? attribute = typeof(TEntityType).GetCustomAttribute<DisplayAttribute>();

        //Load the entity type name, and ensure it exists
        string typeName = attribute?.Name
            ?? throw new InvalidOperationException(string.Format("Type '{0}' must be annotated with a '{1}' and specify the '{2}' property",
                typeof(TEntityType).Name, nameof(DisplayAttribute), nameof(attribute.Name)));

        //Create an entity type and name it
        var configuration = @this.EntityType<TEntityType>();
        configuration.Name = typeName;
        return configuration;
    }
}
