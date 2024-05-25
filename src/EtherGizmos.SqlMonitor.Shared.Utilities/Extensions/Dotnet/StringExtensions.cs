using System.Globalization;
using System.Text;
using System.Text.Json;

namespace EtherGizmos.SqlMonitor.Shared.Utilities.Extensions.Dotnet;

/// <summary>
/// Provides extension methods for <see cref="string"/>.
/// </summary>
public static class StringExtensions
{
    /// <summary>
    /// Converts a string to camel case.
    /// </summary>
    /// <param name="this">The string to convert.</param>
    /// <returns>The string in camel case.</returns>
    public static string ToCamelCase(this string @this)
    {
        if (@this is null)
            throw new ArgumentNullException(nameof(@this));

        if (@this.Trim() == "")
            return @this;

        return JsonNamingPolicy.CamelCase.ConvertName(@this);
    }

    /// <summary>
    /// Converts a string to snake case.
    /// </summary>
    /// <param name="this">The string to convert.</param>
    /// <returns>The string in snake case.</returns>
    public static string ToSnakeCase(this string @this)
    {
        if (@this is null)
            throw new ArgumentNullException(nameof(@this));

        if (@this.Trim() == "")
            return @this;

        var builder = new StringBuilder(@this.Length + Math.Min(2, @this.Length / 5));
        var previousCategory = default(UnicodeCategory?);

        for (var currentIndex = 0; currentIndex < @this.Length; currentIndex++)
        {
            var currentChar = @this[currentIndex];
            if (currentChar == '_')
            {
                builder.Append('_');
                previousCategory = null;
                continue;
            }

            var currentCategory = char.GetUnicodeCategory(currentChar);
            switch (currentCategory)
            {
                case UnicodeCategory.UppercaseLetter:
                case UnicodeCategory.TitlecaseLetter:
                    if (previousCategory == UnicodeCategory.SpaceSeparator ||
                        previousCategory == UnicodeCategory.LowercaseLetter ||
                        previousCategory != UnicodeCategory.DecimalDigitNumber &&
                        previousCategory != null &&
                        currentIndex > 0 &&
                        currentIndex + 1 < @this.Length &&
                        char.IsLower(@this[currentIndex + 1]))
                    {
                        builder.Append('_');
                    }

                    currentChar = char.ToLower(currentChar);
                    break;

                case UnicodeCategory.LowercaseLetter:
                case UnicodeCategory.DecimalDigitNumber:
                    if (previousCategory == UnicodeCategory.SpaceSeparator)
                    {
                        builder.Append('_');
                    }
                    break;

                default:
                    if (previousCategory != null)
                    {
                        previousCategory = UnicodeCategory.SpaceSeparator;
                    }
                    continue;
            }

            builder.Append(currentChar);
            previousCategory = currentCategory;
        }

        return builder.ToString();
    }
}
