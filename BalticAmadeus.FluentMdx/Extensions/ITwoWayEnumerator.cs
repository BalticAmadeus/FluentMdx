using System.Collections.Generic;

namespace BalticAmadeus.FluentMdx.Extensions
{
    internal interface ITwoWayEnumerator<out T> : IEnumerator<T>
    {
        bool MovePrevious();
    }
}