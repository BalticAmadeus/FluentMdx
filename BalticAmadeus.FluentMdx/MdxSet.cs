using System.Collections.Generic;
using System.Linq;

namespace BalticAmadeus.FluentMdx
{
    /// <summary>
    /// Represents Mdx set.
    /// </summary>
    public sealed class MdxSet : MdxExpressionBase, IMdxMember, IMdxExpressionOperand
    {
        private readonly IList<IMdxMember> _children;

        /// <summary>
        /// Initializes a new instance of <see cref="MdxSet"/>.
        /// </summary>
        public MdxSet()
        {
            _children = new List<IMdxMember>();
        }

        /// <summary>
        /// Gets the collection of <see cref="IMdxMember"/>s as set children.
        /// </summary>
        public IEnumerable<IMdxMember> Children
        {
            get { return _children; }
        }

        /// <summary>
        /// Appends the specified <see cref="MdxMember"/> and returns the updated current instance of <see cref="MdxSet"/>. 
        /// If there are any <see cref="MdxTuple"/>s in <see cref="Children"/> then specified <see cref="MdxMember"/> 
        /// is appended to the last <see cref="MdxTuple"/>.
        /// </summary>
        /// <param name="member">Specified <see cref="MdxMember"/>.</param>
        /// <returns>Returns the updated current instance of <see cref="MdxSet"/>.</returns>
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
        /// Appends the specified <see cref="MdxRange"/> and returns the updated current instance of <see cref="MdxSet"/>. 
        /// If there are any <see cref="MdxTuple"/>s in <see cref="Children"/> then specified <see cref="MdxRange"/> 
        /// is appended to the last <see cref="MdxTuple"/>.
        /// </summary>
        /// <param name="range">Specified <see cref="MdxRange"/>.</param>
        /// <returns>Returns the updated current instance of <see cref="MdxSet"/>.</returns>
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
        /// Appends the specified <see cref="MdxFunction"/> and returns the updated current instance of <see cref="MdxSet"/>. 
        /// If there are any <see cref="MdxTuple"/>s in <see cref="Children"/> then specified <see cref="MdxFunction"/> 
        /// is appended to the last <see cref="MdxTuple"/>.
        /// </summary>
        /// <param name="function">Specified <see cref="MdxFunction"/>.</param>
        /// <returns>Returns the updated current instance of <see cref="MdxSet"/>.</returns>
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
        /// Appends the specified <see cref="MdxTuple"/> and returns the updated current instance of <see cref="MdxSet"/>. 
        /// If there are any children other than <see cref="MdxTuple"/> then each child will be wrapped into <see cref="MdxTuple"/>.
        /// </summary>
        /// <param name="tuple">Specified <see cref="MdxSet"/>.</param>
        /// <returns>Returns the updated current instance of <see cref="MdxTuple"/>.</returns>
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

            var copiedChildren = new List<IMdxMember>(_children);
            _children.Clear();

            foreach (var member in copiedChildren)
                _children.Add(Mdx.Tuple().With(member));
            
            return this;
        }

        /// <summary>
        /// Appends the specified <see cref="MdxSet"/>, but wraps it into <see cref="MdxTuple"/> and returns the updated 
        /// current instance of <see cref="MdxSet"/>. If there are any <see cref="MdxTuple"/>s in <see cref="Children"/> 
        /// then specified <see cref="MdxSet"/> is appended to the last <see cref="MdxTuple"/>.
        /// </summary>
        /// <param name="set">Specified <see cref="MdxSet"/>.</param>
        /// <returns>Returns the updated current instance of <see cref="MdxSet"/>.</returns>
        public MdxSet With(MdxSet set)
        {
            var lastTuple = _children.OfType<MdxTuple>().LastOrDefault();
            if (lastTuple == null)
            {
                With(Mdx.Tuple().With(set));

                return this;
            }

            lastTuple.With(set);

            return this;
        }

        /// <summary>
        /// Appends the specified <see cref="IMdxMember"/> and returns the updated current instance of <see cref="MdxSet"/>.
        /// </summary>
        /// <param name="member">Specified <see cref="IMdxMember"/>.</param>
        /// <returns>Returns the updated current instance of <see cref="MdxSet"/>.</returns>
        public MdxSet With(IMdxMember member)
        {
            if (member is MdxMember)
                return With((MdxMember)member);

            if (member is MdxSet)
                return With((MdxSet)member);

            if (member is MdxRange)
                return With((MdxRange)member);

            if (member is MdxFunction)
                return With((MdxFunction)member);

            if (member is MdxTuple)
                return With((MdxTuple)member);

            return this;
        }

        protected override string GetStringExpression()
        {
            return string.Format("( {0} )", string.Join(", ", Children));
        }
    }
}