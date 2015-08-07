using System.Collections.Generic;

namespace BalticAmadeus.FluentMdx.EnumerableExtensions
{
    /// <summary>
    /// Enables two-way state-enabled iteration over generic collection.
    /// </summary>
    /// <typeparam name="T">The type of objects to enumerate.</typeparam>
    internal interface IStatedTwoWayEnumerator<out T> : IEnumerator<T>
    {
        /// <summary>
        /// Advances the enumerator to the previous element of the collection.
        /// </summary>
        /// <returns>
        /// <value>True</value> if the enumerator was successfully advanced to the previous element. 
        /// <value>False</value> if the enumerator is in the beginning of the collection.
        /// </returns>
        bool MovePrevious();

        /// <summary>
        /// Restores the enumerator pointer to the element position the enumerator has been pointing 
        /// to before the position was saved and deletes that record.
        /// </summary>
        void RestoreLastSavedPosition();
        
        /// <summary>
        /// Saves the current position of enumerator.
        /// </summary>
        void SavePosition();
        
        /// <summary>
        /// Removes the last saved position of enumerator.
        /// </summary>
        void RemoveLastSavedState();
    }
}