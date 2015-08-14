using System.Collections.Generic;

namespace BalticAmadeus.FluentMdx
{
    public class MdxDeclaration : MdxExpressionBase
    {
        private readonly IList<string> _titles;

        public MdxDeclaration()
        {
            _titles = new List<string>();
        }

        public IEnumerable<string> Titles
        {
            get { return _titles; }
        } 

        public IMdxExpression Expression { get; private set; }

        public MdxDeclaration Titled(params string[] titles)
        {
            foreach (var title in titles)
                _titles.Add(title);

            return this;
        }

        public MdxDeclaration As(MdxExpression expression)
        {
            Expression = expression;
            return this;
        }

        public MdxDeclaration As(MdxTuple expression)
        {
            Expression = expression;
            return this;
        }

        protected override string GetStringExpression()
        {
            if (Expression is MdxExpression)
                return string.Format("MEMBER [{0}] AS {1}", string.Join("].[", Titles), Expression);

            return string.Format("SET [{0}] AS {1}", string.Join("].[", Titles), Expression);
        }
    }
}