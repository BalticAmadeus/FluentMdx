using System;
using System.Collections.Generic;
using System.Linq;

namespace BalticAmadeus.FluentMdx
{
    public class MdxSetTuple : MdxTuple
    {
        private readonly IList<MdxSet> _sets; 

        public MdxSetTuple() : this(new List<MdxSet>()) { }

        internal MdxSetTuple(IList<MdxSet> sets)
        {
            if (sets == null)
                throw new ArgumentNullException("sets");

            _sets = sets;
        }

        public IEnumerable<MdxSet> Sets
        {
            get { return _sets; }
        }

        public MdxSetTuple With(MdxSet set)
        {
            if (set == null)
                throw new ArgumentNullException("set");

            _sets.Add(set);
            return this;
        }

        public MdxSetTuple Without(MdxSet set)
        {
            if (set == null)
                throw new ArgumentNullException("set");

            _sets.Remove(set);
            return this;
        }

        public override string GetStringExpression()
        {
            if (!Sets.Any())
                throw new ArgumentException("There are no sets in tuple!");
            
            return string.Format(@"{{ {0} }}",
                string.Join(", ", Sets));
        }
    }
}