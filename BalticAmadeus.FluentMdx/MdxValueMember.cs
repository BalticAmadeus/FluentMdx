using System.Collections.Generic;
using System.Linq;

namespace BalticAmadeus.FluentMdx
{
    public class MdxValueMember : MdxMember
    {
        private readonly string _value;

        public MdxValueMember(string value, params string[] titles) : this(titles, value, new List<MdxNavigationFunction>()) { }

        internal MdxValueMember(IList<string> identifiers, string value, IList<MdxNavigationFunction> appliedFunctions)
            : base(new List<string>(identifiers), appliedFunctions)
        {
            _value = value;
        }

        public string Value
        {
            get { return _value; }
        }

        public override string GetStringExpression()
        {
            if (!AppliedFunctions.Any())
                return string.Format("[{0}].&[{1}]", string.Join("].[", Identifiers), Value);

            return string.Format("[{0}].&[{1}].{2}", string.Join("].[", Identifiers), Value, string.Join(".", AppliedFunctions));
        }
    }
}