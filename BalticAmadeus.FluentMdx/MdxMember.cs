using System.Collections.Generic;
using System.Linq;

namespace BalticAmadeus.FluentMdx
{
    public class MdxMember : MdxExpressionBase, IMdxMember
    {
        private readonly IList<string> _titles;
        private readonly IList<MdxNavigationFunction> _navigationFunctions;

        public MdxMember()
        {
            _titles = new List<string>();    
            _navigationFunctions = new List<MdxNavigationFunction>();

            Value = default(string);
        }

        public string Value { get; private set; } 

        public IEnumerable<string> Titles
        {
            get { return _titles; }
        }

        public IEnumerable<MdxNavigationFunction> NavigationFunctions
        {
            get { return _navigationFunctions; }
        }

        public MdxMember Titled(params string[] titles)
        {
            foreach (var title in titles)
                _titles.Add(title);

            return this;
        }

        public MdxMember WithValue(string value)
        {
            Value = value;

            return this;
        }

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