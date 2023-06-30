using System.Linq.Expressions;
using System.Reflection;

namespace EtherGizmos.SqlMonitor.Models.Extensions;

public static class ExpressionExtensions
{
    public static PropertyInfo GetPropertyInfo<TSource, TMember>(this Expression<Func<TSource, TMember>> @this)
    {
        if (@this.Body is not MemberExpression member)
        {
            throw new ArgumentException(string.Format(
                "Expression '{0}' refers to a method, not a property.",
                @this.ToString()));
        }

        if (member.Member is not PropertyInfo property)
        {
            throw new ArgumentException(string.Format(
                "Expression '{0}' refers to a field, not a property.",
                @this.ToString()));
        }

        //Type type = typeof(TSource);
        //if (property.ReflectedType != null && type != property.ReflectedType && !type.IsSubclassOf(property.ReflectedType))
        //{
        //    throw new ArgumentException(string.Format(
        //        "Expression '{0}' refers to a property that is not from type {1}.",
        //        @this.ToString(),
        //        type));
        //}

        return property;
    }
}
