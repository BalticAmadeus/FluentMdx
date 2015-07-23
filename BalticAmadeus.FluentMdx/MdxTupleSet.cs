using System;
using System.Collections.Generic;
using System.Linq;

namespace BalticAmadeus.FluentMdx
{
    public class MdxTupleSet : MdxSet
    {
        private readonly List<MdxTuple> _tuples;

        public MdxTupleSet() : this(new List<MdxTuple>()) { }

        internal MdxTupleSet(List<MdxTuple> tuples)
        {
            if (tuples == null)
                throw new ArgumentNullException("tuples");

            _tuples = tuples;
        }

        public IEnumerable<MdxTuple> Tuples
        {
            get { return _tuples; }
        }

        public MdxTupleSet With(MdxTuple tuple)
        {
            if (tuple == null)
                throw new ArgumentNullException("tuple");

            _tuples.Add(tuple);
            return this;
        }

        public MdxTupleSet Without(MdxTuple tuple)
        {
            if (tuple == null)
                throw new ArgumentNullException("tuple");

            _tuples.Remove(tuple);
            return this;
        }

        public override string GetStringExpression()
        {
            if (!Tuples.Any())
                throw new ArgumentException("There are no tuples in set!");

            return string.Format(@"( {0} )",
                string.Join(", ", Tuples));
        }
    }
}