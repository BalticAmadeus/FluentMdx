namespace BalticAmadeus.FluentMdx
{
    /// <summary>
    /// Represents constant value used in Mdx statements.
    /// </summary>
    public sealed class MdxConstantExpression : MdxExpressionBase, IMdxExpressionOperand
    {
        /// <summary>
        /// Sets the value of expression and returns the instance of <see cref="MdxConstantExpression"/>.
        /// </summary>
        /// <param name="value">Value of expression.</param>
        /// <returns>Returns updated current instance of <see cref="MdxConstantExpression"/>.</returns>
        public MdxConstantExpression WithValue(string value)
        {
            Value = value;
            return this;
        }

        /// <summary>
        /// Gets the value of expression.
        /// </summary>
        public string Value { get; private set; }

        protected override string GetStringExpression()
        {
            return Value;
        }
    }
}
