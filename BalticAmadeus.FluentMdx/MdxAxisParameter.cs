using System;

namespace BalticAmadeus.FluentMdx
{
    public class MdxAxisParameter : IMdxExpression
    {
        private readonly string _name;

        public MdxAxisParameter(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentNullException("name");

            _name = name;
        }

        public string Name
        {
            get { return _name; }
        }

        public string GetStringExpression()
        {
            return Name;
        }

        public override string ToString()
        {
            return GetStringExpression();
        }

        public override int GetHashCode()
        {
            return Name.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            var axisParameter = obj as MdxAxisParameter;
            if (axisParameter == null)
                return false;

            return Name == axisParameter.Name;
        }
    }
}