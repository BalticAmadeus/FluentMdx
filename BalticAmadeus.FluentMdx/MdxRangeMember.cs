using System;

namespace BalticAmadeus.FluentMdx
{
    public class MdxRangeMember : MdxMember
    {
        private readonly string _valueFrom;
        private readonly string _valueTo;

        public MdxRangeMember(string name, string valueFrom, string valueTo) : base(name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentNullException("name");
            if (string.IsNullOrWhiteSpace(valueFrom))
                throw new ArgumentNullException("valueFrom");
            if (string.IsNullOrWhiteSpace(valueTo))
                throw new ArgumentNullException("valueTo");

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
            return string.Format(@"{0}{1}:{0}{2}", Name, ValueFrom, ValueTo);
        }
    }
}