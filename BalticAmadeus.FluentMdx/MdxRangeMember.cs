using System.Collections.Generic;

namespace BalticAmadeus.FluentMdx
{
    public class MdxRangeMember : MdxMember
    {
        private readonly string _valueFrom;
        private readonly string _valueTo;

        public MdxRangeMember(string title, string valueFrom, string valueTo) : base(title)
        {
            _valueFrom = valueFrom;
            _valueTo = valueTo;
        }

        internal MdxRangeMember(IList<string> identifiers, string valueFrom, string valueTo) : base(identifiers, new List<MdxFunction>())
        {
            _valueFrom = valueFrom;
            _valueTo = valueTo;
        }

        public string ValueFrom
        {
            get { return _valueFrom; }
        }

        public string ValueTo
        {
            get { return _valueTo; }
        }

        public override string GetStringExpression()
        {
            return string.Format(@"[{0}].&[{1}]:[{0}].&[{2}]", string.Join("].[", Identifiers), ValueFrom, ValueTo);
        }
    }
}