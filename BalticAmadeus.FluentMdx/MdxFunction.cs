using System.Collections.Generic;

namespace BalticAmadeus.FluentMdx
{
    public class MdxFunction : MdxExpressionBase, IMdxMember, IMdxExpressionOperand
    {
        private readonly IList<string> _titles;
        private readonly IList<MdxExpression> _parameters; 
        
        public MdxFunction()
        {
            _titles = new List<string>();
            _parameters = new List<MdxExpression>();
        }

        public IEnumerable<string> Titles
        {
            get { return _titles; }
        }

        public IEnumerable<MdxExpression> Parameters
        {
            get { return _parameters; }
        }

        public MdxFunction Titled(params string[] titles)
        {
            foreach (var title in titles)
                _titles.Add(title);
                
            return this;
        }

        public MdxFunction WithParameter(MdxExpression parameter)
        {
            _parameters.Add(parameter);
            return this;
        }

        protected override string GetStringExpression()
        {
            return string.Format("{0}({1})", 
                string.Join(".", Titles), 
                string.Join(", ", Parameters));
        }
    }
}
