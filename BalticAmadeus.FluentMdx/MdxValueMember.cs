using System;

namespace BalticAmadeus.FluentMdx
{
    public class MdxValueMember : MdxMember
    {
        private readonly string _value;

        public MdxValueMember(string name, string value) : base(name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentNullException("name");
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentNullException("value");

            _value = value;
        }

        public string Value
        {
            get { return _value; }
        }

        public override string GetStringExpression()
        {
            return string.Format("{0}{1}", Name, Value);
        }
    }
}