using System;
using System.Collections.Generic;
using System.Linq;

namespace BalticAmadeus.FluentMdx
{
    public class MdxQuery : IMdxExpression
    {
        private readonly IList<MdxCube> _cubes;
        private readonly IList<MdxAxis> _axes;
        private MdxTuple _tuple;

        public MdxQuery() : this(new List<MdxAxis>(), new List<MdxCube>()) { }
        
        internal MdxQuery(IList<MdxAxis> axes, IList<MdxCube> cubes, MdxTuple tuple = null)
        {
            if (axes == null)
                throw new ArgumentNullException("axes");
            if (cubes == null)
                throw new ArgumentNullException("cubes");

            _axes = axes;
            _cubes = cubes;
            _tuple = tuple;
        }

        public IEnumerable<MdxCube> Cubes
        {
            get { return _cubes; }
        }

        public IEnumerable<MdxAxis> Axes
        {
            get { return _axes; }
        }

        public MdxTuple Tuple
        {
            get { return _tuple; }
        }

        public MdxQuery On(MdxAxis axis)
        {
            if (axis == null)
                throw new ArgumentNullException("axis");

            _axes.Add(axis);
            return this;
        }

        public MdxQuery WithoutOn(MdxAxis axis)
        {
            if (axis == null)
                throw new ArgumentNullException("axis");

            _axes.Remove(axis);
            return this;
        }

        public MdxQuery From(MdxCube cube)
        {
            if (cube == null)
                throw new ArgumentNullException("cube");

            _cubes.Add(cube);
            return this;
        }

        public MdxQuery WithoutFrom(MdxCube cube)
        {
            if (cube == null)
                throw new ArgumentNullException("cube");

            _cubes.Remove(cube);
            return this;
        }

        public MdxQuery Where(MdxTuple tuple)
        {
            if (tuple == null)
                throw new ArgumentNullException("tuple");

            _tuple = tuple;
            return this;
        }

        public MdxQuery WithoutWhere(MdxTuple tuple)
        {
            if (tuple == null)
                throw new ArgumentNullException("tuple");

            if (Equals(_tuple, tuple))
                _tuple = null;
            return this;
        }

        public string GetStringExpression()
        {
            if (!Axes.Any())
                throw new ArgumentException("There are no axes in query!");
            if (!Cubes.Any())
                throw new ArgumentException("There are no cubes in query!");

            if (Tuple == null)
                return string.Format(@"SELECT {0} FROM {1}",
                    string.Join(", ", Axes),
                    string.Join(", ", Cubes));

            return string.Format(@"SELECT {0} FROM {1} WHERE {2}",
                string.Join(", ", Axes),
                string.Join(", ", Cubes),
                Tuple);
        }

        public override string ToString()
        {
            return GetStringExpression();
        }
    }
}