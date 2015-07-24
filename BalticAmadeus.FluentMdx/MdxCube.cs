using System.Collections.Generic;

namespace BalticAmadeus.FluentMdx
{
    public class MdxCube : MdxIdentifier
    {
        public MdxCube(params string[] titles) 
            : this((IList<string>) titles)
        {
        }

        internal MdxCube(IList<string> identifiers) 
            : base(identifiers, new List<MdxFunction>())
        {
        }
        
        public override string GetStringExpression()
        {
            return string.Format("[{0}]", string.Join("].[", Identifiers));
        }
    }
}