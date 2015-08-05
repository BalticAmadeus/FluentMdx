namespace BalticAmadeus.FluentMdx
{
    public class MdxConstantExpression : MdxExpressionBase, IMdxExpressionOperand
    {
        public MdxConstantExpression WithValue(string value)
        {
            Value = value;
            return this;
        }

        public string Value { get; private set; }

        protected override string GetStringExpression()
        {
            return Value;
        }
    }
}
