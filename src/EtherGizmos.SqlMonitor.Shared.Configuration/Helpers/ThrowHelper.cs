using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using System.Reflection;

namespace EtherGizmos.SqlMonitor.Shared.Configuration.Helpers;

internal static class ThrowHelper
{
    [DoesNotReturn]
    internal static void ForMissingConfiguration<TOptions, TProperty>(string rootPath, TOptions options, Expression<Func<TOptions, TProperty>> propertyExpression)
    {
        var propertyName = propertyExpression.GetPropertyInfo().Name;
        ForMissingConfiguration(rootPath, options, propertyName, typeof(TProperty));
    }

    [DoesNotReturn]
    internal static void ForMissingConfiguration<TOptions>(string rootPath, TOptions options, string propertyName, Type propertyType)
    {
        throw new InvalidOperationException($"The property at configuration path '{rootPath}:{propertyName}' of type '{propertyType}' must be specified.");
    }

    [DoesNotReturn]
    internal static void ForInvalidConfiguration<TOptions, TProperty>(string rootPath, TOptions options, Expression<Func<TOptions, TProperty>> propertyExpression, string intendedValue)
    {
        var propertyName = propertyExpression.GetPropertyInfo().Name;
        throw new InvalidOperationException($"The property at configuration path '{rootPath}:{propertyName}' of type '{typeof(TProperty)}' must {intendedValue}.");
    }

    private static PropertyInfo GetPropertyInfo<TObject, TProperty>(this Expression<Func<TObject, TProperty>> propertyExpression)
    {
        MemberExpression? result = null;
        if (propertyExpression.Body is UnaryExpression unaryExpression)
        {
            if (unaryExpression.Operand is MemberExpression memberExpression)
            {
                result = memberExpression;
            }
            else
            {
                throw new ArgumentException("Expression does not select a member.");
            }
        }
        else if (propertyExpression.Body is MemberExpression memberExpression)
        {
            result = memberExpression;
        }
        else
        {
            throw new ArgumentException("Expression does not select a member.");
        }

        if (result.Member is PropertyInfo propertyInfo)
        {
            return propertyInfo;
        }
        else
        {
            throw new ArgumentException("Expression does not select a member.");
        }
    }
}
