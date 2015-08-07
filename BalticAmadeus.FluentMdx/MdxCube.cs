using System.Collections.Generic;

namespace BalticAmadeus.FluentMdx
{
    /// <summary>
    /// Represents Mdx cube specification.
    /// </summary>
    public sealed class MdxCube : MdxExpressionBase
    {
        private readonly IList<string> _titles; 

        /// <summary>
        /// Initializes a new instance of <see cref="MdxCube"/>.
        /// </summary>
        public MdxCube() 
        {
            _titles = new List<string>();
        }

        /// <summary>
        /// Gets the collection of cube titles.
        /// </summary>
        public IEnumerable<string> Titles
        {
            get { return _titles; }
        }

        /// <summary>
        /// Appends titles and returns updated current instance of <see cref="MdxCube"/>.
        /// </summary>
        /// <param name="titles">Collection of titles.</param>
        /// <returns>Returns updated current instance of <see cref="MdxCube"/>.</returns>
        public MdxCube Titled(params string[] titles)
        {
            foreach (var title in titles)
                _titles.Add(title);

            return this;
        }

        protected override string GetStringExpression()
        {
            return string.Format("[{0}]", string.Join("].[", Titles));
        }
    }
}