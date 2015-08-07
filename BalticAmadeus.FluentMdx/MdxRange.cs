namespace BalticAmadeus.FluentMdx
{
    /// <summary>
    /// Represents Mdx members range.
    /// </summary>
    public sealed class MdxRange : MdxExpressionBase, IMdxMember, IMdxExpressionOperand
    {
        /// <summary>
        /// Gets the starting <see cref="MdxMember"/> of range.
        /// </summary>
        public MdxMember FromMember { get; private set; }

        /// <summary>
        /// Gets the ending <see cref="MdxMember"/> of range.
        /// </summary>
        public MdxMember ToMember { get; private set; }

        /// <summary>
        /// Sets the starting range member and returns the updated current instance of <see cref="MdxRange"/>.
        /// </summary>
        /// <param name="member">Starting <see cref="MdxMember"/>.</param>
        /// <returns>Returns the updated current instance of <see cref="MdxRange"/>.</returns>
        public MdxRange From(MdxMember member)
        {
            FromMember = member;

            return this;
        }

        /// <summary>
        /// Sets the ending range member and returns the updated current instance of <see cref="MdxRange"/>.
        /// </summary>
        /// <param name="member">Ending <see cref="MdxMember"/>.</param>
        /// <returns>Returns the updated current instance of <see cref="MdxRange"/>.</returns>
        public MdxRange To(MdxMember member)
        {
            ToMember = member;

            return this;
        }

        protected override string GetStringExpression()
        {
            return string.Format("{0}:{1}", FromMember, ToMember);
        }
    }
}