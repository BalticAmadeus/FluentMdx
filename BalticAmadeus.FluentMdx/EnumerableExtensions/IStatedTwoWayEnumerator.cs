using System.Collections.Generic;

namespace BalticAmadeus.FluentMdx.EnumerableExtensions
{
    internal interface IStatedTwoWayEnumerator<out T> : IEnumerator<T>
    {
        bool MovePrevious();

        void RestoreState();
        void BackupState();
        void MergeState();
    }
}