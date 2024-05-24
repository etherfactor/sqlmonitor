namespace EtherGizmos.SqlMonitor.Api.Extensions;

/// <summary>
/// Provides extension methods for <see cref="IEnumerable{T}"/>.
/// </summary>
public static class IEnumerableExtensions
{
    /// <summary>
    /// Creates the cross-product of two enumerables.
    /// </summary>
    /// <typeparam name="TOuter">The starting type.</typeparam>
    /// <typeparam name="TInner">The type to add.</typeparam>
    /// <param name="this">Itself.</param>
    /// <param name="inner">The enumerable to add.</param>
    /// <returns>The composite cross-product.</returns>
    public static IEnumerable<(TOuter, TInner)> CrossJoin<TOuter, TInner>(this IEnumerable<TOuter> @this, IEnumerable<TInner> inner)
    {
        //Joins the two enumerables on every record
        //True always matches true
        return @this.Join(inner,
            outer => true,
            inner => true,
            (outer, inner) => (outer, inner));
    }
}
