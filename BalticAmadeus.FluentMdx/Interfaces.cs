namespace BalticAmadeus.FluentMdx
{
    public abstract class MdxExpressionBase
    {
        protected abstract string GetStringExpression();

        public override string ToString()
        {
            return GetStringExpression();
        }
    }

    public interface IMdxMember
    {
    }

    public interface IMdxExpressionOperand
    {
    }

    public interface IMdxParser
    {
        MdxQuery ParseQuery(string source);
    }
}