namespace BalticAmadeus.FluentMdx
{
    /// <summary>
    /// Represents a base of all possible Mdx structures-expressions.
    /// </summary>
    public abstract class MdxExpressionBase
    {
        protected abstract string GetStringExpression();

        /// <summary>
        /// Converts the value of the current <see cref="MdxExpressionBase"/> object to its equivalent string representation.
        /// </summary>
        /// <returns>A string representation of the value of the current <see cref="MdxExpressionBase"/> object.</returns>
        public override string ToString()
        {
            return GetStringExpression();
        }
    }
}