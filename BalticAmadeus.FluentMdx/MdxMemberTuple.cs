using System;
using System.Collections.Generic;
using System.Linq;

namespace BalticAmadeus.FluentMdx
{
    public class MdxMemberTuple : MdxTuple
    {
        private readonly IList<MdxMember> _members;

        public MdxMemberTuple()
        {
            _members = new List<MdxMember>();
        }

        internal MdxMemberTuple(IList<MdxMember> members)
        {
            if (members == null)
                throw new ArgumentNullException("members");

            _members = members;
        }

        public IEnumerable<MdxMember> Members
        {
            get { return _members; }
        }

        public MdxMemberTuple With(MdxMember member)
        {
            if (member == null)
                throw new ArgumentNullException("member");

            _members.Add(member);
            return this;
        }

        public MdxMemberTuple Without(MdxMember member)
        {
            if (member == null)
                throw new ArgumentNullException("member");

            _members.Remove(member);
            return this;
        }

        public override string GetStringExpression()
        {
            if (!_members.Any())
                throw new ArgumentException("There are no members in tuple!");

            if (_members.Count == 1)
                return _members[0].GetStringExpression();

            return string.Format(@"{{ {0} }}",
                string.Join(", ", Members));
        }
    }
}