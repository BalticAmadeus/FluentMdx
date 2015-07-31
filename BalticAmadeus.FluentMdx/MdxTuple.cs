namespace BalticAmadeus.FluentMdx
{
    public class MdxTuple : IMdxExpression
    {
        public override string ToString()
        {
            return GetStringExpression();
        }

        public virtual string GetStringExpression()
        {
            return "{ }";
        }
    }
}