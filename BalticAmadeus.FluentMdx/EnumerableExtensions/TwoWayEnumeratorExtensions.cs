using System.Collections.Generic;

namespace BalticAmadeus.FluentMdx.EnumerableExtensions
{
    internal static class TwoWayEnumeratorExtensions
    {
        public static ITwoWayEnumerator<T> GetTwoWayEnumerator<T>(this IEnumerable<T> source)
        {
            return new TwoWayEnumerator<T>(source.GetEnumerator());
        }
    }
}