using System.Collections.Generic;

namespace BalticAmadeus.FluentMdx
{
    public class MdxValueMember : MdxMember
    {
        private readonly string _value;

        public MdxValueMember(string title, string value) : base(title)
        {
            _value = value;
        }

        internal MdxValueMember(IList<string> identifiers, string value) : base(identifiers, new List<MdxFunction>())
        {
            _value = value;
        }

        public string Value
        {
            get { return _value; }
        }

        public override string GetStringExpression()
        {
            return string.Format("[{0}].&[{1}]", string.Join("].[", Identifiers), Value);
        }
    }
}