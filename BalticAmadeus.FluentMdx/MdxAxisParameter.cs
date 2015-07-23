using System.Collections.Generic;

namespace BalticAmadeus.FluentMdx
{
    public class MdxAxisParameter : MdxIdentifier
    {
        public MdxAxisParameter(string title) : base(title)
        {
        }

        internal MdxAxisParameter(IList<string> identifiers, IList<MdxFunction> appliedFunctions) : base(identifiers, appliedFunctions)
        {
        }
    }
}