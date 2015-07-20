using System;

namespace BalticAmadeus.FluentMdx
{
    public abstract class MdxMember : IMdxExpression
    {
        private readonly string _name;

        protected MdxMember(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentNullException("name");

            _name = name;
        }

        public string Name
        {
            get { return _name; }
        }

        public abstract string GetStringExpression();

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
            var member = obj as MdxMember;
            if (member == null)
                return false;

            return Name == member.Name;
        }
    }
}