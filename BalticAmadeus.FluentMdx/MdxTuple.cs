using System.Collections.Generic;
using System.Linq;

namespace BalticAmadeus.FluentMdx
{
    /// <summary>
    /// 
    /// </summary>
    public sealed class MdxTuple : MdxExpressionBase, IMdxMember, IMdxExpressionOperand
    {
        private readonly IList<IMdxMember> _children;
        
        /// <summary>
        /// 
        /// </summary>
        public MdxTuple()
        {
            _children = new List<IMdxMember>();
        }

        /// <summary>
        /// 
        /// </summary>
        public IEnumerable<IMdxMember> Children
        {
            get { return _children; }
        } 

        /// <summary>
        /// 
        /// </summary>
        /// <param name="member"></param>
        /// <returns></returns>
        public MdxTuple With(MdxMember member)
        {
            var lastSet = _children.OfType<MdxSet>().LastOrDefault();
            if (lastSet == null)
            {
                _children.Add(member);

                return this;
            }

            lastSet.With(member);

            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="range"></param>
        /// <returns></returns>
        public MdxTuple With(MdxRange range)
        {
            var lastSet = _children.OfType<MdxSet>().LastOrDefault();
            if (lastSet == null)
            {
                _children.Add(range);

                return this;
            }

            lastSet.With(range);

            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="set"></param>
        /// <returns></returns>
        public MdxTuple With(MdxSet set)
        {
            if (!_children.Any())
            {
                _children.Add(set);

                return this;
            }

            if (_children.OfType<MdxSet>().Any())
            {
                _children.Add(set);

                return this;
            }

            var newSet = Mdx.Set();

            foreach (var member in _children.OfType<MdxMember>())
                newSet.With(member);

            foreach (var function in _children.OfType<MdxFunction>())
                newSet.With(function);

            _children.Clear();

            _children.Add(newSet);
            _children.Add(set);

            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="function"></param>
        /// <returns></returns>
        public MdxTuple With(MdxFunction function)
        {
            var lastSet = _children.OfType<MdxSet>().LastOrDefault();
            if (lastSet == null)
            {
                _children.Add(function);

                return this;
            }

            lastSet.With(function);

            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected override string GetStringExpression()
        {
            return string.Format("{{ {0} }}", string.Join(", ", Children));
        }
    }
}