using System.Collections.Generic;

namespace BalticAmadeus.FluentMdx.EnumerableExtensions
{
    internal static class StatedIteratorExtensions
    {
        public static IStatedTwoWayEnumerator<T> GetStatedTwoWayEnumerator<T>(this IEnumerable<T> source)
        {
            return new StatedTwoWayEnumerator<T>(source.GetEnumerator());
        }
    }
}