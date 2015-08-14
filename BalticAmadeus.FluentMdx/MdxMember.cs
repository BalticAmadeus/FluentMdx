using System.Collections.Generic;
using System.Linq;

namespace BalticAmadeus.FluentMdx
{
    /// <summary>
    /// Represents Mdx member.
    /// </summary>
    public sealed class MdxMember : MdxExpressionBase, IMdxMember, IMdxExpression
    {
        private readonly IList<string> _titles;
        private readonly IList<MdxNavigationFunction> _navigationFunctions;

        /// <summary>
        /// Initializes a new instance of <see cref="MdxMember"/>.
        /// </summary>
        public MdxMember()
        {
            _titles = new List<string>();    
            _navigationFunctions = new List<MdxNavigationFunction>();

            Value = default(string);
        }

        /// <summary>
        /// Gets the value of <see cref="MdxMember"/>.
        /// </summary>
        public string Value { get; private set; } 

        /// <summary>
        /// Gets the collection of applied titles.
        /// </summary>
        public IEnumerable<string> Titles
        {
            get { return _titles; }
        }

        /// <summary>
        /// Gets the collection of applied navigation functions.
        /// </summary>
        public IEnumerable<MdxNavigationFunction> NavigationFunctions
        {
            get { return _navigationFunctions; }
        }

        /// <summary>
        /// Appends titles and returns the updated current instance of <see cref="MdxMember"/>. 
        /// </summary>
        /// <param name="titles">Collection of titles.</param>
        /// <returns>Returns the updated current instance of <see cref="MdxMember"/>.</returns>
        public MdxMember Titled(params string[] titles)
        {
            foreach (var title in titles)
                _titles.Add(title);

            return this;
        }

        /// <summary>
        /// Sets the value for member and returns the updated current instance of <see cref="MdxMember"/>. 
        /// </summary>
        /// <param name="value">Specified member value.</param>
        /// <returns>Returns the updated current instance of <see cref="MdxMember"/>.</returns>
        public MdxMember WithValue(string value)
        {
            Value = value;

            return this;
        }

        /// <summary>
        /// Appends the specified navigation function and returns the updated current instance of <see cref="MdxMember"/>. 
        /// </summary>
        /// <param name="function">Specified navigation function.</param>
        /// <returns>Returns the updated current instance of <see cref="MdxMember"/>.</returns>
        public MdxMember WithFunction(MdxNavigationFunction function)
        {
            _navigationFunctions.Add(function);

            return this;
        }

        protected override string GetStringExpression()
        {
            if (string.IsNullOrWhiteSpace(Value) && !_navigationFunctions.Any())
                return string.Format("[{0}]", string.Join("].[", Titles));

            if (string.IsNullOrWhiteSpace(Value))
                return string.Format("[{0}].{1}", string.Join("].[", Titles), string.Join(".", NavigationFunctions));

            if (!_navigationFunctions.Any())
                return string.Format("[{0}].&[{1}]", string.Join("].[", Titles), Value);

            return string.Format("[{0}].&[{1}].{2}", string.Join("].[", Titles), Value, string.Join(".", NavigationFunctions));
        }
    }
}