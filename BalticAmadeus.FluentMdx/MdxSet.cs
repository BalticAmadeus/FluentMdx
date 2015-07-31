namespace BalticAmadeus.FluentMdx
{
    public class MdxSet : IMdxExpression
    {
        public virtual string GetStringExpression()
        {
            return "( )";
        }

        public override string ToString()
        {
            return GetStringExpression();
        }
    }
}