using System.Collections.Generic;
using System.Linq;

namespace BalticAmadeus.FluentMdx
{
    public class MdxFunction : IMdxExpression
    {
        private readonly string _title;
        private readonly IList<string> _functionParameters;

        public MdxFunction(string title) : this(title, new List<string>()) { }

        internal MdxFunction(string title, IList<string> functionParameters)
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

        public MdxFunction WithParameters(params string[] parameters)
        {
            foreach (var parameter in parameters)
                _functionParameters.Add(parameter);
            
            return this;
        }

        public string GetStringExpression()
        {
            if (!FunctionParameters.Any())
                return Title;

            return string.Format("{0}({1})", Title, string.Join(", ", FunctionParameters));
        }

        public override string ToString()
        {
            return GetStringExpression();
        }
    }
}
