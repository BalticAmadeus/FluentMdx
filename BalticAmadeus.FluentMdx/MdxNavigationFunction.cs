using System.Collections.Generic;
using System.Linq;

namespace BalticAmadeus.FluentMdx
{
    public class MdxNavigationFunction : MdxExpressionBase
    {
        private readonly IList<string> _functionParameters;

        public MdxNavigationFunction()
        {
            _functionParameters = new List<string>();
        }

        public string Title { get; private set; }

        public IEnumerable<string> FunctionParameters
        {
            get { return _functionParameters; }
        }

        public MdxNavigationFunction Titled(string title)
        {
            Title = title;
            return this;
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
