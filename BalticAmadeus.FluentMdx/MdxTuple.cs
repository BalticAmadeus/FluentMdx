namespace BalticAmadeus.FluentMdx
{
    public abstract class MdxTuple : IMdxExpression
    {
        public override string ToString()
        {
            return GetStringExpression();
        }

        public abstract string GetStringExpression();
    }
}