using System.Collections.Generic;

namespace BalticAmadeus.FluentMdx.EnumerableExtensions
{
    /// <summary>
    /// Provides extension methods used for <see cref="IStatedTwoWayEnumerator{T}" />.
    /// </summary>
    internal static class StatedTwoWayEnumeratorExtensions
    {
        /// <summary>
        /// Creates an <see cref="IStatedTwoWayEnumerator{T}"/> instance based on <see cref="IEnumerable{T}"/>.
        /// </summary>
        /// <typeparam name="T">The type of objects to enumerate.</typeparam>
        /// <param name="source">A collection used to extract the <see cref="IStatedTwoWayEnumerator{T}"/> from.</param>
        /// <returns>New instance of <see cref="IStatedTwoWayEnumerator{T}"/>.</returns>
        public static IStatedTwoWayEnumerator<T> GetStatedTwoWayEnumerator<T>(this IEnumerable<T> source)
        {
            return new StatedTwoWayEnumerator<T>(source.GetEnumerator());
        }
    }
}