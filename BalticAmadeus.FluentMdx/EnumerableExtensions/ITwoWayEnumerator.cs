using System.Collections.Generic;

namespace BalticAmadeus.FluentMdx.EnumerableExtensions
{
    internal interface ITwoWayEnumerator<out T> : IEnumerator<T>
    {
        bool MovePrevious();
    }
}