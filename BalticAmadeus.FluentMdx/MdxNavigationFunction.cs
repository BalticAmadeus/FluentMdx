using System.Collections.Generic;
using System.Linq;

namespace BalticAmadeus.FluentMdx
{
    public class MdxNavigationFunction : MdxExpressionBase
    {
        private readonly string _title;
        private readonly IList<string> _functionParameters;

        public MdxNavigationFunction(string title) : this(title, new List<string>()) { }

        internal MdxNavigationFunction(string title, IList<string> functionParameters)
        {
            _title = title;
            _functionParameters = functionParameters;
        }

        public string Title
        {
            get { return _title; }
        }

        public IEnumerable<string> FunctionParameters
        {
            get { return _functionParameters; }
        }

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
