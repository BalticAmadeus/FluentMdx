using System;
using System.Collections.Generic;
using System.Linq;

namespace BalticAmadeus.FluentMdx.EnumerableExtensions
{
    internal class StatedTwoWayEnumerator<T> : IStatedTwoWayEnumerator<T>
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

        public void RestoreState()
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

        public void BackupState()
        {
            _states.Push(new State());
        }

        public void MergeState()
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
        
        public bool MovePrevious()
        {
            if (_index < 0)
                throw new InvalidOperationException();

            --_states.Peek().Displacement;
            --_index;
            return true;
        }

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

        public T Current
        {
            get
            {
                if (_index < 0 || _index >= _buffer.Count)
                    throw new InvalidOperationException();

                return _buffer[_index];
            }
        }

        public void Reset()
        {
            _enumerator.Reset();
            _buffer.Clear();
            _index = -1;
            _states.Clear();
            _states.Push(new State());
        }

        public void Dispose()
        {
            _enumerator.Dispose();
        }

        object System.Collections.IEnumerator.Current
        {
            get { return Current; }
        }
    }
}