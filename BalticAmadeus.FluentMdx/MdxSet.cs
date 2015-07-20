namespace BalticAmadeus.FluentMdx
{
    public abstract class MdxSet : IMdxExpression
    {
        public abstract string GetStringExpression();

        public override string ToString()
        {
            return GetStringExpression();
        }
    }
}