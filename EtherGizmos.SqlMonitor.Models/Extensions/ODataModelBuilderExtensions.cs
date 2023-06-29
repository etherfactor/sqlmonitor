using Microsoft.OData.ModelBuilder;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace EtherGizmos.SqlMonitor.Models.Extensions;

internal static class ODataModelBuilderExtensions
{
    internal static EntityTypeConfiguration<TEntityType> HasKeyWithAnnotations<TEntityType, TKey>(
        this EntityTypeConfiguration<TEntityType> @this, Expression<Func<TEntityType, TKey>> keyDefinitionExpression)
        where TEntityType : class
    {
        @this.HasKey(keyDefinitionExpression);

        PropertyInfo property = keyDefinitionExpression.GetPropertyInfo();
        DisplayAttribute? attribute = property.GetCustomAttribute<DisplayAttribute>();
        if (attribute != null)
        {
            
        }

        return @this;
    }

    internal static EntityTypeConfiguration<TEntityType> EntityTypeWithAnnotations<TEntityType>(this ODataModelBuilder @this)
        where TEntityType : class
    {
        var configuration = @this.EntityType<TEntityType>();
        DisplayAttribute? attribute = typeof(TEntityType).GetCustomAttribute<DisplayAttribute>();
        if (attribute != null)
        {
            configuration.Name = attribute.Name;
        }

        return configuration;
    }
}
