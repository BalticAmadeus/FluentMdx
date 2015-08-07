using System;
using System.Collections.Generic;
using System.Linq;

namespace BalticAmadeus.FluentMdx
{
    /// <summary>
    /// Represent Mdx query statement.
    /// </summary>
    public sealed class MdxQuery : MdxExpressionBase
    {
        private readonly IList<MdxCube> _cubes;
        private readonly IList<MdxAxis> _axes;
        private readonly IList<MdxTuple> _whereClauseTuples;

        /// <summary>
        /// Initializes a new instance of <see cref="MdxQuery"/>.
        /// </summary>
        public MdxQuery()
        {
            _axes = new List<MdxAxis>();
            _cubes = new List<MdxCube>();
            _whereClauseTuples = new List<MdxTuple>();            
        }

        /// <summary>
        /// Gets the collection of specified <see cref="MdxCube"/>s.
        /// </summary>
        public IEnumerable<MdxCube> Cubes
        {
            get { return _cubes; }
        }

        /// <summary>
        /// Gets the inner <see cref="MdxQuery"/> used as parent source.
        /// </summary>
        public MdxQuery InnerQuery { get; private set; }

        /// <summary>
        /// Gets the collection of specified <see cref="MdxAxis"/>s.
        /// </summary>
        public IEnumerable<MdxAxis> Axes
        {
            get { return _axes; }
        }

        /// <summary>
        /// Gets the collection of specified <see cref="MdxTuple"/>s.
        /// </summary>
        public IEnumerable<MdxTuple> WhereClauseTuples
        {
            get { return _whereClauseTuples; }
        }

        /// <summary>
        /// Appends the specified <see cref="MdxAxis"/> and returns the updated current instance of <see cref="MdxQuery"/>.
        /// </summary>
        /// <param name="axis">Specified <see cref="MdxAxis"/>.</param>
        /// <returns>Returns the updated current instance of <see cref="MdxQuery"/>.</returns>
        public MdxQuery On(MdxAxis axis)
        {
            if (axis == null)
                throw new ArgumentNullException("axis");

            _axes.Add(axis);
            return this;
        }

        /// <summary>
        /// Appends the specified <see cref="MdxAxis"/> and returns the updated current instance of <see cref="MdxQuery"/>.
        /// It will inner query if specified.
        /// </summary>
        /// <param name="cube">Specified <see cref="MdxCube"/> as query source.</param>
        /// <returns>Returns the updated current instance of <see cref="MdxQuery"/>.</returns>
        public MdxQuery From(MdxCube cube)
        {
            InnerQuery = null;

            _cubes.Add(cube);
            return this;
        }

        /// <summary>
        /// Sets the <see cref="MdxQuery"/> as parent query source and returns the updated current instance of <see cref="MdxQuery"/>.
        /// It will clear any cubes if specified.
        /// </summary>
        /// <param name="innerQuery">Specified <see cref="MdxQuery"/> as query source.</param>
        /// <returns>Returns the updated current instance of <see cref="MdxQuery"/>.</returns>
        public MdxQuery From(MdxQuery innerQuery)
        {
            _cubes.Clear();
            
            InnerQuery = innerQuery;
            return this;
        }

        /// <summary>
        /// Appends the <see cref="MdxTuple"/> into where clause and returns the updated current instance of <see cref="MdxQuery"/>.
        /// </summary>
        /// <param name="tuple">Specified <see cref="MdxTuple"/>.</param>
        /// <returns>Returns the updated current instance of <see cref="MdxQuery"/>.</returns>
        public MdxQuery Where(MdxTuple tuple)
        {
            _whereClauseTuples.Add(tuple);
            return this;
        }

        protected override string GetStringExpression()
        {
            if (InnerQuery == null)
            {
                if (!WhereClauseTuples.Any())
                    return string.Format(@"SELECT {0} FROM {1}",
                        string.Join(", ", Axes),
                        string.Join(", ", Cubes));

                return string.Format(@"SELECT {0} FROM {1} WHERE {{ ( {2} ) }}",
                    string.Join(", ", Axes),
                    string.Join(", ", Cubes),
                    string.Join(", ", WhereClauseTuples));
            }

            if (!WhereClauseTuples.Any())
                return string.Format(@"SELECT {0} FROM ( {1} )",
                    string.Join(", ", Axes),
                    InnerQuery);

            return string.Format(@"SELECT {0} FROM ( {1} ) WHERE {{ ( {2} ) }}",
                string.Join(", ", Axes),
                InnerQuery,
                string.Join(", ", WhereClauseTuples));
        }
    }
}