using System.Collections.Generic;

namespace BalticAmadeus.FluentMdx
{
    public class MdxAxisParameter : MdxIdentifier
    {
        public MdxAxisParameter(params string[] titles) 
            : this(titles, new List<MdxFunction>())
        {
        }

        internal MdxAxisParameter(IList<string> identifiers, IList<MdxFunction> appliedFunctions)
            : base(new List<string>(identifiers), appliedFunctions)
        {
        }
    }
}