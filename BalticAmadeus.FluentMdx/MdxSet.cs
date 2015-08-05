using System.Collections.Generic;
using System.Linq;

namespace BalticAmadeus.FluentMdx
{
    /// <summary>
    /// 
    /// </summary>
    public sealed class MdxSet : MdxExpressionBase, IMdxMember, IMdxExpressionOperand
    {
        private readonly IList<IMdxMember> _children;

        /// <summary>
        /// 
        /// </summary>
        public MdxSet()
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
        public MdxSet With(MdxMember member)
        {
            var lastTuple = _children.OfType<MdxTuple>().LastOrDefault();
            if (lastTuple == null)
            {
                _children.Add(member);

                return this;
            }

            lastTuple.With(member);

            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="function"></param>
        /// <returns></returns>
        public MdxSet With(MdxFunction function)
        {
            var lastTuple = _children.OfType<MdxTuple>().LastOrDefault();
            if (lastTuple == null)
            {
                _children.Add(function);

                return this;
            }

            lastTuple.With(function);

            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="range"></param>
        /// <returns></returns>
        public MdxSet With(MdxRange range)
        {
            var lastTuple = _children.OfType<MdxTuple>().LastOrDefault();
            if (lastTuple == null)
            {
                _children.Add(range);

                return this;
            }

            lastTuple.With(range);

            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tuple"></param>
        /// <returns></returns>
        public MdxSet With(MdxTuple tuple)
        {
            if (!_children.Any())
            {
                _children.Add(tuple);

                return this;
            }

            if (_children.OfType<MdxTuple>().Any())
            {
                _children.Add(tuple);

                return this;
            }

            var newTuple = Mdx.Tuple();

            foreach (var member in _children.OfType<MdxMember>())
                newTuple.With(member);

            foreach (var function in _children.OfType<MdxFunction>())
                newTuple.With(function);

            _children.Clear();

            _children.Add(newTuple);
            _children.Add(tuple);

            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected override string GetStringExpression()
        {
            return string.Format("( {0} )", string.Join(", ", Children));
        }
    }
}