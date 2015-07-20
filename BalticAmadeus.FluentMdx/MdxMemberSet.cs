using System;
using System.Collections.Generic;
using System.Linq;

namespace BalticAmadeus.FluentMdx
{
    public class MdxMemberSet : MdxSet
    {
        private readonly List<MdxMember> _members;

        internal MdxMemberSet(List<MdxMember> members)
        {
            if (members == null)
                throw new ArgumentNullException("members");

            _members = members;
        }

        public MdxMemberSet() : this(new List<MdxMember>()) { }

        public IEnumerable<MdxMember> Members
        {
            get { return _members; }
        }

        public MdxMemberSet With(MdxMember member)
        {
            if (member == null)
                throw new ArgumentNullException("member");

            _members.Add(member);
            return this;
        }

        public MdxMemberSet Without(MdxMember member)
        {
            if (member == null)
                throw new ArgumentNullException("member");

            _members.Remove(member);
            return this;
        }

        public override string GetStringExpression()
        {
            if (!Members.Any())
                throw new ArgumentException("There are no members in set!");

            if (_members.Count == 1)
                return _members[0].GetStringExpression();

            return string.Format(@"( {0} )",
                string.Join(", ", Members));
        }
    }
}