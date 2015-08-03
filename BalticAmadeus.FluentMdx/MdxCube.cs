using System.Collections.Generic;

namespace BalticAmadeus.FluentMdx
{
    public class MdxCube : MdxExpressionBase
    {
        private readonly IList<string> _titles; 

        public MdxCube() 
        {
            _titles = new List<string>();
        }

        public IEnumerable<string> Titles
        {
            get { return _titles; }
        }

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