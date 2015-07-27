using System;
using System.Collections.Generic;
using System.Linq;

namespace BalticAmadeus.FluentMdx
{
    public class MdxQuery : IMdxExpression
    {
        private readonly IList<MdxCube> _cubes;
        private readonly IList<MdxAxis> _axes;
        private readonly IList<MdxTuple> _whereClauseTuples;

        public MdxQuery() : this(new List<MdxAxis>(), new List<MdxCube>()) { }

        internal MdxQuery(IList<MdxAxis> axes, IList<MdxCube> cubes) : this(axes, cubes, new List<MdxTuple>()) { }

        internal MdxQuery(IList<MdxAxis> axes, IList<MdxCube> cubes, IList<MdxTuple> whereClauseTuples)
        {
            _axes = axes;
            _cubes = cubes;
            _whereClauseTuples = whereClauseTuples;
        }

        public IEnumerable<MdxCube> Cubes
        {
            get { return _cubes; }
        }

        public IEnumerable<MdxAxis> Axes
        {
            get { return _axes; }
        }

        public IEnumerable<MdxTuple> WhereClauseTuples
        {
            get { return _whereClauseTuples; }
        }

        public MdxQuery On(MdxAxis axis)
        {
            if (axis == null)
                throw new ArgumentNullException("axis");

            _axes.Add(axis);
            return this;
        }

        public MdxQuery From(MdxCube cube)
        {
            if (cube == null)
                throw new ArgumentNullException("cube");

            _cubes.Add(cube);
            return this;
        }

        public MdxQuery Where(MdxTuple tuple)
        {
            _whereClauseTuples.Add(tuple);
            return this;
        }

        public string GetStringExpression()
        {
            if (!Axes.Any())
                throw new ArgumentException("There are no axes in query!");
            if (!Cubes.Any())
                throw new ArgumentException("There are no cubes in query!");

            if (!WhereClauseTuples.Any())
                return string.Format(@"SELECT {0} FROM {1}",
                    string.Join(", ", Axes),
                    string.Join(", ", Cubes));

            return string.Format(@"SELECT {0} FROM {1} WHERE {{ ( {2} ) }}",
                string.Join(", ", Axes),
                string.Join(", ", Cubes),
                string.Join(", ", WhereClauseTuples));
        }

        public override string ToString()
        {
            return GetStringExpression();
        }
    }
}