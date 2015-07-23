using System.Collections.Generic;

namespace BalticAmadeus.FluentMdx
{
    public class MdxMember : MdxIdentifier
    {
        public MdxMember(string title) : base(title)
        {
        }

        internal MdxMember(IList<string> identifiers, IList<MdxFunction> appliedFunctions) : base(identifiers, appliedFunctions)
        {
        }
    }
}