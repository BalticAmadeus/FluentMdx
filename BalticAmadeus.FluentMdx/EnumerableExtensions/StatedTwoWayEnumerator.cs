using System;
using System.Collections.Generic;
using System.Linq;

namespace BalticAmadeus.FluentMdx.EnumerableExtensions
{
    /// <summary>
    /// Represents a two-way state-enabled enumerator.
    /// </summary>
    /// <typeparam name="T">The type of objects to enumerate</typeparam>
    internal struct StatedTwoWayEnumerator<T> : IStatedTwoWayEnumerator<T>
    {
        private class State
        {
            public State()
            {
                Displacement = 0;
            }

            public int Displacement;
        }
        
        private readonly IEnumerator<T> _enumerator;
        private readonly List<T> _buffer;
        private readonly Stack<State> _states; 
        private int _index;

        /// <summary>
        /// Initializes a new instance of <see cref="StatedTwoWayEnumerator{T}"/> extending the specified <see cref="IEnumerator{T}"/>.
        /// </summary>
        /// <param name="enumerator"><see cref="IEnumerator{T}"/> implementation.</param>
        public StatedTwoWayEnumerator(IEnumerator<T> enumerator)
        {
            if (enumerator == null)
                throw new ArgumentNullException("enumerator");

            _enumerator = enumerator;
            _buffer = new List<T>();

            _states = new Stack<State>();
            _states.Push(new State());
            
            _index = -1;
        }

        /// <summary>
        /// Restores the enumerator pointer to the element position the enumerator has been pointing 
        /// to before the position was saved and deletes that record.
        /// </summary>
        public void RestoreLastSavedPosition()
        {
            if (!_states.Any())
                return;

            var lastState = _states.Pop();

            if (!_states.Any())
            {
                _states.Push(lastState);
                return;
            }

            _index = _index - lastState.Displacement;
        }

        /// <summary>
        /// Saves the current position of enumerator.
        /// </summary>
        public void SavePosition()
        {
            _states.Push(new State());
        }

        /// <summary>
        /// Removes the last saved position of enumerator.
        /// </summary>
        public void RemoveLastSavedState()
        {
            if (!_states.Any())
                return;

            var lastState = _states.Pop();

            if (!_states.Any())
            {
                _states.Push(lastState);
                return;
            }

            _states.Peek().Displacement += lastState.Displacement;
        }

        /// <summary>
        /// Advances the enumerator to the previous element of the collection.
        /// </summary>
        /// <returns>
        /// <value>True</value> if the enumerator was successfully advanced to the previous element. 
        /// <value>False</value> if the enumerator is in the beginning of the collection.
        /// </returns>
        public bool MovePrevious()
        {
            if (_index < 0)
                throw new InvalidOperationException();

            --_states.Peek().Displacement;
            --_index;
            return true;
        }

        /// <summary>
        /// Advances the enumerator to the next element of the collection.
        /// </summary>
        /// <returns>
        /// <value>True</value> if the enumerator was successfully advanced to the next element. 
        /// <value>False</value> if the enumerator has passed the end of the collection.
        /// </returns>
        public bool MoveNext()
        {
            if (_index < _buffer.Count - 1)
            {
                ++_states.Peek().Displacement;
                ++_index;
                return true;
            }

            if (_enumerator.MoveNext())
            {
                _buffer.Add(_enumerator.Current);
                ++_states.Peek().Displacement;
                ++_index;
                return true;
            }

            return false;
        }

        /// <summary>
        /// Gets the element in the collection at the current position of the enumerator.
        /// </summary>
        public T Current
        {
            get
            {
                if (_index < 0 || _index >= _buffer.Count)
                    throw new InvalidOperationException();

                return _buffer[_index];
            }
        }

        /// <summary>
        /// Sets the enumerator to its initial position, which is before the first element in the collection.
        /// </summary>
        public void Reset()
        {
            _enumerator.Reset();
            _buffer.Clear();
            _index = -1;
            _states.Clear();
            _states.Push(new State());
        }

        /// <summary>
        /// Clears cached items, states and inner enumerator used by <see cref="StatedTwoWayEnumerator{T}"/>.
        /// </summary>
        public void Dispose()
        {
            _buffer.Clear();
            _states.Clear();

            _enumerator.Dispose();
        }

        object System.Collections.IEnumerator.Current
        {
            get { return Current; }
        }
    }
}