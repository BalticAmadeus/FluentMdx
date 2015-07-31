using System.Collections.Generic;

namespace BalticAmadeus.FluentMdx
{
    public class MdxMember : MdxIdentifier
    {
        public MdxMember(params string[] titles) 
            : this(titles, new List<MdxNavigationFunction>())
        {
        }

        internal MdxMember(IList<string> identifiers, IList<MdxNavigationFunction> appliedFunctions) 
            : base(new List<string>(identifiers), appliedFunctions)
        {
        }
    }
}