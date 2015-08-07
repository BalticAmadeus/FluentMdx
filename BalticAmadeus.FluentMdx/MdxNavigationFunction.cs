using System.Collections.Generic;
using System.Linq;

namespace BalticAmadeus.FluentMdx
{
    /// <summary>
    /// Represents Mdx navigation function that can be applied to <see cref="MdxMember"/>.
    /// </summary>
    public sealed class MdxNavigationFunction : MdxExpressionBase
    {
        private readonly IList<string> _functionParameters;

        /// <summary>
        /// Initializes a new instance of <see cref="MdxNavigationFunction"/>.
        /// </summary>
        public MdxNavigationFunction()
        {
            _functionParameters = new List<string>();
        }

        /// <summary>
        /// Gets the title of <see cref="MdxNavigationFunction"/>.
        /// </summary>
        public string Title { get; private set; }

        /// <summary>
        /// Gets the collection of specified function parameters.
        /// </summary>
        public IEnumerable<string> FunctionParameters
        {
            get { return _functionParameters; }
        }

        /// <summary>
        /// Sets the title and returns the updated current instance of <see cref="MdxNavigationFunction"/>.
        /// </summary>
        /// <param name="title">Navigation function title.</param>
        /// <returns>Returns the updated current instance of <see cref="MdxNavigationFunction"/>.</returns>
        public MdxNavigationFunction Titled(string title)
        {
            Title = title;
            return this;
        }

        /// <summary>
        /// Appends the parameters and returns the updated current instance of <see cref="MdxNavigationFunction"/>.
        /// </summary>
        /// <param name="parameters">Collection of navigation function parameters</param>
        /// <returns>Returns the updated current instance of <see cref="MdxNavigationFunction"/>.</returns>
        public MdxNavigationFunction WithParameters(params string[] parameters)
        {
            foreach (var parameter in parameters)
                _functionParameters.Add(parameter);
            
            return this;
        }

        protected override string GetStringExpression()
        {
            if (!FunctionParameters.Any())
                return Title;

            return string.Format("{0}({1})", Title, string.Join(", ", FunctionParameters));
        }
    }
}
