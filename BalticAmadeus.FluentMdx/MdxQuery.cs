using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BalticAmadeus.FluentMdx
{
    /// <summary>
    /// Represent Mdx query statement.
    /// </summary>
    public sealed class MdxQuery : MdxExpressionBase
    {
        private readonly IList<MdxDeclaration> _withDeclarations; 
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
            _withDeclarations = new List<MdxDeclaration>();
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

        public MdxQuery With(MdxDeclaration withDeclaration)
        {
            _withDeclarations.Add(withDeclaration);
            return this;
        }

        protected override string GetStringExpression()
        {
            var queryStringBuilder = new StringBuilder();

            if (_withDeclarations.Any())
                queryStringBuilder.AppendFormat("WITH {0} ", string.Join(" ", _withDeclarations));

            queryStringBuilder.AppendFormat("SELECT {0} ", string.Join(", ", Axes));

            if (InnerQuery == null)
                queryStringBuilder.AppendFormat("FROM {0}", string.Join(", ", Cubes));
            else
                queryStringBuilder.AppendFormat("FROM ( {0} )", InnerQuery);

            if (_whereClauseTuples.Any())
                queryStringBuilder.AppendFormat(" WHERE {{ ( {0} ) }}", string.Join(", ", _whereClauseTuples));

            return queryStringBuilder.ToString();
        }
    }
}