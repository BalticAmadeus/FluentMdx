using System.Collections.Generic;

namespace BalticAmadeus.FluentMdx
{
    public class MdxMember : MdxIdentifier
    {
        public MdxMember(params string[] titles) 
            : this(titles, new List<MdxFunction>())
        {
        }

        internal MdxMember(IList<string> identifiers, IList<MdxFunction> appliedFunctions) 
            : base(new List<string>(identifiers), appliedFunctions)
        {
        }
    }
}