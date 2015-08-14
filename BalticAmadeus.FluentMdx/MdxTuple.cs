using System.Collections.Generic;
using System.Linq;

namespace BalticAmadeus.FluentMdx
{
    /// <summary>
    /// Represents Mdx tuple.
    /// </summary>
    public sealed class MdxTuple : MdxExpressionBase, IMdxMember, IMdxExpression
    {
        private readonly IList<IMdxMember> _children;
        
        /// <summary>
        /// Initializes a new instance of <see cref="MdxTuple"/>.
        /// </summary>
        public MdxTuple()
        {
            _children = new List<IMdxMember>();
        }

        /// <summary>
        /// Gets the collection of <see cref="IMdxMember"/>s as tuple children.
        /// </summary>
        public IEnumerable<IMdxMember> Children
        {
            get { return _children; }
        } 

        /// <summary>
        /// Appends the specified <see cref="MdxMember"/> and returns the updated current instance of <see cref="MdxTuple"/>. 
        /// If there are any <see cref="MdxSet"/>s in <see cref="Children"/> then specified <see cref="MdxMember"/> 
        /// is appended to the last <see cref="MdxSet"/>.
        /// </summary>
        /// <param name="member">Specified <see cref="MdxMember"/>.</param>
        /// <returns>Returns the updated current instance of <see cref="MdxTuple"/>.</returns>
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
        /// Appends the specified <see cref="MdxRange"/> and returns the updated current instance of <see cref="MdxTuple"/>. 
        /// If there are any <see cref="MdxSet"/>s in <see cref="Children"/> then specified <see cref="MdxRange"/> 
        /// is appended to the last <see cref="MdxSet"/>.
        /// </summary>
        /// <param name="range">Specified <see cref="MdxRange"/>.</param>
        /// <returns>Returns the updated current instance of <see cref="MdxTuple"/>.</returns>
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
        /// Appends the specified <see cref="MdxFunction"/> and returns the updated current instance of <see cref="MdxTuple"/>. 
        /// If there are any <see cref="MdxSet"/>s in <see cref="Children"/> then specified <see cref="MdxFunction"/> 
        /// is appended to the last <see cref="MdxSet"/>.
        /// </summary>
        /// <param name="function">Specified <see cref="MdxFunction"/>.</param>
        /// <returns>Returns the updated current instance of <see cref="MdxTuple"/>.</returns>
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
        /// Appends the specified <see cref="MdxTuple"/>, but wraps it into <see cref="MdxSet"/> and returns the updated 
        /// current instance of <see cref="MdxTuple"/>. If there are any <see cref="MdxSet"/>s in <see cref="Children"/> 
        /// then specified <see cref="MdxTuple"/> is appended to the last <see cref="MdxSet"/>.
        /// </summary>
        /// <param name="tuple">Specified <see cref="MdxTuple"/>.</param>
        /// <returns>Returns the updated current instance of <see cref="MdxTuple"/>.</returns>
        public MdxTuple With(MdxTuple tuple)
        {
            var lastSet = _children.OfType<MdxSet>().LastOrDefault();
            if (lastSet == null)
            {
                return With(Mdx.Set().With(tuple));
            }

            lastSet.With(tuple);

            return this;
        }

        /// <summary>
        /// Appends the specified <see cref="MdxSet"/> and returns the updated current instance of <see cref="MdxTuple"/>. 
        /// If there are any children other than <see cref="MdxSet"/> then each child will be wrapped into <see cref="MdxSet"/>.
        /// </summary>
        /// <param name="set">Specified <see cref="MdxSet"/>.</param>
        /// <returns>Returns the updated current instance of <see cref="MdxTuple"/>.</returns>
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

            var copiedChildren = new List<IMdxMember>(_children);
            _children.Clear();

            foreach (var member in copiedChildren)
                _children.Add(Mdx.Set().With(member));

            return this;
        }

        /// <summary>
        /// Appends the specified <see cref="IMdxMember"/> and returns the updated current instance of <see cref="MdxTuple"/>.
        /// </summary>
        /// <param name="member">Specified <see cref="IMdxMember"/>.</param>
        /// <returns>Returns the updated current instance of <see cref="MdxTuple"/>.</returns>
        public MdxTuple With(IMdxMember member)
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
            return string.Format("{{ {0} }}", string.Join(", ", Children));
        }
    }
}