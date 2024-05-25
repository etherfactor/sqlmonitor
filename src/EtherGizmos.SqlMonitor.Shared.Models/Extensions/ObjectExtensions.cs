namespace EtherGizmos.SqlMonitor.Shared.Models.Extensions;

/// <summary>
/// Provides extension methods for <see cref="object"/>.
/// </summary>
internal static class ObjectExtensions
{
    /// <summary>
    /// Converts an object to an <see cref="IEnumerable{T}"/>.
    /// </summary>
    /// <typeparam name="T">The object type.</typeparam>
    /// <param name="this">Itself.</param>
    /// <returns>Itself, as an enumerable.</returns>
    internal static IEnumerable<T> Yield<T>(this T @this)
    {
        yield return @this;
    }
}
