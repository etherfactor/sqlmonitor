using System.Linq.Expressions;
using System.Reflection;

namespace EtherGizmos.SqlMonitor.Models.Extensions;

/// <summary>
/// Provides extension methods for <see cref="Expression{TDelegate}"/>.
/// </summary>
public static class ExpressionExtensions
{
    /// <summary>
    /// Fetches property information from a property expression.
    /// </summary>
    /// <typeparam name="TSource">The type of entity.</typeparam>
    /// <typeparam name="TMember">The type of entity member.</typeparam>
    /// <param name="this">Itself.</param>
    /// <returns>The property info of the selected property.</returns>
    /// <exception cref="ArgumentException"></exception>
    public static PropertyInfo GetPropertyInfo<TSource, TMember>(this Expression<Func<TSource, TMember>> @this)
    {
        //Cover each possible case of expression body
        MemberExpression? member = null;
        switch (@this.Body)
        {
            case UnaryExpression expression:
                //Handle convert (happens with GUID)
                if (expression.NodeType == ExpressionType.Convert)
                    if (expression.Operand is MemberExpression unaryMember)
                        member = unaryMember;
                break;

            case MemberExpression expression:
                //Handle basic member
                member = expression;
                break;
        }

        //Ensure there is a member being selected
        if (member is null)
        {
            throw new ArgumentException(string.Format(
                "Expression '{0}' refers to a method, not a property.",
                @this.ToString()));
        }

        //Ensure that member has property information
        if (member.Member is not PropertyInfo property)
        {
            throw new ArgumentException(string.Format(
                "Expression '{0}' refers to a field, not a property.",
                @this.ToString()));
        }

        return property;
    }
}
